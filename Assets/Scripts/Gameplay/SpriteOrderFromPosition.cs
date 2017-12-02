using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class SpriteOrderFromPosition : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------
	private SpriteRenderer m_renderer;


	//-------------------------------------------------------------------------------------------------
	private void Start ()
	{
		m_renderer = GetComponent<SpriteRenderer>();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		m_renderer.sortingOrder = (int)(-100.0f * (transform.position.y + 10.0f) + 2000.0f);
	}
}
