using System;
using Mono.Security.Cryptography;
using Okapi;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace OkapiEditor
{

  public enum OkDeviceType
  {
    None,
    Screen,
    Mobile
  }

  public class OkResolution
  {
    public static GUIContent[] DisplayNames
    {
      get
      {
        if (msNames == null)
        {
          UnityEngine.Resolution[] resolutions = Screen.resolutions;

          int totalCount = msStockResolutions.Length + resolutions.Length;
          msNames = new GUIContent[totalCount];
          msResolutions = new OkResolution[totalCount];

          for (int i = 0; i < msStockResolutions.Length; i++)
          {
            msNames[i] = new GUIContent(msStockResolutions[i].displayName);
            msResolutions[i] = msStockResolutions[i];
          }
          for (int i = 0; i < resolutions.Length; i++)
          {
            Resolution r = resolutions[i];
            OkResolution res = new OkResolution("This Computer", String.Empty, OkDeviceType.Screen, r.width, r.height);
            msNames[i + msStockResolutions.Length] = new GUIContent(res.displayName);
            msResolutions[i + msStockResolutions.Length] = res;
          }
        }
        return msNames;
      }
    }

    public static OkResolution[] Resolutions
    {
      get { return msResolutions; }
    }

    private readonly static OkResolution[] msStockResolutions =
    {
      new OkResolution(String.Empty, "480p", OkDeviceType.Screen, 640,480),
      new OkResolution(String.Empty, "720p", OkDeviceType.Screen, 1280,720),
      new OkResolution(String.Empty, "1080p", OkDeviceType.Screen, 1920,1080),
      new OkResolution(String.Empty, "Flash", OkDeviceType.None, 640, 480),
      new OkResolution(String.Empty, "Common Android and Apple", OkDeviceType.Mobile, 960, 640),
      new OkResolution("PC", "SVGA", OkDeviceType.Screen, 800, 600),
      new OkResolution("PC", "XGA", OkDeviceType.Screen, 1024, 768),
      new OkResolution("PC", "WXGA", OkDeviceType.Screen, 1280, 800),
      new OkResolution("Retro", "Amiga Hires Laced", OkDeviceType.Screen, 640, 512),
      new OkResolution("Retro", "Atari 400, 800", OkDeviceType.Screen, 320, 192),
      new OkResolution("Retro", "Nintendo Entertainment System", OkDeviceType.Screen, 256, 240),
      new OkResolution("Retro", "Sega Genesis", OkDeviceType.Screen, 320, 224),
      new OkResolution("Apple", "iPod Touch 1-3 Gen", OkDeviceType.Mobile, 480, 320),
      new OkResolution("Apple", "iPod Touch 4 Gen", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Apple", "iPhone 3G, 3Gs", OkDeviceType.Mobile, 480, 320),
      new OkResolution("Apple", "iPhone 4, 4S", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Apple", "iPhone 5, 5c, 5S", OkDeviceType.Mobile, 1136, 640),
      new OkResolution("Apple", "iPad 1-2 Gen, Mini", OkDeviceType.Mobile, 1024, 768),
      new OkResolution("Apple", "iPad 3-4 Gen, Air, Mini 2 Gen", OkDeviceType.Mobile, 2048, 1536),
      new OkResolution("Android", "3:2", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Android", "4:3", OkDeviceType.Mobile, 1024, 768),
      new OkResolution("Android", "5:3", OkDeviceType.Mobile, 1280, 768),
      new OkResolution("Android", "16:9", OkDeviceType.Mobile, 1280, 720),
      new OkResolution("Android", "16:10", OkDeviceType.Mobile, 1650, 1050),
    };

    private static OkResolution[] msResolutions;
    private static GUIContent[] msNames;

    protected OkResolution(String group_, String name_, OkDeviceType type_, int width_, int height_)
    {
      width = width_;
      height = height_;
      type = type_;
      name = name_;
      if (String.IsNullOrEmpty(group_))
      {
        if (String.IsNullOrEmpty(name) == false)
          displayName = String.Format("{0} x {1} - {2}", width, height, name);
        else
          displayName = String.Format("{0} x {1}", width, height);
      }
      else
      {
        if (String.IsNullOrEmpty(name) == false)
          displayName = String.Format("{0}/{1} x {2} - {3}", group_, width, height, name);
        else
          displayName = String.Format("{0}/{1} x {2}", group_, width, height);
      }
    }

    public readonly String displayName, name;
    public readonly int width, height;
    public OkDeviceType type;

  }

  public class OkSetupPreview
  {

    enum OkDeviceOrientation
    {
      Landscape,
      Portrait
    }

    private GUIStyle mBackgroundStyle, mToolbarStyle, mToolbarLabelStyle, mToolbarDropdownStyle, mMonitorBezelStyle;
    private OkSetupWizard mWizard;
    public int mDisplayWidth, mDisplayHeight;
    public OkDeviceType mDeviceType;
    private OkDeviceOrientation mDeviceOrientation;

    private float mPreviewMaxWidth, mPreviewMaxHeight;
    private Vector2 mSizeArea;
    private Rect mRectBezel, mRectStand1, mRectStand2, mRectDisplay, mRectGame;
    private float mScreenScale;

    private OkRect mComputedGameRect;
    private OkU.DisplayForcedOrientation mComputedGameOrientation;
    private float mComputedGameScale;
    private bool mComputedGameFits;
    private String mComputedGameComment;

    private static readonly GUIContent kBlankContent = new GUIContent();
    private static readonly Color kGameFitsColour = new Color(1.0f, 1.0f, 1.0f);
    private static readonly Color kGameDoesntFitColour = new Color(1.0f, 0.5f, 0.5f);

    private static Color msBackgroundColour = new Color32(85, 180, 255, 255);
    private static Texture2D msBackgroundTexture, msBackgroundTextureRotated;
    private static Rect msBackgroundCloud = new Rect(0, 64, 160, 64);
    private Vector2 msBackgroundCloudPos = new Vector2(0, 0);
    private static Rect msBackgroundLandPos = new Rect(0, 0, 0, 0);
    private static Rect msBackgroundLandPosUV = new Rect(0, 0, 0, 0);


    public OkSetupPreview(OkSetupWizard wizard)
    {
      mWizard = wizard;
      mDisplayWidth = OkSetup.gameResolutionWidth;
      mDisplayHeight = OkSetup.gameResolutionHeight;
      mDeviceType = OkDeviceType.Mobile;
      mPreviewMaxWidth = OkSetupWizard.kPaneWidth - 10;
      mPreviewMaxHeight = OkSetupWizard.kPaneHeight - 35;

      if (msBackgroundTexture == null)
      {
        CreateTextures();
      }

      Refresh();
    }

    void CreateTextures()
    {
      msBackgroundTexture = new Texture2D(320, 128, TextureFormat.ARGB32, false, true);
      msBackgroundTexture.hideFlags = HideFlags.DontSave;
      msBackgroundTexture.LoadImage(OkSetupData.Landscape);
      msBackgroundTexture.Apply(false, true);

    }

    public void Inspect()
    {

      if (mBackgroundStyle == null)
      {
        mBackgroundStyle = new GUIStyle(EditorStyles.objectFieldThumb);
        mBackgroundStyle.padding = new RectOffset(1, 2, 1, 1);
        mToolbarStyle = new GUIStyle(EditorStyles.toolbar);
        mToolbarStyle.fixedHeight = 25;
        mToolbarStyle.padding = new RectOffset(0, 0, 0, -1);
        mToolbarStyle.margin = new RectOffset(0, 0, 0, -1);

        mToolbarDropdownStyle = new GUIStyle(EditorStyles.toolbarDropDown);
        mToolbarDropdownStyle.fixedHeight = 25;
        mToolbarLabelStyle = new GUIStyle(EditorStyles.toolbarDropDown);
        mToolbarLabelStyle.normal.background = null;
        mToolbarLabelStyle.fixedHeight = 25;

        mMonitorBezelStyle = new GUIStyle(GUI.skin.button);
      }

      GUILayout.Label("Resolution Preview", EditorStyles.boldLabel);

      GUILayout.BeginVertical(mBackgroundStyle);

      GUILayout.BeginHorizontal(mToolbarStyle, GUILayout.Height(25));
      GUILayout.Label("Resolution", mToolbarLabelStyle, GUILayout.Height(25));

      if (mWizard.ResolutionField(ref mDisplayWidth, ref mDisplayHeight, 1))
      {
        mDeviceType = OkDeviceType.None;
        Refresh();
      }

      GUI.changed = false;
      mDeviceOrientation = (OkDeviceOrientation)EditorGUILayout.EnumPopup(mDeviceOrientation, mToolbarDropdownStyle, GUILayout.Height(25), GUILayout.Width(100));
      if (GUI.changed)
      {
        Refresh();
      }

      GUILayout.EndHorizontal();

      GUILayout.EndVertical();

      GUILayout.BeginVertical(mBackgroundStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();

      GUILayout.FlexibleSpace();

      Rect baseRect = GUILayoutUtility.GetRect(mSizeArea.x, mSizeArea.y);

      GUILayout.FlexibleSpace();

      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();

      GUILayout.EndVertical();

      Color guiColour = GUI.color;

      switch (mDeviceType)
      {
        case OkDeviceType.None:
        {
          GUI.color = Color.black;
          GUI.DrawTexture(baseRect, EditorGUIUtility.whiteTexture);
          GUI.color = guiColour;
        }
        break;
        case OkDeviceType.Screen:
        {
          GUI.color = Color.gray;
          GUI.BeginGroup(baseRect);
          GUI.Box(mRectBezel, kBlankContent, mMonitorBezelStyle);
          GUI.Box(mRectStand1, kBlankContent, mMonitorBezelStyle);
          GUI.Box(mRectStand2, kBlankContent, mMonitorBezelStyle);
          GUI.color = Color.black;
          GUI.DrawTexture(mRectDisplay, EditorGUIUtility.whiteTexture);
          GUI.EndGroup();
        }
        break;
        case OkDeviceType.Mobile:
        {
          GUI.color = Color.white;
          GUI.BeginGroup(baseRect);
          GUI.Box(mRectBezel, kBlankContent, mMonitorBezelStyle);
          GUI.color = Color.black;
          GUI.DrawTexture(mRectDisplay, EditorGUIUtility.whiteTexture);
          GUI.EndGroup();
        }
        break;
      }

      GUI.BeginGroup(baseRect);
      GUI.BeginGroup(mRectDisplay);
      GUI.BeginGroup(mRectGame);

      if (mComputedGameFits)
        GUI.color = msBackgroundColour;
      else
        GUI.color = kGameDoesntFitColour;

      GUI.DrawTexture(mRectGame, EditorGUIUtility.whiteTexture);

      if (mComputedGameFits)
        GUI.color = Color.white;
      else
        GUI.color = kGameDoesntFitColour;

      GUI.DrawTextureWithTexCoords(msBackgroundLandPos, msBackgroundTexture, msBackgroundLandPosUV, false);

      GUI.EndGroup();
      GUI.EndGroup();
      GUI.EndGroup();

      GUI.color = guiColour;

      EditorGUI.DropShadowLabel(new Rect(baseRect.x, baseRect.yMax + 5, mSizeArea.x, 35), mComputedGameComment);

    }

    public void Refresh()
    {
      const int kBezel = 4;
      const float kAreaBoundary = 0.75f;
      OkPoint displayResolution = new OkPoint(mDisplayWidth, mDisplayHeight);
      OkPoint gameResolution = new OkPoint(OkSetup.gameResolutionWidth, OkSetup.gameResolutionHeight);

      if (mDeviceOrientation == OkDeviceOrientation.Portrait)
      {
        displayResolution.SwapElements();
      }

      mScreenScale = 1.0f / (displayResolution.MaxElement() / Mathf.Max(mPreviewMaxWidth * kAreaBoundary, mPreviewMaxHeight * kAreaBoundary));

      switch (mDeviceType)
      {
        case OkDeviceType.None:
        {

          OkPoint areaRes = displayResolution * mScreenScale;
          mSizeArea = new Vector2(areaRes.x, areaRes.y);
        }
        break;
        case OkDeviceType.Screen:
        {
          const int kStandHeight = 8;
          const int kStandWidth = kStandHeight * 8;

          float realLifeScale = 1080.0f / ((float)displayResolution.MaxElement());
          float standWidth = Mathf.Min(mPreviewMaxWidth * kAreaBoundary, realLifeScale * kStandWidth);

          OkPoint areaSize = new OkPoint(displayResolution.x, displayResolution.y);


          mScreenScale = 1.0f / (areaSize.MaxElement() / Mathf.Max(mPreviewMaxWidth * kAreaBoundary, mPreviewMaxHeight * kAreaBoundary));

          mSizeArea = new Vector2(areaSize.x, areaSize.y) * mScreenScale;
          mSizeArea.x += kBezel * 2;
          mSizeArea.y += kBezel * 2 + kStandHeight * 2;

          OkPoint bezelSize = new OkPoint(displayResolution.x, displayResolution.y) * mScreenScale;
          bezelSize.x += kBezel * 2;
          bezelSize.y += kBezel * 2;

          mRectBezel = new Rect(0, 0, bezelSize.x, bezelSize.y);

          OkPoint displaySize = new OkPoint(displayResolution.x, displayResolution.y) * mScreenScale;

          mRectDisplay = new Rect(kBezel, kBezel, displaySize.x, displaySize.y);

          mRectStand1 = new Rect(mRectBezel.x + mRectBezel.width / 2 - standWidth / 4, mRectBezel.yMax - 1, standWidth / 2, kStandHeight);
          mRectStand2 = new Rect(mRectBezel.x + mRectBezel.width / 2 - standWidth / 2, mRectStand1.yMax - 1, standWidth, kStandHeight);

        }
        break;
        case OkDeviceType.Mobile:
        {

          OkPoint areaSize = new OkPoint(displayResolution.x, displayResolution.y);

          if (mDeviceOrientation == OkDeviceOrientation.Portrait)
          {
            areaSize.y += displayResolution.y / 8;
          }
          else
          {
            areaSize.x += displayResolution.x / 8;
          }

          mScreenScale = 1.0f / (areaSize.MaxElement() / Mathf.Max(mPreviewMaxWidth * kAreaBoundary, mPreviewMaxHeight * kAreaBoundary));

          mSizeArea = new Vector2(areaSize.x, areaSize.y) * mScreenScale;
          mSizeArea.x += kBezel * 2;
          mSizeArea.y += kBezel * 2;

          OkPoint bezelSize = new OkPoint((int)mSizeArea.x, (int)mSizeArea.y);

          mRectBezel = new Rect(0, 0, bezelSize.x, bezelSize.y);

          OkPoint displaySize = new OkPoint(displayResolution.x, displayResolution.y) * mScreenScale;

          mRectDisplay = new Rect(kBezel, kBezel, displaySize.x, displaySize.y);
        }
        break;
      }

      mComputedGameFits = OkU.ComputeGamePlacementOnDisplay(gameResolution, OkSetup.gameCustomScale, OkSetup.gamePlacement,
        OkSetup.gameOrientation,
        new OkPoint(mDisplayWidth, mDisplayHeight),
        mDeviceOrientation == OkDeviceOrientation.Landscape
          ? OkU.DisplayOrientation.Landscape
          : OkU.DisplayOrientation.Portrait, out mComputedGameRect, out mComputedGameOrientation, out mComputedGameScale
        );

      mRectGame = new Rect(
        mComputedGameRect.x * mScreenScale,
        mComputedGameRect.y * mScreenScale,
        mComputedGameRect.width * mScreenScale,
        mComputedGameRect.height * mScreenScale);

      float k = 128.0f * mComputedGameScale * mScreenScale;

      msBackgroundLandPos = new Rect(
        mRectGame.x,
        mRectGame.y + mRectGame.height - k,
        mRectGame.width,
        k);

      msBackgroundLandPosUV = new Rect(0, 0, (OkSetup.gameResolutionWidth / 320.0f), 1);
      mComputedGameComment = String.Format("Scale: {0}x", mComputedGameScale);
    }

  }

}
