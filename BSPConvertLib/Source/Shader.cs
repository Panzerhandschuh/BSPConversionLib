﻿using System.Numerics;

namespace BSPConversionLib
{
	[Flags]
	public enum ShaderStageFlags
	{
		GLS_SRCBLEND_ZERO =						0x00000001,
		GLS_SRCBLEND_ONE = 						0x00000002,
		GLS_SRCBLEND_DST_COLOR =				0x00000003,
		GLS_SRCBLEND_ONE_MINUS_DST_COLOR =		0x00000004,
		GLS_SRCBLEND_SRC_ALPHA =				0x00000005,
		GLS_SRCBLEND_ONE_MINUS_SRC_ALPHA =		0x00000006,
		GLS_SRCBLEND_DST_ALPHA =				0x00000007,
		GLS_SRCBLEND_ONE_MINUS_DST_ALPHA =		0x00000008,
		GLS_SRCBLEND_ALPHA_SATURATE =			0x00000009,
		GLS_SRCBLEND_BITS =						0x0000000f,

		GLS_DSTBLEND_ZERO =						0x00000010,
		GLS_DSTBLEND_ONE =						0x00000020,
		GLS_DSTBLEND_SRC_COLOR =				0x00000030,
		GLS_DSTBLEND_ONE_MINUS_SRC_COLOR =		0x00000040,
		GLS_DSTBLEND_SRC_ALPHA =				0x00000050,
		GLS_DSTBLEND_ONE_MINUS_SRC_ALPHA =		0x00000060,
		GLS_DSTBLEND_DST_ALPHA =				0x00000070,
		GLS_DSTBLEND_ONE_MINUS_DST_ALPHA =		0x00000080,
		GLS_DSTBLEND_BITS =						0x000000f0,

		GLS_DEPTHMASK_TRUE =					0x00000100,
		
		GLS_POLYMODE_LINE =						0x00001000,
		
		GLS_DEPTHTEST_DISABLE =					0x00010000,
		GLS_DEPTHFUNC_EQUAL =					0x00020000,
		
		GLS_ATEST_GT_0 =						0x10000000,
		GLS_ATEST_LT_80 =						0x20000000,
		GLS_ATEST_GE_80 =						0x40000000,
		GLS_ATEST_BITS =						0x70000000,
		
		GLS_DEFAULT =							GLS_DEPTHMASK_TRUE
	}

	public enum TexCoordGen
	{
		TCGEN_BAD,
		TCGEN_IDENTITY,         // clear to 0,0
		TCGEN_LIGHTMAP,
		TCGEN_TEXTURE,
		TCGEN_ENVIRONMENT_MAPPED,
		TCGEN_FOG,
		TCGEN_VECTOR            // S and T from world coordinates
	}

	public enum CullType
	{
		FRONT_SIDED,
		BACK_SIDED,
		TWO_SIDED
	}

	public class ShaderStage
	{
		public const int NUM_TEXTURE_BUNDLES = 2;

		public TextureBundle[] bundles = new TextureBundle[NUM_TEXTURE_BUNDLES]; // Path to image file
		public ShaderStageFlags flags;

		public ShaderStage()
		{
			for (var i = 0; i < NUM_TEXTURE_BUNDLES; i++)
				bundles[i] = new TextureBundle();
		}
	}

	public class TextureBundle
	{
		public const int MAX_IMAGE_ANIMATIONS = 8;

		public string[] images = new string[MAX_IMAGE_ANIMATIONS]; // Path to image files
		public int numImageAnimations;
		public float imageAnimationSpeed;

		public TexCoordGen tcGen;
		public Vector3[] tcGenVectors = new Vector3[2];
	}

	public class Shader
	{
		public class SkyParms
		{
			public string outerBox;
			public string cloudHeight;
			public string innerBox;
		}

		public class FogParms
		{
			public Vector3 color;
			public float depthForOpaque;
		}

		public SkyParms skyParms;
		public FogParms fogParms;
		public Q3SurfaceFlags surfaceFlags;
		public Q3ContentsFlags contents;
		public CullType cullType;
		public ShaderStage[] stages;

		/// <summary>
		/// Returns all stages with images (ignores $lightmap and $whiteimage)
		/// </summary>
		public IEnumerable<ShaderStage> GetImageStages()
		{
			return stages.Where(s => !string.IsNullOrEmpty(s.bundles[0].images[0]) &&
				!s.bundles[0].images[0].StartsWith('$'));
		}
	}
}
