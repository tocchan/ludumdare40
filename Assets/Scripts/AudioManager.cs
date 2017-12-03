using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-------------------------------------------------------------------
//-------------------------------------------------------------------
public enum eSoundType
{
   BUNNY_BUMP = 0, 
   BUNNY_SCREAM,
   BUNNY_SLEEP, 
   BUNNY_DEATH, 
   BUNNY_LAUGH, 
   BUNNY_SEXY_TIMES, 
   BUNNY_SCARED, 
   FOX_BITE, 
   FOX_EAT, 
   FOX_ANGRY, 
   FOX_HAPPY, 
   FOX_EXHAUSTED, 
   FOX_WORKOUT, 
   FOX_HOWL, 
   POOF,

   SOUND_TYPE_COUNT, 
};

//-------------------------------------------------------------------
//-------------------------------------------------------------------
[System.Serializable]
public class AudioGroup
{
   public eSoundType Type;
   public AudioClip[] Clips; 
}

//-------------------------------------------------------------------
//-------------------------------------------------------------------
public class AudioManager : MonoSingleton<AudioManager>
{

   //-------------------------------------------------------------------
   public List<AudioGroup> Groups = new List<AudioGroup>(); 
   private List<List<AudioClip>> ClipBuckets;

   //-------------------------------------------------------------------
   void Start()
   {
      ClipBuckets = new List<List<AudioClip>>(); 
      int max = (int)eSoundType.SOUND_TYPE_COUNT;

      for (int i = 0; i < max; ++i) {
         ClipBuckets.Add( new List<AudioClip>() );
      }

      for (int i = 0; i < Groups.Count; ++i) {
         AudioGroup group = Groups[i];
         int idx = (int)group.Type;
         ClipBuckets[idx].AddRange( group.Clips );
      }
   }

   //-------------------------------------------------------------------
   public void PlayClip( AudioClip clip )
   {
      PlayClipOn( clip, null ); 
   }

   //-------------------------------------------------------------------
   public void PlayClipOn( AudioClip clip, Transform parent )
   {
      GameObject go = new GameObject(); 
      go.transform.parent = parent; 
      go.name = "Clip: " + clip.name; 
      AudioSource source = go.AddComponent<AudioSource>();
      go.AddComponent<DestroyOnAudioFinished>(); 

      source.clip = clip; 
      source.Play();  
   }

   //-------------------------------------------------------------------
   public void PlayInternal( eSoundType type, Transform parent )
   {
      int idx = (int)type;
      List<AudioClip> clips = ClipBuckets[idx];
      if (clips.Count == 0) {
         return; 
      }

      int audioIdx = Random.Range( 0, clips.Count );
      PlayClipOn( clips[audioIdx], parent );  
   }


   //-------------------------------------------------------------------
   public static void Play( eSoundType type )
   {
      GetInstance().PlayInternal( type, null ); 
   }

   //-------------------------------------------------------------------
   public static void PlayOn( eSoundType type, Transform parent )
   {
      GetInstance().PlayInternal( type, parent ); 
   }
   
}

