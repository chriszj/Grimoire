namespace GLIB.Network
{
        
    using UnityEngine;
    using UnityEngine.Networking;

    using System;
    using System.IO;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    using GLIB.Libraries;

    public class DownloadManager : MonoBehaviour
    {

        NetworkClient _networkClient {
            get
            {
                if (!m_networkClient)
                {
                    m_networkClient = Toolbox.ResolveComponent<NetworkClient>();
                }

                return m_networkClient;
            }

        }

        NetworkClient m_networkClient;

        Dictionary<UnityWebRequest, bool> _downloadQueue = new Dictionary<UnityWebRequest, bool>();

        List<DownloadHandler> _downloadHandlersCache = new List<DownloadHandler>();
        List<Exception> _networkErrorsCache = new List<Exception>();

        public void AddDownloadRequest(string url, string filepathToSave, Action<UnityWebRequest> onRequestBuild = null) {                      

            if (File.Exists(filepathToSave)) {
                Debug.LogWarning("DownloadManager => File from url: "+url+" \nto save at: "+filepathToSave+" already exist.");
                return;
            }

            UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);

            //File.Create(filepathToSave).Dispose();

            Directory.CreateDirectory(Path.GetDirectoryName(filepathToSave));

            DownloadHandlerFile downloadHandler = new DownloadHandlerFile(filepathToSave);

            request.downloadHandler = downloadHandler;
                                                
            _downloadQueue.Add(request, false);

        }

        public void ProcessDownloads(Action<List<DownloadHandler>> onDownloadsFinished, Action<List<Exception>> onDownloadsFail) {

            _downloadHandlersCache.Clear();
            _networkErrorsCache.Clear();

            DownloadNext(onDownloadsFinished, onDownloadsFail);

        }

        void DownloadNext(Action<List<DownloadHandler>> onDownloadsFinished, Action<List<Exception>> onDownloadsFail) {

            // Get the next request
            UnityWebRequest request = _downloadQueue.Where(kvp => kvp.Value == false).Select(kvp => kvp.Key).FirstOrDefault();

            if (request != null)
            {

                Action<DownloadHandler> onSuccess = delegate (DownloadHandler handler)
                {

                    _downloadQueue[request] = true;

                    _downloadHandlersCache.Add(handler);

                    DownloadNext(onDownloadsFinished, onDownloadsFail);

                };

                Action<Exception> onFail = delegate (Exception exception)
                {

                    _downloadQueue[request] = true;

                    _networkErrorsCache.Add(exception);

                    DownloadNext(onDownloadsFinished, onDownloadsFail);

                };

                _networkClient.SendDownloadRequest(request, onSuccess, onFail);

            }
            else
            {

                if (_networkErrorsCache.Count > 0 && onDownloadsFail != null)
                    onDownloadsFail(_networkErrorsCache);
                else if (onDownloadsFinished != null)
                    onDownloadsFinished(_downloadHandlersCache);
                               

            }

        }
                

    }

}
