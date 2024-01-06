using Assets.SymOntoClaySamles.Spawners;
using System;
using System.Collections;
using UnityEngine;

namespace SymOntoClay.UnityAsset.GizmoDecorators
{
    [AddComponentMenu("SymOntoClay Samles/GizmoDecorator")]
    public class GizmoDecorator : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color Color;

        void OnDrawGizmos()
        {
#if DEBUG
            //Debug.Log($"GizmoDecorator OnDrawGizmos PropsToSpawn?.Count = {PropsToSpawn?.Count}");
            //Debug.Log($"GizmoDecorator OnDrawGizmos Color = {Color}");
#endif

            //Gizmos.color = Color;

            //Gizmos.color = Color.green;
            

#if DEBUG
            //Debug.Log($"GizmoDecorator OnDrawGizmos PropsToSpawn?.Count = {PropsToSpawn?.Count}");
            //Debug.Log($"GizmoDecorator OnDrawGizmos Gizmos.color = {Gizmos.color}");
#endif

            var size = GetComponent<Renderer>().bounds.size;

#if DEBUG
            //Debug.Log($"GizmoDecorator OnDrawGizmos PropsToSpawn?.Count = {PropsToSpawn?.Count}");
            //Debug.Log($"GizmoDecorator OnDrawGizmos size = {size}");
#endif

            //TmpDr();
            DrawGizmosHelper.DrawGizmos(transform, size, DrawGizmosHelper.ToGizmosColor(Color), true, 3, true, null, true);
        }
#endif
    }
}