/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using UnityEngine.SceneManagement;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/World")]
    public class World : MonoBehaviour
    {
        public WorldFile WorldFile;

        private bool _isStarded;

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
            //Debug.Log($"World Awake worldFullFileName = {worldFullFileName}");
#endif

            var wspaceDir = WorldSpaceHelper.GetRootWorldSpaceDir(worldFullFileName);

#if DEBUG
            //Debug.Log($"World Awake wspaceDir = {wspaceDir}");
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

        void Stop()
        {
            _world.Dispose();
        }

        private IWorld _world;
        private InvokerInMainThread _invokerInMainThread;
    }
}

