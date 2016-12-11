using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.ComponentModel;
using GLIB.Core;
using GLIB.Extended;


using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.Threading;
//using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Configuration;
using Microsoft.Win32;
using System.Linq;


using System.IO.IsolatedStorage;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using GLIB.Utils;

namespace GLIB.Net {

	public class NetClient : Singleton<NetClient> {

		#region WebRequest properties 

		bool _requestErrorOcurred = false;
		string _remoteString = "";
		public string RemoteString {get{return _remoteString;}}

		public delegate void OnWebRequestDoneDelegate(string stringRequested);
		OnWebRequestDoneDelegate _onWebRequestDone;

		public delegate void OnWebRequestFailDelegate();
		OnWebRequestFailDelegate _onWebRequestFail;

        float _webRequestMaxTimeOut = 20.0f;
        float _webRequestTimeToTimeOut = 0;
        
        #endregion

		#region FileDownload properties
		List<RemoteFileMetaData> _downloadList = new List<RemoteFileMetaData>();
		GrimoireWebClient _activeClient;
        public GrimoireWebClient ActiveWebClient {
            get { return _activeClient; }
        }

		float _allFilesDownloadProgress;
		public float allFilesDownloadProgress {get{ return _allFilesDownloadProgress; } }

        float _singleFileDownloadProgress;
        public float currentFileDownloadProgress { get { return _singleFileDownloadProgress; } }

        // For TimeOut Exception
        float _singleFileDownloadPrevProgress;
        float _secondsWithoutFileDownloadResponse;
        float _maxSecondsWithoutFileDownloadResponse = 20.0f;
        public float maxSecondsWithoutFileDownloadResponse { get { return _maxSecondsWithoutFileDownloadResponse; } set { _maxSecondsWithoutFileDownloadResponse = value; } }
        int _singleFileDownloadRetries;
        int _maxSingleFileDownloadRetries = 1;
        public int maxSingleFileDownloadRetries { get { return _maxSingleFileDownloadRetries; } set { _maxSingleFileDownloadRetries = value; } }
        //

        float _allFilesDecompressProgress;
        public float allFilesDecompress { get { return _allFilesDecompressProgress; } }
        
        List<bool> _filesDownloaded;

		bool _errorOcurred = false;

		bool _downloading = false;
		public bool IsDownloading {get{return _downloading;}}

		#endregion

		#region DecompressionQueue

		bool _decompressing;
		List<string> _filesToDecompress = new List<String>();

        public delegate void FileDecompressionMethod(string filePath);
        private FileDecompressionMethod _fileDecompressionMethod;
        /// <summary>
        /// Set the Decompression Method as your desire, your method must raise exceptions!
        /// </summary>
        public FileDecompressionMethod fileDecompressionMethod {
            set {
                _fileDecompressionMethod = value;
            }
        }

		#endregion

		void Awake() {
			gameObject.GroupIntoBackModuleObject ();

			_activeClient = new GrimoireWebClient ();
			_activeClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnFileDownload);
			_activeClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler (OnFileDownloadProgress);
			_activeClient.UploadValuesCompleted += new UploadValuesCompletedEventHandler (OnUploadValuesCompleted);
			_activeClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler (OnStringDownloadCompleted);

            _filesDownloaded = new List<bool>();
		}


		// Use this for initialization
		void Start () {
				
			
		}
		
		// Update is called once per frame
		void Update () {
			
			if (_downloading) {

                try
                {
                    if (!_activeClient.IsBusy)
                    {

                        lock (_filesDownloaded)
                        {

                            if (_filesDownloaded.Count < _downloadList.Count)
                            {

                                // Reset timeout and retries
                                _secondsWithoutFileDownloadResponse = _maxSecondsWithoutFileDownloadResponse;
                                _singleFileDownloadRetries = _maxSingleFileDownloadRetries;

                                Debug.Log("Downloading File: " + (_filesDownloaded.Count + 1) + " / " + _downloadList.Count);
                                RemoteFileMetaData fileToDownload = _downloadList[_filesDownloaded.Count];
                                System.Uri url = new System.Uri(fileToDownload.fileURI);
                                string fileName = _downloadList[_filesDownloaded.Count].savePath + "/" + (string.IsNullOrEmpty(fileToDownload.saveAsFileName) ? Path.GetFileName(url.AbsolutePath) : fileToDownload.saveAsFileName);
                                Debug.Log("Downloading file " + fileName + " at: " + url.AbsoluteUri);
                                _activeClient.DownloadFileAsync(url, fileName);


                            }

                        }

                    }
                    else
                    {
                        // Timeout handling
                        if (_singleFileDownloadPrevProgress == _singleFileDownloadProgress && _secondsWithoutFileDownloadResponse > 0)
                        {

                            _secondsWithoutFileDownloadResponse -= Time.deltaTime;

                            if (_secondsWithoutFileDownloadResponse < 0)
                            {

                                _secondsWithoutFileDownloadResponse = _maxSecondsWithoutFileDownloadResponse;
                                // Cancel download
                                _activeClient.CancelAsync();

                                _singleFileDownloadRetries--;
                                if (_singleFileDownloadRetries < 0)
                                {
                                    _singleFileDownloadRetries = _maxSingleFileDownloadRetries;
                                    throw new Exception("Could not download file, timeout exceed exception and number of retries exceeded.");

                                }

                            }

                        }

                    }
                }
                catch (Exception e) {

                    //Skip the download and trigger error, 
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                    _filesDownloaded.Add(false);
                    _errorOcurred = true;

                }
                finally {

                    if (_filesDownloaded.Count >= _downloadList.Count && _downloadList.Count > 0)
                    {

                        //for IOS only set no backup flag 
                        #if UNITY_IOS

						foreach(RemoteFileMetaData fileMeta in _downloadList)
						{

							string fileNamePath = fileMeta.savePath +"/"+ (string.IsNullOrEmpty(fileMeta.saveAsFileName)?Path.GetFileName(fileMeta.fileURI):fileMeta.saveAsFileName);

							if(System.IO.File.Exists(fileNamePath)){
								UnityEngine.iOS.Device.SetNoBackupFlag (fileNamePath);
							}

						}
                        #endif

                        _downloadList.Clear();
                        _filesDownloaded.Clear();

                    }

                }
				
			}

		}

		public bool InternetConnectivity (){
		
			try
			{
				_activeClient.OpenRead("http://www.google.com");
				return true;
			}
			catch (WebException e)
			{
				Debug.LogError(e.Message + "\n No Internet connectivity.\n"+e.StackTrace);
				return false;
			}

		}

		public IEnumerator MakeWebRequest(string uri, NameValueCollection parameters, OnWebRequestDoneDelegate onWebRequestDoneFunction = null, OnWebRequestFailDelegate onWebRequestFailFunction = null){

            _webRequestTimeToTimeOut = _webRequestMaxTimeOut;

			_onWebRequestDone = onWebRequestDoneFunction;
			_onWebRequestFail = onWebRequestFailFunction;
					
			var remoteStringRoutine = this.StartCoroutine<string>(SendWebRequest(uri, parameters));

            bool timeOutException = false;

            if (_webRequestTimeToTimeOut > 0) {
                _webRequestTimeToTimeOut -= Time.deltaTime;
                yield return remoteStringRoutine.coroutine;
            }
            else {
                timeOutException = true;
            }

			try {

                if (timeOutException)
                    throw new TimeoutException("Exceeded time making web request");

				string result = remoteStringRoutine.Value;

				if(_onWebRequestDone != null)
					_onWebRequestDone(result);

			}
			catch (Exception e)
			{
				Debug.LogError(e.Message+"\n"+e.StackTrace);

				if(_onWebRequestFail != null)
					_onWebRequestFail();
			}

		}

		IEnumerator SendWebRequest (string uri, NameValueCollection parameters)
		{

            // Handle https.
            ServicePointManager.ServerCertificateValidationCallback = CustomRemoteCertificateValidationCallback;

            _remoteString = null;

			if (parameters != null)
				_activeClient.UploadValuesAsync (new Uri (uri), "POST", parameters);
			else
				_activeClient.DownloadStringAsync(new Uri(uri));

			while (string.IsNullOrEmpty (_remoteString))
				yield return null;

			if(_requestErrorOcurred)
			{
				_requestErrorOcurred = false;
				throw new Exception(_remoteString);
			}

			yield return _remoteString;
							
		}

		public void OnUploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e){

			if (e.Error != null) {
				_requestErrorOcurred = true;
				_remoteString = e.Error.Message;
				Debug.LogError(e.Error.Message + "\n"+e.Error.StackTrace);
			} 
			else
				_remoteString = Encoding.UTF8.GetString (e.Result);

		}

		public void OnStringDownloadCompleted(object sender, DownloadStringCompletedEventArgs e){

			if (e.Error != null) {
				_requestErrorOcurred = true;
				_remoteString = e.Error.Message;
				Debug.LogError(e.Error.Message + "\n"+e.Error.StackTrace);
			} 
			else
				_remoteString = e.Result;
			
		}

		#region FileDownload Methods

		/// <summary>
		/// Adds the download file. if _saveAsFileName is empty or null then the file will be saved as it received in the server
		/// </summary>
		/// <param name="_fileURI">_file UR.</param>
		/// <param name="_savePath">_save path.</param>
		/// <param name="_saveAsName">_save as name.</param>
		/// <param name="requireDecompression">If set to <c>true</c> require decompression.</param>
		public void AddDownloadFile( string _fileURI, string _savePath, string _saveAsFileName, bool requireDecompression = false ){

            // Check if the file to download has a valid url
            Uri uri = new Uri(_fileURI);
            string urifileName = System.IO.Path.GetFileName(uri.AbsolutePath);

            if (string.IsNullOrEmpty(urifileName) || !Path.HasExtension(urifileName))
            {
                Debug.LogWarning("Attempted to download a file with an invalid URL: " + _fileURI);
                return;
            }

            //Check if file exist first
            string fileName = string.IsNullOrEmpty(_saveAsFileName)?Path.GetFileName (_fileURI):_saveAsFileName;

			if (System.IO.File.Exists (_savePath+"/"+fileName)) {
				Debug.Log ("File "+fileName+" at path: " + _savePath + " already Exists");
				return;
			} 

			//Create file directory so that the downloaded bytes can be written
			Directory.CreateDirectory(_savePath);
			Debug.Log ("Added Download item with url: " + _fileURI + " and path: " + _savePath);

			#if UNITY_IOS

			string noBackupPath = _savePath;

			// We don't want to store anything downloaded in the ICloud so we recursively set the nobackupflag on all the folders touched by the download
			while(noBackupPath != Application.persistentDataPath){

				UnityEngine.iOS.Device.SetNoBackupFlag (_savePath);
				noBackupPath = Path.GetDirectoryName(noBackupPath);
							
			}

			#endif
						
			//Add to the downloadList
			RemoteFileMetaData file = new RemoteFileMetaData (_fileURI, _savePath, _saveAsFileName, requireDecompression);
			_downloadList.Add (file);
			
		}
		
		void OnFileDownload(object sender, AsyncCompletedEventArgs e){

			_activeClient.Dispose();

			RemoteFileMetaData fetchedFileMetaData = _downloadList[_filesDownloaded.Count];

			string filePath = fetchedFileMetaData.savePath +"/"+ (string.IsNullOrEmpty(fetchedFileMetaData.saveAsFileName)?Path.GetFileName(fetchedFileMetaData.fileURI):fetchedFileMetaData.saveAsFileName);

			// There is a bug in DownloadFileAsync which sometimes triggers this function but the file is incomplete and no error is received.
			if (_singleFileDownloadProgress < 100 || e.Error != null) {
								                                 
				System.IO.File.Delete (filePath);

				if(e.Error != null)
				{
					_errorOcurred = true;
					Debug.LogError ("Failed Downloading File At URL: " + fetchedFileMetaData.fileURI + "\n" + e.Error.Message);

					// there was an error in the server-client request so we must skip the file!
					_filesDownloaded.Add(false);
					return;
				}

				// The file must be redownloaded!
				Debug.LogError("the file: "+filePath+" has not been downloaded completely. Trying to re-download the file.");
				return;
			}

            // If decompression method is null then files to be decompressed won't be added at all.
            if (fetchedFileMetaData.requireDecompresion)
            {
                if (_fileDecompressionMethod != null)
                    _filesToDecompress.Add(filePath);
                else
                    Debug.LogWarning("No Decompression Method Found, please assign on by using the fileDecompressionMethod property");
            }

            _filesDownloaded.Add(true);
						
		}
		
		void OnFileDownloadProgress(object sender, ProgressChangedEventArgs e){

            _allFilesDownloadProgress = ((e.ProgressPercentage / 100.0f) + _filesDownloaded.Count)/_downloadList.Count*100;
		
			_singleFileDownloadProgress = e.ProgressPercentage;

            _singleFileDownloadPrevProgress = _singleFileDownloadProgress;

		}
		
		// Yielded Capable function!
		// Can be used to continue with other stuff until downloads has finished!
		IEnumerator ProcessDownloads(){

			//_displayProgress = displayProgress;

			_downloading = true;
			
			while (_downloadList.Count > 0) {
				
				yield return null;
				
			}

			bool errorDecompressing = false;
            
			for(int i = 0; i < _filesToDecompress.Count; i++) {

				try{
                    
                    if (_fileDecompressionMethod != null)
                        _fileDecompressionMethod(_filesToDecompress[i]);
                    
                    _allFilesDecompressProgress = (i / 100.0f) / _filesToDecompress.Count;
                
                }
				catch(Exception e){

					Debug.LogError(e.Message);
					errorDecompressing = true;

					//If File exist delete it
					if(System.IO.File.Exists(_filesToDecompress[i]))
						System.IO.File.Delete(_filesToDecompress[i]);
				}

			}
            			
			_filesToDecompress = new List<String> ();

			_downloadList = new List<RemoteFileMetaData>();

            _downloading = false;

            if (_errorOcurred || errorDecompressing) {

				string errMessage = (_errorOcurred?"Failed to download Files.":"")+(errorDecompressing?"Failed to decompress files.":"");

				_errorOcurred = false;
				errorDecompressing = false;

				throw new Exception(errMessage);
			}

        }

		public IEnumerator DownloadFiles( Action onSuccess = null, Action onFail = null ){

			//Data Download
			var routine = this.StartCoroutine<bool> ( NetClient.Instance.ProcessDownloads());
			
			yield return routine.coroutine;
			
			try {

				Debug.Log(routine.Value);

				if(onSuccess != null)
					onSuccess();
				
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message+"\n"+e.StackTrace);
				
				if(onFail != null)
					onFail();
				
				yield break;
			}
		}

        #endregion

        #region HTTPS Handling

        public bool CustomRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        #endregion
    }

}
