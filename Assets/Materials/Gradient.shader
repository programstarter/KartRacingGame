Shader "Gradient"
{
    Properties
    {
        [PerRendererData]_MainTex ("Base (RGB)", 2D) = "white" { }
        _ColorFrom ("Color From", Color) = (1, 1, 1, 1)
        _ColorTo ("Color To", Color) = (1, 1, 1, 1)
        _LinearPosFrom ("Position From", Range(0, 0.99)) = 0
        _LinearPosTo ("Position To", Range(0.01, 1)) = 1
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            float4 _ColorFrom;
            float4 _ColorTo;
            float _LinearPosFrom;
            float _LinearPosTo;
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            fixed4 frag(v2f i): SV_Target
            {
                fixed4 col = fixed4(0, 0, 0, 1);
                col.rgba = lerp(_ColorFrom, _ColorTo, (i.uv.x - _LinearPosFrom) / (_LinearPosTo - _LinearPosFrom));
                col.rgba = lerp(_ColorFrom.rgba, col.rgba, step(_LinearPosFrom, i.uv.x));
                col.rgba = lerp(_ColorTo.rgba, col.rgba, step(i.uv.x, _LinearPosTo));
                return col;
            }
            ENDCG
        }
    }
}