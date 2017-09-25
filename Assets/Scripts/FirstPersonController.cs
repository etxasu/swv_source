using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private GameObject MyResetTarget;
        [SerializeField]
        private GameObject LeanController;

        [SerializeField]
        private bool UseQuaternionRotation;

        public GameObject MySceneController;
        private GameObject MyCurrentTarget;
        public bool AutoTrackCurrentTarget;

        [SerializeField]
        private bool m_IsWalking;
        [SerializeField]
        private float m_WalkSpeed;
        [SerializeField]
        private float m_RunSpeed;
        [SerializeField]
        [Range(0f, 1f)]
        private float m_RunstepLenghten;
        [SerializeField]
        private float m_JumpSpeed;
        [SerializeField]
        private float m_StickToGroundForce;
        [SerializeField]
        private float m_GravityMultiplier;
        [SerializeField]
        private MouseLook m_MouseLook;
        [SerializeField]
        private bool m_UseFovKick;
        [SerializeField]
        private FOVKick m_FovKick = new FOVKick();
        [SerializeField]
        private bool m_UseHeadBob;
        [SerializeField]
        private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField]
        private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField]
        private float m_StepInterval;
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private CharacterController m_CharacterController;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;

		public float ProjectionLeft;
		public float ProjectionRight;
		public float ProjectionTop;
		public float ProjectionBottom;

        public GameObject TouchedElement;

        private bool FirstUpdate = true;
       
        public bool ControlEnabled;

        protected virtual void OnEnable()
        {
            // Hook into the OnFingerDown event
            Lean.LeanTouch.OnFingerDown += OnFingerDown;

            // Hook into the OnFingerUp event
            Lean.LeanTouch.OnFingerUp += OnFingerUp;
        }

        protected virtual void OnDisable()
        {
            // Unhook the OnFingerDown event
            Lean.LeanTouch.OnFingerDown -= OnFingerDown;

            // Unhook the OnFingerUp event
            Lean.LeanTouch.OnFingerUp -= OnFingerUp;
        }

        //[SerializeField] private Plane[] frustum;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        private void ExposeMyCapi()
        {
            Capi.expose<bool>("Camera.AutoTrackCurrentTarget", () => { return AutoTrackCurrentTarget; }, (value) => { return AutoTrackCurrentTarget = value; });
        }


        // Update is called once per frame
        private void Update()
        {
            if(FirstUpdate)
            {
                ExposeMyCapi();
                FirstUpdate = false;
            }

            if (!AutoTrackCurrentTarget)
            {
                if (CrossPlatformInputManager.GetAxis("Fire1") > 0.0f)
                {
                    if (ControlEnabled)
                    {
                        // Check to see if they're over a GUI element
                        if (TouchedElement != null && TouchedElement.layer == 12)
                        {
                            //RotateView();
                        }
                        else
                        {
                            RotateView();
                        }
                    }
                }
            }
            else
            {
                AutoTrackTarget();
            }
        }

		void LateUpdate()
		{
			//Matrix4x4 _newProjection = PerspectiveOffCenter(ProjectionLeft, ProjectionRight, ProjectionBottom, ProjectionTop, m_Camera.nearClipPlane, m_Camera.farClipPlane);
			//m_Camera.projectionMatrix = _newProjection;

			SetObliqueness (ProjectionRight, ProjectionTop);
		}

		private void SetObliqueness(float _h, float _v)
		{
			Matrix4x4 mat  = Camera.main.projectionMatrix;
			mat[0, 2] = _h;
			mat[1, 2] = _v;
			m_Camera.projectionMatrix = mat;
		}

		static Matrix4x4 PerspectiveOffCenter(float _left, float _right, float _bottom, float _top, float _near, float _far)
		{
			float x = 2.0F * _near / (_right - _left);
			float y = 2.0F * _near / (_top - _bottom);
			float a = (_right + _left) / (_right - _left);
			float b = (_top + _bottom) / (_top - _bottom);
			float c = -(_far + _near) / (_far - _near);
			float d = -(2.0F * _far * _near) / (_far - _near);
			float e = -1.0F;

			Matrix4x4 m = new Matrix4x4();
			m[0, 0] = x;
			m[0, 1] = 0;
			m[0, 2] = a;
			m[0, 3] = 0;
			m[1, 0] = 0;
			m[1, 1] = y;
			m[1, 2] = b;
			m[1, 3] = 0;
			m[2, 0] = 0;
			m[2, 1] = 0;
			m[2, 2] = c;
			m[2, 3] = d;
			m[3, 0] = 0;
			m[3, 1] = 0;
			m[3, 2] = e;
			m[3, 3] = 0;
			return m;
		}

        private void PlayLandingSound()
        {
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

            //ProgressStepCycle(speed);
            //UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {

        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;

            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            //float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            //float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            float horizontal = Lean.LeanTouch.DragDelta.x;
            float vertical = Lean.LeanTouch.DragDelta.y;

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            if (UseQuaternionRotation)
            {
                m_MouseLook.LookRotation(transform, m_Camera.transform);
            }
            else
            {
                m_MouseLook.EulerRotation(transform);
            }
            //Lean.LeanTouch.RotateObject(transform, Lean.LeanTouch.DragDelta.y);
        }

        private void AutoTrackTarget()
        {
            transform.LookAt(MySceneController.GetComponent<SceneController>()._selected.transform);
            transform.parent.LookAt(MySceneController.GetComponent<SceneController>()._selected.transform);

            // Flatten our magic out
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
            transform.parent.localEulerAngles = new Vector3(0.0f, transform.parent.localEulerAngles.y, 0.0f);
        }

        public void ResetView()
        {
            Debug.Log("Resetting main camera view");
            m_MouseLook.ResetLook(transform, m_Camera.transform);
        }

        public void OnFingerDown(Lean.LeanFinger finger)
        {
            if (finger.WhatTouched() != null)
            {
                TouchedElement = finger.WhatTouched();
            }
                // deeerp
        }

        public void OnFingerUp(Lean.LeanFinger finger)
        {
            TouchedElement = null;
            Capi.set("Camera.Rotation.x", transform.rotation.x);
            Capi.set("Camera.Rotation.y", transform.parent.rotation.y);
        }
    }
}