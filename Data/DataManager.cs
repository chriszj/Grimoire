using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Security.Cryptography;

using LitJson;

using GLIB.Core;
using GLIB.Extended;

using ICSharpCode.SharpZipLib.Zip;
using System.IO;

/// <summary>
/// Data manager allow to download files from URL and store them in a particular mobile storage path.
/// Automatically creates a graphical loader windows that automatically closes when downloads are done.
/// 
/// * To Download File call DownloadFile(string _url, string _savePath, bool _showDownloadWindow = true);
/// * To remove a File call RemoveFile(string _filePath);
/// * When DownloadFile(...) is called multiple times the consequent downloads will be queued
/// 
/// </summary>

namespace GLIB.Data  {

	public class DataManager : Singleton<DataManager> {

		#region BackModule Overrides

		/*public override void OnApplicationQuit ()
		{
			base.OnApplicationQuit ();
			WriteCacheData ();
		}*/

		#endregion

		#region Variables

		/*AppPrefs _appPrefs;
		public AppPrefs AppPreferences {get{if( _appPrefs == null) _appPrefs = new AppPrefs(); return _appPrefs;}}

		Device _deviceData;
		public Device DeviceData {get{if(_deviceData == null) _deviceData = new Device(); return _deviceData;}}

		User _user;

		public UserData UserData{ get{
				if(_user == null)
					_user = new User();

				return _user.Data;
			}
		}*/

		#endregion

		#region SetUserData variables

		public delegate void OnLoadUserDataDoneDelegate();
		OnLoadUserDataDoneDelegate _onLoadUserDataDone;
		public OnLoadUserDataDoneDelegate OnLoadUserDataDone{get{return _onLoadUserDataDone;} set{_onLoadUserDataDone = value;}}

		public delegate void OnLoadUserDataFailDelegate(string errorMessage);
		OnLoadUserDataFailDelegate _onLoadUserDataFail;
		public OnLoadUserDataFailDelegate OnLoadUserDataFail{get{return _onLoadUserDataFail;} set{_onLoadUserDataFail = value;}}
			

		#endregion

		#region Encryption

		/*string PasswordHash { get{return UserData.Token;}}
		readonly string SaltKey = "0zn83iM77fQ2SYXj";
		readonly string VIKey = "@1B2c3D4e5F6g7H8";*/

		#endregion

		#region Monodevelop Methods


		void Awake() {
			gameObject.GroupIntoBackModuleObject ();
		}

		/*void OnApplicationPause (bool pause)
		{
			if (pause)
				WriteCacheData ();
		}*/

		#endregion

		#region SetUserData Methods

		/*public IEnumerator LoadUserData(string dataSource, OnLoadUserDataDoneDelegate onLoadDone = null, OnLoadUserDataFailDelegate onLoadFail = null, bool displayDownloadProgress = true){

			_onLoadUserDataDone = onLoadDone;
			_onLoadUserDataFail = onLoadFail;

			bool userDataSet = false;
			bool userValidated = false;

			try{



				//TODO HARDCODED PARSER
				//dataSource = dataSource.Replace("\\", "");
				//dataSource = dataSource.Replace("\u0022", "\"");

				Debug.LogError(dataSource);

				_user = JsonMapper.ToObject<User> (dataSource);

				userDataSet = true;

				if (_user.ErrorType != 0) {
					
					string errorMessage = _user.Message;
					
					_user = null;
					
					throw new Exception (errorMessage);
				}

				userValidated = true;


			}
			catch(Exception e)
			{
				Debug.LogError(e.Message+"\n"+e.StackTrace);

				if(_onLoadUserDataFail != null) {

					// Send a user friendly message from Application Strings
					string friendlyErrorMsg = "";

					if(!userDataSet)
						friendlyErrorMsg = ApplicationStrings.DATAMANAGER_LOADUSERDATA_USERSET_FAIL;
					else 
						friendlyErrorMsg = ApplicationStrings.DATAMANAGER_LOADUSERDATA_USERVALIDATE_FAIL;

					_onLoadUserDataFail(friendlyErrorMsg);
				}

				yield break;

			}

			//Data Download
			var routine = this.StartCoroutine<bool> ( NetClient.Instance.ProcessDownloads (displayDownloadProgress));
			
			yield return routine.coroutine;

			try {

				Debug.Log(routine.Value);
								
				if(_onLoadUserDataDone != null)
					_onLoadUserDataDone();
			
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message+"\n"+e.StackTrace);
				
				if(_onLoadUserDataFail != null)
					_onLoadUserDataFail(ApplicationStrings.NETCLIENT_DATAFETCH_FAIL);
								
				yield break;
			}


		}*/

		#endregion

		#region DataManagement Methods
		/// <summary>
		/// Attemtps to load user data with stored credential.
		/// </summary>
		/// <returns>The data with credential.</returns>
		/*
		public void LoadCacheData(){

			if (System.IO.File.Exists(ApplicationPaths.CACHE_FILEPATH)) {

				try {

					string plainText = "";
					string result = "";

					// Cache
					plainText = System.IO.File.ReadAllText (ApplicationPaths.CACHE_FILEPATH);
					result = Decrypt(plainText);

					_user = JsonMapper.ToObject<User> (result);
					//

					// Prefs
					plainText = System.IO.File.ReadAllText(ApplicationPaths.APPPREFS_FILEPATH);
					result = Decrypt(plainText);

					_appPrefs = JsonMapper.ToObject<AppPrefs>(result);
					//


				}
				catch (Exception e)
				{
					ClearCacheData();
					throw new Exception("Decription failed, wrong password");
				}

			}
			else
				throw new Exception("Cache file not found.");

		}
		
		public void WriteCacheData(){

			// If the user has not logged in then don't write anything
			if (!string.IsNullOrEmpty (UserData.Token)) {

				// Cache
				string cacheData = JsonMapper.ToJson (_user);

				Debug.Log ("Saved cacheData as = " + cacheData);

				string encCache = Encrypt (cacheData);
				//

				// AppPrefs
				string prefsData = JsonMapper.ToJson(_appPrefs);

				Debug.Log("Saved prefs as = "+prefsData);

				string encPrefs = Encrypt(prefsData);
				//

				Directory.CreateDirectory(ApplicationPaths.USER_PATH);

				// Write the files
				System.IO.File.WriteAllText (ApplicationPaths.CACHE_FILEPATH, encCache);
				System.IO.File.WriteAllText(ApplicationPaths.APPPREFS_FILEPATH, encPrefs);

				#if UNITY_IOS
				
				UnityEngine.iOS.Device.SetNoBackupFlag(ApplicationPaths.CACHE_FILEPATH);
				UnityEngine.iOS.Device.SetNoBackupFlag(ApplicationPaths.APPPREFS_FILEPATH);
				
				#endif
				
			}

		}

		public void ClearCacheData(){
			
			PlayerPrefs.DeleteAll ();

			_user = null;
		
			_appPrefs = null;

			_deviceData = null;

		}


		public void DeleteAllData(){

			ClearCacheData ();

			System.IO.Directory.Delete (Application.persistentDataPath, true);

		}
		*/
		#endregion

		#region Tool Methods

		public void DecompressFile(string filePath, bool deleteFileAfterDone = false){

			try {

				bool isCorrupted = false;

				using (ZipFile zip = new ZipFile(filePath)){

					if(!zip.TestArchive(true))
						isCorrupted = true;
				}

				if(isCorrupted) {
					System.IO.File.Delete(filePath);
					throw new Exception("Error, File: "+filePath+" is Corrupted.");
				}

				using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(filePath))) {
					
					ZipEntry theEntry;
					while ((theEntry = zipInputStream.GetNextEntry()) != null) {
						
						string directoryName = Path.GetDirectoryName(theEntry.Name);
						string fileName      = Path.GetFileName(theEntry.Name);
						
						// create directory
						if ( directoryName.Length > 0 ) {
							Directory.CreateDirectory(Application.persistentDataPath+"/"+directoryName);
						}

						if (fileName != String.Empty) {

							using (FileStream streamWriter = File.Create(Path.GetDirectoryName(filePath)+"/"+theEntry.Name)) {
								
								int size = 2048;
								byte[] data = new byte[2048];
								while (true) {
									size = zipInputStream.Read(data, 0, data.Length);
									if (size > 0) {
										streamWriter.Write(data, 0, size);
									} else {
										break;
									}
								}
							}

							#if UNITY_IOS
							
							UnityEngine.iOS.Device.SetNoBackupFlag(Path.GetDirectoryName(filePath)+"/"+theEntry.Name);
							
							#endif

						}
					}
				}

				if(deleteFileAfterDone)
					System.IO.File.Delete(filePath);

			}
			catch (Exception e)
			{
				throw new Exception("Error Unziping file: "+ filePath +" . Cause: "+e.Message+"\n"+e.StackTrace);
			}


		}

		#endregion

		#region DataEncryption

		/*public string GetMD5(string input)
		{
			using (MD5 m5dHash = MD5.Create()) {

				// Convert the input string to a byte array and compute the hash. 
				byte[] data = m5dHash.ComputeHash(Encoding.UTF8.GetBytes(input));
				
				// Create a new Stringbuilder to collect the bytes 
				// and create a string.
				StringBuilder sBuilder = new StringBuilder();
				
				// Loop through each byte of the hashed data  
				// and format each one as a hexadecimal string. 
				for (int i = 0; i < data.Length; i++)
				{
					sBuilder.Append(data[i].ToString("x2"));
				}
				
				// Return the hexadecimal string. 
				return sBuilder.ToString();
			}
		}
		
		public string Encrypt(string plainText)
		{
			
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			
			byte[] cipherTextBytes;
			
			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			return Convert.ToBase64String(cipherTextBytes);
		}

		public string Decrypt(string encryptedText)
		{
			byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
			byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
			
			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
			var memoryStream = new MemoryStream(cipherTextBytes);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];
			
			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}
		*/
		#endregion


	}

}
