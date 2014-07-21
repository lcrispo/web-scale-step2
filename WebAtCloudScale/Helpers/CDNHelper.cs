using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebAtScale.Helpers
{
    public static class CDNHelper
    {
        public static string CDN(this UrlHelper helper, string target)
        {
            string CDNTarget = "";
            if (WebConfigurationManager.AppSettings["BlobStore"] == "CDN") 
            {
                Uri URITarget = new Uri(target);
                CDNTarget = WebConfigurationManager.AppSettings["CDNEndpoint"] + URITarget.AbsolutePath;
            }
            else 
            {
                CDNTarget = helper.Content(target);
            }
            return  CDNTarget;

        }

   
    }
}