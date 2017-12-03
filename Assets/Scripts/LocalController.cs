using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalController : MonoBehaviour 
{
   public int GamepadID = -1; 

   VirtualNetworkController VirtualController; 

   public float X; 
   public float Y; 
   public bool Action;

   //-------------------------------------------------------------------
   private void Start()
   {
      VirtualController = GetComponent<VirtualNetworkController>(); 
   }

   //-------------------------------------------------------------------
   void Update()
   {
      if (GamepadID < 0) {
         return; 
      }

      X = Input.GetAxis( "Horizontal" + GamepadID ); 
      Y = Input.GetAxis( "Vertical" + GamepadID ); 
      Action = Input.GetKeyDown( GetActionKey(GamepadID) ); 

      VirtualController.SetMovement( new Vector2(X, Y).normalized ); 
      if (Action) {
         VirtualController.DoAction(); 

         if (GameManager.GetInstance().m_currentState == eGameState.WAIT_FOR_READY) {
            // toggle ready
            if (Action) {
               VirtualController.SetReady( !VirtualController.ClientIsReady ); 
            }
         }
      }
   }
      
   //-------------------------------------------------------------------
   public static KeyCode GetActionKey( int joy_idx )
   {
      switch (joy_idx) {
         case 0:
            return KeyCode.Joystick1Button0;
         case 1:
            return KeyCode.Joystick2Button0;
         case 2:
            return KeyCode.Joystick3Button0;
         case 3:
            return KeyCode.Joystick4Button0;
         default:
            return KeyCode.Joystick1Button0; 
      }
   }

   //-------------------------------------------------------------------
   public static KeyCode GetCancelKey( int joy_idx )
   {
      switch (joy_idx) {
         case 0:
            return KeyCode.Joystick1Button1;
         case 1:
            return KeyCode.Joystick2Button1;
         case 2:
            return KeyCode.Joystick3Button1;
         case 3:
            return KeyCode.Joystick4Button1;
         default:
            return KeyCode.Joystick1Button1; 
      }
   }
}
