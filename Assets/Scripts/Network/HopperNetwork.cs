using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class HopperNetwork : NetworkManager
{
   public void Start()
   {
      if (ApplicationUtil.IsGame()) {
         if (StartServer()) {
            Debug.Log( "Starting to Host." );
         } else {
            Debug.Log( "Failed to start Host." );
         }
      } else {
         // do nothing, wait until broadcast
      }
   }

   public override void OnServerConnect( NetworkConnection conn )
   {
      Debug.Log( "Client Connected to Server: " + conn.address ); 
   }

   public override void OnClientConnect( NetworkConnection conn )
   {
      Debug.Log( "Client connected successful" ); 
   }
}

