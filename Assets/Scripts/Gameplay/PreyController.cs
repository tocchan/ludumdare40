﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class PreyController : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------
	// Members
	//-------------------------------------------------------------------------------------------------
	[Header("Settings")]
	public float m_hopDuration = 0.5f;
	public float m_hopSpeed = 1.0f;


	//-------------------------------------------------------------------------------------------------
	[Header("AI")]
	public float m_moveDelayMin = 0.1f;
	public float m_moveDelayMax = 1.0f;


	//-------------------------------------------------------------------------------------------------
	[Header("Debug")]
	public bool m_debugControl = false;


	//-------------------------------------------------------------------------------------------------
	private bool m_isHopping = false;
	private bool m_isMating = false;
	private float m_hopTimer = 0.0f;
	private Vector2 m_hopDirection = Vector2.zero;

	private float m_moveTimerAI = 0.0f;

	private VirtualNetworkController m_netController = null;

	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public GameObject m_preyPrefab;
	Rigidbody2D m_rigidbody;


	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateInputDebug();
		UpdateInputNet();
		UpdateInputAI();
		UpdateHop();
	}


	//-------------------------------------------------------------------------------------------------
	// Functions
	//-------------------------------------------------------------------------------------------------
	private void UpdateInputDebug()
	{
		if(!m_debugControl)
		{
			return;
		}

		if(Input.GetKeyDown(KeyCode.W)
			|| Input.GetKeyDown(KeyCode.UpArrow))
		{
			Hop(Vector2.up);
		}

		if (Input.GetKeyDown(KeyCode.S)
			|| Input.GetKeyDown(KeyCode.DownArrow))
		{
			Hop(Vector2.down);
		}

		if (Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Hop(Vector2.left);
		}

		if (Input.GetKeyDown(KeyCode.D)
			|| Input.GetKeyDown(KeyCode.RightArrow))
		{
			Hop(Vector2.right);
		}

		//Left click
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) 
			{
				Vector2 positionMouse = new Vector2(hit.point.x, hit.point.y);
				Vector2 positionCurrent = transform.position;
				Vector2 hopDirection = positionMouse - positionCurrent;
				Hop(hopDirection);
			}
		}

		//Right click
		if(Input.GetMouseButtonDown(1))
		{
			Mate();
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateInputNet()
	{

	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateInputAI()
	{
		if(m_debugControl)
		{
			return;
		}

		if(m_isHopping)
		{
			return;
		}

		m_moveTimerAI -= Time.deltaTime;
		if(m_moveTimerAI <= 0.0f)
		{
			Vector2 randomDirection = Random.insideUnitCircle;
			Hop(randomDirection);
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateHop()
	{
		if (m_isHopping)
		{
			m_hopTimer += Time.deltaTime;

			//Maybe alter it over time eventually
			m_rigidbody.velocity = m_hopDirection * m_hopSpeed;

			if (m_hopTimer > m_hopDuration)
			{
				StopHop();
			}
		}
	}


	//-------------------------------------------------------------------------------------------------
	public void Hop(Vector2 hopDirection)
	{
		if (m_isHopping)
		{
			return;
		}

		m_hopDirection = hopDirection.normalized;
		StartHop();
	}


	//-------------------------------------------------------------------------------------------------
	private void StartHop()
	{
		m_isHopping = true;
		m_hopTimer = 0.0f;

		//Rest AI random move delay
		m_moveTimerAI = Random.Range(m_moveDelayMin, m_moveDelayMax);
	}
	

	//-------------------------------------------------------------------------------------------------
	private void StopHop()
	{
		m_isHopping = false;
		m_isMating = false;
		m_rigidbody.velocity = Vector2.zero;
	}


	//-------------------------------------------------------------------------------------------------
	public void Mate()
	{
		//Find closest mate
		PreyController closestPrey = GameManager.GetClosestPrey(this);
		if(closestPrey != null)
		{
			//Mate in their direction
			Vector2 positionMate = closestPrey.transform.position;
			Vector2 positionCurrent = transform.position;
			Vector2 hopDirection = positionMate - positionCurrent;
			m_isMating = true;
			Hop(hopDirection);
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void SpawnBabiesWith(PreyController otherPrey)
	{
		//If mating then spawn babies
		if(m_isMating || otherPrey.m_isMating)
		{
			//Spawn setup
			int spawnTotal = GameManager.GetInstance().m_babySpawnTotal;
			Vector2 spawnLocation = (transform.position + otherPrey.transform.position) / 2;
			Vector3 babyLocation = spawnLocation;
			babyLocation.z = GameManager.GAME_Z;

			//Spawn babies
			for (int babyIndex = 0; babyIndex < spawnTotal; ++babyIndex )
			{
				Instantiate(m_preyPrefab, babyLocation, Quaternion.identity);
			}

			StartHop();
			otherPrey.StopHop();
		}
	}


	//-------------------------------------------------------------------------------------------------
	private bool IsPlayer()
	{
		//return m_netController != null;

		//Debug
		return true;
	}


	//-------------------------------------------------------------------------------------------------
	private void OnCollisionEnter2D(Collision2D other)
	{
		GameObject hitObject = other.gameObject;
		if(IsPlayer())
		{
			PreyController hitPrey = hitObject.GetComponent<PreyController>();
			if (hitPrey == null)
			{
				return;
			}

			if (hitPrey.IsPlayer())
			{
				SpawnBabiesWith(hitPrey);
			}
		}
	}
}
