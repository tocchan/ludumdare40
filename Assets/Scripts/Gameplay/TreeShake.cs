using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class TreeShake : MonoBehaviour
{
	public ParticleSystem m_leafParticleSystem;

	public float m_emitRateShake;
	public float m_emitRateNormal;
	float m_emitRate = 0.0f;
	float m_emitCounter = 0.0f;



	//-------------------------------------------------------------------------------------------------
	// Unity
	//-------------------------------------------------------------------------------------------------
	private void Start()
	{
		m_emitRate = m_emitRateNormal;
	}


	//-------------------------------------------------------------------------------------------------
	private void Update ()
	{
		//Decay rate of emission
		m_emitRate -= Time.deltaTime;
		m_emitRate = Mathf.Clamp(m_emitRate, m_emitRateNormal, m_emitRateShake);

		//Update emitter
		m_emitCounter += m_emitRate * Time.deltaTime;
		if(m_emitCounter >= 1.0f)
		{
			//m_leafParticleSystem.Emit(1);
			m_emitCounter -= 1.0f;
		}
	}


	//-------------------------------------------------------------------------------------------------
	private void OnCollisionEnter2D(Collision2D other)
	{
		Shake();
	}


	//-------------------------------------------------------------------------------------------------
	// Functions
	//-------------------------------------------------------------------------------------------------
	public void Shake()
	{
		m_emitRate += m_emitRateShake;
		m_leafParticleSystem.Emit(5);
	}
}
