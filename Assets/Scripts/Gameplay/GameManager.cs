using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class GameManager : MonoSingleton<GameManager>
{
	//-------------------------------------------------------------------------------------------------
	public static string TAG_PREY = "Prey";
	public static float GAME_Z = -1.0f;


	//-------------------------------------------------------------------------------------------------
	[Header("Settings")]
	public int m_babySpawnTotal = 4;


	//-------------------------------------------------------------------------------------------------
	public static PreyController GetClosestPrey(PreyController fromPrey)
	{
		return GetClosestPrey(fromPrey.gameObject.transform.position, fromPrey);
	}


	//-------------------------------------------------------------------------------------------------
	public static PreyController GetClosestPrey(Vector2 fromPosition, PreyController ignore = null)
	{
		return GetClosestPrey(fromPosition, -1.0f, ignore);
	}


	//-------------------------------------------------------------------------------------------------
	public static PreyController GetClosestPrey(Vector2 fromPosition, float range, PreyController ignore = null)
	{
		//Setup
		float closestDistance = -1.0f;
		PreyController closestPrey = null;
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);

		//Find the closest one
		for(int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController currentPrey = preys[preyIndex].GetComponent<PreyController>();
			if(currentPrey == null)
			{
				continue;
			}

			Vector2 preyPosition = currentPrey.gameObject.transform.position;
			float preyDistance = Vector2.Distance(preyPosition, fromPosition);

			//Is close
			if(closestDistance == -1.0f
				|| preyDistance < closestDistance)
			{
				//Is not the prey to ignore
				if(currentPrey != ignore)
				{
					//New closest prey
					closestPrey = currentPrey;
					closestDistance = preyDistance;
				}
			}
		}

		//Dont care about range
		if(range == -1.0f)
		{
			return closestPrey;
		}

		//Make sure it's less than range
		if(closestDistance < range)
		{
			return closestPrey;
		}

		//Closest prey not within range
		return null;
	}
}
