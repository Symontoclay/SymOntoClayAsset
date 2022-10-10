/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.UnityAsset.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Navigation
{
    [AddComponentMenu("SymOntoClay/Navigation/Waypoint")]
    public class Waypoint : BaseElementaryArea
    {
        private InstancesRegistry _instancesRegistry;

        protected override void Awake()
        {
            base.Awake();

#if DEBUG
            //Debug.Log($"Waypoint Awake name = '{name}' gameObject.GetInstanceID() = {gameObject.GetInstanceID()}");
#endif

            var istanceId = gameObject.GetInstanceID();
            _instancesRegistry = InstancesRegistry.GetRegistry();
            _instancesRegistry.RegisterInstance(istanceId, Id, KindOfInstance.Waypoint);
        }
    }
}
