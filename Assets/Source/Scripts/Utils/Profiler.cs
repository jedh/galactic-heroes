using System;
using System.Runtime.CompilerServices;

namespace GH.Utils
{
    public struct Profiler : IDisposable
    {
        public Profiler(string sampleName)
        {
            UnityEngine.Profiling.Profiler.BeginSample(sampleName);
        }

        public void Dispose()
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public static Profiler TakeSample([CallerMemberName] string memberName = "")
        {
            return new Profiler(memberName);
        }
    }
}
