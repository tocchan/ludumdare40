using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
   public float BufferRoom = 2.0f; 

   // Update is called once per frame
   void Update()
   {
      Bounds b = ScreenUtil.GetVisibleBounds(); 
      b.Expand(BufferRoom); 

      Vector2 pos = transform.position; 
      if (!b.Contains(pos)) {
         Debug.Log(" Destroyed" );
         GameObject.Destroy(gameObject);
      }
   }

   void OnDrawGizmos()
   {
      Bounds b = ScreenUtil.GetVisibleBounds(); 
      b.Expand(BufferRoom); 

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube( b.center, b.extents * 2.0f ); 
   }
}

