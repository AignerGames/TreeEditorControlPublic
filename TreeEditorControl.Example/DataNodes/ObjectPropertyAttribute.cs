﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.DataNodes
{
    public class ObjectPropertyAttribute : NodePropertyAttribute
    {
        public ObjectPropertyAttribute(string propertyName = null) : base(propertyName)
        {
        }
    }
}