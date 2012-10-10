using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClassLibrary
{
    public class OrientedBoundingBox
    {
        public OrientedBoundingBox()
        {
        }

        public OrientedBoundingBox(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;

            UpdateFromMinMax();
        }

        //======================================================================

        public Vector3 Min
        {
            get { return _min; }
            set { _min = value; UpdateFromMinMax(); }
        }

        public Vector3 Max
        {
            get { return _max; }
            set { _max = value; UpdateFromMinMax(); }
        }

        public Vector3 Center
        {
            get { return _center; }
        }

        public Vector3 Extents
        {
            get { return _extents; }
        }

        public Matrix Transforms
        {
            get { return _transforms; }
            set { _transforms = value; }
        }

        //======================================================================

        public bool Intersects(OrientedBoundingBox other)
        {
            // Matrix to transform other OBB into my reference to allow me to be treated as an AABB
            Matrix toMe = other.Transforms * Matrix.Invert(_transforms);

            Vector3 centerOther = Utility.Multiply(other.Center, toMe);
            Vector3 extentsOther = other.Extents;
            Vector3 separation = centerOther - _center;

            Matrix3 rotations = new Matrix3(toMe);
            Matrix3 absRotations = Utility.Abs(rotations);

            float r, r0, r1, r01;

            //--- Test case 1 - X axis

            r = Math.Abs(separation.X);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
            r01 = _extents.X + r1;

            if (r > r01) return false;

            //--- Test case 1 - Y axis

            r = Math.Abs(separation.Y);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
            r01 = _extents.Y + r1;

            if (r > r01) return false;

            //--- Test case 1 - Z axis

            r = Math.Abs(separation.Z);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
            r01 = _extents.Z + r1;

            if (r > r01) return false;

            //--- Test case 2 - X axis

            r = Math.Abs(Vector3.Dot(rotations.Row(0), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(0));
            r01 = r0 + extentsOther.X;

            if (r > r01) return false;

            //--- Test case 2 - Y axis

            r = Math.Abs(Vector3.Dot(rotations.Row(1), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(1));
            r01 = r0 + extentsOther.Y;

            if (r > r01) return false;

            //--- Test case 2 - Z axis

            r = Math.Abs(Vector3.Dot(rotations.Row(2), separation));
            r0 = Vector3.Dot(_extents, absRotations.Row(2));
            r01 = r0 + extentsOther.Z;

            if (r > r01) return false;

            //--- Test case 3 # 1

            r = Math.Abs(separation.Z * rotations[0, 1] - separation.Y * rotations[0, 2]);
            r0 = _extents.Y * absRotations[0, 2] + _extents.Z * absRotations[0, 1];
            r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 2

            r = Math.Abs(separation.Z * rotations[1, 1] - separation.Y * rotations[1, 2]);
            r0 = _extents.Y * absRotations[1, 2] + _extents.Z * absRotations[1, 1];
            r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 3

            r = Math.Abs(separation.Z * rotations[2, 1] - separation.Y * rotations[2, 2]);
            r0 = _extents.Y * absRotations[2, 2] + _extents.Z * absRotations[2, 1];
            r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 4

            r = Math.Abs(separation.X * rotations[0, 2] - separation.Z * rotations[0, 0]);
            r0 = _extents.X * absRotations[0, 2] + _extents.Z * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 5

            r = Math.Abs(separation.X * rotations[1, 2] - separation.Z * rotations[1, 0]);
            r0 = _extents.X * absRotations[1, 2] + _extents.Z * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 6

            r = Math.Abs(separation.X * rotations[2, 2] - separation.Z * rotations[2, 0]);
            r0 = _extents.X * absRotations[2, 2] + _extents.Z * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 7

            r = Math.Abs(separation.Y * rotations[0, 0] - separation.X * rotations[0, 1]);
            r0 = _extents.X * absRotations[0, 1] + _extents.Y * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 8

            r = Math.Abs(separation.Y * rotations[1, 0] - separation.X * rotations[1, 1]);
            r0 = _extents.X * absRotations[1, 1] + _extents.Y * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 9

            r = Math.Abs(separation.Y * rotations[2, 0] - separation.X * rotations[2, 1]);
            r0 = _extents.X * absRotations[2, 1] + _extents.Y * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            return true;  // No separating axis, then we have intersection
        }

        //======================================================================

        protected void UpdateFromMinMax()
        {
            _center = (_min + _max) * 0.5f;
            _extents = (_max - _min) * 0.5f;
        }

        //======================================================================

        protected Vector3 _min;
        protected Vector3 _max;
        protected Vector3 _center;
        protected Vector3 _extents;
        protected Matrix _transforms = Matrix.Identity;
    }
}
