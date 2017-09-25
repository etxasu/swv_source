using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NameplateUpdater : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<Text>().text = transform.parent.parent.name;
        gameObject.GetComponent<Text>().text.ToUpper();

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
