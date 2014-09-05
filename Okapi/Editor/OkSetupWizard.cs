using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Okapi;
using UnityEditor;
using UnityEngine;

namespace OkapiEditor
{
  public class OkSetupWizard : EditorWindow
  {
    [MenuItem("Okapi/Setup Okapi Game")]
    static void Menu_SetupOkapiGame()
    {
      if (msResolutionPickerNames == null || msResolutionPickerNames.Length == 0)
      {
        msResolutionPickerNames = new String[msResolutions.Length];
        for (int i = 0; i < msResolutions.Length; i++)
        {
          msResolutionPickerNames[i] = msResolutions[i].name;
        }
      }

      OkSetup.Reset();
      OkSetup.mode = OkSetup.kState_SetupGUI;
      msScaleIndex = 0;

      var win = EditorWindow.GetWindow<OkSetupWizard>(true);
      win.title = "Okapi";
      win.maxSize = msWindowSize;
      win.minSize = msWindowSize;
      win.position = new Rect(Screen.currentResolution.width / 2 - msWindowSize.x / 2, Screen.currentResolution.height / 2 - msWindowSize.y / 2, msWindowSize.x, msWindowSize.y);

      win.ToggleScreenSimulator(); // temp
      win.previewResolutionIndex = -1;
      win.RefreshPreview();
    }
    private static Vector2 msWindowSize = new Vector2(400, 580);
    private static GUIStyle msHeadingText, msPicker, msToolbarPicker, msSimulationArea, msDeviceArea;
    private static readonly String[] msScaleNames = new String[] { "1x", "2x", "4x", "8x" };
    private static readonly int[] msScaleValues = new int[] { 1, 2, 4, 8 };
    private static readonly String[] msSceneNames = new string[] { "This Scene", "New Scene" };
    private static int msScaleIndex = 0;
    private static Texture2D msPreviewTexture;

    private static String[] msResolutionPickerNames;

    private static readonly String[] msScaleRules =
    {
      "Fixed Scale",
      "Scale to Display",
    };

    class Resolution
    {
      public const int kDevice_None = 0;
      public const int kDevice_Screen = 1;
      public const int kDevice_Mobile = 2;

      public Resolution(String deviceName_, int deviceType_, int w_, int h_, int scale_)
      {
        deviceName = deviceName_;
        deviceType = deviceType_;

        w = w_;
        h = h_;
        scale = scale_;

        if (scale == 1)
          name = String.Format("{0} -- {1} x {2}", deviceName, w, h);
        else
          name = String.Format("{0} -- {1} x {2} -- {3}x Scale", deviceName, w, h, scale);

      }

      public String deviceName, name;
      public int w, h, scale, deviceType;
    }

    private static readonly Resolution[] msResolutions = 
    {
      // All figures from: http://gamedevelopment.tutsplus.com/articles/quick-tip-what-is-the-best-screen-resolution-for-your-game--gamedev-14723
      new Resolution("480p", Resolution.kDevice_Screen, 640,480, 1),
      new Resolution("720p", Resolution.kDevice_Screen, 1280,720,1),
      new Resolution("1080p", Resolution.kDevice_Screen, 1920,1080,1),
      new Resolution("1080p", Resolution.kDevice_Screen,  1920, 1080, 2),
      new Resolution("NES", Resolution.kDevice_None, 256, 240,1),
      new Resolution("NES", Resolution.kDevice_None, 256, 240, 2),
      new Resolution("Flash", Resolution.kDevice_Screen, 640, 480,1),
      new Resolution("Flash", Resolution.kDevice_Screen, 640, 480, 2),
      new Resolution("iPhone 3G,3Gs", Resolution.kDevice_Mobile, 480, 320,1),
      new Resolution("iPod Touch 1-3 Gen", Resolution.kDevice_Mobile, 480, 320,1),
      new Resolution("iPhone 4,4S", Resolution.kDevice_Mobile, 960, 640, 1),
      new Resolution("iPod Touch 4 Gen", Resolution.kDevice_Mobile, 960, 640, 1),
      new Resolution("iPhone 5, 5c, 5S", Resolution.kDevice_Mobile, 1136, 640, 1),
      new Resolution("iPad 1-2 Gen, Mini", Resolution.kDevice_Mobile, 1024, 768, 1),
      new Resolution("iPad 3-4 Gen, Air, Mini 2 Gen", Resolution.kDevice_Mobile, 2048, 1536, 1),
      new Resolution("iPad 3-4 Gen, Air, Mini 2 Gen", Resolution.kDevice_Mobile, 2048, 1536, 2),
      new Resolution("Android 3:2", Resolution.kDevice_Mobile, 960, 640, 1),
      new Resolution("Android 4:3", Resolution.kDevice_Mobile, 1024, 768, 1),
      new Resolution("Android 5:3", Resolution.kDevice_Mobile, 1280, 768, 1),
      new Resolution("Android 16:9", Resolution.kDevice_Mobile, 1280, 720, 1),
      new Resolution("Android 16:10", Resolution.kDevice_Mobile, 1650, 1050, 1),
      new Resolution("Common Android and Apple", Resolution.kDevice_Mobile, 960, 640, 1),
      new Resolution("Common Android and Apple", Resolution.kDevice_Mobile, 960, 640, 2),
    };

    void RefreshScaleIndex()
    {
      for (int i = 0; i < msScaleValues.Length; i++)
      {
        if (msScaleValues[i] == OkSetup.gameScale)
        {
          msScaleIndex = i;
          return;
        }
      }
      msScaleIndex = 0;
    }

    private Vector2 scroll;
    private bool screenSimulator;

    private void OnGUI()
    {
      if (msHeadingText == null)
      {
        msHeadingText = new GUIStyle(EditorStyles.largeLabel);
        msHeadingText.fontSize = 24;
        msHeadingText.alignment = TextAnchor.MiddleCenter;
        msPicker = new GUIStyle(EditorStyles.popup);
        msPicker.imagePosition = ImagePosition.ImageOnly;
        msToolbarPicker = new GUIStyle(EditorStyles.toolbarPopup);
        msToolbarPicker.imagePosition = ImagePosition.ImageOnly;
        msSimulationArea = new GUIStyle(EditorStyles.objectFieldThumb);
        msDeviceArea = new GUIStyle(GUI.skin.button);
        msDeviceArea.imagePosition = ImagePosition.ImageOnly;
      }

      if (msPreviewTexture == null)
      {
        msPreviewTexture = new Texture2D(16, 16);
        msPreviewTexture.hideFlags = HideFlags.DontSave;
        Color c0 = new Color(0, 255, 255);
        Color c1 = new Color(255, 0, 255);
        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) msPreviewTexture.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) msPreviewTexture.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) msPreviewTexture.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) msPreviewTexture.SetPixel(x, y, c1);
        msPreviewTexture.Apply();
        msPreviewTexture.filterMode = FilterMode.Point;
      }

      GUILayout.Label("Okapi Game Maker", msHeadingText);

      GUILayout.BeginHorizontal();

      scroll = GUILayout.BeginScrollView(scroll, GUILayout.MaxWidth(msWindowSize.x));

      InspectScene();
      GUILayout.Space(25);

      InspectScreen();

      GUILayout.EndScrollView();

      if (screenSimulator)
      {
        InspectScreenSimulator();
      }

      GUILayout.EndHorizontal();

      GUILayout.Space(25);

      GUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins, GUILayout.Height(50));

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Create"))
      {
        OkSetup.mode = OkSetup.kState_OkapiOrdered;
        OkSetup.Begin();
        Close();
      }
      GUILayout.Space(25);

      GUILayout.EndHorizontal();
      GUILayout.EndHorizontal();

    }

    void InspectScene()
    {
      GUILayout.Label("Scene", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        OkSetup.sceneRule = EditorGUILayout.Popup("Where", OkSetup.sceneRule, msSceneNames);
        if (OkSetup.sceneRule == 1)
        {
          OkSetup.sceneName = EditorGUILayout.TextField("Name", OkSetup.sceneName);
        }

        EditorGUI.indentLevel--;
      }
    }

    void InspectScreen()
    {
      GUILayout.Label("Game Area", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        GUI.changed = false;

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Size");
        int indentlevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        GUILayout.BeginHorizontal();

        GUI.changed = false;
        OkSetup.gameResolutionWidth = EditorGUILayout.IntField(OkSetup.gameResolutionWidth);
        if (GUI.changed && screenSimulator)
        {
          RefreshPreview();
        }

        GUI.changed = false;

        OkSetup.gameResolutionHeight = EditorGUILayout.IntField(OkSetup.gameResolutionHeight);

        if (GUI.changed && screenSimulator)
        {
          RefreshPreview();
        }

        GUI.changed = false;

        int picker = EditorGUILayout.Popup(0, msResolutionPickerNames, msPicker, GUILayout.Width(20));

        if (GUI.changed)
        {
          var resolution = msResolutions[picker];
          OkSetup.gameResolutionWidth = resolution.w;
          OkSetup.gameResolutionHeight = resolution.h;
          OkSetup.gameScale = resolution.scale;
          RefreshScaleIndex();
          if (screenSimulator)
          {
            RefreshPreview();
          }
        }

        GUILayout.EndHorizontal();

        EditorGUI.indentLevel = indentlevel;
        GUILayout.EndHorizontal();

        GUI.changed = false;
        OkSetup.gamePlacement = (OkU.GamePlacement)EditorGUILayout.EnumPopup("Placement", OkSetup.gamePlacement);
        if (GUI.changed && screenSimulator)
        {
          RefreshPreview();
        }

        if (OkSetup.gamePlacement <= OkU.GamePlacement.FixedScaleCentred)
        {
          GUI.changed = false;
          msScaleIndex = EditorGUILayout.Popup("Scale", msScaleIndex, msScaleNames);
          if (GUI.changed)
          {
            OkSetup.gameScale = msScaleValues[msScaleIndex];
            if (screenSimulator)
            {
              RefreshPreview();
            }
          }
        }

        GUI.changed = false;
        OkSetup.gameOrientation = (OkU.DisplayOrientation)EditorGUILayout.EnumPopup("Orientation", OkSetup.gameOrientation);
        if (GUI.changed && screenSimulator)
        {
          RefreshPreview();
        }

        if (GUILayout.Button("Simulator"))
        {
          ToggleScreenSimulator();
        }

        EditorGUI.indentLevel--;
      }
    }

    void ToggleScreenSimulator()
    {
      screenSimulator = !screenSimulator;

      float w = msWindowSize.x;
      if (screenSimulator)
      {
        w *= 2;
      }

      this.maxSize = new Vector2(w, msWindowSize.y);
      this.minSize = new Vector2(w, msWindowSize.y);
      RefreshPreview();
    }

    private int previewResolutionIndex = -1;
    private OkPoint previewDisplayResolution = new OkPoint(1280, 720);
    private OkU.DisplayOrientation previewDisplayOrientation = OkU.DisplayOrientation.Landscape;
    private String previewDisplayName;
    private Resolution previewCurrentResolution;
    private int previewAnimationOffset;

    private int previewDisplayType;
    private Rect previewAreaRect, previewDrawDeviceRect, previewDrawDisplayRect, previewGameRect, previewDrawButtonRect, previewDrawStand1Rect, previewDrawStand2Rect;
    private bool previewDrawDevice, previewDrawButton, previewDrawStand;
    private Color previewButtonColour, previewDeviceColour;

    private bool previewGameFits;
    private float previewGameScale;
    private bool previewGameMentionScale;
    private float previewTextureSizeX;
    private float previewTextureSizeY;

    private OkRect previewGameComputedRect;
    private float previewComputedGameScale;
    private String previewComment;

    void RefreshPreview()
    {
      if (previewResolutionIndex != -1)
      {
        previewCurrentResolution = msResolutions[previewResolutionIndex];
        previewDisplayResolution.x = previewCurrentResolution.w;
        previewDisplayResolution.y = previewCurrentResolution.h;
        previewDisplayType = previewCurrentResolution.deviceType;
      }
      else
      {
        previewDisplayType = Resolution.kDevice_None;
      }

      RefreshPreviewDrawRects();
      RefreshPreviewString();
    }

    void RefreshPreviewDrawRects()
    {

      const int kDevicePadding = 4;

      float drawHeight = msWindowSize.y - 120;
      float drawWidth = msWindowSize.x - 20;
      float screenScale = 0.0f;

      OkPoint displayRes = new OkPoint(previewDisplayResolution.x, previewDisplayResolution.y);
      OkPoint gameRes = new OkPoint(OkSetup.gameResolutionWidth, OkSetup.gameResolutionHeight);

      OkPoint screenAreaRes, // Total Area
              screenDeviceRes, // Main physical white/black plastic area,
              screenDisplayRes, // Black screen
              screenGameRes;  // Area on the black screen which is the game

      switch (previewDisplayType)
      {
        // Embedded WebPlayer, Application or anything else;  Non-physical display.
        case Resolution.kDevice_None:
        {
          previewDrawDevice = false;
          previewDrawButton = false;

          if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
          {
            displayRes.SwapElements();
            gameRes.SwapElements();
          }

          screenScale = 1.0f / (displayRes.MaxElement() / Mathf.Max(drawWidth * 0.75f, drawHeight * 0.75f - 60));

          screenAreaRes = displayRes * screenScale;
          screenGameRes = displayRes * screenScale;

          previewDrawDisplayRect = previewAreaRect = new Rect(0, 0, screenAreaRes.x, screenAreaRes.y);

        }
        break;
        // TV/Computer monitor.
        case Resolution.kDevice_Screen:
        {
          previewDrawDevice = true;
          previewDrawButton = true;
          previewDrawStand = true;

          previewButtonColour = Color.green;
          previewDeviceColour = Color.gray;

          if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
          {
            displayRes.SwapElements();
            gameRes.SwapElements();
          }

          screenScale = 1.0f / (displayRes.MaxElement() / Mathf.Max(drawWidth * 0.75f, drawHeight * 0.75f - 60));

          screenDisplayRes = displayRes * screenScale;
          screenDeviceRes = screenDisplayRes;

          previewAreaRect = new Rect(0, 0, screenDeviceRes.x + kDevicePadding * 2, screenDeviceRes.y + kDevicePadding * 2 + 8 + 6);
          previewDrawDeviceRect = new Rect(0, 0, screenDeviceRes.x + kDevicePadding * 2, screenDeviceRes.y + kDevicePadding * 2);
          previewDrawDisplayRect = new Rect(kDevicePadding, kDevicePadding, screenDeviceRes.x, screenDeviceRes.y);

          if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
          {
            previewDrawButtonRect = new Rect(2, kDevicePadding + screenDeviceRes.y - 2, 1, 2);
            previewDrawStand1Rect = new Rect(previewDrawDeviceRect.width / 2 - 6, previewDrawDeviceRect.height - 1, 12, 8);
            previewDrawStand2Rect = new Rect(previewDrawDeviceRect.width / 2 - 24, previewDrawDeviceRect.height - 1 + 6, 48, 6);
          }
          else
          {
            previewDrawButtonRect = new Rect(kDevicePadding + screenDeviceRes.x - 2, kDevicePadding * 2 + screenDeviceRes.y - 3, 2, 1);
            previewDrawStand1Rect = new Rect(previewDrawDeviceRect.width / 2 - 6, previewDrawDeviceRect.height - 1, 12, 8);
            previewDrawStand2Rect = new Rect(previewDrawDeviceRect.width / 2 - 24, previewDrawDeviceRect.height - 1 + 6, 48, 6);
          }


        }
        break;
        // Tablet/Mobile Phone
        case Resolution.kDevice_Mobile:
        {
          previewDrawDevice = true;
          previewDrawButton = true;
          previewDrawStand = false;

          previewButtonColour = Color.grey;
          previewDeviceColour = Color.white;

          if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
          {
            displayRes.SwapElements();
            gameRes.SwapElements();
          }

          screenScale = 1.0f / (displayRes.MaxElement() / Mathf.Max(drawWidth * 0.75f, drawHeight * 0.75f - 60));

          screenDisplayRes = displayRes * screenScale;
          screenDeviceRes = screenDisplayRes;

          previewAreaRect = new Rect(0, 0, screenDeviceRes.x + kDevicePadding * 6, screenDeviceRes.y + kDevicePadding * 6);
          previewDrawDisplayRect = new Rect(kDevicePadding, kDevicePadding, screenDeviceRes.x, screenDeviceRes.y);

          if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
          {
            previewDrawDeviceRect = new Rect(0, 0, screenDeviceRes.x + kDevicePadding * 2, screenDeviceRes.y + kDevicePadding * 6);
            previewDrawButtonRect = new Rect(kDevicePadding + previewDrawDisplayRect.width / 2 - 6, kDevicePadding + previewDrawDisplayRect.height + kDevicePadding * 2.5f - 6, 12, 12);
          }
          else
          {
            previewDrawDeviceRect = new Rect(0, 0, screenDeviceRes.x + kDevicePadding * 6, screenDeviceRes.y + kDevicePadding * 2);
            previewDrawButtonRect = new Rect(kDevicePadding + previewDrawDisplayRect.width + kDevicePadding * 2.5f - 6, kDevicePadding + previewDrawDisplayRect.height / 2 - 6, 12, 12);
          }
        }
        break;
      }

      OkU.DisplayOrientation gameOrientation;

      previewGameFits = OkU.ComputeScreenBoundary(new OkPoint(OkSetup.gameResolutionWidth, OkSetup.gameResolutionHeight),
        OkSetup.gameScale, OkSetup.gamePlacement, OkSetup.gameOrientation, previewDisplayResolution, previewDisplayOrientation, out previewGameComputedRect,
        out gameOrientation, out previewComputedGameScale);

      previewGameRect = new Rect(previewGameComputedRect.x * screenScale, previewGameComputedRect.y * screenScale, previewGameComputedRect.width * screenScale, previewGameComputedRect.height * screenScale);
    }

    void RefreshPreviewString()
    {
      StringBuilder sb = new StringBuilder(40);
      sb.AppendLine(String.Format("{0} x {1} ({2}x)", previewGameComputedRect.width, previewGameComputedRect.height, previewComputedGameScale));

      if (previewDisplayOrientation == OkU.DisplayOrientation.Portrait)
        sb.AppendLine("(Portrait)");
      else
        sb.AppendLine("(Landscape)");

      previewComment = sb.ToString();
    }

    void InspectScreenSimulator()
    {
      previewAnimationOffset++;

      float drawHeight = msWindowSize.y - 120;
      float drawWidth = msWindowSize.x - 20;

      float scale = OkSetup.gameScale;
      float invScale = 1.0f / scale;

      GUILayout.BeginVertical(msSimulationArea,
        GUILayout.Width(drawWidth), GUILayout.MinWidth(drawWidth), GUILayout.MaxWidth(drawWidth),
        GUILayout.Height(drawHeight), GUILayout.MinHeight(drawHeight), GUILayout.MaxHeight(drawHeight));

      GUILayout.BeginHorizontal(EditorStyles.toolbar);

      EditorGUILayout.PrefixLabel("Display Resolution", EditorStyles.miniLabel);

      GUI.changed = false;
      previewDisplayResolution.x = EditorGUILayout.IntField(previewDisplayResolution.x, EditorStyles.toolbarTextField);
      if (GUI.changed)
      {
        previewResolutionIndex = -1;
        RefreshPreview();
      }
      GUILayout.Label(" \u00D7 ");

      GUI.changed = false;
      previewDisplayResolution.y = EditorGUILayout.IntField(previewDisplayResolution.y, EditorStyles.toolbarTextField);
      if (GUI.changed)
      {
        previewResolutionIndex = -1;
        RefreshPreview();
      }

      GUILayout.Label("px", EditorStyles.miniLabel);

      GUI.changed = false;
      previewResolutionIndex = EditorGUILayout.Popup(previewResolutionIndex, msResolutionPickerNames, msToolbarPicker, GUILayout.Width(25));
      if (GUI.changed)
      {
        RefreshPreview();
      }

      if (GUILayout.Button(previewDisplayOrientation == OkU.DisplayOrientation.Portrait ? "Portrait" : "Landscape", EditorStyles.toolbarButton, GUILayout.Width(65)))
      {
        previewDisplayOrientation = OkU.SwapDisplayOrientation(previewDisplayOrientation);
        RefreshPreview();
      }

      GUILayout.EndHorizontal();


      // Get a centered base rect
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      Rect baseRect = GUILayoutUtility.GetRect(previewAreaRect.width, previewAreaRect.height);
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();

      GUI.BeginGroup(baseRect);

      Color guiColour = GUI.color;
      if (previewDrawDevice)
      {
        GUI.color = previewDeviceColour;
        GUI.Box(previewDrawDeviceRect, String.Empty, msDeviceArea);

        if (previewDrawStand)
        {
          GUI.Box(previewDrawStand1Rect, String.Empty, msDeviceArea);
          GUI.Box(previewDrawStand2Rect, String.Empty, msDeviceArea);
        }

        GUI.color = previewButtonColour;
        GUI.DrawTexture(previewDrawButtonRect, EditorGUIUtility.whiteTexture);
      }

      GUI.color = Color.black;
      GUI.DrawTexture(previewDrawDisplayRect, EditorGUIUtility.whiteTexture);


      GUI.BeginGroup(new Rect(previewDrawDisplayRect.x + previewGameRect.x, previewDrawDisplayRect.y + previewGameRect.y, previewGameRect.width, previewGameRect.height));
      GUI.color = Color.white;
      GUI.DrawTexture(new Rect(0, 0, previewGameRect.width, previewGameRect.height), msPreviewTexture);
      GUI.EndGroup();

      GUI.color = guiColour;
      GUI.EndGroup();

      EditorGUI.DropShadowLabel(new Rect(baseRect.x, baseRect.yMax, baseRect.width, 48), previewComment);


      GUILayout.EndVertical();
    }

  }


}
