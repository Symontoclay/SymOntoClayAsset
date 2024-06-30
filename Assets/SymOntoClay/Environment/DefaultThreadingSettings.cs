using SymOntoClay.Core;
using SymOntoClay.Threading;

namespace Assets.SymOntoClay.Environment
{
    public static class DefaultThreadingSettings
    {
        public static ThreadingSettings Settings = new ThreadingSettings
        {
            AsyncEvents = new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 50
            },
            CodeExecution = new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 50
            }
        };
    }
}
