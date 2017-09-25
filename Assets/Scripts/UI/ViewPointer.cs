using UnityEngine;
using System.Collections;

public class ViewPointer : MonoBehaviour
{
    public GameObject MyTarget;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(MyTarget.transform);
	}
}
