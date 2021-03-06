﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

[NetworkSettings( channel=1, sendInterval = 0.05f )]
public class VirtualNetworkController : NetworkBehaviour
{
   public delegate void DOnPlayerReady( VirtualNetworkController conn );
   public delegate void DOnPlayerUnready( VirtualNetworkController conn ); 

   public Vector2 Movement; 
   
   public uint ActionCount; 
   private uint LastConsumedAction = 0;

   public bool ClientIsReady = false;
   public bool IsPresentInGame = false;
   public bool IsDead = false; 
   public bool IsWolf = false; 

   public static DOnPlayerReady OnPlayerReady;
   public static DOnPlayerUnready OnPlayerUnready; 

   //----------------------------------------------------------------------
   private void Start()
   {
      if (HopperNetwork.Instance.OnPlayerJoin != null) {
         HopperNetwork.Instance.OnPlayerJoin(this); 
      }
   }

   //----------------------------------------------------------------------
   private void OnDestroy()
   {
      if (ClientIsReady) {
         if (OnPlayerUnready != null) {
            OnPlayerUnready(this);
         }
      }
      if (HopperNetwork.Instance.OnPlayerLeave != null) {
         HopperNetwork.Instance.OnPlayerLeave(this); 
      }
   }

   //----------------------------------------------------------------------
   public void SetMovement( Vector2 v )
   {
      Movement = v; 
      if (HopperNetwork.IsClient()) {
         CmdMovement( v ); 
      }
   }
   
   //----------------------------------------------------------------------
   public void DoAction()
   {
      ++ActionCount; 
      if (HopperNetwork.IsClient()) {
         CmdAction(); 
      }
   }

   //----------------------------------------------------------------------
   private bool HasAction()
   {
      // handle wrapping; 
      uint diff = ActionCount - LastConsumedAction; 
      return (diff > 0U) && (diff < (uint.MaxValue / 2));
   }

   //----------------------------------------------------------------------
   // Consume a single button press; 
   // may want to store timing eventually with this?
   public bool ConsumeAction()
   {
      // this is to handle wrapping;
      if (HasAction()) {
         ++LastConsumedAction;
         return true;
      } 
      return false; 
   }

   //----------------------------------------------------------------------
   // Consume all button presses
   public bool ConsumeAllActions()
   {
      if (HasAction()) {
         LastConsumedAction = ActionCount;
         return true;
      }
      return false; 
   }
   
   //----------------------------------------------------------------------
   public bool IsInGame()
   {
      return IsPresentInGame; 
   }

   
   //----------------------------------------------------------------------
   private void SetReadyInternal( bool ready )
   {
      if (ClientIsReady != ready) {
         ClientIsReady = ready; 
         if (ready) {
            if (OnPlayerReady != null) {
               OnPlayerReady(this);
            }
         } else {
            if (OnPlayerUnready != null) {
               OnPlayerUnready(this);
            }
         }
      }
   }

   //----------------------------------------------------------------------
   public void SetReady( bool ready )
   {
      SetReadyInternal(ready); 
      if (HopperNetwork.IsClient()) {
         CmdSetReady(ready);
      }
   }

   //----------------------------------------------------------------------
   public void SetWolf()
   {
      IsWolf = true;
      RpcSetWolf(); 
   }

   //----------------------------------------------------------------------
   public void SetInGame( bool inGame )
   {
      IsPresentInGame = inGame;
      if (!inGame) {
         ConsumeAllActions(); 
         SetReady(false); 
         IsWolf = false; 
         IsDead = false; 
      }

      RpcSetInGame(inGame);
   }

   //----------------------------------------------------------------------
   public void SetDead()
   {
      IsDead = true; 
      RpcSetDead(); 
   }

   //----------------------------------------------------------------------
   public void PlaySoundOnPhone( eSoundType type )
   {
      RpcPlaySound(type);
   }

   //----------------------------------------------------------------------
   // Commands
   //----------------------------------------------------------------------
   //----------------------------------------------------------------------
   [Command (channel=1)]
   void CmdMovement( Vector2 v )
   {
      Movement = v; 
   }
   
   //----------------------------------------------------------------------
   [Command (channel=0)]
   void CmdAction()
   {
      ++ActionCount; 
   }

   //----------------------------------------------------------------------
   [Command(channel=0)]
   public void CmdSetReady( bool ready )
   {
      SetReadyInternal(ready);
   }

   //----------------------------------------------------------------------
   // RPC
   //----------------------------------------------------------------------
   //----------------------------------------------------------------------
   [ClientRpc(channel = 0)]
   public void RpcSetDead()
   {
      IsDead = true; 
   }

   //----------------------------------------------------------------------
   [ClientRpc(channel = 0)]
   public void RpcSetWolf()
   {
      IsWolf = true; 
   }

   //----------------------------------------------------------------------
   [ClientRpc(channel = 0)]
   public void RpcSetInGame( bool inGame )
   {
      IsPresentInGame = inGame;
      ConsumeAllActions(); // get rid of all actions;  
      if (!inGame) {
         SetReady(false);
         IsWolf = false; 
         IsDead = false; 
      }
   }

   //----------------------------------------------------------------------
   [ClientRpc(channel = 0)]
   public void RpcPlaySound( eSoundType type )
   {
      AudioManager.Play(type);
   }
}

