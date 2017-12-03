using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------------------------------------
public class ExplosionEffect : MonoBehaviour
{
	public ParticleSystem m_explosionBase;
	public ParticleSystem m_explosionTrail;


	//-------------------------------------------------------------------------------------------------
	void Start()
	{
		m_explosionBase.Play();
		m_explosionTrail.Play();
		Destroy(gameObject, 10.0f);
	}
}
