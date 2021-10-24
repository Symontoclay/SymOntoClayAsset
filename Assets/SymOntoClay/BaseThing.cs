using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    public class BaseThing : BaseSymOntoClayGameObject
    {
        private object GetHostListener()
        {
            var hostListener = GetComponent<IUHostListener>();

            if (hostListener == null)
            {
                return this;
            }

            return hostListener;
        }

        protected virtual void Awake()
        {
#if DEBUG
            Debug.Log($"BaseThing Awake name = '{name}' gameObject.GetInstanceID() = {gameObject.GetInstanceID()}");
#endif

            var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

#if DEBUG
            //Debug.Log($"Thing Awake fullFileName = {fullFileName}");
#endif

            var settings = new GameObjectSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();

            settings.HostFile = fullFileName;

            settings.HostListener = GetHostListener();

            settings.PlatformSupport = this;

#if DEBUG
            //Debug.Log($"Thing Awake settings = {settings}");
#endif

            _thing = WorldFactory.WorldInstance.GetGameObject(settings);

            SetSelfWorldComponent(_thing);
        }

        private IGameObject _thing;

        protected virtual void Stop()
        {
            _thing.Dispose();
        }
    }
}
