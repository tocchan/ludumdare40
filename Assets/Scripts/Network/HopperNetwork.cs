using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class HopperNetwork : NetworkManager
{
   public void Start()
   {
      Debug.Log( GetLocalAddress() ); 
      networkAddress = GetLocalAddress(); 

      /*
      if (ApplicationUtil.IsGame()) {
         if (StartServer()) {
            Debug.Log( "Starting to Host." );
         } else {
            Debug.Log( "Failed to start Host." );
         }
      } else {
         // do nothing, wait until broadcast
      }
      */
   }

   public override void OnServerConnect( NetworkConnection conn )
   {
      Debug.Log( "Client Connected to Server: " + conn.address ); 
      base.OnServerConnect(conn); 
   }

   public override void OnServerReady( NetworkConnection conn )
   {
      Debug.Log( "Client is Ready: " + conn.address ); 
      base.OnServerReady(conn); 

   }

   public override void OnClientConnect( NetworkConnection conn )
   {
      Debug.Log( "Client connected successful" ); 
      base.OnClientConnect(conn); 
   }

   public string GetLocalAddress()
   {
      return Network.player.ipAddress;
   }

   public static bool IsHost()
   {
      NetworkManager mgr = NetworkManager.singleton;
      if ((mgr == null) || (!mgr.isNetworkActive)) {
         return false; 
      }

      return (NetworkServer.active); 
   }
}

