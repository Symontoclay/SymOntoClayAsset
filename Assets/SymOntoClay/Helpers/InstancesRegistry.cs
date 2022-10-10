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
