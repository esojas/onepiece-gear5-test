Shader "Custom/BouncySurfaceURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _WaveStrength ("Wave Strength", Float) = 0.3
        _WaveSpeed ("Wave Speed", Float) = 4.0
        _WaveFrequency ("Wave Frequency", Float) = 8.0
        _WaveDuration ("Wave Duration", Float) = 1.5
        _ImpactPoint ("Impact Point", Vector) = (0,0,0,0)
        _ImpactTime ("Impact Time", Float) = -999
        _CurrentTime ("Current Time", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float  _WaveStrength;
                float  _WaveSpeed;
                float  _WaveFrequency;
                float  _WaveDuration;
                float4 _ImpactPoint;
                float  _ImpactTime;
                float  _CurrentTime;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Convert to world space to measure distance from impact
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                float elapsed = _CurrentTime - _ImpactTime;

                if (elapsed >= 0 && elapsed < _WaveDuration)
                {
                    float dist      = distance(worldPos, _ImpactPoint.xyz);
                    float waveFront = elapsed * _WaveSpeed;

                    // Spreading wave ring
                    float ringWidth = 1.5;
                    float ring      = exp(-pow(dist - waveFront, 2.0) * ringWidth);

                    // Sin oscillation
                    float wave      = sin(_WaveFrequency * dist - _WaveSpeed * elapsed);

                    // Fade out over time
                    float decay     = 1.0 - smoothstep(0.0, _WaveDuration, elapsed);

                    float displacement = ring * wave * decay * _WaveStrength;

                    // Push vertex along its normal
                    IN.positionOS.xyz += IN.normalOS * displacement;
                }

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Basic Lambert lighting
                Light mainLight = GetMainLight();
                float NdotL     = saturate(dot(normalize(IN.normalWS), mainLight.direction));
                half3 lighting  = mainLight.color * (NdotL * 0.7 + 0.3); // 0.3 = ambient

                return half4(texColor.rgb * _Color.rgb * lighting, texColor.a * _Color.a);
            }

            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}