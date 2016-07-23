Shader "Curve/ColorDiffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE

#include "Curve_Include.cginc"

#ifdef LIGHTMAP_ON
v2f_diffuse vert1(appdata_base_LM v)
#else
v2f_diffuse vert1(appdata_base v)
#endif
{
    v2f_diffuse o;

	o.pos = Curve_OutputPosition(v.vertex);

    o.uv = v.texcoord.xy;
#ifdef LIGHTMAP_ON
	o.uvLM.xy = v.texcoord1.xy;
#endif

    //TANGENT_SPACE_ROTATION;
    //o.lightDirT = mul(rotation, ObjSpaceLightDir(v.vertex));
    //o.viewDirT = mul(rotation, ObjSpaceViewDir(v.vertex));

	o.lightDirT = ObjSpaceLightDir(v.vertex);
	//o.viewDirT = ObjSpaceViewDir(v.vertex);
	//o.halfVecT = normalize(o.lightDirT + ObjSpaceViewDir(v.vertex));

	o.normal = v.normal;

	VERTEX_FOG(v, o);
    TRANSFER_VERTEX_TO_FRAGMENT(o);

    return o;
}

fixed4 frag1(v2f_diffuse i) : COLOR
{
	return Curve_OutputColorColorDiffuse(i);
}

ENDCG

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

	Pass {
            Name "ContentBase"
            Tags {"LightMode" = "ForwardBase"}

CGPROGRAM

#pragma exclude_renderers flash
#pragma fragmentoption ARB_precision_hint_fastest
#pragma glsl_no_auto_normalization
#pragma multi_compile_fwdbase
#pragma vertex vert1
#pragma fragment frag1

ENDCG

	}
}

Fallback "Mobile/VertexLit"
}
