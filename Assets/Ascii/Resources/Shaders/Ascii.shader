///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Ascii.
//
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// http://unity3d.com/support/documentation/Components/SL-Shader.html
Shader "Hidden/Ascii"
{
  // http://unity3d.com/support/documentation/Components/SL-Properties.html
  Properties
  {
    _MainTex("Base (RGB)", 2D) = "white" {}
  }

  CGINCLUDE
  #include "UnityCG.cginc"
  #include "BlendCG.cginc"

  sampler2D _MainTex;
  sampler2D _FontTexture;

#if defined(MODE_DEPTH) || defined(MODE_LAYER)
  sampler2D _CameraDepthTexture;
#endif

#ifdef MODE_LAYER
  sampler2D _RTT;
#endif

  half _Amount;
  half _ModeDepthRangeMin;
  half _ModeDepthRangeMax;
  half _Saturation;
  half _Brightness;
  half _Contrast;
  half _Gamma;
  half _InvertVCoord;
  half4 _Color0;
  half4 _Color1;
  half4 _FontParams;
  half _FontCount;
  half _GradienRadius;
  half _GradienHorizontalOffset;
  half _GradienVerticalOffset;

  inline half3 Berp(half3 s, half3 d, half a)
  {
    half3 blended = half3(0.0, 0.0, 0.0);

#ifdef BLENDOP_ADDITIVE
    blended = Additive(s, d);
#elif BLENDOP_BURN
    blended = Burn(s, d);
#elif BLENDOP_COLOR
    blended = Color(s, d);
#elif BLENDOP_DARKEN
    blended = Darken(s, d);
#elif BLENDOP_DARKER
    blended = Darker(s, d);
#elif BLENDOP_DIFFERENCE
    blended = Difference(s, d);
#elif BLENDOP_DIVIDE
    blended = Divide(s, d);
#elif BLENDOP_DODGE
    blended = Dodge(s, d);
#elif BLENDOP_HARDMIX
    blended = HardMix(s, d);
#elif BLENDOP_HUE
    blended = Hue(s, d);
#elif BLENDOP_HARDLIGHT
    blended = HardLight(s, d);
#elif BLENDOP_LIGHTEN
    blended = Lighten(s, d);
#elif BLENDOP_LIGHTER
    blended = Lighter(s, d);
#elif BLENDOP_MULTIPLY
    blended = Multiply(s, d);
#elif BLENDOP_OVERLAY
    blended = Overlay(s, d);
#elif BLENDOP_SCREEN
    blended = Screen(s, d);
#elif BLENDOP_SOLID
    blended = Solid(s, d);
#elif BLENDOP_SOFTLIGHT
    blended = SoftLight(s, d);
#elif BLENDOP_PINLIGHT
    blended = PinLight(s, d);
#elif BLENDOP_SATURATION
    blended = Saturation(s, d);
#elif BLENDOP_SUBTRACT
    blended = Subtract(s, d);
#elif BLENDOP_VIVIDLIGHT
    blended = VividLight(s, d);
#elif BLENDOP_LUMINOSITY
    blended = Luminosity(s, d);
#endif

    return lerp(s, blended, a);
  }

// Do not activate. Only to promotional videos.
//#define ENABLE_DEMO

#ifdef ENABLE_DEMO
  inline float3 PixelDemo(float3 pixel, float3 final, float2 uv)
  {
    const float separator = 0.825f;
    const float separatorWidth = 0.05f;

    if (uv.x > separator)
      final = pixel;
    else if (abs(uv.x - separator) < separatorWidth)
      final = lerp(pixel, final, (separator - uv.x) / separatorWidth);

    return final;
  }
#endif

  half4 frag(v2f_img i) : COLOR
  {
    half2 pixelateUV = (floor(i.uv * _FontParams.zw) / _FontParams.zw);

    half4 original = tex2D(_MainTex, i.uv);

    half3 pixel = tex2D(_MainTex, pixelateUV).rgb;

#ifdef MODE_LAYER
    half2 uv = i.uv;

#if SHADER_API_D3D9
    if (_MainTex_TexelSize.y < 0.0)
      uv.y = 1.0 - uv.y;
#endif
    half depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);
    half depthMask = tex2D(_RTT, uv).a;

    if (depthMask != 1.0 && (depthMask - depth < 0.004))
    {
#endif
    half luminance = dot(pixel, half3(0.299, 0.587, 0.114));
    half fontIndex = floor(luminance * _FontCount);

    half2 fontUV = (frac((_ScreenParams.xy * i.uv) / _FontParams.xy) + half2(fontIndex, 0.0)) / half2(_FontCount, 1.0);

#if SHADER_API_D3D9
    if (_MainTex_TexelSize.y < 0.0)
      fontUV.y = 1.0 - fontUV.y;
#endif

    half3 fontPixel = tex2D(_FontTexture, fontUV).rgb;

    pixel = lerp(luminance * fontPixel, pixel * fontPixel, _Saturation);

#ifdef COLORGRADIENT_SOLID
    pixel *= _Color0;
#elif COLORGRADIENT_HORIZONTAL
    pixel *= lerp(_Color1, _Color0, i.uv.y + _GradienHorizontalOffset);
#elif COLORGRADIENT_VERTICAL
    pixel *= lerp(_Color0, _Color1, i.uv.x + _GradienVerticalOffset);
#elif COLORGRADIENT_CIRCULAR
    pixel *= lerp(_Color0, _Color1, distance(half2(0.5, 0.5), i.uv) * _GradienRadius);
#endif

    pixel = (pixel - 0.5) * _Contrast + 0.5 + _Brightness;
    pixel = clamp(pixel, 0.0, 1.0);
    pixel = pow(pixel, _Gamma);

#ifdef MODE_SCREEN
    pixel = Berp(original.rgb, pixel, _Amount);
#elif MODE_DEPTH
    half depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);

    half modulation = sin(radians(((depth - _ModeDepthRangeMin) / (_ModeDepthRangeMax - _ModeDepthRangeMin)) * 180.0)) * _Amount;
    depth = (depth < _ModeDepthRangeMin || depth > _ModeDepthRangeMax) ? 0.0 : modulation;

    pixel = Berp(original.rgb, pixel, depth);
#elif MODE_LAYER
    pixel = Berp(original.rgb, pixel, _Amount);
    }
    else
      pixel = original.rgb;
#endif

#ifdef ENABLE_DEMO
    pixel = PixelDemo(original, pixel, i.uv);
#endif

    return half4(pixel, 1.0);
  }

  ENDCG

  // Techniques (http://unity3d.com/support/documentation/Components/SL-SubShader.html).
  SubShader
  {
    // Tags (http://docs.unity3d.com/Manual/SL-CullAndDepth.html).
    ZTest Always
    Cull Off
    ZWrite Off
    Fog { Mode off }

    Pass
    {
      CGPROGRAM
      #pragma glsl
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile ___ BLENDOP_ADDITIVE BLENDOP_BURN BLENDOP_COLOR BLENDOP_DARKEN BLENDOP_DARKER BLENDOP_DIFFERENCE BLENDOP_DIVIDE BLENDOP_DODGE BLENDOP_HARDMIX BLENDOP_HUE BLENDOP_HARDLIGHT BLENDOP_LIGHTEN BLENDOP_LIGHTER BLENDOP_MULTIPLY BLENDOP_OVERLAY BLENDOP_SCREEN BLENDOP_SOLID BLENDOP_SOFTLIGHT BLENDOP_PINLIGHT BLENDOP_SATURATION BLENDOP_SUBTRACT BLENDOP_VIVIDLIGHT BLENDOP_LUMINOSITY
      #pragma multi_compile ___ COLORGRADIENT_SOLID COLORGRADIENT_HORIZONTAL COLORGRADIENT_VERTICAL COLORGRADIENT_CIRCULAR
      #pragma multi_compile ___ MODE_SCREEN MODE_DEPTH MODE_LAYER
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment frag
      ENDCG      
    }
  }

  Fallback off
}