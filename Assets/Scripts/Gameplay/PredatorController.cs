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
	public bool debugControl = false;


	//-------------------------------------------------------------------------------------------------
	private float m_stomachSizeCurrent = 0.0f;
	private float m_attackTimer = 0.0f;
	private Vector2 m_moveCurrent = Vector2.zero;
	private Vector2 m_movePrevious = Vector2.one;
	private bool m_moveNeedsUpdate = false;
	private bool m_isAttacking = false;


	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
	[Header("References")]
	public GameObject m_attackAreaReference;
	Rigidbody2D m_rigidbody;
	CircleCollider2D m_attackAreaCollider;
	ContactFilter2D m_attackAreaFilter;


	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_attackAreaCollider = m_attackAreaReference.GetComponent<CircleCollider2D>();
		m_attackAreaFilter = new ContactFilter2D();
		m_attackAreaFilter.NoFilter();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		UpdateFoodConsumption();
		UpdateDebugInput();
		UpdateMove();
		UpdateAttack();
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateFoodConsumption()
	{
		m_stomachSizeCurrent -= Time.deltaTime * m_foodDigestSpeed;
		m_stomachSizeCurrent = Mathf.Clamp(m_stomachSizeCurrent, 0.0f, m_stomachSizeMax);
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateDebugInput()
	{
		if (!debugControl)
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
				PreyController attackedPrey = attackedObject.GetComponent<PreyController>();
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
			float attackAreaOffset = 0.25f;
			m_attackAreaReference.transform.localPosition = m_movePrevious * attackAreaOffset;
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
		Move(m_movePrevious);
		m_isAttacking = true;
		m_attackTimer = 0.0f;
	}
}
