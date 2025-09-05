using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MenuTools
{
    [MenuItem("OnTarget/Play Game")]
    public static void PlayGameFromGameLoader()
    {
        string currentSceneName = "";
        currentSceneName = EditorSceneManager.GetActiveScene().name;

        File.WriteAllText(".lastScene", currentSceneName);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene($"{Directory.GetCurrentDirectory()}/Assets/Scenes/GameManager.unity");

        EditorApplication.isPlaying = true;
    }

    [MenuItem("OnTarget/Load Last Edited Scene")]
    public static void ReturnToLastScene()
    {
        string lastScene = File.ReadAllText(".lastScene");
        
        string scenesPath = $"{Directory.GetCurrentDirectory()}/Assets/Scenes";
        string scenePath = FindSceneFile(scenesPath, lastScene);
        
        if (!string.IsNullOrEmpty(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
        }
        else
        {
            Debug.LogError($"Scene '{lastScene}' not found in {scenesPath} or its subfolders");
        }
    }
    
    private static string FindSceneFile(string directory, string sceneName)
    {
        string[] sceneFiles = Directory.GetFiles(directory, "*.unity", SearchOption.AllDirectories);
        
        foreach (string sceneFile in sceneFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(sceneFile);
            if (fileName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return sceneFile;
            }
        }
        
        return null;
    }
}
