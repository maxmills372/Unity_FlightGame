using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour {

	public GameObject[] lock_on_objects = new GameObject[1000];
	public List<GameObject> visible_objects;
	Transform current_target, target;

	public GameObject target_sphere;
	public bool map_generated_, stop_ = false; 

	public GameObject map_generator;
	public GameObject player_object;

	// Use this for initialization
	void Start () 
	{
		//lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");			
	}
	
	// Update is called once per frame
	void Update ()
	{
		map_generated_ = map_generator.GetComponent<GridGenerator>().map_generated_;
		if(map_generated_ && !stop_)
		{
			lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");
			stop_ = true;
		}
		foreach(GameObject g in lock_on_objects)
		{
//			if(g.GetComponent<Renderer>().isVisible)
//			{
//				if(!visible_objects.Contains(g))
//				{
//					visible_objects.Add(g);
//				}
//
//			}
//			else
//			{
//				visible_objects.Remove(g);
//			}
		}

		FindTarget();
		target_sphere.transform.position = target.position;
	}

	void FindTarget()
	{
		float current_min = 100000.0f;

		foreach(GameObject g in visible_objects)
		{
			float distance = Vector3.Distance(g.transform.position, player_object.transform.position);
			if(distance < current_min)
			{
				current_min = distance;
				current_target = g.transform;
			}

		}

		target = current_target;
	}

	void OnTriggerEnter(Collider col)
	{
		if(!visible_objects.Contains(col.gameObject))
		{
			visible_objects.Add(col.gameObject);
		}	
	}
	void OnTriggerExit(Collider col)
	{
		if(visible_objects.Contains(col.gameObject))
		{
			visible_objects.Remove(col.gameObject);
		}	
	}
}
