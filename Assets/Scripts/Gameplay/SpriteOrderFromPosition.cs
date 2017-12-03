using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class SpriteOrderFromPosition : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------
	public GameObject m_reference;
	private SpriteRenderer m_renderer;


	//-------------------------------------------------------------------------------------------------
	private void Start ()
	{
		m_renderer = GetComponent<SpriteRenderer>();
		if(m_reference == null)
		{
			m_reference = m_renderer.gameObject;
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		m_renderer.sortingOrder = (int)(-100.0f * (m_reference.transform.position.y + 10.0f) + 2000.0f);
	}
}
