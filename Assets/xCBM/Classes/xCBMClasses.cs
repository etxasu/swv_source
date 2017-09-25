#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace xCBM{
	
	public enum xCBMColorBlindnessType // strings are used by shader! 
		{Protanopia=0, Protanomaly=1, Deuteranopia=2, Deuteranomaly=3, Tritanopia=4, Tritanomaly=5, Achromatopsia=6, Achromatomaly=7, Normal=100};

	public enum EditorMode {Other, Edit, Pause, Play};
	
	public class xCBMScreenCap {

		#region Fields
		public static bool ListChanged = false;

		public xCBMColorBlindnessType Type;
		public string Name;
		public string TypeColl;
		public string LongName;
		public string Description;
		public float FrequencyMalePercent;
		public float FrequencyFemalePercent;
		private Texture2D _texture;
		private bool enabled;
		[XmlIgnore]
		public double LastUpdateTime = 0;
		[XmlIgnore]
		public double LastUpdateTryTime = 0;
		[XmlIgnore]
		public bool UpdatedSuccessful = true;

		#endregion
		
		#region Properties
		[XmlIgnore]
		public Texture2D Texture{
			get {
				if(_texture == null) _texture = EditorGUIUtility.whiteTexture;
				return _texture;
				}
			set {
				_texture = value;
			}
		}

		public bool Enabled {
			get {
				return enabled;
			}
			set {
				// mark SC list as changed to enable config save on change
				if (enabled != value){
					ListChanged = true;
				}

				enabled = value;
			}
		}		
		#endregion
		
		#region Init
		public xCBMScreenCap (){}
		
		public xCBMScreenCap (xCBMColorBlindnessType type, string typeColl, float frequencyMalePerc, float frequencyFemalePerc, string desc){

			Type = type; 
			Name = type.ToString ();
			TypeColl = typeColl;
			Description = desc;
			FrequencyMalePercent = frequencyMalePerc;
			FrequencyFemalePercent = frequencyFemalePerc;
			enabled = false;
			LongName = Name + " (" + TypeColl + ")";
		}
		#endregion
		
		#region Functions

		#endregion 
		
		#region Operators
		public static bool operator == (xCBMScreenCap a, xCBMScreenCap b){
 			if(a.Name == b.Name){
				return true;
			} else {
				return false;
			}
		}
		
		public static bool operator != (xCBMScreenCap a, xCBMScreenCap b){
			return !(a == b);	
		}
		
		public override bool Equals (object otherObject){
			if(!(otherObject is xCBMScreenCap)) return false;
			return this == (xCBMScreenCap)otherObject;	
		}
		
		public override int GetHashCode(){
			unchecked{
				int hash = 17;
				hash = hash * 23 + Name.GetHashCode();
				return hash;
			}
		}
		#endregion
	}

	// holds refs to all xCBM Shader variants
	public class xCBMMaterial{
		private Material[] mat = new Material[8];

		public Material this[int i]{
			get{
				if(mat[i] == null){
					mat[i] = new Material(Shader.Find("Hidden/xCBMShader"));
					xCBMColorBlindnessType type = (xCBMColorBlindnessType)(i);
					mat[i].EnableKeyword(type.ToString().ToUpper());
				}
				return mat[i];
			}
		}
	}
}
#endif