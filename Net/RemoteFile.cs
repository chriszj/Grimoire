using UnityEngine;
using System.Collections;

namespace GLIB.Net {

	public class RemoteFileRequester {
		public bool errorWhileDownloading;
	}

	public class RemoteFileMetaData {
		
		public string fileURI;
		public string savePath;
		public string saveAsFileName;
		public bool requireDecompresion;

		public RemoteFileMetaData( string _fileURI, string _savePath, string _saveAsFileName, bool _requireDecompression){
			
			fileURI = _fileURI;
			savePath = _savePath;
			saveAsFileName = _saveAsFileName;
			requireDecompresion = _requireDecompression;
			
		}
		
	}

}