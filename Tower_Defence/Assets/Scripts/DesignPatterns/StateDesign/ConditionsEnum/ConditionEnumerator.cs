using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ConditionEnumerator
{
    const string fileName = "TransitionConditions";
    const string fileExtensions = ".cs";

    static string[] conditions;
    static string previousEnumText;
    static string pathToThis = string.Empty;

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        // Dynamically fetch unique conditions from all TransitionSO assets
        conditions = GetUniqueConditions();

        // Get the path to this script
        GetPathToThis();

        // Combine the directory path with the file name to get the full file path
        string filePath = Path.Combine(pathToThis, fileName + fileExtensions);

        // Combine the whole enum string
        string enumText = "public static class TransitionConditions\n{\n" + GetEnumMembersString() + "}";

        // Save only if there are changes
        if (previousEnumText != enumText)
        {
            File.WriteAllText(filePath, enumText);
            previousEnumText = enumText;
            Debug.Log("Updated Enumerated Transition Conditions");
        }
    }

    static string[] GetUniqueConditions()
    {
        // Find all TransitionSO assets in the project
        string[] guids = AssetDatabase.FindAssets("t:TransitionSO");

        HashSet<string> uniqueConditions = new HashSet<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TransitionSO transition = AssetDatabase.LoadAssetAtPath<TransitionSO>(path);

            if (transition != null)
            {
                uniqueConditions.Add(transition.condition); // Add the condition string
            }
        }

        return uniqueConditions.ToArray();
    }

    static void GetPathToThis()
    {
        if (pathToThis.Length == 0)
        {
            string thisName = "ConditionEnumerator.cs";

            string[] res = Directory.GetFiles(Application.dataPath, thisName, SearchOption.AllDirectories);

            if (res.Length == 0)
            {
                Debug.LogError("Couldn't find path to ConditionEnumerator");
            }
            else
            {
                pathToThis = res[0].Replace(thisName, "").Replace("\\", "/");
            }
        }
    }

    static string GetEnumMembersString()
    {
        string members = "    public enum Condition\n    {\n";

        for (int i = 0; i < conditions.Length; i++)
        {
            members += "        " + ClearSpecialCharacters(conditions[i]) + ",\n";
        }

        members += "    }\n";
        return members;
    }

    static string ClearSpecialCharacters(string str)
    {
        string pattern = "[^a-zA-Z0-9]";
        string replacement = "_";
        string result = Regex.Replace(str, pattern, replacement);

        return result;
    }
}
#endif
