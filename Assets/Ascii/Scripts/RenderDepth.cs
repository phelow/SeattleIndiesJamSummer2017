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
using UnityEngine;

namespace AsciiImageEffect
{
  /// <summary>
  /// Internal use only. Used in the Depth mode.
  /// </summary>
  [ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  public sealed class RenderDepth : MonoBehaviour
  {
    [HideInInspector]
    public int layer = -1;

    [HideInInspector]
    public RenderTexture renderTexture;

    private Shader shader;

    private Material material;

    private new Camera camera;

    private Camera parentCamera;

    private void Start()
    {
      camera = GetComponent<Camera>();

      shader = Resources.Load<Shader>(@"Shaders/RenderDepth");
      if (shader == null)
      {
        Debug.LogError(@"Shader 'Resources/Shaders/RenderDepth.shader' not found.");

        this.enabled = false;
      }
      else
      {
        material = new Material(shader);
        material.hideFlags = HideFlags.HideAndDontSave;

        parentCamera = transform.parent.GetComponent<Camera>();

        CreateRenderTexture();
      }
    }

    /// <summary>
    /// Destroy the material.
    /// </summary>
    private void OnDisable()
    {
      if (material != null)
        DestroyImmediate(material);
    }

    private void Update()
    {
      if (MustCreateRenderTexture() == true)
        CreateRenderTexture();
    }

    private void LateUpdate()
    {
      if (parentCamera != null && renderTexture != null)
      {
        camera.CopyFrom(parentCamera);

        camera.cullingMask = layer;
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
        camera.targetTexture = renderTexture;
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (material != null)
        Graphics.Blit(source, destination, material);
    }

    private bool MustCreateRenderTexture()
    {
      if (renderTexture == null)
        return true;

      return renderTexture.IsCreated() == false || (Screen.width != renderTexture.width) || (Screen.height != renderTexture.height);
    }

    private void CreateRenderTexture()
    {
      if (Screen.width > 0 && Screen.height > 0)
      {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24,
                                          Application.isMobilePlatform == true ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);

        if (renderTexture != null)
        {
          renderTexture.isPowerOfTwo = false;
          renderTexture.antiAliasing = 1;
          renderTexture.name = @"RenderTexture from Ascii";
          if (renderTexture.Create() != true)
          {
            Debug.LogErrorFormat("Hardware not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

            this.enabled = false;
          }
        }
        else
        {
          Debug.LogErrorFormat("RenderTexture null, hardware may not support Render-To-Texture, '{0}' disabled.", this.GetType().ToString());

          this.enabled = false;
        }
      }
      else
      {
        Debug.LogErrorFormat("Wrong screen resolution '{0}x{1}', '{2}' disabled.", Screen.width, Screen.height, this.GetType().ToString());

        this.enabled = false;
      }
    }
  }
}
