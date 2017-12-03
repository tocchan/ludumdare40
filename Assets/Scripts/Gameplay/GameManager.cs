using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public enum eGameState
{
	WAIT_FOR_READY,
	IN_GAME,
	GAME_OVER,
}


//-------------------------------------------------------------------------------------------------
public class GameManager : MonoSingleton<GameManager>
{
	//-------------------------------------------------------------------------------------------------
	// Static Members
	//-------------------------------------------------------------------------------------------------
	public static string TAG_PREY = "Prey";
	public static string TAG_PREDATOR = "Predator";
	public static string ANIM_RABBIT_HOP = "anim_rabbit_hop";
	public static string ANIM_WOLF_IDLE = "anim_wolf_idle";
	public static string ANIM_WOLF_SNIFF = "anim_wolf_sniff";
	public static string ANIM_WOLF_BITE = "anim_wolf_bite";
	public static float GAME_Z = -1.0f;


	//-------------------------------------------------------------------------------------------------
	// Members
	//-------------------------------------------------------------------------------------------------
	[Header("Settings")]
	public int m_babySpawnTotal = 4;
	public float m_gameDuration = 60.0f * 5.0f; //5 minutes


	//-------------------------------------------------------------------------------------------------
	[Header("Prefabs")]
	public GameObject m_preyPrefab;
	public GameObject m_predatorPrefab;


	//-------------------------------------------------------------------------------------------------
	private float m_gameTimer = 0.0f;
	private eGameState m_currentState = eGameState.WAIT_FOR_READY;
	private bool m_preyWins = false;


	//-------------------------------------------------------------------------------------------------
	// Static Functions
	//-------------------------------------------------------------------------------------------------
	public int GetHumanPreyCount()
	{
		int count = 0;
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);
		for (int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController prey = preys[preyIndex].GetComponent<PreyController>();
			if(prey != null)
			{
				if(prey.IsPlayer())
				{
					count += 1;
				}
			}
		}
		return count;
	}


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


	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		HopperNetwork.Instance.OnPlayerJoin += AddPlayer;
		HopperNetwork.Instance.OnPlayerLeave += RemovePlayer;
		EnterState(m_currentState);
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateState(m_currentState);
	}


	//-------------------------------------------------------------------------------------------------
	private void OnApplicationQuit()
	{
		HopperNetwork.Instance.OnPlayerJoin -= AddPlayer;
		HopperNetwork.Instance.OnPlayerLeave -= RemovePlayer;
	}


	//-------------------------------------------------------------------------------------------------
	// Functions
	//-------------------------------------------------------------------------------------------------
	private void EnterState(eGameState state)
	{
		if(state == eGameState.IN_GAME)
		{
			m_gameTimer = 0.0f;
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateState(eGameState state)
	{
		if(state == eGameState.WAIT_FOR_READY)
		{
			bool isReady = HopperNetwork.IsEveryoneReady();
			bool isEnoughPlayers = HopperNetwork.GetConnectionCount() >= 3;
			if(isReady && isEnoughPlayers)
			{
				HopperNetwork.StartGame();
				//Set Wolf
				ChangeState(eGameState.IN_GAME);
			}
		}

		if(state == eGameState.IN_GAME)
		{
			m_gameTimer += Time.deltaTime;
			int preyCount = GetHumanPreyCount();

			//No players left wolf wins
			if(preyCount == 0)
			{
				m_preyWins = true;
				ChangeState(eGameState.GAME_OVER);
			}

			//Times up, bunnies win
			if(m_gameTimer >= m_gameDuration)
			{
				m_preyWins = false;
				ChangeState(eGameState.GAME_OVER);
			}
		}

		if(state == eGameState.GAME_OVER)
		{
			HopperNetwork.EndGame();
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void ExitState(eGameState state)
	{

	}


	//-------------------------------------------------------------------------------------------------
	private void ChangeState(eGameState state)
	{
		ExitState(m_currentState);
		m_currentState = state;
		EnterState(m_currentState);
	}


	//-------------------------------------------------------------------------------------------------
	private void AddPlayer(VirtualNetworkController controller)
	{
		Vector3 spawnLocation = Vector3.zero;
		spawnLocation.z = GAME_Z;
		GameObject preyObject = Instantiate(m_preyPrefab, spawnLocation, Quaternion.identity);
		PreyController prey = preyObject.GetComponent<PreyController>();
		prey.m_netController = controller;
	}


	//-------------------------------------------------------------------------------------------------
	private void RemovePlayer(VirtualNetworkController controller)
	{
		//Find controller if it was a prey
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);
		for(int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController prey = preys[preyIndex].GetComponent<PreyController>();
			if(prey != null)
			{
				if(prey.m_netController == controller)
				{
					prey.Eaten();
					return;
				}
			}
		}

		//Find controller if it was a predator
		GameObject[] predators = GameObject.FindGameObjectsWithTag(TAG_PREDATOR);
		for (int predatorIndex = 0; predatorIndex < predators.Length; ++predatorIndex)
		{
			PredatorController predator = predators[predatorIndex].GetComponent<PredatorController>();
			if (predator != null)
			{
				if (predator.m_netController == controller)
				{
					Destroy(predator);
					return;
				}
			}
		}
	}
}
