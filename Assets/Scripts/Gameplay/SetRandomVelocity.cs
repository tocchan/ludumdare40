using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomVelocity : MonoBehaviour
{
   public float m_minSpeed = 5.0f; 
   public float m_maxSpeed = 10.0f; 
   public float m_minAngle = 0.0f; 
   public float m_maxAngle = 1.0f; 

   private Vector2 m_velocity; 

   // Use this for initialization
   void Start()
   {
      Launch( Vector2.right );  
   }

   public void Launch( Vector2 forward, float scale = 1.0f ) 
   {
      Vector2 up = new Vector2( -forward.y, forward.x ); 

      float angle = Random.Range( m_minAngle, m_maxAngle ); 
      float x = Mathf.Cos( angle ); 
      float y = -Mathf.Sin( angle ); 

      Vector2 dir = up * y + forward * x;
      float speed = Random.Range( m_minSpeed, m_maxSpeed ) * scale; 
      m_velocity = dir * speed;  
   }

   public void Dampen( float amount )
   {
      m_velocity = amount * m_velocity; 
   }
   
   // Update is called once per frame
   void Update()
   {
      Vector3 localPos = transform.localPosition; 
      Vector2 pos = localPos;
      pos = pos + Time.deltaTime * m_velocity;
      localPos.x = pos.x;
      localPos.y = pos.y;
      transform.localPosition = localPos; 
   }
}

