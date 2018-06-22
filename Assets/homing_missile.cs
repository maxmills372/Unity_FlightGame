using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class homing_missile : MonoBehaviour {

	public float move_speed = 10.0f;
	public float rotate_speed = 200.0f;
	public GameObject[] targets = new GameObject[10];
	Transform current_target;
	public Transform target;
	Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		// This could be done in update and could also find the closest
		targets = GameObject.FindGameObjectsWithTag("Player");
		//target = GameObject.FindGameObjectWithTag("Player").transform;


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

		FindTarget();


		Vector3 direction = target.position - rb.position;

		direction.Normalize();

		Vector3 rotate_amount = Vector3.Cross(direction,transform.up);

		rb.angularVelocity = -rotate_amount * rotate_speed * Time.deltaTime;//,0.0f,-rotate_amount_z * rotate_speed);

		rb.velocity = transform.up * move_speed;
	}
}
