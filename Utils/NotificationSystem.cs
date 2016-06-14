using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

using GLIB.Interface;
using GLIB.Extended;

// Grimoire Util that provide you with a simple message system. 
// By its simplicity its also limited please check Proccess Initialization in order
// to check the neccesary names on your prefab in order to work

namespace GLIB.Utils {

	public class NotificationSystem : UIModule<NotificationSystem> {

		#region Components

		RectTransform _messageWindow;

		Text _messageTextComp;
		Image _messageImageComp;

		Button _messageAcceptButton;
		Button _messageDeclineButton;

		#endregion

		#region Properties
		float _messageWindowMargin = 20;
		
		// Uses the default template from Resources/Grimoire/Views/NotificationSystem
		string _templatePath = "Grimoire/Views/NotificationSystem"; 
		public string TemplatePath{
			get{ 
				return _templatePath;
			}
			set { 
				_templatePath = value;	
			}
		}

		string _messageText = "";
		public string MessageText {
			get{ 
				return _messageText;
			}
			set { 

				_messageText = value;

				if (isRunning) {

					_messageTextComp.text = value;

					Vector2 windowSize = _messageWindow.sizeDelta;

					windowSize.y = _messageTextComp.preferredHeight + Mathf.Abs(_messageTextComp.rectTransform.sizeDelta.y);

					_messageWindow.sizeDelta = windowSize;

					//Debug.Log (_messageWindow.sizeDelta);
				}
			}
		}

		Sprite _messageIcon = null;
		public Sprite MessageIcon {
			get { 
				return _messageIcon;
			}
			set {			

				_messageIcon = value;

				if (isRunning) { 			

					_messageImageComp.sprite = _messageIcon;

					bool iconDisabled = _messageIcon == null ? true : false;

					Vector2 nTPos = _messageTextComp.rectTransform.anchoredPosition;
					Vector2 nTSize = _messageTextComp.rectTransform.sizeDelta;

					float buttonSize = 0;

					if (_messageAcceptButton.IsActive () || _messageDeclineButton.IsActive ())
						buttonSize = ((RectTransform)_messageAcceptButton.transform).sizeDelta.y + _messageWindowMargin;

					float iconSize = iconDisabled ? 0 : Mathf.Abs (_messageImageComp.rectTransform.sizeDelta.y)+_messageWindowMargin;

					nTPos.y = -(iconSize + _messageWindowMargin);

					// iconsize + top margin + lower margin + button size + lower button margin 
					nTSize.y = -(iconSize+(_messageWindowMargin*2)+buttonSize);

					_messageTextComp.rectTransform.anchoredPosition = nTPos;
					_messageTextComp.rectTransform.sizeDelta = nTSize;
						
					// If sprite is null then why would display the icon?
					_messageImageComp.enabled = !iconDisabled; 

				}

				// Update text
				MessageText = _messageText;
			}
		}

		bool _messagePreviousProgressMode = false; //
		bool _messageProgressMode = false;
		public bool MessageProgressMode {
			get{ 
				return _messageProgressMode;
			}
			set{ 
				_messageProgressMode = value;

				//Set display of the buttons according to the value
				if(isRunning){

					_messageAcceptButton.gameObject.SetActive (_onMessageAccept == null || value? false : true);
					_messageDeclineButton.gameObject.SetActive (_onMessageDecline == null || value? false : true);

					HandleWindowButtons (_messageAcceptButton, _messageDeclineButton, _messageWindow);

					if(!_messageProgressMode)
						_messageImageComp.rectTransform.localRotation = Quaternion.identity;
				}
					
				// The following code must be run once for every different mode values
				if (_messageProgressMode != _messagePreviousProgressMode) {
					
					if (_messageProgressMode) {
				
						/*if (IsRunning) {
							_messageAcceptButton.gameObject.SetActive (false);
							_messageDeclineButton.gameObject.SetActive (false);
						}*/

						_messagePreviousIcon = MessageIcon;
						MessageIcon = MessageProgressIcon;

					} else {

						/*if (IsRunning) {
							_messageAcceptButton.gameObject.SetActive (_onMessageAccept != null ? true : false);
							_messageDeclineButton.gameObject.SetActive (_onMessageDecline != null ? true : false);

							// Restore image component rotation
							_messageImageComp.rectTransform.localRotation = Quaternion.identity;
						}*/

						// Restore Default Icon
						MessageIcon = _messagePreviousIcon;
					}

					/*if (IsRunning)
						HandleWindowButtons (_messageAcceptButton, _messageDeclineButton, _messageWindow);*/

					_messagePreviousProgressMode = _messageProgressMode;
				}
					
			}
		}

		// Keep a reference of the previous icon before being replaced by the progress icon;
		Sprite _messagePreviousIcon = null;

		Sprite _messageProgressIcon = null;
		public Sprite MessageProgressIcon {
			get{ 
				return _messageProgressIcon;
			}
			set {
				_messageProgressIcon = value;
			}
		}

		#endregion

		#region Delegates

		public delegate void OnMessageAcceptDelegate(bool force = false);
		OnMessageAcceptDelegate _onMessageAccept;

		public OnMessageAcceptDelegate OnMessageAccept{
			get{
				return _onMessageAccept;
			}
			set { 
				_onMessageAccept = value;

				if (isRunning) {
					_messageAcceptButton.gameObject.SetActive (_onMessageAccept == null ? false : true);
					HandleWindowButtons (_messageAcceptButton, _messageDeclineButton, _messageWindow);
				
				}

				// update text size
				MessageIcon = _messageIcon;
			}
		}

		public delegate void OnMessageDeclineDelegate ();
		OnMessageDeclineDelegate _onMessageDecline;

		public OnMessageDeclineDelegate OnMessageDecline{
			get{ 
				return _onMessageDecline;
			}
			set{
				_onMessageDecline = value;

				if (isRunning) {
					_messageDeclineButton.gameObject.SetActive (_onMessageDecline == null ? false : true);
					HandleWindowButtons (_messageAcceptButton, _messageDeclineButton, _messageWindow);


				}

				// update text size
				MessageIcon = _messageIcon;

			}
		}

		#endregion

		#region Abstract functions override

		protected override Transform DisplayObjectParent {
			get {
				return null;
			}
		}

		protected override string DisplayObjectPath {
			get {
				return _templatePath;
			}
		}

		protected override Vector2? DisplayObjectPosition {
			get {
				return null;
			}
		}

		protected override int DisplayObjectZIndex {
			get {
				return (int)ZIndexPlacement.TOP;
			}
		}

		protected override Transition InOutTransition {
			get {
				return new Transition (Transition.InOutAnimations.SCALE);
			}
		}

		protected override void ProcessInitialization (){
			
			_messageTextComp = this.FindAndResolveComponent<Text>("Message<Text>", DisplayObject);

			_messageWindow = this.FindAndResolveComponent<RectTransform>("MessageWindow", DisplayObject);

			_messageImageComp = this.FindAndResolveComponent<Image> ("MessageImage<Image>", DisplayObject);

			_messageAcceptButton = this.FindAndResolveComponent<Button> ("Accept<Button>", DisplayObject);

			_messageDeclineButton = this.FindAndResolveComponent<Button> ("Decline<Button>", DisplayObject);

			if (_messageTextComp == null || _messageWindow == null || _messageImageComp == null || _messageAcceptButton == null || _messageDeclineButton == null)
				throw new Exception ("Message required components not found");

			_messageAcceptButton.onClick.AddListener (delegate {

				if(_onMessageAccept != null)
					_onMessageAccept();
				else
					Terminate();

			});

			_messageDeclineButton.onClick.AddListener (delegate {

				if(_onMessageDecline != null)
					_onMessageDecline();
				else
					Terminate();

			});

			// Sync properties with what is currently set.
			OnMessageAccept = _onMessageAccept;
			OnMessageDecline = _onMessageDecline;
			MessageProgressMode = _messageProgressMode;
			MessageIcon = _messageIcon;
			MessageText = _messageText;

		}

		protected override void ProcessUpdate ()
		{
			if (_messageProgressMode)
				_messageImageComp.rectTransform.Rotate (0, 0, 1);
		}

		protected override void ProcessTermination ()
		{

		}

		#endregion

		#region Private functions

		// Modify the buttons layout with a single or double button layout
		// Todo needs more optmization for now only modifies x position of the buttons
		void HandleWindowButtons(Button acceptBtn, Button declineBtn, RectTransform container){

			if (acceptBtn == null && declineBtn == null) {
				Debug.LogError ("Could not align Accept button nor Decline button, null objects passed.");
				return;
			} else {
			
				// Get Rects
				RectTransform acceptBtnRect = (RectTransform)acceptBtn.transform;
				RectTransform declineBtnRect = (RectTransform)declineBtn.transform;

				// Adjust anchors and pivots to make manipulations
				acceptBtnRect.anchorMax = new Vector2(0.5f, 0);
				acceptBtnRect.anchorMin = new Vector2 (0.5f, 0);
				acceptBtnRect.pivot = new Vector2 (0.5f, 0);

				declineBtnRect.anchorMax = new Vector2 (0.5f, 0);
				declineBtnRect.anchorMin = new Vector2 (0.5f, 0);
				declineBtnRect.pivot = new Vector2 (0.5f, 0);

				Vector2 acceptPos = acceptBtnRect.anchoredPosition;
				Vector2 declinePos = declineBtnRect.anchoredPosition;
	 
				// Both buttons are visible adjust them in a double button layout.
				if (acceptBtn.IsActive() && declineBtn.IsActive()) {
					acceptPos.x = ((container.sizeDelta.x - _messageWindowMargin) / 4);
					declinePos.x = -((container.sizeDelta.x - _messageWindowMargin) / 4);
				} else if (acceptBtn.IsActive() && !declineBtn.IsActive())
					acceptPos.x = 0;
				else
					declinePos.x = 0;


				acceptBtnRect.anchoredPosition = acceptPos;
				declineBtnRect.anchoredPosition = declinePos;

			}
				

		}

		#endregion

		#region Public functions

		/// <summary>
		/// Shows a simple message with an accept button which if specified execute the OnMessageAccept delegate passed, and closes the window.
		/// </summary>
		public void NotifyMessage (string message, Sprite icon = null, OnMessageAcceptDelegate onAccept = null){

			// Disable progress mode if enabled
			MessageProgressMode = false;

			OnMessageAccept = Terminate;
            OnMessageAccept = delegate {

                if (onAccept != null)
                    onAccept();

                Terminate();
            };

			OnMessageDecline = null;

			MessageIcon = icon;
			MessageText = message;

			Initialize ();

		}

		/// <summary>
		/// Shows a simple button-less window with a progress message about a happening action. 
		/// </summary>
		public void NotifyProgress (string progressMessage){
			
			MessageProgressMode = true;

			MessageText = progressMessage;

			Initialize ();

		}

		/// <summary>
		/// Prompts the user with a message of an action to be executed with two programable buttons to proceed and cancel 
		/// </summary>
		public void PromptAction (string message, OnMessageAcceptDelegate onAccept, OnMessageDeclineDelegate onDecline,  Sprite icon = null){

			// Disable progress mode if enabled
			MessageProgressMode = false;

			OnMessageAccept = onAccept;
			OnMessageDecline = onDecline;

			MessageIcon = icon;
			MessageText = message;

			Initialize ();

		}

		#endregion

	}

}