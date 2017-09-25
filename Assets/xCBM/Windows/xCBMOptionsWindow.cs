#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using xCBM;

public class xCBMOptionsWindow : ScriptableWizard {
	
	#region Fields
	// Ref to itself
	private static xCBMOptionsWindow _myWindow;
	
	// GUI look
	private static Vector2 scrollPos;
	private static bool prevToggleState;
	
	// sizes
	private static int spacing = 10;
	private static int labelWidthEnabled = 36;
	private static int labelWidthName = 100;
	private static int labelWidthStats = 100;
	private static int labelMinWidthDescription = 100;
	
	// unicode chars
	private static char charAsc = '\u25B2';
	private static char charDesc = '\u25BC';
	
	// last sort
	private static Func<xCBMScreenCap, object> lastSortedColumn;
	private static bool lastSortWasAsc = false;
	#endregion
	
	#region Properties
	#endregion
	
	#region Functions
	[MenuItem ("Window/xCBM/xCBM Options", false, 150)]
	public static void DisplayWizard (){
		_myWindow = EditorWindow.GetWindow<xCBMOptionsWindow>(true, "xCBM Options", true);
		_myWindow.minSize = new Vector2(350, 150);
    }

	
	void OnGUI (){
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

		DrawOptions ();
		EditorGUILayout.Space ();
		DrawScreenCapGroups ();
		EditorGUILayout.Space ();
		DrawFooter ();
		
		EditorGUILayout.EndScrollView ();
	}
	
	#region Draw
	private static void DrawOptions (){
		int blockWidth = 400;

		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.BeginVertical ("box", GUILayout.Width(blockWidth));

		xCBMManager.Config.FoldoutContact = Foldout (xCBMManager.Config.FoldoutContact, "Contact", true, EditorStyles.foldout);
		if(xCBMManager.Config.FoldoutContact){

			GUIStyle style = new GUIStyle();
			style.fontStyle = FontStyle.Italic;

			GUILayout.Label ("Thanks for purchasing xCBM.", style, GUILayout.MaxWidth(blockWidth));
			GUILayout.Label ("Feedback and feature requests are highly appreciated.", style, GUILayout.MaxWidth(blockWidth));
			GUILayout.Label ("Please take a moment to give xCBM a review at the Asset Store.", style, GUILayout.MaxWidth(blockWidth));
			GUILayout.Label ("Thanks!", style, GUILayout.MaxWidth(blockWidth));

			EditorGUILayout.Space();
			GUILayout.Label ("Support & Feedback:");
			if(GUILayout.Button("Unity Forum: xCBM Thread", GUILayout.Width(blockWidth / 2))) Application.OpenURL("http://forum.unity3d.com/threads/xcbm-color-blindness-master.396978/");
			if(GUILayout.Button("Email: support@flyingwhale.de", GUILayout.Width(blockWidth / 2))) Application.OpenURL("mailto:support@flyingwhale.de");
			EditorGUILayout.Space();

			GUILayout.Label ("Infos & Updates:");
			if(GUILayout.Button("Twitter: @ThavronFW", GUILayout.Width(blockWidth / 2))) Application.OpenURL("https://twitter.com/ThavronFW");
			if(GUILayout.Button("YouTube: Flying Whale", GUILayout.Width(blockWidth / 2))) Application.OpenURL("https://www.youtube.com/channel/UC2CU8aCaWclJ5C6dOQFzdVg");
			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("All assets by Flying Whale", GUILayout.Width(blockWidth / 2))) Application.OpenURL("http://u3d.as/5aJ");
			EditorGUILayout.BeginVertical();
			GUILayout.Space(8);
			GUILayout.Label ("more comming soon...", style, GUILayout.MaxWidth(blockWidth));
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(4);
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical ("box");
		GUILayout.Label ("Options", EditorStyles.boldLabel, GUILayout.MaxWidth(blockWidth));

		// Global
		EditorGUILayout.BeginVertical ("box", GUILayout.Width(blockWidth));
		xCBMManager.Config.FoldoutOptions = Foldout (xCBMManager.Config.FoldoutOptions, "Global Options", true, EditorStyles.foldout);
		if(xCBMManager.Config.FoldoutOptions){

			GUILayout.Label ("Update Delay", EditorStyles.boldLabel);
			GUILayout.Label ("Frames to wait between update start and rendering");
			GUILayout.Label ("(in Editor and Pause mode)");
			xCBMManager.Config.FramesToWait = EditorGUILayout.IntField ("Frames (default: 1)", xCBMManager.Config.FramesToWait);
			EditorGUILayout.Space ();

			GUILayout.Label ("Rendering Mode", EditorStyles.boldLabel);
			GUILayout.Label ("Only change if some parts aren't visible on the previews");
			xCBMManager.Config.UseWorkaroundUGuiCanvasScreenSpace = GUILayout.Toggle (xCBMManager.Config.UseWorkaroundUGuiCanvasScreenSpace, "Use workaround for Unity uGUI Canvas Screen Space bug");
			EditorGUILayout.Space ();

		}
		EditorGUILayout.EndVertical ();

		// Preview
		EditorGUILayout.BeginVertical ("box", GUILayout.Width(blockWidth));
		xCBMManager.Config.FoldoutOptionsPreview= Foldout (xCBMManager.Config.FoldoutOptionsPreview, "xCBM Preview Options", true, EditorStyles.foldout);
		if(xCBMManager.Config.FoldoutOptionsPreview){
			GUILayout.Label ("Update Limit", EditorStyles.boldLabel);
			GUILayout.Label ("Maximum updates per second (FPS)");
			xCBMManager.Config.PreviewUpdateIntervalLimitEdit = EditorGUILayout.IntField ("Edit mode", xCBMManager.Config.PreviewUpdateIntervalLimitEdit);
			xCBMManager.Config.PreviewUpdateIntervalLimitPlay = EditorGUILayout.IntField ("Play mode", xCBMManager.Config.PreviewUpdateIntervalLimitPlay);
			EditorGUILayout.Space ();
		}
		EditorGUILayout.EndVertical ();

		// Gallery
		EditorGUILayout.BeginVertical ("box", GUILayout.Width(blockWidth));
		xCBMManager.Config.FoldoutOptionsGallery = Foldout (xCBMManager.Config.FoldoutOptionsGallery, "xCBM Gallery Options", true, EditorStyles.foldout);
		if(xCBMManager.Config.FoldoutOptionsGallery){
			GUILayout.Label ("Update Limit", EditorStyles.boldLabel);
			GUILayout.Label ("Maximum updates per second (FPS)");
			xCBMManager.Config.GalleryUpdateIntervalLimitEdit = EditorGUILayout.IntField ("Edit mode", xCBMManager.Config.GalleryUpdateIntervalLimitEdit);
			xCBMManager.Config.GalleryUpdateIntervalLimitPlay = EditorGUILayout.IntField ("Play mode", xCBMManager.Config.GalleryUpdateIntervalLimitPlay);
			EditorGUILayout.Space ();
			GUILayout.Label ("ScreenCap Size", EditorStyles.boldLabel);
			xCBMManager.Config.UseFixedScreenCapSize = GUILayout.Toggle (xCBMManager.Config.UseFixedScreenCapSize, "Display all ScreenCaps at the same fixed size");
			if(xCBMManager.Config.UseFixedScreenCapSize){
				xCBMManager.Config.FixedScreenCapSize = EditorGUILayout.Vector2Field ("Size", xCBMManager.Config.FixedScreenCapSize);
			}
			EditorGUILayout.Space ();
		}
		EditorGUILayout.EndVertical ();
		
		EditorGUILayout.EndVertical ();
	}

	
	private static void DrawScreenCapGroups (){
		
		EditorGUILayout.BeginVertical ("box");
		
		GUILayout.Label ("Color Blindness Previews ('ScreenCaps')", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		GUILayout.Label ("Select the color blindness types you like to preview.");
		EditorGUILayout.Space ();

		DrawDefaultScreenCapGroup ();
		EditorGUILayout.Space ();

		EditorGUILayout.EndVertical ();
	}
	
	
	// Draws all ScreenCap entries of the specified group
	private static void DrawDefaultScreenCapGroup (){
		// Header
		GUILayout.BeginHorizontal ("box");
		
		DrawColumnHeader ("Active", labelWidthEnabled + spacing, xCBMScreenCap=>xCBMScreenCap.Enabled);
		DrawColumnHeader ("Medical Name", labelWidthName, xCBMScreenCap=>xCBMScreenCap.Name);
		DrawColumnHeader ("Colloquial Name", labelWidthName, xCBMScreenCap=>xCBMScreenCap.TypeColl);
		DrawColumnHeader ("Stats (% Male)", labelWidthStats, xCBMScreenCap=>xCBMScreenCap.FrequencyMalePercent);
		DrawColumnHeader ("Stats (% Female)", labelWidthStats, xCBMScreenCap=>xCBMScreenCap.FrequencyFemalePercent);
		DrawColumnHeader ("Description", labelMinWidthDescription, xCBMScreenCap=>xCBMScreenCap.Description, true);
		GUILayout.EndHorizontal ();
		
		// Entries
		foreach(xCBMScreenCap currScreenCap in xCBMManager.AvailScreenCaps){
			DrawDefaultScreenCapEntry (currScreenCap);
		}
		
		EditorGUILayout.Space ();
	}
	
	
	private static void DrawColumnHeader(string text, int width, Func<xCBMScreenCap, object> sortBy, bool isMinWidth = false){
		
		// fit correctly toolbar/header above entries
		int toolbarOffset = 4;
		char charSortIndicator = new char();
		
		if(lastSortedColumn == sortBy && lastSortWasAsc){
			charSortIndicator = charAsc;
		} else if(lastSortedColumn == sortBy && !lastSortWasAsc){
			charSortIndicator = charDesc;
		}
		
		if(isMinWidth){
			if(GUILayout.Button (text + " " + charSortIndicator, EditorStyles.toolbarButton, GUILayout.MinWidth (width + toolbarOffset)))
				SortAvailScreenCaps(sortBy);
		} else {
			if(GUILayout.Button (text + " " + charSortIndicator, EditorStyles.toolbarButton, GUILayout.Width (width + toolbarOffset)))
				SortAvailScreenCaps(sortBy);
		}
	}
	
	
	// Draws an default (=bultin) ScreenCap entry line
	private static void DrawDefaultScreenCapEntry (xCBMScreenCap screenCap){
		
		GUILayout.BeginHorizontal ("box");
		GUILayout.Space (spacing);

		screenCap.Enabled = GUILayout.Toggle (screenCap.Enabled, "", GUILayout.Width (labelWidthEnabled));
		GUILayout.Label (screenCap.Name, GUILayout.Width (labelWidthName));
		GUILayout.Label (screenCap.TypeColl, GUILayout.Width (labelWidthName));

		if(screenCap.FrequencyMalePercent == -1f){
			GUILayout.Label ("n/a", GUILayout.Width (labelWidthStats));
		} else {
			GUILayout.Label (screenCap.FrequencyMalePercent.ToString() + "%", GUILayout.Width (labelWidthStats));
		}
		if(screenCap.FrequencyFemalePercent == -1f){
			GUILayout.Label ("n/a", GUILayout.Width (labelWidthStats));
		} else {
			GUILayout.Label (screenCap.FrequencyFemalePercent.ToString() + "%", GUILayout.Width (labelWidthStats));
		}

		GUILayout.Label (screenCap.Description, GUILayout.MinWidth (labelMinWidthDescription));
		
		GUILayout.EndHorizontal ();
	}
	
	private static void DrawFooter(){
		
		GUILayout.Label ("Although all information were compiled with great care the accuracy of the information can not been guaranteed. " +
		"xCBM: Color Blindness Master can not simulate every aspect of color blindness and color vision deficiency," +
		" therefore previews may differ from what an individual person with color blindness or color vision deficiency sees.", EditorStyles.wordWrappedMiniLabel);
		GUILayout.Label ("xCBM v" + xCBMConfig.xCBMVersion, EditorStyles.wordWrappedMiniLabel);
	}
	
	// helper to make foldout-labels clickable (source: http://answers.unity3d.com/questions/684414/custom-editor-foldout-doesnt-unfold-when-clicking.html)
	public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style){
		Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
		// EditorGUI.kNumberW == 40f but is internal
		return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
	}
	public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style){
		return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
	}

	#endregion

	private static void SortAvailScreenCaps(Func<xCBMScreenCap, object> sortBy){
		
		// only sort desc if this column was sorted asc the last time/sort
		if(lastSortWasAsc && lastSortedColumn == sortBy){
			xCBMManager.AvailScreenCaps = xCBMManager.AvailScreenCaps.OrderByDescending (sortBy).ToList();
			lastSortWasAsc = false;
		} else {
			xCBMManager.AvailScreenCaps = xCBMManager.AvailScreenCaps.OrderBy (sortBy).ToList();
			lastSortWasAsc = true;
		}
		
		lastSortedColumn = sortBy;
		
	}
	
	#endregion
}
#endif