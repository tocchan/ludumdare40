using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
   public RectTransform Origin;
   public RectTransform Knib; 

   public float MaxRadius = 100.0f; 
   public float InputWidth = 400.0f; 

   public int ActiveFingerID = -1; 

   //---------------------------------------------------------------------
   // Use this for initialization
   void Start()
   {
      
   }
   
   //---------------------------------------------------------------------
   void UpdateKnib( Vector2 pos )
   {
      Vector2 localPos; 
      if (!RectTransformUtility.ScreenPointToLocalPointInRectangle( (RectTransform)transform, pos, Camera.main, out localPos )) {
         return; 
      }

      Vector2 offset = localPos - new Vector2(Origin.transform.localPosition.x, Origin.transform.localPosition.y);
      float length = offset.magnitude; 
      Vector2 dir = Vector2.zero; 
      if (length > .1f) {
         dir = offset / length; 
         length = Mathf.Clamp( length, 0.0f, MaxRadius );
         offset = dir * length; 
      } 

      Knib.transform.localPosition = Origin.localPosition + new Vector3( offset.x, offset.y, 0.0f ); 

      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller != null) {
         controller.SetMovement( dir * (length / MaxRadius) );
      }
   }

   //---------------------------------------------------------------------
   bool InMoveArea( Vector2 pos )
   {
      float nx = pos.x / Camera.main.pixelWidth;
      return nx < .5f; 
   }

   //---------------------------------------------------------------------
   bool InActionArea( Vector2 pos )
   {
      float nx = pos.x / Camera.main.pixelWidth;
      return nx > .5f;
   }

   //---------------------------------------------------------------------
   void UpdateAction( Touch touch )
   {
      Debug.Log( "ACTION" ); 
      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller != null) {
         controller.DoAction();
      }
   }

   //---------------------------------------------------------------------
   void ProcessTouch( Touch touch )
   {
      if (ActiveFingerID == touch.fingerId) {
         if ((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled)) {
            ActiveFingerID = -1;
            VirtualNetworkController controller = HopperNetwork.GetMyController(); 
            if (controller != null) {
               controller.SetMovement( Vector2.zero );  
            }
            Knib.transform.localPosition = Origin.transform.localPosition;
         } else if (touch.phase == TouchPhase.Moved) {
            UpdateKnib( touch.position ); 
         }
         return; 
      }

      if (touch.phase == TouchPhase.Began) {
         if ((ActiveFingerID == -1) && InMoveArea(touch.position)) {
            // Okay, starting a touch;
            ActiveFingerID = touch.fingerId; 
            UpdateKnib( touch.position ); 
         } else if (InActionArea(touch.position)) {
            UpdateAction(touch); 
         }
      }
   }

   //---------------------------------------------------------------------
   // Update is called once per frame
   void Update()
   {
      if (!Input.touchSupported) {
         // fake touch
         if (Input.GetMouseButton(0)) {
            Touch touch = new Touch(); 
            touch.fingerId = 1; 
            if (ActiveFingerID == -1) {
               touch.phase = TouchPhase.Began;
            } else {
               touch.phase = TouchPhase.Moved; 
            }

            touch.position = Input.mousePosition;
            ProcessTouch(touch); 
         } else if (ActiveFingerID != -1) {
            Touch touch = new Touch(); 
            touch.phase = TouchPhase.Ended; 
            touch.fingerId = 1; 
            touch.position = Input.mousePosition;
            ProcessTouch(touch);
         }
      }

      if (Input.touches.Length > 0) {
         for (int i = 0; i < Input.touches.Length; ++i) {
            ProcessTouch( Input.touches[i] ); 
         }
      }
   }
}
