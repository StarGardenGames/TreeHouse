using UnityEngine;

namespace SuperPerspective.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        
        #region Properties and Variables

        
        private static bool _applicationIsQuitting;
        protected static bool applicationIsQuitting { get { return _applicationIsQuitting; } }

        private static object _lock = new object();

        protected static T _instance;
        public static T instance
        {
            get
            {
                if (applicationIsQuitting) {
                    Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
                                     "' already destroyed on application quit." +
                                     " Won't create again - returning null.");
                    return null;
                }
                
                lock(_lock)
                {
                    if (_instance == null)
                    {
                        
                        _instance = (T) FindObjectOfType(typeof(T));
                        
                        // Ensure we don't have multiple singletons
                        if ( FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            
                        }
                        
                        if (_instance == null)
                        {
                            string singletonName = "(singleton) " + typeof(T).Name;
                            _instance = new GameObject(singletonName).AddComponent<T>();
                            Debug.LogWarning("Creating new " + singletonName);
                        }
                        
                    }
                    
                    return _instance;
                }
            }
        }

        #endregion Properties and Variables
        
        
        #region MonoBehavior Implementation

        public virtual void Awake()
        {
            // Set the first instance
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Debug.LogWarning("Destroying duplicate " + this.name);
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// If any script calls Instance after it have been destroyed due the 
        ///   application quiting, it will create a buggy ghost object that will 
        ///   stay on the Editor scene even after the application stops. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        #endregion MonoBehavior Implementation
    }
}
