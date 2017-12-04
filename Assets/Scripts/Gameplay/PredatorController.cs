using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class PredatorController : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------
	// Members
	//-------------------------------------------------------------------------------------------------
	[Header("Settings")]
	public float m_moveSpeedMax = 5.0f;
	public float m_moveSpeedMin = 1.0f;
	public AnimationCurve m_moveSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1); 
	public float m_stomachSizeMax = 3.0f;
	public float m_foodSizeBunny = 1.0f;
	public float m_foodDigestSpeed = 1.0f;
	public float m_attackDuration = 0.25f;


	//-------------------------------------------------------------------------------------------------
	[Header("Debug")]
	public bool m_debugControl = false;


	//-------------------------------------------------------------------------------------------------
	private float m_stomachSizeCurrent = 0.0f;
	private float m_attackTimer = 0.0f;
	public float m_delayMovement = 0.0f;
	private Vector2 m_moveCurrent = Vector2.zero;
	private Vector2 m_movePrevious = Vector2.one;
	private bool m_moveNeedsUpdate = false;
	private bool m_isAttacking = false;
	private bool m_isFirstPrey = false; 


	[HideInInspector]
	public VirtualNetworkController m_netController;


	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public GameObject m_attackAreaReference;
	public SpriteRenderer m_attackBiteReference;
	public GameObject m_visualReference;
	public GameObject m_shadowReference;
	Rigidbody2D m_rigidbody;
	CircleCollider2D m_attackAreaCollider;
	ContactFilter2D m_attackAreaFilter;
	Animator m_animator;


	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_animator = m_visualReference.GetComponent<Animator>();
		m_attackAreaCollider = m_attackAreaReference.GetComponent<CircleCollider2D>();
		m_attackAreaFilter = new ContactFilter2D();
		m_attackAreaFilter.NoFilter();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateFoodConsumption();
		UpdateInputDebug();
		UpdateInputNet();
		UpdateMove();
		UpdateAttack();
		UpdateVisual();
		UpdateAudio();
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateFoodConsumption()
	{
		m_animator.SetFloat("WeightPercent", GetStomachPercent());

	  float cur_size = m_stomachSizeCurrent; 
		m_stomachSizeCurrent -= Time.deltaTime * m_foodDigestSpeed;
		m_stomachSizeCurrent = Mathf.Clamp(m_stomachSizeCurrent, 0.0f, m_stomachSizeMax);

	  if ((cur_size > 0.0f) && (m_stomachSizeCurrent == 0.0f)) 
	  {
		 AudioManager.Play( eSoundType.FOX_ANGRY ); 
	  }
	  else if (m_stomachSizeCurrent == m_stomachSizeMax)
	  {
		 AudioManager.Play( eSoundType.FOX_EXHAUSTED ); 
	  }
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateInputDebug()
	{
		if (!m_debugControl)
		{
			return;
		}

		if (Input.GetKey(KeyCode.W)
			|| Input.GetKey(KeyCode.UpArrow))
		{
			Move(Vector2.up);
		}

		if (Input.GetKey(KeyCode.S)
			|| Input.GetKey(KeyCode.DownArrow))
		{
			Move(Vector2.down);
		}

		if (Input.GetKey(KeyCode.A)
			|| Input.GetKey(KeyCode.LeftArrow))
		{
			Move(Vector2.left);
		}

		if (Input.GetKey(KeyCode.D)
			|| Input.GetKey(KeyCode.RightArrow))
		{
			Move(Vector2.right);
		}

		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit))
			{
				Vector2 mouseClickPosition = new Vector2(hit.point.x, hit.point.y);
				Vector2 currentPosition = transform.position;
				Vector2 moveDirection = mouseClickPosition - currentPosition;
				Move(moveDirection);
			}
		}

		if(Input.GetMouseButtonDown(1))
		{
			Attack();
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateInputNet()
	{
		if (!IsPlayer())
		{
			return;
		}

		if (m_netController.ConsumeAllActions())
		{
			Attack();
		}

		if (m_netController.Movement != Vector2.zero)
		{
			Move(m_netController.Movement);
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateMove()
	{
		if(!m_moveNeedsUpdate)
		{
			return;
		}

		//Don't allow movement for delay seconds
		if(m_delayMovement > 0.0f)
		{
			m_delayMovement -= Time.deltaTime;
			return;
		}

		//Get move speed from stomach
		float moveLerp = m_moveSpeedCurve.Evaluate( Mathf.Clamp01(1.0f - GetStomachPercent()) ); 
		float moveSpeed = Mathf.Lerp(m_moveSpeedMin, m_moveSpeedMax, moveLerp);

		//Move predator
		m_moveCurrent.Normalize();
		m_rigidbody.velocity = m_moveCurrent * moveSpeed;

		//Reset move direction
		m_moveCurrent = Vector2.zero;

		m_moveNeedsUpdate = false;
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateAttack()
	{
		if(m_isAttacking)
		{
			m_attackTimer += Time.deltaTime;

			//Find what we can attack
			int maxThingsToEatThisFrame = 10;
			Collider2D[] colliders = new Collider2D[maxThingsToEatThisFrame];
			m_attackAreaCollider.OverlapCollider(m_attackAreaFilter, colliders);

			//Loop through all things attacked
			for(int colliderIndex = 0; colliderIndex < colliders.Length; ++colliderIndex)
			{
				if(colliders[colliderIndex] == null)
				{
					continue;
				}

				GameObject attackedObject = colliders[colliderIndex].gameObject;
				Transform attackedObjectParent = attackedObject.transform.parent;
				if(attackedObjectParent == null)
				{
					continue;
				}

				PreyController attackedPrey = attackedObjectParent.GetComponent<PreyController>();
				if(attackedPrey != null)
				{
					if(attackedPrey.m_isDead)
					{
						continue;
					}

					if (m_isFirstPrey) {
						AudioManager.Play(eSoundType.FOX_EAT);
						AudioManager.Play(eSoundType.BUNNY_SCREAM); 
						m_isFirstPrey = false;
					}

					attackedPrey.Eaten();
					IncreaseStomachSize( m_foodSizeBunny ); 
				}
			}

			//Stop attacking
			if (m_attackTimer >= m_attackDuration)
			{
				m_isAttacking = false;
			}
		}
		else
		{
			//Update attack area location
			float attackAreaOffset = 0.75f;
			m_attackAreaReference.transform.localPosition = m_movePrevious * attackAreaOffset;
			m_attackAreaReference.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(m_movePrevious.y, m_movePrevious.x));
		}

		if(m_isAttacking)
		{
			float attackAlpha = (m_attackDuration - m_attackTimer) / m_attackDuration;
			m_attackBiteReference.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sqrt(attackAlpha));
		}
		else
		{
			m_attackBiteReference.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		}
	}

	//-------------------------------------------------------------------------------------------------
	void IncreaseStomachSize( float amount )
	{
		float prev_size = m_stomachSizeCurrent; 
		m_stomachSizeCurrent += amount; 

		if ((prev_size < 1.0f) && (m_stomachSizeCurrent >= 1.0f)) 
		{
			// 50% chance to make a happy sound
			if (Random.value > .5f) {
			Invoke( "PlayHappySound", Random.Range( .5f, 1.5f ) ); 
			}
		}
	}

	//-------------------------------------------------------------------------------------------------
	void PlayHappySound()
	{
		AudioManager.Play(eSoundType.FOX_HAPPY);
	}

	//-------------------------------------------------------------------------------------------------
	private void UpdateVisual()
	{
		//Hop Height
		float bitePercent = m_attackTimer / m_attackDuration;

		//Facing direction
		if (m_movePrevious.x > 0.0f)
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

			scale = m_shadowReference.transform.localScale;
			scale.x = Mathf.Abs(scale.x);
			m_shadowReference.transform.localScale = scale;
		}

		//Setting sprite animation
		if(m_isAttacking)
		{
			m_animator.Play(GameManager.ANIM_WOLF_BITE, 0, bitePercent);
		}

		else if (m_rigidbody.velocity.magnitude > 0.5f)
		{
			m_animator.Play(GameManager.ANIM_WOLF_WALK, 0, Time.time % 1.0f);
		}

		else
		{
			m_animator.Play(GameManager.ANIM_WOLF_IDLE, 0, 0.0f);
		}

		//Faded when delayed
		if(m_delayMovement > 0.0f)
		{
			m_visualReference.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f,0.5f);
		}
		else
		{
			m_visualReference.GetComponent<SpriteRenderer>().color = Color.white;
		}

		//Can't be pushed while a ghost
		//if(m_delayMovement > 0.0f)
		//{
		//	gameObject.layer = LayerMask.NameToLayer(GameManager.LAYER_GHOST);
		//}
		//else
		//{
		//	gameObject.layer = LayerMask.NameToLayer(GameManager.LAYER_LIVING);
		//}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateAudio()
	{
		float stomachPercent = GetStomachPercent();
		GameManager.GetInstance().SetTargetBGPitch(stomachPercent);
	}


	//-------------------------------------------------------------------------------------------------
	public void Move(Vector2 moveDirection)
	{
		m_moveCurrent += moveDirection.normalized;
		m_moveNeedsUpdate = true;
		m_movePrevious = moveDirection.normalized;
	}


	//-------------------------------------------------------------------------------------------------
	public void Attack()
	{
		if(m_isAttacking)
		{
			return;
		}

		if(GameManager.GetInstance().m_currentState == eGameState.GAME_OVER)
		{
			return;
		}

		if(m_delayMovement > 0.0f)
		{
			return;
		}

		m_isFirstPrey = true; 
		m_isAttacking = true;
		m_attackTimer = 0.0f;

		AudioManager.Play(eSoundType.FOX_BITE); 
	}


	//-------------------------------------------------------------------------------------------------
	public bool IsPlayer()
	{
		return m_netController != null;
	}


	//-------------------------------------------------------------------------------------------------
	public void TransformIntoPrey()
	{
		GameObject prey = Instantiate(GameManager.GetInstance().m_preyPrefab, transform.position, transform.rotation);
		prey.GetComponent<PreyController>().m_netController = m_netController;
		Destroy(gameObject);
	}

	
	//-------------------------------------------------------------------------------------------------
	float GetStomachPercent()
	{
		return m_stomachSizeCurrent / m_stomachSizeMax;
	}
}
