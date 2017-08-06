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
using UnityEditor;

namespace AsciiImageEffect
{
  /// <summary>
  /// Ascii editor.
  /// </summary>
  [CustomEditor(typeof(Ascii))]
  public class AsciiEditor : Editor
  {
    /// <summary>
    /// Desc.
    /// </summary>
    private readonly string asciiDesc = @"Ascii image effect with steroids.";

    /// <summary>
    /// Help text.
    /// </summary>
    public string Help { get; set; }

    /// <summary>
    /// Warnings.
    /// </summary>
    public string Warnings { get; set; }

    /// <summary>
    /// Errors.
    /// </summary>
    public string Errors { get; set; }

    private string displayColorControlsKey;
    private string displayImageControlsKey;

    private bool displayColorControls = false;
    private bool displayImageControls = false;

    private void OnEnable()
    {
      string productID = GetType().ToString().Replace(@"Editor", string.Empty);
      
      displayColorControlsKey = string.Format("{0}.displayColorControls", productID);
      displayImageControlsKey = string.Format("{0}.displayImageControls", productID);

      displayColorControls = PlayerPrefs.GetInt(displayColorControlsKey, 0) == 1;
      displayImageControls = PlayerPrefs.GetInt(displayImageControlsKey, 0) == 1;
    }

    /// <summary>
    /// OnInspectorGUI.
    /// </summary>
    public override void OnInspectorGUI()
    {
      EditorGUI.indentLevel = 0;

      EditorGUIUtility.labelWidth = 100.0f;

      Ascii targetObject = (Ascii)target;

      Undo.RecordObject(targetObject, targetObject.GetType().Name);

      EditorGUILayout.BeginVertical();
      {
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical();
        {
          targetObject.Charset = (AsciiCharsets)EditorGUILayout.EnumPopup(new GUIContent(@"Charset", @"Characters to be used."), targetObject.Charset);
          if (targetObject.Charset == AsciiCharsets.Custom)
          {
            EditorGUI.indentLevel++;

            targetObject.fontCount = EditorGUILayout.IntField(new GUIContent(@"Char count", @"The number of characters in the texture."), targetObject.fontCount);

            targetObject.fontTexture = EditorGUILayout.ObjectField(new GUIContent(@"Font texture", @"The texture with the characters."), targetObject.fontTexture, typeof(Texture), false) as Texture;

            EditorGUI.indentLevel--;
          }

          targetObject.Amount = AsciiEditorHelper.SliderWithReset(@"Amount", "Effect strength (0 none, 1 full).", targetObject.Amount, 0.0f, 1.0f, 1.0f);

          targetObject.Mode = (AsciiModes)EditorGUILayout.EnumPopup(new GUIContent(@"Mode", @"Modes."), targetObject.Mode);

          EditorGUI.indentLevel++;

          switch (targetObject.Mode)
          {
            case AsciiModes.Screen: break;
            case AsciiModes.Depth:
              EditorGUILayout.MinMaxSlider(new GUIContent(@"Range", @"Depth range modulation in Depth mode (0 near, 1 far)."), ref targetObject.modulationDepthRangeMin, ref targetObject.modulationDepthRangeMax, 0.0f, 1.0f);
              break;
            case AsciiModes.Layer:
              targetObject.Layer = AsciiEditorHelper.LayerMaskField(@"Layer mask", targetObject.Layer);
              break;
          }

          EditorGUI.indentLevel--;

          EditorGUILayout.Separator();

          displayColorControls = AsciiEditorHelper.Foldout(displayColorControls, @"Color");
          if (displayColorControls == true)
          {
            EditorGUI.indentLevel++;

            targetObject.BlendOp = (AsciiBlendOps)EditorGUILayout.EnumPopup(new GUIContent(@"Blend op.", @"Color blend operation."), targetObject.BlendOp);

            targetObject.Gradient = (AsciiGradients)EditorGUILayout.EnumPopup(@"Gradient", targetObject.Gradient);

            EditorGUI.indentLevel++;

            if (targetObject.Gradient == AsciiGradients.Solid)
            {
              targetObject.Color = EditorGUILayout.ColorField(new GUIContent(@"Color", @"Text color"), targetObject.Color);
            }
            else
            {
              targetObject.Color0 = EditorGUILayout.ColorField(new GUIContent(@"Color 0", @"First text color"), targetObject.Color0);

              targetObject.Color1 = EditorGUILayout.ColorField(new GUIContent(@"Color 1", @"Second text color"), targetObject.Color1);
            }

            if (targetObject.Gradient == AsciiGradients.Horizontal)
              targetObject.GradientHorizontalOffset = AsciiEditorHelper.SliderWithReset(@"Offset", "Horizontal offset.", targetObject.GradientHorizontalOffset, 0.0f, 2.0f, 1.0f);
            else if (targetObject.Gradient == AsciiGradients.Vertical)
              targetObject.GradientVerticalOffset = AsciiEditorHelper.SliderWithReset(@"Offset", "Vertical offset.", targetObject.GradientVerticalOffset, 0.0f, 2.0f, 1.0f);
            else if (targetObject.Gradient == AsciiGradients.Circular)
              targetObject.GradientCircularRadius = AsciiEditorHelper.SliderWithReset(@"Radius", "Gradient radius.", targetObject.GradientCircularRadius, 0.0f, 10.0f, 1.0f);

            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
          }

          EditorGUILayout.Separator();

          displayImageControls = AsciiEditorHelper.Foldout(displayImageControls, @"Image");
          if (displayImageControls == true)
          {
            EditorGUI.indentLevel++;
            
            targetObject.Saturation = AsciiEditorHelper.SliderWithReset(@"Saturation", "The saturation.\nFrom 0 (grey) to 1 (color).", targetObject.Saturation, 0.0f, 1.0f, 1.0f);

            targetObject.Brightness = AsciiEditorHelper.SliderWithReset(@"Brightness", "The Screen appears to be more o less radiating light.\nFrom -1 (dark) to 1 (full light).", targetObject.Brightness, -1.0f, 1.0f, 0.0f);

            targetObject.Contrast = AsciiEditorHelper.SliderWithReset(@"Contrast", "The difference in color and brightness.\nFrom -1 (no constrast) to 1 (full constrast).", targetObject.Contrast, -1.0f, 1.0f, 0.0f);

            targetObject.Gamma = AsciiEditorHelper.SliderWithReset(@"Gamma", "Optimizes the contrast and brightness in the midtones.\nFrom 0.01 to 10.", targetObject.Gamma, 0.01f, 10.0f, 1.2f);


            EditorGUI.indentLevel--;
          }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        {
          if (GUILayout.Button(new GUIContent(@"[doc]", @"Online documentation."), GUI.skin.label) == true)
            Application.OpenURL(@"http://www.ibuprogames.com/2015/05/09/ascii-image-effect/");

          GUILayout.FlexibleSpace();

          if (GUILayout.Button(@"Reset") == true)
            targetObject.Reset();
        }
        EditorGUILayout.EndHorizontal();

        Help += asciiDesc;

        EditorGUILayout.Separator();

        if (string.IsNullOrEmpty(Warnings) == false)
        {
          EditorGUILayout.HelpBox(Warnings, MessageType.Warning);

          EditorGUILayout.Separator();
        }

        if (string.IsNullOrEmpty(Errors) == false)
        {
          EditorGUILayout.HelpBox(Errors, MessageType.Error);

          EditorGUILayout.Separator();
        }

        if (string.IsNullOrEmpty(Help) == false)
          EditorGUILayout.HelpBox(Help, MessageType.Info);
      }
      EditorGUILayout.EndVertical();

      if (GUI.changed == true)
      {
        PlayerPrefs.SetInt(displayColorControlsKey, displayColorControls == true ? 1 : 0);
        PlayerPrefs.SetInt(displayImageControlsKey, displayImageControls == true ? 1 : 0);

        EditorUtility.SetDirty(targetObject);
      }

      EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0.0f;

      Help = Warnings = Errors = string.Empty;
    }
  }
}