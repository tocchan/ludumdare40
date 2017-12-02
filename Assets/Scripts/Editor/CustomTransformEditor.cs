using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//---------------------------------------------------------------------------------------------------
[CustomEditor(typeof(Transform))]
public class CustomTransformEditor : Editor
{
	private bool scaleLocked = false;
	private Vector3 scaleRatio;
	private Vector3 previousScale;
	Texture locked;


	//---------------------------------------------------------------------------------------------------
	public override void OnInspectorGUI()
	{
		Transform myTransform = (Transform)target;
		GUILayoutOption[] buttonOptions = { GUILayout.Width(20.0f) };

		//Position
		EditorGUILayout.BeginHorizontal();
		Vector3 position = EditorGUILayout.Vector3Field("Position", myTransform.localPosition);
		if (GUILayout.Button("0", buttonOptions))
		{
			Undo.RegisterCompleteObjectUndo(myTransform, "Reset Position " + target.name);
			position = Vector3.zero;
		}
		EditorGUILayout.EndHorizontal();

		//Rotation
		EditorGUILayout.BeginHorizontal();
		Vector3 rotation = EditorGUILayout.Vector3Field("Rotation", myTransform.localEulerAngles);
		if (GUILayout.Button("0", buttonOptions))
		{
			Undo.RegisterCompleteObjectUndo(myTransform, "Reset Rotation " + target.name);
			rotation = Vector3.zero;
		}
		EditorGUILayout.EndHorizontal();

		//Scale
		EditorGUILayout.BeginHorizontal();
		Vector3 scale = EditorGUILayout.Vector3Field("Scale", myTransform.localScale);
		if(scaleLocked)
		{
			if (GUILayout.Button("L", buttonOptions))
			{
				scaleLocked = false;
			}
		}
		else
		{
			if (GUILayout.Button("U", buttonOptions))
			{
				scaleLocked = true;
				scaleRatio = scale;
			}
		}
		EditorGUILayout.EndHorizontal();

		if (GUI.changed)
		{
			Undo.RegisterCompleteObjectUndo(myTransform, "Transform Change " + target.name);
			myTransform.localPosition = FixIfNaN(position);
			myTransform.localEulerAngles = FixIfNaN(rotation);
			if(scaleLocked)
			{
				scale = MatchRatio(scale);
			}
			myTransform.localScale = FixIfNaN(scale);
			previousScale = myTransform.localScale;
		}
	}


	//---------------------------------------------------------------------------------------------------
	private Vector3 FixIfNaN(Vector3 value)
	{
		if (value.x == float.NaN)
		{
			value.x = 0;
		}
		if (value.y == float.NaN)
		{
			value.y = 0;
		}
		if (value.z == float.NaN)
		{
			value.z = 0;
		}
		return value;
	}


	//---------------------------------------------------------------------------------------------------
	private Vector3 MatchRatio(Vector3 scale)
	{
		if(previousScale.x != scale.x)
		{
			if(scaleRatio.x == 0.0f)
			{
				scaleLocked = false;
			}
			else
			{
				float change = scale.x / scaleRatio.x;
				scale = scaleRatio * change;
			}
		}
		else if(previousScale.y != scale.y)
		{
			if (scaleRatio.y == 0.0f)
			{
				scaleLocked = false;
			}
			else
			{
				float change = scale.y / scaleRatio.y;
				scale = scaleRatio * change;
			}
		}
		else if(previousScale.z != scale.z)
		{
			if(scaleRatio.z == 0.0f)
			{
				scaleLocked = false;
			}
			else
			{
				float change = scale.z / scaleRatio.z;
				scale = scaleRatio * change;
			}
		}

		return scale;
	}
}
