using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffect : MonoBehaviour
{
   public Shader m_shader; 
   private Material m_material; 

   // Use this for initialization
   void Start()
   {
      if (m_shader != null) {
         m_material = new Material(m_shader); 
      }
   }

   private void OnRenderImage( RenderTexture source, RenderTexture destination )
   {
      if (m_material != null) {
         Graphics.Blit( source, destination, m_material ); 
      }
   }
}
