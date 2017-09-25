#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xCBM;


public class xCBMGalleryWindow : EditorWindow {
	
	#region Fields
	// Ref to itself
	private static xCBMGalleryWindow _myWindow;
	private static string myWindowTitle = "xCBM Gallery";
	
	// cache of all active ScreenCaps
	private static List<xCBMScreenCap> activeScreenCaps = new List<xCBMScreenCap>();
	// which ScreenCap is next to update (update only one ScreenCap at a time/Update())
	private static int currScreenCapIndex = 0;
	// count of screenCaps to update once (used by the update1x-button)
	private static int screenCapsToUpdateOnce = 0;
	private static bool updateOnce = false;
	private static int postUpdateFramesToGo = 0;
	// limit update interval
	private double lastUpdateTime = 0.0f;
	private static float updateInterval;
	
	// GUI help (error, warning, info) to display
	public static string WarningBoxText = "";
	public static string InfoBoxText = "";
	
	// GUI look
	public static bool RepaintNextUpdate = false;
	private static Vector2 scrollPos;
	private static GUIStyle labelStyle = new GUIStyle();
	
	private static int screenCapMinWidth = 160;
	private static int screenCapMinHeight = 160;

	#endregion
	
	#region Properties
	private static xCBMGalleryWindow myWindow{
		get {
			// refresh Ref if it's lost
			if(!_myWindow) _myWindow = GetWindow<xCBMGalleryWindow>();
			return _myWindow;
		}
	}
	#endregion
		
	#region Functions
	// open GalleryWindow
	[MenuItem ("Window/xCBM/xCBM Gallery", false, 120)]
    public static void ShowxCBMGalleryWindow() {
			_myWindow = GetWindow<xCBMGalleryWindow>();
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3|| UNITY_4_5 || UNITY_4_6 || UNITY_5_0
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

		// Init GUI look
		labelStyle.alignment = TextAnchor.UpperCenter;
		
		// ensure correct display after Edit->Pause/Play mode switch
		RepaintNextUpdate = true;
	}
	
	// update one ScreenCap per Update()
	void Update(){
		// send heartbeat
		xCBMManager.SendHeartbeatGallery();

		// ensures a xCBM Proxy exists
		xCBMManager.CreateProxyGO ();

		// finalize SC update (set default GV size etc.)
		if(xCBMManager.AllScreenCapsUpdatedRecently() && !xCBMManager.FinalizeScreenCapInProgress){
			// reset 1xUpdate
			if(updateOnce) {
				screenCapsToUpdateOnce = 0;
				updateOnce = false;
			}

			xCBMManager.GalleryIsUpdating = false;
			xCBMManager.FinalizeScreenCapInProgress = true;
			xCBMManager.FinalizeScreenCapUpdate ();
		}
		// ensure some frames after default resolution is set
		else if(xCBMManager.AllScreenCapsUpdatedRecently() && xCBMManager.FinalizeScreenCapInProgress){
			// still frames to go?
			if(postUpdateFramesToGo <= 0){
				xCBMManager.SetAllScreenCapsUpdated();

				xCBMManager.FinalizeScreenCapInProgress = false;
			} else {
				xCBMManager.EnsureNextFrame ();
				postUpdateFramesToGo--;
			}
		}
		// auto update or 1xUpdate
		else if(((xCBMManager.Config.GalleryAutoUpdateInEditorMode && xCBMManager.CurrEditorMode == EditorMode.Edit) || // Editor mode
				(xCBMManager.Config.GalleryAutoUpdateInPauseMode && xCBMManager.CurrEditorMode == EditorMode.Pause) || // Pause mode
				(xCBMManager.Config.GalleryAutoUpdateInPlayMode && xCBMManager.CurrEditorMode == EditorMode.Play) || // Play
		    	updateOnce) && // 1x Update
			!xCBMManager.ScreenCapUpdateInProgress)
		{ // update ScreenCap

			// limit SC-block updates
			if(!updateOnce && !xCBMManager.GalleryIsUpdating && xCBMManager.CurrEditorMode == EditorMode.Edit){ // limit auto update in Edit mode (only for SC-blocks)
				updateInterval = 1f / xCBMManager.Config.GalleryUpdateIntervalLimitEdit;

			} else if(!updateOnce && !xCBMManager.GalleryIsUpdating && xCBMManager.CurrEditorMode == EditorMode.Play){ // limit auto update in Play mode (only for SC-blocks)
				updateInterval = 1f / xCBMManager.Config.GalleryUpdateIntervalLimitPlay;

			} else {
				updateInterval = 0f; // do not limit update interval
			}
			
			if(EditorApplication.timeSinceStartup > lastUpdateTime + updateInterval){ // limit updates per sec
				lastUpdateTime = EditorApplication.timeSinceStartup;

				// cache list
				activeScreenCaps = xCBMManager.ActiveScreenCaps;

				// execute update?
				if(activeScreenCaps.Count != 0 && xCBMManager.ProxyGO && !xCBMManager.PreviewIsUpdating && !xCBMManager.OtherUpdateInProgress){
					// find next ScreenCap to update
					if(currScreenCapIndex >= activeScreenCaps.Count) currScreenCapIndex = 0;
					xCBMScreenCap currScreenCap = activeScreenCaps[currScreenCapIndex];
					
					// update ScreenCap only if it's outdated (try one time) OR update1x is clicked
					if(xCBMManager.IsToUpdate(currScreenCap) || updateOnce){
						if(xCBMManager.GalleryIsUpdating == false && xCBMManager.OnStartScreenCapUpdate != null) xCBMManager.OnStartScreenCapUpdate ();

						xCBMManager.GalleryIsUpdating = true;
						xCBMManager.UpdateScreenCap (currScreenCap);
						
						currScreenCapIndex++;
						
						// decrease Update1x-to-go count
						if(updateOnce && screenCapsToUpdateOnce > 0){
							screenCapsToUpdateOnce--;
						}

						// number os frames to step after all SCs are updated
						postUpdateFramesToGo = xCBMManager.Config.FramesToWait;
						
					} else { // skip SC
						currScreenCapIndex++;
					}
				}
			}
		}
		
		// ensure a next frame while waiting for one
		if(xCBMManager.ScreenCapUpdateInProgress && xCBMManager.GalleryIsUpdating && xCBMManager.GalleryWindowIsAlive) xCBMManager.EnsureNextFrame ();

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

	void OnGUI (){ // is only triggered by Repaint after SC update
		// cache list
		activeScreenCaps = xCBMManager.ActiveScreenCaps;
		
		DrawControls ();
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
		DrawGallery ();
		EditorGUILayout.EndScrollView ();
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
		
		GUILayout.Label ("Update:", EditorStyles.toolbarButton);
		if(GUILayout.Button ("1x", EditorStyles.toolbarButton)) {
			updateOnce = true;
			screenCapsToUpdateOnce = activeScreenCaps.Count;
			xCBMManager.SceneChanged (); // fake scene change
		}
		xCBMManager.Config.GalleryAutoUpdateInEditorMode = GUILayout.Toggle (xCBMManager.Config.GalleryAutoUpdateInEditorMode, "Edit", EditorStyles.toolbarButton);
		xCBMManager.Config.GalleryAutoUpdateInPauseMode = GUILayout.Toggle (xCBMManager.Config.GalleryAutoUpdateInPauseMode, "Pause", EditorStyles.toolbarButton);
		xCBMManager.Config.GalleryAutoUpdateInPlayMode = GUILayout.Toggle (xCBMManager.Config.GalleryAutoUpdateInPlayMode, "Play", EditorStyles.toolbarButton);
		EditorGUILayout.Space ();
		GUILayout.FlexibleSpace ();
		// ScreenCaps per row silder
		xCBMManager.Config.GalleryScreenCapsPerRow = Mathf.RoundToInt (GUILayout.HorizontalSlider (xCBMManager.Config.GalleryScreenCapsPerRow, 1, Mathf.Max (activeScreenCaps.Count, 1), 
			GUILayout.ExpandWidth (true), GUILayout.MinWidth (40), GUILayout.MaxWidth (100)));

		// Tools foldout
		GUILayout.Label ("Tools", EditorStyles.toolbarButton);
		switch (EditorGUILayout.Popup (-1, new string[1] {"Options"}, EditorStyles.toolbarDropDown, GUILayout.Width(15))){
		case 0: // open Options window
			xCBMOptionsWindow.DisplayWizard ();
			break;

		}
		EditorGUILayout.EndHorizontal ();

		if(WarningBoxText != "") EditorGUILayout.HelpBox (WarningBoxText, MessageType.Warning, true); // Unity 3.3 Error
		if(InfoBoxText != "") EditorGUILayout.HelpBox (InfoBoxText, MessageType.Info, true); // Unity 3.3 Error
	}
	
	
	private static void DrawGallery (){
		int entriesPerRow = xCBMManager.Config.GalleryScreenCapsPerRow;

		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginHorizontal ();
		
		for(int x=0; x < activeScreenCaps.Count; x++){
			xCBMScreenCap currScreenCap = activeScreenCaps[x];
			
			// begin new row?
			if(x % entriesPerRow == 0){
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
			}
				
			DrawScreenCap (currScreenCap, x);

			// last entry?
			if(x == activeScreenCaps.Count - 1){

				// fill last row to ensure same/correct size of last row entries
				int entriesInLastRow = activeScreenCaps.Count % entriesPerRow;

				if(entriesInLastRow < entriesPerRow && entriesInLastRow != 0 && !xCBMManager.Config.UseFixedScreenCapSize){
					int entriesToFill = entriesPerRow - entriesInLastRow;

					// add invisible dummy entries
					for(int y=0; y < entriesToFill; y++){
						// use same layout cammands as below for similar spacing

						EditorGUILayout.BeginVertical ();
						float aspect = (float)currScreenCap.Texture.width / currScreenCap.Texture.height;
						GUILayoutUtility.GetAspectRect (aspect , GUILayout.MinWidth (screenCapMinWidth), GUILayout.MinHeight (screenCapMinHeight));

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
					}

				}
			}

		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		
	}


	private static void DrawScreenCap (xCBMScreenCap screenCap, int screenCapIndex){
		Rect screenCapRect, screenCapBox;

		// create Box and Rect
		if(xCBMManager.Config.UseFixedScreenCapSize){
			screenCapBox = EditorGUILayout.BeginVertical ("box", GUILayout.Width (xCBMManager.Config.FixedScreenCapSize.x), GUILayout.Height (xCBMManager.Config.FixedScreenCapSize.y));
			screenCapRect = GUILayoutUtility.GetRect (xCBMManager.Config.FixedScreenCapSize.x, xCBMManager.Config.FixedScreenCapSize.y);

		} else {
			screenCapBox =  EditorGUILayout.BeginVertical ("box");
			float aspect = (float)screenCap.Texture.width / screenCap.Texture.height;
			screenCapRect = GUILayoutUtility.GetAspectRect (aspect, GUILayout.MinWidth (screenCapMinWidth), GUILayout.MinHeight (screenCapMinHeight));
		}

		// draw SC
		if(screenCap.UpdatedSuccessful){
			// draw ScreenCap in prepared rect (use EditorGUI.DrawTextureTransparent to draw it without Play mode tint)
			EditorGUI.DrawTextureTransparent (screenCapRect, screenCap.Texture, ScaleMode.ScaleToFit);

			// select SC in xCBM Preview on click
			if(screenCapBox.Contains (Event.current.mousePosition)){
				if(Event.current.type == EventType.MouseDown && Event.current.button == 0){
					xCBMPreviewWindow.ChangeActiveScreenCap (screenCapIndex);
				}
			}

		} else {
			EditorGUILayout.HelpBox ("ScreenCap could not been updated", MessageType.Warning, true);
		}
		
		// description
		GUILayout.Label (screenCap.Name + "\n(" + screenCap.TypeColl + ")", labelStyle);

		EditorGUILayout.EndVertical ();
	}
	#endregion
	
	#endregion
}
#endif