using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Okapi;
using UnityEditor;
using UnityEngine;

namespace OkapiEditor
{
  public class OkSetupWizard : EditorWindow
  {

    private static Vector2 msWindowSize = new Vector2(400, 580);
    private static Vector2 msScreenPreviewWindowSize = new Vector2(800, 580);
    public const int kPaneWidth = 390;
    public const int kPaneHeight = 490;

    [MenuItem("Okapi/Setup Okapi Game")]
    static void Menu_SetupOkapiGame()
    {
      OkSetup.Reset();
      OkSetup.mode = OkSetup.kState_SetupGUI;

      var win = EditorWindow.GetWindow<OkSetupWizard>(true);
      win.title = "Okapi Setup Wizard";
      win.maxSize = msWindowSize;
      win.minSize = msWindowSize;
      win.position = new Rect(Screen.currentResolution.width / 2 - msWindowSize.x / 2, Screen.currentResolution.height / 2 - msWindowSize.y / 2, msWindowSize.x, msWindowSize.y);

      win.resolutionPreview = true;
      win.ShowPreview();
      win.RefreshPreview();
    }

    private OkSetupPreview mScreenPreview;

    private void OnGUI()
    {
      InspectHeading();
      EditorGUILayout.BeginHorizontal(GUILayout.Height(kPaneHeight));
      {
        EditorGUILayout.BeginVertical(GUILayout.Width(kPaneWidth));
        {
          InspectSetup();
        }
        EditorGUILayout.EndVertical();
        if (mScreenPreview != null)
        {
          EditorGUILayout.BeginVertical(GUILayout.Width(kPaneWidth));
          {
            mScreenPreview.Inspect();
          }
          EditorGUILayout.EndVertical();
        }
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.BeginHorizontal(GUILayout.Width(kPaneWidth));
      {
        InspectApply();
      }
      EditorGUILayout.EndHorizontal();
    }


    private static GUIStyle msHeadingText;

    void InspectHeading()
    {
      if (msHeadingText == null)
      {
        msHeadingText = new GUIStyle(EditorStyles.largeLabel);
        msHeadingText.fontSize = 24;
        msHeadingText.alignment = TextAnchor.MiddleCenter;
      }
      GUILayout.Label("Okapi Game Maker", msHeadingText, GUILayout.Width(kPaneWidth));
    }

    private static readonly String[] kSceneNames = { "This Scene", "New Scene" };

    private static readonly String[] kCustomScaleNames = { "1x", "1.5x", "2x", "3x", "4x", "8x" };
    private static readonly float[] kCustomScaleValues = { 1, 1.5f, 2, 3.0f, 4, 8 };

    private bool resolutionPreview;
    private int customScaleIndex;

    void InspectSetup()
    {
      GUILayout.Label("Game", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        OkSetup.name = CSharpSymbolField("Name", OkSetup.sceneName);

        OkSetup.state = CSharpSymbolField("Main State", OkSetup.state);

        OkSetup.namespace_ = CSharpSymbolField("Namespace", OkSetup.namespace_);

        EditorGUI.indentLevel--;
      }

      EditorGUILayout.Space();

      GUILayout.Label("Scene", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        OkSetup.sceneRule = EditorGUILayout.Popup("Where", OkSetup.sceneRule, kSceneNames);
        if (OkSetup.sceneRule == 0)
          GUI.enabled = false;

        OkSetup.sceneName = EditorGUILayout.TextField("Name", OkSetup.sceneName);

        GUI.enabled = true;

        EditorGUI.indentLevel--;
      }

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Resolution", EditorStyles.boldLabel);
      GUILayout.FlexibleSpace();
      GUI.changed = false;
      resolutionPreview = GUILayout.Toggle(resolutionPreview, "Live Preview", EditorStyles.miniButton);
      if (GUI.changed)
      {
        if (resolutionPreview)
        {
          ShowPreview();
          RefreshPreview();
        }
        else
        {
          HidePreview();
        }
      }
      GUILayout.EndHorizontal();
      {
        EditorGUI.indentLevel++;

        // Resolution Width and Resolution Height

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Size");
        int indentlevel = EditorGUI.indentLevel;
        GUILayout.BeginHorizontal();
        EditorGUI.indentLevel = 0;

        if (ResolutionField(ref OkSetup.gameResolutionWidth, ref OkSetup.gameResolutionHeight, 0))
        {
          RefreshPreview();
        }

        EditorGUI.indentLevel = indentlevel;
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();


        // Resolution Placement

        GUI.changed = false;
        OkSetup.gamePlacement = (OkU.GamePlacement)EditorGUILayout.EnumPopup("Placement", OkSetup.gamePlacement);
        if (GUI.changed && resolutionPreview)
        {
          RefreshPreview();
        }

        // Resolution Custom Scale

        if (OkSetup.gamePlacement > OkU.GamePlacement.FixedScaleCentred)
        {
          GUI.enabled = false;
        }

        GUI.changed = false;
        customScaleIndex = EditorGUILayout.Popup("Custom Scale", customScaleIndex, kCustomScaleNames);
        if (GUI.changed)
        {
          OkSetup.gameCustomScale = kCustomScaleValues[customScaleIndex];
          if (resolutionPreview)
          {
            RefreshPreview();
          }
        }

        GUI.enabled = true;

        EditorGUI.indentLevel--;
      }

      EditorGUILayout.Space();

      GUILayout.Label("Atlas", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        OkSetup.atlasReference = EditorGUILayout.ObjectField("Atlas", OkSetup.atlasReference, typeof(GiraffeAtlas), false) as GiraffeAtlas;
        if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(35)))
        {
          String path = EditorUtility.SaveFilePanelInProject("Create an Atlas", "Art", "asset",
            "Create an Giraffe Atlas", "/Assets/Resources");

          StringBuilder sb = new StringBuilder(path);
          sb.Replace(Application.dataPath, String.Empty);
          sb.Replace(Path.GetFileName(path), String.Empty);
          if (sb[sb.Length - 1] == '/')
          {
            sb.Remove(sb.Length - 1, 1);
          }

          String folderPath = sb.ToString();
          String assetName = Path.GetFileNameWithoutExtension(path);

          OkSetup.atlasReference = GiraffeAtlasEditor.CreateAtlas(folderPath, assetName);

        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
      }

      EditorGUILayout.Space();


      GUILayout.Label("Directories", EditorStyles.boldLabel);
      {
        EditorGUI.indentLevel++;

        OkSetup.pathScripts = FolderField("Scripts", OkSetup.pathScripts, "Scripts");

        OkSetup.pathResources = FolderField("Resources", OkSetup.pathResources, "Resources");

        OkSetup.pathImportedAssets = FolderField("Imported Assets", OkSetup.pathImportedAssets, "Import");

        EditorGUI.indentLevel--;
      }

    }

    void InspectApply()
    {
      if (GUILayout.Button("Cancel"))
      {
        OkSetup.mode = OkSetup.kState_None;
        Close();
      }

      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Create"))
      {
        OkSetup.mode = OkSetup.kState_OkapiOrdered;
        OkSetup.Begin();
        Close();
      }
    }


    public static String CSharpSymbolField(String name, String symbol, params GUILayoutOption[] options)
    {
      GUI.changed = false;
      symbol = EditorGUILayout.TextField(name, symbol, options);

      if (GUI.changed)
      {
        symbol = Regex.Replace(symbol, @"[^a-zA-Z0-9]", "");
      }

      return symbol;
    }

    public static String FolderField(String name, String path, String defaultName)
    {
      EditorGUILayout.BeginHorizontal();
      path = EditorGUILayout.TextField(name, path);
      if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(35)))
      {
        String newPath = EditorUtility.SaveFolderPanel("Choose a folder for " + name, path, defaultName);

        if (String.IsNullOrEmpty(newPath) == false && newPath != path)
        {
          try
          {
            path = newPath.Substring(Application.dataPath.Length + 1);
          }
          catch (Exception)
          {
            path = String.Empty;
          }
        }
      }
      EditorGUILayout.EndHorizontal();
      return path;
    }

    private static GUIStyle msResolutionFieldStyle;
    private static float msResolutionFieldWidth;
    private static float msResolutionXWidth;

    public bool ResolutionField(ref int width, ref int height, int dropDownReference)
    {
      if (msResolutionFieldStyle == null)
      {
        msResolutionFieldStyle = new GUIStyle(EditorStyles.label);
        msResolutionFieldStyle.imagePosition = ImagePosition.TextOnly;
        msResolutionXWidth = (int)GUI.skin.label.CalcSize(new GUIContent("x")).x;
      }
      bool changed = false;

      int indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;

      GUILayout.BeginHorizontal(EditorStyles.textArea);

      GUI.changed = false;

      int widthWidth = (int)EditorStyles.label.CalcSize(new GUIContent(width.ToString())).x;

      width = EditorGUILayout.IntField(width, EditorStyles.label, GUILayout.Width(widthWidth));
      changed |= GUI.changed;

      GUILayout.Label("\u00D7", GUILayout.Width(msResolutionXWidth));

      GUI.changed = false;

      height = EditorGUILayout.IntField(height, EditorStyles.label, GUILayout.ExpandWidth(true));
      changed |= GUI.changed;

      GUI.changed = false;

      GUIContent content = new GUIContent("\u25bc");
      Rect dropDownRect = GUILayoutUtility.GetRect(content, msResolutionFieldStyle);

      if (GUI.Button(dropDownRect, content, msResolutionFieldStyle))
      {
        mDropDownFieldCallbackIndex = dropDownReference;
        EditorUtility.DisplayCustomMenu(dropDownRect, OkResolution.DisplayNames, -1, OnResolutionFieldCallback, null);
      }

      GUILayout.EndHorizontal();

      EditorGUI.indentLevel = indent;
      return changed;

    }

    private int mDropDownFieldCallbackIndex;

    private void OnResolutionFieldCallback(object userData, string[] options, int selected)
    {
      if (selected >= 0 && selected < OkResolution.Resolutions.Length)
      {
        OkResolution res = OkResolution.Resolutions[selected];
        switch (mDropDownFieldCallbackIndex)
        {
          case 0:
          {
            OkSetup.gameResolutionWidth = res.width;
            OkSetup.gameResolutionHeight = res.height;
          }
          break;
          case 1:
          {
            mScreenPreview.mDisplayWidth = res.width;
            mScreenPreview.mDisplayHeight = res.height;
          }
          break;
        }
        RefreshPreview();
      }
    }

    void ShowPreview()
    {
      mScreenPreview = new OkSetupPreview(this);
      this.maxSize = msScreenPreviewWindowSize;
      this.minSize = msScreenPreviewWindowSize;
    }

    void HidePreview()
    {
      mScreenPreview = null;
      this.maxSize = msWindowSize;
      this.minSize = msWindowSize;
    }

    void RefreshPreview()
    {
      if (mScreenPreview != null)
      {
        mScreenPreview.Refresh();
      }
    }


  }


}
