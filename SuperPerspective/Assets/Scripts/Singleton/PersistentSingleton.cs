using UnityEngine;


namespace SuperPerspective.Singleton
{
    public class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        public override void Awake()
        {
            // Call Singleton Awake function to ensure only one instance is created
            // If this is a duplicate instance, don't initialize
            base.Awake();
            if (this != _instance)
                return;

            // Make object persistent between scenes
            Object.DontDestroyOnLoad(this.gameObject);
        }
 
    }
}
