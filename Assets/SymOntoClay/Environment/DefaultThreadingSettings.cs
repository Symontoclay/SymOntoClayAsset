using SymOntoClay.Core;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;

namespace Assets.SymOntoClay.Environment
{
    public static class DefaultThreadingSettings
    {
        public static void ConfigureThreadingSettings(WorldSettings settings)
        {
            settings.WorldThreadingSettings = ConfigureWorldThreadingSettings();
            settings.HumanoidNpcDefaultThreadingSettings = ConfigureHumanoidNpcDefaultThreadingSettings();
            settings.PlayerDefaultThreadingSettings = ConfigurePlayerDefaultThreadingSettings();
            settings.GameObjectDefaultThreadingSettings = ConfigureGameObjectDefaultThreadingSettings();
            settings.PlaceDefaultThreadingSettings = ConfigurePlaceDefaultThreadingSettings();
        }

        public static ThreadingSettings ConfigureWorldThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigureHumanoidNpcDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                }
            };
        }

        public static ThreadingSettings ConfigurePlayerDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigureGameObjectDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigurePlaceDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static CustomThreadPoolSettings ConfigureSoundBusThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 1
            };
        }

        public static CustomThreadPoolSettings ConfigureMonitorThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 10
            };
        }
    }
}
