using Assets.SymOntoClay.Convertors;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Scriptables;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/World")]
    public class World : MonoBehaviour
    {
        public WorldFile WorldFile;

        void Awake()
        {
            _invokerInMainThread = new InvokerInMainThread();

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), Application.productName);

            QuickLogger.Log($"World Awake Application.supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            _world = WorldFactory.WorldInstance;

            _world.AddConvertor(new Vector3UnityAndSystemNumericConvertor());
            _world.AddConvertor(new Vector3AndWayPointValueConvertor());

            var worldFullFileName = Path.Combine(Application.dataPath, WorldFile.FullName);

#if DEBUG
            Debug.Log($"World Awake worldFullFileName = {worldFullFileName}");
#endif

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(worldFullFileName);

#if DEBUG
            Debug.Log($"World Awake wspaceDir = {wspaceDir}");
#endif

            var settings = new WorldSettings();

            settings.SharedModulesDirs = new List<string>() { Path.Combine(wspaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), Application.productName);

            settings.HostFile = worldFullFileName;

            settings.InvokerInMainThread = _invokerInMainThread;

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = Application.productName,
                //PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance, CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

#if DEBUG            
            Debug.Log($"World Awake settings = {settings}");
#endif

            QuickLogger.Log($"World Awake settings = {settings}");

            _world.SetSettings(settings);
        }

        void Start()
        {
#if DEBUG
            Debug.Log("World Start");
#endif

            _world.Start();
        }

        void Update()
        {
            _invokerInMainThread.Update();
        }

        void Stop()
        {
            _world.Dispose();
        }

        private IWorld _world;
        private InvokerInMainThread _invokerInMainThread;
    }
}

