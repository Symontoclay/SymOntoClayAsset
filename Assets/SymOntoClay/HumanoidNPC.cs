﻿using Assets.SymOntoClay.Data;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Scriptables;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/HumanoidNPC")]
    public class HumanoidNPC : MonoBehaviour, IPlatformSupport, IUHumanoidNPC, IVisionProvider
    {
        public SobjFile SobjFile;
        public string Id;

        private string _oldName;
        private string _idForFacts;

        private bool _isDead;

        public string IdForFacts => _idForFacts;

        public GameObject Head;
        public int RaysDistance = 20;
        public int TotalRaysAngle = 120;
        public int TotalRaysInterval = 30;
        public int FocusRaysAngle = 30;
        public int FocusRaysInterval = 10;

        public bool IsImmortal;
        public int Health = 100;
        public bool IsResurrected;
        public bool IsInitiallyDead;

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

            if(Id.StartsWith("#`"))
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

        void Awake()
        {
#if DEBUG
            //Debug.Log("HumanoidNPC Awake");
#endif

            var npcFullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

#if DEBUG
            //Debug.Log($"HumanoidNPC Awake npcFullFileName = {npcFullFileName}");
#endif

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = Id;
            npcSettings.InstanceId = gameObject.GetInstanceID();

            npcSettings.LogicFile = npcFullFileName;

            npcSettings.HostListener = GetHostListener();
            npcSettings.PlatformSupport = this;
            npcSettings.VisionProvider = this;

#if DEBUG
            //Debug.Log($"HumanoidNPC Awake npcSettings = {npcSettings}");
#endif

            //QuickLogger.Log($"HumanoidNPC Awake npcSettings = {npcSettings}");

            _npc = WorldFactory.WorldInstance.GetHumanoidNPC(npcSettings);
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

        private Transform _targetHeadTransform;
        private List<UVisibleItem> _rawVisibleItemsList = new List<UVisibleItem>();
        private bool _isRawVisibleItemsListChanged;
        private object _rawVisibleItemsListLockObj = new object();
        private List<VisibleItem> _visibleItemsResultList = new List<VisibleItem>();

        void Start()
        {
            CalculateRaysAngles();

            _targetHeadTransform = Head.transform;
        }

        void Update()
        {
            if (_isDead)
            {
                return;
            }

            var newRawVisibleItemsList = new List<UVisibleItem>();

            foreach (var rayDirection in _rayDirectionsList)
            {
                var localDirection = rayDirection.Item1;

                var globalDirection = _targetHeadTransform.TransformDirection(localDirection);

                var hit = new RaycastHit();

                var pos = _targetHeadTransform.position;

                if (Physics.Raycast(pos, globalDirection, out hit, RaysDistance))
                {
#if UNITY_EDITOR
                    Debug.DrawLine(pos, hit.point, Color.blue);
#endif

                    var hitTransform = hit.transform;

                    var visibleItem = new UVisibleItem();
                    visibleItem.InstanceId = hitTransform.gameObject.GetInstanceID();
                    visibleItem.Position = hitTransform.position;
                    visibleItem.Distance = hit.distance;
                    visibleItem.IsInFocus = rayDirection.Item2;

                    newRawVisibleItemsList.Add(visibleItem);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.DrawRay(pos, globalDirection * RaysDistance, Color.red);
                }
#endif
            }

            lock (_rawVisibleItemsListLockObj)
            {
                _rawVisibleItemsList = newRawVisibleItemsList;
                _isRawVisibleItemsListChanged = true;
            }
        }

        public IList<VisibleItem> GetCurrentVisibleItems()
        {
            List<UVisibleItem> rawVisibleItemsList;

            lock (_rawVisibleItemsListLockObj)
            {
                if (!_isRawVisibleItemsListChanged)
                {
                    return _visibleItemsResultList;
                }

                _isRawVisibleItemsListChanged = false;

                rawVisibleItemsList = _rawVisibleItemsList;
            }

            var result = new List<VisibleItem>();

            var rawVisibleItemsDict = rawVisibleItemsList.GroupBy(p => p.InstanceId);

            foreach (var rawVisibleItemsKVPItem in rawVisibleItemsDict)
            {
                var firstElem = rawVisibleItemsKVPItem.First();

                var initPosition = firstElem.Position;

                var item = new VisibleItem();
                item.InstanceId = firstElem.InstanceId;
                item.Position = new System.Numerics.Vector3(initPosition.x, initPosition.y, initPosition.z);
                item.IsInFocus = rawVisibleItemsKVPItem.Any(p => p.IsInFocus);
                item.MinDistance = rawVisibleItemsKVPItem.Min(p => p.Distance);

                result.Add(item);
            }

            _visibleItemsResultList = result;

            return result;
        }

        public void Die()
        {
            _isDead = true;
        }

        void Stop()
        {
            _npc.Dispose();
        }

        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            var distance = relativeCoordinate.Distance;
            var angle = relativeCoordinate.HorizontalAngle;

#if DEBUG
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute angle = {angle}");
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute distance = {distance}");
#endif

            var radAngle = angle * Mathf.Deg2Rad;
            var x = Mathf.Sin(radAngle);
            var y = Mathf.Cos(radAngle);
            var localDirection = new Vector3(x * distance, 0f, y * distance);

#if DEBUG
            //Debug.Log($"HumanoidNPC ConvertFromRelativeToAbsolute localDirection = {localDirection}");
#endif

            var globalDirection = transform.TransformDirection(localDirection);

            var newPosition = globalDirection + transform.position;

            return new System.Numerics.Vector3(newPosition.x, newPosition.y, newPosition.z);
        }

        private IHumanoidNPC _npc;

        public IHumanoidNPC NPC => _npc;

        private List<(Vector3, bool)> _rayDirectionsList;

        private void CalculateRaysAngles()
        {
            _rayDirectionsList = new List<(Vector3, bool)>();

            var anglesList = new List<(float, bool)>();

            var halfOfTotalRaysAngle = TotalRaysAngle / 2;
            var halfOfFocusRaysAngle = FocusRaysAngle / 2;

            var currAngle = 0f;
            var inFocus = true;

            do
            {
                var radAngle = currAngle * Mathf.Deg2Rad;

                anglesList.Add((radAngle, inFocus));

                if (currAngle != 0)
                {
                    anglesList.Add((-1 * radAngle, inFocus));
                }

                if (currAngle >= halfOfTotalRaysAngle)
                {
                    break;
                }

                if (currAngle < halfOfFocusRaysAngle)
                {
                    currAngle += FocusRaysInterval;

                    if (currAngle > halfOfFocusRaysAngle)
                    {
                        currAngle = halfOfFocusRaysAngle;
                    }
                }
                else
                {
                    currAngle += TotalRaysInterval;
                    inFocus = false;
                }

                if (currAngle > halfOfTotalRaysAngle)
                {
                    currAngle = halfOfTotalRaysAngle;
                }
            } while (true);

            foreach(var vAngle in anglesList)
            {
                var inVFocus = vAngle.Item2;

                var z = Mathf.Sin(vAngle.Item1);

                foreach(var hAngle in anglesList)
                {
                    inFocus = hAngle.Item2 && inVFocus;

                    var hAngleVal = hAngle.Item1;

                    var x = Mathf.Sin(hAngleVal);
                    var y = Mathf.Cos(hAngleVal);

                    _rayDirectionsList.Add((new Vector3(x, z, y), inFocus));
                }
            }
        }        
    }
}
