using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpliceTest : MonoBehaviour {

	Mesh this_mesh;
	List<Plane> planes = new List<Plane>();
	public Vector3 point1,
            point2,
            point3,
	point4;
	List<Vector3> verts = new List<Vector3>();


	// Use this for initialization
	void Start () {
		this_mesh = GetComponent<MeshFilter>().mesh;
		this_mesh.GetVertices(verts);
	}
	
	// Update is called once per frame
	void Update () 
	{
		print(this_mesh.vertexCount);
		if(Input.GetKeyDown(KeyCode.A))
		{
			verts.Add(transform.position);
		}
	
		for (int i = 0; i < this_mesh.vertexCount; i++) 
		{
			Debug.DrawRay(this_mesh.vertices[i],Vector3.up, Color.red,1000f);


		}

		this_mesh.SetVertices(verts);

	}


	void OnCollisionEnter(Collision collision)
	{
		/*for (int i = 0; i < this_mesh.vertexCount; i++) 
		{
			Debug.DrawRay(collision.collider.ClosestPoint(this_mesh.vertices[i]),Vector3.up, Color.red,1000f);
				
		}
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, Vector3.up, Color.red,1000f);
			print("Count");
		}*/

	}
}



