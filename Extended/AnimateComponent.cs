using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLIB.Extended
{

    /// <summary>
    /// Animation component that lets you perform local translation, local rotation, and local scaling. 
    /// Currently only supports one type of transition.
    /// </summary>
    public class AnimateComponent:MonoBehaviour
    {

        public delegate void OnTranslationFinishDelegate();
        private OnTranslationFinishDelegate _onTranslationFinish;

        public delegate void OnRotationFinishDelegate();
        private OnRotationFinishDelegate _onRotationFinish;

        public delegate void OnScaleFinishDelegate();
        private OnScaleFinishDelegate _onScaleFinish;

        public delegate void OnColorInterpolationFinishDelegate();
        private OnColorInterpolationFinishDelegate _onColorInterpolationFinish;

        public delegate void OnColorInterpolationUpdateDelegate();
        private OnColorInterpolationUpdateDelegate _onColorInterpolationUpdate;

        private bool _translating;
        private bool _rotating;
        private bool _scaling;
        private bool _interpolatingColor;

        public bool isTranslating {
            get { return _translating; }
        } 

        public bool isRotating {
            get { return _rotating; }
        }

        public bool isScaling {
            get { return _scaling; }
        }

        public bool isInterpolatingColor {
            get { return _interpolatingColor; }
        }
        
        private float _translationTime;
        private float _translationDuration;
        private float _translationPercent;
        private Vector3 _translationTarget;

        private float _rotationTime;
        private float _rotationDuration;
        private float _rotationPercent;
        private Vector3 _rotationTarget;

        private float _scaleTime;
        private float _scaleDuration;
        private float _scalePercent;
        private Vector3 _scaleTarget;

        private float _colorInterpolationTime;
        private float _colorInterpolationDuration;
        private float _colorInterpolationPercent;
        private Color32 _colorInterpolationStart;
        private Color32 _colorInterpolationEnd;
        private Color32 _interpolatedColor;
        public Color32 interpolatedColor {
            get { return _interpolatedColor; }
        }

        void Awake() {
            _translating = false;
            _rotating = false;
            _scaling = false;
            _interpolatingColor = false;
        }

        void Start() {
            
        }

        void Update()
        {
            
            //Check if it is a UIObject by getting its rectTransform;
            RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();

            if (_translating) {

                _translationTime += Time.deltaTime;
                _translationPercent = _translationTime / _translationDuration;
                // _translationPercent = _translationPercent * _translationPercent * _translationPercent * (_translationPercent * (6f * _translationPercent - 15f) + 10f);

                if (rectTransform)
                {
                    rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, _translationTarget, _translationPercent);
                   
                }
                else
                this.gameObject.transform.localPosition = Vector3.Lerp(this.gameObject.transform.localPosition, _translationTarget, _translationPercent);

                if (_translationPercent >= 1) {

                    _translating = false;

                    if (_onTranslationFinish != null)
                        _onTranslationFinish();

                }

            }

            if (_rotating) {

                _rotationTime += Time.deltaTime;
                _rotationPercent = _rotationTime / _rotationDuration;
              //  _rotationPercent = _rotationPercent * _rotationPercent * _rotationPercent * (_rotationPercent * (6f * _rotationPercent - 15f) + 10f);

                this.gameObject.transform.localRotation = Quaternion.Lerp(this.gameObject.transform.localRotation, Quaternion.Euler(_rotationTarget), _rotationPercent);

                if (_rotationPercent >= 1) {

                    _rotating = false;

                    if (_onRotationFinish != null)
                        _onRotationFinish();

                }

            }

            if (_scaling) {

                _scaleTime += Time.deltaTime;
                _scalePercent = _scaleTime / _scaleDuration;
               // _scalePercent = _scalePercent * _scalePercent * _scalePercent * (_scalePercent * (6f * _scalePercent - 15f) + 10f);

                this.gameObject.transform.localScale = Vector3.Lerp(this.gameObject.transform.localScale, _scaleTarget, _scalePercent);

                if (_scalePercent >= 1) {

                    _scaling = false;

                    if (_onScaleFinish != null)
                        _onScaleFinish();

                }

            }

            if (_interpolatingColor) {

                _colorInterpolationTime += Time.deltaTime;
                _colorInterpolationPercent = _colorInterpolationTime / _colorInterpolationDuration;

                _interpolatedColor = Color.Lerp(_interpolatedColor, _colorInterpolationEnd, _colorInterpolationPercent);

                if (_onColorInterpolationUpdate != null)
                    _onColorInterpolationUpdate();

                if (_colorInterpolationPercent >= 1) {

                    _interpolatingColor = false;

                    if (_onColorInterpolationFinish != null)
                        _onColorInterpolationFinish();

                }
                                
            }

        }

        /// <summary>
        /// Interpolates the object's translation from A to B(translateTo) in local coordinates;
        /// </summary>
        /// <param name="translateTo"></param>
        /// <param name="duration"></param>
        /// <param name="onTranslationFinish"></param>
        public void TranslateObject(Vector3 translateTo,  float duration, OnTranslationFinishDelegate onTranslationFinish)
        {
            _translating = true;

            _translationTarget = translateTo;
            _translationTime = 0;
            _translationDuration = duration;
            _onTranslationFinish = onTranslationFinish;
            
        }

        /// <summary>
        /// Interpolates the object's rotation from A to B(rotateTo) in local rotation;
        /// </summary>
        /// <param name="rotateTo"></param>
        /// <param name="duration"></param>
        /// <param name="onRotationFinish"></param>
        public void RotateObject(Vector3 rotateTo, float duration, OnRotationFinishDelegate onRotationFinish) {

            _rotating = true;

            _rotationTarget = rotateTo;
            _rotationTime = 0;
            _rotationDuration = duration;
            _onRotationFinish = onRotationFinish;

        }

        /// <summary>
        /// Interpolates the object's scale from A to B(scaleTo) in local scale;
        /// </summary>
        /// <param name="scaleTo"></param>
        /// <param name="duration"></param>
        /// <param name="onScaleFinish"></param>
        public void ScaleObject(Vector3 scaleTo, float duration, OnScaleFinishDelegate onScaleFinish) {

            _scaling = true;

            _scaleTarget = scaleTo;
            _scaleTime = 0;
            _scaleDuration = duration;
            _onScaleFinish = onScaleFinish;

        }

        /// <summary>
        /// Perform a Color Interpolation from A to B, use InterpolatedColor read property to apply to any object color of you like.
        /// You may use onUpdate to set the interpolated color to a custom property of yours, but have in mind that this process might be heavy.
        /// </summary>
        /// <param name="fadeTo"></param>
        /// <param name="duration"></param>
        /// <param name="onFadeFinish"></param>
        public void InterpolateColor(Color colorA, Color colorB, float duration, OnColorInterpolationUpdateDelegate onUpdate, OnColorInterpolationFinishDelegate onFinish) {

            _interpolatingColor = true;

            _colorInterpolationStart = colorA;
            _interpolatedColor = _colorInterpolationStart;
            _colorInterpolationEnd = colorB;
            _colorInterpolationTime = 0;
            _colorInterpolationDuration = duration;
            _onColorInterpolationUpdate = onUpdate;
            _onColorInterpolationFinish = onFinish;

        }

        /// <summary>
        /// Stops all animations being applied to the object, the object transform won't be reverted to its original values.
        /// That is the object's position, rotation, and scale values won't be reverted to the starting values before animating.
        /// </summary>
        public void StopAllAnimations() {
            _translating = false;
            _rotating = false;
            _scaling = false;
            _interpolatingColor = false;
        }

    }
}
