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
                MaxThreadsCount = 20,
                MinThreadsCount = 5
            },
            CodeExecution = new CustomThreadPoolSettings
            {
                MaxThreadsCount = 50,
                MinThreadsCount = 5
            }
        };
    }
}
