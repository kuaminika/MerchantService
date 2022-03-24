using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MerchantService
{
    /// <summary>
    /// Instances of this class will instanciate anything inside of the same namespace
    /// </summary>
    class KInstanciater
    {
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public KInstanciater(Type _type)
        {
            this.AssemblyName = _type.Assembly.FullName;
            this.Namespace = _type.Namespace;
        }

        public object Instanciate(params object[] args)
        {
            if (string.IsNullOrEmpty(ClassName)) throw new Exception("ClassName is needed to instanciate");

            string fullReaderClassName = Namespace + "." + ClassName;
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            Type designatedType = Type.GetType(fullReaderClassName);
            if (args==null || args.Length ==0 )
            {
                object reult = Activator.CreateInstance(designatedType);
                return reult;
            }
            // CreateInstance(Type type, params object[] args);
            Object result = Activator.CreateInstance(designatedType, true, bindingFlags, null, args, null, null);

            return result;
        }
    }
}
