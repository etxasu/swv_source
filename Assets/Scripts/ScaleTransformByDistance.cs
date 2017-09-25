using UnityEngine;
using System.Collections;

public class ScaleTransformByDistance : MonoBehaviour
{
    public bool DoScaling;
    public Camera MiniMapCamera;
    private Vector3 _StartingScale;
    public bool QueueMeshRendering = false;
    public bool QueueCanvasRendering = false;
    public int RenderQueueValue;
    public bool DoMovement;
    public float MoveScalar;

    // Use this for initialization
    void Start ()
    {
        if (MiniMapCamera == null)
        {
            MiniMapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        }

        if(QueueMeshRendering)
        {
            gameObject.GetComponent<MeshRenderer>().material.renderQueue = RenderQueueValue;
        }

        if(QueueCanvasRendering)
        {
            //Material _tempMat = transform.GetChild(0).GetComponent<CanvasRenderer>().GetMaterial();
            //Debug.Log(transform.GetChild(0).GetComponent<CanvasRenderer>().renderer.);
            transform.GetChild(0).GetComponent<CanvasRenderer>().GetMaterial().renderQueue = RenderQueueValue;
        }

        _StartingScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(DoScaling)
        {
            ScaleByDistance();
        }

        if(DoMovement)
        {
            TranslateAgainstCamera();
        }
    }

    private void TranslateAgainstCamera()
    {
        transform.localPosition = new Vector3(0.0f, MiniMapCamera.transform.position.y * MoveScalar, 0.0f);
    }

    private void ScaleByDistance()
    {
        transform.localScale = _StartingScale * (float)(Vector3.Distance(Vector3.zero, MiniMapCamera.transform.position));
    }

}
