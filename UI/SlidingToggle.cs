namespace GLIB.Utils
{

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using System;

    public class SlidingToggle : MonoBehaviour
    {

        public Slider Target;
        public float FillDuration = 0.8f;


        bool _isOn;
        bool _shouldSliderMove = false;
        float _timeSpan;
        bool _displayedError = false;

        public void SetValue(bool isOn) {

            _isOn = isOn;

            _timeSpan = 0;

            _shouldSliderMove = true;
            
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            try
            {

                if (_shouldSliderMove) {

                    float percent = _timeSpan / FillDuration;

                    Target.value = Mathf.Lerp(Target.value, _isOn ? Target.maxValue : Target.minValue, percent);

                    _timeSpan += Time.deltaTime;

                    if (percent >= 100) {
                        _shouldSliderMove = false;
                    }

                }

                _displayedError = false;

            }
            catch (Exception e)
            {
                if (!_displayedError)
                {
                    Debug.LogError("SlidingToggle => Unable to upate slider. Cause: "+e.Message+"\n"+e.StackTrace);
                    _displayedError = true;
                }
            }

        }

    }

}
