using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pool : MonoBehaviour {

	public int child_count;

	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public int size;

	}

	public static Object_Pool instance;

	public List<Pool> pools;
	public Dictionary<string, Queue<GameObject>> pool_dictionary;


	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		float st_ = Time.realtimeSinceStartup;

		CreatePool();

		float et_ = Time.realtimeSinceStartup;
		print("Finished: " + (et_ - st_));

	}

	public void CreatePool()
	{
		pool_dictionary = new Dictionary<string, Queue<GameObject>>();

		foreach(Pool p in pools)
		{
			Queue<GameObject> object_pool = new Queue<GameObject>();

			for(int i = 0; i < p.size; i++)
			{
				GameObject obj = Instantiate(p.prefab,transform);
				obj.SetActive(false);

				object_pool.Enqueue(obj);	
			}

			pool_dictionary.Add(p.tag, object_pool); 
		}
	}

	public void AddToPool(string tag, int size)
	{		
		for(int i = 0; i < size; i++)
		{
			GameObject obj = Instantiate(pools[0].prefab,transform);
			obj.SetActive(false);

			pool_dictionary[tag].Enqueue(obj);	
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation,Transform parent)
	{

		if(!pool_dictionary.ContainsKey(tag))
		{
			print("Wrong tag");
			return null;

		}

		GameObject object_to_spawn = pool_dictionary[tag].Dequeue();

		if(tag == "Skittle")
		{
			object_to_spawn.GetComponent<Renderer>().material.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
		}
		object_to_spawn.SetActive(true);

		object_to_spawn.transform.position = position;
		object_to_spawn.transform.rotation = rotation;
		//object_to_spawn.transform.parent = parent;

		pool_dictionary[tag].Enqueue(object_to_spawn); 

		return object_to_spawn;
	}

	void Update()
	{
		
		child_count = transform.childCount;
	}


}
