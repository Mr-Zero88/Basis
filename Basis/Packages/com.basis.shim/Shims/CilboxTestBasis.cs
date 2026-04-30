using System.Collections.Generic;
using System;

namespace Cilbox
{
	[CilboxTarget]
	public class CilboxTestBasis : CilboxBasis
	{

        // public CilboxBasis(List<HashSet<string>> typeWhitelists, List<HashSet<string>> fieldWhitelists, List<Dictionary<Type, HashSet<string>>> methodWhitelists, List<Dictionary<Type, Type>> typeOverrideLists )
		// {
		// 	this.typeWhitelists.AddRange( typeWhitelists );
		// 	this.fieldWhitelists.AddRange( fieldWhitelists );
		// 	this.methodWhitelists.AddRange( methodWhitelists );
		// 	this.typeOverrideLists.AddRange( typeOverrideLists );
		// }

        public CilboxTestBasis() : base(
            new List<HashSet<string>>(),
            new List<HashSet<string>>(),
            new List<Dictionary<Type, HashSet<string>>>(),
            new List<Dictionary<Type, Type>>()
        )
        {
            
        }
    }
}