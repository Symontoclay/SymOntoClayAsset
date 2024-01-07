using System.Collections;
using UnityEngine;

namespace Assets.SymOntoClaySamles.GizmoDecorators
{
    public class BaseGizmoDecorator : MonoBehaviour
    {
        public Color Color;

        public bool EnableCentralVetricalLine;
        public bool UseCustomCentralVetricalLineHeight;
        public int CentralVetricalLineHeight;
        public bool EnableRadialLines;
        public bool UseCustomRadialLineRadius;
        public int RadialLineRadius;
        public bool EnableBoundLines;
    }
}