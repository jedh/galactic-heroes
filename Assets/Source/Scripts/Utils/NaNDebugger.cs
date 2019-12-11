using UnityEngine;

namespace GH.Utils
{
    public static class NaNDebugger
    {
        [System.Diagnostics.Conditional("DEBUG_NAN")]
        public static void IsNan(float n, string msg)
        {
            if (float.IsNaN(n))
            {
                Debug.LogError(msg);
            }
        }
    }
}