using System;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Decorates a property that should not be displayed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayIgnoreAttribute : Attribute
    {
    }
}
