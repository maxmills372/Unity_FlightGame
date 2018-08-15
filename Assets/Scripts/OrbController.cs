using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
	public bool follow_line = false;
	public bool move_forward = true;

	public float move_speed = 10f;
	public float rotate_speed = 200f;
	public Vector3 target_pos;
	public bool accelerate_ = false;
	public float max_lifetime = 100f;
	float explode_timer;
	int limit = 0;
	public float accel_amount = 0.2f;
	public float rising_speed = 5f;
	Rigidbody rb;

	public Transform target_;
	public GameObject PS_Explode;

	// Use this for initialization
	void Start () 
	{
		
		rb = GetComponent<Rigidbody>();
	
	}

	public void Orb_Init(float speed)
	{
		speed = move_speed;
	}

	// Update is called once per frame
	void Update () 
	{
		explode_timer += Time.time / Time.renderedFrameCount;
		if(explode_timer > max_lifetime)
		{
			Explode();
		}

		if(target_ != null)
		{
			Vector3 direction = target_.position - rb.position;

			direction.Normalize();

			Vector3 rotate_amount = Vector3.Cross(direction,transform.forward);

			rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;//,0.0f,-rotate_amount_z * rotate_speed);

			rb.velocity = transform.forward * move_speed;

		}
		else
		{			
			//transform.Translate(transform.forward.normalized * move_speed * Time.deltaTime, Space.World);
		}
		if(accelerate_)
		{
			//rb.velocity += accel_amount;

			rb.velocity = (transform.forward * move_speed)  - (transform.up * rising_speed);
			move_speed += accel_amount;
		}



	}

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Player")
		{
			print("HITPLAYER");
		}
		if(col.tag != "Orb")
		{
			Explode();
		}
	}

	void Explode()
	{
		if(PS_Explode != null)
		{	
			GameObject PS_Instance = Instantiate(PS_Explode,transform.position,transform.rotation) as GameObject;
			Destroy(PS_Instance,1.0f);
		}

		move_forward = false;
		follow_line = false;



		Destroy(this.gameObject);

	}
}
