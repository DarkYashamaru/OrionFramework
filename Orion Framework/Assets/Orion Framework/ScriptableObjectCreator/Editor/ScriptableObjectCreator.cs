using UnityEngine;
using UnityEditor;
using System.IO;

public class ScriptableObjectCreator : EditorWindow
{
	string Name;
	string Path;

	[MenuItem("Window/Create Scriptable Object")]
	public static void ScriptableObjectWindow()
	{
		GetWindow<ScriptableObjectCreator>();

	}
	
	public void OnGUI()
	{
		Name = EditorGUILayout.TextField("Name",Name);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Path");
		if(GUILayout.Button("Browse"))
		{
			Path = EditorUtility.OpenFolderPanel("","","");
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Label(Path);
		if(GUILayout.Button("Create"))
		{
			CreateScriptableObject();
		}
	}

	void CreateScriptableObject()
	{
		if(!string.IsNullOrEmpty(Name))
		{
			string finalPath = "Assets";

			if(!string.IsNullOrEmpty( Path))
			{
				string[] split = Path.Split(new string[]{"Assets"},System.StringSplitOptions.None);
				if(split.Length > 1 && !string.IsNullOrEmpty(split[1]))
				{
					finalPath += split[1] + "/";
					Debug.Log(finalPath);
				}
				else
				{
					finalPath += "/";
					EditorUtility.DisplayDialog("Alerta","Invalid path, creating in assets","OK");
				}
			}
			else
			{
				finalPath += "/";
				EditorUtility.DisplayDialog("Alerta","Invalid path, creating in assets","OK");
			}
			ScriptableObject asset = ScriptableObject.CreateInstance<ScriptableObject>();
			AssetDatabase.CreateAsset(asset, finalPath + Name + ".asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
		else
		{
			EditorUtility.DisplayDialog("Alerta","Please assign a name", "OK");
		}
	}

}





























