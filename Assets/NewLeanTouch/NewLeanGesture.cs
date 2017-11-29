using UnityEngine;
using System.Collections.Generic;

namespace NewLean.Touch
{
	// This class calculates gesture information based on a list of input fingers
	public static class NewLeanGesture
	{
		// Gets the average ScreenPosition of the fingers
		public static Vector2 GetScreenCenter()
		{
			return GetScreenCenter(NewLeanTouch.Fingers);
		}

		public static Vector2 GetScreenCenter(List<NewLeanFinger> fingers)
		{
			var center = default(Vector2); TryGetScreenCenter(fingers, ref center); return center;
		}
		
		public static bool TryGetScreenCenter(List<NewLeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				var total = Vector2.zero;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.ScreenPosition;
						count += 1;
					}
				}
				
				if (count > 0)
				{
					center = total / count; return true;
				}
			}

			return false;
		}
		
		// Gets the last average ScreenPosition of the fingers
		public static Vector2 GetLastScreenCenter()
		{
			return GetLastScreenCenter(NewLeanTouch.Fingers);
		}

		public static Vector2 GetLastScreenCenter(List<NewLeanFinger> fingers)
		{
			var center = default(Vector2); TryGetLastScreenCenter(fingers, ref center); return center;
		}
		
		public static bool TryGetLastScreenCenter(List<NewLeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				var total = Vector2.zero;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.LastScreenPosition;
						count += 1;
					}
				}
				
				if (count > 0)
				{
					center = total / count; return true;
				}
			}

			return false;
		}
		
		// Gets the average ScreenDelta of the fingers
		public static Vector2 GetScreenDelta()
		{
			return GetScreenDelta(NewLeanTouch.Fingers);
		}

		public static Vector2 GetScreenDelta(List<NewLeanFinger> fingers)
		{
			var delta = default(Vector2); TryGetScreenDelta(fingers, ref delta); return delta;
		}

		public static bool TryGetScreenDelta(List<NewLeanFinger> fingers, ref Vector2 delta)
		{
			if (fingers != null)
			{
				var total = Vector2.zero;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.ScreenDelta;
						count += 1;
					}
				}
				
				if (count > 0)
				{
					delta = total / count; return true;
				}
			}

			return false;
		}

		// Gets the average ScreenDelta * NewLeanTouch.ScalingFactor of the fingers
		public static Vector2 GetScaledDelta()
		{
			return GetScreenDelta() * NewLeanTouch.ScalingFactor;
		}

		public static Vector2 GetScaledDelta(List<NewLeanFinger> fingers)
		{
			return GetScreenDelta(fingers) * NewLeanTouch.ScalingFactor;
		}

		public static bool TryGetScaledDelta(List<NewLeanFinger> fingers, ref Vector2 delta)
		{
			if (TryGetScreenDelta(fingers, ref delta) == true)
			{
				delta *= NewLeanTouch.ScalingFactor; return true;
			}

			return false;
		}

		// Gets the average WorldDelta of the fingers
		public static Vector3 GetWorldDelta(float distance, Camera camera = null)
		{
			return GetWorldDelta(NewLeanTouch.Fingers, distance, camera);
		}

		public static Vector3 GetWorldDelta(List<NewLeanFinger> fingers, float distance, Camera camera = null)
		{
			var delta = default(Vector3); TryGetWorldDelta(fingers, distance, ref delta, camera); return delta;
		}

		public static bool TryGetWorldDelta(List<NewLeanFinger> fingers, float distance, ref Vector3 delta, Camera camera = null)
		{
			if (NewLeanTouch.GetCamera(ref camera) == true)
			{
				if (fingers != null)
				{
					var total = Vector3.zero;
					var count = 0;

					for (var i = fingers.Count - 1; i >= 0; i--)
					{
						var finger = fingers[i];

						if (finger != null)
						{
							total += finger.GetWorldDelta(distance, camera);
							count += 1;
						}
					}
					
					if (count > 0)
					{
						delta = total / count; return true;
					}
				}
			}

			return false;
		}
		
		// Gets the average ScreenPosition distance between the fingers
		public static float GetScreenDistance()
		{
			return GetScreenDistance(NewLeanTouch.Fingers);
		}

		public static float GetScreenDistance(List<NewLeanFinger> fingers)
		{
			var distance = default(float);
			var center   = default(Vector2);

			if (TryGetScreenCenter(fingers, ref center) == true)
			{
				TryGetScreenDistance(fingers, center, ref distance);
			}

			return distance;
		}
		
		public static float GetScreenDistance(List<NewLeanFinger> fingers, Vector2 center)
		{
			var distance = default(float); TryGetScreenDistance(fingers, center, ref distance); return distance;
		}
		
		public static bool TryGetScreenDistance(List<NewLeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (fingers != null)
			{
				var total = 0.0f;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.GetScreenDistance(center);
						count += 1;
					}
				}
				
				if (count > 0)
				{
					distance = total / count; return true;
				}
			}

			return false;
		}
		
		// Gets the average ScreenPosition distance * NewLeanTouch.ScalingFactor between the fingers
		public static float GetScaledDistance()
		{
			return GetScreenDistance() * NewLeanTouch.ScalingFactor;
		}

		public static float GetScaledDistance(List<NewLeanFinger> fingers)
		{
			return GetScreenDistance(fingers) * NewLeanTouch.ScalingFactor;
		}

		public static float GetScaledDistance(List<NewLeanFinger> fingers, Vector2 center)
		{
			return GetScreenDistance(fingers, center) * NewLeanTouch.ScalingFactor;
		}

		public static bool TryGetScaledDistance(List<NewLeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (TryGetScreenDistance(fingers, center, ref distance) == true)
			{
				distance *= NewLeanTouch.ScalingFactor; return true;
			}

			return false;
		}

		// Gets the last average ScreenPosition distance between all fingers
		public static float GetLastScreenDistance()
		{
			return GetLastScreenDistance(NewLeanTouch.Fingers);
		}

		public static float GetLastScreenDistance(List<NewLeanFinger> fingers)
		{
			var distance = default(float);
			var center   = default(Vector2);

			if (TryGetLastScreenCenter(fingers, ref center) == true)
			{
				TryGetLastScreenDistance(fingers, center, ref distance);
			}

			return distance;
		}
		
		public static float GetLastScreenDistance(List<NewLeanFinger> fingers, Vector2 center)
		{
			var distance = default(float); TryGetLastScreenDistance(fingers, center, ref distance); return distance;
		}
		
		public static bool TryGetLastScreenDistance(List<NewLeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (fingers != null)
			{
				var total = 0.0f;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.GetLastScreenDistance(center);
						count += 1;
					}
				}
				
				if (count > 0)
				{
					distance = total / count; return true;
				}
			}

			return false;
		}

		// // Gets the last average ScreenPosition distance * NewLeanTouch.ScalingFactor between all fingers
		public static float GetLastScaledDistance()
		{
			return GetLastScreenDistance() * NewLeanTouch.ScalingFactor;
		}

		public static float GetLastScaledDistance(List<NewLeanFinger> fingers)
		{
			return GetLastScreenDistance(fingers) * NewLeanTouch.ScalingFactor;
		}

		public static float GetLastScaledDistance(List<NewLeanFinger> fingers, Vector2 center)
		{
			return GetLastScreenDistance(fingers, center) * NewLeanTouch.ScalingFactor;
		}

		public static bool TryGetLastScaledDistance(List<NewLeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (TryGetLastScreenDistance(fingers, center, ref distance) == true)
			{
				distance *= NewLeanTouch.ScalingFactor; return true;
			}

			return false;
		}
		
		// Gets the pinch scale of the fingers
		public static float GetPinchScale(float wheelSensitivity = 0.0f)
		{
			return GetPinchScale(NewLeanTouch.Fingers, wheelSensitivity);
		}

		public static float GetPinchScale(List<NewLeanFinger> fingers, float wheelSensitivity = 0.0f)
		{
			var scale      = 1.0f;
			var center     = GetScreenCenter(fingers);
			var lastCenter = GetLastScreenCenter(fingers);

			TryGetPinchScale(fingers, center, lastCenter, ref scale, wheelSensitivity);

			return scale;
		}

		public static bool TryGetPinchScale(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float scale, float wheelSensitivity = 0.0f)
		{
			var distance     = GetScreenDistance(fingers, center);
			var lastDistance = GetLastScreenDistance(fingers, lastCenter);

			if (lastDistance > 0.0f)
			{
				scale = distance / lastDistance; return true;
			}

			if (wheelSensitivity != 0.0f)
			{
				var scroll = Input.mouseScrollDelta.y;

				if (scroll > 0.0f)
				{
					scale = 1.0f - wheelSensitivity; return true;
				}
				
				if (scroll < 0.0f)
				{
					scale = 1.0f + wheelSensitivity; return true;
				}
			}

			return false;
		}

		// Gets the pinch ratio of the fingers (reciprocal of pinch scale)
		public static float GetPinchRatio(float wheelSensitivity = 0.0f)
		{
			return GetPinchRatio(NewLeanTouch.Fingers, wheelSensitivity);
		}

		public static float GetPinchRatio(List<NewLeanFinger> fingers, float wheelSensitivity = 0.0f)
		{
			var ratio      = 1.0f;
			var center     = GetScreenCenter(fingers);
			var lastCenter = GetLastScreenCenter(fingers);

			TryGetPinchRatio(fingers, center, lastCenter, ref ratio, wheelSensitivity);

			return ratio;
		}

		public static bool TryGetPinchRatio(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float ratio, float wheelSensitivity = 0.0f)
		{
			var distance     = GetScreenDistance(fingers, center);
			var lastDistance = GetLastScreenDistance(fingers, lastCenter);

			if (distance > 0.0f)
			{
				ratio = lastDistance / distance;

				return true;
			}

			if (wheelSensitivity != 0.0f)
			{
				var scroll = Input.mouseScrollDelta.y;

				if (scroll > 0.0f)
				{
					ratio = 1.0f + wheelSensitivity; return true;
				}
				
				if (scroll < 0.0f)
				{
					ratio = 1.0f - wheelSensitivity; return true;
				}
			}

			return false;
		}

		// Gets the average twist of the fingers in degrees
		public static float GetTwistDegrees()
		{
			return GetTwistDegrees(NewLeanTouch.Fingers);
		}

		public static float GetTwistDegrees(List<NewLeanFinger> fingers)
		{
			return GetTwistRadians(fingers) * Mathf.Rad2Deg;
		}

		public static float GetTwistDegrees(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter)
		{
			return GetTwistRadians(fingers, center, lastCenter) * Mathf.Rad2Deg;
		}

		public static bool TryGetTwistDegrees(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float degrees)
		{
			if (TryGetTwistRadians(fingers, center, lastCenter, ref degrees) == true)
			{
				degrees *= Mathf.Rad2Deg;

				return true;
			}

			return false;
		}

		// Gets the average twist of the fingers in radians
		public static float GetTwistRadians()
		{
			return GetTwistRadians(NewLeanTouch.Fingers);
		}

		public static float GetTwistRadians(List<NewLeanFinger> fingers)
		{
			var center     = NewLeanGesture.GetScreenCenter(fingers);
			var lastCenter = NewLeanGesture.GetLastScreenCenter(fingers);
			
			return GetTwistRadians(fingers, center, lastCenter);
		}

		public static float GetTwistRadians(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter)
		{
			var radians = default(float); TryGetTwistRadians(fingers, center, lastCenter, ref radians); return radians;
		}

		public static bool TryGetTwistRadians(List<NewLeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float radians)
		{
			if (fingers != null)
			{
				var total = 0.0f;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];

					if (finger != null)
					{
						total += finger.GetDeltaRadians(center, lastCenter);
						count += 1;
					}
				}
				
				if (count > 0)
				{
					radians = total / count; return true;
				}
			}

			return false;
		}
	}
}