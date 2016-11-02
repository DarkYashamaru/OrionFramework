using UnityEngine;
using System.Collections;
using OrionFramework;

public class TestScript : MonoBehaviour {

	[ContextMenu("Run code")]
	public void Code1 ()
	{
		LoginManager.Init (SerializationType.XML);
		Debug.Log (LoginManager.Database.Users.Count);
		UserDatabase ud = LoginManager.Database;
		ud.Users = new System.Collections.Generic.List<User> ();
		User u = new User ();
		ud.Users.Add (u);
		ud.Users.Add (u);
		ud.Users.Add (u);
		LoginManager.Save ();
	}
}
