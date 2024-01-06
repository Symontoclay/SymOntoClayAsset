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

        private void TmpDr()
        {
            Gizmos.color = new Color(Color.r, Color.g, Color.b, 1);

            var size = GetComponent<Renderer>().bounds.size;

            //var scaleX = transform.localScale.x / 2;

            var scaleX = size.x / 2;

            if(scaleX < 0.1)
            {
                scaleX = 1;
            }

            var rightPoint = transform.position + transform.right.normalized * scaleX;

            var rX = rightPoint.x;

            var leftPoint = transform.position + transform.right.normalized * scaleX * -1;

            var lX = leftPoint.x;

            //Gizmos.DrawLine(transform.position, rightPoint);

            //var scaleZ = transform.localScale.z / 2;

            var scaleZ = size.z / 2;

            if (scaleZ < 0.1)
            {
                scaleZ = 1;
            }

            var forvardPoint = transform.position + transform.forward.normalized * scaleZ;

            var fZ = forvardPoint.z;

            var backPoint = transform.position + transform.forward.normalized * scaleZ * -1;

            var bZ = backPoint.z;

            //Gizmos.DrawLine(transform.position, forvardPoint);

            //var scaleY = transform.localScale.y / 2;

            var scaleY = size.y / 2;

#if DEBUG
            Debug.Log($"GizmoDecorator OnDrawGizmos size.y = {size.y}");
            Debug.Log($"GizmoDecorator OnDrawGizmos scaleY = {scaleY}");
#endif

            if (scaleY < 0.1)
            {
                scaleY = 1;
            }

            var upPoint = transform.position + transform.up.normalized * scaleY;

            var uY = upPoint.y;

            var downPoint = transform.position + transform.up.normalized * scaleY * -1;

            var dY = downPoint.y;

            //Gizmos.DrawLine(transform.position, upPoint);

            var ufrPoint = new Vector3(rX, uY, fZ);
            var ubrPoint = new Vector3(rX, uY, bZ);
            var uflPoint = new Vector3(lX, uY, fZ);
            var ublPoint = new Vector3(lX, uY, bZ);

            Gizmos.DrawLine(ufrPoint, ubrPoint);
            Gizmos.DrawLine(ufrPoint, uflPoint);
            Gizmos.DrawLine(ublPoint, uflPoint);
            Gizmos.DrawLine(ublPoint, ubrPoint);

            var dfrPoint = new Vector3(rX, dY, fZ);
            var dbrPoint = new Vector3(rX, dY, bZ);
            var dflPoint = new Vector3(lX, dY, fZ);
            var dblPoint = new Vector3(lX, dY, bZ);

            Gizmos.DrawLine(dfrPoint, dbrPoint);
            Gizmos.DrawLine(dfrPoint, dflPoint);
            Gizmos.DrawLine(dblPoint, dflPoint);
            Gizmos.DrawLine(dblPoint, dbrPoint);

            Gizmos.DrawLine(dfrPoint, ufrPoint);
            Gizmos.DrawLine(dbrPoint, ubrPoint);
            Gizmos.DrawLine(dflPoint, uflPoint);
            Gizmos.DrawLine(dblPoint, ublPoint);
        }
#endif
    }
}