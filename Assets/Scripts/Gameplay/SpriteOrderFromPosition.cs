using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class SpriteOrderFromPosition : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------
	public GameObject m_reference;
	private SpriteRenderer m_renderer;
	private ParticleSystem m_particleSystem;


	//-------------------------------------------------------------------------------------------------
	private void Start ()
	{
		if(m_reference == null)
		{
			m_reference = gameObject;
		}

		m_renderer = GetComponent<SpriteRenderer>();
		m_particleSystem = GetComponent<ParticleSystem>();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		int order = (int)(-100.0f * (m_reference.transform.position.y + 10.0f) + 2000.0f);
		if (m_renderer != null)
		{
			m_renderer.sortingOrder = order;
		}

		else if(m_particleSystem != null)
		{
			m_particleSystem.GetComponent<Renderer>().sortingOrder = order + 1;
		}
	}
}
