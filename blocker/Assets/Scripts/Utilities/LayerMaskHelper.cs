//This code came from:
//http://pixelplacement.com/2012/01/31/layermasks-simplified/

//It is meant to make layers (bitwise flags) easier to handle.

using UnityEngine;

public class LayerMaskHelper {
	
	public static int OnlyIncluding( params int[] layers ){
		return MakeMask( layers );
	}
	
	public static int EverythingBut( params int[] layers ){
		return ~MakeMask( layers );
	}
	
	public static bool ContainsLayer( LayerMask layerMask, int layer ){
		return ( layerMask.value & 1 << layer ) != 0 ;	
	}
	
	static int MakeMask( params int[] layers ){
		int mask = 0;
		foreach ( int item in layers ) {
			mask |= 1 << item;
		}
		return mask;	
	}
	
}
