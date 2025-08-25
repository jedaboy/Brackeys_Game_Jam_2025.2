using GRD.FSM;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class PlayFromLaunch
{
    private const string menuItem = "GRD/Play from launch";
    private const string settingsMenuItem = "GRD/Play from launch settings";

    public static bool playFromLaunchScene
    {
        get { return EditorPrefs.GetBool(menuItem, false); }
        set { EditorPrefs.SetBool(menuItem, value); }
    }
    public static string launchScenePath
    {
        get { return EditorPrefs.GetString("lauchScenePath", ""); }
        set { EditorPrefs.SetString("lauchScenePath", value); }
    }

    static PlayFromLaunch() 
    {
        SetDefaultScene();

        EditorApplication.delayCall += () =>
        {
            Menu.SetChecked(menuItem, playFromLaunchScene);
        };
    }

    public static void SetDefaultScene() 
    {
        SceneAsset defaultStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(launchScenePath);
        if (playFromLaunchScene && !defaultStartScene) 
        {
            Debug.LogError($"A cena Launch não foi encontrada em {launchScenePath}");
        }

        Menu.SetChecked(menuItem, playFromLaunchScene);
        EditorSceneManager.playModeStartScene = playFromLaunchScene ?
            defaultStartScene : null;
    }

    [MenuItem(menuItem)]
    private static void PlayFromLaunchToggle()
    {
        playFromLaunchScene = !playFromLaunchScene;
        PlayFromLaunch.SetDefaultScene();
    }

    [MenuItem(settingsMenuItem)]
    private static void OpenPlayFromLauchWindow() 
    {
        PlayFromLaunchEditorWindow.OpenWindow();
    }
}

public class PlayFromLaunchEditorWindow : EditorWindow
{
    private Toggle _playFromLaunchToggle;
    private ObjectField _sceneField;

    public static void OpenWindow()
    {
        PlayFromLaunchEditorWindow window = GetWindow<PlayFromLaunchEditorWindow>();
        window.titleContent = new GUIContent("Play From Launch");
    }

    private void CreateGUI() 
    {
        VisualElement root = rootVisualElement;

        _playFromLaunchToggle = new Toggle("Play from Launch");
        _playFromLaunchToggle.value = PlayFromLaunch.playFromLaunchScene;
        root.Add(_playFromLaunchToggle);

        _sceneField = new ObjectField("Launch Scene");
        _sceneField.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(PlayFromLaunch.launchScenePath);
        root.Add(_sceneField);

        Button saveButton = new Button(Save);
        saveButton.name = "Save";
        saveButton.text = "Save";
        root.Add(saveButton);
    }

    private void Save()
    {
        PlayFromLaunch.playFromLaunchScene = _playFromLaunchToggle.value;
        PlayFromLaunch.launchScenePath = AssetDatabase.GetAssetOrScenePath(_sceneField.value);
        PlayFromLaunch.SetDefaultScene();
    }
}
