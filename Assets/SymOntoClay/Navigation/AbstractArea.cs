using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Helpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Navigation
{
    public abstract class AbstractArea : MonoBehaviour, IPlatformSupport
    {
        System.Numerics.Vector3 IPlatformSupport.ConvertFromRelativeToAbsolute(SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            return PlatformSupportHelper.ConvertFromRelativeToAbsolute(transform, relativeCoordinate);
        }

        System.Numerics.Vector3 IPlatformSupport.GetCurrentAbsolutePosition()
        {
            return PlatformSupportHelper.GetCurrentAbsolutePosition(transform);
        }

        float IPlatformSupport.GetDirectionToPosition(System.Numerics.Vector3 position)
        {
            return PlatformSupportHelper.GetDirectionToPosition(transform, position);
        }

        bool IPlatformSupport.CanBeTakenBy(IEntity subject)
        {
            return false;
        }
    }
}
