using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollBarKludge : MonoBehaviour
{
    public Scrollbar KludgeTarget;
    private bool FirstUpdate = true;

	// Use this for initialization
	void Start ()
    {
        //transform.GetComponent<Scrollbar>().size = 0.0f;
    }

    void Awake()
    {
        //Debug.Log("I'M AWAKE");
    }

    void OnEnable()
    {
        KludgeTarget.size = 0.0f;
        //KludgeTarget.handleRect.rect.width = 20.0f;
        if(KludgeTarget.value > 0.5f)
        {
            KludgeTarget.value = KludgeTarget.value - 0.001f;
        }
        else
        {
            KludgeTarget.value = KludgeTarget.value + 0.001f;
        }        
        FirstUpdate = false;
        //Debug.Log("Size reset for Scroll bar");
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
