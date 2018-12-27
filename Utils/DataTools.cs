namespace GLIB.Utils
{

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DataTools : MonoBehaviour
    {

        public static List<T> GetJSONList<T>(string json)
        {
            try
            {
                string newJson = "{ \"list\": " + json + "}";
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
                return wrapper.list;
            }
            catch (Exception e) {
                throw e;
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public List<T> list;
        }

    }

}
