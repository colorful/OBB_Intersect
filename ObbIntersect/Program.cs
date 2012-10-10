using System;
using System.Collections.Generic;

using ClassLibrary;
using Microsoft.Xna.Framework;

namespace ObbIntersect
{
    class Program
    {
        static void Main(string[] args)
        {
            OrientedBoundingBox x1 = new OrientedBoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            OrientedBoundingBox x2 = new OrientedBoundingBox(new Vector3(0, 0, 0), new Vector3(1, 2, 3));


            x1.Transforms = new Matrix(1.00f, 0.00f, 0.00f, 0.00f,
                                    0.00f, 1.00f, 0.00f, 0.00f,
                                    0.00f, 0.00f, 1.00f, 0.00f,
                                    0.00f, 0.00f, 0.00f, 1.00f);

            x2.Transforms = new Matrix(0.84739756089084f, 0.27533615807316f, 0.45399049973955f, 0.00000000000000f,
                                    -0.37155081922366f, -0.30330867844183f, 0.87747013300516f, 0.00000000000000f,
                                    0.37929851374673f, -0.91224659256112f, -0.15472165921466f, 0.00000000000000f,
                                    0.00000000000000f, 0.00000000000000f, 1.01000000000000f, 1.00000000000000f);

            bool res = x1.Intersects(x2);
        }
    }
}
