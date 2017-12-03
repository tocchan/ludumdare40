using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
   public float m_minSpinSpeed = 30.0f; 
   public float m_MaxSpinSpeed = 720.0f; 

   private float m_spinSpeed; 

   void Start()
   {
      Spin();
   }

   public void Spin()
   {
      m_spinSpeed = Random.Range( m_minSpinSpeed, m_MaxSpinSpeed );    
      m_spinSpeed *= (Random.value >= .5f) ? 1.0f : -1.0f; 
   }

   public void Dampen( float amount )
   {
      m_spinSpeed *= amount; 
   }
   
   // Update is called once per frame
   void Update()
   {
      Vector3 euler = transform.localRotation.eulerAngles;
      euler.z += m_spinSpeed * Time.deltaTime;
      transform.localRotation = Quaternion.Euler(euler); 
   }
}

