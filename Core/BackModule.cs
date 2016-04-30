using UnityEngine;
using System.Collections;
using GLIB.Utils;
using GLIB.Extended;

namespace GLIB.Core {

	public abstract class BackModule<N> : Singleton<N> where N : BackModule<N> {

		bool _isRunning = false;
		public bool isRunning {get{return _isRunning;}}

		static bool _isApplicationQuitting;

		protected abstract void ProcessInitialization();
		
		protected abstract void ProcessTermination();
		
		protected abstract void ProcessUpdate();

		public virtual void Awake(){
			if(gameObject.transform.parent == null)
				gameObject.GroupIntoBackModuleObject ();
		}

		public virtual void Initialize ()
		{
			if (!_isRunning) {
				
				try{
					_isRunning = true;
					ProcessInitialization();
				}
				catch(System.Exception e)
				{
					Debug.LogError("Could not Process Initialization\n"+e.Message+"\n"+e.StackTrace);
					Terminate();
				}
				
			}
		}

		public virtual void Update(){
			
			if (_isRunning) {
				try {
					ProcessUpdate ();
				}
				catch(System.Exception e) {
					Debug.LogError("Could not Process Update\n"+e.Message+"\n"+e.StackTrace);
				}
			}
			
		}

		public virtual void Terminate(){
			
			if (_isRunning) {
				try{
					_isRunning = false;
					ProcessTermination();
				}
				catch (System.Exception e)
				{
					Debug.LogError("Could not Process Termination\n"+e.Message+"\n"+e.StackTrace);
				}
			}  
			
		}

		/*public override void OnDestroy ()
		{
			base.OnDestroy ();
			Terminate ();
		}*/

		public override void OnDestroy ()
		{
			_isApplicationQuitting = true;
			Terminate ();
		}

		public override void OnApplicationQuit ()
		{
			_isApplicationQuitting = true;
			Terminate ();
		}

	}

}
