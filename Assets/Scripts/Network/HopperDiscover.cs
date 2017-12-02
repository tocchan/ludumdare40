using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

//------------------------------------------------------
//------------------------------------------------------
public class HopperDiscover : NetworkDiscovery
{
   public readonly ushort GAME_PORT = 5239;

   //------------------------------------------------------
   void Start()
   {
      Object.DontDestroyOnLoad(gameObject); 
   }

   //------------------------------------------------------
   public void Discover()
   {
      Initialize(); 
      if (HopperNetwork.IsHost()) {
         Debug.Log( "Broadcast as Server" ); 
         StartAsServer(); 
      } else {
         StartAsClient();
      }
   }

   //------------------------------------------------------
   public override void OnReceivedBroadcast(string fromAddress, string data)
   {
      if (NetworkManager.singleton != null) {
         NetworkManager.singleton.StartClient();
      }
   }

   //------------------------------------------------------
   public void Stop()
   {
      StopBroadcast(); 
      Debug.Log( "Stopping Broadcast" ); 
   }

   public bool IsRunning()
   {
      return isClient || isServer; 
   }
}

