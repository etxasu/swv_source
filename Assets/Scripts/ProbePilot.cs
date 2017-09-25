using UnityEngine;
using System.Collections;

public class ProbePilot : MonoBehaviour
{
    public Transform Origin;
    public Transform Destination;
    public GameObject ProbePrefab;

    public float Speed;
    private float FlightStep;
    public bool StartFlying;

    [SerializeField]
    private float DespawnTimer;
    private bool _DoTheFlying;


	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(StartFlying)
        {
            StartFlying = !StartFlying;
            InitializePosition();
            _DoTheFlying = true;
            Debug.Log("STARTING PROBE FLIGHT");         
        }

        if(_DoTheFlying)
        {
            FlightStep = Speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, Destination.position, FlightStep);

            if(Vector3.Distance(Origin.position, Destination.position) == 0.0f)
            {
                _DoTheFlying = false;
            }
        }
	}

    // I can see this method getting more complicated in the future.
    // JOS: 8/25/2016
    public void InitializePosition()
    {
        transform.position = Origin.position;
    }

    public IEnumerator Despawn()
    {
        yield return new WaitForSeconds(DespawnTimer);

        Destroy(gameObject);

        yield return null;
    }
}
