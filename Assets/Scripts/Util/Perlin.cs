using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin
{
   //----------------------------------------------------------------------------------
   // Changes Unity's from 0 to 1 to instead of -1.0f to 1.0f
   public static float Noise( Vector2 pos )
   {
      return Mathf.PerlinNoise( pos.x, pos.y ) * 2.0f - 1.0f; 
   }

   //----------------------------------------------------------------------------------
   public static float OctaveNoise( Vector2 pos, float frequency = 1.0f, float persistance = .5f, int steps = 1 )
   {
      if (steps == 0) {
         return 0.0f; 
      }

      float val = 0.0f; 
      float total_val = 0.0f; 
      for (int i = 0; i < steps; ++i) {
         total_val += persistance; 

         Vector2 p = pos * frequency; 
         val += persistance * Noise( p ); 

         // Go to the next layer
         persistance *= persistance;  // persistance keeps scaling itself down
         frequency *= 2.0f;           // frequency doubles - mostly to help with wrapping
      }

      val /= total_val; // renormalize
      return val; 
   }
};
