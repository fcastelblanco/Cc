 

using System;
using System.Reflection;
using Ionic.Zip;

namespace Isn.Common.Implementations
{

    public class Compress
    {

     

        public static void CompressFolder(string pZipFileToCreate, string pDirectoryToZip)
        {

            if (!System.IO.Directory.Exists(pDirectoryToZip))
            {
                Console.Error.WriteLine("exception: Directorio .zipya existe" );
            }
            if (System.IO.File.Exists(pZipFileToCreate))
            {
                Console.Error.WriteLine("exception: Directorio ya existe" );
            }
            if (!pZipFileToCreate.EndsWith(".zip"))
            {
                Console.Error.WriteLine("exception: el directorio a crear no es un zip" );
            }

            string ZipFileToCreate = pZipFileToCreate;
            string DirectoryToZip = pDirectoryToZip;
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.StatusMessageTextWriter = Console.Out;
                    zip.AddDirectory(DirectoryToZip);
                    zip.Save(ZipFileToCreate);
                }
            }
            catch (Exception ex1)
            {
                Console.Error.WriteLine("exception: " + ex1);
            }

        }
    }
}