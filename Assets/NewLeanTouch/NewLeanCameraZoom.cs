using UnityEngine;
namespace NewLean.Touch
{
	// This script allows you to zoom a camera in and out based on the pinch gesture
	// This supports both perspective and orthographic cameras
	[ExecuteInEditMode]
	public class NewLeanCameraZoom : MonoBehaviour
	{
        public GameObject AsessmentCamera;

        public GameObject MiniMapCamera;

        public GameObject MyCloseUpCamera;

        [Tooltip("The camera that will be zoomed")]
		public Camera Camera;

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = false;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
		[Range(-1.0f, 1.0f)]
		public float WheelSensitivity;

		[Tooltip("The current FOV/Size")]
		public float Zoom = 50.0f;

		[Tooltip("The minimum FOV/Size we want to zoom to")]
		public float ZoomMin = 20.0f;

		[Tooltip("The maximum FOV/Size we want to zoom to")]
		public float ZoomMax = 960.0f;

        [Tooltip("The sensativity for mini map zoom")][Range(-1.0f, 1.0f)]
        public float MiniMapSensitivity = 1.0f;

        [Tooltip("The selected zoom feature")]
        public bool minimapZoom = false;

        [Tooltip("The previous zoom used by touch")]
        private float prevZoom;

        protected virtual void LateUpdate()
		{
			// Make sure the camera exists
			if (NewLeanTouch.GetCamera(ref Camera, gameObject) == true)
			{
				// Get the fingers we want to use
				var fingers = NewLeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
                
                // Reset pinch ratio before checking new pinch ratio
                var pinchRatio = prevZoom;

                // Check position of figures
                for (var i = 0; i < fingers.Count; i++)
                {
                    if (!minimapZoom && !MyCloseUpCamera.GetComponent<Camera>().enabled)
                    {
                        var checkFinger = fingers[i];
                        if (checkFinger.ScreenPosition.x > 740 && checkFinger.ScreenPosition.y < 225)
                            minimapZoom = true;
                    }
                }

                // Get script from assesment camera (which is default)
                Zoomer ZoomerScript = (Zoomer)AsessmentCamera.GetComponent(typeof(Zoomer));
                CloseUpCamera CloseUpCameraScript = (CloseUpCamera)MyCloseUpCamera.GetComponent(typeof(CloseUpCamera));

                if (minimapZoom)
                {
                    // Get the pinch ratio of these fingers with minimap sensativty
                    pinchRatio = NewLeanGesture.GetPinchRatio(fingers, MiniMapSensitivity);
                    // Get script from mini map camera
                    ZoomerScript = (Zoomer)MiniMapCamera.GetComponent(typeof(Zoomer));
                    // Mini map min and max zoom
                    ZoomMin = 20.0f;
                    ZoomMax = 960.0f;
                    // Reset mini map zoom
                    minimapZoom = false;
                } else {
                    // Get the pinch ratio of these fingers
                    pinchRatio = NewLeanGesture.GetPinchRatio(fingers, WheelSensitivity);
                    // Assessment camera min and max zoom
                    ZoomMin = 20.0f;
                    ZoomMax = 440.0f;
                }

                // Modify the zoom value
                Zoom *= pinchRatio;

                if (Zoom < ZoomMin)
                    Zoom = ZoomMin;
                else if (Zoom > ZoomMax)
                    Zoom = ZoomMax;

                if (pinchRatio != prevZoom)
                {
                    //Calculate percentage of zoom
					float ZoomPercent = (Zoom-ZoomMin)/(ZoomMax-ZoomMin);
					float distanceMin = CloseUpCameraScript.distanceMin;
					float distanceMax = CloseUpCameraScript.distanceMax;
					CloseUpCameraScript.distance = Mathf.Clamp((ZoomPercent*(distanceMax-distanceMin)+distanceMin), distanceMin, distanceMax);
                    ZoomerScript.UpdateSSVCameraPosition(Zoom);
                }
                Zoom = ZoomerScript.getZoomValue();
                prevZoom = pinchRatio;
			}
		}
	}
}