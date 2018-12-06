namespace GLIB.Libraries
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Class modified from UnifyCommunity (wiki.unity3d.com)
    /// </summary>
    public class Toolbox : Singleton<Toolbox>
    {
        protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

        void Awake()
        {
            // Your initialization code here
        }

        // (optional) allow runtime registration of global objects
        static public T ResolveComponent<T>() where T : Component
        {
            T component = Instance.gameObject.GetComponent<T>();

            if (component == null)
            {
                component = Instance.gameObject.AddComponent<T>();
            }

            return component;
        }

    }

}