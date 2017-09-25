#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xCBM;

public class xCBMPreviewWindow : EditorWindow {

	#region Fields
	// Ref to itself
	private static xCBMPreviewWindow _myWindow;
	private static string myWindowTitle = "xCBM Preview";
	private static int toolbarOffset = 17;
	
	// cache of all active ScreenCaps
	private static List<xCBMScreenCap> activeScreenCaps = new List<xCBMScreenCap>();
	public static xCBMScreenCap SelectedScreenCap;
	private static bool updateOnce = false;
//	private static int postGameViewResizeFramesToGo = 0;
	// limit update interval
	private double lastUpdateTime = 0.0f;
	private static float updateInterval;
	// switch Game View to new default resolution on next update
//	private static bool resizeGameViewToNewDefault = false;

	// GUI
	public static bool RepaintNextUpdate = false;
	private static Vector2 scrollPos;
	
	// GUI help (error, warning, info) to display
	public static string WarningBoxText = "";
	public static string InfoBoxText = "";
	#endregion
	
	#region Properties
	private static xCBMPreviewWindow myWindow{
		get {
			// refresh Ref if it's lost
			if(!_myWindow) _myWindow = GetWindow<xCBMPreviewWindow>();
			return _myWindow;
		}
	}
	#endregion
	
	#region Functions
	[MenuItem ("Window/xCBM/xCBM Preview", false, 119)]
    public static void ShowxCBMPreviewWindow() {
			_myWindow = GetWindow<xCBMPreviewWindow>();
#if UNITY_5_0
			_myWindow.title = myWindowTitle;
#else
			_myWindow.titleContent = new GUIContent(myWindowTitle);
#endif
			_myWindow.minSize = new Vector2(300, 150);
    }
	
	#region OnMessage
	void OnEnable (){
		// Init everything
		xCBMManager.CreateProxyGO ();
	
		// ensure correct display after Edit->Pause/Play mode switch
		RepaintNextUpdate = true;
	}
	
	void Update(){
		// send heartbeat
		xCBMManager.SendHeartbeatPreview();

		// ensures a xCBM Proxy exists
		xCBMManager.CreateProxyGO ();

		// finalize SC update
		if(xCBMManager.PreviewIsUpdating && !xCBMManager.ScreenCapUpdateInProgress){
			xCBMManager.PreviewIsUpdating = false;
			xCBMManager.FinalizeScreenCapInProgress = true;
			xCBMManager.FinalizeScreenCapUpdate ();
			xCBMManager.FinalizeScreenCapInProgress = false;

		}

		// update SC if auto update is enabled or Update1x was clicked
		if(((xCBMManager.Config.PreviewAutoUpdateInEditorMode && xCBMManager.CurrEditorMode == EditorMode.Edit) || // Edit
		    (xCBMManager.Config.PreviewAutoUpdateInPauseMode && xCBMManager.CurrEditorMode == EditorMode.Pause) || // Pause
		    (xCBMManager.Config.PreviewAutoUpdateInPlayMode && xCBMManager.CurrEditorMode == EditorMode.Play) || // Play
		    updateOnce) &&
			!xCBMManager.GalleryIsUpdating && 
			!xCBMManager.ScreenCapUpdateInProgress &&
			!xCBMManager.OtherUpdateInProgress)
		{
			
			// limit SC updates
			if(!updateOnce && xCBMManager.CurrEditorMode == EditorMode.Edit){ // limit auto update in Edit mode
				updateInterval = 1f / xCBMManager.Config.PreviewUpdateIntervalLimitEdit;

			} else if(!updateOnce && xCBMManager.CurrEditorMode == EditorMode.Play){ // limit auto update in Play mode
				updateInterval = 1f / xCBMManager.Config.PreviewUpdateIntervalLimitPlay;

			} else {
				updateInterval = 0f; // do not limit update interval
			}

			if(EditorApplication.timeSinceStartup > lastUpdateTime + updateInterval){ // limit updates per sec
				lastUpdateTime = EditorApplication.timeSinceStartup;
				if(SelectedScreenCap is xCBMScreenCap && xCBMManager.ProxyGO /*&& !xCBMManager.GalleryIsUpdating*/){ 
					xCBMScreenCap currScreenCap = SelectedScreenCap;
					
					// update ScreenCap only if it's outdated (try one time) OR update1x is clicked
					if(xCBMManager.IsToUpdate(currScreenCap) || updateOnce){
						// run custom code
						if(xCBMManager.PreviewIsUpdating == false && xCBMManager.OnStartScreenCapUpdate != null) xCBMManager.OnStartScreenCapUpdate ();

						xCBMManager.PreviewIsUpdating = true;
						xCBMManager.UpdateScreenCap (currScreenCap);
						updateOnce = false;
					}
				}
			}
		}

		// ensure a next frame while waiting for one
		if(xCBMManager.ScreenCapUpdateInProgress && xCBMManager.PreviewIsUpdating && xCBMManager.PreviewWindowIsAlive){ // while SC update
			xCBMManager.EnsureNextFrame ();
		}

	 	if(activeScreenCaps.Count == 0){
			InfoBoxText = "No ScreenCaps active. Open xCBM Options to activate color blindness types.";
		} else {
			InfoBoxText = "";
		}


		// repaint window
		if(RepaintNextUpdate){
			myWindow.Repaint ();
			RepaintNextUpdate = false;
		}
	}
	
	
	void OnGUI () { // is only triggered by Repaint after SC update
		// cache list
		activeScreenCaps = xCBMManager.ActiveScreenCaps;
		
		DrawControls ();
		DrawPreview ();
	}


	void OnDestroy (){ // isn't executed on Editor close
		// cleanup
		xCBMManager.RemoveProxyGO ();
	}
	#endregion
	
	#region Draw
	private static void DrawControls (){
		
		// draw toolbar filling the whole window width
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.MaxWidth(myWindow.position.width));
		
		ChangeActiveScreenCap(EditorGUILayout.Popup (xCBMManager.Config.PreviewSelectedScreenCapIndex, xCBMManager.ScreenCapList, EditorStyles.toolbarDropDown, GUILayout.MaxWidth (250)));
		EditorGUILayout.Space ();

		GUILayout.Label ("Update:", EditorStyles.toolbarButton);
		if(GUILayout.Button ("1x", EditorStyles.toolbarButton)){
			updateOnce = true;
		}
		xCBMManager.Config.PreviewAutoUpdateInEditorMode = GUILayout.Toggle (xCBMManager.Config.PreviewAutoUpdateInEditorMode, "Edit", EditorStyles.toolbarButton);
		xCBMManager.Config.PreviewAutoUpdateInPauseMode = GUILayout.Toggle (xCBMManager.Config.PreviewAutoUpdateInPauseMode, "Pause", EditorStyles.toolbarButton);
		xCBMManager.Config.PreviewAutoUpdateInPlayMode = GUILayout.Toggle (xCBMManager.Config.PreviewAutoUpdateInPlayMode, "Play", EditorStyles.toolbarButton);
		EditorGUILayout.Space ();

		xCBMManager.Config.PreviewOneToOnePixel = GUILayout.Toggle (xCBMManager.Config.PreviewOneToOnePixel, "1:1px", EditorStyles.toolbarButton);
		EditorGUILayout.Space ();
		GUILayout.FlexibleSpace ();

		// Tools foldout
		GUILayout.Label ("Tools", EditorStyles.toolbarButton);
		switch (EditorGUILayout.Popup (-1, new string[1] {"Options"}, EditorStyles.toolbarDropDown, GUILayout.Width(15))){
		case 0: // open Options window
			xCBMOptionsWindow.DisplayWizard ();
			break;

		}
		EditorGUILayout.EndHorizontal ();
		
		if(WarningBoxText != "") EditorGUILayout.HelpBox (WarningBoxText, MessageType.Warning, true);
		if(InfoBoxText != "") EditorGUILayout.HelpBox (InfoBoxText, MessageType.Info, true);
	}
	
	
	private static void DrawPreview(){
		if(activeScreenCaps.Count > 0){
			
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			
			// center preview
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

			
			Rect screenCapRect;
			if(xCBMManager.Config.PreviewOneToOnePixel){ // 1:1px
				screenCapRect = GUILayoutUtility.GetRect (SelectedScreenCap.Texture.width, SelectedScreenCap.Texture.height, 
					GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
				
			} else { // scale to fit
				screenCapRect = GUILayoutUtility.GetRect (myWindow.position.width, myWindow.position.height - toolbarOffset);
			}

			// draw ScreenCap in prepared rect (use EditorGUI.DrawTextureTransparent to draw it without Play mode tint)
			EditorGUI.DrawTextureTransparent (screenCapRect, SelectedScreenCap.Texture, ScaleMode.ScaleToFit);

			// center preview
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();

			EditorGUILayout.EndScrollView ();
		}
	}

	public static void ChangeActiveScreenCap (int screenCapIndex){
		// ensure correct index
		if(screenCapIndex >= activeScreenCaps.Count){
		 	screenCapIndex = 0; 
		} else if(screenCapIndex < 0){
			screenCapIndex = activeScreenCaps.Count - 1;
		}
		xCBMManager.Config.PreviewSelectedScreenCapIndex = screenCapIndex;

		// repaint windows on next update
		xCBMManager.RepaintAllWindows();

		if(activeScreenCaps.Count > 0){

			SelectedScreenCap = activeScreenCaps[screenCapIndex]; // get Ref to selected SC
		}
	}

	public static void SelectNextScreencap (){
		ChangeActiveScreenCap (xCBMManager.Config.PreviewSelectedScreenCapIndex + 1);
	}

	public static void SelectPreviousScreencap (){
		ChangeActiveScreenCap (xCBMManager.Config.PreviewSelectedScreenCapIndex - 1);
	}

	#endregion
	
	#endregion
}
#endif