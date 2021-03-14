using UnityEngine;

namespace Megatowel.Debugging
{
    public static class MTDebug
    {
        const string logPrefix = "<color=#812776>[MTLog]</color>:";
        const string warnPrefix = "<color=#BA7240>[MTWarning]</color>:";
        const string errPrefix = "<color=#BA2C40>[MTError]</color>:";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void LoadDebugResources()
        {
            LineMaterial = Resources.Load<Material>("DebugLineMat");
        }

        private static Material LineMaterial;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public static bool Verbose = true;
#else
        public static bool Verbose = false;
#endif

        public static void Log(object value)
        {
            if (Verbose)
            {
                UnityEngine.Debug.Log($"{logPrefix} {value}");
            }
        }

        public static void Log(object value, UnityEngine.Object context)
        {
            if (Verbose)
            {
                UnityEngine.Debug.Log($"{logPrefix} {value}", context);
            }
        }

        public static void LogWarning(object value)
        {
            if (Verbose)
            {
                UnityEngine.Debug.LogWarning($"{warnPrefix} {value}");
            }
        }

        public static void LogWarning(object value, UnityEngine.Object context)
        {
            if (Verbose)
            {
                UnityEngine.Debug.LogWarning($"{warnPrefix} {value}", context);
            }
        }

        public static void LogError(object value)
        {
            if (Verbose)
            {
                UnityEngine.Debug.LogError($"{errPrefix} {value}");
            }
        }

        public static void LogError(object value, UnityEngine.Object context)
        {
            if (Verbose)
            {
                UnityEngine.Debug.LogError($"{errPrefix} {value}", context);
            }
        }

        public static void DrawLine(Vector3 start, Vector3 end, float lineWidth)
        {
            Vector3 normal = Vector3.Cross(start, end);
            Vector3 side = Vector3.Cross(normal, end - start);
            side.Normalize();
            Vector3 a = start + side * (lineWidth / 2);
            Vector3 b = start + side * (lineWidth / -2);
            Vector3 c = end + side * (lineWidth / 2);
            Vector3 d = end + side * (lineWidth / -2);
            Mesh linemesh = new Mesh();
            linemesh.SetVertices(new Vector3[] { a, b, c, d });
            linemesh.SetTriangles(new int[]
            {
                0, 1, 2,
                3, 2, 1
            }, 0);
            Graphics.DrawMesh(linemesh, Vector3.zero, Quaternion.identity, LineMaterial, 29);
        }
    }
}
