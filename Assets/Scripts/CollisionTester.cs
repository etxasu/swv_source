using UnityEngine;
using System.Collections;

public class CollisionTester : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("GOT A HIT");
        Debug.Log(col.gameObject.name);

        if (col.gameObject.transform.parent.gameObject.name == "Urf")
        {
            Debug.Log("Collided with Urf at " + System.DateTime.Now.ToString());
        }
    }
}
