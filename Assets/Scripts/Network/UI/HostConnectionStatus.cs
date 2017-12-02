using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostConnectionStatus : MonoBehaviour
{
   public Text AddressText;
   public Text ConnectionCountText;

   // Use this for initialization
   void Start()
   {
      AddressText.text = "Hosting: " + Network.player.ipAddress;
      ConnectionCountText.text = "Players: " + HopperNetwork.GetConnectionCount();

      HopperNetwork.Instance.OnPlayerJoin += OnConnectionChanged;
      HopperNetwork.Instance.OnPlayerLeave += OnConnectionChanged;
   }

   void OnConnectionChanged( NetworkConnection conn )
   {
      ConnectionCountText.text = "Players: " + HopperNetwork.GetConnectionCount();
   }
}

