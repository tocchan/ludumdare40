using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public static class BuildScripts 
{
   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Android Controller")]
   static void MakeAndroidController()
   {
      BuildController( "/android/hopcontrol.apk", BuildTarget.Android ); 
   }

      //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Windows Controller")]
   static void MakeWindowsController()
   {
      BuildController( "/win32/hopcontrol.exe", BuildTarget.StandaloneWindows ); 
   }


   //---------------------------------------------------------------------------------------------------------------------
   static void BuildController( string path, BuildTarget target )
   {
      string root = "Builds/";

      UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
      string[] levels = { "Assets/Scenes/Controller.unity" }; 
      if (string.IsNullOrEmpty(levels[0])) {
         Debug.LogError( "No scene loaded. " ); 
         return; 
      }
      BuildPipeline.BuildPlayer( levels, root + path, target, BuildOptions.AutoRunPlayer | BuildOptions.Development ); 
   }


   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Game")]
   static void MakeGameBuild()
   {
      string root = "Builds/";

      UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
      string[] levels = { "Assets/Scenes/Game.unity" }; 
      if (string.IsNullOrEmpty(levels[0])) {
         Debug.LogError( "No scene loaded. " ); 
         return; 
      }
      BuildPipeline.BuildPlayer( levels, root + "game/hophop.exe", BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer | BuildOptions.Development ); 
   }
}
