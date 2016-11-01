using UnityEngine;
using System.Collections;

public class BezierNode : MonoBehaviour {
	[HideInInspector]
	public BezierObject Parent;

	void Start ()
	{
		if(GetComponent<Renderer>()!=null){GetComponent<Renderer>().enabled = false;}
		if(GetComponent<Collider>()!=null){	GetComponent<Collider>().enabled = false;}
	}
}
