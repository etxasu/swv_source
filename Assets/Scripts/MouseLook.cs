using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public bool clampHorizontalRotation = true;
        public float MinPitch = -90F;
        public float MaxPitch = 90F;
        public float MinYaw = -90F;
        public float MaxYaw = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private Quaternion m_parentTargetRot;
        private bool m_cursorIsLocked = true;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            m_parentTargetRot = camera.transform.parent.localRotation;

            //Debug.Log(camera.transform.parent.name);
        }


        public void LookRotation(Transform character, Transform camera)
        {

            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            //float yRot = Lean.LeanTouch.DragDelta.x * YSensitivity;
            //float xRot = -Lean.LeanTouch.DragDelta.y * XSensitivity;

            //Debug.Log("NARK");

            m_CharacterTargetRot *= Quaternion.Euler(0.0f, 0.0f, 0.0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0.0f, 0.0f);
            m_parentTargetRot *= Quaternion.Euler(0.0f, 0.0f , yRot);

            // Lock in some human like movement.
            //JOS: 6/30/2016

            //m_CameraTargetRot = new Quaternion(m_CameraTargetRot.x, m_CameraTargetRot.y, 0.0f, m_CameraTargetRot.w);

            //Debug.Log("CLICKED A MOVER");

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
            /*
            if (clampHorizontalRotation)
                m_CameraTargetRot = ClampRotationAroundZAxis(m_CameraTargetRot);
            */
            if (smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
                camera.transform.parent.localRotation = m_parentTargetRot;
            }

            UpdateCursorLock();
        }

        public void EulerRotation(Transform Target)
        {
            //yaw = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            //pitch = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            yaw = Lean.LeanTouch.DragDelta.x * XSensitivity;
            pitch += Lean.LeanTouch.DragDelta.y * YSensitivity;
            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);
            //Debug.Log(Target.localEulerAngles);

            Target.localEulerAngles = new Vector3(-pitch, 0.0f, 0.0f);
            Target.parent.localEulerAngles += new Vector3(0.0f, yaw, 0.0f);

            // These are flipped intentionally because of shitty nested camera rotation problems
            float _angleY = Target.localEulerAngles.x;
            float _angleX = Target.parent.localEulerAngles.y;

            //Debug.Log(Target.parent.name);

            //Debug.Log(MaxPitch + " | " + _angleY);
            if (clampHorizontalRotation)
            {
                // Turning too much right
                if (_angleX > MaxYaw)
                {
                    //Debug.Log("lol");
                    Target.parent.localEulerAngles += new Vector3(0.0f, -yaw, 0.0f);
                }
                // Turning too much left
                if (_angleX < MinYaw)
                {
                    Target.parent.localEulerAngles += new Vector3(0.0f, -yaw, 0.0f);
                }
            }

            if (clampVerticalRotation)
            {
                /*
                if (_angleY < 180.0f)
                {
                    if (_angleY > MinPitch)
                    {
                        Target.localEulerAngles += new Vector3(pitch, 0.0f, 0.0f);
                    }
                }

                if (_angleY > 180.0f)
                {
                    if (_angleY < MaxPitch)
                    {
                        Target.localEulerAngles += new Vector3(pitch, 0.0f, 0.0f);
                    }
                }
                */
            }

        }

        public void ResetLook(Transform character, Transform camera)
        {
            m_CameraTargetRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            m_CharacterTargetRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            /*
            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
            */

            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;

            // SUPER KLUDGE
            //
            

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinPitch, MaxPitch);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        Quaternion ClampRotationAroundZAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

            angleZ = Mathf.Clamp(angleZ, MinYaw, MaxYaw);

            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q;
        }

    }
}
