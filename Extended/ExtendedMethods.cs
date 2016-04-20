using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GLIB.Core;
/// <summary>
/// This Class extends Unity's standar method "StartCoroutine" to "StartCoroutine<T>" so that you can use the Extended Coroutine within GenUI
/// </summary>

namespace GLIB.Extended {

	public static class ExtendedMethods {

		public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine){
			Coroutine<T> coroutineObject = new Coroutine<T>();
			coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
			return coroutineObject;
		}

		public static GameObject Find(this MonoBehaviour gmob, string name, GameObject parent){

			GameObject obj = null;
			
			Transform[] childs = parent.GetComponentsInChildren<Transform> ();
			
			foreach (Transform cTransform in childs) {
				
				if (cTransform.name == name) {
					obj = cTransform.gameObject;
					break;
				}
				
			}
			
			if(obj == null)
				throw new System.NullReferenceException("Game Object wit name: "+name+" could not be found within "+parent.name+".");
			
			return obj;

		}

		/// <summary>
		/// Search for a Game Object provided its name within parent if given, and then check if that object has the given component attached and returns it. 
		/// If no such component is found, one is automatically added.
		/// </summary>
		/// <returns>The Component requested.</returns>
		/// <param name="gameObj">Game object.</param>
		/// <param name="name">Name.</param>
		/// <param name="parent">Parent.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T FindAndResolveComponent<T>(this MonoBehaviour gameObj, string name, GameObject parent) where T:Component
		{
			GameObject obj = null;

            if (parent != null)
            {

                Transform[] childs = parent.GetComponentsInChildren<Transform>();

                foreach (Transform cTransform in childs)
                {

                    if (cTransform.name == name)
                    {
                        obj = cTransform.gameObject;
                        break;
                    }

                }

            }
            else { // look the object in the root hierarchy

                obj = GameObject.Find(name);

            }

			if(obj == null)
				throw new System.NullReferenceException("Game Object wit name: "+name+" could not be found within "+parent.name+".");

			T component = obj.GetComponent<T>();
			
			if (component == null) {
				Debug.LogWarning("Component: "+typeof(T).Name+" not found, adding one.");
				component = obj.AddComponent<T>();
			}
			
			return component;

		}

		public static Transform GetMainCanvas(this Transform canvas){
			
			GameObject mainCanvas = GameObject.Find("MainCanvas");
						
			if (mainCanvas == null) {
				
				Debug.LogWarning("No MainCanvas found! Creating one.");
				
				mainCanvas = new GameObject("MainCanvas");

				mainCanvas.transform.localScale = new Vector3(1, 1, 1);
								
				Canvas comp_Canvas = mainCanvas.AddComponent<Canvas>();
				
				comp_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				
				CanvasScaler comp_CanvasScl = mainCanvas.AddComponent<CanvasScaler>();
				
				comp_CanvasScl.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				comp_CanvasScl.referenceResolution = new Vector2(800,600);
				comp_CanvasScl.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
				comp_CanvasScl.referencePixelsPerUnit = 100;
				
				mainCanvas.AddComponent<GraphicRaycaster>();
				
			}

			return mainCanvas.transform;
			
		}

		/// <summary>
		/// Get the requested component of a given GameObject, if the GameObject doesn't have the requested component, one will be
		/// added automatically.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="gameObject">Game object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T ResolveComponent<T>(this GameObject gameObject) where T:Component
		{
			T component = gameObject.GetComponent<T>();
			
			if (component == null) {
				Debug.LogWarning("Component: "+typeof(T).Name+" not found, adding one.");
				component = gameObject.AddComponent<T>();
			}
			
			return component;
		}
						
		//Groups given GameObject into BackModules Group
		public static void GroupIntoBackModuleObject(this GameObject gObject){
			
			GameObject _BackModuleObj = GameObject.Find("BackModules");
			
			//If the grouping game object is null then create one;
			if(_BackModuleObj == null)
				_BackModuleObj = new GameObject("BackModules");
			
			gObject.transform.SetParent (_BackModuleObj.transform);
		}
		
		//Groups given GameObject into UIModules Group
		public static void GroupIntoUIModuleObject(this GameObject gObject){
			
			GameObject _UIModuleObj = GameObject.Find("UIModules");
			
			//If the grouping game object is null then create one;
			if(_UIModuleObj == null)
				_UIModuleObj = new GameObject("UIModules");
			
			gObject.transform.SetParent (_UIModuleObj.transform);
		}
		
		//Groups given GameObject into Utils Group
		public static void GroupIntoUtilsObject(this GameObject gObject){
			
			GameObject _Obj = GameObject.Find("Utils");
			
			//If the grouping game object is null then create one;
			if(_Obj == null)
				_Obj = new GameObject("Utils");
			
			gObject.transform.SetParent (_Obj.transform);

		}


	}
}