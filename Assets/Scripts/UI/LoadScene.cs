using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    public int SceneToLoad;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // This could not be more straightforward
    // JOS: 7/5/2016
    public void LoadThisScene()
    {
        SceneManager.LoadScene(SceneToLoad);     
    }
}
