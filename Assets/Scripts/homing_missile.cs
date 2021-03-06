﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class homing_missile : MonoBehaviour {
	
	[System.Serializable]
	public enum RocketType
	{
		Straight,
		Target,
		Target_Pathfind,
		Closest_Pathfind,
		Closest,
		Null
	};
	public RocketType CurrentRocketType;

	[Header("Attributes")]

	public float move_speed = 60.0f;
	public float rotate_speed = 360.0f;
	public float explosion_radius = 100f;
	public float explosion_force = 100f;
	public float no_target_explode_time = 5f;

	public GameObject explodePS;
	public Transform target;
	public GameObject[] targets = new GameObject[10];


	[Header("Pathfinding")]

	public float ray_dist = 20f;
	public float ray_offset = 2.5f;
	public float rotatation_damp = .5f;
	public float turn_speed = 20f;
	public string target_tag = "Lockon";
	public bool draw_debug_lines = false;

	public Vector3 ray_offset_angle_X = Vector3.zero;
	public Vector3 ray_offset_angle_Y = Vector3.zero;

	Vector3 original_target_pos; 

	Transform current_target;
	Rigidbody rb;
	homing_missile Homing_Missile_Script;
	MeshRenderer mesh_renderer;
	Vector3 direction;
	Vector3 rotate_amount;
	float timer;

	Vector3 left, right, up, down,forward;
	Vector3 rotation_offset;
	float turn_angle = 0.5f;
	Vector3 offset;
	Vector3 ray_direction;
	float t, duration;

	public bool test_boom;
	bool not_hit = true;

	// Use this for initialization
	void Start () 
	{
		// This could be done in update and could also find the closest
		targets = GameObject.FindGameObjectsWithTag(target_tag);

		// Get compenents
		rb = GetComponent<Rigidbody>();
		Homing_Missile_Script = GetComponent<homing_missile>();
		mesh_renderer = GetComponent<MeshRenderer>();

		if(target != null)
			original_target_pos = target.position;

	}
		
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.T) && test_boom)
		{
			ExplodeForceFromHere();
		}

		switch (CurrentRocketType)
		{
		case RocketType.Closest:
			// Finds the closest target
			FindTarget();

			// Rotates towards target
			TrackTarget();
			break;

		case RocketType.Target:			

			// Remember to set target first!
			// Rotates towards target
			TrackTarget();
			break;

		case RocketType.Target_Pathfind:
			
			// Remember to set target first!
			// Navigates to target
			Pathfinding();

			break;

		case RocketType.Closest_Pathfind:
			
			// Finds closest target
			FindTarget();

			// Navigates to target
			Pathfinding();

			break;
		case RocketType.Straight:
			
			// Move in straight line, exploding if nothing in front of it
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
		//transform.Translate(transform.forward * move_speed * Time.deltaTime);	
		//rb.MovePosition(rb.position + transform.forward * Time.deltaTime * move_speed);
		rb.velocity = transform.forward * move_speed;

		rb.angularVelocity = Vector3.zero;

		// Create forward ray
		forward = transform.position + transform.forward;

		// If nothing is hit ahead of rocket, explode after 5 secs
		RaycastHit hit;
		if(!Physics.Raycast(forward, transform.forward, out hit, 1000f))
		{
			StartCoroutine("ExplodeAfterTime", 5f);
		}
		else
		{
			StopCoroutine("ExplodeAfterTime");
		}

	}
	void TrackTarget()
	{
		target.position = original_target_pos + new Vector3(5f,0f,0f);
		direction = target.position - rb.position;

		direction.Normalize();

		rotate_amount = Vector3.Cross(direction,transform.forward);

		rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;

		rb.velocity = transform.forward * move_speed;
	}

	void OnTriggerEnter(Collider col)
	{
		// If not hitting an object with this Layer
		if(col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
		{
			//print("HIT: " + col.name);

			// Voronoi break effect
			if(col.GetComponent<Voronoi_Test>())
				col.GetComponent<Voronoi_Test>().m_AllOfYa = true;

			// #Boom
			Explode();
		}
	}
	public void Explode()
	{
		// Create explosion force 
		ExplodeForceFromHere();

		// Instantiate explode effect
		GameObject Explode_Instance = Instantiate(explodePS,transform.position,Quaternion.identity) as GameObject;
		Destroy(Explode_Instance,1.5f);

		// Destroy this after 2 secs (after smoke effect stored as a child has faded away)
		Destroy(this.gameObject,2f);

		// Hide + remove components 
		mesh_renderer.enabled = false;

		// Removed so object stops moving 
		Destroy(Homing_Missile_Script);
		Destroy(rb);
	}

	public void Pathfinding_Vol2()
	{
		// TODO
		// Use box colldiders instead of rays
		// Also if both left and right / up and down sides are hit,
		// 		last resort action? - swerve left or right
	}

	public void Pathfinding()
	{
		rotation_offset = Vector3.zero;

		// Directional ray positions
		left = transform.position - transform.right * ray_offset;
		right = transform.position + transform.right * ray_offset;
		up = transform.position + transform.up * ray_offset;
		down = transform.position - transform.up * ray_offset;

		forward = transform.position + transform.forward;

		// If LEFT or RIGHT side raycasts are hit by an object
		RaycastHit hit;
	
		if(Physics.Raycast(left, transform.forward - ray_offset_angle_X, out hit, ray_dist))
		{
			// Move in opposite direction if object is not the target
			if(hit.collider.gameObject != target.gameObject)
				rotation_offset += Vector3.up;

			/*if(Physics.Raycast(right, transform.forward + ray_offset_angle_X, out hit, ray_dist))	
			{
				// Expand search
				ray_offset_angle_X.x += 0.5f;
			}*/
		}
		else if(Physics.Raycast(right, transform.forward + ray_offset_angle_X, out hit, ray_dist))
		{
			// Move in opposite direction if object is not the target
			if(hit.collider.gameObject != target.gameObject)				
				rotation_offset -= Vector3.up;
		}

		// If UP or DOWN side raycasts are hit by an object
		if(Physics.Raycast(up, transform.forward + ray_offset_angle_Y, out hit, ray_dist))
		{
			// Move in opposite direction if object is not the target
			if(hit.collider.gameObject != target.gameObject)				
				rotation_offset += Vector3.right;

			// Check 
			/*if(Physics.Raycast(down, transform.forward - ray_offset_angle_Y, out hit, ray_dist))	
			{
				// Expand search
				if(ray_offset_angle_Y.y < 1f)
					ray_offset_angle_Y.y += 0.5f;

			}*/

		}
		else if(Physics.Raycast(down, transform.forward - ray_offset_angle_Y, out hit, ray_dist))
		{
			// Move in opposite direction if object is not the target
			if(hit.collider.gameObject != target.gameObject)				
				rotation_offset -= Vector3.right;
		}

		// If movement offset has been set
		if(rotation_offset != Vector3.zero)
		{
			// Apply that rotation to avoid obstacles
			transform.Rotate(rotation_offset * turn_speed * Time.deltaTime);

		}
		else
		{
			/*
			direction = (target.position - rb.position);
			Quaternion rot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotatation_damp * Time.deltaTime);
			*/



			// Rotate towards target
			if(target != null)
			{
				direction = target.position - rb.position;

				direction.Normalize();

				rotate_amount = Vector3.Cross(direction,transform.forward);

				t += Time.deltaTime;
				rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;
				//rb.angularVelocity  = Vector3.Lerp(rb.angularVelocity, -rotate_amount * rotate_speed,  t);


			}

			// Reset ray angle offset
			ray_offset_angle_X = Vector3.zero;
			ray_offset_angle_Y = Vector3.zero;


		}

		// Move towards target
		if(not_hit)
			rb.velocity = transform.forward * move_speed;

		// If no target, explode after time
		if(target == null)
			StartCoroutine("ExplodeAfterTime", no_target_explode_time);
		else
			StopCoroutine("ExplodeAfterTime");


		if(draw_debug_lines)
		{
			Debug.DrawRay(left ,(transform.forward - ray_offset_angle_X) * ray_dist , Color.red);
			Debug.DrawRay(right ,(transform.forward + ray_offset_angle_X)  * ray_dist,Color.red);
			Debug.DrawRay(up,(transform.forward  + ray_offset_angle_Y)  * ray_dist,Color.green);
			Debug.DrawRay(down,(transform.forward  - ray_offset_angle_Y)  * ray_dist,Color.green);
			Debug.DrawRay(forward,transform.forward * ray_dist,Color.cyan);
		}
	}

	public void ExplodeForceFromHere()
	{
		Vector3 explosion_pos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosion_pos, explosion_radius);
		Rigidbody hit_rb;

		foreach(Collider hit in colliders)
		{
			hit_rb = hit.GetComponent<Rigidbody>();
			if (hit_rb != null)
				hit_rb.AddExplosionForce(explosion_force,explosion_pos,explosion_radius,3f);
			
			if(hit.gameObject.layer == 9)
				hit.SendMessage("ExplosionHit");
		}
	
	}
	void ExplosionHit()
	{
		StartCoroutine("Test");
	}

	IEnumerator Test()
	{
		
		not_hit = false;
		gameObject.transform.LookAt(target);

		yield return new WaitForSeconds(1f);
		not_hit = true;
		yield return null;
	}
	// Explodes after time (with no target) has been reached
	IEnumerator ExplodeAfterTime(float time)
	{
		yield return new WaitForSeconds(time);

		Explode();			
		yield return null;


	}
}
