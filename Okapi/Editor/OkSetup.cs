using System;
using System.Text;
using Okapi;
using UnityEditor;
using UnityEngine;

namespace OkapiEditor
{
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
    public const int kState_Setup_8_SetupGameObject = 10;

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
    public static String namespace_;
    public static int gameResolutionWidth;
    public static int gameResolutionHeight;
    public static OkU.GamePlacement gamePlacement;
    public static OkU.DisplayOrientation gameOrientation;
    public static float gameCustomScale;

    public static String sceneName;
    public static int sceneRule;
    public static GiraffeAtlas atlasReference;

    public static String pathResources;
    public static String pathScripts;
    public static String pathImportedAssets;

    private static String tResourcesFolderName;
    private static String tResourcesAbsolutePath;

    private static String tScriptsFolderName;
    private static String tScriptsRelativePath;
    private static String tScriptsAbsolutePath;

    private static String tImportFolderName;
    private static String tImportAbsolutePath;

    private static GameObject tGameObject;
    private static OkOkapi tGame;

    public static void Reset()
    {
      mode = 0;
      name = "MyGame";
      sceneName = "MyGame";
      namespace_ = String.Empty;
      state = "Play";
      gameCustomScale = 1;
      gameResolutionWidth = 960;
      gameResolutionHeight = 640;
      gamePlacement = OkU.GamePlacement.FitToScreenCentred;
      gameOrientation = OkU.DisplayOrientation.Landscape;
      sceneRule = kScene_Create;
      atlasReference = null;
      pathScripts = "Scripts";
      pathResources = "Resources";
      pathImportedAssets = "Imported Assets";
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
          //
          //          if (atlasRule != kAtlas_None)
          //          {
          //            tImportFolderName = "Import";
          //            tImportAbsolutePath = String.Format("{0}/{1}", Application.dataPath, tScriptsFolderName);
          //            if (System.IO.Directory.Exists(tImportAbsolutePath) == false)
          //            {
          //              AssetDatabase.CreateFolder("Assets", tImportFolderName);
          //            }
          //          }

          WaitThen(100, kState_Setup_3_GameMonobehaviour);
        }
        break;
        case kState_Setup_3_GameMonobehaviour:
        {
          SendUpdate(String.Format("Creating the {0} MonoBehaviour", name), 30);

          StringBuilder sb = new StringBuilder(msGameMonoBehaviourSource);

          if (namespace_ == String.Empty)
          {
            sb.Replace("$NSB", String.Empty);
            sb.Replace("$NSE", String.Empty);
          }
          else
          {
            sb.Replace("$NSB", String.Format("namespace {0}\n{\n", namespace_));
            sb.Replace("$NSE", "}");
          }

          sb.Replace("$CLA", name);
          sb.Replace("$STA", state);
          sb.Replace("$SCA", gameCustomScale.ToString());

          String assetPath = String.Format("{0}/{1}.cs", tScriptsAbsolutePath, name);

          System.IO.File.WriteAllText(assetPath, sb.ToString());

          WaitThen(10, kState_Setup_4_State);
        }
        break;
        case kState_Setup_4_State:
        {
          SendUpdate(String.Format("Creating the {0} State", state), 35);

          StringBuilder sb = new StringBuilder(msStateSource);

          if (namespace_ == String.Empty)
          {
            sb.Replace("$NSB", String.Empty);
            sb.Replace("$NSE", String.Empty);
          }
          else
          {
            sb.Replace("$NSB", String.Format("namespace {0}\n{\n", namespace_));
            sb.Replace("$NSE", "}");
          }

          sb.Replace("$CLA", state);

          System.IO.File.WriteAllText(String.Format("{0}/{1}.cs", tScriptsAbsolutePath, state), sb.ToString());

          WaitThen(10, kState_Setup_5_Atlas);
        }
        break;
        case kState_Setup_5_Atlas:
        {
          //          if (atlasRule == kAtlas_New)
          //          {
          //            SendUpdate(String.Format("Creating the {0} atlas", atlasName), 40);
          //            GiraffeAtlasEditor.CreateAtlas(String.Format("Assets/{0}", tResourcesFolderName), atlasName);
          //            Debug.Log("Created atlas");
          //          }

          WaitThen(10, kState_Setup_7_RefreshAssets);
        }
        break;
        case kState_Setup_7_RefreshAssets:
        {
          SendUpdate("Refreshing the Asset database", 45);
          AssetDatabase.Refresh();
          WaitThen(10, kState_Setup_8_SetupGameObject);
        }
        break;
        case kState_Setup_8_SetupGameObject:
        {
          SendUpdate("Setting up the Scene", 50);

          tGameObject = new GameObject("Okapi Game");

          Camera cam = tGameObject.AddComponent<Camera>();
          cam.clearFlags = CameraClearFlags.Nothing;
          cam.depth = 100;
          cam.orthographic = true;

          Giraffe giraffe = tGameObject.AddComponent<Giraffe>();

          tGame = tGameObject.AddComponent<Okapi.OkOkapi>();
          tGame.gameName = name;

          if (atlasReference != null)
          {
            tGame.AddAtlas(atlasReference);
          }

          tGame.customScale = gameCustomScale;
          tGame.resolution = new OkPoint(gameResolutionWidth, gameResolutionHeight);
          tGame.placement = gamePlacement;
          tGame.orientation = gameOrientation;

          EditorUtility.SetDirty(tGameObject);
          EditorUtility.SetDirty(tGame);


          WaitThen(10, kState_Finished);
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

$NSB
public sealed class $CLA : OkGame
{

  public static $CLA Instance
  {
    get;
    private set;
  }

  public $CLA()
  {
    Instance = this;
    Add<$STA>();
    // Any extra game related code you want can go here.
  }

}
$NSE
";

    private static String msStateSource =
      @"using Okapi;
using UnityEngine;

$NSB
public sealed class $CLA : OkState
{
  public $CLA()
  {
    // Your code here.
  }
}
$NSE
";

  }
}