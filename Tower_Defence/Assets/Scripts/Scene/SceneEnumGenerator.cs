#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class SceneEnumGenerator : MonoBehaviour
{
    private const string enumPath = "Assets/Scripts/SceneNames.cs"; // Save path

    [MenuItem("Tools/Generate Scene Enum")]
    public static void GenerateSceneEnum()
    {
        string enumCode = "public enum SceneNames\n{\n";

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
            enumCode += $"\t{sceneName.Replace(" ", "_")},\n";
        }

        enumCode += "}";

        File.WriteAllText(enumPath, enumCode);
        AssetDatabase.Refresh();

        Debug.Log("Scene Enum Updated!");
    }
}
#endif
