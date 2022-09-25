using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Navigation
{
    public abstract class BaseElementaryArea: AbstractArea
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        public string IdForFacts => _idForFacts;

        protected virtual void Awake()
        {
            GameObjectsRegistry.AddGameObject(gameObject);
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = GetIdByName();
            }
            else
            {
                if (Id == GetIdByName(_oldName))
                {
                    Id = GetIdByName();
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
    }
}
