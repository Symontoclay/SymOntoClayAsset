using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Converters
{
    public class Vector3AndStrongIdentifierValueConverter : BasePlatformTypesConverter
    {
        /// <inheritdoc/>
        public override Type PlatformType => typeof(Vector3);

        /// <inheritdoc/>
        public override Type CoreType => typeof(StrongIdentifierValue);

        /// <inheritdoc/>
        public override bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public override bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public override object ConvertToCoreType(object platformObject, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object ConvertToPlatformType(object coreObject, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var identifier = (StrongIdentifierValue)coreObject;

#if DEBUG
            //var logger = context.Logger;
            //logger.Log($"identifier = {identifier}");
#endif

            var kindOfName = identifier.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Entity:
                    return ConvertEntityToPlatformType(identifier, context, localContext);

                case KindOfName.Concept:
                    return ConvertConceptToPlatformType(identifier, context, localContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private object ConvertEntityToPlatformType(StrongIdentifierValue identifier, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var entityValue = PlatformTypesConverterHelper.GetResolvedEntityValue(identifier, context, localContext);

            return entityValue.Position;
        }

        private object ConvertConceptToPlatformType(StrongIdentifierValue concept, IEngineContext context, LocalCodeExecutionContext localContext)
        {
#if DEBUG
            //var logger = context.Logger;
            //logger.Log($"concept = {concept}");
#endif

            var conditionalEntityValue = PlatformTypesConverterHelper.GetResolvedConditionalEntityValue(concept, context, localContext);

            var position = conditionalEntityValue.Position.Value;

            return new Vector3(position.X, position.Y, position.Z);
        }
    }
}
