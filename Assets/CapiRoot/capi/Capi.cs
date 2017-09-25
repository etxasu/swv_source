using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using SimpleJSON;

public static class Capi {

	private static Dictionary<string, CapiMetadata> capiMetadata = new Dictionary<string, CapiMetadata>();

	private static bool runCapiScript = false;

    public static void initializeExpose ()
    {
		if (!runCapiScript)
        {
            //Application.ExternalEval(CapiSetupCode.SetupCode);
            //Application.ExternalEval();
			runCapiScript = true;
		}
	}

	public static CapiType typeOf (string propertyName) {
		CapiMetadata md;
		capiMetadata.TryGetValue (propertyName, out md);
		return md.type;
	}

	// stringify data
	private static object prepareValue (object value) {
		if (value.GetType ().IsArray) {
			var arr = (object[])value;
			string[] res = new string[arr.Length];
			for (int i = 0; i < arr.Length; i++ ) {
				res[i] = arr[i].ToString(); // TODO this is dirty
			}
			return res;
		} else {
			return value.ToString();
		}
	}

	public static void expose<T> (string propertyName, Func<object> getter, Func<T, object> setter, object[] allowedValues = null) {
		// TODO if already exposed throw error
		
		object value = prepareValue(getter ());

		if (allowedValues != null) {
			allowedValues = (object[])prepareValue(allowedValues);
		}

		CapiType type = CapiType.STRING;
		if (typeof(T) == typeof(float)) {
			type = CapiType.NUMBER;
		} else if (typeof(T) == typeof(String)) {
			type = CapiType.STRING;
		} else if (typeof(T) == typeof(Boolean)) {
			type = CapiType.BOOLEAN;
		} else if (typeof(T) == typeof(string[])) {
			type = CapiType.ARRAY;
		} else {
			// TODO throw type not supported exception
		}
		
		var res = GameObject.Find ("CAPI");
		if (res == null) {
			var capiGO = new GameObject ("CAPI");
			capiGO.AddComponent<CapiBehaviour> ();
		}
		
		Capi.initializeExpose ();

		capiMetadata.Add (propertyName, new CapiMetadata (value, type, getter, (v) => { return setter ((T) v); }));

		Application.ExternalCall ("receiveExposeFromUnity", new object[]{propertyName, (int)type, value, allowedValues});
	}
	
	public static void setInternal (string propertyName, object value) {
		capiMetadata[propertyName].setter(value);
	}

	public static object set (string propertyName, object value) {
		if (!runCapiScript) { return value; }
		try {
			var md = capiMetadata[propertyName];
			value = prepareValue(value);
			md.value = value;
			Application.ExternalCall ("receiveValueFromUnity", new object[]{propertyName, (int)md.type, value});
		} catch (KeyNotFoundException e)
        {
            Debug.Log(e.Message + " || =>" + propertyName);
			// Throw error into Unity debugger if something explodes
            // If in a deployed environment, will do nothing instead
		}
		return value;
	}
}
 