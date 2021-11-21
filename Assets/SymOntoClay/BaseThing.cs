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
        protected IUHostListener _hostListener;

        protected override void Awake()
        {
            base.Awake();
#if DEBUG
            //Debug.Log($"BaseThing Awake name = '{name}' gameObject.GetInstanceID() = {gameObject.GetInstanceID()}");
#endif

            var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

#if DEBUG
            //Debug.Log($"Thing Awake fullFileName = {fullFileName}");
#endif

            var settings = new GameObjectSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();

            settings.HostFile = fullFileName;

            var hostListener = GetComponent<IUHostListener>();

            if (hostListener == null)
            {
                settings.HostListener = this;
            }
            else
            {
                settings.HostListener = hostListener;
                _hostListener = hostListener;
            }

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
