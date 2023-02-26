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

using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Components
{
    [AddComponentMenu("SymOntoClay/Hand Thing")]
    public class HandThing : BaseThing, IHandThingBehavior
    {
        public TakingPolicy TakingPolicy;

        public float TakingDistance = 2;

        protected override void Awake()
        {
            base.Awake();

            _uHandThingHost = GetComponent<IHandThingHostBehavior>();
        }

        private IHandThingHostBehavior _uHandThingHost;

        protected override bool CanBeTakenBy(IEntity subject)
        {
            switch(TakingPolicy)
            {
                case TakingPolicy.EveryWhere:
                    return _hostListener.CanBeTakenBy(subject);

                case TakingPolicy.NoWhere:
                    return false;

                case TakingPolicy.ByDistance:
                    {
                        if(!subject.Position.HasValue)
                        {
                            return false;
                        }

                        var tmpPos = subject.Position.Value;

                        var subjectPos = new Vector3(tmpPos.X, tmpPos.Y, tmpPos.Z);

                        var distance = RunInMainThread(() => {
                            return Vector3.Distance(transform.position, subjectPos);
                        });

                        if(distance > TakingDistance)
                        {
                            return false;
                        }

                        return _hostListener.CanBeTakenBy(subject);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(TakingPolicy), TakingPolicy, null);
            }
        }

        void IHandThingBehavior.HideForBackpackInMainThread()
        {
            _uHandThingHost?.HideForBackpackInMainThread();
        }

        void IHandThingBehavior.HideForBackpackInUsualThread()
        {
            _uHandThingHost?.HideForBackpackInUsualThread();
        }
    }
}
