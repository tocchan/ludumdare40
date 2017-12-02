using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking;

public class NetworkTest : MonoBehaviour {

   public Text HostText;
   public Text AddressText; 
   public Text BroadcastText; 

   public HopperDiscover Discoverer; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update() 
   {
	   if (NetworkManager.singleton == null) {
         SetStateText( false, false ); 
      } else {
         NetworkManager mgr = NetworkManager.singleton;
         bool isActive = mgr.isNetworkActive;
         bool isServer = HopperNetwork.IsHost(); 
         SetStateText( isActive, isServer ); 

         SetHostText( mgr.networkAddress.ToString(), mgr.networkPort ); 
         SetBroadcasting( Discoverer.isServer || Discoverer.isClient ); 
      }

      if (Discoverer.FoundAddress) {
         Discoverer.Stop(); 
         NetworkManager.singleton.StartClient(); 
      }
	}

   void SetStateText( bool isActive, bool isServer )
   {
      AddressText.gameObject.SetActive(isActive);
      // BroadcastText.gameObject.SetActive(isActive); 

      if (isActive) {
         HostText.text = isServer ? " HOST" : "CLIENT"; 
      } else {
         HostText.text = "Disconnected"; 
      }
   }

   void SetHostText( string addr, int port )
   {
      if (string.IsNullOrEmpty(addr)) {
         AddressText.text = "Address: None";
      } else {
         AddressText.text = "Address: " + addr + ":" + port; 
      }
   }

   void SetBroadcasting( bool broadcast )
   {
      BroadcastText.text = "Broadcasting: " + (broadcast ? "Yes" : " No"); 
   }

   public void Host()
   {
      if (NetworkManager.singleton != null) {
         NetworkManager.singleton.StartServer(); 
      }
   }

   public void Discover()
   {
      Debug.Log( "Discover" ); 
      if (Discoverer.IsRunning()) {
         Discoverer.Stop(); 
      } else {
         Discoverer.Discover(); 
      }
   }

   public void Join()
   {
      Debug.Log( "Trying to Join" ); 
      NetworkManager.singleton.networkAddress = "::ffff:192.168.0.27"; 
      NetworkManager.singleton.StartClient();
   }


}
