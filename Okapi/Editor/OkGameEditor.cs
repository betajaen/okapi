using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Okapi
{

  internal class OkapiGameMaker : EditorWindow
  {
    static void Menu_SetupOkapiGame()
    {
      var win = EditorWindow.GetWindow<OkapiGameMaker>(true);
      win.title = "Okapi";
      Vector2 size = new Vector2(400, 300);
      win.maxSize = size;
      win.minSize = size;
      win.position = new Rect(Screen.currentResolution.width / 2 - size.x / 2, Screen.currentResolution.height / 2 - size.y / 2, size.x, size.y);
    }

    internal enum AtlasCreationRule
    {
      Create,
      Use,
      None
    }

    private static GUIStyle msHeadingText;
    private static String[] msScaleNames = new String[] { "1x", "2x", "4x", "8x" };
    private static int[] msScaleValues = new int[] { 1, 2, 4, 8 };
    private static String[] msSceneNames = new string[] { "This Scene", "New Scene" };


    private String mSceneName = "MyGame";
    private int mSceneCreationRule;
    private String mGameName = "MyGame";
    private String mStateName = "Play";
    private int mScale = 1;
    private int mScaleIndex = 1;
    private AtlasCreationRule mUseOrCreateAtlas = AtlasCreationRule.Create;
    private String mCreateAtlasName = "Art";
    private GiraffeAtlas mUseAtlas;

    OkapiGameMaker()
    {
    }

    void OnGUI()
    {
      if (msHeadingText == null)
      {
        msHeadingText = new GUIStyle(EditorStyles.largeLabel);
        msHeadingText.fontSize = 24;
        msHeadingText.alignment = TextAnchor.MiddleCenter;
      }

      GUILayout.BeginVertical();
      GUILayout.Label("Okapi Game Maker", msHeadingText);
      GUILayout.Space(16);

      GUILayout.Label("Scene");
      EditorGUI.indentLevel++;
      mSceneCreationRule = EditorGUILayout.Popup("Where", mSceneCreationRule, msSceneNames);
      if (mSceneCreationRule == 1)
      {
        mSceneName = EditorGUILayout.TextField("Name", mSceneName);
      }

      mScaleIndex = EditorGUILayout.Popup("Scale", mScaleIndex, msScaleNames);
      mScale = msScaleValues[mScaleIndex];

      EditorGUI.indentLevel--;

      GUILayout.Label("Scripts", EditorStyles.boldLabel);
      EditorGUI.indentLevel++;
      mGameName = EditorGUILayout.TextField("Name", mGameName);
      mStateName = EditorGUILayout.TextField("Main State", mStateName);


      EditorGUI.indentLevel--;

      GUILayout.Label("Atlas", EditorStyles.boldLabel);
      EditorGUI.indentLevel++;

      mUseOrCreateAtlas = (AtlasCreationRule)EditorGUILayout.EnumPopup("Atlas", mUseOrCreateAtlas);

      switch (mUseOrCreateAtlas)
      {
        case AtlasCreationRule.Create:
        {
          if (mCreateAtlasName == String.Empty)
          {
            mCreateAtlasName = "Art";
          }
          mCreateAtlasName = EditorGUILayout.TextField("Name", mCreateAtlasName);
        }
        break;
        case AtlasCreationRule.Use:
        {
          mUseAtlas = EditorGUILayout.ObjectField("Asset", mUseAtlas,
            typeof(GiraffeAtlas), false) as GiraffeAtlas;
        }
        break;
      }

      EditorGUI.indentLevel--;

      GUILayout.EndVertical();


      GUILayout.BeginHorizontal();
      bool doCreate = GUILayout.Button("Create");
      GUILayout.EndHorizontal();

      if (doCreate)
      {
        String scene = String.Empty;
        if (mSceneCreationRule == 1)
          scene = mSceneName;

        switch (mUseOrCreateAtlas)
        {
          case AtlasCreationRule.Create:
          OkGameEditor.CreateOkapiFiles(scene, mGameName, mStateName, mScale, mUseOrCreateAtlas, mCreateAtlasName);
          break;
          case AtlasCreationRule.Use:
          OkGameEditor.CreateOkapiFiles(scene, mGameName, mStateName, mScale, mUseOrCreateAtlas, mUseOrCreateAtlas);
          break;
          case AtlasCreationRule.None:
          OkGameEditor.CreateOkapiFiles(scene, mGameName, mStateName, mScale, mUseOrCreateAtlas, null);
          break;
        }
        Close();
      }

    }

  }

  [CustomEditor(typeof(OkGame))]
  internal class OkGameEditor : Editor
  {

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

    public static void CreateOkapiFiles(String scene, String gameName, String stateName, int scale,
      OkapiGameMaker.AtlasCreationRule createAtlas, object atlas)
    {

      EditorUtility.DisplayProgressBar("Setting up your game", "Initialising Scene", 0.0f);

      if (scene != String.Empty)
      {
        EditorApplication.NewScene();
        bool result = EditorApplication.SaveScene(String.Format("Assets/{0}.unity", scene));
        UnityEngine.Object.DestroyImmediate(GameObject.Find("Main Camera"));
      }

      EditorUtility.DisplayProgressBar("Setting up your game", "Creating any folders", 0.15f);

      // Create Folders
      String resourcesFolderName = "Resources";
      String scriptsFolderName = "Scripts"; // Or Source (if it exists)

      if (System.IO.Directory.Exists(String.Format("{0}/{1}", Application.dataPath, resourcesFolderName)) == false)
      {
        AssetDatabase.CreateFolder("Assets", "Resources");
      }

      EditorUtility.DisplayProgressBar("Setting up your game", "Writing out your scripts", 0.3f);

      // Create the source folder, it's either called Scripts or Source, depending on the user. Scripts by default
      // Find a 'Source' folder first, otherwise find/create Scripts folder.
      if (System.IO.Directory.Exists(String.Format("{0}/{1}", Application.dataPath, "Source")))
      {
        scriptsFolderName = "Source";
      }
      else if (System.IO.Directory.Exists(String.Format("{0}/{1}", Application.dataPath, scriptsFolderName)) == false)
      {
        AssetDatabase.CreateFolder("Assets", scriptsFolderName);
      }

      String scriptsFullPath = String.Format("{0}/{1}", Application.dataPath, scriptsFolderName);
      String gameSource = msGameMonoBehaviourSource.Replace("$", gameName);
      gameSource = gameSource.Replace("^", stateName);
      gameSource = gameSource.Replace("*", scale.ToString());

      String stateSource = msStateSource.Replace("$", stateName);

      System.IO.File.WriteAllText(String.Format("{0}/{1}.cs", scriptsFullPath, gameName), gameSource);
      System.IO.File.WriteAllText(String.Format("{0}/{1}.cs", scriptsFullPath, stateName), stateSource);

      EditorUtility.DisplayProgressBar("Setting up your game", "Compiling your scripts", 0.4f);
      gn = gameName;
      EditorApplication.update += TestUpdate;

    }

    private static String gn;

    private static void TestUpdate()
    {
      if (EditorApplication.isCompiling == false)
      {
        Debug.Log(gn);
        EditorUtility.ClearProgressBar();
        EditorApplication.update -= TestUpdate;

        //        EditorUtility.DisplayProgressBar("Setting up your game", "Setting up the scene", 0.5f);
        //        CreateOkapiScene("MyGame", "Play");
        //        Debug.Log("done.");
      }
    }

    static void CreateOkapiScene(String gameName, String stateName)
    {

      EditorUtility.DisplayProgressBar("Setting up your game", "Setting up the scene", 0.5f);
      // Create Game Object
      GameObject gameObject = new GameObject("Okapi Game");
      Camera camera = gameObject.AddComponent<Camera>();
      Giraffe giraffe = gameObject.AddComponent<Giraffe>();

      // Configure GameObject
      gameObject.isStatic = true;

      // Configure camera
      camera.clearFlags = CameraClearFlags.Nothing;
      camera.cullingMask = 0;
      camera.nearClipPlane = 0.0f;
      camera.farClipPlane = 10.0f;
      camera.depth = 100;
      camera.orthographic = true;

      gameObject.AddComponent(gameName);

      Selection.activeGameObject = gameObject;

      EditorApplication.SaveScene();
      EditorApplication.SaveAssets();

      EditorUtility.ClearProgressBar();

    }


  }
}