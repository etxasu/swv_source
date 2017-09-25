using UnityEngine;
using System.Collections;

public class LaunchCloseUpCameraProbe : MonoBehaviour
{
    public GameObject MySceneController;
    private SceneController _SceneController;

	// Use this for initialization
	void Start ()
    {
        _SceneController = MySceneController.GetComponent<SceneController>();
	
	}
	


	// Update is called once per frame
	void Update ()
    {
	
	}
}
