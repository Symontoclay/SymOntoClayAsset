/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Helpers
{
    public static class PlatformSupportHelper
    {
        public static System.Numerics.Vector3 ConvertFromRelativeToAbsolute(IMonitorLogger logger, Transform transform, SymOntoClay.Core.RelativeCoordinate relativeCoordinate)
        {
            var distance = relativeCoordinate.Distance;
            var angle = relativeCoordinate.HorizontalAngle;

#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute angle = {angle}");
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute distance = {distance}");
#endif

            var radAngle = angle * Mathf.Deg2Rad;
            var x = Mathf.Sin(radAngle);
            var y = Mathf.Cos(radAngle);
            var localDirection = new Vector3(x * distance, 0f, y * distance);

#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute localDirection = {localDirection}");
#endif

            var globalDirection = transform.TransformDirection(localDirection);

            var newPosition = globalDirection + transform.position;

            return new System.Numerics.Vector3(newPosition.x, newPosition.y, newPosition.z);
        }

        public static System.Numerics.Vector3 GetCurrentAbsolutePosition(IMonitorLogger logger, Transform transform)
        {
            var currPosition = transform.position;

            return new System.Numerics.Vector3(currPosition.x, currPosition.y, currPosition.z);
        }

        public static float GetDirectionToPosition(IMonitorLogger logger, Transform transform, System.Numerics.Vector3 position)
        {
#if DEBUG
            //Debug.Log($"BaseSymOntoClayGameObject ConvertFromRelativeToAbsolute position = {position}");
#endif

            var aimPosition = new Vector3(position.X, position.Y, position.Z);

            var myPosition = transform.position;

#if UNITY_EDITOR
            //Debug.Log($"aimPosition = {aimPosition}");
            //Debug.Log($"myPosition = {myPosition}");
#endif

            var heading = aimPosition - myPosition;

#if UNITY_EDITOR
            //Debug.Log($"heading = {heading}");
#endif

            var distance = heading.magnitude;

            var direction = heading / distance;

#if UNITY_EDITOR
            //Debug.Log($"distance = {distance}");
            //Debug.Log($"direction = {direction}");
#endif

            var rotation = Quaternion.FromToRotation(Vector3.forward, direction);

#if UNITY_EDITOR
            //Debug.Log($"rotation = {rotation}");
#endif

            var angle = Quaternion.Angle(transform.rotation, rotation);

#if UNITY_EDITOR
            //Debug.Log($"angle = {angle}");
#endif

            return angle;
        }
    }
}
