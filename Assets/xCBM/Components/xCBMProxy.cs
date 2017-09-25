using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xCBM;
#endif

[ExecuteInEditMode]
public class xCBMProxy : MonoBehaviour {
	#if UNITY_EDITOR
	
	#region Fields

	private static Camera _myCamera;
	#endregion
	
	#region Properties
	private Camera myCamera{
		get {
			if(!_myCamera){
				_myCamera = GetComponent<Camera>();
			}
			return _myCamera;
		}
	}
	#endregion


	#region Function

	void OnEnable(){
		// add delegate to reset on scene switch
		EditorApplication.hierarchyWindowChanged += xCBMManager.ResetOnSceneSwitch;
		// check heartbeat of windows
		EditorApplication.update += xCBMManager.CheckHeartbeat;
		// abort SC updates on Play>Edit mode switch
		EditorApplication.update += xCBMManager.AbortUpdatesOnEditorModeChange;
	}

	void Update(){
		// self destroy if Ref to EditorWindow is lost
		if(xCBMManager.Proxy != this) DestroyImmediate (this.gameObject);
		
		// skip updates caused by xCBM
		if(xCBMManager.ExecuteUpdate){
			xCBMManager.SceneChanged (); // mark scene as changed
		}
	}

	void OnDestroy(){
		// remove delegates
		EditorApplication.hierarchyWindowChanged -= xCBMManager.ResetOnSceneSwitch;
		EditorApplication.update -= xCBMManager.CheckHeartbeat;
		EditorApplication.update -= xCBMManager.AbortUpdatesOnEditorModeChange;
	}

	public void DebugLog(string text){
		Debug.Log(text);
	}

	#region Update SC
	public void StartUpdateScreenCapCoroutine(xCBMScreenCap ScreenCapToUpdate){
		StartCoroutine(xCBMManager.ReadScreenCap (ScreenCapToUpdate));
	}
	#endregion
	
	#region Wait x frames
	// coroutine to wait a few frames between resolution change and SC update
	public void StartWaitXFramesCoroutine(xCBMScreenCap ScreenCap, int frameCount){
		StartCoroutine (WaitXFrames (ScreenCap, frameCount));
	}
	
	private IEnumerator WaitXFrames(xCBMScreenCap ScreenCap, int frameCount){
		while (frameCount > 0){
			yield return null; // wait until next frame
			frameCount--;
		}

		xCBMManager.UpdateScreenCapAtEOF (ScreenCap);
	}
	#endregion
	#endregion
	
	#endif
}