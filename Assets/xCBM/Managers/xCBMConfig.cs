#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using xCBM;


public class xCBMConfig {

	#region Fields

	[XmlIgnore]
	public static string xCBMVersion = "0.9";

	private static string settingsFilePath = Application.dataPath + "/../ProjectSettings/xCBMSettings.xml"; // Unity 3.3 Error

	[XmlIgnore]
	public bool ConfigChanged = false;

	public string Version = "";
	// limit save interval
	private double nextPossibleSaveTime = 0.0f;
	private float saveInterval = 0.5f;

	// foldouts
	private bool foldoutOptionsPreview = true;
	private bool foldoutOptionsGallery = true;
	private bool foldoutContact = true;
	private bool foldoutOptions = true;

	// xCBM Options
	private bool useFixedScreenCapSize = false;
	private Vector2 fixedScreenCapSize = new Vector2(160, 160);
	private int framesToWait = 1;
	private bool useWorkaroundUGuiCanvasScreenSpace = true;

	// xCBM Preview window
	private int previewSelectedScreenCapIndex = 0;
	private bool previewAutoUpdateInEditorMode = false;
	private bool previewAutoUpdateInPauseMode = false;
	private bool previewAutoUpdateInPlayMode = false;
	private bool previewOneToOnePixel = false;
	private int previewUpdateIntervalLimitEdit = 12;
	private int previewUpdateIntervalLimitPlay = 30;
	
	// xCBM Gallery window
	private bool galleryAutoUpdateInEditorMode = false;
	private bool galleryAutoUpdateInPauseMode = false;
	private bool galleryAutoUpdateInPlayMode = false;
	private int galleryScreenCapsPerRow = 1;
	private int galleryUpdateIntervalLimitEdit = 12;
	private int galleryUpdateIntervalLimitPlay = 30;

	// list of all ScreenCaps
	public List<xCBMScreenCap> AvailScreenCaps = new List<xCBMScreenCap>(); // no property; sets ConfigChanged
	#endregion
	
	#region Properties
	// every config change marks config as changed (to enable save on change)
	
	#region Foldouts
	public bool FoldoutContact {
		get {
			return foldoutContact;
		}
		set {
			MarkConfigAsChanged (foldoutContact, value);

			foldoutContact = value;
		}
	}

	public bool FoldoutOptions {
		get {
			return foldoutOptions;
		}
		set {
			MarkConfigAsChanged (foldoutOptions, value);

			foldoutOptions = value;
		}
	}

	public bool FoldoutOptionsPreview {
		get {
			return foldoutOptionsPreview;
		}
		set {
			MarkConfigAsChanged (foldoutOptionsPreview, value);

			foldoutOptionsPreview = value;
		}
	}
	
	public bool FoldoutOptionsGallery {
		get {
			return foldoutOptionsGallery;
		}
		set {
			MarkConfigAsChanged (foldoutOptionsGallery, value);

			foldoutOptionsGallery = value;
		}
	}
	#endregion

	#region Options (global, Preview and Gallery)
	public bool UseFixedScreenCapSize {
		get {
			return useFixedScreenCapSize;
		}
		set {
			MarkConfigAsChanged (useFixedScreenCapSize, value);

			useFixedScreenCapSize = value;
		}
	}

	public Vector2 FixedScreenCapSize{
		get{return fixedScreenCapSize;}
		set {
			MarkConfigAsChanged (fixedScreenCapSize, value);

			if(value.x < 100) value.x = 100;
			if(value.y < 100) value.y = 100;
			
			fixedScreenCapSize = value;
		}
	}

	public int FramesToWait {
		get {
			return framesToWait;
		}
		set {
			MarkConfigAsChanged (framesToWait, value);

			if(value >= 0){
				framesToWait = value;
			} else {
				framesToWait = 1;
			}
		}
	}

	public bool UseWorkaroundUGuiCanvasScreenSpace {
		get {
			return useWorkaroundUGuiCanvasScreenSpace;
		}
		set {
			MarkConfigAsChanged (useWorkaroundUGuiCanvasScreenSpace, value);

			useWorkaroundUGuiCanvasScreenSpace = value;
		}
	}

	#endregion

	#region Preview window
	public int PreviewSelectedScreenCapIndex {
		get {
			return previewSelectedScreenCapIndex;
		}
		set {
			MarkConfigAsChanged (previewSelectedScreenCapIndex, value);

			previewSelectedScreenCapIndex = value;
		}
	}

	public bool PreviewAutoUpdateInEditorMode {
		get {
			return previewAutoUpdateInEditorMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInEditorMode, value);

			previewAutoUpdateInEditorMode = value;
		}
	}

	public bool PreviewAutoUpdateInPauseMode {
		get {
			return previewAutoUpdateInPauseMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInPauseMode, value);

			previewAutoUpdateInPauseMode = value;
		}
	}

	public bool PreviewAutoUpdateInPlayMode {
		get {
			return previewAutoUpdateInPlayMode;
		}
		set {
			MarkConfigAsChanged (previewAutoUpdateInPlayMode, value);

			previewAutoUpdateInPlayMode = value;
		}
	}

	public bool PreviewOneToOnePixel{
		get {return previewOneToOnePixel;}
		set {
			MarkConfigAsChanged (previewOneToOnePixel, value);

			previewOneToOnePixel = value;
		}
	}

	public int PreviewUpdateIntervalLimitEdit {
		get {
			return previewUpdateIntervalLimitEdit;
		}
		set {
			MarkConfigAsChanged (previewUpdateIntervalLimitEdit, value);
			
			previewUpdateIntervalLimitEdit = value;
		}
	}
	
	public int PreviewUpdateIntervalLimitPlay {
		get {
			return previewUpdateIntervalLimitPlay;
		}
		set {
			MarkConfigAsChanged (previewUpdateIntervalLimitPlay, value);
			
			previewUpdateIntervalLimitPlay = value;
		}
	}
	#endregion
	
	#region Gallery window
	public bool GalleryAutoUpdateInEditorMode {
		get {
			return galleryAutoUpdateInEditorMode;
		}
		set {
			MarkConfigAsChanged (galleryAutoUpdateInEditorMode, value);

			galleryAutoUpdateInEditorMode = value;
		}
	}

	public bool GalleryAutoUpdateInPauseMode {
		get {
			return galleryAutoUpdateInPauseMode;
		}
		set {
			MarkConfigAsChanged (galleryAutoUpdateInPauseMode, value);

			galleryAutoUpdateInPauseMode = value;
		}
	}

	public bool GalleryAutoUpdateInPlayMode {
		get {
			return galleryAutoUpdateInPlayMode;
		}
		set {
			MarkConfigAsChanged (galleryAutoUpdateInPlayMode, value);

			galleryAutoUpdateInPlayMode = value;
		}
	}
	
	public int GalleryScreenCapsPerRow {
		get {
			return galleryScreenCapsPerRow;
		}
		set {
			MarkConfigAsChanged (galleryScreenCapsPerRow, value);

			galleryScreenCapsPerRow = value;
		}
	}

	public int GalleryUpdateIntervalLimitEdit {
		get {
			return galleryUpdateIntervalLimitEdit;
		}
		set {
			MarkConfigAsChanged (galleryUpdateIntervalLimitEdit, value);

			galleryUpdateIntervalLimitEdit = value;
		}
	}

	public int GalleryUpdateIntervalLimitPlay {
		get {
			return galleryUpdateIntervalLimitPlay;
		}
		set {
			MarkConfigAsChanged (galleryUpdateIntervalLimitPlay, value);

			galleryUpdateIntervalLimitPlay = value;
		}
	}
	#endregion

	#endregion
	
	#region Functions
	#region Save
	public xCBMConfig() {
		// add save delegate
		EditorApplication.update += SaveOnChange;
	}

	~xCBMConfig(){
		// remove save delegate
		EditorApplication.update -= SaveOnChange;
	}

	// save if config has changed
	public void SaveOnChange(){
		// don't save every frame
		if(EditorApplication.timeSinceStartup > nextPossibleSaveTime){
			nextPossibleSaveTime = EditorApplication.timeSinceStartup + saveInterval;

			// save on config change
			if(ConfigChanged || xCBMScreenCap.ListChanged){
				save ();
				
				ConfigChanged = false;
				xCBMScreenCap.ListChanged = false;
			}
		}
	}

	// was the value realy changed? (needed because GUI sets value while only displaying it)
	private void MarkConfigAsChanged<T>(T oldValue, T newValue){
		// was the value really changed? (needed because GUI sets value while only displaying it)
		if(!EqualityComparer<T>.Default.Equals (oldValue, newValue)){
			ConfigChanged = true;
		}
	}
	#endregion

	#region Init, Save and Load
	public static xCBMConfig InitOrLoad() {
		// load values form XML if possible
		if(File.Exists (settingsFilePath)){
			return load ();
			
		} else { // default values
			return new xCBMConfig();
		}
	}

	// store config to XML
	private void save() {
		// ensure version is set
		if(Version == "") Version = xCBMVersion;

		XmlSerializer serializer = new XmlSerializer(typeof(xCBMConfig));
		using(StreamWriter stream = new StreamWriter(settingsFilePath, false, Encoding.UTF8)){
			serializer.Serialize (stream, this);
		}
	}

	// load config from XML
	private static xCBMConfig load() {
		XmlSerializer serializer = new XmlSerializer(typeof(xCBMConfig));
		object config;
		xCBMConfig loadedxCBMConfig;

		using(FileStream stream = new FileStream(settingsFilePath, FileMode.Open)){
			config = serializer.Deserialize (stream); // create xCBMConfig instance
		}

		// convert loaded XML
		loadedxCBMConfig = (xCBMConfig)config;

		// mark loaded config as unchanged to prevent resave
		loadedxCBMConfig.ConfigChanged = false;

		// detect new version
		if(loadedxCBMConfig.Version != xCBMVersion){

			// reopen Contact foldout
			loadedxCBMConfig.FoldoutContact = true;

			// set new version (and mark as chaged)
			loadedxCBMConfig.Version = xCBMVersion;
		}

		return loadedxCBMConfig;
	}
	#endregion
	#endregion
}
#endif