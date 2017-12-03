using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

[RequireComponent(typeof(HopperDiscover))]
public class HopperNetwork : NetworkManager
{
   //-------------------------------------------------------------------
   public enum eState
   {
      DISCONNECTED,
      HOSTING,
      CLIENT_LOOKING,
      CLIENT_JOINED,
      CLIENT_READY, 
   };

   //-------------------------------------------------------------------
   public delegate void DOnPlayerJoin( VirtualNetworkController conn );
   public delegate void DOnPlayerLeave( VirtualNetworkController conn ); 

   //-------------------------------------------------------------------
   public bool IsHostScene = false; 
   public HopperDiscover Discovery; 
   public DOnPlayerJoin OnPlayerJoin;
   public DOnPlayerLeave OnPlayerLeave; 

   //-------------------------------------------------------------------
   public void Start()
   {
      Discovery = GetComponent<HopperDiscover>();

      Debug.Log( GetLocalAddress() ); 
      networkAddress = GetLocalAddress(); 

      StartConnecting();
   }

   //-------------------------------------------------------------------
   void StartConnecting()
   {
      Discovery.Initialize();
      if (IsHostScene) {
         if (StartServer()) {
            Debug.Log( "Starting to Host." );
            Discovery.StartAsServer(); 
         } 
      } else {
         Discovery.StartAsClient();
      }
   }

   //-------------------------------------------------------------------
   public void Update()
   {
      if (Discovery.IsRunning()) {
         if (Discovery.FoundAddress) {
            Discovery.Stop();
            StartClient();
         }
      } else if (!isNetworkActive) {
         StartConnecting();
      }
   }

   //-------------------------------------------------------------------
   public override void OnServerConnect( NetworkConnection conn )
   {
      Debug.Log( "Client Connected to Server: " + conn.address ); 
      base.OnServerConnect(conn); 
   }

   //-------------------------------------------------------------------
   public override void OnServerReady( NetworkConnection conn )
   {
      Debug.Log( "Client is Ready: " + conn.address ); 
      base.OnServerReady(conn); 
   }

   //-------------------------------------------------------------------
   public override void OnServerDisconnect(NetworkConnection conn)
   {
      base.OnServerDisconnect(conn);
   }

   //-------------------------------------------------------------------
   public override void OnClientConnect( NetworkConnection conn )
   {
      Debug.Log( "Client connected successful" ); 
      base.OnClientConnect(conn); 
   }

   //-------------------------------------------------------------------
   public string GetLocalAddress()
   {
      return Network.player.ipAddress;
   }

   //-------------------------------------------------------------------
   public static bool IsHost()
   {
      NetworkManager mgr = NetworkManager.singleton;
      if ((mgr == null) || (!mgr.isNetworkActive)) {
         return false; 
      }

      return (NetworkServer.active); 
   }

   //-------------------------------------------------------------------
   public static NetworkConnection GetMyConnection()
   {
      NetworkClient client = NetworkManager.singleton.client; 
      if (client != null) {
         return client.connection; 
      }

      return null; 
   }

   //-------------------------------------------------------------------
   public static int GetConnectionCount()
   {
      NetworkManager mgr = NetworkManager.singleton;
      if (mgr == null) {
         return 0;
      } else {
         if (NetworkServer.connections.Count > 0) {
            return NetworkServer.connections.Count - 1; // don't count the server
         } else {
            return 0;
         }
      }
   }

   //-------------------------------------------------------------------
   public static VirtualNetworkController GetMyController()
   {
      NetworkConnection conn = GetMyConnection();
      if ((conn != null)  && (conn.playerControllers.Count > 0)) {
         return conn.playerControllers[0].gameObject.GetComponent<VirtualNetworkController>();
      }

      return null; 
   }

   //-------------------------------------------------------------------
   public static eState GetState()
   {
      HopperNetwork net = (HopperNetwork) NetworkManager.singleton;
      if (!net.isNetworkActive) {
         if (net.Discovery.IsRunning()) {
            return eState.CLIENT_LOOKING;
         } else {
            return eState.DISCONNECTED;
         }
      }

      if (IsHost()) {
         return eState.HOSTING;
      }

      NetworkConnection conn = GetMyConnection(); 
      if (conn == null) {
         return eState.CLIENT_LOOKING;
      } else {
         if (conn.playerControllers.Count > 0) {
            return eState.CLIENT_READY;
         } else {
            return eState.CLIENT_JOINED;
         }
      }
   }
   
   //-------------------------------------------------------------------
   public static int GetReadyCount()
   {
      int count = 0; 
      var objects = GameObject.FindObjectsOfType<VirtualNetworkController>(); 
      for (int i = 0; i < objects.Length; ++i) {
         if (objects[i].ClientIsReady) {
            ++count; 
         }
      }

      return count; 
   }

   //-------------------------------------------------------------------
   public static bool IsEveryoneReady()
   {
      var objects = GameObject.FindObjectsOfType<VirtualNetworkController>(); 
      for (int i = 0; i < objects.Length; ++i) {
         if (objects[i].ClientIsReady == false) {
            return false; 
         }
      }

      return true; 
   }

   //-------------------------------------------------------------------
   public static void StartGame()
   {
      var objects = GameObject.FindObjectsOfType<VirtualNetworkController>(); 
      for (int i = 0; i < objects.Length; ++i) {
         objects[i].RpcSetInGame(true); 
      }
   }

   //-------------------------------------------------------------------
   public static void EndGame()
   {
      var objects = GameObject.FindObjectsOfType<VirtualNetworkController>(); 
      for (int i = 0; i < objects.Length; ++i) {
         objects[i].RpcSetInGame(false); 
      }
   }

   //-------------------------------------------------------------------
   public static HopperNetwork Instance
   {
      get {
         return (HopperNetwork)NetworkManager.singleton;
      }
   }
}

