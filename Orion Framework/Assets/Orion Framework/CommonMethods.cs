using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class CommonMethods
{

    public static void TextWriter(string path, string fileName, string text)
    {
        Directory.CreateDirectory(path).Create();
        using (StreamWriter writetext = new StreamWriter(path + fileName))
        {
            writetext.WriteLine(text);
        }
    }

    public static void TextWriter(string path, string text)
    {
        using (StreamWriter writetext = new StreamWriter(path))
        {
            writetext.WriteLine(text);
        }
    }

    public static string TextReader(string path)
    {
        string result = string.Empty;
        if(File.Exists(path))
        {
            using (StreamReader readtext = new StreamReader(path))
            {
                result = readtext.ReadToEnd();
            }
        }
        else
        {
            Debug.Log("File doesn't exist path : " + path);
        }
        return result;
    }

    public static string TextReader(string path, string fileName)
    {
        return TextReader(path + fileName);
    }

    public static byte[] ObectToByteArray (object obj)
	{
		if(obj == null)
			return null;
		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	public static T ByteArrayToObject<T>(byte[] byteArray)
	{
		MemoryStream ms = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		ms.Write(byteArray, 0, byteArray.Length);
		ms.Seek(0, SeekOrigin.Begin);
		object obj = bf.Deserialize(ms);
		return (T)obj;
	}
	
	static GameObject tempParent;
	public static void AgrupateTemporalObjects (GameObject obj, string parentObject = "TemporalObjects")
	{
		tempParent = GameObject.Find(parentObject);
		if(tempParent==null)
		{
			tempParent = new GameObject();
			tempParent.name = parentObject;
		}
		obj.transform.SetParent(tempParent.transform);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static class ThreadSafeRandom
	{
		[ThreadStatic] static System.Random Local;
		
		public static System.Random ThisThreadsRandom
		{
			get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + System.Threading.Thread.CurrentThread.ManagedThreadId))); }
		}
	}

	static CoroutineHelper ch;
	public static CoroutineHelper CH
	{
		get
		{
			if(ch==null)
			{
				GameObject g = new GameObject();
				g.name = "Coroutine Helper";
				UnityEngine.Object.DontDestroyOnLoad(g);
				ch = g.AddComponent<CoroutineHelper>();
			}
			return ch;
		}
	}

	public static void CopyTransform(this Transform trans, Transform toCopy)
	{
		trans.position = toCopy.position;
		trans.rotation = toCopy.rotation;
		trans.localScale = toCopy.localScale;
	}

	public static void ResetLocalTransform(this Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}
	
	public static void ResetTransform(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.rotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}

	public static void DisableChilds(this Transform trans)
	{
		for (int i = 0; i < trans.childCount; i++) 
		{
			trans.GetChild (i).gameObject.SetActive (false);
		}
	}

	public static Toggle GetActive(this ToggleGroup aGroup)
	{
		return aGroup.ActiveToggles().FirstOrDefault();
	}

	public static T[] GetComponentsJustInChilds<T>(this GameObject parent)  where T:Component
	{
		List<T> temp = new List<T>();
		T[] childs = parent.GetComponentsInChildren<T>();
		temp.AddRange(childs);
		temp.Remove(parent.GetComponent<T>());
		return temp.ToArray();
	}


	public static T[] GetComponentsInAllChildrens<T>(this GameObject parent)  where T:Component
	{
		List<T> temp = new List<T>();
		GetComponentsInAllChilds<T>(parent, ref temp);
		return temp.ToArray();

	}

	static void GetComponentsInAllChilds<T>(GameObject parent, ref List<T> currentList)  where T:Component
	{
		for(int x = 0; x < parent.transform.childCount; x++)
		{
			T temp = parent.transform.GetChild(x).GetComponent<T>();
			if(temp != null)
				currentList.Add(temp);
			GetComponentsInAllChilds<T>(parent.transform.GetChild(x).gameObject, ref currentList);
		}
	}

	public static void FadeTo (this CanvasGroup group, float to, float speed = 1, float finalDelay = 0, Action callback = null)
	{
		CH.StartCoroutine(CanvasFade(group, to, group.alpha, speed, finalDelay, callback));

	}

	public static void FadeTo (this CanvasGroup group, float to, float from, float speed = 1, float finalDelay = 0, Action callback = null)
	{
		CH.StartCoroutine(CanvasFade(group, to, from, speed, finalDelay, callback));
	}

	static IEnumerator CanvasFade(CanvasGroup group, float to, float from, float speed = 1, float finalDelay = 0, Action callback = null)
	{
		group.alpha = from;

		while(to > from)
		{
			from += speed * Time.unscaledDeltaTime;
			group.alpha = from;
			yield return null;
		}
		while(to < from)
		{
			from -= speed * Time.unscaledDeltaTime;
			group.alpha = from;
			yield return null;
		}
		group.alpha = to;
		yield return new WaitForRealSeconds(finalDelay);
		if(callback != null)
			callback();
	}

	public static void IncreaseValue (this Slider slider, float amount)
	{
		slider.value += amount;
	}

	public static string[] ToStringArray<T>(this IList<T> list)
	{
		string[] s = new string[list.Count];
		for(int i = 0; i < s.Length; i++)
		{
			s[i] = list[i].ToString();
		}
		return s;
	}

	public static T LastObject<T>(this IList<T> list)
	{
		return list [list.Count - 1];
	}

	public static Vector3[] TransformToVectorArrray<T>(this T[] list) where T : Transform
	{
		Vector3[] v = new Vector3[list.Length];
		for(int i = 0; i < v.Length; i++)
		{
			v [i] = list [i].transform.position;
		}
		return v;
	}

	public static Vector3[] ToVectorArrray<T>(this T[] list) where T : MonoBehaviour
	{
		Vector3[] v = new Vector3[list.Length];
		for(int i = 0; i < v.Length; i++)
		{
			v [i] = list [i].transform.position;
		}
		return v;
	}

	public static Vector3 RandomRange(Vector3 min, Vector3 max)
	{
		Vector3 temp = new Vector3();

		temp.x = UnityEngine.Random.Range(min.x, max.x);
		temp.y = UnityEngine.Random.Range(min.y, max.y);
		temp.z = UnityEngine.Random.Range(min.z, max.z);

		return temp;
	}

	public static Vector2 RandomRange(Vector2 min, Vector2 max)
	{
		Vector2 temp = new Vector2();

		temp.x = UnityEngine.Random.Range(min.x, max.x);
		temp.y = UnityEngine.Random.Range(min.y, max.y);

		return temp;
	}

	const string ScreenShotFormat = "{0}{1}.png";

	public static void TakeScreenShot (Camera cam, string path, string name, int width, int height, TextureFormat format)
	{
		RenderTexture rt = new RenderTexture(width,height,32,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
		cam.targetTexture = rt;
		Texture2D screenShot = new Texture2D(width,height,TextureFormat.ARGB32,false);
		cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0,0,width,height),0,0);
		screenShot.Apply ();
		cam.targetTexture = null;
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(rt);
		byte[] bytes = screenShot.EncodeToPNG();
		string finalPath = string.Format(ScreenShotFormat,path,name);
		File.WriteAllBytes(finalPath,bytes);
		Debug.Log("ScreenShot saved at "+finalPath);
	}

	public static void TakeScreenShotWithTransparency (Camera cam, string path, string name, int width, int height)
	{
//		cam.clearFlags = CameraClearFlags.Color;
//		cam.backgroundColor = Color.black;
//		RenderTexture rtb = new RenderTexture (width, height, 24);
//		cam.targetTexture = rtb;
//		cam.Render ();
//		RenderTexture.active = rtb;
//		Texture2D ss = new Texture2D (width, height, TextureFormat.RGB24,false);
//		ss.ReadPixels(new Rect(0,0,width,height),0,0);
//		ss.Apply ();
//		byte[] bytes = ss.EncodeToPNG();
//		string finalPath = string.Format(ScreenShotFormat,path+"/",name);
//		File.WriteAllBytes(finalPath,bytes);
//		Debug.Log("ScreenShot saved at "+finalPath);

		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.magenta;
		RenderTexture rtb = new RenderTexture (width, height, 24);
		cam.targetTexture = rtb;
		cam.Render ();
		RenderTexture.active = rtb;
		Texture2D ss = new Texture2D (width, height, TextureFormat.RGB24,false);
		ss.ReadPixels(new Rect(0,0,width,height),0,0);
		ss.Apply ();
		byte[] bytes = ss.EncodeToPNG();
		string finalPath = string.Format(ScreenShotFormat,path+"/",name);
		File.WriteAllBytes(finalPath,bytes);
		Debug.Log("ScreenShot saved at "+finalPath);


//		RenderTexture rt = new RenderTexture(width,height,32,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
//		cam.targetTexture = rt;
//		Texture2D screenShot = new Texture2D(width,height,TextureFormat.ARGB32,false);
//		cam.Render();
//		RenderTexture.active = rt;
//		screenShot.ReadPixels(new Rect(0,0,width,height),0,0);
//		screenShot.Apply ();
//		cam.targetTexture = null;
//		RenderTexture.active = null;
//		UnityEngine.Object.Destroy(rt);
//		byte[] bytes = screenShot.EncodeToPNG();
//		string finalPath = string.Format(ScreenShotFormat,path,name);
//		File.WriteAllBytes(finalPath,bytes);
//		Debug.Log("ScreenShot saved at "+finalPath);
	}

	public static void SetToStretch(this RectTransform rt)
	{
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.sizeDelta = Vector2.zero;
		rt.anchoredPosition = Vector2.zero;
	}

	public static void SetUpperCenter(this RectTransform rt)
	{
		rt.pivot = new Vector2(0.5f,1);
		rt.anchorMin = new Vector2(0.5f,1);
		rt.anchorMax = new Vector2(0.5f,1);
		rt.anchoredPosition = Vector2.zero;
	}

	public static void ResetVelocity(this Rigidbody rb)
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	public static InputResponse WaitForInput ()
	{
			InputResponse ir = new InputResponse();
			if(Input.GetButtonDown("Submit"))
			{
					ir.Response = true;
					return ir;
			}
			if(Input.GetButtonDown("Cancel"))
			{
					ir.Response = false;
					return ir;
			}
			return ir;
	}

	public static T FindIfNull<T>(ref T t) where T : Component
	{
		if (t == null)
			t = UnityEngine.Object.FindObjectOfType<T> ();
		return t;
	}

	public static IEnumerator TextAlpha (Graphic toAnimate, float from, float to, float time)
	{
			float stime;
			Color c;
			float cTime = 0;
			while(time > cTime)
			{
					stime = cTime/time;
					c = toAnimate.color;
					c.a = Lerp(from,to,stime);
					toAnimate.color = c;
					cTime += Time.deltaTime;
					yield return null;
			}
			c = toAnimate.color;
			c.a = to;
			toAnimate.color = c;
	}

	static float Lerp(float v0, float v1, float t) {
			return (1-t)*v0 + t*v1;
	}

	public static Vector3 MiddlePointBetweenVectors (Vector3[] points)
	{
		AverageVector av = new AverageVector ();
		av.Init (points [0]);
		for (int i = 0; i < points.Length; i++) 
		{
			if (points [i].x > av.maxX)
				av.maxX = points [i].x;

			if (points [i].x < av.minX)
				av.minX = points [i].x;

			if (points [i].y > av.maxY)
				av.maxY = points [i].y;

			if (points [i].y < av.minY)
				av.minY = points [i].y;

			if (points [i].z > av.maxZ)
				av.maxZ = points [i].z;

			if (points [i].z < av.minZ)
				av.minZ = points [i].z;
		}

		return av.GetMiddlePoint ();
	}

	public static Vector3 MoveToPositionX (Vector3 current, Vector3 target, float speed)
	{
		if (current.x < target.x) 
		{
			if ((current.x - (Vector3.right * speed).x < target.x)) 
			{
				return target;
			}
			return current - (Vector3.right * speed);
		}
		if ((current.x + (Vector3.right * speed).x > target.x)) 
		{
			return target;
		}
		return current + (Vector3.right * speed);
	}

	public static void TurnOffAllChilds (Transform transform)
	{
		for (int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	public static void TurnOnChilds (Transform transform, int amount)
	{
		try
		{
			for (int i = 0; i < amount; i++) {
				transform.GetChild (i).gameObject.SetActive (true);
			}	
		}
		catch(Exception e) {
			Debug.Log (e.Message);
		}
	}
}

[System.Serializable]
public struct AverageVector
{
	public float maxX;
	public float minX;
	public float minY;
	public float maxY;
	public float minZ;
	public float maxZ;

	public void Init (Vector3 first)
	{
		maxX = minX = first.x;
		maxY = minY = first.y;
		maxZ = minZ = first.z;
	}

	public Vector3 GetMiddlePoint ()
	{
		return new Vector3 ((maxX + minX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f); 
	}
}
