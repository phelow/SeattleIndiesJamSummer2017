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
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace AsciiImageEffect
{
  /// <summary>
  /// Utilities for the Editor.
  /// </summary>
  public static class AsciiEditorHelper
  {
    private class HeaderStyle
    {
      public GUIStyle header = @"ShurikenModuleTitle";
      public GUIStyle headerCheckbox = @"ShurikenCheckMark";

      internal HeaderStyle()
      {
        header.font = (new GUIStyle(@"Label")).font;
        header.border = new RectOffset(15, 7, 4, 4);
        header.fixedHeight = 22;
        header.contentOffset = new Vector2(20f, -2f);
      }
    }

    private static HeaderStyle headerStyle;

    static AsciiEditorHelper()
    {
      headerStyle = new HeaderStyle();
    }

    public static bool Foldout(bool display, string title)
    {
      Rect rect = GUILayoutUtility.GetRect(16.0f, 22.0f, headerStyle.header);
      GUI.Box(rect, title, headerStyle.header);

      Rect toggleRect = new Rect(rect.x + 4.0f, rect.y + 2.0f, 13.0f, 13.0f);
      if (Event.current.type == EventType.Repaint)
        EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

      Event e = Event.current;
      if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
      {
        display = !display;
        e.Use();
      }

      return display;
    }

    public static bool Header(ref bool display, bool enabled, string title)
    {
      Rect rect = GUILayoutUtility.GetRect(16.0f, 22.0f, headerStyle.header);
      GUI.Box(rect, title, headerStyle.header);

      Rect toggleRect = new Rect(rect.x + 4.0f, rect.y + 4.0f, 13.0f, 13.0f);
      if (Event.current.type == EventType.Repaint)
        headerStyle.headerCheckbox.Draw(toggleRect, false, false, enabled, false);

      Event e = Event.current;
      if (e.type == EventType.MouseDown)
      {
        if (toggleRect.Contains(e.mousePosition))
        {
          enabled = !enabled;
          e.Use();
          GUI.changed = true;
        }
        else if (rect.Contains(e.mousePosition))
        {
          display = !display;
          e.Use();
          GUI.changed = true;
        }
      }

      return enabled;
    }

    /// <summary>
    /// A slider with a reset button.
    /// </summary>
    public static float SliderWithReset(string label, string tooltip, float value, float minValue, float maxValue, float defaultValue)
    {
      EditorGUILayout.BeginHorizontal();
      {
        value = EditorGUILayout.Slider(new GUIContent(label, tooltip), value, minValue, maxValue);

        if (GUILayout.Button("R", GUILayout.Width(18.0f), GUILayout.Height(17.0f)) == true)
          value = defaultValue;
      }
      EditorGUILayout.EndHorizontal();

      return value;
    }

    /// <summary>
    /// A slider with a reset button.
    /// </summary>
    public static int IntSliderWithReset(string label, string tooltip, int value, int minValue, int maxValue, int defaultValue)
    {
      EditorGUILayout.BeginHorizontal();
      {
        value = EditorGUILayout.IntSlider(new GUIContent(label, tooltip), value, minValue, maxValue);

        if (GUILayout.Button(new GUIContent("R", "Reset to '" + defaultValue + "'."), GUILayout.Width(18.0f), GUILayout.Height(17.0f)) == true)
          value = defaultValue;
      }
      EditorGUILayout.EndHorizontal();

      return value;
    }

    /// <summary>
    /// Layer.
    /// </summary>
    public static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
      List<string> layers = new List<string>();
      List<int> layerNumbers = new List<int>();

      for (int i = 0; i < 32; ++i)
      {
        string layerName = LayerMask.LayerToName(i);
        if (string.IsNullOrEmpty(layerName) == false)
        {
          layers.Add(layerName);
          layerNumbers.Add(i);
        }
      }

      int maskWithoutEmpty = 0;
      for (int i = 0; i < layerNumbers.Count; ++i)
      {
        if (((1 << layerNumbers[i]) & layerMask.value) > 0)
          maskWithoutEmpty |= (1 << i);
      }

      maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
      int mask = 0;
      for (int i = 0; i < layerNumbers.Count; ++i)
      {
        if ((maskWithoutEmpty & (1 << i)) > 0)
          mask |= (1 << layerNumbers[i]);
      }

      layerMask.value = mask;

      return layerMask;
    }
  }
}