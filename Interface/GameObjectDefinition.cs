using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// GameObjectDefinition. Version 0.9, Author: GenUI
///  
/// This class creates a GameObject with the necessary components 
/// with parameters previously defined and stored in the instance  
/// when the function Activate() is called and destroyed.
/// 
/// To use first create a new GameObjectDefinition
/// 
/// entity = new GameObjectDefinition();
/// 
/// Then add whatever you need
/// 
/// //This will automatically set _ImageEnabled to true
/// entity.imageColor = new color32(180, 0, 220, 255); 
/// 
/// So when you call 
/// 
/// entity.activate()
/// 
/// It will create the gameObject and add the necesary components for you
/// in this case it will add an Image Component and will change its color
/// 
/// Enjoy!!!
/// 
/// </summary>

namespace GLIB.Interface{

	public class GameObjectDefinition {

		GameObject _display;
		public GameObject display {get{return _display;}}

		// General Properties
		string _name;
		public string name {get{return _name;} set{_name = value;}}

		Transform _parent;
		public Transform parent { get{return _parent;} set{_parent = value;} } 

		Vector2 _anchorMin;
		public Vector2 anchorMin { get{return _anchorMin;} set{_anchorMin = value;} }

		Vector2 _anchorMax;
		public Vector2 anchorMax { get{return _anchorMax;} set{_anchorMax = value;} }

		Vector2 _pivot = new Vector2(0.5f, 0.5f);
		public Vector2 pivot { get{return _pivot;} set{_pivot = value;} }

		Vector2 _anchoredPosition;
		public Vector2 anchoredPosition { get{return _anchoredPosition;} set{_anchoredPosition = value;} }
		
		Vector2 _sizeDelta;
		public Vector2 sizeDelta { get{return _sizeDelta;} set{_sizeDelta = value;} }

		//For ImageComponent
		bool _imageEnabled;

		Sprite _imageSprite;
		public Sprite imageSprite{ get{return _imageSprite;} set{_imageSprite = value; _imageEnabled = true;} }

		Color _imageColor; // The default color
		public Color imageColor{ get{return _imageColor;} set{_imageColor = value; _imageEnabled = true;} } 

		// Image Type which is simple, sliced or tiled
		UnityEngine.UI.Image.Type _imageType;
		public UnityEngine.UI.Image.Type imageType{ get{return _imageType;} set{_imageType = value; _imageEnabled = true;} }

		//For TextComponent
		bool _textEnabled;

		string _textString;
		public string textString{ get{return _textString;} set{_textString = value; _textEnabled = true;} }

		Font _textFont;
		public Font textFont{ get{return _textFont;} set{_textFont = value; _textEnabled = true;} } 

		Color _textColor = new Color32(255, 255, 255, 255);//default font color
		public Color textColor{ get{return _textColor;} set{_textColor = value; _textEnabled = true;} }

		int _textSize;
		public int textSize{ get{return _textSize;} set{_textSize = value; _textEnabled = true;} }

		//For Button
		bool _buttonEnabled;

		public delegate void ClickActionDelegate();
		ClickActionDelegate _clickAction;
		public ClickActionDelegate clickAction { get{return _clickAction;} set{_clickAction = value; _buttonEnabled = true; _imageEnabled = true;} }

		// Is alive?
		bool _alive = false;
		public bool isAlive { get{return _alive;} set{ _alive = value; } }

		public GameObjectDefinition(){

		}

		public void Activate(){

			if (_display == null) {
				_display = new GameObject (!string.IsNullOrEmpty (_name) ? _name : "GameObjectDefinition");

				if (_parent != null)
					_display.transform.SetParent (_parent);

				_display.transform.localScale = new Vector3 (1, 1, 1);

				RectTransform _displayRect = _display.AddComponent<RectTransform> ();

				if (_anchoredPosition != null)
					_displayRect.anchoredPosition = _anchoredPosition;

				if (_anchorMin != null)
					_displayRect.anchorMin = _anchorMin;

				if (_anchorMax != null)
					_displayRect.anchorMax = _anchorMax;

				if (_pivot != null)
					_displayRect.pivot = _pivot;

				if (_sizeDelta != null)
					_displayRect.sizeDelta = _sizeDelta;

				//Set image component properties
				UnityEngine.UI.Image _displayImage = null;

				if (_imageEnabled) {
					_displayImage = _display.AddComponent<UnityEngine.UI.Image> ();

					// Set the default color to full white with alpha in order to display images correctly
					if (_imageSprite != null) {
						//Set color to correct values to display properly Image
						_imageColor = new Color (255, 255, 255, 255);
						_displayImage.sprite = _imageSprite;
					}

					if (_imageColor != null)
						_displayImage.color = _imageColor;

					if (_imageType != null)
						_displayImage.type = _imageType;
				}

				//Set text component properties
				Text _displayText = null;

				if (_textEnabled) {

					if (_imageEnabled) {
						GameObject _displayTextChild = new GameObject ("Text");

						_displayTextChild.transform.SetParent (_display.transform);

						_displayTextChild.transform.localScale = new Vector3 (1, 1, 1);

						RectTransform _displayTextChildRect = _displayTextChild.AddComponent<RectTransform> ();

						_displayTextChildRect.anchorMin = new Vector2 (0, 0);

						_displayTextChildRect.anchorMax = new Vector2 (1, 1);

						_displayTextChildRect.sizeDelta = new Vector2 (5, 5);

						_displayTextChildRect.anchoredPosition = new Vector2 ();

						_displayText = _displayTextChild.AddComponent<Text> ();
					} else {
						_displayText = _display.AddComponent<Text> ();
					}

					_displayText.alignment = TextAnchor.MiddleCenter;

					_displayText.text = _textString;

					if (_textFont != null)
						_displayText.font = _textFont;
					else
						_displayText.font = Resources.Load<Font> ("Media/Fonts/OpenSans-Regular");

					if (_textColor != null)
						_displayText.color = _textColor;

					if (_textSize != 0)
						_displayText.fontSize = _textSize;

				}

				//Set Button component properties
				if (_buttonEnabled) {
					Button _displayButton = _display.AddComponent<Button> ();

					_displayButton.onClick.AddListener (delegate {
						OnClickEvent ();
					});

					if (_displayImage != null && _imageEnabled)
						_displayButton.image = _displayImage;

				}

				_alive = true;
			}
		}

		public void Deactivate(){

			if(_display != null)
			{
				Object.Destroy(_display);
				_display = null;
			}

			_alive = false;

		}


		void OnClickEvent(){

			if(_clickAction != null)
				_clickAction();

		}

	}

}
