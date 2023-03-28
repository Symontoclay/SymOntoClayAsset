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

using SymOntoClay.UnityAsset.Data;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Scriptables;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using SymOntoClay.UnityAsset.Interfaces;
using UnityEditor.SceneManagement;

namespace SymOntoClay.UnityAsset.Components
{
    [AddComponentMenu("SymOntoClay/Humanoid NPC")]
    public class HumanoidNPC : BaseSymOntoClayGameObject, IPlatformSupport, IHumanoidNPCBehavior, IVisionProvider
    {
        private bool _isDead;

        public GameObject Head;
        public int RaysDistance = 20;
        public int TotalRaysAngle = 120;
        public int TotalRaysInterval = 30;
        public int FocusRaysAngle = 30;
        public int FocusRaysInterval = 10;

#if UNITY_EDITOR
        public bool ShowRayCastGizmo;
#endif

        public List<GameObject> Backpack;

        private bool _needInitilizeBackpack;

        //public bool IsImmortal;
        //public int Health = 100;
        //public bool IsResurrected;
        //public bool IsInitiallyDead;

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            //Debug.Log($"HumanoidNPC Awake ({name})");
#endif

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = Id;
            npcSettings.InstanceId = gameObject.GetInstanceID();

            if(SobjFile != null)
            {
                var npcFullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

#if UNITY_EDITOR
                //Debug.Log($"HumanoidNPC Awake ({name}) npcFullFileName = {npcFullFileName}");
#endif

                npcSettings.LogicFile = npcFullFileName;
            }

            npcSettings.HostListener = GetHostListener();
            npcSettings.PlatformSupport = this;
            npcSettings.VisionProvider = this;

            npcSettings.Categories = Categories;
            npcSettings.EnableCategories = EnableCategories;

#if UNITY_EDITOR
            //Debug.Log($"HumanoidNPC Awake  ({name}) npcSettings = {npcSettings}");
#endif

            //QuickLogger.Log($"HumanoidNPC Awake  ({name}) npcSettings = {npcSettings}");

            _npc = WorldFactory.WorldInstance.GetHumanoidNPC(npcSettings);

            SetSelfWorldComponent(_npc);
        }

        private object GetHostListener()
        {
            var hostListener = GetComponent<IHostListenerBehavior>();

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

            if(Head == null)
            {
                var headLocator = GetComponentInChildren<HeadLocator>();
                var head = headLocator.gameObject;
                _targetHeadTransform = head.transform;
            }
            else
            {
                _targetHeadTransform = Head.transform;
            }

            if (Backpack != null && Backpack.Any())
            {
                _needInitilizeBackpack = true;
            }
        }

        protected override void Update()
        {
            if (_isDead)
            {
                return;
            }

            base.Update();

            if(_needInitilizeBackpack)
            {
                _needInitilizeBackpack = false;

                InitilizeBackpack();
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
                    if(ShowRayCastGizmo)
                    {
                        Debug.DrawLine(pos, hit.point, Color.blue);
                    }                    
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
                    if (ShowRayCastGizmo)
                    {
                        Debug.DrawRay(pos, globalDirection * RaysDistance, Color.red);
                    }                    
                }
#endif
            }

            lock (_rawVisibleItemsListLockObj)
            {
                _rawVisibleItemsList = newRawVisibleItemsList;
                _isRawVisibleItemsListChanged = true;
            }
        }

        private void InitilizeBackpack()
        {
            foreach (var gObj in Backpack)
            {
                var targetHandThingComponent = gObj.GetComponent<IHandThingBehavior>();

                _npc.AddToBackpack(targetHandThingComponent.SocGameObject);

                targetHandThingComponent.HideForBackpackInMainThread();
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

            _npc.Die();
        }

        void Stop()
        {
            _npc.Dispose();
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
