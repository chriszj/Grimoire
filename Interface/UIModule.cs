using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GLIB.Core;
using GLIB.Extended;
using GLIB.Utils;

/// <summary>
/// Use this to transform a class into a UI singleton module.
/// </summary>

// UI Module 
// It requires RectTransform Component to fix position bugs
namespace GLIB.Interface {	

	public sealed class UIModuleDisplayObject:MonoBehaviour{

		public delegate void OnKillDelegate(bool force = false);
		OnKillDelegate _OnKill;
		public OnKillDelegate OnKill {get{return _OnKill;}set{_OnKill = value;}}

		// Call parent's Terminate() when the displayObject has been destroyed
		public void OnDestroy ()
		{
			try{
				_OnKill(true);
			}
			catch(System.NullReferenceException e)
			{
				Debug.LogError(e.Message+"Parent not set! Use UIModule = YourClass to set it.");
			}

		}

	}

	[RequireComponent (typeof(RectTransform))]
	public abstract class UIModule<N> : Singleton<N> where N : UIModule<N> {

		static bool _isApplicationQuitting;

		public enum ZIndexPlacement:int {
			BOTTOM = -3,
			MIDDLE = -2,
			TOP = -1
		}

		bool _isRunning = false;
		public bool IsRunning{get{return _isRunning;}}

		GameObject _displayObject;
		/// <summary>
		/// Returns the Display Object for the UIModule;
		/// </summary>
		public GameObject DisplayObject{ get{ return _displayObject; } }

		string _displayObjectPath;
		/// <summary>
		/// The path where a GameObject prefab exists, if an empty string is returned
		/// an empty Display Object will be created. 
		/// </summary>
		protected abstract string DisplayObjectPath { get; }

		Transform _displayObjectParent;
		/// <summary>
		/// Return null if you want displayObject to be in the default MainCanvas
		/// </summary>
		/// <value>The display object parent.</value>
		protected abstract Transform DisplayObjectParent { get; }
		
		Vector2? _displayObjectPosition;
		/// <summary>
		/// Set the initial position of the Display Object. If you want to use the original
		/// prefab's position then return a null vector in this way: Vector2? nullvector = null; return nullvector;
		/// </summary>
		/// <value>The display object position.</value>
		protected abstract Vector2? DisplayObjectPosition { get; }

		int _displayObjectZIndex;
		/// <summary>
		/// Set the Display Object z index; Use (int)ZIndexPlacement for a predefined placement if you
		/// are not shure about what z index to use
		/// </summary>
		/// <value>The display index of the object Z.</value>
		protected abstract int DisplayObjectZIndex{ get; }

		/* Animations */

		public struct Transition {

			/// Animation Types to perform when Module is initialized / terminated, mix is not supported yet.
			public enum InOutAnimations:int {
				NONE = 0,
				SCALE = 1
			}

			public InOutAnimations animationType;// = InOutAnimation.NONE;

			/// <summary>
			/// The _in out animation duration.
			/// </summary>
			public float animationDuration;// = 0.5f;

			/// <summary>
			/// The _in out animation deformation amount, sets how much far or near the display object will be affected. 
			/// Must be in percentage value 0-1;
			/// </summary>
			public float animationDeform;// = 0.5f;

			public float animationPercent;

			/// <summary>
			/// Initializes a new instance of the <see cref="GLIB.Interface.UIModule`1+Transition"/> struct.
			/// </summary>
			/// <param name="animation">The type of animation, currently only supports: None and Scale</param>
			/// <param name="duration">The duration of the animation</param>
			/// <param name="offset">How much the module will be deformed. Use a value from 0-100</param>
			public Transition(InOutAnimations animation = InOutAnimations.NONE, float duration = 0.5f, float deformAmount = 0.5f){
				animationType = animation;
				animationDuration = duration;
				animationDeform= deformAmount;
				animationPercent = 0;
			}

		}

		Transition _inOutTransition;
		/// <summary>
		/// Returns the transition animation that will be executed when the module initializes or ends, override to customize
		/// By default returns a transition without animation.
		/// </summary>
		protected virtual Transition InOutTransition{ get { return new Transition (); } }

		bool? _animateIn = null;
		bool? _animateOut = null;

	    /// <summary>
	    /// Save original values;
	    /// </summary>
	    Vector3 _inOutOriginalScale;

		/*End Animations*/

		protected abstract void ProcessInitialization();
		
		protected abstract void ProcessTermination();
		
		protected abstract void ProcessUpdate();

		public virtual void Awake(){
			if(this.transform.parent == null)
				gameObject.GroupIntoUIModuleObject ();
		
		}

		// /// <summary>
		/// Initialize this instance with a custom displayObjectPath.
		/// </summary>
		/// <param name="displayObjectPath">Display object path.</param>
		public virtual void Initialize (string displayObjPath, Transform displayObjParent, Vector2? displayObjPosition, int displayObjZIndex, Transition transitionAnim)
		{
			if (!_isRunning) {

				try{
					_isRunning = true;

					_displayObjectPath = displayObjPath;
					_displayObjectParent = displayObjParent;
					_displayObjectPosition = displayObjPosition;
					_displayObjectZIndex = displayObjZIndex;
					_inOutTransition = transitionAnim;

					if(_inOutTransition.animationType != Transition.InOutAnimations.NONE)
						_animateIn = true;

					RenderDisplayObject();
					ProcessInitialization();

				}
				catch (System.Exception e)
				{
					Debug.LogError("Could not Process Initialization\n"+e.Message);
					Terminate(true);
				}

			}
		}

		/// <summary>
		/// Initialize this instance with the default displayObjectPath
		/// </summary>
		public virtual void Initialize ()
		{
			if (!_isRunning) {

				try{

					_isRunning = true;
					_displayObjectPath = DisplayObjectPath;
					_displayObjectParent = DisplayObjectParent;
					_displayObjectPosition = DisplayObjectPosition;
					_displayObjectZIndex = DisplayObjectZIndex;
					_inOutTransition = InOutTransition;

					if(_inOutTransition.animationType != Transition.InOutAnimations.NONE)
						_animateIn = true;

					RenderDisplayObject();
					ProcessInitialization();

				}
				catch (System.Exception e)
				{
					Debug.LogError("Could not Process Initialization\n"+e.Message+"\n"+e.StackTrace);
					Terminate(true);
				}

			}
		}

		public virtual void Update(){
			
			if (_isRunning) {
				try {

					/*Animation*/

					if(_animateIn == true){

						if(_inOutTransition.animationType == Transition.InOutAnimations.SCALE){
							
							Vector3 fscale = _displayObject.transform.localScale;
							Vector3 tscale = new Vector3(_inOutOriginalScale.x,
							                             _inOutOriginalScale.y,
							                             _inOutOriginalScale.z);
							
							Vector3 nscale = Vector3.Lerp (fscale, tscale, _inOutTransition.animationPercent);
							_displayObject.transform.localScale = nscale;
							
							Image[] imgs = _displayObject.GetComponentsInChildren<Image> ();
							
							Text[] texts = _displayObject.GetComponentsInChildren<Text> ();
							
							foreach (Image img in imgs) {
								
								Color32 ncolor = img.color;
								ncolor.a = 255;
								
								img.color = Color32.Lerp (img.color, ncolor, _inOutTransition.animationPercent);
							}
							
							foreach (Text text in texts) {
								
								Color32 tncolor = text.color;
								tncolor.a = 255;
								
								text.color = Color32.Lerp (text.color, tncolor, _inOutTransition.animationPercent);
								
							}
							
							if (_inOutTransition.animationPercent <= 1){
								_inOutTransition.animationPercent += Time.deltaTime / _inOutTransition.animationDuration;
							}
							else{
								_inOutTransition.animationPercent = 0;
								_animateIn = false;
							}

						}
						else {
							_animateIn = false;
						}

					}
					else if(_animateOut == true){

						if(_inOutTransition.animationType == Transition.InOutAnimations.SCALE){

							Vector3 fscale = _displayObject.transform.localScale;
							Vector3 tscale = new Vector3(_inOutOriginalScale.x * _inOutTransition.animationDeform,
							                             _inOutOriginalScale.y * _inOutTransition.animationDeform,
							                             _inOutOriginalScale.z * _inOutTransition.animationDeform);

							Vector3 nscale = Vector3.Lerp (fscale, tscale, _inOutTransition.animationPercent);
							DisplayObject.transform.localScale = nscale;
							
							Image[] imgs = DisplayObject.GetComponentsInChildren<Image> ();
							
							Text[] texts = DisplayObject.GetComponentsInChildren<Text> ();
							
							foreach (Image img in imgs) {
								
								Color32 ncolor = img.color;
								ncolor.a = 0;
								
								img.color = Color32.Lerp (img.color, ncolor, _inOutTransition.animationPercent);
							}
							
							foreach (Text text in texts) {
								
								Color32 tncolor = text.color;
								tncolor.a = 0;
								
								text.color = Color32.Lerp (text.color, tncolor, _inOutTransition.animationPercent);
								
							}
							
							if (_inOutTransition.animationPercent <= 1){
								_inOutTransition.animationPercent += Time.deltaTime / _inOutTransition.animationDuration;
							}
							else{
								_inOutTransition.animationPercent = 0;
								_animateOut = false;
								Terminate ();
							}
						}
						else {
							Terminate();
							_animateOut = false;
						}
					}
						
					/*En Animation*/

					ProcessUpdate ();
				}
				catch(System.Exception e) {
					Debug.LogError("Could not Process Update\n"+e.Message+"\n"+e.StackTrace);
				}
			}
			
		}

		public virtual void Terminate (bool force = false)
		{
			if (_isRunning) {

				if(_inOutTransition.animationType == Transition.InOutAnimations.NONE || _animateOut == false || force){

					try {

						_isRunning = false;
						ClearDisplayObject();
						ProcessTermination();

						_animateIn = null;
						_animateOut = null;

					}
					catch (System.Exception e)
					{
						Debug.LogError("Could not Process Termination\n"+e.Message+"\n"+e.StackTrace);
						ClearDisplayObject();

					}

				}
				else{

					_animateOut = true;

				}

			}  
		}

		void RenderDisplayObject(){

			try{

				GameObject objectToLoad = Resources.Load<GameObject>(_displayObjectPath);

				RectTransform displayRect;

				if(objectToLoad != null){

					_displayObject = (GameObject)Instantiate(objectToLoad);
					_displayObject.name = "DisplayObj<"+objectToLoad.name+">";

				}
				else{
					_displayObject = new GameObject("DisplayObj<"+typeof(N).Name+">");

					displayRect = _displayObject.AddComponent<RectTransform>();

					displayRect.anchorMin = new Vector2();
					displayRect.anchorMax = new Vector2(1,1);
					displayRect.anchoredPosition = new Vector2();
					displayRect.sizeDelta = new Vector2();
				}

				// Add a simple script that will call Terminate if it is destroyed
				UIModuleDisplayObject _displayObjectScript = _displayObject.AddComponent<UIModuleDisplayObject>();
				_displayObjectScript.OnKill = Terminate;

				displayRect = _displayObject.ResolveComponent<RectTransform>();//ResolveComponent<RectTransform>(_displayObject);

				// Keep original size references to overcome the canvas scaler transformations later
				Vector2 origSize = displayRect.sizeDelta;
				Vector2 origPos = displayRect.anchoredPosition;

				// If there is a Parent specified insert this object within.
				Transform _parent = DisplayObjectParent; 

				// If parent is null, then perform the method ResolveMainCanvas;
				if (_parent == null){
					_parent =  transform.GetMainCanvas();//ResolveMainCanvas();

				}

				// Finally set the suitable parent to the _displayObject
				_displayObject.transform.SetParent (_parent);

				// Fix scale bug
				_displayObject.transform.localScale = new Vector3(1,1,1);

				// Set Z index order
				int siblingIndex = _displayObjectZIndex;
						
				int numChildren = _parent.childCount;

				//Crop the value between -3 and numChildren
						
				//if sibling is negative then it is using predefined position
				if(siblingIndex < 0)
				{
							
					//crop the value to -3
					int newValue = siblingIndex % -3;

					//fix the value if the previous line returns a 0 as new value
					newValue -= newValue == 0?3:0;
					
					siblingIndex = newValue;

					switch((ZIndexPlacement)siblingIndex)
					{
						case ZIndexPlacement.BOTTOM:
							siblingIndex = 0;
							break;
						case ZIndexPlacement.MIDDLE:
							siblingIndex = (int)(numChildren/2);
							break;
						case ZIndexPlacement.TOP:
							siblingIndex = numChildren-1;
							break;

					}
									
				}
				else // just crop the value to numChildren
					siblingIndex %= numChildren;
								
				_displayObject.transform.SetSiblingIndex(siblingIndex);
						
				if(_displayObjectPosition.HasValue)
					origPos = _displayObjectPosition.Value;
						
				// Readjust Position and Scale in order to prevent "Canvas Scaler" deformations
				_displayObject.GetComponent<RectTransform>().anchoredPosition = origPos;
				_displayObject.GetComponent<RectTransform>().sizeDelta = origSize;


				/*Animation Preparation*/

				if( _inOutTransition.animationType == Transition.InOutAnimations.SCALE ){
				
					_inOutOriginalScale = _displayObject.transform.localScale;
					_displayObject.transform.localScale = new Vector3(_inOutOriginalScale.x * _inOutTransition.animationDeform, 
					                                                  _inOutOriginalScale.y * _inOutTransition.animationDeform,
					                                                  _inOutOriginalScale.z * _inOutTransition.animationDeform);

					Image[] imgs = _displayObject.GetComponentsInChildren<Image>();

					foreach(Image img in imgs){
						Color32 ncolor = img.color;
						ncolor.a = 0;
						img.color = ncolor;
					}
				
					Text[] texts = _displayObject.GetComponentsInChildren<Text>();

					foreach(Text text in texts){
						Color32 tncolor = text.color;
						tncolor.a = 0;
						text.color = tncolor;
					}

				}

				/*End Animation*/
				
			}
			catch(System.NullReferenceException e)
			{
				Debug.LogError(e.Message+"\nSomething could not be rendered, Clearing Display Object");
				ClearDisplayObject();
			}

		}

		void ClearDisplayObject(){

			if (_displayObject != null) {
			
				Destroy(_displayObject);
				_displayObject = null;

			}

		}

		public override void OnDestroy ()
		{
			_isApplicationQuitting = true;
			Terminate (true);
		}

		public override void OnApplicationQuit ()
		{
			_isApplicationQuitting = true;
			Terminate (true);
		}

	}

}