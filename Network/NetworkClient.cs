namespace GLIB.Network
{

    using System;
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;

    public class NetworkClientException : Exception
    {
        public NetworkClientException() : base() { }
        public NetworkClientException(string message) : base(message) { }
    }

    public class NetworkClient : MonoBehaviour
    {

        bool IsProcessingRequest
        {
            get
            {
                return _isProcessingRequest;
            }
        }

        bool _isProcessingRequest;

        public virtual void SendRequest(string url, WWWForm form, Dictionary<string, string> headers, Action<string> onRequestResult)
        {
            if (!_isProcessingRequest)
            {
                byte[] rawData = form.data;
                Dictionary<string, string> hdrs = form.headers;

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        if (hdrs.ContainsKey(pair.Key))
                            hdrs[pair.Key] = pair.Value;
                        else
                            hdrs.Add(pair.Key, pair.Value);
                    }
                }

                WWW www = new WWW(url, rawData, hdrs);

                StartCoroutine(WaitForRequest(www, onRequestResult));
            }
            else
                throw new NetworkClientException("NetworkClient => Client is busy with another request.");
        }

        public virtual void SendRequest(string url, Dictionary<string, string> headers, Action<string> onRequestResult)
        {
            if (!_isProcessingRequest)
            {

                WWW www = new WWW(url, null, headers);

                StartCoroutine(WaitForRequest(www, onRequestResult));
            }
            else
                throw new NetworkClientException("NetworkClient => Client is busy with another request.");
        }

        public virtual void SendRequest(WWW www, Action<string> onRequestResult)
        {

            if (!_isProcessingRequest)
            {
                StartCoroutine(WaitForRequest(www, onRequestResult));
            }
            else
                throw new NetworkClientException("NetworkClient => Client is busy with another request.");

        }

        public virtual void SendRequest(UnityWebRequest request, Action<string> onRequestResult, Action<Exception> onRequestFail)
        {

            try
            {
                if (!_isProcessingRequest)
                {
                    StartCoroutine(WaitForRequest(request, onRequestResult, onRequestFail));
                }
                else
                    throw new NetworkClientException("NetworkClient => Client is busy with another request.");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private IEnumerator WaitForRequest(WWW www, Action<string> onRequestResult)
        {
            _isProcessingRequest = true;

            yield return www;
            _isProcessingRequest = false;

            // check for errors
            if (www.error == null)
            {
                //SignupToLobby(www.text);
                if (onRequestResult != null)
                    onRequestResult(www.text);
            }
            else
            {
                //Debug.Log("WWW Error: " + www.error);
                throw new NetworkClientException("NetworkClient => An error ocurred on request: " + www.error);
            }

        }

        private IEnumerator WaitForRequest(UnityWebRequest request, Action<string> onRequestResult, Action<Exception> onRequestFail)
        {

            _isProcessingRequest = true;

            yield return request.SendWebRequest();

            _isProcessingRequest = false;

            if (!request.isNetworkError)
            {

                if (onRequestResult != null)
                    onRequestResult(request.downloadHandler.text);

            }
            else
            {

                if (onRequestFail != null)
                    onRequestFail(new NetworkClientException("NetworkClient => An error ocurred on request: " + request.error));
                else
                    throw new NetworkClientException("NetworkClient => An error ocurred on request: " + request.error);
            }

        }


    }

}
