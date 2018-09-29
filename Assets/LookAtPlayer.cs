using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {
	public Transform player_;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target_pos = player_.position;
		target_pos = new Vector3(0f,0f ,player_.position.z); 

		transform.LookAt( player_,transform.up);
	}
}
