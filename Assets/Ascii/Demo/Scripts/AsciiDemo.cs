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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AsciiImageEffect;

/// <summary>
/// Ascii demo.
/// </summary>
public sealed class AsciiDemo : MonoBehaviour
{
  public bool guiShow = true;

  public bool demoReelMode = false;

  public AudioClip musicClip = null;

  private Ascii amazingAscii;

  private bool menuOpen = false;

  private const float guiMargen = 10.0f;
  private const float guiWidth = 200.0f;

  private float updateInterval = 0.5f;
  private float accum = 0.0f;
  private int frames = 0;
  private float timeleft;
  private float fps = 0.0f;

  private GUIStyle effectNameStyle;
  private GUIStyle menuStyle;
  private GUIStyle boxStyle;

  private readonly string[] charsetsStrings = { "Lucida", "Courier", "NewRoman" };
  private readonly string[] modulationStrings = { "Screen", "Depth", "Layer" };
  private readonly string[] colorModesStrings = { "Solid", "Horizontal", "Vertical", "Circular" };

  private enum DemoReelJobType
  {
    DemoGradients,  // Demo with gradients.
    DemoDepth,      // Demo with Depth.
    DemoLayer,      // Demo with Layers.
    Wait,           // Wait X seconds.
    Loop,           // Repeat.
    End,            // End.
  }

  private class DemoReelJob
  {
    public DemoReelJobType type;

    public float param0, param1;

    public string string0;
  }

  private List<DemoReelJob> jobs = new List<DemoReelJob>();

  private DemoReelJob currentJob = null;
  private IEnumerator currentCoroutine;

  private void OnEnable()
  {
    Camera selectedCamera = null;
    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

    for (int i = 0; i < cameras.Length; ++i)
    {
      if (cameras[i].enabled == true)
      {
        selectedCamera = cameras[i];

        break;
      }
    }

    if (selectedCamera != null)
    {
      amazingAscii = selectedCamera.gameObject.GetComponent<Ascii>();
      if (amazingAscii == null)
        amazingAscii = selectedCamera.gameObject.AddComponent<Ascii>();

      if (musicClip != null)
      {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
      }

      amazingAscii.Gamma = 1.2f;
      amazingAscii.Contrast = 0.15f;

      if (demoReelMode == true)
        BeginDemoReel();
    }
    else
      Debug.LogWarning("No camera found.");
  }

  private void Update()
  {
    timeleft -= Time.deltaTime;
    accum += Time.timeScale / Time.deltaTime;
    frames++;

    if (timeleft <= 0.0f)
    {
      fps = accum / frames;
      timeleft = updateInterval;
      accum = 0.0f;
      frames = 0;
    }

    if (Input.GetKeyUp(KeyCode.F1) == true)
      guiShow = !guiShow;

    if (currentJob == null && jobs.Count > 0)
      NextJob();

#if !UNITY_WEBPLAYER
    if (Input.GetKeyDown(KeyCode.Escape))
      Application.Quit();
#endif
  }

  private void OnGUI()
  {
#if UNITY_ANDROID || UNITY_IPHONE
    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1280.0f, (float)Screen.height / 720.0f, 1.0f));
#endif
    if (amazingAscii == null)
      return;

    if (effectNameStyle == null)
    {
      effectNameStyle = new GUIStyle(GUI.skin.textArea);
      effectNameStyle.alignment = TextAnchor.MiddleCenter;
      effectNameStyle.fontSize = 22;
    }

    if (menuStyle == null)
    {
      menuStyle = new GUIStyle(GUI.skin.textArea);
      menuStyle.alignment = TextAnchor.MiddleCenter;
      menuStyle.fontSize = 14;
    }

    if (boxStyle == null)
    {
      boxStyle = new GUIStyle(GUI.skin.box);
      boxStyle.normal.background = MakeTex(2, 2, new Color(0.75f, 0.75f, 0.75f, 0.75f));
      boxStyle.focused.textColor = Color.red;
    }

    if (guiShow == false)
    {
      if (demoReelMode == true && currentJob != null)
      {
        string label = string.Empty;
        switch (currentJob.type)
        {
          case DemoReelJobType.DemoGradients: label = "GRADIENTS"; break;
          case DemoReelJobType.DemoDepth:     label = "DEPTH"; break;
          case DemoReelJobType.DemoLayer:     label = "LAYER MASKS"; break;
        }

        if (string.IsNullOrEmpty(label) == false)
        {
          GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
          {
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, menuStyle, GUILayout.Width(200.0f));
            GUILayout.FlexibleSpace();
          }
          GUILayout.EndHorizontal();
        }
      }

      return;
    }

    GUILayout.BeginHorizontal(boxStyle, GUILayout.Width(Screen.width));
    {
      GUILayout.Space(guiMargen);

      if (GUILayout.Button("MENU", menuStyle, GUILayout.Width(80.0f)) == true)
        menuOpen = !menuOpen;

      GUILayout.FlexibleSpace();

      GUILayout.Label(@"AMAZING ASCII", menuStyle, GUILayout.Width(200.0f));

      GUILayout.FlexibleSpace();

      if (fps < 30.0f)
        GUI.contentColor = Color.yellow;
      else if (fps < 15.0f)
        GUI.contentColor = Color.red;
      else
        GUI.contentColor = Color.green;

      GUILayout.Label(fps.ToString("000"), menuStyle, GUILayout.Width(40.0f));

      GUI.contentColor = Color.white;

      GUILayout.Space(guiMargen);
    }
    GUILayout.EndHorizontal();

    if (menuOpen == true)
    {
      GUILayout.BeginVertical(boxStyle, GUILayout.Width(guiWidth));
      {
        GUILayout.Space(guiMargen);

        if (GUILayout.Button(demoReelMode == true ? "Disable 'Demo' mode" : "Enable 'Demo' mode") == true)
        {
          demoReelMode = !demoReelMode;

          if (demoReelMode == true)
            BeginDemoReel();
          else
            ResetDemoReel();
        }

        GUI.enabled = !demoReelMode;

        GUILayout.Space(guiMargen);

        // Parameters.

        GUILayout.BeginVertical(boxStyle);
        {
          amazingAscii.Charset = (AsciiCharsets)GUILayout.SelectionGrid((int)amazingAscii.Charset, charsetsStrings, charsetsStrings.Length);

          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Amount", GUILayout.Width(70));
            amazingAscii.Amount = GUILayout.HorizontalSlider(amazingAscii.Amount, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();

          amazingAscii.Mode = (AsciiModes)GUILayout.Toolbar((int)amazingAscii.Mode, modulationStrings);
          switch (amazingAscii.Mode)
          {
            case AsciiModes.Screen:
              // No special parameters.
              break;

            case AsciiModes.Depth:
              GUILayout.BeginHorizontal();
              {
                GUILayout.Label(" Min", GUILayout.Width(30));
                amazingAscii.ModulationDepthRangeMin = GUILayout.HorizontalSlider(amazingAscii.ModulationDepthRangeMin, 0.0f, 1.0f);
              }
              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();
              {
                GUILayout.Label(" Max", GUILayout.Width(30));
                amazingAscii.ModulationDepthRangeMax = GUILayout.HorizontalSlider(amazingAscii.ModulationDepthRangeMax, 0.0f, 1.0f);
              }
              GUILayout.EndHorizontal();
              break;

            case AsciiModes.Layer:
              DrawLayerMasks();
              break;
          }

          GUILayout.Space(guiMargen);

          amazingAscii.Gradient = (AsciiGradients)GUILayout.Toolbar((int)amazingAscii.Gradient, colorModesStrings);
          switch (amazingAscii.Gradient)
          {
            case AsciiGradients.Solid:
              if (GUILayout.Button("Change color") == true)
                amazingAscii.Color = NiceColor();
              break;

            case AsciiGradients.Horizontal:
              if (GUILayout.Button("Change color 0") == true)
                amazingAscii.Color0 = NiceColor();

              if (GUILayout.Button("Change color 1") == true)
                amazingAscii.Color1 = NiceColor();

              GUILayout.BeginHorizontal();
              {
                GUILayout.Label(" Offset", GUILayout.Width(50));
                amazingAscii.GradientHorizontalOffset = GUILayout.HorizontalSlider(amazingAscii.GradientHorizontalOffset, 0.0f, 1.0f);
              }
              GUILayout.EndHorizontal();
              break;

            case AsciiGradients.Vertical:
              if (GUILayout.Button("Change color 0") == true)
                amazingAscii.Color0 = NiceColor();

              if (GUILayout.Button("Change color 1") == true)
                amazingAscii.Color1 = NiceColor();

              GUILayout.BeginHorizontal();
              {
                GUILayout.Label(" Offset", GUILayout.Width(50));
                amazingAscii.GradientVerticalOffset = GUILayout.HorizontalSlider(amazingAscii.GradientVerticalOffset, 0.0f, 1.0f);
              }
              GUILayout.EndHorizontal();
              break;

            case AsciiGradients.Circular:
              if (GUILayout.Button(" Change color 0") == true)
                amazingAscii.Color0 = NiceColor();

              if (GUILayout.Button(" Change color 1") == true)
                amazingAscii.Color1 = NiceColor();

              GUILayout.BeginHorizontal();
              {
                GUILayout.Label(" Radius", GUILayout.Width(50));
                amazingAscii.GradientCircularRadius = GUILayout.HorizontalSlider(amazingAscii.GradientCircularRadius, 0.0f, 10.0f);
              }
              GUILayout.EndHorizontal();
              break;
          }

          GUILayout.Space(guiMargen);

          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Saturation", GUILayout.Width(70));
            amazingAscii.Saturation = GUILayout.HorizontalSlider(amazingAscii.Saturation, 0.0f, 1.0f);
          }
          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Brightness", GUILayout.Width(70));
            amazingAscii.Brightness = GUILayout.HorizontalSlider(amazingAscii.Brightness, -1.0f, 1.0f);
          }
          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Contrast", GUILayout.Width(70));
            amazingAscii.Contrast = GUILayout.HorizontalSlider(amazingAscii.Contrast, -1.0f, 1.0f);
          }
          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Gamma", GUILayout.Width(70));
            amazingAscii.Gamma = GUILayout.HorizontalSlider(amazingAscii.Gamma, 0.1f, 10.0f);
          }
          GUILayout.EndHorizontal();

          GUILayout.Space(guiMargen);

          if (GUILayout.Button("Reset") == true)
          {
            amazingAscii.Reset();

            amazingAscii.Color0 = Color.white;
            amazingAscii.Color1 = Color.white;
          }
        }
        GUILayout.EndVertical();

        GUI.enabled = true;

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical(boxStyle);
        {
          GUILayout.Label("F1 - Hide/Show gui.");
        }
        GUILayout.EndVertical();

        GUILayout.Space(guiMargen);

        if (GUILayout.Button(@"Open Web") == true)
          Application.OpenURL(@"http://www.ibuprogames.com/2015/05/09/ascii-image-effect/");

#if !UNITY_WEBPLAYER
        if (GUILayout.Button(@"Quit") == true)
          Application.Quit();
#endif
      }
      GUILayout.EndVertical();
    }
  }

  private void BeginDemoReel()
  {
    ResetDemoReel();

    float globalSpeed = 1.25f;

    // Wait.
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.Wait, param0 = 0.5f });

    // Demo gradients.
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.DemoGradients, param0 = globalSpeed * 2.0f });

    // Demo Depth.
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.DemoDepth, param0 = globalSpeed * 0.25f });

    // Demo Layers.
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.DemoLayer, string0 = "Floor", param0 = globalSpeed, param1 = (float)AsciiCharsets.TimesNewRoman_11x15_4 });
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.DemoLayer, string0 = "Walls", param0 = globalSpeed, param1 = (float)AsciiCharsets.Courier_8x12_94 });
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.DemoLayer, string0 = "Shapes", param0 = globalSpeed, param1 = (float)AsciiCharsets.Lucida_5x8_94 });

    // Loop.
    jobs.Add(new DemoReelJob() { type = DemoReelJobType.Loop });
  }

  private void ResetDemoReel()
  {
    currentJob = null;
    jobs.Clear();

    if (currentCoroutine != null)
    {
      this.StopCoroutine(currentCoroutine);

      currentCoroutine = null;
    }

    amazingAscii.Reset();
    amazingAscii.Charset = AsciiCharsets.Lucida_5x8_94;
    amazingAscii.Mode = AsciiModes.Screen;
    amazingAscii.Gradient = AsciiGradients.Solid;
    amazingAscii.Color0 = Color.white;
    amazingAscii.Color1 = Color.white;
    amazingAscii.Amount = 0.0f;
  }

  private void NextJob()
  {
    if (jobs.Count > 0 && currentJob == null)
    {
      currentJob = jobs[0];
      jobs.RemoveAt(0);

      if (currentCoroutine != null)
      {
        this.StopCoroutine(currentCoroutine);

        currentCoroutine = null;
      }

      switch (currentJob.type)
      {
        case DemoReelJobType.DemoGradients:
          currentCoroutine = JobDemoGradients(currentJob.param0);
          break;

        case DemoReelJobType.DemoDepth:
          currentCoroutine = JobDemoDepth(currentJob.param0);
          break;

        case DemoReelJobType.DemoLayer:
          currentCoroutine = JobDemoLayer(currentJob.string0, currentJob.param0, (AsciiCharsets)currentJob.param1);
          break;

        case DemoReelJobType.End:
          ResetDemoReel();
          break;

        case DemoReelJobType.Loop:
          BeginDemoReel();
          break;

        case DemoReelJobType.Wait:
          currentCoroutine = JobWait(currentJob.param0);
          break;
      }

      if (currentCoroutine != null)
        this.StartCoroutine(currentCoroutine);
    }
  }

  private IEnumerator JobDemoGradients(float speed)
  {
    amazingAscii.Mode = AsciiModes.Screen;
    amazingAscii.BlendOp = AsciiBlendOps.Solid;

    amazingAscii.Color0 = Color.red;
    amazingAscii.Color1 = Color.blue;

    amazingAscii.Gradient = AsciiGradients.Vertical;
    amazingAscii.GradientVerticalOffset = 0.0f;
    amazingAscii.Charset = AsciiCharsets.Lucida_5x8_94;

    // Fade in.
    while (amazingAscii.Amount < 1.0f)
    {
      amazingAscii.Amount += Time.deltaTime * speed * 2.0f;

      yield return null;
    }

    // Vertical 0 --> 2.
    while (amazingAscii.GradientVerticalOffset < 2.0f)
    {
      amazingAscii.GradientVerticalOffset += Time.deltaTime * speed * 0.75f;

      yield return null;
    }

    // Vertical 2 --> 0.
    while (amazingAscii.GradientVerticalOffset > 0.0f)
    {
      amazingAscii.GradientVerticalOffset -= Time.deltaTime * speed * 0.75f;

      yield return null;
    }

    // Desaturation.
    while (amazingAscii.Saturation > 0.0f)
    {
      amazingAscii.Saturation -= Time.deltaTime * speed * 1.0f;

      yield return null;
    }

    amazingAscii.Color0 = Color.red;
    amazingAscii.Color1 = Color.blue;

    amazingAscii.Gradient = AsciiGradients.Horizontal;
    amazingAscii.GradientHorizontalOffset = 0.0f;

    // Saturation.
    while (amazingAscii.Saturation < 1.0f)
    {
      amazingAscii.Saturation += Time.deltaTime * speed * 1.0f;

      yield return null;
    }

    // Horizontal 0 --> 2.
    while (amazingAscii.GradientHorizontalOffset < 2.0f)
    {
      amazingAscii.GradientHorizontalOffset += Time.deltaTime * speed * 0.75f;

      yield return null;
    }

    // Horizontal 2 --> 0.
    while (amazingAscii.GradientHorizontalOffset > 0.0f)
    {
      amazingAscii.GradientHorizontalOffset -= Time.deltaTime * speed * 0.75f;

      yield return null;
    }

    // Desaturation.
    while (amazingAscii.Saturation > 0.0f)
    {
      amazingAscii.Saturation -= Time.deltaTime * speed * 1.0f;

      yield return null;
    }

    amazingAscii.Color0 = Color.blue;
    amazingAscii.Color1 = Color.red;

    amazingAscii.Gradient = AsciiGradients.Circular;
    amazingAscii.GradientCircularRadius = 0.0f;

    // Saturation.
    while (amazingAscii.Saturation < 1.0f)
    {
      amazingAscii.Saturation += Time.deltaTime * speed * 1.0f;

      yield return null;
    }

    // Circular 0 --> 10.
    while (amazingAscii.GradientCircularRadius < 10.0f)
    {
      amazingAscii.GradientCircularRadius += Time.deltaTime * speed * 10.0f;

      yield return null;
    }

    // Circular 10 --> 0.
    while (amazingAscii.GradientCircularRadius > 0.0f)
    {
      amazingAscii.GradientCircularRadius -= Time.deltaTime * speed * 10.0f;

      yield return null;
    }

    // Fade out.
    while (amazingAscii.Amount > 0.0f)
    {
      amazingAscii.Amount -= Time.deltaTime * speed * 2.0f;

      yield return null;
    }

    amazingAscii.Color0 = amazingAscii.Color1 = Color.white;

    amazingAscii.BlendOp = AsciiBlendOps.Solid;

    currentJob = null;
  }

  private IEnumerator JobDemoDepth(float speed)
  {
    const float depthWidth = 0.15f;

    amazingAscii.Mode = AsciiModes.Depth;
    amazingAscii.BlendOp = AsciiBlendOps.HardLight;

    amazingAscii.ModulationDepthRangeMin = 0.0f;
    amazingAscii.ModulationDepthRangeMax = depthWidth;
    amazingAscii.Charset = AsciiCharsets.TimesNewRoman_11x15_4;

    // Fade in.
    while (amazingAscii.Amount < 1.0f)
    {
      amazingAscii.Amount += Time.deltaTime * speed * 2.0f;

      yield return null;
    }

    // Near --> Far.
    while (amazingAscii.ModulationDepthRangeMax < 0.75f)
    {
      amazingAscii.ModulationDepthRangeMax += Time.deltaTime * speed;
      amazingAscii.ModulationDepthRangeMin = amazingAscii.ModulationDepthRangeMax - depthWidth;

      yield return null;
    }

    amazingAscii.Charset = AsciiCharsets.Lucida_5x8_94;

    // Far --> Near.
    while (amazingAscii.ModulationDepthRangeMax > 0.0f)
    {
      amazingAscii.ModulationDepthRangeMax -= Time.deltaTime * speed;
      amazingAscii.ModulationDepthRangeMin = amazingAscii.ModulationDepthRangeMax - depthWidth;

      yield return null;
    }

    amazingAscii.Amount = 0.0f;
    amazingAscii.BlendOp = AsciiBlendOps.Solid;

    currentJob = null;
  }

  private IEnumerator JobDemoLayer(string layer, float speed, AsciiCharsets charset)
  {
    amazingAscii.Mode = AsciiModes.Layer;
    amazingAscii.BlendOp = AsciiBlendOps.Solid;

    amazingAscii.Layer = 1 << LayerMask.NameToLayer(layer);
    amazingAscii.Charset = charset;

    // Fade in.
    while (amazingAscii.Amount < 1.0f)
    {
      amazingAscii.Amount += Time.deltaTime * speed * 2.0f;

      yield return null;
    }

    // Wait.
    yield return new WaitForSeconds(1.5f);

    // Fade out.
    while (amazingAscii.Amount > 0.0f)
    {
      amazingAscii.Amount -= Time.deltaTime * speed * 2.0f;

      yield return null;
    }

    currentJob = null;
  }

  private IEnumerator JobWait(float seconds)
  {
    yield return new WaitForSeconds(seconds);

    currentJob = null;
  }

  private void DrawLayerMasks()
  {
    GUILayout.BeginVertical(boxStyle);
    {
      GUILayout.Label("Select layers ...");

      if (GUILayout.Button("Nothing") == true)
        amazingAscii.Layer = 0;

      if (GUILayout.Button("Everything") == true)
        amazingAscii.Layer = -1;

      if (GUILayout.Button("Floor") == true)
        amazingAscii.Layer ^= 1 << LayerMask.NameToLayer("Floor");

      if (GUILayout.Button("Walls") == true)
        amazingAscii.Layer ^= 1 << LayerMask.NameToLayer("Walls");

      if (GUILayout.Button("Shapes") == true)
        amazingAscii.Layer ^= 1 << LayerMask.NameToLayer("Shapes");
    }
    GUILayout.EndVertical();
  }

  private Color NiceColor()
  {
    Color source = (Random.value > 0.33f) ? Color.red : ((Random.value > 0.5f) ? Color.green : Color.blue);

    return new Color((Random.value + source.r) * 0.5f, (Random.value + source.g) * 0.5f, (Random.value + source.b) * 0.5f);
  }

  private Texture2D MakeTex(int width, int height, Color col)
  {
    Color[] pix = new Color[width * height];
    for (int i = 0; i < pix.Length; ++i)
      pix[i] = col;

    Texture2D result = new Texture2D(width, height);
    result.SetPixels(pix);
    result.Apply();

    return result;
  }
}
