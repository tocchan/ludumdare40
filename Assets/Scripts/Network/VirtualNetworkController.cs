using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

[NetworkSettings( channel=1, sendInterval = 0.05f )]
public class VirtualNetworkController : NetworkBehaviour
{
   public Vector2 Movement; 
   
   public uint ActionCount; 
   private uint LastConsumedAction = 0;

   public bool ClientIsReady = false;
   public bool IsPresentInGame = false; 


   public void SetMovement( Vector2 v )
   {
      Movement = v; 
   }

   public void DoAction()
   {
      ++ActionCount; 
   }

   private bool HasAction()
   {
      // handle wrapping; 
      uint diff = ActionCount - LastConsumedAction; 
      return (diff < (uint.MaxValue / 2));
   }

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

   // Consume all button presses
   public bool ConsumeAllActions()
   {
      if (HasAction()) {
         LastConsumedAction = ActionCount;
         return true;
      }
      return false; 
   }

   [Command(channel=0)]
   public void CmdSetReady( bool ready )
   {
      ClientIsReady = ready; 
   }
   
   public bool IsInGame()
   {
      return IsPresentInGame; 
   }
}

