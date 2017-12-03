using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class ReadyCheck : MonoBehaviour
{
	public PreyController m_controllerReference;
	private SpriteRenderer m_renderer;


	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_renderer = GetComponent<SpriteRenderer>();
	}


	//-------------------------------------------------------------------------------------------------
	private void Update()
	{
		if(m_controllerReference == null)
		{
			return;
		}

		//Visible if ready / Hidden if not ready
		bool isVisible = false;
		if(GameManager.GetInstance().m_currentState == eGameState.WAIT_FOR_READY)
		{
			if(m_controllerReference.m_netController != null)
			{
				isVisible = m_controllerReference.m_netController.ClientIsReady;
			}
		}
		m_renderer.enabled = isVisible;
	}
}
