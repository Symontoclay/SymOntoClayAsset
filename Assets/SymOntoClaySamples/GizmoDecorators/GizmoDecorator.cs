using Assets.SymOntoClaySamples.GizmoDecorators;
using Assets.SymOntoClaySamples.Spawners;
using System;
using System.Collections;
using UnityEngine;

namespace SymOntoClay.UnityAsset.GizmoDecorators
{
    [AddComponentMenu("SymOntoClay Samples/GizmoDecorator")]
    public class GizmoDecorator : BaseGizmoDecorator
    {
        public GizmoDecoratorGroup GizmoDecoratorGroup;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var size = GetComponent<Renderer>().bounds.size;

            if(GizmoDecoratorGroup == null)
            {
                DrawGizmosHelper.DrawGizmos(
                    transform,
                    size,
                    DrawGizmosHelper.ToGizmosColor(Color),
                    EnableCentralVerticalLine,
                    UseCustomCentralVerticalLineHeight ? CentralVerticalLineHeight : null,
                    EnableRadialLines,
                    UseCustomRadialLineRadius ? RadialLineRadius: null,
                    EnableBoundLines);
            }
            else
            {
                DrawGizmosHelper.DrawGizmos(
                    transform,
                    size,
                    DrawGizmosHelper.ToGizmosColor(GizmoDecoratorGroup.Color),
                    GizmoDecoratorGroup.EnableCentralVerticalLine,
                    GizmoDecoratorGroup.UseCustomCentralVerticalLineHeight ? GizmoDecoratorGroup.CentralVerticalLineHeight : null,
                    GizmoDecoratorGroup.EnableRadialLines,
                    GizmoDecoratorGroup.UseCustomRadialLineRadius ? GizmoDecoratorGroup.RadialLineRadius : null,
                    GizmoDecoratorGroup.EnableBoundLines);
            }
        }
#endif
    }
}