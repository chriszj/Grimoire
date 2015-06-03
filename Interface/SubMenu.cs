/*
 * Name SubMenu
 * Version: 0.9;
 * Author: Christian Salvador Zermeño Juárez
 * */

/// <summary>
/// This class creates a simple submenu, it has the following custom attributes:
/// Size,
/// Position.
/// Also has the following methods:
/// AddEntry();
/// OpenMenu();
/// CloseMenu();
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
using System.Collections.Generic;
using System;

namespace GLIB.Interface {

		public class SubMenu {

		Vector2 _position = new Vector2(0.0f, 0.0f);
		public Vector2 position { get{return _position;} set{_position = value;} }

		Vector2 _anchorMin = new Vector2(0.0f, 1.0f);
		public Vector2 anchorMin {get{return _anchorMin;} set{_anchorMin = value;} }

		Vector2 _anchorMax = new Vector2(0.0f, 1.0f);
		public Vector2 anchorMax {get{return _anchorMax;} set {_anchorMax = value;} }

		Vector2 _pivot = new Vector2 ();
		public Vector2 pivot {get{return _pivot;} set {_pivot = value;} }

		Vector2 _entrySize = new Vector2(50.0f, 100.0f);
		public Vector2 entrySize { get{return _entrySize;} set{_entrySize = value;} }

		Sprite _entrySprite;
		public Sprite entrySprite { get{return _entrySprite;} set{_entrySprite = value;} }

		/*Color _entryTextColor = new Color32(255, 255, 255, 255);
		public Color entryTextColor { get{return _entryTextColor;} set{_entryTextColor = value;} }*/

		bool _scrollable;
		public bool scrollable { get{return _scrollable;} set{_scrollable = value;} }  

		Transform _parent;
		public Transform parent { get{return _parent;} set{_parent = value;} }

		GameObjectDefinition SubMenuFrameOutFocus;

		GameObject SubMenuFrame;

		//List of entries
		List<GameObjectDefinition> _entries = new List<GameObjectDefinition>();

		public SubMenu (){

			SubMenuFrameOutFocus = new GameObjectDefinition();
			SubMenuFrameOutFocus.name = "SubMenuFrameOutFocus";
			SubMenuFrameOutFocus.anchorMin = new Vector2();
			SubMenuFrameOutFocus.anchorMax = new Vector2(1, 1);
			SubMenuFrameOutFocus.anchoredPosition = new Vector2(0, 0);
			SubMenuFrameOutFocus.sizeDelta = new Vector2(0, 0);
			SubMenuFrameOutFocus.clickAction = CloseSubMenu;
		}

		public void AddEntry(GameObjectDefinition entry){

			if (_entrySprite == null)
				_entrySprite = Resources.Load<Sprite>("Media/Sprites/SubMenu/entryBG");

			entry.imageSprite = _entrySprite;

			_entries.Add(entry);

		}

		public void DeleteEntry(int index)
		{
			try{
				_entries.RemoveAt(index);
			}
			catch(IndexOutOfRangeException e)
			{
				Debug.LogWarning(e.Message+"\nEntry not found in entries");
			}
		}

		public void ClearEntries()
		{
			_entries.Clear();
		}


		public void OpenSubMenu(){

			if(_parent == null)
			{
				Canvas canv = GameObject.FindObjectOfType<Canvas>();

				_parent = canv.transform;

			}

			//Draw the outfocus first
			SubMenuFrameOutFocus.parent = _parent;
			SubMenuFrameOutFocus.Activate();


			//Draw the frame first
			SubMenuFrame = new GameObject("SubMenuFrame");

			SubMenuFrame.transform.SetParent(_parent);

			SubMenuFrame.transform.localScale = new Vector3(1, 1, 1);

			RectTransform SubMenuFrameRect = SubMenuFrame.AddComponent<RectTransform>();

			UnityEngine.UI.Image SubMenuFrameImg = SubMenuFrame.AddComponent<UnityEngine.UI.Image>();

			SubMenuFrameImg.color = new Color32 (176, 176, 176, 255);

			GridLayoutGroup SubMenuFrameGrid = SubMenuFrame.AddComponent<GridLayoutGroup>();

			SubMenuFrameRect.anchorMin = _anchorMin;
			SubMenuFrameRect.anchorMax = _anchorMax;

			SubMenuFrameRect.pivot = _pivot;

			SubMenuFrameRect.anchoredPosition = _position;

			SubMenuFrameGrid.cellSize = _entrySize;

			Vector2 subMenuFrameGridSpacing = SubMenuFrameGrid.spacing;

			subMenuFrameGridSpacing.y = 1;

			SubMenuFrameGrid.spacing = subMenuFrameGridSpacing;

			// For a outline Effect
			RectOffset SubMenuFrameGridRectOffset = SubMenuFrameGrid.padding;

			SubMenuFrameGridRectOffset.bottom += 1;
			SubMenuFrameGridRectOffset.top += 1;
			SubMenuFrameGridRectOffset.left += 1;
			SubMenuFrameGridRectOffset.right += 1;

			SubMenuFrameGrid.padding = SubMenuFrameGridRectOffset;

			SubMenuFrameRect.sizeDelta = new Vector2(_entrySize.x + 2, (_entrySize.y * _entries.Count)+2+(_entries.Count-1));

			foreach(GameObjectDefinition entity in _entries)
			{

				entity.parent = SubMenuFrame.transform;

				entity.Activate();

			}

		}

		public void CloseSubMenu(){

			foreach(GameObjectDefinition entity in _entries)
			{
				
				entity.Deactivate();
				
			}

			SubMenuFrameOutFocus.Deactivate();

			if(SubMenuFrame != null)
			{
				UnityEngine.Object.Destroy(SubMenuFrame);
				SubMenuFrame = null;

			}

		}

	}
}
