using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BezierNode))]
public class BezierNodeEditor : Editor {
	BezierObject bo;
	Vector2 scroll;
	
	void OnEnable ()
	{
		bo = ((BezierNode)target).Parent;
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

		for (int i = 0; i < bo.Nodes.Count; i++) 
		{
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
