using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Utils
{
    public static class VectorConvert
    {
        public static float XZDistance(Vector3 pos1, Vector3 pos2)
        {
            Vector3 pos12D = new Vector3(pos1.x, 0, pos1.z);
            Vector3 pos22D = new Vector3(pos2.x, 0, pos2.z);
            return Vector3.Distance(pos12D, pos22D);
        }

        public static Vector3 XZDirection(Vector3 start, Vector3 end)
        {
            return XZVector(start, end).normalized;
        }

        public static Vector3 XZVector(Vector3 start, Vector3 end)
        {
            Vector3 pos12D = new Vector3(start.x, 0, start.z);
            Vector3 pos22D = new Vector3(end.x, 0, end.z);
            return pos22D - pos12D;
        }

        public static Vector3 XZVector(Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }

    }

}