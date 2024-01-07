using Assets.SymOntoClaySamles.GizmoDecorators;
using Assets.SymOntoClaySamles.Spawners;
using System;
using System.Collections;
using UnityEngine;

namespace SymOntoClay.UnityAsset.GizmoDecorators
{
    [AddComponentMenu("SymOntoClay Samles/GizmoDecorator")]
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
                    EnableCentralVetricalLine,
                    UseCustomCentralVetricalLineHeight ? CentralVetricalLineHeight : null,
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
                    GizmoDecoratorGroup.EnableCentralVetricalLine,
                    GizmoDecoratorGroup.UseCustomCentralVetricalLineHeight ? GizmoDecoratorGroup.CentralVetricalLineHeight : null,
                    GizmoDecoratorGroup.EnableRadialLines,
                    GizmoDecoratorGroup.UseCustomRadialLineRadius ? GizmoDecoratorGroup.RadialLineRadius : null,
                    GizmoDecoratorGroup.EnableBoundLines);
            }
        }
#endif
    }
}