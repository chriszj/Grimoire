using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GLIB.Extended {

	/// <summary>
	/// Extended ScrollRect Component which allows you to reference your custom entry data write function and
	/// Has a simple optimization to reduce rendering times by disabling objects out of the scroll rect.
	/// WARNING: YOU SHOULD DISABLE ANY GRID LAYOUT COMPONENT, SINCE IT IS NOT COMPATIBLE WITH THIS, AND REDUCES 
	/// PERFOMANCE.
	/// Usage:
	/// 
	///   1. Define variable
	/// 
	///   UIScrollComponent _uiScroll;
	/// 
	///   2. Initialize On Any Initialization Method:
	///      
	///   _uiScroll = DisplayObject.AddComponent<UIScrollComponent> ();
	///   _uiScroll.enabled = false;
	///   _uiScroll.EntryDisplayObjectPath = "YOUR_PATH_TO_FRONTEND_PREFAB";
	///   _uiScroll.WriteEntryData = YourCustomMethodForWritingEntryData;
	///   _uiScroll.InitialEntriesCount = NUMBER_OF_DESIRED_ENTRIES_AT_Initialization;
	/// 
	///   3. Optional: Terminate _uiScroll on any termination method
	/// 
	///   Destroy(_uiScroll);
	///   _uiScroll = null
	/// 
	/// </summary>
	public class UIScrollComponent : MonoBehaviour {

		public delegate void WriteEntryDataDel(GameObject entry);
		protected WriteEntryDataDel _writeEntryData;
		public WriteEntryDataDel WriteEntryData {get{return _writeEntryData;} set{_writeEntryData = value;}}

		protected List<GameObject> _entries = new List<GameObject>();
		public List<GameObject> Entries {
			get{return _entries;
				
			}
		}

		int _initialEntriesCount;
		public int InitialNumberOfEntries {get{return _initialEntriesCount;} set{_initialEntriesCount = value;}}

		protected GameObject _entryDisplayObject;
		string _entryDisplayObjectPath;
		public string EntryDisplayObjectPath {get{return _entryDisplayObjectPath;} set{_entryDisplayObjectPath = value;}}
		protected RectTransform _entryDefinitionRect;
		
		protected RectTransform _scrollContainer;
		protected int _prevChildCount;
		
		protected ScrollRect _scrollRect;
		protected float _scrollerHeight;
		protected float _prevScrollerYPos;
		
		protected float lastEntryPosY;


		// Use this for initialization
		void Start () {

			StartScroller ();

		}

		protected virtual void StartScroller(){
		
			try {

				_entryDisplayObject = Resources.Load<GameObject>(_entryDisplayObjectPath);

				if(_entryDisplayObject == null)
					throw new NullReferenceException("EntryDisplayObject not found at: "+_entryDisplayObjectPath);

				_scrollRect = gameObject.GetComponentInChildren<ScrollRect>();
				
				_scrollerHeight = _scrollRect.GetComponent<RectTransform>().rect.height;
				
				//_entryDefinitionRect = _entryDisplayObject.GetComponentInChildren<RectTransform>();

				_scrollContainer = _scrollRect.content;

				_scrollContainer.anchorMin = new Vector2(0, 1);
				_scrollContainer.anchorMax = new Vector2(1, 1);
				_scrollContainer.pivot = new Vector2(0.5f, 1);
				_scrollContainer.sizeDelta = new Vector2(0, 0);
				
				for(int i = 0; i < _initialEntriesCount; i ++)
				{
					AddEntry();
				}
				
			}
			catch(NullReferenceException e)
			{
				Debug.LogError(e.Message+"\nFailed to initialize properly the Scroll Content Manager\n"+e.StackTrace);
			}
		
		}
		
		// Update is called once per frame
		void Update () {
			
			UpdateScroll ();
			
		}

		protected virtual void UpdateScroll(){
		
			if (_scrollRect != null) {
				
				float scrollerContentPosY = _scrollContainer.anchoredPosition.y;
				
				
				if(scrollerContentPosY != _prevScrollerYPos)
				{
					
					_prevScrollerYPos = scrollerContentPosY;
					
					foreach(GameObject entry in _entries)
					{
						Vector2 objPos = entry.GetComponent<RectTransform>().anchoredPosition;
						Vector2 objSize = entry.GetComponent<RectTransform>().sizeDelta;
						
						if(scrollerContentPosY + (objPos.y - objSize.y)  <= 0 && scrollerContentPosY + objPos.y >= -_scrollerHeight)
							entry.SetActive(true);
						else
							entry.SetActive(false);
						
					}
					
				}
				
			}

		}

		/// <summary>
		/// Adds an entry, if you have iterative functions in your custom WriteEntryData is recommended to leave the name parameter as default
		/// since entries are named by int indexing method in the initialization of the UIScrollerComponent.
		/// </summary>
		/// <param name="name">Name.</param>
		public virtual void AddEntry(string name = "")
		{
			GameObject entry = null;

			try {
				
				entry = Instantiate(_entryDisplayObject) as GameObject;
				entry.name = string.IsNullOrEmpty(name)?_entries.Count.ToString():name;

				entry.transform.SetParent(_scrollContainer, false);
				
				entry.transform.localScale = new Vector3(1, 1, 1);
				
				entry.SetActive(true);

				_entryDefinitionRect = entry.GetComponentInChildren<RectTransform>();

				//Adjust position and size
				RectTransform entryRect = entry.GetComponent<RectTransform>();
				
				entryRect.anchorMin = new Vector2(0, 1);
				entryRect.anchorMax = new Vector2(1, 1);
				entryRect.pivot = new Vector2(0.5f, 1);
				
				Vector2 entryRectPos = new Vector2(0, lastEntryPosY);
				
				entryRect.anchoredPosition = entryRectPos;

				if(_writeEntryData != null)
					_writeEntryData(entry);
				else 
					Debug.LogWarning("WriteEntryData Delegate method is not implemented, set a custom method to WriteEntryData in order to manipulate entries data.");
				
				lastEntryPosY -= _entryDefinitionRect.sizeDelta.y;
								
				Vector2 containerRectSize = _scrollContainer.sizeDelta;
				containerRectSize.y = Math.Abs(lastEntryPosY);
				
				_scrollContainer.sizeDelta = new Vector2(0, Math.Abs(lastEntryPosY));
				
				_entries.Add(entry);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message+"\nFailed to add entry.\n"+e.StackTrace);
				DeleteEntryAt(_entries.IndexOf(entry));

			}

		}
		
		public virtual void DeleteEntryAt(int index){
			
			try{
				
				GameObject entryToDelete = _entries[index];
				RectTransform entryToDeleteRect = entryToDelete.GetComponent<RectTransform>();
				
				float entrySizeY = entryToDeleteRect.sizeDelta.y;
				
				Destroy(_entries[index]);
				_entries.RemoveAt(index);
				
				// Update all entries position;
				for(int i = index; i < _entries.Count; i++)
				{
					RectTransform entryRect = _entries[i].GetComponent<RectTransform>();
					
					Vector2 entryPos = entryRect.anchoredPosition;
					
					entryPos.y -= entrySizeY;
					entryRect.anchoredPosition = entryPos;
				}
				
				// Update container's size
				Vector2 containerRectSize = _scrollContainer.sizeDelta;
				containerRectSize.y -= entrySizeY;
				
				_scrollContainer.sizeDelta = new Vector2(0, Math.Abs(lastEntryPosY));
								
			}
			catch (ArgumentOutOfRangeException e)
			{
				Debug.LogWarning(e.Message+"\nEntry to delete already destroyed or wasn't added to entries list.\n"+e.StackTrace);
			}
			
		}



	}

}
