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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Components
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
