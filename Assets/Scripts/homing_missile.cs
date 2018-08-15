using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class homing_missile : MonoBehaviour {

	public float move_speed = 10.0f;
	public float rotate_speed = 200.0f;
	public float explosion_radius = 100f;
	public float explosion_force = 100f;
	public GameObject explodePS;

	public GameObject[] targets = new GameObject[10];
	Transform current_target;
	public Transform target;
	Rigidbody rb;

	[System.Serializable]
	public enum RocketType
	{
		Straight,
		Target,
		Closest,
		Null
	};
	public RocketType CurrentRocketType;

	// Use this for initialization
	void Start () 
	{
		// This could be done in update and could also find the closest
		targets = GameObject.FindGameObjectsWithTag("Player");

		rb = GetComponent<Rigidbody>();
	}

	void FindTarget()
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

		target = current_target;
	}

	// Update is called once per frame
	void Update () 
	{
		switch (CurrentRocketType)
		{

		case RocketType.Closest:
			FindTarget();
			TrackTarget();
			break;

		case RocketType.Target:
			
			// Set target first!!!
			TrackTarget();
			break;

		case RocketType.Straight:
			MoveForward();
			break;

		case RocketType.Null:
			print("Rocket type not set");
			break;
		default: 
			break;

		}
	}

	public void SetTarget(Vector3 target_)
	{
		target.position = target_;
	}
	void MoveForward()
	{
		transform.Translate(transform.forward * move_speed * Time.deltaTime);	
		//rb.MovePosition(rb.position + transform.up * Time.deltaTime * move_speed);

		//rb.angularVelocity = Vector3.up * rotate_speed * Time.deltaTime;
	}
	void TrackTarget()
	{
		Vector3 direction = target.position - rb.position;

		direction.Normalize();

		Vector3 rotate_amount = Vector3.Cross(direction,transform.up);

		rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;//,0.0f,-rotate_amount_z * rotate_speed);

		rb.velocity = transform.up * move_speed;
	}

	void OnTriggerEnter(Collider col)
	{
		ExplodeFromHere();
		GameObject Explode_Instance = Instantiate(explodePS,transform.position,Quaternion.identity) as GameObject;
		Destroy(Explode_Instance,2f);
		Destroy(this.gameObject);
	}

	public void ExplodeFromHere()
	{
		Vector3 explosion_pos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosion_pos, explosion_radius);
		Rigidbody hit_rb;

		foreach(Collider hit in colliders)
		{
			hit_rb = hit.GetComponent<Rigidbody>();
			if (hit_rb != null)
				hit_rb.AddExplosionForce(explosion_force,explosion_pos,explosion_radius,3f);
		}
	
	
	}
}
