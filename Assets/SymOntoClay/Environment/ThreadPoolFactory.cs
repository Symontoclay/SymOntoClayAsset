using SymOntoClay.Threading;
using System.Threading;

namespace Assets.SymOntoClay.Environment
{
    public static class ThreadPoolFactory
    {
        public static ICustomThreadPool Create(CancellationToken cancellationToken)
        {
            var threadingSettings = DefaultThreadingSettings.ConfigureWorldThreadingSettings()?.AsyncEvents;

            return new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                cancellationToken);
        }
    }
}
