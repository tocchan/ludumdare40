using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class JoinInput : MonoBehaviour 
{
   public static readonly string JOIN_ADDRESS_KEY = "LastJoinAddress";

   public InputField AddressText; 

	// Use this for initialization
	void Start() 
   {
      string joinAddress = HopperNetwork.Instance.GetLocalAddress();
      joinAddress = PlayerPrefs.GetString( JOIN_ADDRESS_KEY, joinAddress ); 

      AddressText = GetComponent<InputField>(); 
      AddressText.text = joinAddress; 
	}

   public void OnChanged()
   {
      PlayerPrefs.SetString( JOIN_ADDRESS_KEY, AddressText.text ); 
   }
}
