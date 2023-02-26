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

using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.UnityAsset.Samles.Environment;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Helpers
{
    public class InstancesRegistry
    {
        #region static_members
        private static object _sLockObj = new object();
        private static InstancesRegistry _instance;

        public static InstancesRegistry GetRegistry()
        {
            lock (_sLockObj)
            {
                if (_instance == null)
                {
                    _instance = new InstancesRegistry();
                }

                return _instance;
            }
        }
        #endregion

        private object _lockObj = new object();

        public void RegisterInstance(int instanceId, string id, KindOfInstance kindOfInstance)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Debug.Log($"InstancesRegistry RegisterInstance instanceId = {instanceId}; id = '{id}'; kindOfInstance = {kindOfInstance}");
#endif

                _kindOfInstancesDict[instanceId] = kindOfInstance;

                var idForFacts = NameHelper.ShieldString(id);

#if DEBUG
                //Debug.Log($"InstancesRegistry RegisterInstance idForFacts = '{idForFacts}'");
#endif

                _idsDict[instanceId] = idForFacts;
            }
        }

        public KindOfInstance GetKindOfInstance(int instanceId)
        {
            lock (_lockObj)
            {
                if(_kindOfInstancesDict.ContainsKey(instanceId))
                {
                    return _kindOfInstancesDict[instanceId];
                }

                return KindOfInstance.Unknown;
            }
        }

        public string GetId(int instanceId)
        {
            lock (_lockObj)
            {
                if(_idsDict.ContainsKey(instanceId))
                {
                    return _idsDict[instanceId];
                }

                return string.Empty;
            }
        }

        public void RegisterLinkedWaypoint(int instanceId, int waypointInstanceId)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Debug.Log($"InstancesRegistry RegisterInstance instanceId = {instanceId}; waypointInstanceId = {waypointInstanceId}");
#endif

                _linkedWaypointsDict[instanceId] = waypointInstanceId;
            }
        }

        public int? GetLinkedWaypoint(int instanceId)
        {
            lock (_lockObj)
            {
                if(_linkedWaypointsDict.ContainsKey(instanceId))
                {
                    return _linkedWaypointsDict[instanceId];
                }

                return null;
            }
        }

        private Dictionary<int, KindOfInstance> _kindOfInstancesDict = new Dictionary<int, KindOfInstance>();
        private Dictionary<int, string> _idsDict = new Dictionary<int, string>(); 
        private Dictionary<int, int> _linkedWaypointsDict = new Dictionary<int, int>();
    }
}
