﻿using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace OrionFramework
{
	public static class LoginManager {
		static SerializationType Serialization;
		public static string path = "UserDatabase";
		static UserDatabase database;
		public static User CurrentUser;
		public static UserDatabase Database
		{
			get
			{
				if (database == null)
					LoadOrCreate ();
				return database;
			}
			set 
			{
				database = value;
			}
		}

		static string FileName
		{
			get 
			{
				return path + SerializationManager.GetExtention (Serialization);
			}
		}

		static void LoadOrCreate ()
		{
			if (database == null) 
			{
				UnityEngine.Debug.Log ("File exist " + SerializationManager.FileExist(FileName,Serialization) + " FileName " + FileName);
				if (SerializationManager.FileExist (FileName,Serialization)) {
					database = SerializationManager.Load<UserDatabase> (FileName, Serialization);
				}
				if (database == null) 
				{
					UnityEngine.Debug.Log ("Creating new data");
					database = new UserDatabase ();
					SerializationManager.Save<UserDatabase> (FileName, database, Serialization);
				}
			}
		}

		public static void Save ()
		{
			SerializationManager.Save<UserDatabase> (FileName, database, Serialization);
		}

		public static void Init (SerializationType serialization)
		{
			Serialization = serialization;
			LoadOrCreate ();
		}

		public static User GetUserByName (string name, string lastName)
		{
			for (int i = 0; i < database.Users.Count; i++) 
			{
				if (database.Users [i].userName.Equals (name) && database.Users [i].lastName.Equals (lastName))
					return database.Users [i];
			}
			return null;
		}
	}

	[Serializable]
	public class UserDatabase
	{
		public List<User> Users = new List<User>();
	}

	[Serializable]
	public class User
	{
		public string userName = string.Empty;
		public string id = string.Empty;
		public string lastName = string.Empty;
		public List<UserData> data = new List<UserData>();
		public List<ArrayUserData> arrayData = new List<ArrayUserData>();

		public User () {}

		public User (UserData[] data) {
			this.data.Clear ();
			this.data.AddRange (data);
		}

		public UserData GetLastValueInArray (int secondIndex)
		{
			return arrayData.LastObject ().data[secondIndex];
		}

		public override string ToString ()
		{
			string message = string.Empty;
			message += "Name : " + userName+"\n";
			message += "id : " + id+"\n";
			message += "lastName : " + lastName+"\n";
			for (int i = 0; i < data.Count; i++) 
			{
				message+=data [i] + "\n";
			}
			for (int i = 0; i < arrayData.Count; i++) 
			{
				message+=arrayData [i] + "\n";
			}
			return message;
		}
	}

	[Serializable]
	public class UserData
	{
		public string name = string.Empty;
		public string value;

		public override string ToString ()
		{
			return string.Format ("[UserData] name: {0} value : {1}", name, value);
		}
	}

	[Serializable]
	public class ArrayUserData
	{
		public List<UserData> data = new List<UserData>();

		public override string ToString ()
		{
			string message = string.Empty;
			for (int i = 0; i < data.Count; i++) 
			{
				message += data [i].ToString ();
				message += "\n";
			}
			return message;
		}
	}

	public static class SerializationManager
	{
		//Default folder name for serialized assets
		const string FolderName = "/Resources/";
		//Default directory for serialized assets (1 folder above current directory plus folder name)
		static string DirectoryPath
		{
			get
			{
				return Directory.GetParent(Directory.GetCurrentDirectory())+FolderName;
			}
		}
		//The custom directory path
		public static string CustomPath = string.Empty;
		//The final directory path, the default path is used if custom path is empty
		public static string FinalPath
		{
			get
			{
				return !string.IsNullOrEmpty (CustomPath) ? CustomPath : DirectoryPath;
			}
		}

		/// <summary>
		/// Gets the file extention depending on SerializationType
		/// </summary>
		/// <returns>The extention.</returns>
		/// <param name="serialization">Serialization Type.</param>
		public static string GetExtention (SerializationType serialization)
		{
			switch (serialization)
			{
			case SerializationType.Binary:
				return ".dat";
			case SerializationType.JSON:
				return ".json";
			case SerializationType.XML:
				return ".xml";
			default:
				return ".xml";
			}
		}

		public static T Load<T> (string fileName, SerializationType serialization)  where T : class
		{
			return Load<T> (fileName, serialization, Encoding.UTF8);
		}

		public static T Load<T> (string fileName, SerializationType serialization, Encoding encoding)  where T : class
		{
			Serializer serializer = new Serializer (serialization,typeof(T));
			FileStream stream = new FileStream(DirectoryPath+fileName, FileMode.Open);
			StreamReader sr = new StreamReader(stream, encoding);
			T t = serializer.Deserialize(sr) as T;
			stream.Close();
			return t;
		}

		public static void Save<T> (string fileName, object data, SerializationType serialization) where T : class
		{
			Save<T> (fileName, data, serialization, Encoding.UTF8);
		}

		public static void Save<T> (string fileName, object data, SerializationType serialization, Encoding encoding) where T : class
		{
			Directory.CreateDirectory(FinalPath).Create();
			Serializer serializer = new Serializer (serialization,typeof(T));
			FileStream stream = new FileStream(DirectoryPath+fileName, FileMode.Create);
			StreamWriter streamWriter = new StreamWriter(stream, encoding);
			serializer.Serialize(streamWriter,data);
			stream.Close();
		}

		public static bool FileExist (string path, SerializationType serialization)
		{
			UnityEngine.Debug.Log (DirectoryPath + path);
			return File.Exists(DirectoryPath+ path);
		}

		public static void CreateDirectory (string path)
		{
			Directory.CreateDirectory(path);
		}
	}

	public enum SerializationType {XML,JSON,Binary};

	public class Serializer
	{
		readonly XmlSerializer xmlSerializer;
		readonly BinaryFormatter binarySerializer;
		SerializationType CurrentSerialization;

		public Serializer (SerializationType serialization, Type type)
		{
			CurrentSerialization = serialization;
			switch (serialization) 
			{
			case SerializationType.XML:
				xmlSerializer = new XmlSerializer (type);
				break;
			case SerializationType.Binary:
				binarySerializer = new BinaryFormatter ();
				break;
			}
		}

		public void Serialize (StreamWriter streamWriter, object o)
		{
			switch (CurrentSerialization) 
			{
			case SerializationType.XML:
				xmlSerializer.Serialize (streamWriter, o);
				break;
			case SerializationType.Binary:
				binarySerializer.Serialize (streamWriter.BaseStream, o);
				break;
			}
		}

		public object Deserialize (StreamReader streamReader)
		{
			switch (CurrentSerialization) 
			{
			case SerializationType.XML:
				return xmlSerializer.Deserialize (streamReader);
			case SerializationType.Binary:
				return binarySerializer.Deserialize (streamReader.BaseStream);
			}
			return null;
		}
	}
}