using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.GizmoDecorators
{
    public static class DrawGizmosHelper
    {
        public static Color ToGizmosColor(Color source)
        {
            return new Color(source.r, source.g, source.b, 1);
        }

        public static void DrawGizmos
        (
            Transform transform,
            Vector3 size,
            Color color,
            bool enableCentralVetricalLine,
            int? centralVetricalLineHeight,
            bool enableRadialLines,
            int? customRadialLineRadius,
            bool enableBoundLines
        )
        {
            Gizmos.color = color;

            var scaleX = size.x / 2;

            if (scaleX < 0.1)
            {
                scaleX = 1;
            }

            var scaleZ = size.z / 2;

            if (scaleZ < 0.1)
            {
                scaleZ = 1;
            }

            var scaleY = size.y / 2;

            if (scaleY < 0.1)
            {
                scaleY = 1;
            }

            if (enableCentralVetricalLine)
            {
                if (centralVetricalLineHeight.HasValue)
                {
                    Gizmos.DrawLine(transform.position, transform.position + transform.up * centralVetricalLineHeight.Value);
                }
                else
                {
                    Gizmos.DrawLine(transform.position, transform.position + transform.up * Convert.ToInt32(scaleY));
                }
            }

            if (enableRadialLines)
            {
                var radius = 0;

                if (customRadialLineRadius.HasValue)
                {
                    radius = customRadialLineRadius.Value;
                }
                else
                {
                    radius = Convert.ToInt32(Mathf.Min(scaleX, scaleZ));
                }

                Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius);

                Gizmos.DrawRay(transform.position, GetFarPoint(45, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(90, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(135, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(180, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(225, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(270, 0, radius));

                Gizmos.DrawRay(transform.position, GetFarPoint(315, 0, radius));
            }

            if (enableBoundLines)
            {
                var rightPoint = transform.position + transform.right.normalized * scaleX;

                var rX = rightPoint.x;

                var leftPoint = transform.position + transform.right.normalized * scaleX * -1;

                var lX = leftPoint.x;

                //Gizmos.DrawLine(transform.position, rightPoint);

                var forvardPoint = transform.position + transform.forward.normalized * scaleZ;

                var fZ = forvardPoint.z;

                var backPoint = transform.position + transform.forward.normalized * scaleZ * -1;

                var bZ = backPoint.z;

                //Gizmos.DrawLine(transform.position, forvardPoint);

                //var scaleY = transform.localScale.y / 2;

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
        }

        private static Vector3 GetFarPoint(float x, float y, float distance)
        {
            var dy = Mathf.Sin(y * Mathf.Deg2Rad);

            var dx = Mathf.Sin(x * Mathf.Deg2Rad);
            var dz = Mathf.Cos(x * Mathf.Deg2Rad);

            var localDirection = new Vector3(dx, dy, dz) * distance;
            return localDirection;
        }
    }
}
