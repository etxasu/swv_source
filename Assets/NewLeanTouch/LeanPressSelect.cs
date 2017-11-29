using UnityEngine;
using System.Collections.Generic;

namespace NewLean.Touch
{
	// This script allows you to select multiple NewLeanSelectable components while a finger is down
	public class LeanPressSelect : MonoBehaviour
	{
		public enum SelectType
		{
			Raycast3D,
			Overlap2D
		}

		public enum SearchType
		{
			GetComponent,
			GetComponentInParent,
			GetComponentInChildren
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Should the selected object automatically deselect if the selecting finger moves off it?")]
		public bool DeselectOnExit;

		public SelectType SelectUsing;

		[Tooltip("This stores the layers we want the raycast/overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("How should the selected GameObject be searched for the NewLeanSelectable component?")]
		public SearchType Search;

		[Tooltip("The currently selected NewLeanSelectables")]
		public List<NewLeanSelectable> CurrentSelectables;

		protected virtual void OnEnable()
		{
			// Hook events
			NewLeanTouch.OnFingerDown += FingerDown;
			NewLeanTouch.OnFingerSet  += FingerSet;
			NewLeanTouch.OnFingerUp   += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			NewLeanTouch.OnFingerDown -= FingerDown;
			NewLeanTouch.OnFingerSet  -= FingerSet;
			NewLeanTouch.OnFingerUp   -= FingerUp;
		}
        public GameObject TouchedElement;

		private void FingerDown(NewLeanFinger finger)
		{
			// Ignore this finger?
			if (IgnoreGuiFingers == true && finger.StartedOverGui == true)
			{
				return;
			}

			// Find the component under the finger
			var component = FindComponentUnder(finger);

			// Find the selectable associated with this component
			var selectable = FindSelectableFrom(component);

            if (component != null)
            {
                TouchedElement = component.gameObject;
            }
            // Select the found selectable with the selecting finger
            Select(finger, selectable);
		}

		private void FingerSet(NewLeanFinger finger)
		{
			if (DeselectOnExit == true)
			{
				// Run through all selected objects
				for (var i = CurrentSelectables.Count - 1; i >= 0; i--)
				{
					var currentSelectable = CurrentSelectables[i];

					// Is it valid?
					if (currentSelectable != null)
					{
						// Is this object associated with this finger?
						if (currentSelectable.SelectingFinger == finger)
						{
							// Find the component under the finger
							var component = FindComponentUnder(finger);

							// Find the selectable associated with this component
							var selectable = FindSelectableFrom(component);

							// If the associated object is no longer under the finger, deselect it
							if (selectable != currentSelectable)
							{
								Deselect(currentSelectable);
							}
						}
						// Deselect in case the association was lost
						else if (currentSelectable.SelectingFinger == null)
						{
							Deselect(currentSelectable);
						}
					}
					// Remove invalid
					else
					{
						CurrentSelectables.RemoveAt(i);
					}
				}
			}
		}

		private void FingerUp(NewLeanFinger finger)
		{
			for (var i = CurrentSelectables.Count - 1; i >= 0; i--)
			{
				var currentSelectable = CurrentSelectables[i];

				if (currentSelectable != null)
				{
					if (currentSelectable.SelectingFinger == finger || currentSelectable.SelectingFinger == null)
					{
						Deselect(currentSelectable);
					}
				}
				else
				{
					CurrentSelectables.RemoveAt(i);
				}
			}
		}

		public void Select(NewLeanFinger finger, NewLeanSelectable selectable)
		{
			// Something was selected?
			if (selectable != null && selectable.isActiveAndEnabled == true)
			{
				if (CurrentSelectables == null)
				{
					CurrentSelectables = new List<NewLeanSelectable>();
				}

				// Loop through all current selectables
				for (var i = CurrentSelectables.Count - 1; i >= 0; i--)
				{
					var currentSelectable = CurrentSelectables[i];

					if (currentSelectable != null)
					{
						// Already selected?
						if (currentSelectable == selectable)
						{
							return;
						}
					}
					else
					{
						CurrentSelectables.RemoveAt(i);
					}
				}

				// Not selected yet, so select it
				CurrentSelectables.Add(selectable);

				selectable.Select(finger);
			}
		}

		[ContextMenu("Deselect All")]
		public void DeselectAll()
		{
			// Loop through all current selectables and deselect if not null
			if (CurrentSelectables != null)
			{
				for (var i = CurrentSelectables.Count - 1; i >= 0; i--)
				{
					var currentSelectable = CurrentSelectables[i];

					if (currentSelectable != null)
					{
						currentSelectable.Deselect();
					}
				}

				// Clear
				CurrentSelectables.Clear();
			}
		}

		// Deselect the specified selectable, if it exists
		public void Deselect(NewLeanSelectable selectable)
		{
			// Loop through all current selectables
			if (CurrentSelectables != null)
			{
				for (var i = CurrentSelectables.Count - 1; i >= 0; i--)
				{
					var currentSelectable = CurrentSelectables[i];

					if (currentSelectable != null)
					{
						// Match?
						if (currentSelectable == selectable)
						{
							selectable.Deselect();

							CurrentSelectables.Remove(selectable);

							return;
						}
					}
					else
					{
						CurrentSelectables.RemoveAt(i);
					}
				}
			}
		}

		private Component FindComponentUnder(NewLeanFinger finger)
		{
			var component = default(Component);

			switch (SelectUsing)
			{
				case SelectType.Raycast3D:
				{
					// Get ray for finger
					var ray = finger.GetRay();

					// Stores the raycast hit info
					var hit = default(RaycastHit);

					// Was this finger pressed down on a collider?
					if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
					{
						component = hit.collider;
					}
				}
				break;

				case SelectType.Overlap2D:
				{
					// Find the position under the current finger
					var point = finger.GetWorldPosition(1.0f);

					// Find the collider at this position
					component = Physics2D.OverlapPoint(point, LayerMask);
				}
				break;
			}

			return component;
		}

		private NewLeanSelectable FindSelectableFrom(Component component)
		{
			var selectable = default(NewLeanSelectable);

			if (component != null)
			{
				switch (Search)
				{
					case SearchType.GetComponent:           selectable = component.GetComponent          <NewLeanSelectable>(); break;
					case SearchType.GetComponentInParent:   selectable = component.GetComponentInParent  <NewLeanSelectable>(); break;
					case SearchType.GetComponentInChildren: selectable = component.GetComponentInChildren<NewLeanSelectable>(); break;
				}
			}

			return selectable;
		}
	}
}