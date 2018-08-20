using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    // Reset orientation method (all axis or per axis)
    enum EResetOrientationMethod
    {
        AllAxis,
        PerAxis,
		OnButtonPress,
        None
    }
	enum FlightModes
	{
		FLYING,
		HOVER,
		FALL
		/*WALKING,
		TAKEOFF,
		LANDING*/
	};

    #region Attributes
    // Character
    private Character               m_Character                 = null;
    
    // Reset orientation method
    [SerializeField]
    private EResetOrientationMethod m_ResetOrientationMethod    = EResetOrientationMethod.AllAxis;
	private FlightModes flight_mode = FlightModes.FLYING;

    // Reset all axis timer
    [SerializeField]
    private float                   m_ResetAllAxisTimeToWait    = 1;
    private float                   m_ResetAllAxisTimer         = 0;

    // Reset pitch axis timer
    [SerializeField]
    private float                   m_ResetPitchAxisTimeToWait  = 1;
    private float                   m_ResetPitchAxisTimer       = 0;

    // Reset roll axis timer
    [SerializeField]
    private float                   m_ResetRollAxisTimeToWait   = 1;
    private float                   m_ResetRollAxisTimer        = 0;

    // Game camera
    [SerializeField]
    private TPSCamera               m_TPSCamera                = null;

    // FPS camera
    [SerializeField]
    private FPSCamera               m_FPSCamera                 = null;

    // Current camera
    private BaseCamera              m_CurrentCamera             = null;

	public bool m_InvertYAxis = true;
	public bool m_LockMouse = true;
	public float m_TimeBetweenRocketFire = 0.5f;
	public float m_RocketFireDelay = 0.3f;

	float rocket_timer = 0f;
	float timer = 0f;
	public float m_LockonHoldTime = 1.5f;
	bool button_held = false;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        // Get character
        m_Character = GetComponent<Character>();

        // Current camera
        if (m_TPSCamera != null)
        {
            m_CurrentCamera = m_TPSCamera;

            if (m_FPSCamera != null)
            {
                m_FPSCamera.GetComponent<Camera>().enabled = false;
                Camera.SetupCurrent(m_CurrentCamera.CameraComponent);
            }
        }
    }

	// Use this for checking input (apart from movement input)
	void Update()
	{		
		// Check input for weapon actions
		CheckWeaponInput();

		if (!m_Character.IsDodging)
		{
			// Check movement input
			CheckMovementInput();

			// Check input for other action buttons
			CheckActionInput();
		}

		m_Character.Move(m_Character.transform.forward, Input.GetButton("Sprint"));

	}

    // Called at fixed time
    void FixedUpdate()
    {
        if (m_Character != null)
        {

			switch (flight_mode)
			{
			case FlightModes.FLYING:
				{
		            if (!m_Character.IsDodging)
		            {
		                //CheckMovementInput();
		            }

					// Written in fixed update to avoid camera lerp break

		            // Check camera inputs
		            CheckCameraInput();

					// Move character (in fixed update to avoid camera lerp break)
					// Dont need to do this here anymore
					//m_Character.Move(m_Character.transform.forward, Input.GetButton("Sprint"));
				
					break;
				}
			case FlightModes.FALL:
				{
					break;
				}

			default:
				break;
			}
        }
    }

	#endregion

	#region Private Manipulators

    /// Switch current camera with another camera
    private void SwitchCamera()
    {
        // Check camera null
        if (m_TPSCamera != null && m_FPSCamera != null)
        {
            // Switch camera
            if (m_CurrentCamera == m_TPSCamera)
            {
                m_CurrentCamera = m_FPSCamera;
                m_FPSCamera.CameraComponent.enabled = true;
                m_TPSCamera.CameraComponent.enabled = false;
            }
            else
            {
                m_CurrentCamera = m_TPSCamera;
                m_FPSCamera.CameraComponent.enabled = false;
                m_TPSCamera.CameraComponent.enabled = true;
            }

            // Set new current camera
            Camera.SetupCurrent(m_CurrentCamera.CameraComponent);

            // Set current camera to character
            if (m_Character != null)
            {
                m_Character.BaseCamera = m_CurrentCamera;
            }
        }
    }

    /// Check camera inputs and call camera actions associated
	private void CheckCameraInput()
    {
        // Switch camera
        if (Input.GetButtonDown("SwitchCamera"))
        {
            SwitchCamera();
        }
    }

    /// Check action inputs and call character actions associated
	private void CheckActionInput()
    {
		//TODO
        // Input - MAKE THIS BUTTON INSTEAD OF AXIS
        float dodgeAxis = Input.GetAxis("Dodge");

        // Dodge
        if (dodgeAxis != 0)
        {
            m_Character.Dodge(dodgeAxis);
        }

        // Boost effect
        if (Input.GetButtonDown("Sprint"))
        {
            if (m_CurrentCamera != null)
            {
                m_CurrentCamera.SetBoostView(true);
            }
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            if (m_CurrentCamera != null)
            {
                m_CurrentCamera.SetBoostView(false);
            }
        }

		if (Input.GetButtonDown("AirBrake"))
		{
			print("AIRBRAKE");

			//m_CurrentCamera.SetTurnView(true);
			//m_Character.m_DodgeRollSpeed += 100;
			//m_Character.AddRoll(Input.GetAxis("MouseX") * -5.0f,true);

			//m_Character.m_DodgeRollSpeed -= 100;

		}
    }

	/// Checks the weapon input
	private void CheckWeaponInput()
	{
		/// Machine Guns
		if(Input.GetButton("Shoot"))
		{
			m_Character.MachineGunShoot();

		}
		else
		{
			m_Character.m_BulletLineRend.enabled = false;
			m_Character.m_BulletLineRend_2.enabled = false;
		}

		/// Rockets
		rocket_timer += Time.deltaTime;

		if(Input.GetButton("RocketShoot") )
		{
			button_held = true;
			timer += Time.deltaTime;
			if( timer > m_LockonHoldTime)
			{
				//m_Character.TrackClosestTargets();
				print("HEllo there");

			}
		}
		if (Input.GetButtonUp("RocketShoot"))
		{
			timer = 0f;
			button_held = false;
		}
		if(Input.GetButtonUp("RocketShoot") && rocket_timer > m_TimeBetweenRocketFire && !button_held)
		{
			// Shoot stright rocket twice with delay on the 2nd one
			m_Character.RocketShoot(null, true);
			//m_Character.RocketShoot(null, false);

			StartCoroutine("Rocket_Shoot_Delay");

			// Reset timer
			rocket_timer = 0f;
			button_held = false;

		}

	}

    /// Check movement input and call character actions associated
    private void CheckMovementInput()
    {
        // Input
		//float throttleAxis = Input.GetAxis("MouseScroll"); //Change speed increase scale to 10f
		float throttleAxis = Input.GetAxis("Throttle");
        float rudderAxis = Input.GetAxis("Rudder");

		float pitchAxis = Input.GetAxis("MouseY");
		float rollAxis = Input.GetAxis("MouseX");

		if(m_LockMouse)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

        // Pitch
        if (pitchAxis != 0)
        {
			// Invert pitch
			if(m_InvertYAxis)
	            m_Character.AddPitch(pitchAxis);
			else
				m_Character.AddPitch(-pitchAxis);		
        }

        // Roll
        if (rollAxis != 0)
        {
            m_Character.AddRoll(rollAxis);
        }


		m_Character.UpdateSpeed(throttleAxis);

		m_Character.AddYaw(rudderAxis);


        // Reset pitch and roll depending on reset orientation method
        if (m_ResetOrientationMethod == EResetOrientationMethod.AllAxis)
        {
            // Check no inputs
            if (pitchAxis == 0 && rollAxis == 0)
            {
                // Check timer
                if (m_ResetAllAxisTimer > m_ResetAllAxisTimeToWait)
                {
                    // Reset general orientation
                    m_Character.ResetOrientation();
                }

                m_ResetAllAxisTimer += Time.deltaTime;
			}
            else
            {
                m_ResetAllAxisTimer = 0;
            }
		}
        else if (m_ResetOrientationMethod == EResetOrientationMethod.PerAxis)
        {
            // Check pitch no input
            if (pitchAxis == 0)
            {
                // Check pitch timer
                if (m_ResetPitchAxisTimer > m_ResetPitchAxisTimeToWait)
                {
                    // Reset pitch orientation
                    m_Character.ResetPitch();
                }

                m_ResetPitchAxisTimer += Time.deltaTime;
            }
            else
            {
                m_ResetPitchAxisTimer = 0;
            }

            // Check roll no input
            if (rollAxis == 0)
            {
                // Check roll timer
                if (m_ResetRollAxisTimer > m_ResetRollAxisTimeToWait)
                {
                    m_Character.ResetRoll();
                }

                m_ResetRollAxisTimer += Time.deltaTime;
            }
            else
            {
                m_ResetRollAxisTimer = 0;
            }
        }
		else if (m_ResetOrientationMethod == EResetOrientationMethod.OnButtonPress)
		{
			// Check pitch input
			if (Input.GetButton("ResetOrientation"))
			{				
				m_Character.ResetOrientation();
			}

		}
    }

	IEnumerator Rocket_Shoot_Delay()
	{
		yield return new WaitForSeconds(m_RocketFireDelay);
		m_Character.RocketShoot(null,false);
		yield return null;
	}

#endregion
}
