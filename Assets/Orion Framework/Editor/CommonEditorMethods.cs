using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class CommonEditorMethods {

	[MenuItem ("Custom/Commands/Align with ground %t")]
	static void AlignWithGround () {
		Undo.RecordObjects(Selection.transforms, "nombre cualquiera");
		Transform [] transforms = Selection.transforms;
		foreach (Transform myTransform in transforms) {
			RaycastHit hit;
			if (Physics.Raycast (myTransform.position, -Vector3.up, out hit)) {
				Vector3 targetPosition = hit.point;
				if (myTransform.gameObject.GetComponent<MeshFilter>() != null) {
					Bounds bounds = myTransform.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
					targetPosition.y += bounds.extents.y;
				}
				myTransform.position = targetPosition;
				Vector3 targetRotation = new Vector3 (hit.normal.x, myTransform.eulerAngles.y, hit.normal.z);
				myTransform.eulerAngles = targetRotation;
			}
		}
	}

	[MenuItem("GameObject/Custom/Canvas/HD Canvas", false, 10)]
	static void CreateCustomHDCanvas(MenuCommand menuCommand) {
		CreateCanvas (1280, 720, menuCommand);
	}

	[MenuItem("GameObject/Custom/Canvas/Full HD Canvas", false, 10)]
	static void CreateCustomFullHDCanvas(MenuCommand menuCommand) {
		CreateCanvas (1920, 1080, menuCommand);
	}

	[MenuItem("GameObject/Custom/Canvas/4K Canvas", false, 10)]
	static void CreateCustom4KCanvas(MenuCommand menuCommand) {
		CreateCanvas (3840, 2160, menuCommand);
	}

	static void CreateCanvas (int witdh, int height, MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("Canvas Container");
		GameObject content = new GameObject("Content");
		content.transform.parent = go.transform;
		GameObject canvas = new GameObject("Canvas");
		GameObject camera = new GameObject("Canvas Camera");
		camera.transform.position = new Vector3(0,0,-10);
		camera.transform.parent = content.transform;
		Canvas cv = canvas.AddComponent<Canvas>();
		CanvasScaler cs = canvas.AddComponent<CanvasScaler>();
		GraphicRaycaster gr = canvas.AddComponent<GraphicRaycaster>();
		gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		canvas.transform.SetParent(content.transform);
		Camera c = camera.AddComponent<Camera>();
		c.cullingMask = 1 << 5;
		c.useOcclusionCulling = false;
		c.orthographic = true;
		c.orthographicSize = 1;
		c.farClipPlane = 5;
		c.clearFlags = CameraClearFlags.Depth;
		c.depth = 50;
		cv.renderMode = RenderMode.ScreenSpaceCamera;
		cv.worldCamera = c;
		cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		cs.referenceResolution = new Vector2(witdh,height);
		cv.planeDistance = 3;
		cv.gameObject.layer = LayerMask.NameToLayer("UI");
		cv.pixelPerfect = true;
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
		CreateEventSystem();
	}

	static void CreateEventSystem()
	{
		EventSystem e = Object.FindObjectOfType<EventSystem>();
		if(e == null)
		{
			GameObject g = new GameObject("EventSystem");
			e = g.AddComponent<EventSystem>();
		}
		StandaloneInputModule s = e.GetComponent<StandaloneInputModule>();
		if(s == null)
			e.gameObject.AddComponent<StandaloneInputModule>();		
	}

	[MenuItem("Custom/Commands/Clear Unity Data")]
	public static void ClearData ()
	{
		if(EditorUtility.DisplayDialog("Clear Save Data","Are you sure?","Yes","No"))
		{
			PlayerPrefs.DeleteAll();
		}
	}

	[MenuItem("GameObject/Custom/UIList", false, 10)]
	static void CreateUIList(MenuCommand menuCommand) {

		//parent setup
		GameObject go = new GameObject("UI List");
		Image img = go.AddComponent<Image>();
		img.color = new Color(1,1,1,0.5f);
		go.AddComponent<Mask>();
		ScrollRect sr = go.AddComponent<ScrollRect>();
		sr.scrollSensitivity = 10;
		sr.movementType = ScrollRect.MovementType.Clamped;
		sr.horizontal = false;
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(400,400);

		//Content Setup
		GameObject content = new GameObject("Content");
		ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
		csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
		vlg.childAlignment = TextAnchor.UpperCenter;
		vlg.spacing = 5;
		vlg.padding = new RectOffset(10,10,10,10);
		content.transform.SetParent(sr.transform);
		sr.content = (RectTransform)content.transform;
		rt = content.GetComponent<RectTransform>();
		rt.SetUpperCenter();

		//child prefab setup
		GameObject child = new GameObject("Prefab");
		LayoutElement le = child.AddComponent<LayoutElement>();
		le.preferredWidth = 390;
		le.preferredHeight = 50;
		child.AddComponent<Image>();
		child.transform.SetParent(content.transform);
		GameObject text = new GameObject("Label");
		text.transform.SetParent(child.transform);
		Text t = text.AddComponent<Text>();
		t.alignment = TextAnchor.MiddleCenter;
		t.text = "Prefab";
		t.color = Color.black;
		rt = text.GetComponent<RectTransform>();
		rt.SetToStretch();

		//final touches
		UIList ul = go.AddComponent<UIList>();
		ul.Content = content;
		ul.Prefab = child;

		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	public static void DrawLineAndTravelTime (Vector3 startPos, Vector3 endPos, float travelTime, GUIStyle textStyle)
	{
		Vector3 avg = startPos*0.5f+endPos*0.5f;
		Handles.DrawLine(startPos,endPos);
		Handles.Label(avg+Vector3.up,"Tiempo : "+travelTime+"s",textStyle);
		Handles.Label(avg+Vector3.up*2f,"Distancia : "+Vector3.Distance(startPos,endPos)+"m",textStyle);
	}

	[MenuItem("Custom/Commands/Reload Level", false, 10)]
	public static void ReloadLevel ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
