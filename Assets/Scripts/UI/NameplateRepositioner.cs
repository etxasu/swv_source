using UnityEngine;
using System.Collections;

public class NameplateRepositioner : MonoBehaviour
{
    public bool RepositionTransform;
    public Vector3 RepositionOffset;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(RepositionTransform)
        {
            transform.localPosition = RepositionOffset;
            RepositionTransform = false;
        }
	
	}
}
