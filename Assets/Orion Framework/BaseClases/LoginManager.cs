using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using YashaFramework.Serialization;

namespace YashaFramework
{
	namespace BasicLogin
	{
		public static class LoginManager {
			static SerializationType Serialization;
			public static string path = "/UserDatabase";
			public static UserDatabase database;
			public static UserDatabase Database
			{
				get
				{
					if (database == null)
						LoadOrCreate ();
					return database;
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
					if (SerializationManager.FileExist (FileName,Serialization)) {

					} 
					else 
					{

					}
				}
			}

			public static void Init (SerializationType serialization)
			{
				Serialization = serialization;
			}
		}

		[Serializable]
		public class UserDatabase
		{
			public User[] Users;
		}

		[Serializable]
		public class User
		{
			public string userName = string.Empty;
			public string id = string.Empty;
			public UserData[] data;
		}

		[Serializable]
		public class UserData
		{
			public string name = string.Empty;
			public string value;
		}
	}

	namespace Serialization
	{
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
				return File.Exists(path + GetExtention (serialization));
			}

			public static void CreateDirectory (string path)
			{
				Directory.CreateDirectory(path);
			}
		}

		public enum SerializationType {XML,JSON,Binary};
	}

	public class Serializer
	{
		XmlSerializer xmlSerializer;

		public Serializer (SerializationType serialization, Type type)
		{
			switch (serialization) 
			{
			case SerializationType.XML:
				xmlSerializer = new XmlSerializer (type);
				break;
			}
		}
		SerializationType currentType;

		public void SetType (SerializationType serialization)
		{
			currentType = serialization;
		}

		public void Serialize (TextWriter textWriter, object o)
		{
			
		}

		public void Serialize (StreamWriter streamWriter, object o)
		{

		}

		public object Deserialize (TextReader textReader)
		{
			return new object ();
		}

		public object Deserialize (StreamReader streamReader)
		{
			return new object ();
		}
	}
}