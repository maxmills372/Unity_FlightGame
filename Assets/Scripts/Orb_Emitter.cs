using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Emitter : MonoBehaviour {

	[Header("Setup Fields")]
	public Transform pivot_point;
	public Transform spawn_pos;
	public GameObject Orb_Instance;
	public Transform player_;
	public Transform orb_parent;

	[Header("Orb Attributes")]

	public float orb_speed = 10f;
	[Range(0.1f,20f)]
	public float fire_rate = 10f;
	[Range(0.1f,15f)]
	public float burst_delay_rate = 15.0f;
	[Range(0,20)]
	public int burst_amount = 5;
	[Range(0f,20f)]
	public float rotate_speed = 1f;
	public Vector3 rotate_axis = new Vector3(1,0,0);

	public bool orb_accelerate = false;
	public float accel_amount = 0.2f;
	public float orb_rising_speed = 2f;

	float counter, timer;
	float test;
	bool is_homing = false;
	bool do_it;
	public int shoot_count = 0;
	float difference;
	Vector3 defualt_pos;
	Quaternion defualt_rotation;

	[System.Serializable]
	public enum Firing_Modes
	{
		Line,					// Fires in current direction
		Line_with_burst,		// Fires in current direction in short bursts
		Follow,					// Follows player position 
		Homing,					// Orbs follow player position
		Curved_Line,			// Rotates along curved path
		Better_Curved_Line,
		Circular,				// Rotates around an axis
	}
	public Firing_Modes Firing_Mode;

	// Use this for initialization
	void Start () 
	{
		defualt_rotation = transform.rotation;
		defualt_pos = transform.position;
		test = rotate_speed;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		is_homing = false;

		switch(Firing_Mode)
		{
		case Firing_Modes.Line:
			{

				Shoot_with_delay();

				break;
			}
		case Firing_Modes.Line_with_burst:
			{
				if(shoot_count < burst_amount)
				{	
					
					Shoot_with_delay();					
				}
				else
				{
					/*timer += Time.time / 60f;
					if(timer > burst_delay_rate)
					{
						shoot_count = 0;
						timer = 0;
					}*/

					if(timer <= 0f)
					{
						shoot_count = 0;

						timer = 1f/burst_delay_rate;
					}
					timer -= Time.deltaTime;
				}
				break;
			}
		
		case Firing_Modes.Follow:
			{
				
				//transform.parent.transform.LookAt(player_.position);
				// D = S*T
				//float b = player_.GetComponent<Rigidbody>().velocity.magnitude * 
				//distance_ahead = 
				transform.parent.transform.LookAt(player_.position + (player_.GetComponent<Rigidbody>().velocity.normalized * orb_speed));

				Shoot_with_delay();					
				break;
			}
		case Firing_Modes.Homing:
			{
				transform.parent.transform.LookAt(player_.position);
				is_homing = true;
				Shoot_with_delay();			
				break;
			}
		case Firing_Modes.Curved_Line:
			{
							
				difference = Vector3.Angle(transform.up,transform.parent.transform.forward);
				if(difference > 60f)
				{
					rotate_speed = -rotate_speed;

				}
				transform.RotateAround(pivot_point.position,rotate_axis,rotate_speed);
				Shoot_with_delay();
				break;
			}
		case Firing_Modes.Better_Curved_Line:
			{
				/*if(rotate_speed > 0f)
				{		
					rotate_speed = Mathf.Lerp(rotate_speed,0f,(Time.time/60f));
					if(rotate_speed < 0.1f)
					{
						rotate_speed = -0.1f;
					}
					print("+");
				}
				else
				{
					rotate_speed = Mathf.Lerp(rotate_speed,-1f,Time.time/60f);

					if(rotate_speed < -0.9f)
					{
						rotate_speed = test;
						do_it = true;
					}
					print("-");
				}*/
				if((transform.localEulerAngles.z > 0f && transform.localEulerAngles.z > 60f) || (transform.localEulerAngles.z < 0f && transform.localEulerAngles.z < -60f))
				{
					rotate_speed = -rotate_speed;
				}
				transform.RotateAround(pivot_point.position,rotate_axis,rotate_speed);
				Shoot_with_delay();
				break;
			}

		case Firing_Modes.Circular:
			{				
				orb_speed = 10f;
				fire_rate = 20f;
				transform.RotateAround(pivot_point.position,rotate_axis, rotate_speed);
				Shoot_with_delay();

				break;
			}
		
			default:
			break;
		}		
	}

	void Shoot_with_delay()
	{
		if(counter <= 0f)
		{
			shoot_count++;
			Shoot();
			counter = 1f/fire_rate;
		}
		counter -= Time.deltaTime;
	}

	void Shoot()
	{
		// Could use Object pooling here instead 

		GameObject orb_instance = Instantiate(Orb_Instance,spawn_pos.position,spawn_pos.rotation, orb_parent) as GameObject;

		orb_instance.GetComponent<OrbController>().move_speed = orb_speed;
		orb_instance.GetComponent<OrbController>().rising_speed = orb_rising_speed;


		if(orb_accelerate)
		{
			orb_instance.GetComponent<OrbController>().accelerate_ = true;
			orb_instance.GetComponent<OrbController>().accel_amount = accel_amount;
		}

		if(is_homing)
		{
			orb_instance.GetComponent<OrbController>().target_ = player_;
		}
		else
		{
			orb_instance.GetComponent<Rigidbody>().velocity = (orb_instance.transform.forward * orb_speed) - (orb_instance.transform.up * orb_rising_speed);
		}
		// NOTE 
		// If the original orb to clone gets destroyed... no more orbs will exist
		// Could this be a feature???
		// Like if you find the orignal orb thats flying about somewhere and destroy it,
		// 	you stop to enemy weapons from firing...

	}

	public void ResetAxis()
	{
		transform.rotation = defualt_rotation;
		transform.position = defualt_pos;
		rotate_axis = Vector3.zero;

	}


}
