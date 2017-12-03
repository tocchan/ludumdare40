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
	private Vector2 m_moveCurrent = Vector2.zero;
	private Vector2 m_movePrevious = Vector2.one;
	private bool m_moveNeedsUpdate = false;
	private bool m_isAttacking = false;

	[HideInInspector]
	public VirtualNetworkController m_netController;


	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public GameObject m_attackAreaReference;
	public GameObject m_visualReference;
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
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateFoodConsumption()
	{
		m_stomachSizeCurrent -= Time.deltaTime * m_foodDigestSpeed;
		m_stomachSizeCurrent = Mathf.Clamp(m_stomachSizeCurrent, 0.0f, m_stomachSizeMax);
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

		//Get move speed from stomach
		float stomachPercent = m_stomachSizeCurrent / m_stomachSizeMax;
		float moveSpeed = Mathf.Lerp(m_moveSpeedMax, m_moveSpeedMin, stomachPercent);

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
					attackedPrey.Eaten();
					m_stomachSizeCurrent += m_foodSizeBunny;
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
			float attackAreaOffset = 0.5f;
			m_attackAreaReference.transform.localPosition = m_movePrevious * attackAreaOffset;
		}
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
		}
		else
		{
			Vector3 scale = m_visualReference.transform.localScale;
			scale.x = Mathf.Abs(scale.x);
			m_visualReference.transform.localScale = scale;
		}

		//Setting sprite animation
		if(m_isAttacking)
		{
			m_animator.Play(GameManager.ANIM_WOLF_BITE, 0, bitePercent);
		}

		else if (m_rigidbody.velocity.magnitude > 2.0f)
		{
			m_animator.Play(GameManager.ANIM_WOLF_SNIFF, 0, 0.0f);
		}

		else
		{
			m_animator.Play(GameManager.ANIM_WOLF_IDLE, 0, 0.0f);
		}
	}


	//-------------------------------------------------------------------------------------------------
	public void Move(Vector2 moveDirection)
	{
		if(m_isAttacking)
		{
			return;
		}

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

		Move(m_movePrevious);
		m_isAttacking = true;
		m_attackTimer = 0.0f;
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
}
