// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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
Shader "Hidden/RenderDepth"
{
  SubShader
  {
    Pass
    {
      ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      sampler2D _CameraDepthTexture;
      half4 _MainTex_TexelSize;

      struct input
      {
        float4 pos : POSITION;
        half2 uv : TEXCOORD0;
      };

      struct output
      {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
      };

      output vert(input i)
      {
        output o;
        o.pos = UnityObjectToClipPos(i.pos);
        o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, i.uv);
#if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
          o.uv.y = 1 - o.uv.y;
#endif

        return o;
      }

      fixed4 frag(output o) : COLOR
      {
        float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, o.uv));
        depth = Linear01Depth(depth);

        return depth;
      }
      ENDCG
    }
  }
}
