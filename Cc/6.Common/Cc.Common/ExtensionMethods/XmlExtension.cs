// Editado por: Freddy Castelblanco                                                                                     
// CASMASOFT 2015
// Todos los derechos reservados                                                                                                          
// Fecha de creación: 2015 - 05 - 10 - 6:23 p.m.
// Ultima edición: 2015 - 05 - 10 - 6:56 p.m.

using System;
using System.IO;
using System.Xml.Serialization;

namespace Cc.Common.ExtensionMethods
{
    public static class XmlExtension
    {
        public static void SaveClassAsXml<T>(this T theType, string path)
        {
            var writer =
                new XmlSerializer(typeof(T));

            var file = new StreamWriter(path);
            writer.Serialize(file, theType);
            file.Close();
        }

        public static T GetDataFromXml<T>(string path)
        {
            var mySerializer =
           new XmlSerializer(typeof(T));

            var myFileStream =
                new FileStream(path, FileMode.Open);

            var myObject = mySerializer.Deserialize(myFileStream);

            if (!(myObject is T)) throw new Exception("The type retrieved is not the type expected");
            
            myFileStream.Close();
            myFileStream.Dispose();
            return (T)myObject;
        }
    }
}