using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour 
{
	[Header("Attibutes")]

	//public GameObject[] lock_on_objects = new GameObject[1000];


	public GameObject map_generator;
	public GameObject player_object;
	public GameObject raycasthit_cube;

	public bool use_cursor_lockon_origin = false;
	public GameObject[] target_spheres;

	public float cursor_ray_distance = 1000f;

	[Header("Other Stuff")]
	public List <GameObject> targets = new List<GameObject>();
	public List<GameObject> visible_objects;
	public bool map_generated_, stop_ = false; 
	public bool is_locking_on = false;

	Ray ray;
	GameObject current_target, target;
	GameObject closest_target_origin_point;

	// Use this for initialization
	void Start () 
	{
		//lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// This finds all lockon items in the game, after the delay of creating them
		/*map_generated_ = map_generator.GetComponent<GridGenerator>().map_generated_;
		if(map_generated_ && !stop_)
		{
			lock_on_objects = GameObject.FindGameObjectsWithTag("Lockon");
			stop_ = true;
		}*/

		if(use_cursor_lockon_origin)
		{
			// Creates forward shooting ray
			ray = new Ray(player_object.transform.position + player_object.transform.forward, player_object.transform.forward);

			Debug.DrawRay(ray.origin,ray.direction * 100f,Color.white);

			// If Raycast hits anything within distance 
			RaycastHit hit;
			if(Physics.Raycast(player_object.transform.position,player_object.transform.forward,out hit, cursor_ray_distance))
			{
				// Set tracing cube to the hit position
				raycasthit_cube.transform.position = hit.point;

			}
			// Use the cube as the origin point for the closest to calculation 
			closest_target_origin_point = raycasthit_cube;
		}
		else
		{
			// Otherwise use the player position as the origin
			closest_target_origin_point = player_object;
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
		// Default min
		float current_min = 100000.0f;

		// Forevery visible object
		foreach(GameObject g in visible_objects)
		{
			// If Gameobeject exists still and not already a target (DOES THIS WORK??)
			if(g != null && !targets.Contains(g))
			{
				// Set new closest target if closer than current closest
				float distance = Vector3.Distance(g.transform.position, closest_target_origin_point.transform.position);
				if(distance < current_min)
				{
					current_min = distance;
					current_target = g;
				}
			}
		}

		// Returns the closest target
		return current_target;
	}

	void OnTriggerEnter(Collider col)
	{
		// If Colliding and already in list and has correct tag
		if(!visible_objects.Contains(col.gameObject) && col.tag == "Lockon")
		{
			// Add to list of visible objects
			visible_objects.Add(col.gameObject);
		}	
	}
	void OnTriggerExit(Collider col)
	{
		// If not within collider and is in list
		if(visible_objects.Contains(col.gameObject))
		{
			// Remove from list
			visible_objects.Remove(col.gameObject);
		}	
	}
}
