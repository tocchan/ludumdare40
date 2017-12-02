using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

//------------------------------------------------------
//------------------------------------------------------
public class HopperDiscover : NetworkDiscovery
{
   public readonly ushort GAME_PORT = 5239;
   public bool FoundAddress = false; 

   //------------------------------------------------------
   void Start()
   {
      Object.DontDestroyOnLoad(gameObject); 
   }

   //------------------------------------------------------
   private void OnDestroy()
   {
      Stop(); 
   }

   //------------------------------------------------------
   public void Discover()
   {
      Initialize(); 
      FoundAddress = false; 
      if (HopperNetwork.IsHost()) {
         Debug.Log( "Broadcast as Server" ); 
         StartAsServer(); 
      } else {
         StartAsClient();
      }
   }

   //------------------------------------------------------
   public override void OnReceivedBroadcast( string fromAddress, string data )
   {
      Debug.Log( "Broadcast: " + fromAddress ); 
      NetworkManager.singleton.networkAddress = fromAddress; 
      FoundAddress = true; 
   }

   //------------------------------------------------------
   public void Stop()
   {
      FoundAddress = false; 
      StopBroadcast(); 
      Debug.Log( "Stopping Broadcast" ); 
   }

   //------------------------------------------------------
   public bool IsRunning()
   {
      return isClient || isServer; 
   }
}

