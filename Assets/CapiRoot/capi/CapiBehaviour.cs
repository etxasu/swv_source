using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

public class CapiBehaviour : MonoBehaviour {

	void setValueFromJS (string json)
    {
		var result = JSON.Parse(json);

		var propertyName = result ["name"];
		var value = result ["value"];

		switch (Capi.typeOf (propertyName))
        {
			case CapiType.STRING:
				Capi.setInternal(propertyName, (string)value);
				return;
			case CapiType.BOOLEAN:
				Capi.setInternal(propertyName, value.AsBool);
				return;
			case CapiType.NUMBER:
				Capi.setInternal(propertyName, value.AsFloat);
				return;
			case CapiType.ARRAY:
				Capi.setInternal(propertyName, parseArray(value));
				return;
		}
	}

	string[] parseArray (JSONNode value) {
		var arr = value.AsArray;
		var res = new string[arr.Count];
		for (int i = 0 ; i < arr.Count; i ++) {
			res[i] = (string)arr[i];
		}
		return res;
	}

    public void TestBrowserSend(string derp)
    {
        bool herp = Convert.ToBoolean(derp);

        if (herp)
        {
            //Debug.Log("MESSAGE RECEIVED");
        }
        else
        {
            //Debug.Log("MESSAGE ALSO RECEIVED");
        }
        
    }
}
