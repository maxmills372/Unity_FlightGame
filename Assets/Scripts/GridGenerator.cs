using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{

    // Prefab to spawn
    [SerializeField]
    private GameObject m_PrefabToSpawn          = null;

    // Grid size
    [SerializeField]
    private Vector3     m_GridSize              = Vector3.zero;

    // Space between objects
    [SerializeField]
    private float       m_SpaceBetweenObjects   = 3;

	public bool map_generated_ = false;
	public bool do_action = false;

	[HideInInspector]
	public List<GameObject> cubes_;
    public Transform m_ParentObject;

    int counter = 0;

    // Use this for initialization
    void Start()
    {
        // Generate the grid
        GenerateGrid();
    }

	void Update()
	{
		if(map_generated_ && do_action)
		{
			foreach(GameObject g in cubes_)
			{
				g.GetComponent<Rigidbody>().AddExplosionForce(-500f,cubes_[446].transform.position,1000f);

				//g.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0.5f,25.0f),Random.Range(0.5f,25.0f),Random.Range(0.25f,2.0f));
				//g.transform.position -= new Vector3(Random.Range(0.5f,5.0f),Random.Range(0.5f,5.0f),Random.Range(0.5f,2.0f));
				//g.SendMessage("CreateGrid");
			}	

			map_generated_ = false;
		}
	}

    /// Generate a grid of prefab instances
    void GenerateGrid()
    {
        if (m_PrefabToSpawn != null)
        {
            for (int i = 0; i < m_GridSize.x; i++)
            {
                for (int j = 0; j < m_GridSize.y; j++)
                {
                    for (int k = 0; k < m_GridSize.z; k++)
                    {
						counter++;
                        GameObject obj = GameObject.Instantiate(m_PrefabToSpawn, transform.position + new Vector3(i * m_SpaceBetweenObjects, j * m_SpaceBetweenObjects, k * m_SpaceBetweenObjects), Quaternion.identity,m_ParentObject) as GameObject;
                        
						obj.name = counter.ToString();
						cubes_.Add(obj);
                    }
                }
            }
        }
		map_generated_ = true;
    }   
}
