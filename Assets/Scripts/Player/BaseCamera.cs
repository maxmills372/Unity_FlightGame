using UnityEngine;
using System.Collections;

public class BaseCamera : MonoBehaviour
{
    #region Attributes

        // FOV on boost
        [SerializeField]
        protected float     m_FOVOnBoost                    = 80;

        // Boost view transition duration
        [SerializeField]
        protected float     m_BoostFovTransitionDuration    = 1;

        // Default FOV
        protected float     m_DefaultFOV                    = 60;

        // Camera component
        protected Camera    m_Camera                        = null;

        // Boost coroutine running
        protected Coroutine m_RunningBoostCoroutine         = null;
        protected Coroutine m_RunningLockCoroutine = null;

        public ParticleSystem m_BoostEffectPS;

        public Vector3 lockon_target_pos;
        public Vector3 defaultRot;

    #endregion

    #region Getters & Setters

    // Camera component accessors
    public Camera CameraComponent
        {
            get { return m_Camera; }
        }

    #endregion

    #region MonoBehaviour

        // Use this for initialization
        protected void Start()
        {
            // Get camera component
            m_Camera = GetComponent<Camera>();

            // Get default FOV
            m_DefaultFOV = m_Camera.fieldOfView;

			m_BoostEffectPS = GetComponentInChildren<ParticleSystem>();
        }

    #endregion

    #region Public Manipulators

        /// <summary>
        /// Enable / Disable boost view (FOV effect)
        /// <param name="_Mode">Enabled / Disabled mode</param>
        /// </summary>
        public void SetBoostView(bool _Mode)
        {
            // Stop previous running boost coroutine
            if (m_RunningBoostCoroutine != null)
            {
                StopCoroutine(m_RunningBoostCoroutine);
				//m_BoostEffectPS.Stop();
            }

            // Start boost coroutine
            if (_Mode)
			{
                m_RunningBoostCoroutine = StartCoroutine(CR_SetBoostView(m_Camera.fieldOfView, m_FOVOnBoost));
            }
            else
            {
			    //m_BoostEffectPS.Stop();
                m_RunningBoostCoroutine = StartCoroutine(CR_SetBoostView(m_Camera.fieldOfView, m_DefaultFOV));

            }
        }

    public void SetLockOnView(bool enabled)
    {

        // Stop previous running boost coroutine
        if (m_RunningLockCoroutine != null)
        {
            StopCoroutine(m_RunningLockCoroutine);
            

        }
        // Start boost coroutine
        if (enabled)
        {
            m_RunningLockCoroutine = StartCoroutine(CR_SetLockOnView(transform.rotation.eulerAngles, lockon_target_pos));
            
            //m_Camera.transform.LookAt(Vector3.zero);
      
        }
        else
        {
            //m_BoostEffectPS.Stop();
            m_RunningLockCoroutine = StartCoroutine(CR_SetLockOnView(transform.rotation.eulerAngles, Vector3.zero));

        }
    }


    #endregion

    #region Private Manipulators

    /// Boost view FOV transition between 2 FOV over time
    private IEnumerator CR_SetBoostView(float _FromFOV, float _ToFOV)
        {
            float t = 0;

            float duration = Mathf.Abs(_ToFOV - _FromFOV) / Mathf.Abs(m_FOVOnBoost - m_DefaultFOV);
            duration *= m_BoostFovTransitionDuration;

            while (t < duration)
            {
                t += Time.deltaTime;

                if (t > duration)
                {
                    t = duration;
                }

                m_Camera.fieldOfView = Mathf.Lerp(_FromFOV, _ToFOV, t / duration);

                yield return null;
            }
        }

    private IEnumerator CR_SetLockOnView(Vector3 initialRot, Vector3 target)
    {
        // Attempt at lerping camera view from player to target - WIP
        float t = 0;

        float duration = 1.0f;// Mathf.Abs(initialRot - target) / Mathf.Abs(m_FOVOnBoost - m_DefaultFOV);
        //duration *= m_BoostFovTransitionDuration;

        while (t < duration)
        {
            t += Time.deltaTime;

            if (t > duration)
            {
                t = duration;
            }

           // m_Camera.transform.LookAt(Vector3.Lerp(transform.forward, target, t / duration));

            yield return null;
        }
    }
    #endregion
}
