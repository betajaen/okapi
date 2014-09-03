using System;
using System.Text;
using System.Threading;
using Okapi;
using UnityEditor;
using UnityEngine;

namespace OkapiEditor
{

  public class OkSetupAssetPostProcessor : AssetPostprocessor
  {

    public static String assetName;
    public static bool assetNameLoaded;

    static void OnPostprocessAllAssets(String[] imported, String[] deleted, String[] moved, String[] moved2)
    {
      if (OkSetup.mode != OkSetup.kState_None)
      {
        foreach (var str in imported)
        {
          Debug.Log(str + " vs " + assetName);
          if (str == assetName)
          {
            assetNameLoaded = true;
          }
        }
      }
    }
  }

  public static class OkSetup
  {

    public const int kState_None = 0;
    public const int kState_SetupGUI = 1;
    public const int kState_OkapiOrdered = 2;
    public const int kState_WaitTimer = 3;
    public const int kState_Setup_1_Scene = 4;
    public const int kState_Setup_2_Folders = 5;
    public const int kState_Setup_3_GameMonobehaviour = 6;
    public const int kState_Setup_4_State = 7;
    public const int kState_Setup_5_Atlas = 8;
    public const int kState_Setup_7_RefreshAssets = 9;
    public const int kState_Setup_8_WaitForScriptsToCompile = 10;

    public const int kState_Finished = 1000;

    public const int kScene_UseThis = 0;
    public const int kScene_Create = 1;
    public const int kAtlas_None = 1;
    public const int kAtlas_New = 1;
    public const int kAtlas_Use = 2;

    public static int mode;
    public static int nextMode;
    public static int modeTimer;
    public static int modeTimerLimit;

    public static String name;
    public static String state;
    public static int scale;
    public static String sceneName;
    public static int sceneRule;
    public static int atlasRule;
    public static String atlasName;
    public static GiraffeAtlas atlasReference;

    private static String tResourcesFolderName;
    private static String tResourcesAbsolutePath;

    private static String tScriptsFolderName;
    private static String tScriptsRelativePath;
    private static String tScriptsAbsolutePath;

    private static String tImportFolderName;
    private static String tImportAbsolutePath;

    private static GameObject tGameObject;

    public static void Reset()
    {
      mode = 0;
      name = "MyGame";
      sceneName = "MyGame";
      state = "Play";
      scale = 1;
      sceneRule = kScene_UseThis;
      atlasRule = kAtlas_New;
      atlasName = "Art";
      atlasReference = null;

      OkSetupAssetPostProcessor.assetName = String.Empty;
      OkSetupAssetPostProcessor.assetNameLoaded = false;

    }

    public static void Begin()
    {
      EditorApplication.update += EditorUpdate;
      modeTimerLimit = 90000;
    }

    public static void End()
    {
      EditorApplication.update -= EditorUpdate;
    }

    private static void EditorUpdate()
    {
      modeTimerLimit--;

      if (modeTimerLimit <= 0)
      {
        if (EditorUtility.DisplayDialog("Okapi",
          "This Wizard is taking an unusually long time. Do you wish to stop? There may be errors?", "Yes - Stop",
          "No - I'll wait a bit longer"))
        {
          mode = kState_None;
          End();
          Debug.Log("Forced Stop of the Okapi Wizard");
        }
        else
        {
          modeTimerLimit = 160000;
        }
      }

      switch (mode)
      {
        case kState_OkapiOrdered:
        {
          SendUpdate("Setting up", 5);
          modeTimerLimit = 160000;
          WaitThen(200, kState_Setup_1_Scene);
        }
        break;
        case kState_WaitTimer:
        {
          modeTimer--;
          if (modeTimer <= 0)
          {
            mode = nextMode;
            nextMode = kState_None;
          }
        }
        break;
        case kState_Setup_1_Scene:
        {
          if (sceneRule == kScene_UseThis)
          {
            SendUpdate("Saving the scene", 10);
            EditorApplication.SaveScene();
            Debug.Log("Done Scene.");
            WaitThen(200, kState_Setup_2_Folders);
          }
          else if (sceneRule == kScene_Create)
          {
            SendUpdate("Creating the scene", 10);
            EditorApplication.NewScene();
            UnityEngine.Object.DestroyImmediate(GameObject.Find("Main Camera"));
            EditorApplication.SaveScene(String.Format("Assets/{0}.unity", name));
            Debug.Log("Done Scene.");
            WaitThen(200, kState_Setup_2_Folders);
          }
        }
        break;
        case kState_Setup_2_Folders:
        {
          SendUpdate("Creating any necessary folders", 20);

          tResourcesFolderName = "Resources";
          tScriptsFolderName = "Scripts";

          tResourcesAbsolutePath = String.Format("{0}/{1}", Application.dataPath, tResourcesFolderName);

          if (System.IO.Directory.Exists(tResourcesAbsolutePath) == false)
          {
            AssetDatabase.CreateFolder("Assets", tResourcesFolderName);
          }

          // If 'Source' exists use that, otherwise use or create 'Scripts'
          String altScriptsName = "Source";

          if (System.IO.Directory.Exists(String.Format("{0}/{1}", Application.dataPath, "Source")))
          {
            tScriptsFolderName = "Source";
          }
          else
          {
            tScriptsFolderName = "Scripts";
            if (System.IO.Directory.Exists(String.Format("{0}/{1}", Application.dataPath, tScriptsFolderName)) == false)
            {
              AssetDatabase.CreateFolder("Assets", tScriptsFolderName);
            }
          }

          tScriptsRelativePath = String.Format("Assets/{0}", tScriptsFolderName);
          tScriptsAbsolutePath = String.Format("{0}/{1}", Application.dataPath, tScriptsFolderName);

          if (atlasRule != kAtlas_None)
          {
            tImportFolderName = "Import";
            tImportAbsolutePath = String.Format("{0}/{1}", Application.dataPath, tScriptsFolderName);
            if (System.IO.Directory.Exists(tImportAbsolutePath) == false)
            {
              AssetDatabase.CreateFolder("Assets", tImportFolderName);
            }
          }

          WaitThen(100, kState_Setup_3_GameMonobehaviour);
        }
        break;
        case kState_Setup_3_GameMonobehaviour:
        {
          SendUpdate(String.Format("Creating the {0} MonoBehaviour", name), 30);

          StringBuilder sb = new StringBuilder(msGameMonoBehaviourSource);
          sb.Replace("$", name);
          sb.Replace("^", state);
          sb.Replace("*", scale.ToString());

          String assetPath = String.Format("{0}/{1}.cs", tScriptsAbsolutePath, name);
          OkSetupAssetPostProcessor.assetName = String.Format("{0}/{1}.cs", tScriptsRelativePath, name);
          OkSetupAssetPostProcessor.assetNameLoaded = false;

          System.IO.File.WriteAllText(assetPath, sb.ToString());

          WaitThen(10, kState_Setup_4_State);
        }
        break;
        case kState_Setup_4_State:
        {
          SendUpdate(String.Format("Creating the {0} State", state), 35);

          StringBuilder sb = new StringBuilder(msStateSource);
          sb.Replace("$", state);

          System.IO.File.WriteAllText(String.Format("{0}/{1}.cs", tScriptsAbsolutePath, state), sb.ToString());

          WaitThen(10, kState_Setup_5_Atlas);
        }
        break;
        case kState_Setup_5_Atlas:
        {
          if (atlasRule == kAtlas_New)
          {
            SendUpdate(String.Format("Creating the {0} atlas", atlasName), 40);
            GiraffeAtlasEditor.CreateAtlas(String.Format("Assets/{0}", tResourcesFolderName), atlasName);
            Debug.Log("Created atlas");
          }

          WaitThen(10, kState_Setup_7_RefreshAssets);
        }
        break;
        case kState_Setup_7_RefreshAssets:
        {
          SendUpdate("Refreshing the Asset database", 45);
          AssetDatabase.Refresh();
          WaitThen(10, kState_Setup_8_WaitForScriptsToCompile);
        }
        break;
        case kState_Setup_8_WaitForScriptsToCompile:
        {
          SendUpdate("Waiting for scripts to update", 50);

          if (OkSetupAssetPostProcessor.assetNameLoaded)
          {
            Debug.Log("Attempting Creation");

            EditorUtility.ClearProgressBar();
            End();
            mode = kState_None;

            var s = MonoImporter.GetAllRuntimeMonoScripts();

            foreach (var d in s)
            {
              if (d.name == name)
              {
                Type type = d.GetClass();

                tGameObject = new GameObject("Okapi Game", type);
                return;
              }
            }
          }

          //
          //          var s = MonoImporter.GetAllRuntimeMonoScripts();
          //          foreach (var d in s)
          //          {
          //            if (d.name == name)
          //            {
          //              MonoImporter.SetExecutionOrder(d, MonoImporter.GetExecutionOrder(d));
          //              Type type = d.GetClass();
          //
          //              WaitThen(10, kState_Finished);
          //              tGameObject = new GameObject("Okapi Game", type);
          //              return;
          //            }
          //          }

        }
        break;
        case kState_Finished:
        {
          EditorUtility.ClearProgressBar();
          End();
          mode = kState_None;
        }
        break;
      }
    }

    static void WaitThen(int length, int thenDo)
    {
      modeTimer = length;
      mode = kState_WaitTimer;
      nextMode = thenDo;
      modeTimerLimit = length * 10;
    }

    static void SendUpdate(String message, int percent)
    {
      EditorUtility.DisplayProgressBar("Okapi", message, percent / 100.0f);
    }

    private static String msGameMonoBehaviourSource =
@"using Okapi;

public class $ : OkGame
{
  void Start()
  {
    SetupGame(*, typeof(^));
  }
}
";

    private static String msStateSource =
@"using Okapi;
using UnityEngine;

public class $ : OkState
{
  public $()
  {
    // Your code here.
  }
}
";

  }

  public class OkWizard : EditorWindow
  {
    [MenuItem("Okapi/Setup Okapi Game")]
    static void Menu_SetupOkapiGame()
    {
      OkSetup.Reset();
      OkSetup.mode = OkSetup.kState_SetupGUI;
      msScaleIndex = 0;

      var win = EditorWindow.GetWindow<OkWizard>(true);
      win.title = "Okapi";
      Vector2 size = new Vector2(400, 300);
      win.maxSize = size;
      win.minSize = size;
      win.position = new Rect(Screen.currentResolution.width / 2 - size.x / 2, Screen.currentResolution.height / 2 - size.y / 2, size.x, size.y);
    }

    private static GUIStyle msHeadingText;
    private static readonly String[] msScaleNames = new String[] { "1x", "2x", "4x", "8x" };
    private static readonly int[] msScaleValues = new int[] { 1, 2, 4, 8 };
    private static readonly String[] msSceneNames = new string[] { "This Scene", "New Scene" };
    private static int msScaleIndex = 0;

    private void OnGUI()
    {
      if (msHeadingText == null)
      {
        msHeadingText = new GUIStyle(EditorStyles.largeLabel);
        msHeadingText.fontSize = 24;
        msHeadingText.alignment = TextAnchor.MiddleCenter;
      }

      GUILayout.BeginVertical();
      GUILayout.Label("Okapi Game Maker", msHeadingText);


      GUILayout.Label("Scene");
      EditorGUI.indentLevel++;

      OkSetup.sceneRule = EditorGUILayout.Popup("Where", OkSetup.sceneRule, msSceneNames);
      if (OkSetup.sceneRule == 1)
      {
        OkSetup.sceneName = EditorGUILayout.TextField("Name", OkSetup.sceneName);
      }

      msScaleIndex = EditorGUILayout.Popup("Scale", msScaleIndex, msScaleNames);
      OkSetup.scale = msScaleValues[msScaleIndex];

      EditorGUI.indentLevel--;

      GUILayout.EndVertical();
      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Create"))
      {
        OkSetup.mode = OkSetup.kState_OkapiOrdered;
        OkSetup.Begin();
        Close();
      }

      GUILayout.EndHorizontal();


    }

  }


}
