using UnityEngine;
using System.Collections;

using System;
using System.Net;


public class GrimoireWebClient : WebClient  {

    protected override WebRequest GetWebRequest(Uri uri)
    {
        WebRequest w = base.GetWebRequest(uri);
        w.Timeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;//20 * 60 * 1000;
        return w;
    }

}
