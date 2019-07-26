using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Downloader
/// </summary>
public static class Downloader
{
    public static void Download(string sFileName, string sFilePath)
    {
        HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
        String Header = "Attachment; Filename=" + sFileName;
        HttpContext.Current.Response.AppendHeader("Content-Disposition", Header);
        System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath));
        HttpContext.Current.Response.WriteFile(Dfile.FullName);
        HttpContext.Current.Response.End();
    }

}