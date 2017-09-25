using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FactoidsController : MonoBehaviour
{
    public OrbitalMovement CurrentDataset;
    [SerializeField]
    private GameObject Sun;
    // Transform hierarchy references
    // BE CAREFUL WITH THIS GAMEOBJECT!
    // JOS: 8/30/2016

    public Text PlanetLabel;
    public Text CurrentAU;
    public Text AverageAU;
    public Text OrbitalPeriod;
    public Text Factoid1;
    public Text Factoid2;
    public Text Factoid3;
    public Text Factoid4;

    private bool TimerToggle = true;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(CurrentDataset != null && TimerToggle == false)
        {
            UpdateCurrentAU();
        }	
	}

    public void FixMyFactoids()
    {
        Debug.Log(CurrentDataset.transform.name);

        PlanetLabel.text = "WORLD NAME: " + CurrentDataset.transform.name + " ";
        //CurrentAU.text = "CURRENT DISTANCE: " + (Vector3.Distance(Sun.transform.position, CurrentDataset.transform.position)/ 10.0f).ToString() + " AU";
        AverageAU.text = "AVERAGE DISTANCE: " + (CurrentDataset.radius / 10.0f).ToString() + " AU";
        Factoid1.text = CurrentDataset.Factoid1;
        Factoid2.text = CurrentDataset.Factoid2;
        Factoid3.text = CurrentDataset.Factoid3;
        //Factoid4.text = "Current AU : <color=white>" + CurrentDataset.Factoid4 + "</color> ";
    }

    private void UpdateCurrentAU()
    {
        TimerToggle = true;
        CurrentAU.text = "CURRENT DISTANCE: " + (Vector3.Distance(Sun.transform.position, CurrentDataset.transform.position) / 10.0f).ToString() + " AU";
        StartCoroutine(WaitTwoSeconds());
    }

    private IEnumerator WaitTwoSeconds()
    {
        yield return new WaitForSeconds(2.0f);

        TimerToggle = false;

        yield return null;
    }
}
