using UnityEngine;
using System.Collections;

/// <summary>
/// GenUI Generic Singleton, use this to ensure a single instance within Unity.
/// Implements Monobehaviour so that you can acess methods such as Start(), Update(), OnDestroy(), etc.
/// </summary>

namespace GLIB.Core {

	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour  {

		static bool _isApplicationQuitting;

		static T _instance = null;

		static object _lock = new object();

		public static T Instance
		{
			
			get {

				lock(_lock)
				{

					// If application is quitting return null 
					if(_isApplicationQuitting)
					{
						Debug.LogWarning("Could not request "+typeof(T).Name+" Instance, Module has turned off.");
						_instance = default(T);
						return null;
					}
					
					_instance = (T)FindObjectOfType<T>();
					
					if(_instance == null)
					{

						string gameObjectName = typeof(T).Name;

						GameObject gameObject = GameObject.Find(gameObjectName);

						if(gameObject == null)
						{

							gameObject = new GameObject();
							gameObject.name = gameObjectName;

						}

						_instance = gameObject.AddComponent<T>();

					}
					
					return _instance;

				}
				
			}
			
		}

		/// <summary>
		/// Raises the application quit event. WARNING WHEN OVERRIDING CALL BASE METHOD TO PREVENT MEMORY LEAKS!
		/// </summary>
		public virtual void OnApplicationQuit ()
		{
			// release reference on exit
			_isApplicationQuitting = true;


		}

		/// <summary>
		/// Raises the destroy event. WARNING WHEN OVERRIDING CALL BASE METHOD TO PREVENT MEMORY LEAKS!
		/// </summary>
		public virtual void OnDestroy ()
		{
			//If game object is externally removed finish the instance 
			_isApplicationQuitting = true;
		}
	}

}
