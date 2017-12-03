using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public static class BuildScripts 
{
   static string WIN_GAME_PATH = "win32/game/bunnybumper.exe";
   static string WIN_CONTROL_PATH = "win32/control/bbcontrol.exe";
   static string ANDROID_CONTROL_PATH = "android/control/bbcontrol.apk";

   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Android Controller")]
   static void MakeAndroidController()
   {
      BuildController( ANDROID_CONTROL_PATH, BuildTarget.Android, BuildOptions.AutoRunPlayer | BuildOptions.Development ); 
   }

      //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Windows Controller")]
   static void MakeWindowsController()
   {
      BuildController( WIN_CONTROL_PATH, BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer | BuildOptions.Development ); 
   }


   //---------------------------------------------------------------------------------------------------------------------
   static void BuildController( string path, BuildTarget target, BuildOptions options )
   {
      string root = "Builds/";

      UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
      string[] levels = { "Assets/Scenes/Controller.unity" }; 
      if (string.IsNullOrEmpty(levels[0])) {
         Debug.LogError( "No scene loaded. " ); 
         return; 
      }
      BuildPipeline.BuildPlayer( levels, root + path, target, options ); 
   }


   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Run Game")]
   static void RunGam()
   {
      BuildGame( WIN_GAME_PATH, BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer | BuildOptions.Development );
   }

   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Build Game")]
   static void BuildGame()
   {
      BuildGame( WIN_GAME_PATH, BuildTarget.StandaloneWindows, BuildOptions.Development );
   }

   //---------------------------------------------------------------------------------------------------------------------
   [MenuItem("ClayByte/Build All")]
   static void BuildAll()
   {
      BuildGame( WIN_GAME_PATH, BuildTarget.StandaloneWindows, BuildOptions.Development );
      BuildController( WIN_CONTROL_PATH, BuildTarget.StandaloneWindows, BuildOptions.Development ); 
      BuildController( ANDROID_CONTROL_PATH, BuildTarget.Android, BuildOptions.Development ); 
   }

   //---------------------------------------------------------------------------------------------------------------------
   static void BuildGame( string path, BuildTarget target, BuildOptions options )
   {
      string root = "Builds/";

      UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
      string[] levels = { "Assets/Scenes/Game.unity" }; 
      if (string.IsNullOrEmpty(levels[0])) {
         Debug.LogError( "No scene loaded. " ); 
         return; 
      }
      BuildPipeline.BuildPlayer( levels, root + path, target, options ); 
   }
}
