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

	// Pathfinding variables
	Ray forward_ray,left_ray, right_ray;
	float turn_angle = 0.5f;
	Vector3 offset;
	Vector3 ray_direction;
	public float ray_dist = 20f;
	public float ray_offset = 2.5f;
	public float rotatation_damp = .5f;
	public float turn_speed = 20f;
	public string target_tag = "Player";

	Vector3 direction;
	Vector3 rotate_amount;

	[System.Serializable]
	public enum RocketType
	{
		Straight,
		Target,
		Target_Pathfind,
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
			
			//SetTarget();
			// Set target first!!!
			TrackTarget();
			break;
		case RocketType.Target_Pathfind:
			
			// Uses target passed in for now
			// target = ...

			Pathfinding();
			//PathfindToTarget();
			//TrackTarget();

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
		direction = target.position - rb.position + offset;

		direction.Normalize();

		rotate_amount = Vector3.Cross(direction,transform.forward);

		rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;

		rb.velocity = transform.forward * move_speed;
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
		{
			print("HIT: " + col.name);

			//ExplodeFromHere();
			GameObject Explode_Instance = Instantiate(explodePS,transform.position,Quaternion.identity) as GameObject;
			Destroy(Explode_Instance,2f);
			Destroy(this.gameObject);
		}
	}

	public void Pathfinding()
	{
		
		Vector3 left, right, up, down;
		Vector3 rotation_offset = Vector3.zero;

		left = transform.position - transform.right * ray_offset;
		right = transform.position + transform.right * ray_offset;
		up = transform.position + transform.up * ray_offset;
		down = transform.position - transform.up * ray_offset;

		/*Debug.DrawRay(left ,transform.forward * ray_dist, Color.red);
		Debug.DrawRay(right ,transform.forward * ray_dist,Color.blue);
		Debug.DrawRay(up,transform.forward * ray_dist,Color.green);
		Debug.DrawRay(down,transform.forward * ray_dist,Color.green);
*/
		RaycastHit hit;
		if(Physics.Raycast(left, transform.forward, out hit, ray_dist))
		{
			if(hit.collider.tag != target_tag)
				rotation_offset += Vector3.up;
		}
		else if(Physics.Raycast(right, transform.forward, out hit, ray_dist))
		{
			if(hit.collider.tag != target_tag)
				rotation_offset -= Vector3.up;
		}

		if(Physics.Raycast(up, transform.forward, out hit, ray_dist))
		{
			if(hit.collider.tag != target_tag)
				rotation_offset += Vector3.right;
		}
		else if(Physics.Raycast(down, transform.forward, out hit, ray_dist))
		{
			if(hit.collider.tag != target_tag)				
				rotation_offset -= Vector3.right;
		}


		if(rotation_offset != Vector3.zero)
		{
			transform.Rotate(rotation_offset * turn_speed * Time.deltaTime);
			rb.velocity = transform.forward * move_speed;
		}
		else
		{/*
			direction = (target.position - rb.position);
			Quaternion rot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotatation_damp * Time.deltaTime);
*/
			rb.velocity = transform.forward * move_speed;
		
			direction = target.position - rb.position;

			direction.Normalize();

			rotate_amount = Vector3.Cross(direction,transform.forward);

			rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;

		}
	}

	public void PathfindToTarget()
	{
		


		rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;

		rb.velocity = transform.forward * move_speed;

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
