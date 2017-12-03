using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBomb : MonoBehaviour
{
   public GameObject[] PrefabList = new GameObject[0]; 
   public int m_minCount = 1; 
   public int m_maxCount = 1; 

   // Use this for initialization
   void Start()
   {
      if (PrefabList.Length > 0) {
         int count = Random.Range( m_minCount, m_maxCount + 1 ); 
         for (int i = 0; i < count; ++i) {
            int idx = Random.Range( 0, PrefabList.Length ); 
            GameObject.Instantiate( PrefabList[idx], transform.position, Quaternion.identity ); 
         }
      }

      // I've done my job; 
      GameObject.Destroy(gameObject); 
   }
}

