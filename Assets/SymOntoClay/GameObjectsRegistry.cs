using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    public static class GameObjectsRegistry
    {
        public static void AddGameObject(GameObject gameObject)
        {
            var instanceId = gameObject.GetInstanceID();

            if(_gameObjectsByInstancesIdDict.ContainsKey(instanceId))
            {
                return;
            }

            _gameObjectsByInstancesIdDict[instanceId] = gameObject;
        }

        public static GameObject GetGameObject(int instanceId)
        {
            if(_gameObjectsByInstancesIdDict.ContainsKey(instanceId))
            {
                return _gameObjectsByInstancesIdDict[instanceId];
            }

            return null;
        }

        public static T GetComponent<T>(int instanceId)
        {
            var gameObject = GetGameObject(instanceId);

            if(gameObject == null)
            {
                return default;
            }

            return gameObject.GetComponent<T>();
        }

        private static readonly Dictionary<int, GameObject> _gameObjectsByInstancesIdDict = new Dictionary<int, GameObject>();
    }
}
