using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class BinaryManager {

	public static T Load<T> (string fileName) where T : class
	{
		//Debug.Log(Application.persistentDataPath+fileName);
		BinaryFormatter bf = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath+fileName, FileMode.Open);
		T t = (T)bf.Deserialize(stream);
		stream.Close();
		//Debug.Log("Loaded Sucessfully");
		return t;
	}

	public static void Save(string fileName, object data)
	{
		BinaryFormatter bf = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath+fileName, FileMode.Create);
		bf.Serialize(stream,data);
		stream.Close();
	}

	public static bool FileExist (string fileName)
	{
		return File.Exists(Application.persistentDataPath+fileName);
	}
}
