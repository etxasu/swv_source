using UnityEngine;
using System.Collections;

public class CoinRotate : MonoBehaviour
{
    public float YAxisSpeed;
    public float XAxisSpeed;
    public float ZAxisSpeed;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(XAxisSpeed, YAxisSpeed, ZAxisSpeed);
	}
}
