using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientSceneController : MonoBehaviour
{
   //-------------------------------------------------------------------
   public enum eClientState
   {
      DISCOVER, 
      LOBBY, 
      IN_GAME,  
   }

   //-------------------------------------------------------------------
   public GameObject Title;
   public GameObject HopperUI; 
   public Image ActionImage; 
   public Text DebugText; 

   public Sprite ReadyBunny;
   public Sprite SleepyBunny;
   public Sprite BunnyAction;
   public Sprite WolfAction; 

   public bool IsReady = false; 
   public VirtualNetworkController Controller; 

   public eClientState CurrentState = eClientState.DISCOVER;

   //-------------------------------------------------------------------
   public void Start()
   {
      SetState( eClientState.DISCOVER ); 
   }

   //-------------------------------------------------------------------
   public void Update()
   {
      if ((CurrentState != eClientState.DISCOVER) && (!NetworkManager.singleton.isNetworkActive)) {
         SetState( eClientState.DISCOVER ); 
      }

      switch (CurrentState) {
         case eClientState.DISCOVER:
            DebugText.gameObject.SetActive(false); 
            UpdateDiscovery(); 
            break;

         case eClientState.LOBBY:
            UpdateDebugText(); 
            UpdateLobby();
            break;

         case eClientState.IN_GAME:
            UpdateDebugText(); 
            UpdateInGame();
            break;

         default:
            break;
      }
   }

   //-------------------------------------------------------------------
   public void SetState( eClientState state )
   {
      HopperUI.SetActive(false); 

      CurrentState = state; 
      switch (state) {
         case eClientState.LOBBY:
         case eClientState.IN_GAME:
            HopperUI.SetActive(true); 
            break;
      }
   }

   //-------------------------------------------------------------------
   public void UpdateDiscovery()
   {
      var state = HopperNetwork.GetState();
      if (state == HopperNetwork.eState.CLIENT_READY) {
         Controller = HopperNetwork.GetMyController(); 
         SetState( eClientState.LOBBY );
      }
   }

   //-------------------------------------------------------------------
   public void UpdateLobby()
   {
      // this state, just wait around until server tells us to move on
      VirtualNetworkController control = HopperNetwork.GetMyController(); 
      if (control.ConsumeAllActions()) {
         ToggleReady(); 
      }

      if (Controller.IsInGame()) {
         SetState( eClientState.IN_GAME );
      }

      if (IsReady) {
         ActionImage.sprite = ReadyBunny;
      } else {
         ActionImage.sprite = SleepyBunny;
      }
   }

   //-------------------------------------------------------------------
   public void ToggleReady()
   {
      if (CurrentState != eClientState.LOBBY) {
         return;
      }

      IsReady = !IsReady; 
      Controller.SetReady(IsReady);  
   }

   //-------------------------------------------------------------------
   public void UpdateInGame()
   {
      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller.IsWolf) {
         ActionImage.sprite = WolfAction;
      } else {
         ActionImage.sprite = BunnyAction; 
      }
   }

   //-------------------------------------------------------------------
   public void UpdateDebugText()
   {
      if (DebugText == null) {
         return;
      }

      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller == null) {
         DebugText.text = "No Controller"; 
         return; 
      }

      string str = "";
      str += "Ready: " + (controller.ClientIsReady ? "Y" : "N") + "\n";
      str += "Game: " + (controller.IsInGame() ? "Y" : "N") + "\n";
      str += "Wolf: " + (controller.IsWolf ? "Y" : "N") + "\n";
      DebugText.text = str; 
   }
}

