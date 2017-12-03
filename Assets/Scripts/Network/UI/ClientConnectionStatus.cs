using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientConnectionStatus : MonoBehaviour
{
   public Text StatusText; 
   public Text AddressText; 

   // Use this for initialization
   void Start()
   {
   }
   
   // Update is called once per frame
   void Update()
   {
      if (NetworkManager.singleton.isNetworkActive) {
         AddressText.gameObject.SetActive(true);
         AddressText.text = HopperNetwork.Instance.networkAddress + ":" + HopperNetwork.Instance.networkPort; 
      } else {
         AddressText.gameObject.SetActive(false);
      }

      HopperNetwork.eState state = HopperNetwork.GetState();
      switch (state) {
         case HopperNetwork.eState.DISCONNECTED: 
            StatusText.text = "DISCONNECTED";
            break;

         case HopperNetwork.eState.CLIENT_LOOKING:
            StatusText.text = "SEARCHING..."; 
            break;

         case HopperNetwork.eState.CLIENT_READY: 
            VirtualNetworkController me = HopperNetwork.GetMyController();
            if (me.ClientIsReady) {
               StatusText.text = "READY";
            } else {
               StatusText.text = "NOT READY"; 
            }
            break;

         default:
            StatusText.text = "UNKNOWN: " + state;
            break;
      }
   }
}

