using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Orb_Emitter))]
public class Turret_Editor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		Orb_Emitter my_script = (Orb_Emitter)target;

		if(GUILayout.Button("Reset Axis"))
		{
			my_script.ResetAxis();
		}
	}
}
