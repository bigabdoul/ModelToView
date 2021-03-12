using System.Collections.Generic;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    internal sealed class ObjectDictionary : Dictionary<string, object>
    {
        public ObjectDictionary(object obj)
        {
            var props = obj.GetType().GetProperties();
            foreach (var pi in props) Add(pi.Name, pi.GetValue(obj));
        }
    }
}
