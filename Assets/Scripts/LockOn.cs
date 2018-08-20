using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour {

	public GameObject[] lock_on_objects = new GameObject[1000];
	public List<GameObject> visible_objects;
	GameObject current_target, target;
	public List <GameObject> targets = new List<GameObject>();

	public GameObject[] target_spheres;
	public bool map_generated_, stop_ = false; 

	public GameObject map_generator;
	public GameObject player_object;

	public bool is_locking_on = false;

	// Use this for initialization
	void Start () 
	{
		//lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");	
	}
	
	// Update is called once per frame
	void Update ()
	{
		// This finds all lockon items in the game, after the delay of creating them
		map_generated_ = map_generator.GetComponent<GridGenerator>().map_generated_;
		if(map_generated_ && !stop_)
		{
			lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");
			stop_ = true;
		}
	
		if(is_locking_on)
		{
			// Clears target list
			targets.Clear();

			// If there are visible targets
			if(visible_objects.Count != 0)
			{
				// For every target lockon sphere
				for (int i = 0; i < target_spheres.Length; i++) 
				{
					// Find clostest target and add to list
					targets.Add(FindTarget());

					// If target still exists
					if(targets[i] != null) 
					{
						// Enable and track target
						target_spheres[i].SetActive(true);
						target_spheres[i].transform.position = targets[i].transform.position;
					}
					else 
					{
						// Disable target
						target_spheres[i].SetActive(false);
					}
				}

			}
			else
			{		
				// Clears target list
				targets.Clear();
			}
		}

	}

	// Finds the closest target
	GameObject FindTarget()
	{
		float current_min = 100000.0f;

		foreach(GameObject g in visible_objects)
		{
			if(g != null && !targets.Contains(g))
			{
				float distance = Vector3.Distance(g.transform.position, player_object.transform.position);
				if(distance < current_min)
				{
					current_min = distance;
					current_target = g;
				}
			}
		}

		//target = current_target;
		return current_target;
	}

	void OnTriggerEnter(Collider col)
	{
		if(!visible_objects.Contains(col.gameObject) && col.tag == "Lockon")
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
