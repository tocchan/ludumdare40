using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUtil 
{
   private static bool s_loadingScene = false; 
   private static MonoBehaviour SceneUtilObject = null;

   private static MonoBehaviour GetOrCreateSceneUtilObject()
   {
      if (SceneUtilObject == null) {
         GameObject obj = new GameObject(); 
         obj.name = "__SceneUtilObject";
         GameObject.DontDestroyOnLoad(obj); 

         SceneUtilObject = obj.AddComponent<MonoBehaviour>();
      }
      return SceneUtilObject; 
   }

   public static void LoadLevel( string level_name ) 
   {
      // don't allow a double load
      if (s_loadingScene) { 
         return; 
      }

      s_loadingScene = true; 

      MonoBehaviour obj = GetOrCreateSceneUtilObject();  
      obj.StartCoroutine( LoadLevelWithFade( level_name, .3f, .3f ) ); 

      // UnityEngine.SceneManagement.SceneManager.LoadScene( level_name ); 
      // s_loadingScene = false ;
   }

   static IEnumerator LoadLevelWithFade( string level_name, float fadeOutTime, float fadeInTime )
   {
      Fader.FadeOut( Color.black, fadeOutTime ); 
      yield return new WaitForSeconds( fadeOutTime ); 

      // load, and wait a frame for it it load. 
      UnityEngine.SceneManagement.SceneManager.LoadScene( level_name ); 
      yield return null; 
      
      Fader.FadeIn( fadeInTime ); 
      yield return new WaitForSeconds( fadeInTime ); 
      
      s_loadingScene = false; 
   }
}
