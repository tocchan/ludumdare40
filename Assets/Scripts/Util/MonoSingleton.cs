using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour 
   where T : MonoSingleton<T>
{
   static T s_instance;

   protected virtual void Awake()
   {
      if (s_instance == null) {
         s_instance = this as T;
      } else {
         GameObject.Destroy(gameObject);
      }
   }

   protected bool IsInstance()
   {
      return this == s_instance;
   }

   public static T GetInstance()
   {
      if (s_instance == null) {
         GameObject go = new GameObject();
         go.name = "_" + typeof(T).Name;

         go.AddComponent<T>();
         DontDestroyOnLoad(go);
      }

      return s_instance;
   }
}
