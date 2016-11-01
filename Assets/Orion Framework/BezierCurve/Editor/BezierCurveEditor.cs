using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BezierObject))]
public class BezierCurveEditor : Editor {
	BezierObject bo;
	Vector2 scroll;

	void OnEnable ()
	{
		bo = (BezierObject)target;
	}

	[MenuItem("GameObject/Create Other/Custom/Bezier Curve", false, 10)]
	static void CreateBezier(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = new GameObject("Bezier Curve");
		BezierObject bo = go.AddComponent<BezierObject>();
		bo.Initialize();
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		//GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	void OnSceneGUI ()
	{
		for (int i = 0; i < bo.Nodes.Count; i++) 
		{
			if(bo.Nodes[i]==null)
			{
				bo.Nodes.Remove(bo.Nodes[i]);
			}
		}
		
		for (int i = 0; i < bo.Nodes.Count; i++) {
			if(i != bo.Nodes.Count-1)
			{
				Transform t = bo.Nodes [i];
				Vector3 pos = (bo.Nodes[i+1].position -t.position) * 0.5f + t.position;
				if(Handles.Button(pos ,Quaternion.identity,0.1f,0.1f,DrawFunc))
				{
					GameObject g = bo.CreateNode(i+1, pos);
					Selection.activeObject = g;
				}
			}
			EditorGUILayout.Separator();
		}
	}

	void DrawFunc(int controlId, Vector3 position, Quaternion rotation, float size)
	{
		Handles.DotCap(controlId, position, rotation, size);
	}
}
