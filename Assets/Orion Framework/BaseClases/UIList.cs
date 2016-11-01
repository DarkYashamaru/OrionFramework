using UnityEngine;
using System.Collections.Generic;

public class UIList : MonoBehaviour {
	public GameObject Prefab;
	public GameObject Content;
	GameObject temp;
	UIListElement element;
	public List<UIListElement> List = new List<UIListElement>();

	public void Initialize<T> (object[] list ) where T : Component, UIListElement
	{
		DestroyAll();
		List.Clear();
		for(int i = 0; i < list.Length; i++)
		{
			if(list[i]!=null)
			{
				temp = Instantiate(Prefab);
				temp.transform.SetParent(Content.transform);
				temp.transform.ResetLocalTransform();
				temp.AddComponent<T>();
				element = temp.GetComponent<UIListElement>();
				element.Activate(list[i], i);
				element.Refresh();
				List.Add(element);
			}
		}
		Prefab.SetActive(false);
	}

	public void Initialize (object[] list )
	{
		Initialize<BasicElement>(list);
	}

	public void DestroyAll ()
	{
		if(List!=null)
		{
			for(int i = 0; i < List.Count; i++)
			{
				List[i].Destroy();
			}	
		}
	}

	public void Refresh ()
	{
		for(int i = 0; i < List.Count; i++)
		{
			List[i].Refresh();
		}
	}

	public T[] GetComponentsFromList<T> ()  where T : Component
	{
		T[] components = new T[List.Count];
		for(int i = 0; i < List.Count; i++)
		{
			components[i] = List[i].GetElementComponent<T>();
		}
		return components;
	}
}

public interface UIListElement
{
	event System.Action Enabled;
	event System.Action<object> EnabledData;
	event System.Action Updated;
	void Refresh();
	void Activate(object o, int index);
	void Destroy();
	int Index{get;}
	object ObjectData{get;}
	T GetElementComponent<T> ();
}

public class BasicElement : MonoBehaviour, UIListElement
{
	public object Data;
	public event System.Action Enabled;
	public event System.Action<object> EnabledData;
	public event System.Action Updated;
	int index;

	#region UIListElement implementation

	public T GetElementComponent<T> ()
	{
		return gameObject.GetComponent<T>();
	}

	public object ObjectData {
		get {
			return Data;
		}
	}

	public int Index {
		get {
			return index;
		}
	}
	public void Refresh ()
	{
		if(Updated!=null)
			Updated();
	}

	public void Activate (object o, int index)
	{
		gameObject.SetActive(true);
		Data = o;
		if(Enabled!=null)
			Enabled();
		if(EnabledData!=null)
			EnabledData(Data);
		this.index = index;
	}

	public void Destroy ()
	{
		Destroy(gameObject);
	}
	#endregion
	
}
