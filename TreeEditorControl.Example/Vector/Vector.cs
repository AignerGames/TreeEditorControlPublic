using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeEditorControl.Utility;

namespace TreeEditorControl.Example
{
    public class Vector : ObservableObject
    {
        private float _x;
        private float _y;
        private float _z;

        public Vector()
        {
        }

        public Vector(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float X
        {
            get => _x;
            set => SetAndNotify(ref _x, value);
        }

        public float Y
        {
            get => _y;
            set => SetAndNotify(ref _y, value);
        }

        public float Z
        {
            get => _z;
            set => SetAndNotify(ref _z, value);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }

        public void CopyFrom(Vector vector)
        {
            if(vector == null)
            {
                return;
            }

            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }
    }
}
