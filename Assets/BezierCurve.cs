using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {

	public LineRenderer Line_;
	int num_positions = 50;
	public Vector3[] positions = new Vector3[50];

	public Transform point0,point1,point2,point3;

	// Use this for initialization
	void Start () 
	{
		Line_ .positionCount = num_positions;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CreateBezier();
	}

	void CreateBezier()
	{
		for(int i = 0;i<num_positions;i++)
		{
			float t_ = (float)i/num_positions;

			positions[i] = CubicBezier(t_,point0.position,point1.position,point2.position,point3.position);
		}
		Line_.SetPositions(positions);
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

	Vector3 CubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 num = ((1-t) *(1-t) * (1-t)) * p0;
		num += (3*(1-t)*(1-t)) *t * p1;
		num += (3*(1-t)) * (t*t) * p2;
		num += (t*t*t) * p3;

		return num;

	}
}
