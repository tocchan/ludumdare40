using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class ScreenUtil
{
   // Used to just get the bounding rect for my game
   public static Bounds GetVisibleBounds()
   {
      Camera mc = Camera.main;
      if (null == mc) {
         return new Bounds();
      }

      // move toward the 0 plane
      float z = -mc.transform.position.z;
      Vector3 vp_bl = new Vector3( 0, 0, z );
      Vector3 vp_tr = new Vector3( 1, 1, z );

      Vector3 world_bl = mc.ViewportToWorldPoint( vp_bl );
      Vector3 world_tr = mc.ViewportToWorldPoint( vp_tr );


      float width = world_tr.x - world_bl.x;
      float height = world_tr.y - world_bl.y;

      Vector2 pos = mc.transform.position;
      Vector2 dim = new Vector2( width, height );

      return new Bounds( pos, dim );
   }


   // Extension for Camera
   public static Vector2 GetMouseScreenPosition2D( this Camera cam )
   {
      if (cam != null) {
         Vector2 mp = Input.mousePosition;

         // figure out the point on the plane
         Vector3 pos = new Vector3( mp.x, mp.y, -Camera.main.transform.position.z ); 
         Vector3 screen = cam.ScreenToWorldPoint( pos );
         return screen;
      }

      return Vector2.zero;
   }
}

