using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickRandomSprite : MonoBehaviour
{
   public Sprite[] Sprites = new Sprite[0]; 
   private SpriteRenderer m_renderer; 

   // Use this for initialization
   void Start()
   {
      m_renderer = GetComponent<SpriteRenderer>(); 
      if (Sprites.Length > 0) {
         int idx = Random.Range( 0, Sprites.Length ); 
         Sprite spr = Sprites[idx];
         m_renderer.sprite = spr; 
      }
   }
}

