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

// Luminance.
inline half Luminance601(half3 pixel)
{
  return dot(half3(0.299f, 0.587f, 0.114f), pixel);
}

// RGB -> HSV http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
inline half3 RGB2HSV(half3 c)
{
  const half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  const half Epsilon = 1.0e-10;

  half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
  half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

  half d = q.x - min(q.w, q.y);

  return half3(abs(q.z + (q.w - q.y) / (6.0 * d + Epsilon)), d / (q.x + Epsilon), q.x);
}

// HSV -> RGB http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
inline half3 HSV2RGB(half3 c)
{
  const half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);

  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Additive.
inline half3 Additive(half3 s, half3 d)
{
  return s + d;
}

// Color burn.
inline half3 Burn(half3 s, half3 d)
{
  return 1.0 - (1.0 - d) / s;
}

// Color.
half3 Color(half3 s, half3 d)
{
  s = RGB2HSV(s);
  s.z = RGB2HSV(d).z;

  return HSV2RGB(s);
}

// Darken.
inline half3 Darken(half3 s, half3 d)
{
  return min(s, d);
}

// Darker color.
inline half3 Darker(half3 s, half3 d)
{
  return (Luminance601(s) < Luminance601(d)) ? s : d;
}

// Difference.
inline half3 Difference(half3 s, half3 d)
{
  return abs(d - s);
}

// Divide.
inline half3 Divide(half3 s, half3 d)
{
  return (d > 0.0) ? s / d : s;
}

// Color dodge.
inline half3 Dodge(half3 s, half3 d)
{
  return (s < 1.0) ? d / (1.0 - s) : s;
}

// HardMix.
inline half3 HardMix(half3 s, half3 d)
{
  return floor(s + d);
}

// Hue.
half3 Hue(half3 s, half3 d)
{
  d = RGB2HSV(d);
  d.x = RGB2HSV(s).x;

  return HSV2RGB(d);
}

// HardLight.
half3 HardLight(half3 s, half3 d)
{
  return (s < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d);
}

// Lighten.
inline half3 Lighten(half3 s, half3 d)
{
  return max(s, d);
}

// Lighter color.
inline half3 Lighter(half3 s, half3 d)
{
  return (Luminance601(s) > Luminance601(d)) ? s : d;
}

// Multiply.
inline half3 Multiply(half3 s, half3 d)
{
  return s * d;
}

// Overlay.
inline half3 Overlay(half3 s, half3 d)
{
  return (s > 0.5) ? 1.0 - 2.0 * (1.0 - s) * (1.0 - d) : 2.0 * s * d;
}

// Screen.
inline half3 Screen(half3 s, half3 d)
{
  return s + d - s * d;
}

// Solid.
inline half3 Solid(half3 s, half3 d)
{
  return d;
}

// Soft light.
half3 SoftLight(half3 s, half3 d)
{
  return (1.0 - s) * s * d + s * (1.0 - (1.0 - s) * (1.0 - d));
}

// Pin light.
half3 PinLight(half3 s, half3 d)
{
  return (2.0 * s - 1.0 > d) ? 2.0 * s - 1.0 : (s < 0.5 * d) ? 2.0 * s : d;
}

// Saturation.
half3 Saturation(half3 s, half3 d)
{
  d = RGB2HSV(d);
  d.y = RGB2HSV(s).y;

  return HSV2RGB(d);
}

// Subtract.
inline half3 Subtract(half3 s, half3 d)
{
  return s - d;
}

// VividLight.
half3 VividLight(half3 s, half3 d)
{
  return (s < 0.5) ? (s > 0.0 ? 1.0 - (1.0 - d) / (2.0 * s) : s) : (s < 1.0 ? d / (2.0 * (1.0 - s)) : s);
}

// Luminosity.
half3 Luminosity(half3 s, half3 d)
{
  half dLum = Luminance601(s);
  half sLum = Luminance601(d);

  half lum = sLum - dLum;

  half3 c = d + lum;
  half minC = min(min(c.r, c.g), c.b);
  half maxC = max(max(c.r, c.b), c.b);

  if (minC < 0.0)
    return sLum + ((c - sLum) * sLum) / (sLum - minC);
  else if (maxC > 1.0)
    return sLum + ((c - sLum) * (1.0 - sLum)) / (maxC - sLum);

  return c;
}
