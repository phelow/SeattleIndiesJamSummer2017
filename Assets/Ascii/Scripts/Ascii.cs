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
  /// Ascii.
  /// </summary>
  [ExecuteInEditMode]
  [RequireComponent(typeof(Camera))]
  [AddComponentMenu("Image Effects/Ascii")]
  public class Ascii : MonoBehaviour
  {
    #region Common.
    /// <summary>
    /// Sets charset (for custom fonts, use SetCharset).
    /// </summary>
    public AsciiCharsets Charset
    {
      get { return charset; }
      set { SetCharset(value); }
    }

    /// <summary>
    /// Effect strength (0 none, 1 full).
    /// </summary>
    public float Amount
    {
      get { return amount; }
      set { amount = Mathf.Clamp01(value); }
    }

    /// <summary>
    /// Mode (Screen, Depth or Layer).
    /// </summary>
    public AsciiModes Mode
    {
      get { return asciiMode; }
      set
      {
        if (value != asciiMode)
        {
          asciiMode = value;

          if (asciiMode == AsciiModes.Layer)
            CreateDepthCamera();
          else
          {
            this.GetComponent<Camera>().depthTextureMode = (asciiMode == AsciiModes.Depth ? DepthTextureMode.Depth : DepthTextureMode.None);

            DestroyDepthCamera();
          }
        }
      }
    }
    #endregion

    #region Depth modulation (Mode == Modes.Depth).
    /// <summary>
    /// Modulation range min.
    /// </summary>
    public float ModulationDepthRangeMin
    {
      get { return modulationDepthRangeMin; }
      set
      {
        modulationDepthRangeMin = Mathf.Clamp01(value);
        modulationDepthRangeMin = modulationDepthRangeMin < modulationDepthRangeMax ? modulationDepthRangeMin : modulationDepthRangeMax;
      }
    }

    /// <summary>
    /// Modulation range max.
    /// </summary>
    public float ModulationDepthRangeMax
    {
      get { return modulationDepthRangeMax; }
      set
      {
        modulationDepthRangeMax = Mathf.Clamp01(value);
        modulationDepthRangeMax = modulationDepthRangeMax > modulationDepthRangeMin ? modulationDepthRangeMax : modulationDepthRangeMin;
      }
    }
    #endregion

    #region Layer modulation (Mode == Modes.Layer).
    /// <summary>
    /// The layer to which the effect affects.
    /// </summary>
    public LayerMask Layer
    {
      get { return layer; }
      set
      {
        layer = value;

        if (renderDepth != null)
          renderDepth.layer = layer;
      }
    }
    #endregion

    #region Color.
    /// <summary>
    /// Color blend operations (default: Solid).
    /// </summary>
    public AsciiBlendOps BlendOp
    {
      get { return blendOp; }
      set { blendOp = value; }
    }

    /// <summary>
    /// Sets gradient mode (Solid (default), Horizontal, Vertical or Circular).
    /// </summary>
    public AsciiGradients Gradient
    {
      get { return colorGradient; }
      set { colorGradient = value; }
    }

    /// <summary>
    /// Text color. Used in solid gradient.
    /// </summary>
    public Color Color
    {
      get { return color0; }
      set { color0 = value; }
    }

    /// <summary>
    /// Text color 0.
    /// Used in horizontal, vertical and circular gradients.
    /// </summary>
    public Color Color0
    {
      get { return color0; }
      set { color0 = value; }
    }

    /// <summary>
    /// Text color 1. Used in horizontal, vertical and circular gradients.
    /// </summary>
    public Color Color1
    {
      get { return color1; }
      set { color1 = value; }
    }

    /// <summary>
    /// Gradient radius (0 .. 10). Used in circular gradient.
    /// </summary>
    public float GradientCircularRadius
    {
      get { return gradientCircularRadius; }
      set { gradientCircularRadius = Mathf.Clamp(value, 0.0f, 10.0f); }
    }

    /// <summary>
    /// Gradient offset horizontal (0 .. 1). Used in horizontal gradient.
    /// </summary>
    public float GradientHorizontalOffset
    {
      get { return gradientHorizontalOffset; }
      set { gradientHorizontalOffset = Mathf.Clamp(value, 0.0f, 2.0f); }
    }

    /// <summary>
    /// Gradient offset vertical (0 .. 1). Used in vertical gradient.
    /// </summary>
    public float GradientVerticalOffset
    {
      get { return gradientVerticalOffset; }
      set { gradientVerticalOffset = Mathf.Clamp(value, 0.0f, 2.0f); }
    }
    #endregion

    #region Image.
    /// <summary>
    /// Saturation (0 grey, 1 color).
    /// </summary>
    public float Saturation
    {
      get { return saturation; }
      set { saturation = Mathf.Clamp01(value); }
    }

    /// <summary>
    /// Brightness (-1 .. 1).
    /// </summary>
    public float Brightness
    {
      get { return brightness; }
      set { brightness = Mathf.Clamp(value, -1.0f, 1.0f); }
    }

    /// <summary>
    /// Contrast (-1 .. 1).
    /// </summary>
    public float Contrast
    {
      get { return contrast; }
      set { contrast = Mathf.Clamp(value, -1.0f, 1.0f); }
    }

    /// <summary>
    /// Gamma (0.1 .. 10).
    /// </summary>
    public float Gamma
    {
      get { return gamma; }
      set { gamma = Mathf.Clamp(value, 0.1f, 10.0f); }
    }
    #endregion

    #region Private data.
    [SerializeField]
    private float amount = 1.0f;

    // Internal use only.
    [SerializeField]
    public float modulationDepthRangeMin = 0.0f;

    // Internal use only.
    [SerializeField]
    public float modulationDepthRangeMax = 1.0f;

    [SerializeField]
    private float saturation = 1.0f;

    [SerializeField]
    private float brightness = 0.0f;

    [SerializeField]
    private float contrast = 0.0f;

    [SerializeField]
    private float gamma = 1.2f;

    [SerializeField]
    private float gradientCircularRadius = 2.0f;

    [SerializeField]
    private float gradientHorizontalOffset = 1.0f;

    [SerializeField]
    private float gradientVerticalOffset = 1.0f;

    [SerializeField]
    private Color color0 = Color.white;

    [SerializeField]
    private Color color1 = Color.white;

    [SerializeField]
    private AsciiCharsets charset = AsciiCharsets.Lucida_5x8_94;

    [SerializeField]
    private AsciiModes asciiMode = AsciiModes.Screen;

    [SerializeField]
    private AsciiBlendOps blendOp = AsciiBlendOps.Solid;

    [SerializeField]
    private AsciiGradients colorGradient = AsciiGradients.Solid;

    [SerializeField]
    private LayerMask layer = 1;

    // Internal use only.
    [SerializeField]
    public Texture fontTexture;

    // Internal use only.
    [SerializeField]
    public int fontCount;

    private Shader shader;

    private Material material;

    [SerializeField]
    private RenderDepth renderDepth;

    private const string variableAmount = @"_Amount";
    private const string variableModeScreen = @"_ModeScreen";
    private const string variableModeDepth = @"_ModeDepth";
    private const string variableModeDepthRangeMin = @"_ModeDepthRangeMin";
    private const string variableModeDepthRangeMax = @"_ModeDepthRangeMax";
    private const string variableSaturation = @"_Saturation";
    private const string variableBrightness = @"_Brightness";
    private const string variableContrast = @"_Contrast";
    private const string variableGamma = @"_Gamma";
    private const string variableColor0 = @"_Color0";
    private const string variableColor1 = @"_Color1";
    private const string variableFontParams = @"_FontParams";
    private const string variableFontCount = @"_FontCount";
    private const string variableFontTexture = @"_FontTexture";
    private const string variableGradienHorizontalOffset = @"_GradienHorizontalOffset";
    private const string variableGradienVerticalOffset = @"_GradienVerticalOffset";
    private const string variableGradienRadius = @"_GradienRadius";
    private const string variableRenderToTexture = @"_RTT";

    private const string keywordModeScreen = @"MODE_SCREEN";
    private const string keywordModeDepth = @"MODE_DEPTH";
    private const string keywordModeLayer = @"MODE_LAYER";
    private const string keywordColorGradientSolid = @"COLORGRADIENT_SOLID";
    private const string keywordColorGradientHorizontalGradient = @"COLORGRADIENT_HORIZONTAL";
    private const string keywordColorGradientVerticalGradient = @"COLORGRADIENT_VERTICAL";
    private const string keywordColorGradientCircular = @"COLORGRADIENT_CIRCULAR";

    private const string keywordBlenOpAdditive = @"BLENDOP_ADDITIVE";
    private const string keywordBlenOpBurn = @"BLENDOP_BURN";
    private const string keywordBlenOpColor = @"BLENDOP_COLOR";
    private const string keywordBlenOpDarken = @"BLENDOP_DARKEN";
    private const string keywordBlenOpDarker = @"BLENDOP_DARKER";
    private const string keywordBlenOpDifference = @"BLENDOP_DIFFERENCE";
    private const string keywordBlenOpDivide = @"BLENDOP_DIVIDE";
    private const string keywordBlenOpDodge = @"BLENDOP_DODGE";
    private const string keywordBlenOpHardmix = @"BLENDOP_HARDMIX";
    private const string keywordBlenOpHue = @"BLENDOP_HUE";
    private const string keywordBlenOpHardlight = @"BLENDOP_HARDLIGHT";
    private const string keywordBlenOpLighten = @"BLENDOP_LIGHTEN";
    private const string keywordBlenOpLighter = @"BLENDOP_LIGHTER";
    private const string keywordBlenOpMultiply = @"BLENDOP_MULTIPLY";
    private const string keywordBlenOpOverlay = @"BLENDOP_OVERLAY";
    private const string keywordBlenOpScreen = @"BLENDOP_SCREEN";
    private const string keywordBlenOpSolid = @"BLENDOP_SOLID";
    private const string keywordBlenOpSoftlight = @"BLENDOP_SOFTLIGHT";
    private const string keywordBlenOpPinlight = @"BLENDOP_PINLIGHT";
    private const string keywordBlenOpSaturation = @"BLENDOP_SATURATION";
    private const string keywordBlenOpSubtract = @"BLENDOP_SUBTRACT";
    private const string keywordBlenOpVividlight = @"BLENDOP_VIVIDLIGHT";
    private const string keywordBlenOpLuminosity = @"BLENDOP_LUMINOSITY";
    #endregion

    #region Public functions.
    /// <summary>
    /// Sets custom charset.
    /// </summary>
    /// <param name="texturePath">Texture path with the characters.</param>
    /// <param name="fontCount">The number of characters in the texture.</param>
    public void SetCustomCharset(string texturePath, int fontCount)
    {
      if (string.IsNullOrEmpty(texturePath) == false && fontCount > 0)
      {
        this.charset = AsciiCharsets.Custom;

        fontTexture = Resources.Load<Texture>(texturePath);
        if (fontTexture != null)
          this.fontCount = fontCount;
        else
        {
          Debug.LogErrorFormat("Texture '{0}' not found!", texturePath);

          this.enabled = false;
        }
      }
      else
        Debug.LogError(@"Invalid params.");
    }

    /// <summary>
    /// Resets params.
    /// </summary>
    public void Reset()
    {
      amount = 1.0f;

      blendOp = AsciiBlendOps.Solid;
      gradientHorizontalOffset = 1.0f;
      gradientVerticalOffset = 1.0f;
      gradientCircularRadius = 2.0f;

      saturation = 1.0f;
      brightness = 0.0f;
      contrast = 0.0f;
      gamma = 1.2f;
    }
    #endregion

    #region Private functions.
    private void Awake()
    {
      shader = Resources.Load<Shader>(@"Shaders/Ascii");
      if (shader == null)
      {
        Debug.LogError(@"Shader 'Resources/Shaders/Ascii.shader' not found.");

        this.enabled = false;
      }
    }

    private void OnEnable()
    {
      if (SystemInfo.supportsImageEffects == false)
      {
        Debug.LogError(@"Hardware not support Image Effects.");

        this.enabled = false;
      }
      else if (shader == null)
      {
        Debug.LogErrorFormat("'{0}' shader null. Please contact with 'hello@ibuprogames.com' and send the log file.", this.GetType().ToString());

        this.enabled = false;
      }
      else if (shader.isSupported == false)
      {
        Debug.LogErrorFormat("'{0}' shader not supported. Please contact with 'hello@ibuprogames.com' and send the log file.", this.GetType().ToString());

        this.enabled = false;
      }
      else
      {
        CreateMaterial();

        if (asciiMode == AsciiModes.Layer && renderDepth == null)
          CreateDepthCamera();

        this.GetComponent<Camera>().depthTextureMode = (asciiMode == AsciiModes.Depth ? DepthTextureMode.Depth : DepthTextureMode.None);
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
    
    /// <summary>
    /// Creates the material.
    /// </summary>
    private void CreateMaterial()
    {
      if (shader != null)
      {
        material = new Material(shader);
        material.name = @"Ascii-Material";

        if (material != null)
          SetCharset(charset);
        else
        {
          Debug.LogWarningFormat("'{0}' material null. Please contact with 'hello@ibuprogames.com' and send the log file.", this.name);

          this.enabled = false;
        }
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (material != null && fontTexture != null && fontCount > 0)
      {
        material.SetFloat(variableAmount, amount);

        float fontWidth = fontTexture.width / (float)fontCount;

        // Ascci Mode.

        material.DisableKeyword(keywordModeScreen);
        material.DisableKeyword(keywordModeDepth);
        material.DisableKeyword(keywordModeLayer);

        switch (asciiMode)
        {
          case AsciiModes.Screen:
            material.EnableKeyword(keywordModeScreen);
            break;
          case AsciiModes.Depth:
            material.EnableKeyword(keywordModeDepth);
            material.SetFloat(variableModeDepthRangeMin, modulationDepthRangeMin);
            material.SetFloat(variableModeDepthRangeMax, modulationDepthRangeMax);
            break;
          case AsciiModes.Layer:
            material.EnableKeyword(keywordModeLayer);

            if (renderDepth != null)
              material.SetTexture(variableRenderToTexture, renderDepth.renderTexture);
            break;
        }

        material.SetTexture(variableFontTexture, fontTexture);
        material.SetVector(variableFontParams, new Vector4(fontWidth, fontTexture.height, Screen.width / fontWidth, Screen.height / fontTexture.height));
        material.SetFloat(variableFontCount, (float)fontCount);

        // Color.

        material.DisableKeyword(keywordBlenOpAdditive);
        material.DisableKeyword(keywordBlenOpBurn);
        material.DisableKeyword(keywordBlenOpColor);
        material.DisableKeyword(keywordBlenOpDarken);
        material.DisableKeyword(keywordBlenOpDarker);
        material.DisableKeyword(keywordBlenOpDifference);
        material.DisableKeyword(keywordBlenOpDivide);
        material.DisableKeyword(keywordBlenOpDodge);
        material.DisableKeyword(keywordBlenOpHardmix);
        material.DisableKeyword(keywordBlenOpHue);
        material.DisableKeyword(keywordBlenOpHardlight);
        material.DisableKeyword(keywordBlenOpLighten);
        material.DisableKeyword(keywordBlenOpLighter);
        material.DisableKeyword(keywordBlenOpMultiply);
        material.DisableKeyword(keywordBlenOpOverlay);
        material.DisableKeyword(keywordBlenOpScreen);
        material.DisableKeyword(keywordBlenOpSolid);
        material.DisableKeyword(keywordBlenOpSoftlight);
        material.DisableKeyword(keywordBlenOpPinlight);
        material.DisableKeyword(keywordBlenOpSaturation);
        material.DisableKeyword(keywordBlenOpSubtract);
        material.DisableKeyword(keywordBlenOpVividlight);
        material.DisableKeyword(keywordBlenOpLuminosity);

        switch (blendOp)
        {
          case AsciiBlendOps.Additive:   material.EnableKeyword(keywordBlenOpAdditive); break;
          case AsciiBlendOps.Burn:       material.EnableKeyword(keywordBlenOpBurn); break;
          case AsciiBlendOps.Color:      material.EnableKeyword(keywordBlenOpColor); break;
          case AsciiBlendOps.Darken:     material.EnableKeyword(keywordBlenOpDarken); break;
          case AsciiBlendOps.Darker:     material.EnableKeyword(keywordBlenOpDarker); break;
          case AsciiBlendOps.Difference: material.EnableKeyword(keywordBlenOpDifference); break;
          case AsciiBlendOps.Divide:     material.EnableKeyword(keywordBlenOpDivide); break;
          case AsciiBlendOps.Dodge:      material.EnableKeyword(keywordBlenOpDodge); break;
          case AsciiBlendOps.HardMix:    material.EnableKeyword(keywordBlenOpHardmix); break;
          case AsciiBlendOps.Hue:        material.EnableKeyword(keywordBlenOpHue); break;
          case AsciiBlendOps.HardLight:  material.EnableKeyword(keywordBlenOpHardlight); break;
          case AsciiBlendOps.Lighten:    material.EnableKeyword(keywordBlenOpLighten); break;
          case AsciiBlendOps.Lighter:    material.EnableKeyword(keywordBlenOpLighter); break;
          case AsciiBlendOps.Multiply:   material.EnableKeyword(keywordBlenOpMultiply); break;
          case AsciiBlendOps.Overlay:    material.EnableKeyword(keywordBlenOpOverlay); break;
          case AsciiBlendOps.Screen:     material.EnableKeyword(keywordBlenOpScreen); break;
          case AsciiBlendOps.Solid:      material.EnableKeyword(keywordBlenOpSolid); break;
          case AsciiBlendOps.SoftLight:  material.EnableKeyword(keywordBlenOpSoftlight); break;
          case AsciiBlendOps.PinLight:   material.EnableKeyword(keywordBlenOpPinlight); break;
          case AsciiBlendOps.Saturation: material.EnableKeyword(keywordBlenOpSaturation); break;
          case AsciiBlendOps.Subtract:   material.EnableKeyword(keywordBlenOpSubtract); break;
          case AsciiBlendOps.VividLight: material.EnableKeyword(keywordBlenOpVividlight); break;
          case AsciiBlendOps.Luminosity: material.EnableKeyword(keywordBlenOpLuminosity); break;
        }

        material.DisableKeyword(keywordColorGradientSolid);
        material.DisableKeyword(keywordColorGradientHorizontalGradient);
        material.DisableKeyword(keywordColorGradientVerticalGradient);
        material.DisableKeyword(keywordColorGradientCircular);

        material.SetColor(variableColor0, color0);

        switch (colorGradient)
        {
          case AsciiGradients.Solid:
            material.EnableKeyword(keywordColorGradientSolid);
            break;
          case AsciiGradients.Horizontal:
            material.EnableKeyword(keywordColorGradientHorizontalGradient);
            material.SetColor(variableColor1, color1);
            material.SetFloat(variableGradienHorizontalOffset, gradientHorizontalOffset - 1.0f);
            break;
          case AsciiGradients.Vertical:
            material.EnableKeyword(keywordColorGradientVerticalGradient);
            material.SetColor(variableColor1, color1);
            material.SetFloat(variableGradienVerticalOffset, gradientVerticalOffset);
            break;
          case AsciiGradients.Circular:
            material.EnableKeyword(keywordColorGradientCircular);
            material.SetColor(variableColor1, color1);
            material.SetFloat(variableGradienRadius, gradientCircularRadius);
            break;
        }

        // Image.

        material.SetFloat(variableSaturation, saturation);
        material.SetFloat(variableBrightness, brightness);
        material.SetFloat(variableContrast, contrast + 1.0f);
        material.SetFloat(variableGamma, 1.0f / gamma);

        Graphics.Blit(source, destination, material);
      }
    }

    private void SetCharset(AsciiCharsets charset)
    {
      switch (charset)
      {
        case AsciiCharsets.Courier_8x12_94:        SetCharsetTexture(charset, @"Textures/Courier_8x12_94", 94); break;
        case AsciiCharsets.Lucida_5x8_94:          SetCharsetTexture(charset, @"Textures/Lucida_5x8_94", 94); break;
        case AsciiCharsets.TimesNewRoman_11x15_4:  SetCharsetTexture(charset, @"Textures/TimesNewRoman_11x15_4", 4); break;
        case AsciiCharsets.Custom:
          if (Application.isEditor == true)
            this.charset = charset;
          else
            Debug.LogWarning(@"Use SetCustomCharset().");
          break;
      }
    }

    private void SetCharsetTexture(AsciiCharsets charset, string texturePath, int fontCount)
    {
      this.charset = charset;

      fontTexture = Resources.Load<Texture>(texturePath);
      if (fontTexture != null)
        this.fontCount = fontCount;
      else
      {
        Debug.LogErrorFormat("Texture '{0}' not found. Please contact with 'hello@ibuprogames.com' and send the log file.", texturePath);

        this.enabled = false;
      }
    }

    private void CreateDepthCamera()
    {
      if (renderDepth == null)
      {
        GameObject go = new GameObject(@"Ascii Camera", typeof(Camera));
        go.hideFlags = HideFlags.HideAndDontSave;
        go.transform.parent = this.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        renderDepth = go.AddComponent<RenderDepth>();
        renderDepth.layer = layer;
      }
    }

    private void DestroyDepthCamera()
    {
      if (renderDepth != null)
      {
        GameObject obj = renderDepth.gameObject;
        renderDepth = null;

        DestroyImmediate(obj);
      }
    }
    #endregion
  }
}
