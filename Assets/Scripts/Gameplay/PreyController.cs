using System.Collections;
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
	public float m_hopHeightMax = 0.25f;
	public AnimationCurve m_hopCurve;


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
	private Vector2 m_visualStartLocation = Vector2.zero;
	private float m_moveTimerAI = 0.0f;

	[HideInInspector]
	public bool m_isDead = false;

	[HideInInspector]
	public VirtualNetworkController m_netController = null;


	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public GameObject m_visualReference;
	public GameObject m_shadowReference;
	public GameObject m_collisionReference;
	Rigidbody2D m_rigidbody;
	Animator m_animator;


	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_animator = m_visualReference.GetComponent<Animator>();
		m_visualStartLocation = m_visualReference.transform.localPosition;
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateInputDebug();
		UpdateInputNet();
		UpdateInputAI();
		UpdateHop();
		UpdateVisual();
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
		if(!IsPlayer())
		{
			return;
		}

		if(m_netController.ConsumeAllActions())
		{
			Mate();
		}

		if(m_netController.Movement != Vector2.zero)
		{
			Hop(m_netController.Movement);
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateInputAI()
	{
		if(m_debugControl)
		{
			return;
		}

		if(IsPlayer())
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
	private void UpdateVisual()
	{
		//Hop Height
		float hopPercent = m_hopTimer / m_hopDuration;
		Vector2 hopHeight = m_hopCurve.Evaluate(hopPercent) * m_hopHeightMax * Vector2.up;
		Vector2 hopLocation = m_visualStartLocation + hopHeight;
		m_visualReference.transform.localPosition = hopLocation;

		//Facing direction
		if(m_hopDirection.x > 0.0f)
		{
			Vector3 scale = m_visualReference.transform.localScale;
			scale.x = -1.0f * Mathf.Abs(scale.x);
			m_visualReference.transform.localScale = scale;

			scale = m_shadowReference.transform.localScale;
			scale.x = -1.0f * Mathf.Abs(scale.x);
			m_shadowReference.transform.localScale = scale;
		}
		else
		{
			Vector3 scale = m_visualReference.transform.localScale;
			scale.x = Mathf.Abs(scale.x);
			m_visualReference.transform.localScale = scale;

			scale = m_visualReference.transform.localScale;
			scale.x = Mathf.Abs(scale.x);
			m_shadowReference.transform.localScale = scale;
		}

		//Dead bunnies are ghost
		m_shadowReference.SetActive( !m_isDead ); 
		if(m_isDead)
		{
			m_visualReference.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
			m_collisionReference.layer = LayerMask.NameToLayer("Ghost");
		}
		else
		{
			m_visualReference.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			m_collisionReference.layer = LayerMask.NameToLayer("Living");
		}

		//Hop animation
		m_animator.Play(GameManager.ANIM_RABBIT_HOP, 0, hopPercent);
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
		m_hopTimer = 0.0f;
		m_rigidbody.velocity = Vector2.zero;
	}


	//-------------------------------------------------------------------------------------------------
	public void Mate()
	{
		if (m_isDead)
		{
			return;
		}

		if (GameManager.GetInstance().m_currentState != eGameState.IN_GAME)
		{
			return;
		}

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
			AudioManager.Play(eSoundType.BUNNY_SEXY_TIMES);

			//Spawn setup
			int spawnTotal = GameManager.GetInstance().m_babySpawnTotal;
			Vector2 spawnLocation = (transform.position + otherPrey.transform.position) / 2;
			Vector3 babyLocation = spawnLocation;
			babyLocation.z = GameManager.GAME_Z;

			//Spawn babies
			for (int babyIndex = 0; babyIndex < spawnTotal; ++babyIndex )
			{
				Instantiate(GameManager.GetInstance().m_preyPrefab, babyLocation, Quaternion.identity);
			}

			StopHop();
			otherPrey.StopHop();
		}
	}


	//-------------------------------------------------------------------------------------------------
	public void Eaten()
	{
		//Death sound
		AudioManager.Play(eSoundType.BUNNY_DEATH);

		//Spawn bone pile
		Instantiate(GameManager.GetInstance().m_bonePilePrefab, transform.position, transform.rotation);

		//Handle death
		if(IsPlayer())
		{
			m_isDead = true;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	//-------------------------------------------------------------------------------------------------
	public bool IsPlayer()
	{
		return m_netController != null;
	}


	//-------------------------------------------------------------------------------------------------
	public void TransformIntoPredator()
	{
		m_netController.SetWolf();
		GameObject predator = Instantiate(GameManager.GetInstance().m_predatorPrefab, transform.position, transform.rotation);
		predator.GetComponent<PredatorController>().m_netController = m_netController;
		predator.GetComponent<PredatorController>().m_delayMovement = GameManager.GetInstance().m_wolfDelayOnStart;
		Destroy(gameObject);
	}


	//-------------------------------------------------------------------------------------------------
	private void OnCollisionEnter2D(Collision2D other)
	{
		GameObject hitObject = other.gameObject;
		if (IsPlayer())
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
			else
			{
				AudioManager.Play(eSoundType.BUNNY_BUMP);
			}
		}
	}
}
