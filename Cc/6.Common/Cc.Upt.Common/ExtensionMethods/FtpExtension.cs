using System;
using System.IO;
using System.Net;
using Cc.Upt.Common.LogHelper;

namespace Cc.Upt.Common.ExtensionMethods
{
    public static class FtpExtension
    {
        public static bool Download(string file, string ftpServer, string ftpUser, string ftpPassword, int bufferSize,
            string downloadPath)
        {
            try
            {
                if (File.Exists(downloadPath + @"\" + file))
                {
                    File.Delete(downloadPath + @"\" + file);
                }

                var ftpfullpath = ftpServer + "/" + file;

                using (var request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(ftpUser, ftpPassword);

                    if (request.IsBusy)
                    {
                        return false;
                    }

                    var fileData = request.DownloadData(ftpfullpath);

                    using (var fileStream = File.Create(downloadPath + @"\" + file))
                    {
                        fileStream.Write(fileData, 0, fileData.Length);
                        fileStream.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                return false;
            }

            //var request = (FtpWebRequest)WebRequest.Create(ftpServer + "/" + file);
            //request.KeepAlive = true;
            //request.UsePassive = true;
            //request.UseBinary = true;
            //request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            //request.Method = WebRequestMethods.Ftp.DownloadFile;

            //using (var ftpResponse = (FtpWebResponse)request.GetResponse())
            //{
            //    using (var responseStream = ftpResponse.GetResponseStream())
            //    {
            //        var fileStream = new FileStream(downloadPath + @"\" + file, FileMode.Create);
            //        var byteBuffer = new byte[bufferSize];
            //        if (responseStream == null) return;

            //        var bytesRead = responseStream.Read(byteBuffer, 0, bufferSize);
            //        try
            //        {
            //            while (bytesRead > 0)
            //            {
            //                fileStream.Write(byteBuffer, 0, bytesRead);
            //                bytesRead = responseStream.Read(byteBuffer, 0, bufferSize);
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            Log.Instance.Error(e);
            //        }
            //    }
            //}
        }
    }
}