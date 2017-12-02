using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientConnectionStatus : MonoBehaviour
{
   public Text StatusText; 

   // Use this for initialization
   void Start()
   {
   }
   
   // Update is called once per frame
   void Update()
   {
      HopperNetwork.eState state = HopperNetwork.GetState();
      switch (state) {
         case HopperNetwork.eState.DISCONNECTED: 
            StatusText.text = "Disconnected";
            break;

         case HopperNetwork.eState.CLIENT_LOOKING:
            StatusText.text = "SEARCHING..."; 
            break;

         case HopperNetwork.eState.CLIENT_READY: 
            StatusText.text = "READY";
            break;

         default:
            StatusText.text = "UNKNOWN";
            break;
      }
   }
}

