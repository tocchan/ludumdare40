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


	//-------------------------------------------------------------------------------------------------
	[Header("Debug")]
	public bool debugControl = false;


	//-------------------------------------------------------------------------------------------------
	private Vector2 m_hopDirection = Vector2.zero;
	private float m_hopTimer = 0.0f;
	private bool m_isHopping = false;


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
		UpdateDebugInput();
		UpdateHop();
	}


	//-------------------------------------------------------------------------------------------------
	// Functions
	//-------------------------------------------------------------------------------------------------
	private void UpdateHop()
	{
		if(m_isHopping)
		{
			m_hopTimer += Time.deltaTime;

			//Maybe alter it over time eventually
			m_rigidbody.velocity = m_hopDirection * m_hopSpeed;

			if(m_hopTimer > m_hopDuration)
			{
				StopHop();
			}
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void UpdateDebugInput()
	{
		if(!debugControl)
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

		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) 
			{
				Vector2 mouseClickPosition = new Vector2(hit.point.x, hit.point.y);
				Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
				Vector2 hopDirection = mouseClickPosition - currentPosition;
				Hop(hopDirection);
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
	}
	

	//-------------------------------------------------------------------------------------------------
	private void StopHop()
	{
		m_isHopping = false;
		m_rigidbody.velocity = Vector2.zero;
	}


	//-------------------------------------------------------------------------------------------------
	public void Mate()
	{

	}
}
