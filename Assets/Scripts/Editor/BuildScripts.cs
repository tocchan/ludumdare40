using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public static class BuildScripts 
{
   [MenuItem("ClayByte/Build Android Controller")]
   static void MakeCustomBuild()
   {
      string path = "Builds/hopcontroller.apk";

      UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
      string[] levels = { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path }; 
      if (string.IsNullOrEmpty(levels[0])) {
         Debug.LogError( "No scene loaded. " ); 
         return; 
      }
      BuildPipeline.BuildPlayer( levels, path + "/debug_build.apk", BuildTarget.Android, BuildOptions.AutoRunPlayer | BuildOptions.Development ); 
   }
}
