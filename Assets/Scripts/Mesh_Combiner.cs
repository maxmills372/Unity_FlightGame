using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Combiner : MonoBehaviour {

	public bool delete_children;
	public bool combine_;
	public int vertex_count = 0;

	// Use this for initialization
	void Start () {
		//CombineChildMeshes();
	}

	// Update is called once per frame
	void Update () {
		if(combine_)
		{
			CombineChildMeshes();

		}
	}

	public void CombineChildMeshes()
	{
		Quaternion old_rot = transform.rotation;
		Vector3 old_pos = transform.position;
		Vector3 old_scale = transform.localScale;

		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;

		// TODO
		// SOMETHING NOT WORKING HERE - to do with the scale???
		//transform.localScale = Vector3.zero;

		MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
		//BoxCollider[] box_colliders = GetComponentsInChildren<BoxCollider>();

		// Create a list of meshes for when a mesh exceeds 64k vertices limit
		List<Mesh> combined_meshes = new List<Mesh>();
		List<MeshFilter> new_filters = new List<MeshFilter>();
		Mesh mesh_instance = new Mesh();
		MeshFilter filter_instance = new MeshFilter();

		Mesh final_mesh = new Mesh();

		CombineInstance[] combiners = new CombineInstance[filters.Length];


		for (int i = 0; i<filters.Length; i++)
		{				
			if(vertex_count >= 65000)	
			{			
				mesh_instance.name = "MyMesh";
				mesh_instance.CombineMeshes(combiners);

				filter_instance.sharedMesh = mesh_instance;

				combined_meshes.Add(mesh_instance);
				new_filters.Add(filter_instance);

				vertex_count = 0;
			}

			if(filters[i].transform == transform)
				continue;

			combiners[i].subMeshIndex = 0;
			combiners[i].mesh = filters[i].sharedMesh;
			combiners[i].transform = filters[i].transform.localToWorldMatrix;

			vertex_count += combiners[i].mesh.vertexCount;


		}
		/*
		Mesh mesh_instance2 = new Mesh();	
		MeshFilter filter_instance2 = new MeshFilter();

		mesh_instance2.name = "MyMesh";
		mesh_instance2.CombineMeshes(combiners);
		filter_instance2.mesh = mesh_instance2;

		combined_meshes.Add(mesh_instance2);
		new_filters.Add(filter_instance2);

		CombineInstance[] mesh_combiners = new CombineInstance[new_filters.Count];
		for (int i = 0; i < combined_meshes.Count; i++)
		{

			mesh_combiners[i].subMeshIndex = 0;
			mesh_combiners[i].mesh = new_filters[i].mesh;
			mesh_combiners[i].transform = new_filters[i].transform.localToWorldMatrix;

			vertex_count += mesh_combiners[i].mesh.vertexCount;

		}
		*/

		final_mesh.name = "MyFinalMesh";
		final_mesh.CombineMeshes(combiners);

		GetComponent<MeshFilter>().sharedMesh = final_mesh;


		// Components to add:
		//gameObject.AddComponent<MeshRenderer>();

		gameObject.AddComponent<MeshCollider>();
		gameObject.GetComponent<MeshCollider>().convex = true;


		// Colour of region
		GetComponent<Renderer>().material.color = GetComponentInChildren<Renderer>().material.color;



		/*
		// Add all box colldiers from children instead of mesh collider (TEST)
		for (int i = 0;i<box_colliders.Length;i++)
		{
			gameObject.AddComponent<BoxCollider>().center = filters[i].transform.position;//box_colliders[i].center;


		}
		box_colliders = gameObject.GetComponents<BoxCollider>();

		for (int i = 0;i<box_colliders.Length;i++)
		{
			box_colliders[i].size = old_scale;
		}*/

		transform.position = old_pos;
		transform.rotation = old_rot;
		transform.localScale = Vector3.one;


		if(delete_children)
		{
			for (int i = 1;i<filters.Length;i++)
			{
				Destroy(filters[i].transform.gameObject);
			}

		}

		combine_ = false;
	}

}
