﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SymOntoClay.Convertors
{
    public class Vector3AndWayPointValueConvertor : IPlatformTypesConvertor
    {
        /// <inheritdoc/>
        public Type PlatformType => typeof(Vector3);

        /// <inheritdoc/>
        public Type CoreType => typeof(WaypointValue);

        /// <inheritdoc/>
        public bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public object ConvertToCoreType(object platformObject, IEntityLogger logger)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object ConvertToPlatformType(object coreObject, IEntityLogger logger)
        {
            var targetObject = (WaypointValue)coreObject;

            var absoluteCoordinates = targetObject.AbcoluteCoordinates;

            return new Vector3(absoluteCoordinates.X, absoluteCoordinates.Y, absoluteCoordinates.Z);
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