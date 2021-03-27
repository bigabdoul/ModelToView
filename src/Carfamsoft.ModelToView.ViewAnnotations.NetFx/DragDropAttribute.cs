﻿using System;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Indicates that an object supports drag and drop operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DragDropAttribute : FileCapableAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropAttribute"/> class.
        /// </summary>
        public DragDropAttribute()
        {
        }
    }
}
