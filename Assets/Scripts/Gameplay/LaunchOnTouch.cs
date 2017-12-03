using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchOnTouch : MonoBehaviour
{
   public Bounce m_bounce; 
   public SetRandomVelocity m_velocity;
   public Spinner m_spinner; 

   private int m_layer; 

   void Start()
   {
      m_layer = LayerMask.NameToLayer("Living"); 
   }

   void OnTriggerEnter2D( Collider2D collider )
   {
      if (!m_bounce.IsNearGround()) {
         return;
      }

      GameObject go = collider.gameObject;
      if (go.layer != m_layer) { 
         return;
      }

      // get distance from me to object;
      Vector2 dir = transform.position - go.transform.position;
      dir.Normalize();  

      m_velocity.m_minAngle = -30.0f; 
      m_velocity.m_maxAngle = 30.0f; 
      m_velocity.Launch( dir, 3.0f ); 

      m_bounce.Launch(.5f); 
      m_spinner.Spin(); 
   }
}

