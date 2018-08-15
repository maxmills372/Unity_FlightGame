using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
	#region PublicAttributes    
	[Header("Attributes")]
	public float		m_MovementSpeed         = 40f;
	public float        m_SprintScale           = 2f;        
	public float        m_RollRotationSpeed     = 20f;
	public float 		m_PitchRotationSpeed 	= 20f;
	public float 		m_YawRotationSpeed 		= 50f;
	public float        m_DodgeRollSpeed        = 80;
	public float        m_DodgeMovementSpeed    = 100;   
	public float 		bullet_offset_amount   = 0.01f;
    // Rotation limits
    [Range(0, 90)] 
	public float        m_PitchLimit            = 45;
    [Range(0, 90)]
	public float        m_RollLimit             = 45;
    // Yaw sensibility on roll
    [Range(0, 1)]
	public float        m_YawSensibilityOnRoll  = 0.33f;
	public float max_increase_speed 			= 20f;
	public float min_increase_speed				= -20f;
	public float speed_increase_scale 			= 0.1f;


	[Header("Unity Gameobjects")]
	public Transform 		bullet_spawn_pos;
	public Transform 		bullet_spawn_pos_2;
	public ParticleSystem 	Muzzle_Flash_PS,Muzzle_Flash_PS_2;
	public GameObject 		hit_PS;
	public LineRenderer		bullet_line, bullet_line_2;
	public Text 			UI_Speed;
	public Transform 		rocket_target;
	public GameObject		rocket_prefab;

	#endregion     

	#region PrivateVariables

	private Controller  m_Controller            = null;
    private bool        m_IsDodging             = false;
    private BaseCamera  m_Camera             	= null;
	private Rigidbody   m_Rigidbody             = null;
	Vector3 bullet_offset;
	float speed_increase = 0f;
	float increase_amount = 0f;

	GameObject[] targets = new GameObject[100];
	Transform current_target;
	Transform[] current_targets = new Transform[6];

    #endregion

    #region MonoBehaviour

        // Use this for initialization
        void Start()
        {		
            // Get controller
            m_Controller = GetComponent<Controller>();

            // Get rigidbody
            m_Rigidbody = GetComponent<Rigidbody>();

            if (m_Rigidbody != null)
            {
                m_Rigidbody.useGravity = false;
            }

			targets = GameObject.FindGameObjectsWithTag("Player");


        }

        // Called at fixed time
        void FixedUpdate()
        {
            // Update yaw from roll angle. Writtent in fixed update to avoid camera lerp break
            UpdateYawFromRoll();

			if (Input.GetButtonUp("Fall"))
			{
				FreeFall(false);
				print("DONE");
			}
			if (Input.GetButton("Fall"))
			{

				FreeFall(true);
			}

        }

        // On collision enter
        void OnCollisionEnter(Collision collision)
        {
            if (m_Rigidbody != null)
            {
                /*
                // Falling
                m_Rigidbody.useGravity = true;

                // Disable controller
                m_Controller.enabled = false;

                // Stop boost view
                if (m_Camera != null)
                {
                    m_Camera.SetBoostView(false);
                }
                 * */
            }
        }

	public void FreeFall(bool toggle)
	{
		if(toggle)
		{
			// Falling
			m_Rigidbody.useGravity = true;

			// Disable controller
			m_Controller.enabled = false;

			// Stop boost view
			if (m_Camera != null)
			{
				m_Camera.SetBoostView(false);
			}
		}
		else
		{
			// Falling
			m_Rigidbody.useGravity = false;

			// Disable controller
			m_Controller.enabled = true;

			// Stop boost view
			if (m_Camera != null)
			{
				m_Camera.SetBoostView(true);
			}
		}

	}
    #endregion

    #region Getters & Setters

        // Base camera accessors
        public BaseCamera BaseCamera
        {
            get { return m_Camera; }
            set { m_Camera = value; }
        }

        // Is dodging
        public bool IsDodging
        {
            get { return m_IsDodging; }
        }

        // Controller accessors
        public Controller Controller
        {
            get { return m_Controller; }
        }

    #endregion

    #region Public Manipulators

		// Fires a rocket straight or at target
		public void RocketShoot(Transform target)
		{
		
			if(target == null)
			{
			GameObject rocket = Instantiate(rocket_prefab,bullet_spawn_pos.position,Quaternion.LookRotation(transform.forward));
				rocket.GetComponent<homing_missile>().CurrentRocketType = homing_missile.RocketType.Straight;
			}
			else
			{

			}

		}

		/// <summary>
		/// Tracks the closest targets
		/// </summary>
		public void TrackClosestTarget()
		{
			float current_min = 100000.0f;


			foreach(GameObject g in targets)
			{
				float distance = Vector3.Distance(g.transform.position, transform.position);
				if(distance < current_min)
				{
					current_min = distance;
					current_target = g.transform;
				}
			}

			rocket_target = current_target;
		}

	//TODO
	// Have this called 6 times, every time ignoring the closest target found before
	public Transform TrackClosestTargets(int count)
	{
		float current_min = 100000.0f;

		foreach(GameObject g in targets)
		{
			if(count == 6)
				break;
			
			float distance = Vector3.Distance(g.transform.position, transform.position);
			if(distance < current_min)
			{
				current_min = distance;
				current_targets[count] = g.transform;
				count++;
			}
		}
		return current_target;

		//rocket_targets = current_targets;

	}
		/// <summary>
		/// Machines the gun shoot. (lol wtf)
		/// </summary>
		public void MachineGunShoot()
		{
			// Bullet spread offset
			bullet_offset = new Vector3(Random.Range(-bullet_offset_amount,bullet_offset_amount),
										Random.Range(-bullet_offset_amount,bullet_offset_amount),
										0f);
			// Creates firing rays
			Ray gun1_ray = new Ray(bullet_spawn_pos.position, bullet_spawn_pos.forward + bullet_offset);
			Ray gun2_ray = new Ray(bullet_spawn_pos_2.position, bullet_spawn_pos_2.forward + bullet_offset);

			bullet_line.enabled = true;
			bullet_line_2.enabled = true;

			bullet_line.SetPosition(0, bullet_spawn_pos.position);
			bullet_line.SetPosition(1, bullet_spawn_pos.position + gun1_ray.direction.normalized * 100f );
			bullet_line_2.SetPosition(0, bullet_spawn_pos_2.position);
			bullet_line_2.SetPosition(1, bullet_spawn_pos_2.position + gun2_ray.direction.normalized * 100f );

			// Shoots ray for each gun
			Shoot_Bullet_Raycast(gun1_ray);
			Shoot_Bullet_Raycast(gun2_ray);
		
			// Plays both muzzle flash effects
			Muzzle_Flash_PS.Play();
			Muzzle_Flash_PS_2.Play();

			//Debug.DrawRay(gun1_ray.origin,gun1_ray.direction*1000f,Color.red);
		}

		void Shoot_Bullet_Raycast(Ray ray_to_shoot)
		{
			RaycastHit hit_;

			// Raycasts out
			if(Physics.Raycast(ray_to_shoot, out hit_,1000f))
			{
				// If hit a cube
				if(hit_.collider.tag == "Lockon")
				{
					// Destroy cube
					Destroy(hit_.collider.gameObject);
				}

				// Instantiate impact effect and destroy after 1 second
				GameObject hit_PS_instance = Instantiate(hit_PS, hit_.point,Quaternion.LookRotation(hit_.normal));
				Destroy(hit_PS_instance,0.5f);
			}
		}
		/// <summary>
		/// Updates the speed.
		/// </summary>
		public void UpdateSpeed(float speed)
		{
			// This is used if an axis is used, or if speed is on button press
			if(speed > 0f)
				speed_increase = 1f;
			else if(speed < 0f)
				speed_increase = -1f;
			else
				speed_increase = 0f;
		}
        /// <summary>
        /// Start coroutine responsible of dodge
        /// </summary>
        /// <param name="_Input">Input value on [-1, 1]</param>
        public void Dodge(float _Input)
        {
            // Dodge event on camera
            if (m_Camera != null)
            {
                if (m_Camera is TPSCamera)
                {
                    ((TPSCamera)m_Camera).OnCharacterDodge();
                }
            }

            // Start dodge
            StartCoroutine(CR_Dodge(_Input));
        }

        /// <summary>
        /// Move character in specified direction. Move the rigidboy
        /// </summary>
        /// <param name="_Direction">Movement direction (world space)</param>
        /// <param name="_Sprint">Bosst speed</param>
        /// <param name="_Dodge">Is a dodge movement</param>
        public void Move(Vector3 _Direction, bool _Sprint, bool _Dodge = false)
        {
            if (m_Rigidbody != null)
            {
                // Speed management
                float speed = m_MovementSpeed;

                if (_Dodge)
                {
                    speed = m_DodgeMovementSpeed;
                }

                if (_Sprint)
                {
                    speed *= m_SprintScale;
                }
				else if(!_Dodge)
				{
					speed += increase_amount;
				}

				if(speed_increase > 0f)
				{
					if(increase_amount < max_increase_speed)
					{
						//speed += increase_scale;
						increase_amount += speed_increase_scale;
					}
				}
				else if(speed_increase < 0f)
				{
					if(increase_amount > min_increase_speed)
					{
						//speed -= increase_scale;
						increase_amount -= speed_increase_scale;
					}
				}				
				
				UI_Speed.text = speed.ToString();

                // Movement
                m_Rigidbody.MovePosition(m_Rigidbody.position + _Direction.normalized * Time.deltaTime * speed);
            }
        }

        /// <summary>
        /// Reset roll rotation with SLerp over time
        /// </summary>
        public void ResetRoll()
        {
            // Calculate rotation
            Vector3 rightNoY = Vector3.Cross(Vector3.up, transform.forward);
            rightNoY.y = 0;
            Quaternion rotator = Quaternion.FromToRotation(transform.right, rightNoY);

            // Apply rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rotator * transform.rotation, Time.deltaTime);
        }

        /// <summary>
        /// Reset pitch rotation with SLerp over time
        /// </summary>
        public void ResetPitch()
        {
            // Calculate rotation
            Vector3 forwardNoY = transform.forward;
            forwardNoY.y = 0;
            Quaternion rotator = Quaternion.FromToRotation(transform.forward, forwardNoY);

            // Apply rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rotator * transform.rotation, Time.deltaTime);
        }

        /// <summary>
        /// Reset pitch and roll to 0
        /// </summary>
        public void ResetOrientation()
        {
            // Projected forward
            Vector3 forwardNoY = transform.forward;
            forwardNoY.y = 0;
            forwardNoY.Normalize();

            // Apply projected forward
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forwardNoY), Time.deltaTime * 1.5f);
        }

        /// <summary>
        /// Add roll to character rotation
        /// </summary>
        public void AddRoll(float _AdditiveRoll, bool _Dodge = false)
        {
            // Check roll limit
            
		if (m_RollLimit > 0)
		{
			if (!CheckRollLimit(_AdditiveRoll))
			{
				return;
			}
		}
            // Time based rotation
            if (!_Dodge)
            {
				_AdditiveRoll *= Time.deltaTime * m_RollRotationSpeed;
            }
            else
            {
                _AdditiveRoll *= Time.deltaTime * m_DodgeRollSpeed;
            }

	


            // Add rotation
			// Use transform.forward for plane controls
			Quaternion rotator = Quaternion.AngleAxis(_AdditiveRoll, -transform.forward);
            transform.rotation = rotator * transform.rotation;
        }

        /// <summary>
        /// Add pitch to character rotation
        /// </summary>
        public void AddPitch(float _AdditivePitch)
        {
            // Check pitch limit
            /*
            if (m_PitchLimit > 0)
            {
                if (!CheckPitchLimit(_AdditivePitch))
                {
                    return;
                }
            }*/

            // Time based rotation
			_AdditivePitch *= Time.deltaTime * m_PitchRotationSpeed;

            // Add rotation
            Quaternion rotator = Quaternion.AngleAxis(_AdditivePitch, transform.right);
            transform.rotation = rotator * transform.rotation;
        }

		/// <summary>
		/// Adds the yaw.
		/// </summary>
		public void AddYaw(float _AdditiveYaw)
		{
			// Check yaw limit
			/*
	            if (m_YawLimit > 0)
	            {
	                if (!CheckPitchLimit(_AdditiveYaw))
	                {
	                    return;
	                }
	            }*/

			// Time based rotation
			_AdditiveYaw *= Time.deltaTime * m_YawRotationSpeed;

			// Add rotation
			Quaternion rotator = Quaternion.AngleAxis(-_AdditiveYaw, transform.up);
			transform.rotation = rotator * transform.rotation;
		}

    #endregion

    #region Private Manipulators

        /// <summary>
        /// Dodge on left or right depending of input
        /// </summary>
        private IEnumerator CR_Dodge(float _Input)
        {
            float sumInput = 0;
            m_IsDodging = true;

            // Get old right local axis
			Vector3 oldRight = transform.right;

            // Wait entire roll
            while (Mathf.Abs(sumInput) < 360)
            {
                // Roll
                AddRoll(-_Input,true);

                // Movement
                Move(oldRight * _Input, false, true);

                sumInput += _Input * m_DodgeRollSpeed * Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }

            m_IsDodging = false;
        }

        /// <summary>
        /// Check next pitch operation will not go out pitch limit
        /// </summary>
        /// <param name="_AdditivePitch">Next pitch added</param>
        /// <returns>True if roll can be applied, False otherwise</returns>
        private bool CheckPitchLimit(float _AdditivePitch)
        {
            // Project local right on "flat" right
            Vector3 forwardNoY = transform.forward;
            forwardNoY.y = 0;
            forwardNoY.Normalize();

            // Get roll angle
            float pitch = Vector3.Angle(transform.forward, forwardNoY);

            if (Vector3.Cross(transform.right, forwardNoY).y < 0)
            {
                pitch *= -1;
            }

            // Check roll limit
            if (m_PitchLimit > 0)
            {
                if (pitch + _AdditivePitch > m_RollLimit)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check next roll operation will not go out roll limit
        /// </summary>
        /// <param name="_AdditiveRoll">Next roll added</param>
        /// <returns>True if roll can be applied, False otherwise</returns>
        private bool CheckRollLimit(float _AdditiveRoll)
        {
            // Project local right on "flat" right
            Vector3 rightNoY = transform.right;
            rightNoY.y = 0;
            rightNoY.Normalize();

            // Get roll angle
            float roll = Vector3.Angle(transform.right, rightNoY);

            if (Vector3.Cross(transform.forward, rightNoY).y < 0)
            {
                roll *= -1f;
            }

            // Check roll limit
            if (m_RollLimit > 0)
            {
                if (roll + _AdditiveRoll > m_RollLimit)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update the rotation on world Yaw axis depending on roll value.
        /// Represents the turn left / right effect on airplane rolling.
        /// </summary>
        private void UpdateYawFromRoll()
        {
            if (!m_IsDodging)
            {
                float upSign = 1;

                if (transform.up.y < 0)
                {
                    upSign = -1;
                }

                float yawSensibility = m_YawSensibilityOnRoll;
                Vector3 rightNoY = transform.right;
                rightNoY.y = 0;
                rightNoY.Normalize();
                float dot = Vector3.Dot(transform.up, rightNoY);

                yawSensibility *= dot * upSign;

                Quaternion rotator = Quaternion.AngleAxis(yawSensibility, Vector3.up);
                transform.rotation = rotator * transform.rotation;
            }
        }

    #endregion
}
