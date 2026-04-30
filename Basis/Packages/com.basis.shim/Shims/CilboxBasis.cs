using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;

namespace Cilbox
{
	public abstract class CilboxBasis : Cilbox
	{
		public static readonly HashSet<String> systemTypeWhitelist = new HashSet<String>(){
			"System.Action",
			"System.Array",
			"System.BitConverter", // HMMMMMMMMM SUSSY
			"System.Boolean",
			"System.Buffer",
			"System.Byte",
			"System.SByte",
			"System.Char",
			"System.Collections.Generic.*",
			"System.Collections.IEnumerable",
			"System.Collections.IEnumerator",
			"System.Convert", // HMMMMMMMMM SUSSY
			"System.DateTime",
			"System.DateTimeKind",
			"System.DateTimeOffset",
			"System.DayOfWeek",
			"System.Decimal",
			"System.Delegate",
			"System.Diagnostics.Stopwatch",
			"System.Double",
			"System.Enum",
			"System.EventArgs",
			"System.Exception",
			"System.Float",
			"System.Func",
			"System.Globalization.*",
			"System.Guid",
			"System.IComparable",
			"System.IDisposable",
			"System.IEquatable",
			"System.IFormatProvider",
			"System.IFormattable",
			"System.Int*",
			"System.IO.BinaryReader",
			"System.IO.BinaryWriter",
			"System.IO.MemoryStream",
			"System.IO.Stream",
			"System.IO.SeekOrigin",
			"System.KeyValuePair",
			"System.Long",
			"System.ULong",
			"System.Math",
			"System.MathF",
			"System.Nullable",
			"System.Object",
			"System.Predicate",
			"System.Random",
			"System.RuntimeTypeHandle",
			"System.Short",
			"System.Ushort",
			"System.Single",
			"System.String",
			"System.StringComparer",
			"System.StringComparison",
			"System.StringSplitOptions",
			"System.Text.NormalizationForm",
			"System.Text.StringBuilder",
			"System.Text.Encoding",
			"System.TimeSpan",
			"System.TimeZoneInfo",
			"System.Tuple",
			"System.UInt*",
			"System.ValueTuple",
			"System.ValueType",
			"System.Void",
			"<PrivateImplementationDetails>", // Probably remove me? But we need a way to handle string hashing.  We can do it with our own function but that's slower.
		};

		public static readonly HashSet<String> unityTypeWhitelist = new HashSet<String>(){
			// Unity
			"UnityEngine.Application", // Restrictive, see method whitelist.
			"UnityEngine.Behaviour",
			"UnityEngine.Color",
			"UnityEngine.Color32",
			"UnityEngine.Component",

			"UnityEngine.Application",
			"UnityEngine.CullingGroup",
			"UnityEngine.CustomRenderTexture",
			"UnityEngine.DynamicGI",
			"UnityEngine.Events.UnityAction",
			"UnityEngine.Events.UnityEvent",
			"UnityEngine.Events.UnityEventCallState",
			"UnityEngine.GameObject",     // Hyper restrictive.
			"UnityEngine.Gradient",
			"UnityEngine.GradientAlphaKey",
			"UnityEngine.GradientColorKey",
			"UnityEngine.GradientMode",
			"UnityEngine.HideFlags",
			"UnityEngine.KeyCode",
			"UnityEngine.LayerMask",
			"UnityEngine.Mathf",
			"UnityEngine.Matrix4x4",
			"UnityEngine.MonoBehaviour",   // Note this is needed for the 'ctor, but we can be very restrictive.
			"UnityEngine.Object",
			"UnityEngine.PrimitiveType",
			"UnityEngine.QualitySettings",
			"UnityEngine.Random",
			"UnityEngine.RuntimePlatform",
			"UnityEngine.RuntimePlatform*",
			"UnityEngine.Screen",
			"UnityEngine.ScriptableObject",
			"UnityEngine.SendMessageOptions",
			"UnityEngine.Space",
			"UnityEngine.SystemInfo",
			"UnityEngine.SystemLanguage",
			"UnityEngine.TextAsset",
			"UnityEngine.Time",
			"UnityEngine.Transform",
			"UnityEngine.Quaternion",
			"UnityEngine.Vector*",
			"UnityEngine.Vector2",
			"UnityEngine.Vector2Int",
			"UnityEngine.Vector3",
			"UnityEngine.Vector3Int",
			"UnityEngine.Vector4",

			// Unity Math / Spatial structs
			"UnityEngine.Bounds",
			"UnityEngine.BoundsInt",
			"UnityEngine.Plane",
			"UnityEngine.Ray",
			"UnityEngine.RaycastHit",
			"UnityEngine.RaycastHit2D",
			"UnityEngine.Rect",
			"UnityEngine.RectInt",
			"UnityEngine.RectOffset",
			"UnityEngine.Resolution",

			// Unity Audio
			"UnityEngine.AudioClip",
			"UnityEngine.AudioClipLoadType",
			"UnityEngine.AudioDataLoadState",
			"UnityEngine.AudioRolloffMode",
			"UnityEngine.AudioSource",
			"UnityEngine.AudioSourceCurveType",
			"UnityEngine.AudioVelocityUpdateMode",
			"UnityEngine.AudioReverbZone",
			"UnityEngine.AudioReverbPreset",
			"UnityEngine.AudioListener",
			"UnityEngine.Audio.AudioMixer",
			"UnityEngine.Audio.AudioMixerGroup",
			"UnityEngine.Audio.AudioMixerSnapshot",
			"UnityEngine.FFTWindow",

			// Unity Animation
			"UnityEngine.AnimationBlendMode",
			"UnityEngine.AnimationClip",
			"UnityEngine.AnimationCullingType",
			"UnityEngine.AnimationCurve",
			"UnityEngine.AnimationEvent",
			"UnityEngine.AnimationPlayMode",
			"UnityEngine.AnimationState",
			"UnityEngine.Animator",
			"UnityEngine.AnimatorClipInfo",
			"UnityEngine.AnimatorControllerParameter",
			"UnityEngine.AnimatorControllerParameterType",
			"UnityEngine.AnimatorCullingMode",
			"UnityEngine.AnimatorOverrideController",
			"UnityEngine.AnimatorRecorderMode",
			"UnityEngine.AnimatorStateInfo",
			"UnityEngine.AnimatorTransitionInfo",
			"UnityEngine.AnimatorUpdateMode",
			"UnityEngine.Avatar",
			"UnityEngine.AvatarIKGoal",
			"UnityEngine.AvatarIKHint",
			"UnityEngine.AvatarMask",
			"UnityEngine.AvatarMaskBodyPart",
			"UnityEngine.AvatarTarget",
			"UnityEngine.HumanBodyBones",
			"UnityEngine.HumanBone",
			"UnityEngine.HumanLimit",
			"UnityEngine.HumanPose",
			"UnityEngine.HumanPoseHandler",
			"UnityEngine.HumanTrait",
			"UnityEngine.Keyframe",
			"UnityEngine.MatchTargetWeightMask",
			"UnityEngine.PlayMode",
			"UnityEngine.QueueMode",
			"UnityEngine.RuntimeAnimatorController",
			"UnityEngine.SkeletonBone",
			"UnityEngine.WeightedMode",
			"UnityEngine.WrapMode",

			// Unity Animations / Constraints
			"UnityEngine.Animations.AimConstraint",
			"UnityEngine.Animations.AimConstraint+WorldUpType",
			"UnityEngine.Animations.Axis",
			"UnityEngine.Animations.ConstraintSource",
			"UnityEngine.Animations.IConstraint",
			"UnityEngine.Animations.LookAtConstraint",
			"UnityEngine.Animations.ParentConstraint",
			"UnityEngine.Animations.PositionConstraint",
			"UnityEngine.Animations.RotationConstraint",
			"UnityEngine.Animations.ScaleConstraint",

			// Unity Rendering / Materials / Mesh
			"UnityEngine.BillboardRenderer",
			"UnityEngine.BoneWeight",
			"UnityEngine.IndexFormat",
			"UnityEngine.Material",
			"UnityEngine.MaterialGlobalIlluminationFlags",
			"UnityEngine.MaterialPropertyBlock",
			"UnityEngine.Mesh",
			"UnityEngine.MeshFilter",
			"UnityEngine.MeshRenderer",
			"UnityEngine.MeshTopology",
			"UnityEngine.MotionVectorGenerationMode",
			"UnityEngine.LineAlignment",
			"UnityEngine.LineRenderer",
			"UnityEngine.LineTextureMode",
			"UnityEngine.Graphics",
			"UnityEngine.LODGroup",
			"UnityEngine.OcclusionArea",
			"UnityEngine.OcclusionPortal",
			"UnityEngine.ReflectionProbe",
			"UnityEngine.Renderer",
			"UnityEngine.Rendering.AmbientMode",
			"UnityEngine.Rendering.AsyncGPUReadback",
			"UnityEngine.Rendering.AsyncGPUReadbackRequest",
			"UnityEngine.Rendering.CameraEvent",
			"UnityEngine.Rendering.IndexFormat",
			"UnityEngine.Rendering.LightEvent",
			"UnityEngine.Rendering.LightProbeUsage",
			"UnityEngine.Rendering.LightShadowResolution",
			"UnityEngine.Rendering.OpaqueSortMode",
			"UnityEngine.Rendering.ReflectionProbeBlendInfo",
			"UnityEngine.Rendering.ReflectionProbeUsage",
			"UnityEngine.Rendering.ShadowCastingMode",
			"UnityEngine.Rendering.ShadowMapPass",
			"UnityEngine.Rendering.UVChannelFlags",
			"UnityEngine.Rendering.SphericalHarmonicsL2",
			"UnityEngine.RenderSettings",
			"UnityEngine.RenderTexture",
			"UnityEngine.RenderTextureFormat",
			"UnityEngine.RenderTextureReadWrite",
			"UnityEngine.Shader",
			"UnityEngine.ShadowCastingMode",
			"UnityEngine.SkinnedMeshRenderer",
			"UnityEngine.SkinQuality",
			"UnityEngine.Sprite",
			"UnityEngine.SpriteAlignment",
			"UnityEngine.SpriteDrawMode",
			"UnityEngine.SpriteMaskInteraction",
			"UnityEngine.SpriteMeshType",
			"UnityEngine.SpriteRenderer",
			"UnityEngine.SpriteSortPoint",
			"UnityEngine.SpriteTileMode",
			"UnityEngine.Texture",
			"UnityEngine.Texture2D",
			"UnityEngine.Texture2DArray",
			"UnityEngine.Texture3D",
			"UnityEngine.Cubemap",
			"UnityEngine.TextureFormat",
			"UnityEngine.TextureWrapMode",
			"UnityEngine.FilterMode",
			"UnityEngine.TrailRenderer",

			// Unity Collections
			"Unity.Collections.NativeArray*",

			// Unity Lighting
			"UnityEngine.Light",
			"UnityEngine.LightShadowCasterMode",
			"UnityEngine.LightShadows",
			"UnityEngine.LightType",
			"UnityEngine.LightRenderMode",
			"UnityEngine.LightProbeProxyVolume",
			"UnityEngine.LightmapBakeType",
			"UnityEngine.LightProbes",
			"UnityEngine.MixedLightingMode",
			"UnityEngine.ShadowQuality",
			"UnityEngine.ShadowResolution",
			"UnityEngine.ShadowProjection",
			"UnityEngine.ShadowmaskMode",

			// Unity Camera (with method whitelist for safety)
			"UnityEngine.Camera",
			"UnityEngine.Camera+CameraCallback",
			"UnityEngine.Camera+GateFitMode",
			"UnityEngine.Camera+MonoOrStereoscopicEye",
			"UnityEngine.Camera+StereoscopicEye",
			"UnityEngine.CameraClearFlags",
			"UnityEngine.CameraType",
			"UnityEngine.DepthTextureMode",
			"UnityEngine.RenderingPath",
			"UnityEngine.StereoTargetEyeMask",
			"UnityEngine.TransparencySortMode",
			"UnityEngine.FogMode",
			"UnityEngine.ColorSpace",

			// Unity Physics
			"UnityEngine.BoxCollider",
			"UnityEngine.CapsuleCollider",
			"UnityEngine.CharacterController",
			"UnityEngine.Collider",
			"UnityEngine.Collision",
			"UnityEngine.CollisionDetectionMode",
			"UnityEngine.ConfigurableJoint",
			"UnityEngine.ContactPoint",
			"UnityEngine.FixedJoint",
			"UnityEngine.ForceMode",
			"UnityEngine.HingeJoint",
			"UnityEngine.Joint",
			"UnityEngine.JointAngleLimits2D",
			"UnityEngine.JointDrive",
			"UnityEngine.JointLimits",
			"UnityEngine.JointMotor",
			"UnityEngine.JointProjectionMode",
			"UnityEngine.JointSpring",
			"UnityEngine.MeshCollider",
			"UnityEngine.MeshColliderCookingOptions",
			"UnityEngine.PhysicMaterial",
			"UnityEngine.PhysicMaterialCombine",
			"UnityEngine.Physics",
			"UnityEngine.QueryTriggerInteraction",
			"UnityEngine.Rigidbody",
			"UnityEngine.RigidbodyConstraints",
			"UnityEngine.RigidbodyInterpolation",
			"UnityEngine.SphereCollider",
			"UnityEngine.SoftJointLimit",
			"UnityEngine.SoftJointLimitSpring",
			"UnityEngine.SpringJoint",
			"UnityEngine.WheelCollider",
			"UnityEngine.WheelFrictionCurve",
			"UnityEngine.WheelHit",

			// Unity Particles
			"UnityEngine.ParticleSystem",
			"UnityEngine.ParticleSystem+*",
			"UnityEngine.ParticleSystemRenderer",
			"UnityEngine.ParticleSystemForceField",
			"UnityEngine.ParticleSystemSimulationSpace",
			"UnityEngine.ParticleSystemShapeType",
			"UnityEngine.ParticleSystemSortMode",
			"UnityEngine.ParticleSystemRenderMode",
			"UnityEngine.ParticleSystemStopBehavior",
			"UnityEngine.ParticleSystemEmissionType",

			// Unity AI NavMesh
			"UnityEngine.AI.NavMesh",
			"UnityEngine.AI.NavMeshAgent",
			"UnityEngine.AI.NavMeshBuildSettings",
			"UnityEngine.AI.NavMeshHit",
			"UnityEngine.AI.NavMeshObstacle",
			"UnityEngine.AI.NavMeshObstacleShape",
			"UnityEngine.AI.NavMeshPath",
			"UnityEngine.AI.NavMeshPathStatus",
			"UnityEngine.AI.NavMeshQueryFilter",
			"UnityEngine.AI.NavMeshTriangulation",
			"UnityEngine.AI.OffMeshLink",
			"UnityEngine.AI.OffMeshLinkData",
			"UnityEngine.AI.ObstacleAvoidanceType",

			// Unity UI
			"UnityEngine.Canvas",
			"UnityEngine.CanvasGroup",
			"UnityEngine.CanvasRenderer",
			"UnityEngine.RectTransform",
			"UnityEngine.RectTransform+Axis",
			"UnityEngine.RectTransform+Edge",
			"UnityEngine.RenderMode",
			"UnityEngine.TextAnchor",
			"UnityEngine.FontStyle",
			"UnityEngine.HorizontalWrapMode",
			"UnityEngine.VerticalWrapMode",
			"UnityEngine.UI.*",

			// Unity Event Systems
			"UnityEngine.EventSystems.AxisEventData",
			"UnityEngine.EventSystems.BaseEventData",
			"UnityEngine.EventSystems.EventTrigger",
			"UnityEngine.EventSystems.EventTrigger+Entry",
			"UnityEngine.EventSystems.EventTrigger+TriggerEvent",
			"UnityEngine.EventSystems.EventTriggerType",
			"UnityEngine.EventSystems.PointerEventData",
			"UnityEngine.EventSystems.PointerEventData+InputButton",
			"UnityEngine.EventSystems.PointerEventData+FramePressState",
			"UnityEngine.EventSystems.RaycastResult",
		};

		public static readonly HashSet<String> basisTypeWhitelist = new HashSet<String>(){
			// Basis types
			"Basis.Scripts.BasisSdk.Interactions.BasisPickUpUseMode",
			"Basis.Scripts.Device_Management.Devices.BasisInput", // Restrictive, only used as a type.
			"Basis.Scripts.BasisSdk.Interactions.BasisPickupInteractable", // Restrictive (See below), only access field.
			"Basis.Scripts.BasisSdk.Interactions.BasisInteractableObject", // Restrictive (See below), only access field.
			"Basis.BasisNetworkBehaviour",
			"Basis.Network.Core.DeliveryMethod",
			"Basis.SafeUtil",
			"Basis.Scripts.BasisSdk.Players.BasisLocalPlayer",
			"Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer",
			"Basis.Shims.*",
            "Basis.BasisImageDownloader",
			"Basis.IBasisImageDownload",
			"Basis.Shims.BasisNet*", // Restrictive, only used as a type and for events.
			"Basis.Shims.BasisAvatarShim",
			"Basis.Shims.BasisAvatarShim+OnReady",
			"Basis.Shims.BasisAvatarShim+AvatarReadyEvent",
			"Basis.Shims.BasisCilboxInstantiateShim", // Restrictive, only used as a type and for Instantiate methods.
			"Basis.Shims.BasisDebugPropsShim", // Restrictive, only used as a type and for logging methods.

			// Cilbox types
			"Cilbox.CilboxPublicUtils",

			// TMPro types
			"TMPro.*",
		};



		public static readonly HashSet<String> systemFieldWhitelist = new HashSet<String>(){
			"System.Array.*",
			"System.String.*",
			"System.DateTime.*",
			"System.TimeSpan.*",
			"System.Guid.*",
			"System.Collections.Generic.KeyValuePair*",
			"System.KeyValuePair*",
		};

		public static readonly HashSet<String> unityFieldWhitelist = new HashSet<String>(){
			// Unity Vector / Quaternion math fields
			"UnityEngine.Vector*.x",
			"UnityEngine.Vector*.y",
			"UnityEngine.Vector*.z",
			"UnityEngine.Vector*.w",
			"UnityEngine.Quaternion*",

			// Unity Color fields (raw r/g/b/a access for both Color and Color32)
			"UnityEngine.Color.r",
			"UnityEngine.Color.g",
			"UnityEngine.Color.b",
			"UnityEngine.Color.a",
			"UnityEngine.Color32.r",
			"UnityEngine.Color32.g",
			"UnityEngine.Color32.b",
			"UnityEngine.Color32.a",

			// Unity math/spatial struct fields
			"UnityEngine.Bounds.*",
			"UnityEngine.BoundsInt.*",
			"UnityEngine.Plane.*",
			"UnityEngine.Ray.*",
			"UnityEngine.RaycastHit.*",
			"UnityEngine.Rect.*",
			"UnityEngine.RectInt.*",
			"UnityEngine.Resolution.*",
			"UnityEngine.Matrix4x4.m*", // m00..m33 entries
			"UnityEngine.Keyframe.*",
			"UnityEngine.GradientAlphaKey.*",
			"UnityEngine.GradientColorKey.*",
			"UnityEngine.AnimatorClipInfo.*",
			"UnityEngine.AnimatorControllerParameter.*",
			"UnityEngine.HumanBone.*",
			"UnityEngine.HumanLimit.*",
			"UnityEngine.SkeletonBone.*",
			"UnityEngine.Animations.ConstraintSource.*",

			// Unity physics struct fields
			"UnityEngine.ContactPoint.*",
			"UnityEngine.JointAngleLimits2D.*",
			"UnityEngine.JointDrive.*",
			"UnityEngine.JointLimits.*",
			"UnityEngine.JointMotor.*",
			"UnityEngine.JointSpring.*",
			"UnityEngine.SoftJointLimit.*",
			"UnityEngine.SoftJointLimitSpring.*",
		};

		public static readonly HashSet<String> basisFieldWhitelist = new HashSet<String>(){
			"Basis.Scripts.BasisSdk.Interactions.BasisPickupInteractable.OnPickupUse",
            "Basis.Scripts.BasisSdk.Interactions.BasisInteractableObject.OnInteractStartEvent",
            "Basis.Scripts.BasisSdk.Interactions.BasisInteractableObject.OnInteractEndEvent",
			"Basis.BasisNetworkBehaviour.CurrentOwnerId",
			"Basis.BasisNetworkBehaviour.IsOwnedLocallyOnServer",
            "Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer.playerId",
        };

		// Whitelist methods on native types.
		// If a type is not in this dictionary, then all methods are allowed.
		public static readonly Dictionary<Type, HashSet<string>> methodWhitelist = new Dictionary<Type, HashSet<string>>()
		{
			{ typeof(UnityEngine.MonoBehaviour), new HashSet<string>{ ".ctor" } },
			{ typeof(UnityEngine.ScriptableObject), new HashSet<string>{ ".ctor" } },
			{ typeof(UnityEngine.Component), new HashSet<string>{ ".ctor" } },
			{ typeof(UnityEngine.Animator), new HashSet<string>{ ".ctor" } },
			{ typeof(UnityEngine.Events.UnityAction), new HashSet<string>{ ".ctor" } },
			{ typeof(Basis.Scripts.BasisSdk.Interactions.BasisPickupInteractable), new HashSet<string> { } },
			{ typeof(Basis.Scripts.BasisSdk.Interactions.BasisInteractableObject), new HashSet<string> { } },
			{ typeof(Basis.Scripts.Device_Management.Devices.BasisInput), new HashSet<string> { } },
			{ typeof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer), new HashSet<string> {
				typeof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer).GetProperty(nameof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer.playerId)).GetGetMethod().Name,
				typeof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer).GetProperty(nameof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer.Player)).GetGetMethod().Name,
				typeof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer).GetProperty(nameof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer.LocalPlayer)).GetGetMethod().Name,
				typeof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer).GetProperty(nameof(Basis.Scripts.Networking.NetworkedAvatar.BasisNetworkPlayer.displayName)).GetGetMethod().Name,
			} },
			{ typeof(UnityEngine.GameObject),          new HashSet<string>{
				nameof(UnityEngine.GameObject.SetActive),
				nameof(UnityEngine.GameObject.GetComponents),
				typeof(UnityEngine.GameObject).GetProperty(nameof(UnityEngine.GameObject.transform)).GetGetMethod().Name,
				typeof(UnityEngine.GameObject).GetProperty(nameof(UnityEngine.GameObject.activeSelf)).GetGetMethod().Name,
				typeof(UnityEngine.GameObject).GetProperty(nameof(UnityEngine.GameObject.activeInHierarchy)).GetGetMethod().Name,
				typeof(UnityEngine.GameObject).GetProperty(nameof(UnityEngine.GameObject.layer)).GetGetMethod().Name,
			} },
			{ typeof(UnityEngine.Graphics), new HashSet<string>{
				nameof(UnityEngine.Graphics.Blit),
			} },
			{ typeof(UnityEngine.Rendering.AsyncGPUReadback), new HashSet<string>{
				nameof(UnityEngine.Rendering.AsyncGPUReadback.Request),
			} },
			{ typeof(Buffer), new HashSet<string>{
				nameof(Buffer.BlockCopy),
			} },
			{ typeof(BitConverter), new HashSet<string>{
				nameof(BitConverter.GetBytes),
				nameof(BitConverter.ToBoolean),
				nameof(BitConverter.ToChar),
				nameof(BitConverter.ToDouble),
				nameof(BitConverter.ToInt16),
				nameof(BitConverter.ToInt32),
				nameof(BitConverter.ToInt64),
				nameof(BitConverter.ToSingle),
				nameof(BitConverter.ToString),
				nameof(BitConverter.ToUInt16),
				nameof(BitConverter.ToUInt32),
				nameof(BitConverter.ToUInt64),
				nameof(BitConverter.DoubleToInt64Bits),
				nameof(BitConverter.Int64BitsToDouble),
				nameof(BitConverter.SingleToInt32Bits),
				nameof(BitConverter.Int32BitsToSingle),
			} },
			{ typeof(Convert), new HashSet<string>{
				nameof(Convert.ToInt16),
				nameof(Convert.ToInt32),
				nameof(Convert.ToInt64),
				nameof(Convert.ToUInt16),
				nameof(Convert.ToUInt32),
				nameof(Convert.ToUInt64),
				nameof(Convert.ToByte),
				nameof(Convert.ToSByte),
				nameof(Convert.ToBoolean),
				nameof(Convert.ToChar),
				nameof(Convert.ToSingle),
				nameof(Convert.ToDouble),
				nameof(Convert.ToString),
				nameof(Convert.ToBase64String),
				nameof(Convert.FromBase64String),
				nameof(Convert.ToDateTime),
				nameof(Convert.ToDecimal),
			} },
			{ typeof(System.Type), new HashSet<string>() }, // nothing allowed
			{ typeof(UnityEngine.Application), new HashSet<string>{ // UnityEngine.Application is whitelisted only for harmless read-only platform info.
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.companyName)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.genuine)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.genuineCheckAvailable)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.identifier)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.installerName)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.installMode)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.internetReachability)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isBatchMode)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isConsolePlatform)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isEditor)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isFocused)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isMobilePlatform)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.isPlaying)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.platform)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.productName)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.runInBackground)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.sandboxType)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.systemLanguage)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.targetFrameRate)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.unityVersion)).GetGetMethod().Name,
				typeof(UnityEngine.Application).GetProperty(nameof(UnityEngine.Application.version)).GetGetMethod().Name,
				nameof(UnityEngine.Application.IsPlaying),
			} },
		};

		public static readonly Dictionary<Type, Type> typeOverrideList = new Dictionary<Type, Type>()
		{
			{ typeof(UnityEngine.Video.VideoPlayer), typeof(Basis.Shims.VideoPlayerShim) },
			{ typeof(UnityEngine.Video.VideoPlayer.ErrorEventHandler), typeof(Basis.Shims.VideoPlayerShim.ErrorEventHandlerShim) },
			{ typeof(UnityEngine.Video.VideoPlayer.EventHandler), typeof(Basis.Shims.VideoPlayerShim.EventHandlerShim) },
			{ typeof(UnityEngine.Video.VideoPlayer.FrameReadyEventHandler), typeof(Basis.Shims.VideoPlayerShim.FrameReadyEventHandlerShim) },
			{ typeof(UnityEngine.Video.VideoPlayer.TimeEventHandler), typeof(Basis.Shims.VideoPlayerShim.TimeEventHandlerShim) },
			{ typeof(UnityEngine.Debug), typeof(Basis.Shims.BasisDebugPropsShim) },
		};


		private List<HashSet<string>> typeWhitelists = new List<HashSet<string>>(){
			systemTypeWhitelist,
			unityTypeWhitelist,
			basisTypeWhitelist,
		};
		private List<HashSet<string>> fieldWhitelists = new List<HashSet<string>>(){
			systemFieldWhitelist,
			unityFieldWhitelist,
			basisFieldWhitelist,
		};
		private List<Dictionary<Type, HashSet<string>>> methodWhitelists = new List<Dictionary<Type, HashSet<string>>>(){
			methodWhitelist,
		};
		private List<Dictionary<Type, Type>> typeOverrideLists = new List<Dictionary<Type, Type>>(){
			typeOverrideList,
		};

		public CilboxBasis(List<HashSet<string>> typeWhitelists, List<HashSet<string>> fieldWhitelists, List<Dictionary<Type, HashSet<string>>> methodWhitelists, List<Dictionary<Type, Type>> typeOverrideLists )
		{
			this.typeWhitelists.AddRange( typeWhitelists );
			this.fieldWhitelists.AddRange( fieldWhitelists );
			this.methodWhitelists.AddRange( methodWhitelists );
			this.typeOverrideLists.AddRange( typeOverrideLists );
		}

		// This is called by CilboxUsage to decide of a type is allowed.
		// If a type is allowed, by defalt it is all allowed.
		override public bool CheckTypeAllowed( String sType )
		{
			return MatchList( sType, typeWhitelists );
		}

		override public bool CheckFieldAllowed( String sType, String sFieldName )
		{
			if( !CheckTypeAllowed( sType ) ) return false;
			return MatchList( sType + "." + sFieldName, fieldWhitelists );
		}

		// After a type is allowed, this is called to see if the specific method is OK.
		override public bool CheckMethodAllowed( out MethodInfo mi, Type declaringType, String name, Serializee [] parametersIn, Serializee [] genericArgumentsIn, String fullSignature )
		{
			mi = null;
			if( name.Contains( "Invoke" ) ) return false;
			foreach( var whitelist in methodWhitelists )
			{
				if( whitelist.TryGetValue( declaringType, out var allowed ) )
				{
					if( !allowed.Contains( name ) ) return false;
				}
			}
			return true;
		}

        public override bool GetTypeOverride(string sType, out Type t)
        {
			foreach( var dict in typeOverrideLists )
			{
				if(Type.GetType( sType ) == null) continue;
				if( dict.TryGetValue( Type.GetType( sType ), out t ) ) return true;
			}
			t = null;
			return false;
        }

		private bool MatchList( string signature, List<HashSet<string>> list )
		{
			foreach( var whitelist in typeWhitelists )
			{
				if( whitelist.Contains( signature ) ) return true;
				foreach( string pattern in whitelist )
				{
					if( !pattern.Contains( '*' ) ) continue;
					string[] allowedPrefix = pattern.Split( '*' );
					if( signature.StartsWith( allowedPrefix[0], StringComparison.Ordinal ) && signature.EndsWith( allowedPrefix[1], StringComparison.Ordinal ) ) return true;
				}
			}
			return false;
		}
	}
}
