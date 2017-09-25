#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using xCBM;


public static class xCBMManager {
	
	#region Fields
	private static xCBMConfig config;
	private static RenderTexture renderTex;
	private static xCBMMaterial xCBMMat = new xCBMMaterial();

	// Ref to the xCBMProxy
	private static GameObject myxCBMProxyGO;
	private static xCBMProxy myxCBMProxy;
	private static Camera myxCBMCamera;
	private static xCBMShader myxCBMShader;
	
	// Refs to xCBM windows
	private static xCBMPreviewWindow _myxCBMPreviewWindow;
	private static xCBMGalleryWindow _myxCBMGalleryWindow;
	// Window states
	public static bool PreviewWindowIsAlive = false;
	public static bool GalleryWindowIsAlive = false;
	// helper
	private static double previewHeartbeat;
	private static double galleryHeartbeat;
	private static double checkedPreviewHeartbeat;
	private static double checkedGalleryHeartbeat;
	private static EditorMode lastEditorMode = EditorMode.Other;

	// Ref to GameView
	private static EditorWindow _gameView;
	
	// was the list of SCs changed?
	public static bool AvailScreenCapsChanged = false;
	public static xCBMScreenCap UpdatingScreenCap;

	// time of last change in editor
	private static double lastChangeInEditorTime = 0;
	private static double lastAllScreenCapsUpdatedTime = 0;

	// states
	public static bool GalleryIsUpdating = false;
	public static bool PreviewIsUpdating = false;
	public static bool ScreenCapUpdateInProgress = false; // true from trigger SC-update, wait x frames, until read SC from screen
	public static bool FinalizeScreenCapInProgress = false; // true after all SCs are updated, while GV changes
	private static bool skipNextUpdate = false;
	private static string currentScene = GetCurrentScene(); // used to detect scene switch
	public static bool HideGameViewToggle = false;



	// Delegate to run code before/after ScreenCap update
	public static OnStartScreenCapUpdateDelegate OnStartScreenCapUpdate;
	public static OnPreScreenCapUpdateDelegate OnPreScreenCapUpdate;
	public static OnPostScreenCapUpdateDelegate OnPostScreenCapUpdate;
	public static OnFinalizeScreenCapUpdateDelegate OnFinalizeScreenCapUpdate;
	
	// Delegates
	public delegate void OnStartScreenCapUpdateDelegate();
	public delegate void OnPreScreenCapUpdateDelegate();
	public delegate void OnPostScreenCapUpdateDelegate();
	public delegate void OnFinalizeScreenCapUpdateDelegate();
	#endregion
	
	#region Properties
	public static xCBMConfig Config{
		get {
			if(config == null){
				config = xCBMConfig.InitOrLoad ();
			}
			return config;
		}
	}

	public static List<xCBMScreenCap> AvailScreenCaps{
		get {return Config.AvailScreenCaps;}
		set {Config.AvailScreenCaps = value;}
	}

	public static List<xCBMScreenCap> ActiveScreenCaps{
		get {
			List<xCBMScreenCap> activeScreenCaps = new List<xCBMScreenCap>();
			
			foreach(xCBMScreenCap currScreenCap in AvailScreenCaps){
				if(currScreenCap.Enabled) activeScreenCaps.Add (currScreenCap);
			}
			
			return activeScreenCaps;
		}
	}
	
	public static string[] ScreenCapList{
		get {
			string[] longNameList = new string[ActiveScreenCaps.Count];
			for(int x = 0; x < ActiveScreenCaps.Count; x++){
				longNameList[x] = ActiveScreenCaps[x].LongName;
			}
			return longNameList;
		}
	}
	
	public static xCBMProxy Proxy{
		get {return myxCBMProxy;}
	}
	
	public static GameObject ProxyGO{
		get {return myxCBMProxyGO;}
	}
	
	public static EditorWindow GameView{
		get {
			// get Ref if necessary
			if(!_gameView){
				foreach(EditorWindow curr in Resources.FindObjectsOfTypeAll(typeof(EditorWindow))){
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3|| UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
					if(curr.title == "UnityEditor.GameView") _gameView = curr;
#else
					if(curr.titleContent.text == "Game") _gameView = curr;
#endif
				}
			}
			
			return _gameView;
		}
	}
		
	private static xCBMPreviewWindow myxCBMPreviewWindow{
		get {
			// get Ref if necessary
			if(!_myxCBMPreviewWindow){
				foreach(xCBMPreviewWindow curr in Resources.FindObjectsOfTypeAll(typeof(xCBMPreviewWindow))){
					_myxCBMPreviewWindow = curr;
				}
			}
			
			return _myxCBMPreviewWindow;
		}
	}
	
	private static xCBMGalleryWindow myxCBMGalleryWindow{
		get {
			// get Ref if necessary
			if(!_myxCBMGalleryWindow){
				foreach(xCBMGalleryWindow curr in Resources.FindObjectsOfTypeAll(typeof(xCBMGalleryWindow))){
					_myxCBMGalleryWindow = curr;
				}
			}
			
			return _myxCBMGalleryWindow;
		}
	}


	// skip Update()-Events caused by xCBM (+ GameView has focus)
	public static bool ExecuteUpdate{
		get{
			// execute this update?
			if(
				!skipNextUpdate &&
				!ScreenCapUpdateInProgress &&
				!FinalizeScreenCapInProgress
				)
			{ // use update
				skipNextUpdate = false;
				return true;

			} else { // skip scene change
				skipNextUpdate = false;
				return false;
			}
		}
	}


	// used by other assets
	public static bool UpdateInProgress{
		get{
			if(GalleryIsUpdating == false && PreviewIsUpdating == false && ScreenCapUpdateInProgress == false && FinalizeScreenCapInProgress == false){
				return false;
			} else {
				return true;
			}
		}
	}

	// used to check if other assets are updating
	public static bool OtherUpdateInProgress{
		get{
			// what asset exist and is it updating?
			bool xARMIsUpdating = GetUpdateInProgress("xARMManager", "UpdateInProgress");

			if(xARMIsUpdating){
				skipNextUpdate = true; // skip updates caused by other assets

				return true;
			} else {
				return false;
			}
		}

	}

	// returns Editor's current mode
	public static EditorMode CurrEditorMode{
		get {
			if(!EditorApplication.isPlaying){ // Edit
				return EditorMode.Edit;

			} else if(EditorApplication.isPlaying && EditorApplication.isPaused){ // Pause
				return EditorMode.Pause;

			} else if(EditorApplication.isPlaying && !EditorApplication.isPaused){ // Play
				return EditorMode.Play;

			} else {
				return EditorMode.Other;

			}
		}
	}
	#endregion
	
	#region Functions
	static xCBMManager() {
		InitAvailScreenCaps ();
	}

	#region Init
	private static void InitAvailScreenCaps(){

		// List of all available types of color blindness (ScreenCaps)

		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Normal, 		"unaltered", 	   91.2f,  99.57f, "Unaltered rendering result"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Protanopia, 	"red blind", 		1.3f, 	0.02f, "Difficulties: red vs green and blue vs green"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Protanomaly, 	"red deficiency", 	1.3f, 	0.02f, "Difficulties: red vs green and blue vs green"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Deuteranopia, 	"green blind", 		1.2f, 	0.01f, "Difficulties: red vs green and blue vs green"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Deuteranomaly, 	"green deficiency", 5.0f, 	0.35f, "Difficulties: red vs green and blue vs green"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Tritanopia, 	"blue blind", 		0.001f, 0.03f, "Difficulties: blue vs green and yellow vs red/violet"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Tritanomaly, 	"blue deficiency", 	0.0001f,0.0001f, "Difficulties: blue vs green and yellow vs red/violet"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Achromatopsia, 	"color blind", 		-1f, -1f, "Very rare. Difficulties: distinguish between colors"));
		addOrUpdateScreenCap (new xCBMScreenCap (xCBMColorBlindnessType.Achromatomaly, 	"color deficiency", -1f, -1f, "Very rare. Difficulties: distinguish between colors"));

		// mark loaded SC list as unchanged to prevent resave
		xCBMScreenCap.ListChanged = false;
	}
	
	private static void addOrUpdateScreenCap (xCBMScreenCap screenCapToAdd){
		int screenCapIndex = getScreenCapIndex (screenCapToAdd);
		
		if(screenCapIndex >= 0){ // update (don't add duplicates)
			// values to keep
			bool origEnabledState = AvailScreenCaps[screenCapIndex].Enabled;
			
			AvailScreenCaps[screenCapIndex] = screenCapToAdd;
			AvailScreenCaps[screenCapIndex].Enabled = origEnabledState;
			
		} else { // add
			AvailScreenCaps.Add (screenCapToAdd);
		}
	}

	private static void addOrReplaceScreenCap (xCBMScreenCap screenCapToAdd, xCBMScreenCap screenCapToReplace){
		int screenCapIndexToReplace = getScreenCapIndex (screenCapToReplace);
		int screenCapIndexToAdd = getScreenCapIndex (screenCapToAdd);
		
		if(screenCapIndexToReplace >= 0){ // replace
			// values to keep
			bool origEnabledState = AvailScreenCaps[screenCapIndexToReplace].Enabled;
			
			AvailScreenCaps[screenCapIndexToReplace] = screenCapToAdd;
			AvailScreenCaps[screenCapIndexToReplace].Enabled = origEnabledState;
			
		} else if(screenCapIndexToAdd >= 0){ // update (don't add duplicates)
			// values to keep
			bool origEnabledState = AvailScreenCaps[screenCapIndexToAdd].Enabled;
			
			AvailScreenCaps[screenCapIndexToAdd] = screenCapToAdd;
			AvailScreenCaps[screenCapIndexToAdd].Enabled = origEnabledState;

		} else { // add
			AvailScreenCaps.Add (screenCapToAdd);
		}
	}
	
	private static int getScreenCapIndex(xCBMScreenCap ScreenCapToCheck){
		for (int x= 0; x< AvailScreenCaps.Count; x++){
			if(AvailScreenCaps[x] == ScreenCapToCheck) return x;
		}
		
		return -1;
	}
	#endregion
	
	#region Proxy
	public static void CreateProxyGO (){
		// create Proxy only if needed and not while switching to play mode 
		if(!myxCBMProxyGO && (EditorApplication.isPlaying || !EditorApplication.isPlayingOrWillChangePlaymode)){
			// create GO with components attached
			myxCBMProxyGO = new GameObject("xCBMProxy");
			myxCBMProxy = myxCBMProxyGO.AddComponent<xCBMProxy> ();
			myxCBMCamera = myxCBMProxyGO.AddComponent<Camera> ();
			myxCBMCamera.enabled = false;

			if(!myxCBMProxy){
				RemoveProxyGO ();
				xCBMPreviewWindow.WarningBoxText = "Could not create xCBMProxy. Do NOT put xCBM in the Editor folder.";
				xCBMGalleryWindow.WarningBoxText = "Could not create xCBMProxy. Do NOT put xCBM in the Editor folder.";
			}
		}
	}
	
	public static void RemoveProxyGO (){
		MonoBehaviour.DestroyImmediate (myxCBMProxyGO);
	}

	public static void RecreateProxyGO (){
		RemoveProxyGO();
		CreateProxyGO();
	}

	// reset xCBM values on scene switch
	public static void ResetOnSceneSwitch(){
		if (currentScene != GetCurrentScene()){
			currentScene = GetCurrentScene();
			// reset
			ScreenCapUpdateInProgress = false;
			SceneChanged ();
		}
	}
	#endregion
	
	#region ScreenCaps
	public static bool IsToUpdate(xCBMScreenCap screenCap){
		if(screenCap.LastUpdateTime != lastChangeInEditorTime && screenCap.LastUpdateTryTime != lastChangeInEditorTime){
			return true;
		} else {
			return false;
		}
	}
	

	public static void UpdateScreenCap(xCBMScreenCap screenCap){
		// make current ScreenCap available to delegates
		UpdatingScreenCap = screenCap;

		// run custom code
		if(OnPreScreenCapUpdate != null) OnPreScreenCapUpdate ();

		xCBMManager.ScreenCapUpdateInProgress = true;

		// wait x frames to ensure correct results with other (lazy) plugins
		if(xCBMManager.CurrEditorMode == EditorMode.Play){ // don't wait in Play mode
			Proxy.StartWaitXFramesCoroutine (screenCap, 0);
			
		} else {
			Proxy.StartWaitXFramesCoroutine (screenCap, xCBMManager.Config.FramesToWait);
		}
	}

	public static void UpdateScreenCapAtEOF(xCBMScreenCap screenCap){
		// capture Render at EndOfFrame
		Proxy.StartUpdateScreenCapCoroutine (screenCap);
		// force EndOfFrame - to execute yield
		GameView.Repaint ();
	}

	// coroutine to render all cameras to RT with xCBM shader and get the result at end of the frame
	public static IEnumerator ReadScreenCap(xCBMScreenCap screenCap){
		int width = Screen.width;
		int height = Screen.height;
		renderTex = new RenderTexture(width, height, 24);

		// render only to RT and not to screen
		RenderTexture prevActiveRenderTex = RenderTexture.active;
		RenderTexture.active = renderTex;

		// render all cameras
		Camera[] cams = new Camera[Camera.allCamerasCount];
		Camera.GetAllCameras (cams);
		foreach(Camera cam in cams){
			if(cam.targetTexture == null){
				// copy cam settings
				myxCBMCamera.CopyFrom (cam);
				// render to texture
				myxCBMCamera.targetTexture = renderTex;
				myxCBMCamera.Render();
			}
		}

		yield return new WaitForEndOfFrame(); // wait until rendering is done

		yield return new WaitForEndOfFrame(); // improves performance

		Texture2D screenTex = new Texture2D(width, height, TextureFormat.RGB24, false);

		if(screenCap.Type != xCBMColorBlindnessType.Normal){

			if(Config.UseWorkaroundUGuiCanvasScreenSpace){ // Workaround for RenderTexture - uGUI screen space canvas bug
				screenTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
				screenTex.Apply (false, false);

				// apply shader variant to Tex2D
				Graphics.Blit(screenTex, renderTex, xCBMMat[(int)screenCap.Type]);
			} else {
				// apply shader variant to RT
				Graphics.Blit(renderTex, renderTex, xCBMMat[(int)screenCap.Type]);
			}
		}

		screenTex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
		screenTex.Apply (false, false); // readable to enable export as file
		screenCap.Texture = screenTex;

		// Restore previously active RenderTexture
		RenderTexture.active = prevActiveRenderTex;
		
		screenCap.LastUpdateTime = lastChangeInEditorTime;
		screenCap.LastUpdateTryTime = lastChangeInEditorTime;
		screenCap.UpdatedSuccessful = true;

		// repaint editor windows
		if(myxCBMPreviewWindow) myxCBMPreviewWindow.Repaint ();
		if(myxCBMGalleryWindow) myxCBMGalleryWindow.Repaint ();
		
		// run custom code
		if(OnPostScreenCapUpdate != null) OnPostScreenCapUpdate ();

		xCBMManager.ScreenCapUpdateInProgress = false;
	}
	
	public static void FinalizeScreenCapUpdate(){
		// unload unused ScreenCaps
		Resources.UnloadUnusedAssets ();

		// LiveUpdate - remove help message if it's not longer relevant
		if(AllScreenCapsUpdatedSuccesfull()){
			xCBMPreviewWindow.WarningBoxText = "";
			xCBMGalleryWindow.WarningBoxText = "";
		}
		
		// run custom code
		if(OnFinalizeScreenCapUpdate != null) OnFinalizeScreenCapUpdate ();
	}

	public static bool AllScreenCapsUpdatedRecently(){
		// no recent scene change?
		if(lastAllScreenCapsUpdatedTime == lastChangeInEditorTime) return false;
		
		// update still in progress?
		foreach(xCBMScreenCap currScreenCap in ActiveScreenCaps){
			if(currScreenCap.LastUpdateTryTime != lastChangeInEditorTime) return false;
		}
		
		return true;
	}

	public static void SetAllScreenCapsUpdated(){
		lastAllScreenCapsUpdatedTime = lastChangeInEditorTime;
	}
	
	private static bool AllScreenCapsUpdatedSuccesfull(){
		foreach(xCBMScreenCap currScreenCap in ActiveScreenCaps){
			if(!currScreenCap.UpdatedSuccessful) return false;
		}
		return true;
	}
	
	#endregion
	
	#region GameView
	public static void EnsureNextFrame (){
		// in Editor mode - fake scene change
		if(xCBMManager.CurrEditorMode == EditorMode.Edit){
			if(Proxy){
				// add random rotation to fake scene change (Proxy-HideFlag has to be None)
				Proxy.gameObject.transform.rotation = Random.rotation;
			}
		}

		// in Play mode - frames are rolling by

		// in Pause mode - Step
		if(xCBMManager.CurrEditorMode == EditorMode.Pause){
			EditorApplication.Step ();
		}
	}


	private static string GetCurrentScene(){
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		return EditorApplication.currentScene; 
#else
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#endif
	}

	public static void SceneChanged(){
		lastChangeInEditorTime = EditorApplication.timeSinceStartup;
	}
	#endregion

	#region Preview and Gallery tools
	public static void RepaintAllWindows (){
		xCBMPreviewWindow.RepaintNextUpdate = true;
		xCBMGalleryWindow.RepaintNextUpdate = true;
	}


	public static void SendHeartbeatPreview(){
		// save heartbeat
		previewHeartbeat = EditorApplication.timeSinceStartup;
	}

	public static void SendHeartbeatGallery(){
		// save heartbeat
		galleryHeartbeat = EditorApplication.timeSinceStartup;
	}

	public static void AbortMyUpdates(){
		AbortUpdatePreview();
		AbortUpdateGallery();
	}

	private static void AbortUpdatePreview(){
		AbortUpdate(ref PreviewIsUpdating);
	}

	private static void AbortUpdateGallery(){
		AbortUpdate(ref GalleryIsUpdating);
	}

	private static void AbortUpdate(ref bool windowIsUpdating){
		// rest all states
		windowIsUpdating = false;
		ScreenCapUpdateInProgress = false;
		FinalizeScreenCapUpdate ();
		FinalizeScreenCapInProgress = false;
	}
		
	//check if a window was deactivated (e.g. tab is in background)
	public static void CheckHeartbeat(){
		// time of last heartbeat unchanged & was alive last time = just got missing
		if(previewHeartbeat == checkedPreviewHeartbeat && PreviewWindowIsAlive){
			PreviewWindowIsAlive = false;
			AbortUpdatePreview();

		} else if(previewHeartbeat != checkedPreviewHeartbeat) {
			PreviewWindowIsAlive = true;
			checkedPreviewHeartbeat = previewHeartbeat;
		}

		if(galleryHeartbeat == checkedGalleryHeartbeat && GalleryWindowIsAlive){
			GalleryWindowIsAlive = false;
			AbortUpdateGallery();

		} else if(galleryHeartbeat != checkedGalleryHeartbeat) {
			GalleryWindowIsAlive = true;
			checkedGalleryHeartbeat = galleryHeartbeat;
		}
	}

	public static void AbortUpdatesOnEditorModeChange(){
		if(CurrEditorMode != lastEditorMode){
			AbortMyUpdates(); // prevents update geting stuck on Play>Edit switch
			lastEditorMode = CurrEditorMode;
		}
	}
	#endregion

	#region Other assets

	// get state of other asset
	private static bool GetUpdateInProgress(string reflectType, string reflectProperty){

		System.Type type = System.Type.GetType(reflectType);
		if(type == null) return false;

		System.Reflection.PropertyInfo property = type.GetProperty(reflectProperty);
		if(property == null) return false;

		return (bool)property.GetValue(type, null);
	}
	#endregion

	#endregion
}
#endif