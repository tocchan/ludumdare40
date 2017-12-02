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
      if (ApplicationUtil.IsGame()) {
         Debug.Log( "Broadcast as Server" ); 
      } else {
         Debug.Log( "Listening as Client" );
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
}

