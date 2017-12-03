using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
   public float m_gravity = -9.8f; 
   public float m_minInitialVelocity = 5.0f;
   public float m_maxInitialVelocity = 20.0f;
   public float m_bounce = 1.0f; 

   public Spinner m_spinner; 
   public SetRandomVelocity m_velObject; 

   private float m_velocity; 
   private float m_initialY; 

   // Use this for initialization
   void Start()
   {
      m_initialY = transform.localPosition.y;  
      Launch();
   }

   public void Launch( float scale = 1.0f )
   {
      m_velocity = Random.Range( m_minInitialVelocity, m_maxInitialVelocity ) * scale;
   }

   public bool IsNearGround()
   {
      return (transform.localPosition.y - m_initialY) < .2f;
   }
   
   // Update is called once per frame
   void Update()
   {
      float y = transform.localPosition.y - m_initialY; 
      float dt = Time.deltaTime; 
      dt = Mathf.Clamp( dt, 0.0f, 0.1f ); ;
      m_velocity += dt * m_gravity;
      y += dt * m_velocity; 

      if (y <= 0.0f) {
         m_velocity = -m_velocity * m_bounce; 
         if (m_spinner != null) {
            m_spinner.Dampen(m_bounce);
         }

         if (m_velObject != null) {
            m_velObject.Dampen(m_bounce);
         }

         y = 0.0f;
      }

      Vector3 pos = transform.localPosition;
      pos.y = y + m_initialY; 
      transform.localPosition = pos;  
   }
}

