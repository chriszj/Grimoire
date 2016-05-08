using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

using GLIB.Extended;

namespace GLIB.Interface
{

    /// <summary>
    /// A callout text useful for displaying score gains out of GameObjects. 
    /// This feature is experimental, use with caution.
    /// </summary>
    public class CallOutText
    {

        AnimateComponent _animateComponent;
        Text _textComponent;
        Outline _textOutlineComponent;
        GameObject _displayObject;

        public enum AnimationStyle : int {
            TRANSLATE_AND_SCALE = 0,
            TRANSLATE_AND_ROTATE = 1
        }

        /// <summary>
        /// Build a callout text, currently it only supports two animation styles, which are Translate & Scale and Translate & Rotate.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="duration"></param>
        /// <param name="animStyle"></param>
        /// <param name="parent"></param>
        /// <param name="worldTransformPosition"></param>
        public CallOutText(string text, Font font, int fontSize, Color textColor, float duration, AnimationStyle animStyle = AnimationStyle.TRANSLATE_AND_SCALE, float yTranslation = 10f, RectTransform parent = null, Vector3 worldTransformPosition = new Vector3()) {

            _displayObject = new GameObject("callOutText");

            _animateComponent = _displayObject.AddComponent<AnimateComponent>();
            _textComponent = _displayObject.AddComponent<Text>();
            _textOutlineComponent = _displayObject.AddComponent<Outline>();

            _textComponent.font = font;
            _textComponent.text = text;
            _textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            _textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            _textComponent.alignment = TextAnchor.MiddleCenter;            
            _textComponent.fontSize = fontSize;

            _textOutlineComponent.effectDistance = new Vector2(3, -3);

            if (parent == null)
                parent = _displayObject.transform.GetMainCanvas().GetComponent<RectTransform>();

            _displayObject.transform.SetParent(parent);

            // Convert worldTransformPosition into UI canvas position
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldTransformPosition);
            Vector3 anchored3DPosition = new Vector3(
             ((viewportPosition.x * parent.rect.width) - (parent.rect.width * 0.5f)),
             ((viewportPosition.y * parent.rect.height) - (parent.rect.height * 0.5f)),
             0
            );      

            // Clean weird transform when setting parent.
            _displayObject.transform.localPosition = anchored3DPosition;
            _displayObject.transform.localRotation =animStyle == AnimationStyle.TRANSLATE_AND_ROTATE ? Quaternion.Euler(new Vector3(90f, 0, 180f)) : Quaternion.identity;
            _displayObject.transform.localScale = animStyle == AnimationStyle.TRANSLATE_AND_SCALE ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(1, 1, 1);

            color = textColor;

            Action onAnimFinish = delegate { _animateComponent.InterpolateColor(color, new Color(color.r, color.g, color.b, 0), duration * 0.4f, delegate { color = animateComponent.interpolatedColor; }, delegate { GameObject.Destroy(_displayObject); }); };

            anchored3DPosition.y += yTranslation;

            _animateComponent.TranslateObject(anchored3DPosition, duration * 0.6f, delegate { onAnimFinish(); });
            
            if (animStyle == AnimationStyle.TRANSLATE_AND_SCALE)
                _animateComponent.ScaleObject(new Vector3(1f, 1f, 1f), duration * 0.6f, null);
            else if (animStyle == AnimationStyle.TRANSLATE_AND_ROTATE) 
                _animateComponent.RotateObject(new Vector3(), duration * 0.6f, null);

        }

        public AnimateComponent animateComponent {
            get
            {
                if (_animateComponent == null)
                    _animateComponent = _displayObject.AddComponent<AnimateComponent>();

                return _animateComponent;
            }
        }

        public Text textComponent {
            get {
                if (_textComponent == null)
                    _textComponent = _displayObject.AddComponent<Text>();

                return _textComponent;
            }
        }

        public string text {
            get { return textComponent.text; }
            set { textComponent.text = value; }
        }

        public Color color {
            get { return textComponent.color; }
            set { textComponent.color = value; }
        }

        public Font font {
            get { return textComponent.font; }
            set { textComponent.font = value; }
        }

        

    }

}
