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

   public Animator ActionAnimator; 

   //---------------------------------------------------------------------
   // Use this for initialization
   void Start()
   {
      
   }
   
   //---------------------------------------------------------------------
   void UpdateMovementKnib( Vector2 offset )
   {
      Vector3 fullOffset = MaxRadius * offset; 
      Knib.transform.localPosition = fullOffset; 
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
         Vector2 movement = dir * (length / MaxRadius); 
         UpdateMovementKnib( movement ); 
         controller.SetMovement( movement );
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
   void DoAction()
   {
      ActionAnimator.SetTrigger("DoAction");

      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller != null) {
         controller.DoAction();
         if (controller.IsInGame()) {
            if (controller.IsWolf) {
               AudioManager.Play(eSoundType.FOX_BITE);
            } else {
               AudioManager.Play(eSoundType.BUNNY_LAUGH);
            }
         }
      }
   }

   //---------------------------------------------------------------------
   void UpdateAction( Touch touch )
   {
      VirtualNetworkController controller = HopperNetwork.GetMyController(); 
      if (controller != null) {
         DoAction(); 
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

   private bool HackActionDown = false; 

   //---------------------------------------------------------------------
   // Update is called once per frame
   void Update()
   {
      if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsEditor)) {
         VirtualNetworkController controller = HopperNetwork.GetMyController(); 

         if (controller != null) {
            float x = Input.GetAxis("Horizontal"); 
            float y = -Input.GetAxis("Vertical"); 
            bool action = Input.GetButtonDown("Fire2"); 

            Vector2 move = new Vector2(x, y);
            if (move.sqrMagnitude > 1.0f) {
               move.Normalize();
            }


            UpdateMovementKnib(move);
            controller.SetMovement(move);

            if (action) {
               DoAction();
            }
         }
      }

      if (!Input.touchSupported) {
         // fake touch
         if (Input.GetMouseButton(0) && (!HackActionDown || (ActiveFingerID >= 0))) {
            HackActionDown = true; 
            Touch touch = new Touch(); 
            touch.fingerId = 1; 
            if (ActiveFingerID == -1) {
               touch.phase = TouchPhase.Began;
            } else {
               touch.phase = TouchPhase.Moved; 
            }

            touch.position = Input.mousePosition;
            ProcessTouch(touch); 
         } else if (HackActionDown && !Input.GetMouseButton(0)) {
            Touch touch = new Touch(); 
            touch.phase = TouchPhase.Ended; 
            touch.fingerId = ActiveFingerID; 
            touch.position = Input.mousePosition;
            ProcessTouch(touch);
            HackActionDown = false; 
         }
      }

      if (Input.touches.Length > 0) {
         for (int i = 0; i < Input.touches.Length; ++i) {
            ProcessTouch( Input.touches[i] ); 
         }
      }
   }
}
