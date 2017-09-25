using UnityEngine;
using System;

public class CapiMetadata
{
	public object value;
	public CapiType type;
	public Func<object> getter;
	public Func<object, object> setter; // cannot use Func<object, Void> due to bug in unity WebGL compiler
	public object[] allowedValues;

	public CapiMetadata (object value, CapiType type, Func<object> getter, Func<object, object> setter)
    {
		this.value = value;
		this.type = type;
		this.getter = getter;
		this.setter = setter;
        // This throws a warning, but our website architecture requires it as of 6/5/2016.
		this.allowedValues = allowedValues;
	}
}
