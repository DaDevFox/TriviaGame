using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Utils.Extensions
{
    public static class MathExtensions { 
        public static Vector3 Abs(this Vector3 vec)
        {
            vec.x = Math.Abs(vec.x);
            vec.y = Math.Abs(vec.y);
            vec.z = Math.Abs(vec.z);

            return vec;
        }

        public static float DistSqrd(this Vector3 a, Vector3 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
        }

        public static float RoundToFactor(this float value, float factor){
            return Mathf.Round(value / factor) * factor;
        }


    }
}
