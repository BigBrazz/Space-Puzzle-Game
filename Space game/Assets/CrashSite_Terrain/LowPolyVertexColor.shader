Shader "Custom/LowPolyVertexColor"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.0
        _Tint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        struct Input { float4 vcol; };
        half _Glossiness;
        fixed4 _Tint;
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vcol = v.color;
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.vcol.rgb * _Tint.rgb;
            o.Metallic = 0;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
