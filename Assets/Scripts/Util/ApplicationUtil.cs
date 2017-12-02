﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationUtil
{
   //----------------------------------------------------------------
   public static void Quit()
   {
      #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false; 
      #else
         Application.Quit(); 
      #endif
   }

   //----------------------------------------------------------------
   public static bool IsGame()
   {
      return (Application.platform == RuntimePlatform.WindowsPlayer)
         || (Application.platform == RuntimePlatform.WindowsEditor); 
   }

   //----------------------------------------------------------------
   public static bool IsController()
   {
      return (Application.platform == RuntimePlatform.Android)
         || (Application.platform == RuntimePlatform.IPhonePlayer);
   }
}
