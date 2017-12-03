using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostConnectionStatus : MonoBehaviour
{
   public Text AddressText;
   public Text ConnectionCountText;
   public Text ConnectionReadyText; 

   //-------------------------------------------------------------------
   // Use this for initialization
   void Start()
   {
      AddressText.text = "Hosting: " + Network.player.ipAddress;
      ConnectionCountText.text = "Players: " + GameObject.FindObjectsOfType<VirtualNetworkController>().Length;

      HopperNetwork.Instance.OnPlayerJoin += ConnectionsChanged;
      HopperNetwork.Instance.OnPlayerLeave += ConnectionsChanged;

      VirtualNetworkController.OnPlayerReady += ReadyCountChanged;
      VirtualNetworkController.OnPlayerUnready += ReadyCountChanged; 

   }

   //-------------------------------------------------------------------
   void ConnectionsChanged( VirtualNetworkController conn )
   {
      if (ConnectionCountText != null) {
         ConnectionCountText.text = "Players: " + GameObject.FindObjectsOfType<VirtualNetworkController>().Length;
      }
   }

   //-------------------------------------------------------------------
   void ReadyCountChanged( VirtualNetworkController ctrl )
   {
      if (null != ConnectionReadyText) {
         ConnectionReadyText.text = "Ready: " + HopperNetwork.GetReadyCount();
      }
   }
}

