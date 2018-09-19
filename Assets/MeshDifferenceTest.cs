using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDifferenceTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		Mesh m = col.GetComponent<MeshFilter>().mesh;
		Collider[] c = new Collider[1];
		for (int i = 0; i < m.vertices.Length; i++) 
		{
			
		}

	}		

}

