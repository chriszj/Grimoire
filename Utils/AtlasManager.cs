using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Atlas Manager dispatch single sprites from Atlas, if sprite does not exist it
/// returns a white textured sprite;
/// </summary>

namespace GLIB.Utils
{

	public static class AtlasManager {

		static List<Sprite> _sprites = new List<Sprite>();

		public static void AddAtlas(string path)
		{

			Sprite[] sprites = Resources.LoadAll<Sprite> (path);

			foreach (Sprite sprite in sprites)
				_sprites.Add (sprite);

		}

		public static Sprite getSprite(string name)
		{
			Sprite sprite = new Sprite();
			bool found = false;

			foreach (Sprite _sprite in _sprites) {

				if(_sprite.name == name)
				{
					sprite = _sprite;
					found = true;
					break;
				}
			}

			if (!found) {

				Texture2D tempText = new Texture2D(10, 10, TextureFormat.ARGB32, false);
				/*tempText.SetPixel(0, 0, Color.white);
				tempText.SetPixel(0, 1, Color.white);
				tempText.SetPixel(1, 0, Color.white);
				tempText.SetPixel(1, 1, Color.white);
				tempText.Apply();*/

				sprite = Sprite.Create(tempText, new Rect(0, 0, 10, 10), new Vector2());
				Debug.LogError("Sprite : "+name+" was not found!");
			}

			return sprite;

		}

	}

}
