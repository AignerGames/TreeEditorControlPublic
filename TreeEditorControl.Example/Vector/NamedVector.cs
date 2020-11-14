using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example
{
    public class NamedVector : Vector
    {
        private string _name;

        public NamedVector()
        {
        }

        public NamedVector(string name, float x, float y, float z) : base(x, y, z)
        {
            _name = name;
        }

        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        public override string ToString()
        {
            return $"{Name} (X: {X} Y: {Y} Z: {Z})";
        }
    }
}
