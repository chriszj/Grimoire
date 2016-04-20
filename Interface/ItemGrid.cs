/* ITEM GRID
 * VERSION 0.8;
 * DEVELOPED BY CHRISTIAN SALVADOR ZERMEÑO JUAREZ
 * 
 * 
 * */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GLIB.Extended;

/// <summary>
/// Class to construct an Item Grid
/// Recieves a path to add automatically Items (these can be anything, from sprites to prefabs)
/// Recieves a path to asociate the items with their previews
/// User can add manually Items -> not yet actually XD 
/// Constructs a scrollable itemgrid in which items are buttons to be selectable.
/// selectedItem -> stores the selected Item
/// OnSelection -> specifies a custom function to perform after user has selected an item.
/// ShowItemGrid -> Displays Item Grid
/// HideItemGrid -> Hides Item Grid
///

namespace GLIB.Interface {

	public class ItemGrid {

		public delegate void selectionDelegate();
		selectionDelegate _onSelection;
		public selectionDelegate OnSelection {get{return _onSelection;}set{_onSelection = value;}}

		string _path;
		bool _updatePathItems = false;
		public string path{get{return _path;} set{ _updatePathItems = true; _path = value;}}

		string _prevPath;
		bool _updatePathPreviews = false;
		public string prevPath{get{return _prevPath;} set{ _updatePathPreviews = true; _prevPath = value;}}

		/*RectTransform _rectTransform;
		public RectTransform rectTransform{get{return _rectTransform;}set{_rectTransform = value;}} 
		*/

		Transform _parent;
		public Transform parent{get{return _parent;} set{_parent = value;}}

		int _rows = 2;
		public int rows{get{return _rows;} set{_rows = value;}}

		int _columns = 0;
		public int columns{get{return _columns;} set{_columns = value;}}

		Vector2 _gridSize = new Vector2(0.0f, Screen.height/2.0f);
		public Vector2 gridSize { get{return _gridSize;} set{_gridSize = value;} }

		float _gapFromOutFocus = 0.0f;
		public float gapFromOutFocus {get{return _gapFromOutFocus;} set{_gapFromOutFocus = value;}}
		
		Vector2 _screenSize;
		public Vector2 screenSize {get{return _screenSize;}}

		Vector2 _itemSize = new Vector2(100.0f, 100.0f);
		public Vector2 itemSize{get{return _itemSize;} set{_itemSize = value;} }

		Vector2 _spacing = new Vector2(10.0f, 10.0f);
		public Vector2 spacing{get{return _spacing;} set{_spacing = value;} }

		bool _preserveAspect = true;
		public bool preserveAspect{get{return _preserveAspect;} set{_preserveAspect = value;}}

		bool _visible = false;
		public bool visible{get{return _visible;}}

		Object[] _items; // Contains all items

		Sprite[] _previews; // Contains all preview Sprites

		Object _selectedItem; // the selected Item
		public Object selectedItem{get{return _selectedItem;}}

		int _selectedItemIndex = 0;
		public int selectedItemIndex{get{return _selectedItemIndex;}}

		GameObject _itemGridOutFocus; // Has Image and Button component to close when lose focus;

		GameObject _itemGridFrame; // Has Image Component and RectTransform component

		GameObject _itemGridScroll; // Has ScrollRect component and Recttransform component;

		GameObject _itemGridContainer; // Has GridLayout component and RectTransform component, and inside has all the preview items



		public ItemGrid(){


		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		// Graphically Initializes
		public void ShowItemGrid(){

			if(_updatePathItems)
			{
				_items = Resources.LoadAll(_path);

				_updatePathItems = false;
			}

			if(_updatePathPreviews)
			{
				_previews = Resources.LoadAll<Sprite>(_prevPath);

				_updatePathPreviews = false;
			}

			_screenSize = new Vector2(Screen.width, Screen.height);

			// We don't want to modify the original gridSize in the following block, so we initializa a local variable
			// to be edited there.
			Vector2 _ItemGridSize = new Vector2(_gridSize.x, _gridSize.y);

			// Is there a canvas scaler, if there is transform gridSize in the proper size
			if(_parent != null)
			{

				Transform transform = _parent.transform;

				CanvasScaler _parentCanvScaler = transform.GetMainCanvas().gameObject.ResolveComponent<CanvasScaler>();

				if(_parentCanvScaler != null)
				{
					_screenSize = _parentCanvScaler.referenceResolution;

					_ItemGridSize.x = ((_ItemGridSize.x)/Screen.width) * _screenSize.x;
					_ItemGridSize.y = ((_ItemGridSize.y)/Screen.height)* _screenSize.y;

					Debug.Log("ScreenSize "+ _gridSize.y);
				}

			}

			if(_itemGridOutFocus == null)
			{
				_itemGridOutFocus = new GameObject("ItemGridOutFocus");

				if(_parent != null)
					_itemGridOutFocus.transform.SetParent(_parent);
				
				_itemGridOutFocus.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				_itemGridOutFocus.AddComponent<RectTransform>();
				_itemGridOutFocus.AddComponent<UnityEngine.UI.Image>();
				_itemGridOutFocus.AddComponent<Button>();

				RectTransform _rectTransform = _itemGridOutFocus.GetComponent<RectTransform>(); 
				_rectTransform.anchorMin = new Vector2();
				_rectTransform.anchorMax = new Vector2(1.0f, 1.0f);

				float _gridHeight = _gapFromOutFocus + _ItemGridSize.y;

				_rectTransform.anchoredPosition = new Vector2(0.0f, _gridHeight/2.0f);
				_rectTransform.sizeDelta = new Vector2(0.0f, -_gridHeight);

				UnityEngine.UI.Image _itemGridOutFocusImg = _itemGridOutFocus.GetComponent<UnityEngine.UI.Image>();

				_itemGridOutFocusImg.color = new Color32(0, 0, 0, 0);

				Button _itemGridOutFocusBtn = _itemGridOutFocus.GetComponent<Button>();

				_itemGridOutFocusBtn.image = _itemGridOutFocusImg;

				_itemGridOutFocusBtn.onClick.AddListener(() => HideItemGrid());


			}

			/*
			 * Creates the 
			 * */
			if(_itemGridFrame == null)
			{
				_itemGridFrame = new GameObject("ItemGridFrame");

				if(_parent != null)
					_itemGridFrame.transform.SetParent(_parent);
				
				_itemGridFrame.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				_itemGridFrame.AddComponent<RectTransform>();
				_itemGridFrame.AddComponent<UnityEngine.UI.Image>();

				RectTransform _rectTransform = _itemGridFrame.GetComponent<RectTransform>(); 
				_rectTransform.anchorMin = new Vector2();
				_rectTransform.anchorMax = new Vector2(1.0f, 1.0f);

				//TODO make this more flexible

				float _gridHeight = _screenSize.y - _ItemGridSize.y;

				_rectTransform.anchoredPosition = new Vector2(0.0f, -_gridHeight/2.0f );
				_rectTransform.sizeDelta = new Vector2(0.0f, -_gridHeight);

				UnityEngine.UI.Image _itemGridFrameImage = _itemGridFrame.GetComponent<UnityEngine.UI.Image>();

				_itemGridFrameImage.color = new Color32(180, 180, 180, 255);


			}

			if(_itemGridScroll == null)
			{
				_itemGridScroll = new GameObject("ItemGridScroll");

				_itemGridScroll.transform.SetParent(_itemGridFrame.transform);

				_itemGridScroll.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				_itemGridScroll.AddComponent<RectTransform>();
				_itemGridScroll.AddComponent<ScrollRect>();

				RectTransform _rectTransform = _itemGridScroll.GetComponent<RectTransform>();

				_rectTransform.anchorMin = new Vector2();
				_rectTransform.anchorMax = new Vector2(1.0f, 1.0f);

				_rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
				_rectTransform.sizeDelta = new Vector2(-30.0f, -30.0f);
			}

			if(_itemGridContainer == null)
			{
				_itemGridContainer = new GameObject("ItemGridContainer");

				_itemGridContainer.transform.SetParent(_itemGridScroll.transform);

				_itemGridContainer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				_itemGridContainer.AddComponent<RectTransform>();
				_itemGridContainer.AddComponent<GridLayoutGroup>();

				GridLayoutGroup _itemGridContainerGrid = _itemGridContainer.GetComponent<GridLayoutGroup>();

				// TODO Delete following line when is finally not needed
				//Vector2 _cellSize = new Vector2((_itemSize.x/_screenSize.x) * Screen.width, (_itemSize.y/_screenSize.y) * Screen.height);
				Vector2 _cellSize = _itemSize;

				_itemGridContainerGrid.cellSize = _cellSize;

				_itemGridContainerGrid.spacing = _spacing;

				if(_items != null)
				{
					for(int i = 0; i < _items.Length; i++)
					{
						GameObject item = new GameObject();

						item.transform.SetParent(_itemGridContainer.transform);

						item.transform.localScale = new Vector3(1, 1, 1);

						item.AddComponent<RectTransform>();
						item.AddComponent<UnityEngine.UI.Image>();
						item.AddComponent<Button>();

						UnityEngine.UI.Image itemImg = item.GetComponent<UnityEngine.UI.Image>();

						Button itemBtn = item.GetComponent<Button>();

						// A quick fix for delegate is to use a local variable!, since it stores a reference, i is constantly changing, so we need a reference but of a new
						// variable!
						int local_i = i;

						// Delegates keep the value that was captured at the moment. actions does not, they use the current context value 
						itemBtn.onClick.AddListener(delegate{ItemAction(local_i);});

						itemImg.preserveAspect = _preserveAspect;

						if(_previews != null && _previews.Length >= 1)
						{
							itemImg.sprite = i<_previews.Length?_previews[i]:new Sprite();
						}
					}
				}

				RectTransform _rectTransform = _itemGridContainer.GetComponent<RectTransform>();

				ScrollRect _itemGridScrollScrollRect = _itemGridScroll.GetComponent<ScrollRect>();

				if(_columns == 0 && _rows != 0)//Horizontal Scroll
				{
					//TODO Check how many items fit vertically and change aproach!

					_itemGridContainerGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
					_itemGridContainerGrid.constraintCount = _rows;

					_rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
					_rectTransform.anchorMax = new Vector2(0.5f, 1.0f);

					_rectTransform.pivot = new Vector2(0.0f, 0.5f);
					_rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
					_rectTransform.sizeDelta = new Vector2((_cellSize.x * _items.Length) + (_spacing.x * (_items.Length-1))  , 0.0f);

					_itemGridScrollScrollRect.horizontal = true;
					_itemGridScrollScrollRect.vertical = false;
				}
				else if(_rows == 0 && _columns != 0)//Vertical Scroll
				{
					_itemGridContainerGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
					_itemGridContainerGrid.constraintCount = _columns;

					_rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
					_rectTransform.anchorMax = new Vector2(1.0f, 0.5f);

					_rectTransform.pivot = new Vector2(0.5f, 1.5f);
					_rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
					_rectTransform.sizeDelta = new Vector2( 0.0f, (_cellSize.y * _columns) + (_spacing.y * (_columns-1)));
					
					_itemGridScrollScrollRect.horizontal = false;
					_itemGridScrollScrollRect.vertical = true;
				}
				else //TODO Not Implemented yet!!!
				{
					_rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
					_rectTransform.anchorMax = new Vector2(0.1f, 1.0f);
				}

				_itemGridScrollScrollRect.content = _rectTransform;





			}

			_visible = true;

		}

		public void HideItemGrid(){

			if(_itemGridOutFocus != null)
			{
				if(_itemGridOutFocus.activeSelf)
					Object.Destroy(_itemGridOutFocus);

				_itemGridOutFocus = null;	
			}

			if(_itemGridFrame != null)
			{
				if(_itemGridFrame.activeSelf)
					Object.Destroy(_itemGridFrame);

				_itemGridFrame = null;
			}

			if(_itemGridScroll != null)
			{
				if(_itemGridScroll.activeSelf)
					Object.Destroy(_itemGridFrame);

				_itemGridFrame = null;
			}

			if(_itemGridContainer != null)
			{
				if(_itemGridContainer.activeSelf)
					Object.Destroy(_itemGridContainer);

				_itemGridContainer = null;
			}

			_visible = false;
		}

		public void AddItem(){

		}

		public void ItemAction(int index){

			_selectedItem = _items[index];

			_selectedItemIndex = index;

			//Debug.Log("Selected item is: "+index);

			if(_onSelection != null)
				_onSelection();

		}
	}

}
