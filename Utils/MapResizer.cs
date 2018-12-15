namespace GLIB.Utils
{

    using System;

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    /// <summary>
    /// Simple tool that resizes a layout element and changes the scale of a button wrapper.
    /// </summary>
    public class MapResizer : MonoBehaviour
    {

        public enum ResizeMethods
        {
            EXPAND_TO_RECT_WIDTH = 0,
            EXPAND_TO_RECT_HEIGHT,
            EXPAND_TO_PERCENT
        }

        public Vector2 OriginalMapResolution = new Vector2(1000, 1000);

        public RectTransform ParentRect;

        public LayoutElement TargetLayoutElement;

        public GameObject ButtonWrapper;

        public ResizeMethods ResizeMethod = ResizeMethods.EXPAND_TO_RECT_WIDTH;

        [Range(0,100)]
        public float PercentExpand = .2f;

        Vector2 _lastParentRectSize = Vector2.zero;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (ParentRect.rect.size != _lastParentRectSize) {
                ResizeMap();
                _lastParentRectSize = ParentRect.rect.size;
            }

        }

        void ResizeMap() {

            try
            {

                Vector2 newResolution = new Vector2();
                Vector3 newScale = new Vector3();

                switch (ResizeMethod)
                {

                    case ResizeMethods.EXPAND_TO_RECT_WIDTH:

                        
                        newResolution.x = ParentRect.rect.size.x;
                        newResolution.y = (newResolution.x * OriginalMapResolution.y) / OriginalMapResolution.x;

                        newScale = new Vector3(newResolution.x / OriginalMapResolution.x, newResolution.y / OriginalMapResolution.y, newResolution.y / OriginalMapResolution.y);

                        break;

                    case ResizeMethods.EXPAND_TO_RECT_HEIGHT:

                        
                        newResolution.y = ParentRect.rect.size.y;
                        newResolution.x = (newResolution.y * OriginalMapResolution.x) / OriginalMapResolution.y;

                        newScale = new Vector3(newResolution.x / OriginalMapResolution.x, newResolution.y / OriginalMapResolution.y, newResolution.y / OriginalMapResolution.y);

                        break;

                    case ResizeMethods.EXPAND_TO_PERCENT:

                        if (ParentRect.rect.size.y <= ParentRect.rect.size.x)
                        {
                            newResolution.x = ParentRect.rect.size.x;
                            newResolution.y = (newResolution.x * OriginalMapResolution.y) / OriginalMapResolution.x;
                        }
                        else {
                            newResolution.y = ParentRect.rect.size.y;
                            newResolution.x = (newResolution.y * OriginalMapResolution.x) / OriginalMapResolution.y;
                        }

                        newResolution.Scale(new Vector2(PercentExpand, PercentExpand));

                        newScale = new Vector3(newResolution.x / OriginalMapResolution.x, newResolution.y / OriginalMapResolution.y, newResolution.y / OriginalMapResolution.y);
                                              
                        

                        break;


                }

                TargetLayoutElement.minWidth = newResolution.x;
                TargetLayoutElement.minHeight = newResolution.y;

                ButtonWrapper.transform.localScale = newScale;

            }
            catch (Exception e) {

                Debug.LogError("MapResizer => Unable to resize Map. Cause: " + e.Message + "\n" + e.StackTrace);

            }

        }

    }

}