﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
	public static string TAG_BONEPILE = "Bones";
	public static string LAYER_GHOST = "Ghost";
	public static string LAYER_LIVING = "Living";
	public static string ANIM_RABBIT_HOP = "anim_rabbit_hop";
	public static string ANIM_WOLF_IDLE = "anim_wolf_idle";
	public static string ANIM_WOLF_WALK = "anim_wolf_walk";
	public static string ANIM_WOLF_BITE = "anim_wolf_bite";
	public static float GAME_Z = -1.0f;


	//-------------------------------------------------------------------------------------------------
	// Members
	//-------------------------------------------------------------------------------------------------
	[Header("Settings")]
	public int m_babySpawnTotal = 4;
	public float m_gameDurationAdditionalPerRabbit = 20.0f;
	public float m_gameDurationBase = 0.0f;
	private float m_gameDuration = 60.0f;
	public float m_gameOverDuration = 10.0f;
	public float m_wolfDelayOnStart = 3.0f;


	//-------------------------------------------------------------------------------------------------
	[Header("Prefabs")]
	public GameObject m_preyPrefab;
	public GameObject m_predatorPrefab;
	public GameObject m_bonePilePrefab;
	public GameObject m_explosionPrefab;


	//-------------------------------------------------------------------------------------------------
	[Header("Tutorals")]
	public GameObject m_UIJoin; 
	public GameObject m_UIPlayersRequired;
	public GameObject m_UIReadyUp;
	public GameObject m_UIInstructions;
	private bool m_firstGame = true; 


	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public Text m_gameTimeText;
	public Text m_gameOverText;

	public AudioSource m_backgroundMusic;
	public AnimationCurve m_pitchCurve = AnimationCurve.Linear(0, 0, 1, 1); 

	[HideInInspector]
	public float m_targetPitch = .5f;

	public float m_targetVolume = .25f;
	public float m_defaultPitch = .8f;
	public float m_audioBlendSpeed = 1.0f; 


	//-------------------------------------------------------------------------------------------------
	private float m_gameTimer = 0.0f;
	private float m_gameOverTimer = 0.0f;
	private bool m_preyWins = false;

	[HideInInspector]
	public eGameState m_currentState = eGameState.WAIT_FOR_READY;


	//-------------------------------------------------------------------------------------------------
	public void SetBGPitch(float pitch)
	{
		m_backgroundMusic.pitch = pitch;
	}


	//-------------------------------------------------------------------------------------------------
	public void SetTargetBGPitch( float lerpValue )
	{
		lerpValue = Mathf.Clamp01( lerpValue ); 
		m_targetPitch = lerpValue; 
	}


	//-------------------------------------------------------------------------------------------------
	public void SetTargetVolume(float volume)
	{
		m_targetVolume = Mathf.Clamp01(volume);
	}


	//-------------------------------------------------------------------------------------------------
	public PreyController[] GetHumanPrey()
	{
		List<PreyController> preyList = new List<PreyController>();
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);
		for (int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController prey = preys[preyIndex].GetComponent<PreyController>();
			if (prey != null)
			{
				if (prey.IsPlayer())
				{
					preyList.Add(prey);
				}
			}
		}
		return preyList.ToArray();
	}


	//-------------------------------------------------------------------------------------------------
	public int GetHumanPreyAliveCount()
	{
		int count = 0;
		PreyController[] preyList = GetHumanPrey();
		for(int preyIndex = 0; preyIndex < preyList.Length; ++preyIndex)
		{
			if (!preyList[preyIndex].m_isDead)
			{
				count += 1;
			}
		}
		return count;
	}
	

	//-------------------------------------------------------------------------------------------------
	public int GetPreyAliveCount()
	{
		return GameObject.FindGameObjectsWithTag(TAG_PREY).Length;
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
		Application.runInBackground = true; 

		HopperNetwork.Instance.OnPlayerJoin += AddPlayer;
		HopperNetwork.Instance.OnPlayerLeave += RemovePlayer;
		EnterState(m_currentState);

		SetTargetBGPitch(.5f); 

		m_UIJoin.SetActive(true); 
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateState(m_currentState);
		UpdateTimeLabel();
		UpdateGameOverLabel();
		UpdateMusic(); 
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
		if(state == eGameState.WAIT_FOR_READY)
		{
			SetTargetBGPitch(.5f);
			ClearBonePiles();
		}

		else if(state == eGameState.IN_GAME)
		{
			m_gameTimer = 0.0f;
			AudioManager.Play(eSoundType.FOX_HOWL);
		}

		else if(state == eGameState.GAME_OVER)
		{
			m_gameOverTimer = 0.0f;
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateState(eGameState state)
	{
		if (state == eGameState.WAIT_FOR_READY)
		{
			bool isReady = HopperNetwork.IsEveryoneReady();
			bool isEnoughPlayers = HopperNetwork.GetPlayerCount() >= 3;

			m_UIReadyUp.SetActive(false);
			m_UIPlayersRequired.SetActive(false);


			if (isReady && isEnoughPlayers)
			{
				HopperNetwork.StartGame();
				TransformRandomPrey();
				ChangeState(eGameState.IN_GAME);

				int count = HopperNetwork.GetPlayerCount();
				m_gameDuration = m_gameDurationBase + m_gameDurationAdditionalPerRabbit * (count - 1);
				m_UIInstructions.SetActive(false);
				m_firstGame = false;
			}
			else
			{
				if (m_currentState == eGameState.WAIT_FOR_READY)
				{
					if (HopperNetwork.GetPlayerCount() < 3)
					{
						m_UIPlayersRequired.SetActive(true);
					}
					else
					{
						m_UIReadyUp.SetActive(true);
					}
				}
			}
		}

		else if(state == eGameState.IN_GAME)
		{
			m_gameTimer += Time.deltaTime;
			int preyCount = GetHumanPreyAliveCount();

			//No players left wolf wins
			if(preyCount == 0)
			{
				m_preyWins = false;
				ChangeState(eGameState.GAME_OVER);
			}

			//Times up, bunnies win
			if(m_gameTimer >= m_gameDuration)
			{
				m_preyWins = true;
				ChangeState(eGameState.GAME_OVER);
			}
		}

		else if(state == eGameState.GAME_OVER)
		{
			m_gameOverTimer += Time.deltaTime;

			//Game over time expires
			if (m_gameOverTimer >= m_gameOverDuration)
			{
				HopperNetwork.EndGame();
				ReviveAndTransformBack();
				ChangeState(eGameState.WAIT_FOR_READY);
			}
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
	private void UpdateTimeLabel()
	{
		string timeLabel = "";
		if(m_currentState == eGameState.IN_GAME)
		{
			float gameTime = m_gameDuration - m_gameTimer;
			int seconds = (int)gameTime % 60;
			int minutes = (int)gameTime / 60;
			timeLabel = string.Format("{0:0}:{1:00}", minutes, seconds);
		}
		m_gameTimeText.text = timeLabel;
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateGameOverLabel()
	{
		string gameOverText = "";
		if (m_currentState == eGameState.GAME_OVER)
		{
			if(m_preyWins)
			{
				gameOverText = "BUNNIES WIN!";
			}
			
			else
			{
				gameOverText = "WOLVES WIN!";
			}
		}
		
		m_gameOverText.color = new Color(RandomMathColorFloat(3.0f), RandomMathColorFloat(5.0f), RandomMathColorFloat(7.0f));
		m_gameOverText.text = gameOverText;
	}

	//-------------------------------------------------------------------------------------------------
	private void UpdateMusic()
	{
		if (Time.time < 1.0f)
		{
			m_backgroundMusic.volume = 0.0f;
			m_backgroundMusic.pitch = m_defaultPitch;
			return;
		}

		float aliveIntensity = 0.0f;
		float timeIntensity = 0.0f;
		if (m_currentState == eGameState.IN_GAME)
		{
			float remaining = m_gameDuration - m_gameTimer;
			if (remaining < 10.0f)
			{
				timeIntensity = .2f;
			}
		}

		float pitch = m_backgroundMusic.pitch;
		float volume = m_backgroundMusic.volume; 

		float dt = Time.deltaTime;
		float lerpValue = Mathf.Clamp( Mathf.Pow( m_audioBlendSpeed * dt, 1.0f ), 0, .2f );

		float fatnessPitch = m_pitchCurve.Evaluate(m_targetPitch);
		fatnessPitch = m_defaultPitch;  // just get rid of fatness pitch, base it only on intensity
		float targetPitch = fatnessPitch + aliveIntensity + timeIntensity;
		pitch = Mathf.Lerp( pitch, targetPitch, Mathf.Clamp01( 2.0f * Time.deltaTime ) );
		volume = Mathf.Lerp( volume, m_targetVolume, lerpValue ); 

		m_backgroundMusic.pitch = pitch;
		m_backgroundMusic.volume = volume; 
	}


	//-------------------------------------------------------------------------------------------------
	float RandomMathColorFloat(float timeScale)
	{
		return (Mathf.Sin(Time.time * timeScale) + 1.0f) * 0.5f;
	}


	//-------------------------------------------------------------------------------------------------
	private void AddPlayer(VirtualNetworkController controller)
	{
		Vector3 spawnLocation = Vector3.zero;
		spawnLocation.z = GAME_Z;
		GameObject preyObject = Instantiate(m_preyPrefab, spawnLocation, Quaternion.identity);
		PreyController prey = preyObject.GetComponent<PreyController>();
		prey.m_netController = controller;

		m_UIJoin.SetActive(false);
		if (m_firstGame)
		{
			m_UIInstructions.SetActive(true);
		}
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
					// C4 - ghosts were multiplying really fast
					GameObject.Destroy(prey.gameObject); 
				}
			}

			if (m_firstGame && (HopperNetwork.GetPlayerCount() == 0))
			{
				m_UIJoin.SetActive(true);
				m_UIInstructions.SetActive(false);
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


	//-------------------------------------------------------------------------------------------------
	private void SpawnAdditionalPrey()
	{
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);
		for (int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController prey = preys[preyIndex].GetComponent<PreyController>();
			Instantiate(m_preyPrefab, prey.transform.position, Quaternion.identity);
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void TransformRandomPrey()
	{
		//Transform random bunny into wolf
		PreyController[] humanPrey = GetHumanPrey();
		int randomPreyIndex = Random.Range(0, humanPrey.Length);
		humanPrey[randomPreyIndex].TransformIntoPredator();
	}


	//-------------------------------------------------------------------------------------------------
	private void ReviveAndTransformBack()
	{
		//Find controller if it was a prey
		GameObject[] preys = GameObject.FindGameObjectsWithTag(TAG_PREY);
		for (int preyIndex = 0; preyIndex < preys.Length; ++preyIndex)
		{
			PreyController prey = preys[preyIndex].GetComponent<PreyController>();
			if (prey != null)
			{
				//Revive dead players
				if (prey.m_netController != null)
				{
					prey.m_isDead = false;
				}
				//Clear AI
				else
				{
					Destroy(prey.gameObject);
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
				predator.TransformIntoPrey();
			}
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void ClearBonePiles()
	{
		GameObject[] bonePiles = GameObject.FindGameObjectsWithTag(TAG_BONEPILE);
		for (int boneIndex = 0; boneIndex < bonePiles.Length; ++boneIndex)
		{
			Destroy(bonePiles[boneIndex]);
		}
	}
}
