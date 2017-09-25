using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace SmartSparrow
{
	public interface ICapi
	{
		/// <summary>
		/// Invoked when a check event is submitted to the CAPI backend.
		/// </summary>
		UnityEvent onCheckStarted { get; }

		/// <summary>
		/// Invoked when the CAPI backend confirms that a check event has completed processing.
		/// </summary>
		UnityEvent onCheckCompleted { get; }

		void Initialize ();

		/// <summary>
		/// The CAPI backend only allows the processing of one concurrent check event.
		/// User input should be disallowed while a check event is processing in order
		/// to prevent the simulation frontend and CAPI backend states from going out of sync.
		/// </summary>
		bool IsCheckEventInProgress { get; }

		void Expose<T> (string propertyName, Func<T> getter, Action<T> setter, T[] allowedValues = null);

		T Get<T> (string propertyName);

		void Set<T> (string propertyName, T value);

		void TriggerCheckEvent ();

		void GetSimData (string simId, string key, Action<string> onSuccess, Action<string> onError);

		void SetSimData (string simId, string key, string value, Action<string> onSuccess, Action<string> onError);
	}
}
