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

        /// <inheritdoc/>
        public override List<string> DefaultCategories => new List<string>() { "waypoint" };

        /// <inheritdoc/>
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

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            var scaleX = transform.localScale.x / 2;

            var rightPoint = transform.position + transform.right.normalized * scaleX;

            var rX = rightPoint.x;

            var leftPoint = transform.position + transform.right.normalized * scaleX * -1;

            var lX = leftPoint.x;

            //Gizmos.DrawLine(transform.position, rightPoint);

            var scaleZ = transform.localScale.z / 2;

            var forvardPoint = transform.position + transform.forward.normalized * scaleZ;

            var fZ = forvardPoint.z;

            var backPoint = transform.position + transform.forward.normalized * scaleZ * -1;

            var bZ = backPoint.z;

            //Gizmos.DrawLine(transform.position, forvardPoint);

            var scaleY = transform.localScale.y / 2;

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
}
