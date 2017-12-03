using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAudioFinished : MonoBehaviour
{
   AudioSource WatchedClip; 

   void Start()
   {
      WatchedClip = GetComponent<AudioSource>(); 
   }

   void Update()
   {
      if (WatchedClip == null) {
         GameObject.Destroy(gameObject);
      }

      if (!WatchedClip.isPlaying) {
         GameObject.Destroy(gameObject);
      }
   }
}

