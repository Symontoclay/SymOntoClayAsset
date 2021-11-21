using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/HandThing")]
    public class HandThing : BaseThing, IUHandThing
    {
        public TakingPolicy TakingPolicy;

        public float TakingDistance = 2;

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
    }
}
