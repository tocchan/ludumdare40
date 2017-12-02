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


	//-------------------------------------------------------------------------------------------------
	[Header("Debug")]
	public bool debugControl = false;


	//-------------------------------------------------------------------------------------------------
	private float m_stomachSizeCurrent = 0.0f;
	private Vector2 m_moveCurrent = Vector2.zero;


	//-------------------------------------------------------------------------------------------------
	// References
	//-------------------------------------------------------------------------------------------------
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
		UpdateFoodConsumption();
		UpdateDebugInput();
		UpdateMove();
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
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateMove()
	{
		//Get move speed from stomach
		float stomachPercent = m_stomachSizeCurrent / m_stomachSizeMax;
		float moveSpeed = Mathf.Lerp(m_moveSpeedMax, m_moveSpeedMin, stomachPercent);

		//Move predator
		m_moveCurrent.Normalize();
		m_rigidbody.velocity = m_moveCurrent * moveSpeed;

		//Reset move direction
		m_moveCurrent = Vector2.zero;
	}


	//-------------------------------------------------------------------------------------------------
	public void Move(Vector2 moveDirection)
	{
		m_moveCurrent += moveDirection.normalized;
	}


	//-------------------------------------------------------------------------------------------------
	public void Attack()
	{

	}
}
