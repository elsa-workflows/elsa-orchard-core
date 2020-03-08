using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class VersionOptionsInputSerializer
        : IInputSerializer
    {
        private bool _needsInitialization = true;
        private IValueSerializer _booleanSerializer;
        private IValueSerializer _intSerializer;

        public string Name { get; } = "VersionOptionsInput";

        public ValueKind Kind { get; } = ValueKind.InputObject;

        public Type ClrType => typeof(VersionOptionsInput);

        public Type SerializationType => typeof(IReadOnlyDictionary<string, object>);

        public void Initialize(IValueSerializerCollection serializerResolver)
        {
            if (serializerResolver is null)
            {
                throw new ArgumentNullException(nameof(serializerResolver));
            }
            _booleanSerializer = serializerResolver.Get("Boolean");
            _intSerializer = serializerResolver.Get("Int");
            _needsInitialization = false;
        }

        public object Serialize(object value)
        {
            if (_needsInitialization)
            {
                throw new InvalidOperationException(
                    $"The serializer for type `{Name}` has not been initialized.");
            }

            if (value is null)
            {
                return null;
            }

            var input = (VersionOptionsInput)value;
            var map = new Dictionary<string, object>();

            if (input.AllVersions.HasValue)
            {
                map.Add("allVersions", SerializeNullableBoolean(input.AllVersions.Value));
            }

            if (input.Draft.HasValue)
            {
                map.Add("draft", SerializeNullableBoolean(input.Draft.Value));
            }

            if (input.Latest.HasValue)
            {
                map.Add("latest", SerializeNullableBoolean(input.Latest.Value));
            }

            if (input.LatestOrPublished.HasValue)
            {
                map.Add("latestOrPublished", SerializeNullableBoolean(input.LatestOrPublished.Value));
            }

            if (input.Published.HasValue)
            {
                map.Add("published", SerializeNullableBoolean(input.Published.Value));
            }

            if (input.Version.HasValue)
            {
                map.Add("version", SerializeNullableInt(input.Version.Value));
            }

            return map;
        }

        private object SerializeNullableBoolean(object value)
        {
            if (value is null)
            {
                return null;
            }


            return _booleanSerializer.Serialize(value);
        }
        private object SerializeNullableInt(object value)
        {
            if (value is null)
            {
                return null;
            }


            return _intSerializer.Serialize(value);
        }

        public object Deserialize(object value)
        {
            throw new NotSupportedException(
                "Deserializing input values is not supported.");
        }
    }
}
