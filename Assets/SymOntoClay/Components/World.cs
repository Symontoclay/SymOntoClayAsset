/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Scriptables;
using SymOntoClay.SoundBuses;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using SymOntoClay.UnityAsset.Converters;
using SymOntoClay.StandardFacts;
using SymOntoClay.Core;
using SymOntoClay.NLP;
using UnityEngine.Windows;
using SymOntoClay.ProjectFiles;

namespace SymOntoClay.UnityAsset.Components
{
    [AddComponentMenu("SymOntoClay/World")]
    public class World : MonoBehaviour
    {
        public WorldFile WorldFile;
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain;
        public bool EnableAddingRemovingFactLoggingInStorages;
        public bool EnableNLP;

        private bool _isStarded;

        void Awake()
        {
            _invokerInMainThread = new InvokerInMainThread();

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), Application.productName);

#if DEBUG
            //QuickLogger.Log($"World Awake Application.supportBasePath = {supportBasePath}");
#endif
            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            _world = WorldFactory.WorldInstance;

            _world.AddConvertor(new Vector3UnityAndSystemNumericConverter());
            _world.AddConvertor(new SymOntoClay.UnityAsset.Converters.Vector3AndWayPointValueConverter());
            _world.AddConvertor(new SymOntoClay.UnityAsset.Converters.Vector3AndStrongIdentifierValueConverter());
            _world.AddConvertor(new FloatAndNumberValueConverter());
            _world.AddConvertor(new NavTargetAndStrongIdentifierValueConverter());
            _world.AddConvertor(new EntityAndStrongIdentifierValueConverter());

            var worldFullFileName = Path.Combine(Application.dataPath, WorldFile.FullName);

#if DEBUG
            //Debug.Log($"World Awake worldFullFileName = {worldFullFileName}");
#endif

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(worldFullFileName);

#if DEBUG
            //Debug.Log($"World Awake wspaceDir = {wspaceDir}");
#endif

            var settings = new WorldSettings();

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = wspaceDir,
                AppName = "tst",
                SearchMainNpcFile = false
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            var libsDirs = new List<string>();

            if (!string.IsNullOrWhiteSpace(targetFiles.SharedLibsDir))
            {
                libsDirs.Add(targetFiles.SharedLibsDir);
            }

            if (!string.IsNullOrWhiteSpace(targetFiles.LibsDir))
            {
                libsDirs.Add(targetFiles.LibsDir);
            }

            settings.LibsDirs = libsDirs;

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), Application.productName);

            settings.HostFile = worldFullFileName;

            settings.InvokerInMainThread = _invokerInMainThread;

            settings.SoundBus = new SimpleSoundBus();
            settings.StandardFactsBuilder = new StandardFactsBuilder();

#if DEBUG
            //Debug.Log($"World Awake EnableNLP = {EnableNLP}");
#endif

            if (EnableNLP)
            {
                var nlpConverterProviderSettings = new NLPConverterProviderSettings();

                var mainDictPath = Path.Combine(Application.dataPath, "SymOntoClay", "Dicts", "BigMainDictionary.dict");

#if DEBUG
                //Debug.Log($"World Awake mainDictPath = {mainDictPath}");
#endif

                nlpConverterProviderSettings.DictsPaths = new List<string>() { mainDictPath };

                nlpConverterProviderSettings.CreationStrategy = CreationStrategy.Singleton;

#if DEBUG
                //Debug.Log($"World Awake nlpConverterProviderSettings = {nlpConverterProviderSettings}");
#endif

                var nlpConverterProvider = new NLPConverterProvider(nlpConverterProviderSettings);

                settings.NLPConverterProvider = nlpConverterProvider;
            }

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = Application.productName,
                //PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance, CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true,
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain,
                EnableAddingRemovingFactLoggingInStorages = EnableAddingRemovingFactLoggingInStorages
            };

#if DEBUG            
            //Debug.Log($"World Awake settings = {settings}");
#endif

            //QuickLogger.Log($"World Awake settings = {settings}");

            _world.SetSettings(settings);

            SceneManager.sceneLoaded += OnLevelFinishedLoading;

            DontDestroyOnLoad(gameObject);
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if(_isStarded)
            {
                return;
            }

            _isStarded = true;

#if DEBUG
            //Debug.Log("World OnLevelFinishedLoading");
#endif

            _world.Start();
        }

        void Update()
        {
            _invokerInMainThread.Update();
        }

        void OnDestroy()
        {
#if DEBUG
            //Debug.Log("World OnDestroy");
#endif

            _world.Dispose();
        }

        private IWorld _world;
        private InvokerInMainThread _invokerInMainThread;
    }
}

