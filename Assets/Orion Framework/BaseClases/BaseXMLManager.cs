using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class XMLManager
{
	public const string FolderName = "/Resources/";
	public static string DirectoryPath
	{
		get
		{
			return Directory.GetParent(Directory.GetCurrentDirectory())+FolderName;
		}
	}

	public static T LoadXml<T> (string fileName) where T : class
	{
		if (!fileName.Contains (".xml"))
			fileName += ".xml";
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		FileStream stream = new FileStream(DirectoryPath+fileName, FileMode.Open);
		StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
		T t = serializer.Deserialize(sr) as T;
		stream.Close();
		return t;
	}

	public static void SaveXML<T>(string fileName, object data) where T : class
	{
		if (!fileName.Contains (".xml"))
			fileName += ".xml";
		Directory.CreateDirectory(DirectoryPath).Create();
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		FileStream stream = new FileStream(DirectoryPath+fileName, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
		serializer.Serialize(streamWriter,data);
		stream.Close();
	}

	public static bool FileExist (string fileName)
	{
		if (!fileName.Contains (".xml"))
			fileName += ".xml";
		return File.Exists(DirectoryPath+fileName);
	}
}
