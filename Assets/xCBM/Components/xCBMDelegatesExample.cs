#if UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
public class xCBMDelegatesExample : MonoBehaviour {
	/* This is an example of how you can use the xCBM delegates to hook you own code. 
	 * Drop this script on a GameObject in your scene to see what SC is updated.
	 * See xCBM User Guide for more information.
	 * 
	 * Note: Do NOT put your custom code into the xCBM folder to enable easy updating of xCBM.
	 */
	
	// hook delegates
	void OnEnable () {
		xCBMManager.OnPreScreenCapUpdate += StartUpdate;
		xCBMManager.OnPreScreenCapUpdate += PreUpdate;
		xCBMManager.OnPostScreenCapUpdate += PostUpdate;
		xCBMManager.OnFinalizeScreenCapUpdate += FinUpdate;
	}
	
	// unhook delegates
	void OnDisable () {
		xCBMManager.OnPreScreenCapUpdate -= StartUpdate;
		xCBMManager.OnPreScreenCapUpdate -= PreUpdate;
		xCBMManager.OnPostScreenCapUpdate -= PostUpdate;
		xCBMManager.OnFinalizeScreenCapUpdate -= FinUpdate;
	}
	
	
	// your custom functions
	private void StartUpdate (){
		Debug.Log ("StartUpdate: " + xCBMManager.UpdatingScreenCap.LongName);
	}

	private void PreUpdate (){
		Debug.Log ("PreUpdate: " + xCBMManager.UpdatingScreenCap.LongName);
	}
	
	private void PostUpdate (){
		Debug.Log ("PostUpdate: " + xCBMManager.UpdatingScreenCap.LongName);
	}
	
	private void FinUpdate (){
		Debug.Log ("FinUpdate: " + xCBMManager.UpdatingScreenCap.LongName);
	}
	
}
#endif