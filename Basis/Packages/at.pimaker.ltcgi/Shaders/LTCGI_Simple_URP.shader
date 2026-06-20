Shader "LTCGI/Simple URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Roughness ("Roughness", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "LTCGI" = "ALWAYS"
        }
        LOD 100

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/at.pimaker.ltcgi/Shaders/LTCGI_URP_Compat.hlsl"
            #include "Packages/at.pimaker.ltcgi/Shaders/LTCGI.cginc"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Roughness;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv0        : TEXCOORD0;
                float2 uv1        : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 uv         : TEXCOORD0; // xy=main, zw=lightmap UV
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float  fogCoord   : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vp = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs vn = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vp.positionCS;
                output.positionWS = vp.positionWS;
                output.normalWS   = vn.normalWS;
                output.uv.xy      = TRANSFORM_TEX(input.uv0, _MainTex);
                output.uv.zw      = input.uv1;
                output.fogCoord   = ComputeFogFactor(vp.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy) * _Color;

                half3 diff = 0;
                half3 spec = 0;
                LTCGI_Contribution(
                    input.positionWS,
                    normalize(input.normalWS),
                    normalize(LTCGI_GetCameraPosWS() - input.positionWS),
                    _Roughness,
                    input.uv.zw,
                    diff,
                    spec
                );

                col.rgb *= saturate(diff + 0.1);
                col.rgb += spec;

                col.rgb = MixFog(col.rgb, input.fogCoord);
                // Force opaque alpha. This is an Opaque-queue surface, but col.a is just
                // the albedo texture's alpha (often <1: packed AO/smoothness or unused),
                // and hosts that composite the world on the camera-colour alpha (Basis
                // mirrors / desktop window) render an alpha<1 opaque surface as see-through.
                return half4(col.rgb, 1.0);
            }
            ENDHLSL
        }

        // DepthOnly — writes this surface into the camera depth prepass. Without it,
        // under URP depth-priming the surface is absent from _CameraDepthTexture, so
        // screen-space effects that sample scene depth (e.g. VRSL realtime lights) read
        // the geometry *behind* it and the surface appears to "see through" where lit.
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct DAttributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct DVaryings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            DVaryings DepthOnlyVertex(DAttributes input)
            {
                DVaryings output = (DVaryings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 DepthOnlyFragment(DVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return 0;
            }
            ENDHLSL
        }

        // DepthNormals — contributes the authored surface normal to the URP normals prepass
        // (_CameraNormalsTexture) and VRSL's normals prepass (_VRSLNormalsTexture), so VRSL
        // realtime lights use the real surface normal instead of the depth-derivative
        // fallback (faceted/unreliable on large flat surfaces). World-space normal output
        // matches the VRSL deferred shader, which normalizes _VRSLNormalsTexture directly.
        // Stereo-safe (SPSI) — macros mirror LTCGI_Surface_v2_URP's DepthNormals pass.
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct NAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct NVaryings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            NVaryings DepthNormalsVertex(NAttributes input)
            {
                NVaryings output = (NVaryings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS   = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 DepthNormalsFragment(NVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return half4(normalize(input.normalWS), 0.0);
            }
            ENDHLSL
        }
    }
}
