using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------
public class MeshBuilder
{
   public List<Vector3> positions; 
   public List<Vector2> uvs; 
   public List<int> indices; 

   //------------------------------------------------------------------------------------
   public MeshBuilder()
   {
      positions = new List<Vector3>();
      uvs = new List<Vector2>(); 
      indices = new List<int>(); 
   }

   //------------------------------------------------------------------------------------
   public void AddQuad( Vector3 origin, 
      Vector3 Right, Vector3 Up, 
      float min_x, float max_x, 
      float min_y, float max_y, 
      Vector2 uv_min, 
      Vector2 uv_max )
   {
      // [2]---[3]
      //  |  /  |
      // [0]---[1]
      int start_index = positions.Count; 
      positions.Add( origin + min_x * Right + min_y * Up ); 
      uvs.Add( new Vector3( uv_min.x, uv_min.y ) ); 
      
      positions.Add( origin + max_x * Right + min_y * Up ); 
      uvs.Add( new Vector3( uv_max.x, uv_min.y ) ); 

      positions.Add( origin + min_x * Right + max_y * Up ); 
      uvs.Add( new Vector3( uv_min.x, uv_max.y ) ); 
      
      positions.Add( origin + max_x * Right + max_y * Up ); 
      uvs.Add( new Vector3( uv_max.x, uv_max.y ) );
      
      // Add the two triangles
      indices.AddRange( new int[] { start_index + 0, start_index + 2, start_index + 3 } );  
      indices.AddRange( new int[] { start_index + 0, start_index + 3, start_index + 1 } );  
   }

   //------------------------------------------------------------------------------------
   public void AddBlock( Vector3 center, 
      Rect top_uv, 
      Rect side_uv, 
      Rect bot_uv )
   {
      // Top
      AddQuad( center + new Vector3( 0.0f, .5f, 0.0f ), Vector3.right, Vector3.forward, 
         -.5f, .5f, 
         -.5f, .5f, 
         top_uv.min, 
         top_uv.max );  

      // bottom
      AddQuad( center + new Vector3( 0.0f, -.5f, 0.0f ), Vector3.right, Vector3.back, 
         -.5f, .5f, 
         -.5f, .5f, 
         bot_uv.min, 
         bot_uv.max );  

      // Sides
      // Right
      AddQuad( center + new Vector3( 0.5f, 0.0f, 0.0f ), Vector3.forward, Vector3.up, 
         -.5f, .5f, 
         -.5f, .5f, 
         side_uv.min, 
         side_uv.max ); 

      // left
      AddQuad( center + new Vector3( -0.5f, 0.0f, 0.0f ), Vector3.back, Vector3.up, 
         -.5f, .5f, 
         -.5f, .5f, 
         side_uv.min, 
         side_uv.max );
      
      // front
      AddQuad( center + new Vector3( 0.0f, 0.0f, 0.5f ), Vector3.left, Vector3.up, 
         -.5f, .5f, 
         -.5f, .5f, 
         side_uv.min, 
         side_uv.max );  

      // back
      AddQuad( center + new Vector3( 0.0f, 0.0f, -0.5f ), Vector3.right, Vector3.up, 
         -.5f, .5f, 
         -.5f, .5f, 
         side_uv.min, 
         side_uv.max ); 
   }

   //------------------------------------------------------------------------------------
   // This takes a tile location on the sprite sheet
   public void AddBlock( Vector3 center, 
      Vector2 top_sprite, 
      Vector2 side_sprite, 
      Vector2 bottom_sprite )
   {
      // honestly this should pass in bounds; 
      float tile_size = 1.0f / 32.0f; 
      float nudge_val = .5f * tile_size * tile_size;
      Vector2 nudge = new Vector2( nudge_val, nudge_val ); 
      Vector2 dim = new Vector2( tile_size, tile_size ) - nudge; 

      Vector2 top_uv_min = (top_sprite * tile_size) + nudge;  
      Vector2 side_uv_min = (side_sprite * tile_size) + nudge;  
      Vector2 bot_uv_min = (bottom_sprite * tile_size) + nudge;  

      AddBlock( center, 
         new Rect( top_uv_min, dim ), 
         new Rect( side_uv_min, dim ), 
         new Rect( bot_uv_min, dim ) ); 
   }

   //------------------------------------------------------------------------------------
   public void UpdateMesh( Mesh mesh )
   {
      mesh.vertices = positions.ToArray(); 
      mesh.SetUVs( 0, uvs ); 
      mesh.SetTriangles( indices.ToArray(), 0 ); 
      mesh.UploadMeshData(true); 
   }
}; 

