using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientSceneController : MonoBehaviour
{
   enum eClientState
   {
      DISCOVER, 
      NOT_READY,
      READY, 
      IN_GAME,  
   }

   public GameObject Title;
   public GameObject ReadyUpUI;
   public GameObject WolfUI; 
   public GameObject HopperUI; 
}

