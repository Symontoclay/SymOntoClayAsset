using UnityEngine;
using System.Collections;
using SymOntoClay.Scriptables;
using System.IO;
using SymOntoClay.UnityAsset.Core;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/Thing")]
    public class Thing : MonoBehaviour
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = GetIdByName();
            }
            else
            {
                var id = GetIdByName();

                if (Id == GetIdByName(_oldName))
                {
                    Id = id;
                }
            }

            if (Id.StartsWith("#`"))
            {
                _idForFacts = Id;
            }
            else
            {
                _idForFacts = $"{Id.Insert(1, "`")}`";
            }

            _oldName = name;
        }

        private string GetIdByName()
        {
            return GetIdByName(name);
        }

        private string GetIdByName(string nameStr)
        {
            return $"#{nameStr}";
        }

        private object GetHostListener()
        {
            var hostListener = GetComponent<IUHostListener>();

            if (hostListener == null)
            {
                return this;
            }

            return hostListener;
        }

        void Awake()
        {
#if DEBUG
            //Debug.Log("Thing Awake");
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

#if DEBUG
            //Debug.Log($"Thing Awake settings = {settings}");
#endif

            _thing = WorldFactory.WorldInstance.GetGameObject(settings);
        }

        private IGameObject _thing;

        void Stop()
        {
            _thing.Dispose();
        }
    }
}
