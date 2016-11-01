using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeSceneChanger : MonoBehaviour {
    public Image FadeImage;
    static FadeSceneChanger instance;
    public static FadeSceneChanger Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject sc = Resources.Load<GameObject>("Scene Changer");
                GameObject i = Instantiate(sc);
                instance = i.GetComponent<FadeSceneChanger>();
                DontDestroyOnLoad(i);
            }
            return instance;
        }
    }
    public static bool LoadingScene;
	public static event System.Action LevelStartedLoading;
	public static event System.Action LevelFinishedLoading;


    public static void ChangeScene (int index)
    {
        if(!LoadingScene)
        {
			LoadingScene = true;
            Instance.StartCoroutine(Instance.Change(index));
        }
        else
        {
            Debug.Log("Scene Changer is busy");
        }        
    }

    public static void ChangeScene (string name)
    {
        Scene s = SceneManager.GetSceneByName(name);
		if(s.buildIndex>-1)
        {
            int index = s.buildIndex;
            if (!LoadingScene)
            {
				LoadingScene = true;
                Instance.StartCoroutine(Instance.Change(index));
            }
            else
            {
                Debug.Log("Scene Changer is busy");
            }
        }
        else
        {
            Debug.Log("Scene " + name + " not found");
        }

    }

    AsyncOperation ao;
    IEnumerator Change (int index)
    {
        FadeImage.color = new Color(0, 0, 0, 0);
        LeanTween.value(gameObject, UpdateAlpha, new Color(0,0,0,0),Color.black, 1);
        yield return new WaitForSeconds(1);
		if (LevelStartedLoading != null)
			LevelStartedLoading ();
        ao = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        FadeImage.color = Color.black;
        while (!ao.isDone)
        {
            yield return null;
        }
		if (LevelFinishedLoading != null)
			LevelFinishedLoading ();
		LeanTween.value(gameObject, UpdateAlpha, Color.black,new Color(0,0,0,0), 1);
        yield return new WaitForSeconds(1);
        instance = null;
		LoadingScene = false;
        Destroy(gameObject);
    }

    void UpdateAlpha (Color c)
    {
        FadeImage.color = c;
    }
}
