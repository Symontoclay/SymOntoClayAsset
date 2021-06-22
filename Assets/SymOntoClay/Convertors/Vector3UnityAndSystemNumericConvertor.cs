using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Convertors
{
    public class Vector3UnityAndSystemNumericConvertor: IPlatformTypesConvertor
    {
        /// <inheritdoc/>
        public Type PlatformType => typeof(UnityEngine.Vector3);

        /// <inheritdoc/>
        public Type CoreType => typeof(System.Numerics.Vector3);

        /// <inheritdoc/>
        public bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public object ConvertToCoreType(object platformObject, IEntityLogger logger)
        {
            var targetObject = (UnityEngine.Vector3)platformObject;

            return new System.Numerics.Vector3(targetObject.x, targetObject.y, targetObject.z);
        }

        /// <inheritdoc/>
        public object ConvertToPlatformType(object coreObject, IEntityLogger logger)
        {
            var targetObject = (System.Numerics.Vector3)coreObject;

            return new UnityEngine.Vector3(targetObject.X, targetObject.Y, targetObject.Z);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(PlatformType)} = {PlatformType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CoreType)} = {CoreType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToPlatformType)} = {CanConvertToPlatformType}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToCoreType)} = {CanConvertToCoreType}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(PlatformType)} = {PlatformType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CoreType)} = {CoreType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToPlatformType)} = {CanConvertToPlatformType}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToCoreType)} = {CanConvertToCoreType}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(PlatformType)} = {PlatformType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CoreType)} = {CoreType.FullName}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToPlatformType)} = {CanConvertToPlatformType}");
            sb.AppendLine($"{spaces}{nameof(CanConvertToCoreType)} = {CanConvertToCoreType}");

            return sb.ToString();
        }
    }
}
