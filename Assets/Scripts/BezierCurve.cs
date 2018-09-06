using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {

	public GameObject Line_clone;
	public GameObject start_point_clone;// point0,point3;
	public GameObject control_point_clone;// point1,point2;
	int num_positions = 50;
	//public Vector3[] positions = new Vector3[50];
	[System.Serializable]
	public struct bezier_curve_instance
	{
		public GameObject start_point;
		public GameObject control_point;
		public GameObject control_point_2;
		public Vector3[] positions;
		public GameObject Line_;
		//public GameObject parent;


	}

	public List<bezier_curve_instance> Bezier_Curves = new List<bezier_curve_instance>();


	public int current_point = 0;
	public int current_pos = 0;

	public GameObject camera_object;
	float t_,rotation_time = 0f;
	public bool create_start_points = false;
	public float move_speed = 1f;
	public float rotate_speed = 2f;
	Vector3 old_pos;
	int current_start = 0, current_control = 0;

	// Use this for initialization
	void Start () 
	{
		//Line_ .positionCount = num_positions;
		bezier_curve_instance line_instance;
		line_instance.start_point = start_point_clone;
		line_instance.control_point = control_point_clone;
		line_instance.control_point_2 = control_point_clone;
		line_instance.Line_ = Line_clone;
		line_instance.positions = new Vector3[50];

		Init_Curve(line_instance,start_point_clone.transform.position,start_point_clone.transform.rotation);
		Bezier_Curves.Add(line_instance);

	}
	
	// Update is called once per frame
	void Update () 
	{
		int count = 0;
		for(int i = 0;i<Bezier_Curves.Count;i++)
		{
			//CreateBezier(Line_,0,0);
			if(count == 0)
			{		
				CreateBezier(Bezier_Curves[i].Line_,0,0,Bezier_Curves[i]);
				count++;
			}
			else
				CreateBezier(Bezier_Curves[i].Line_,i,i+1,Bezier_Curves[i]);
		}

		CreateStartPoints();


		// Camera lerping test
		t_ = Time.deltaTime * move_speed;


		rotation_time = Time.deltaTime * rotate_speed;
		/*
		if(Vector3.Distance(camera_object.transform.position, start_points[current_point].position) <= 5f)
		{			
			if(current_point != start_points.Count -1)
			{
				current_point ++;
			}
		}
		if(Vector3.Distance(camera_object.transform.position, positions[current_pos]) <= 1f)
		{
			if(current_pos != positions.Length -1)
			{
				current_pos ++;
			}
		}
		camera_object.transform.position = Vector3.MoveTowards(camera_object.transform.position, positions[current_pos],t_);

		camera_object.transform.rotation = Quaternion.Slerp(camera_object.transform.rotation, start_points[current_point].rotation, rotation_time);
		*/

	}

	public void Init_Curve(bezier_curve_instance instance, Vector3 cam_pos,Quaternion cam_rot)
	{
		instance.start_point  = Instantiate(start_point_clone,cam_pos,cam_rot) as GameObject;
		instance.control_point  = Instantiate(control_point_clone,cam_pos + (Vector3.right * 15f),cam_rot);
		instance.control_point_2  = Instantiate(control_point_clone,cam_pos+ (Vector3.right * -15f),cam_rot);
		instance.Line_  = Instantiate(Line_clone,cam_pos,cam_rot);
		instance.positions = new Vector3[50];
	}
	void CreateStartPoints()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Vector3 cam_pos = camera_object.transform.position;
			Quaternion cam_rot = camera_object.transform.rotation;
			/*
			GameObject start_point_instance  = Instantiate(start_points[0].gameObject,cam_pos,cam_rot,transform.parent.transform);
			GameObject control_point_instance  = Instantiate(control_points[0].gameObject,cam_pos + (Vector3.right * 15f),cam_rot,transform.parent.transform);
			GameObject control_point_instance2  = Instantiate(control_points[0].gameObject,cam_pos+ (Vector3.right * -15f),cam_rot,transform.parent.transform);
			GameObject line_instance  = Instantiate(Line_.gameObject,cam_pos,cam_rot,transform.parent.transform);
			Vector3[] new_positions = new Vector3[num_positions];
*/
			bezier_curve_instance instance;
			//instance.Init(cam_pos,cam_rot);
			//Bezier_Curves.Add(instance);


			current_start ++;
			current_control += 2;
		}
	}
	void CreateBezier(GameObject line,int current_start,int current_control,bezier_curve_instance instance )
	{
		for(int i = 0;i<num_positions;i++)
		{
			float t_ = (float)i/num_positions;
			instance.positions[i] = CubicBezier(t_,current_start,current_control);//,point0.position,point1.position,point2.position,point3.position);
		}

		line.GetComponent<LineRenderer>().SetPositions(instance.positions);
	}

	Vector3 LinearBezier(float t,Vector3 p0,Vector3 p1 )
	{
		return p0 + (t*(p1 - p0));
	}
	Vector3 QuadraticBezier(float t,Vector3 p0,Vector3 p1,Vector3 p2 )
	{
		Vector3 num = (1-t) *(1-t) * p0;
		num += 2*(1-t) *t * p1;
		num += (t*t)* p2;
		return num;
	}

	Vector3 CubicBezier(float t,int current_start,int current_control)//, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 num = ((1-t) *(1-t) * (1-t)) * Bezier_Curves[current_start].start_point.transform.position;
		num += (3*(1-t)*(1-t)) *t * Bezier_Curves[current_control].control_point.transform.position;
		num += (3*(1-t)) * (t*t) * Bezier_Curves[current_control].control_point_2.transform.position;
		num += (t*t*t) * Bezier_Curves[current_start+1].start_point.transform.position;

		return num;

	}
}
