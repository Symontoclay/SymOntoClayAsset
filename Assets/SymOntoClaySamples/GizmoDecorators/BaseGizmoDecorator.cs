using UnityEngine;

namespace Assets.SymOntoClaySamples.GizmoDecorators
{
    public class BaseGizmoDecorator : MonoBehaviour
    {
        public Color Color;

        public bool EnableCentralVerticalLine;
        public bool UseCustomCentralVerticalLineHeight;
        public int CentralVerticalLineHeight;
        public bool EnableRadialLines;
        public bool UseCustomRadialLineRadius;
        public int RadialLineRadius;
        public bool EnableBoundLines;
    }
}