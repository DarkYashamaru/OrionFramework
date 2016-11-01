using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BezierObject))]
public class BezierInstancier : MonoBehaviour {
	public GameObject Prefab;
	public List<GameObject> ObjectList = new List<GameObject>();
	BezierObject Bezier
	{
		get {
			if (bezier == null)
				bezier = GetComponent<BezierObject> ();
			return bezier;
		}
	}
	BezierObject bezier;
	public int Amount = 5;
	GameObject temp;

	[ContextMenu("Instantiate")]
	public void Instance ()
	{
		DestroyOld ();
		ObjectList.Clear ();
		Bezier.CalculatePath (Amount);
		for (int i = 0; i < Bezier.FinalPath.Count; i++) 
		{
			temp = (GameObject)Instantiate (Prefab, Bezier.FinalPath [i], Prefab.transform.rotation);
			ObjectList.Add (temp);
			CommonMethods.AgrupateTemporalObjects(temp);
		}
	}

	void DestroyOld ()
	{
		for (int i = 0; i < ObjectList.Count; i++) 
		{
			#if UNITY_EDITOR
				DestroyImmediate(ObjectList [i]);
			#else
				Destroy(ObjectList [i]);
			#endif
		}
	}
}
