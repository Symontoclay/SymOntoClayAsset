using SymOntoClay.Scriptables;
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
    [AddComponentMenu("SymOntoClay/Player")]
    public class Player : BaseSymOntoClayGameObject
    {
        private IPlayer _player;

        protected override void Awake()
        {
            var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

            var settings = new PlayerSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();

            settings.HostFile = fullFileName;

            settings.PlatformSupport = this;

            _player = WorldFactory.WorldInstance.GetPlayer(settings);

            SetSelfWorldComponent(_player);
#if DEBUG
            //Debug.Log($"Player Awake settings = {settings}");
#endif
        }

        void Stop()
        {
            _player.Dispose();
        }
    }
}
