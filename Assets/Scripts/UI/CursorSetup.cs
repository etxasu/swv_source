using UnityEngine;
using System.Collections;

public class CursorSetup : MonoBehaviour
{
    public Texture2D CursorTexture;
    public CursorMode ActiveCursorMode;
    public Vector2 HotSpot = Vector2.zero;

	// Use this for initialization
	void Start ()
    {
        Cursor.SetCursor(CursorTexture, HotSpot, ActiveCursorMode);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
