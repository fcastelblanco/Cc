// Editado por: Freddy Castelblanco                                                                                      
// Cms 2015
// Todos los derechos reservados                                                                                                          
// Fecha de creación: 2015 - 03 - 05 - 10:51 a.m.
// Ultima edición: 2015 - 04 - 23 - 09:49 a.m.

using System;
using System.Reflection;

namespace Isn.Common.Implementations
{
    public class HelperSingleton<T> where T : class
    {
        protected static readonly object SyncRoot = new object();
        private static T _theInstance;

        public static T Instance
        {
            get
            {
                if (_theInstance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_theInstance == null)
                        {
                            var constructorInfo =
                                typeof (T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                                    Type.EmptyTypes, null);

                            if (constructorInfo == null)
                            {
                                return null;
                            }

                            _theInstance = (T) constructorInfo.Invoke(null);
                        }
                    }
                }

                return _theInstance;
            }
            set { _theInstance = value; }
        }
    }
}
