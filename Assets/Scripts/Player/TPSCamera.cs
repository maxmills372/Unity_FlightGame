using UnityEngine;
using System.Collections;

public class TPSCamera : BaseCamera
{
    #region Attributes

        // Character
        [SerializeField]
        private Character   m_Character                     = null;

        // Offset
        [SerializeField]
        private Vector3     m_CharacterOffset               = new Vector3(0, 1, -6);

        // Character up before dodge
        private Vector3     m_CharacterUpBeforeDodge        = Vector3.zero;

        // Lerp factor
        [SerializeField]
        private float       m_LerpFactor                    = 6;
    #endregion

    #region MonoBehaviour

        // Called at fixed time
        void FixedUpdate()
        {
            //  Camera follow character. Written in fixed update to avoid camera lerp break
            if (m_Character != null)
            {
                // Calculate local offset depending on dodge action
                Vector3 localOffset = m_Character.transform.right * m_CharacterOffset.x + m_Character.transform.up * m_CharacterOffset.y + m_Character.transform.forward * m_CharacterOffset.z;
                Vector3 desiredPosition;

                if (m_Character.IsDodging)
                {
                    localOffset = m_Character.transform.right * m_CharacterOffset.x + m_CharacterUpBeforeDodge * m_CharacterOffset.y + m_Character.transform.forward * m_CharacterOffset.z;
                }
                else if (m_Character.m_IsLockedOn)
                {
                    // Offset from player, looking at rotation (which is pointing towards target)
                    localOffset = transform.right * m_CharacterOffset.x + transform.up * m_CharacterOffset.y + transform.forward * m_CharacterOffset.z;
                }
                           
                // Update position based on offset
                desiredPosition = m_Character.transform.position + localOffset;
                transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.fixedDeltaTime * m_LerpFactor);
            }


            // Follow character rotation depending on dodge action
            if (!m_Character.IsDodging)
            {
                if (!m_Character.m_IsLockedOn)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, m_Character.transform.rotation, Time.fixedDeltaTime * m_LerpFactor);
                    //defaultRot = transform.rotation.eulerAngles;

                }                
            }
        }

    #endregion

    #region Public Manipulators

        /// <summary>
        /// Called when character start a dodge
        /// </summary>
        public void OnCharacterDodge()
        {
            // Keep old character up
            m_CharacterUpBeforeDodge = m_Character.transform.up;
        }

    #endregion
}
