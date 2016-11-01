using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BezierObject : MonoBehaviour {
	public List<Transform> Nodes = new List<Transform>();
	public GameObject first;
	public GameObject last;
	GameObject ToMove;
	Camera TestPath;
	public float MoveSpeed = 2;
	public Transform FocusPoint;
	bool moving;
	public float T;
	public int Points = 100;
	public GameObject TestObject;
	public int CurveCount;
	public List<Vector3> nodes = new List<Vector3>();

	public List<Vector3> FinalPath = new List<Vector3>();
	public BezierCurve MovePath = new BezierCurve();

	public List<LookTableValue> LookUpTable = new List<LookTableValue> ();
	public float LookUpIncreaseValue = 0.1f;
	public float ErrorThreshold = 0.05f;

	public List<Vector3> GetPath ()
	{
		//CalculatePath();
		return FinalPath;
	}

	public BezierCurve GetMovePath ()
	{
		//CalculatePath();
		return MovePath;
	}

	void Awake ()
	{
		CalculatePath();
	}

	[ContextMenu("Serialize LookUp Table")]
	public void GenerateLookUpTable ()
	{
		LookUpTable.Clear ();
		float value = 0;
		LookTableValue last = new LookTableValue ();
		while (value < 1) 
		{
			LookTableValue a = new LookTableValue ();
			a.Position = PointOnBezierPath (value);
			if (value > 0)
				a.Distance = Mathf.Abs(a.Position.x) - Mathf.Abs(last.Position.x);
			LookUpTable.Add (a);
			value += LookUpIncreaseValue;
			last = a;
		}
	}

	public float GetYFromLookUpTable (float x)
	{
		for (int i = 0; i < LookUpTable.Count; i++) 
		{
			if ((Mathf.Abs(LookUpTable [i].Position.x) - Mathf.Abs(x)) < ErrorThreshold) 
			{
				Debug.Log ("Index "+i+ " table y "+LookUpTable [i].Position.y);
				return LookUpTable [i].Position.y;
			}
		}
		return 9999;
	}

	List<Vector3> temp = new List<Vector3>();
	public void CalculatePath (int amount = 100)
	{
		nodes.Clear();
		temp = new List<Vector3>();
		for (int i = 0; i < Nodes.Count; i++) {
			if(Nodes [i]!=null)
			{
				nodes.Add(Nodes [i].position);
			}
		}
		MovePath =  BezierPath.GetBezierPath(nodes,out CurveCount);
		FinalPath = BezierPath.GetBezierCurve(nodes,out CurveCount,amount);
		PathLenght = Lenght;
		List<Vector3> temp2 = new List<Vector3>();
		for(float i = 0; i < 1; i += 0.1f)
		{
			for(int x = 0; x < MovePath.Path.Count; x += 4)
			{
				temp2.Clear();
				for(int y = 0; y < 4; y++)
				{
					temp2.Add(MovePath.Path[y + x]);
				}
				temp.Add(PointOnCubicBezier(temp2.ToArray(), i));
			}
		}
	}

	public float PathLenght;

	float Lenght
	{
		get
		{
			float lenght = 0;
			if(FinalPath.Count > 0)
			{
				for (int i = 0; i < FinalPath.Count; i++) {
					if(i!=0)
					{
						lenght+= Vector3.Distance(FinalPath[i],FinalPath[i-1]);
					}
				}
			}
			return lenght;
		}
	}


	public void Initialize ()
	{
		first = Creation(0);
		last = Creation(1);
		first.name = "First Node";
		last.name = "Last Node";
	}

	public GameObject CreateNode (int index)
	{
		return index == 0 ? Creation (1) : Creation (index);
	}

	public GameObject CreateNode (int index, Vector3 pos)
	{
		if(index == 0)
		{
			return Creation(1, pos);
		}
		return Creation(index, pos);
	}

	GameObject Creation (int index)
	{
		GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		g.transform.parent = transform;
		g.name = "Node";
		Nodes.Insert(index,g.transform);
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;
		g.transform.localScale = Vector3.one * 0.1f;
		(g.AddComponent<BezierNode>()).Parent = this;
		return g;
	}

	GameObject Creation (int index, Vector3 pos)
	{
		GameObject g = Creation(index);
		g.transform.position = pos;
		return g;
	}

	void OnDrawGizmos  ()
	{
		if(Nodes!=null && Nodes.Count > 1)
		{
			CalculatePath();
			Vector3 v;
			for(int i = 0; i < FinalPath.Count; i++)
			{
				v = FinalPath[i];
				if(i == 0)
				{
					Gizmos.DrawLine(v, FinalPath[i+1]);
				}
				else
				{
					Gizmos.DrawLine(v,FinalPath[i-1]);
				}
			}
		}

//		Gizmos.color = Color.white;
//		for (int i = 0; i < MovePath.Path.Count; i++)
//		{
//			/Gizmos.DrawWireSphere (MovePath.Path [i], 0.15f);
//		}

		List<Vector3> a = new List<Vector3>();
		for (int i = 0; i < 4; i++) {
			a.Add (MovePath.Path [i]);
		}
//		Vector3 p = Vector3.zero;
//		Gizmos.DrawCube(p,Vector3.one);
	}

	[ContextMenu("TestPathAnimation")]
	public void TestPathAnimation ()
	{
		ToMove = new GameObject();
		ToMove.transform.rotation = Quaternion.identity;
		TestPath = ToMove.AddComponent<Camera>();
		TestPath.depth = 100;
		if(FocusPoint!=null)
		{
			StartCoroutine(MoveAlongPath(ToMove, MoveSpeed, FocusPoint.gameObject, true));
		}
		else
		{
			StartCoroutine(MoveAlongPath(ToMove, MoveSpeed, null, true));
		}

	}

	GameObject ToMoves;
	public IEnumerator MoveAlongPath (GameObject toMove, float speed, GameObject focusPoint = null, bool destroy = false, LeanTweenType easeType = LeanTweenType.linear)
	{
		CalculatePath();
		moving = true;
		ToMoves = toMove;
		if(focusPoint!=null)
		{
			StartCoroutine(CustomUpdate(toMove, focusPoint));
		}
		for(int i = 0; i < CurveCount; i++)
		{
			LeanTween.value(gameObject,asd,0,1,speed/CurveCount);
			yield return new WaitForSeconds(speed/CurveCount);
			index ++;
		}
		moving = false;
	}

	public IEnumerator MoveAlongPathLeanTween (GameObject toMove, float speed, GameObject focusPoint = null)
	{
		CalculatePath();
		moving = true;
		ToMoves = toMove;
		if(focusPoint!=null)
		{
			StartCoroutine(CustomUpdate(toMove, focusPoint));
		}
		LeanTween.move(toMove,MovePath.Path.ToArray(),speed);
		yield return new WaitForSeconds(speed);
		moving = false;
	}

	int index = 0;
	public float Ti;
	void asd(float t)
	{
		Ti = t;
		List<Vector3> temp2 = new List<Vector3>();
		for(int x = 0; x < 4; x++)
		{
			temp2.Add(MovePath.Path[x + (index * 4)]); 
		}
		ToMoves.transform.position = PointOnCubicBezier(temp2.ToArray(), t);
	}

	IEnumerator CustomUpdate (GameObject toMove, GameObject focusPoint)
	{
		while(moving)
		{
			if(toMove!=null && focusPoint!=null)
			{				
				//toMove.transform.rotation = Quaternion.Lerp(toMove.transform.rotation, Quaternion.LookRotation(focusPoint.transform.position - toMove.transform.position), 1 * Time.deltaTime);
				toMove.transform.LookAt(focusPoint.transform);
			}
			yield return null;
		}
	}

	public Vector3 PointOnBezierPath (float t)
	{
		t = Mathf.Clamp(t,0,1);
		CalculatePath();
		if(t > 0 && t < 1)
		{
			float value = 1.0f/(float)CurveCount;
			float result = t/value;
			int counter = Mathf.FloorToInt(result);
			float finalT = result-counter;
			List<Vector3> temp2 = new List<Vector3>();
			int j;
			for(int x = 0; x < 4; x++)
			{
				j = x + counter * 4;
				temp2.Add(MovePath.Path[j]);
			}
			return PointOnCubicBezier(temp2.ToArray(),finalT);
		}
		return t.Equals (1) ? MovePath.Path [MovePath.Path.Count - 1] : MovePath.Path [0];
	}

	public static Vector3 PointOnCubicBezier( Vector3[] cp, float t )
	{
		float   ax, bx, cx;
		float   ay, by, cy;
		float   az, bz, cz;
		float   tSquared, tCubed;
		Vector3 result;
		
		/* cálculo de los coeficientes polinomiales */
		
		cx = 3.0f * (cp[1].x - cp[0].x);
		bx = 3.0f * (cp[2].x - cp[1].x) - cx;
		ax = cp[3].x - cp[0].x - cx - bx;
		
		cy = 3.0f * (cp[1].y - cp[0].y);
		by = 3.0f * (cp[2].y - cp[1].y) - cy;
		ay = cp[3].y - cp[0].y - cy - by;

		cz = 3.0f * (cp[1].z - cp[0].z);
		bz = 3.0f * (cp[2].z - cp[1].z) - cz;
		az = cp[3].z - cp[0].z - cz - bz;
		
		/* calculate the curve point at parameter value t */
		
		tSquared = t * t;
		tCubed = tSquared * t;
		
		result.x = (ax * tCubed) + (bx * tSquared) + (cx * t) + cp[0].x;
		result.y = (ay * tCubed) + (by * tSquared) + (cy * t) + cp[0].y;
		result.z = (az * tCubed) + (bz * tSquared) + (cz * t) + cp[0].z;
		
		return result;
	}
	

}

[System.Serializable]
public struct LookTableValue
{
	public Vector3 Position;
	public float Distance;
}
