using UnityEngine;
using System.Collections;

//
// Controls objects in the primary view panel.
// JOS: 6/30/2016
//

public class ObjectController : MonoBehaviour
{
    public Material[] ObjectMaterials;
    private bool ColorToggle;

	// Use this for initialization
	void Start ()
    {        
        //Debug.Log(gameObject.transform.childCount.ToString());	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // Colorize the objects on the screen.
    public void Colorize()
    {
        if(!ColorToggle)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject MiddleChild = gameObject.transform.GetChild(i).gameObject;

                for (int j = 0; j < MiddleChild.transform.childCount; j++)
                {
                    OrbitalMovement Go = MiddleChild.transform.GetChild(j).gameObject.GetComponent<OrbitalMovement>();

                    Go.UpdateMaterial(ObjectMaterials[(int)Go.MyObjectType]);
                }
            }

        }
        else
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject MiddleChild = gameObject.transform.GetChild(i).gameObject;

                for (int j = 0; j < MiddleChild.transform.childCount; j++)
                {
                    OrbitalMovement Go = MiddleChild.transform.GetChild(j).gameObject.GetComponent<OrbitalMovement>();

                    Go.UpdateMaterial(ObjectMaterials[0]);
                }
            }
        }

        ColorToggle = !ColorToggle;
    }

    //
    // Toggles the displayed objects. Will consider using enum or int if it proves more efficient.
    // Note that this function completely disables the objects. If you just want to turn off the visibility in ENV,
    // consider using the SceneController.ToggleXXX methods.
    // JOS: 6/30/2016

    public void ToggleDisplayedObjects(int _t)
    {
        SmallWorldType _obj = (SmallWorldType)_t;

        switch(_obj)
        {
            case SmallWorldType.MAB:

                transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);

                break;
            case SmallWorldType.OCO:
                // NYI
                break;
            case SmallWorldType.NEO:

                transform.GetChild(1).gameObject.SetActive(!transform.GetChild(1).gameObject.activeSelf);

                break;
            case SmallWorldType.KB:
                // NYI
                break;
            default:
                // obvs totes do nada
                break;
        }
    }
}
