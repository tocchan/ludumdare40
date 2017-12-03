using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientSceneController : MonoBehaviour
{
   public enum eClientState
   {
      DISCOVER, 
      LOBBY, 
      IN_GAME,  
   }

   public GameObject Title;
   public GameObject ReadyUpUI;
   public GameObject WolfUI; 
   public GameObject HopperUI; 

   public bool IsReady = false; 
   public VirtualNetworkController Controller; 

   public eClientState CurrentState = eClientState.DISCOVER;

   public void Start()
   {
      SetState( eClientState.DISCOVER ); 
   }

   public void Update()
   {
      if ((CurrentState != eClientState.DISCOVER) && (!NetworkManager.singleton.isNetworkActive)) {
         SetState( eClientState.DISCOVER ); 
      }

      switch (CurrentState) {
         case eClientState.DISCOVER:
            UpdateDiscovery(); 
            break;

         case eClientState.LOBBY:
            UpdateLobby();
            break;

         case eClientState.IN_GAME:
            UpdateInGame();
            break;

         default:
            break;
      }
   }

   public void SetState( eClientState state )
   {
      ReadyUpUI.SetActive(false);
      // WolfUI.SetActive(false);
      // HopperUI.SetActive(false); 

      CurrentState = state; 
      switch (state) {
         case eClientState.LOBBY: 
            ReadyUpUI.SetActive(true); 
            break;
      }
   }

   public void UpdateDiscovery()
   {
      var state = HopperNetwork.GetState();
      if (state == HopperNetwork.eState.CLIENT_READY) {
         Controller = HopperNetwork.GetMyController(); 
         SetState( eClientState.LOBBY );
      }
   }

   public void UpdateLobby()
   {
      // this state, just wait around until server tells us to move on
      if (Controller.IsInGame()) {
         SetState( eClientState.IN_GAME );
      }
   }

   public void ToggleReady()
   {
      if (CurrentState != eClientState.LOBBY) {
         return;
      }

      Debug.Log( "GOT'EM" ); 
      IsReady = !IsReady; 
      Controller.CmdSetReady(IsReady); 
      // do something else; 
   }

   public void UpdateInGame()
   {
   }
}

