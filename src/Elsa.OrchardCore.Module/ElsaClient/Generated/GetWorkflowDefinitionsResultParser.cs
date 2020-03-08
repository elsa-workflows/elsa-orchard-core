using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using StrawberryShake;
using StrawberryShake.Configuration;
using StrawberryShake.Http;
using StrawberryShake.Http.Subscriptions;
using StrawberryShake.Transport;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class GetWorkflowDefinitionsResultParser
        : JsonResultParserBase<IGetWorkflowDefinitions>
    {
        private readonly IValueSerializer _stringSerializer;
        private readonly IValueSerializer _intSerializer;
        private readonly IValueSerializer _booleanSerializer;

        public GetWorkflowDefinitionsResultParser(IValueSerializerCollection serializerResolver)
        {
            if (serializerResolver is null)
            {
                throw new ArgumentNullException(nameof(serializerResolver));
            }
            _stringSerializer = serializerResolver.Get("String");
            _intSerializer = serializerResolver.Get("Int");
            _booleanSerializer = serializerResolver.Get("Boolean");
        }

        protected override IGetWorkflowDefinitions ParserData(JsonElement data)
        {
            return new GetWorkflowDefinitions
            (
                ParseGetWorkflowDefinitionsWorkflowDefinitions(data, "workflowDefinitions")
            );

        }

        private global::System.Collections.Generic.IReadOnlyList<global::Elsa.OrchardCore.IWorkflowDefinitionVersion> ParseGetWorkflowDefinitionsWorkflowDefinitions(
            JsonElement parent,
            string field)
        {
            JsonElement obj = parent.GetProperty(field);

            int objLength = obj.GetArrayLength();
            var list = new global::Elsa.OrchardCore.IWorkflowDefinitionVersion[objLength];
            for (int objIndex = 0; objIndex < objLength; objIndex++)
            {
                JsonElement element = obj[objIndex];
                list[objIndex] = new WorkflowDefinitionVersion
                (
                    DeserializeNullableString(element, "id"),
                    DeserializeNullableString(element, "definitionId"),
                    DeserializeNullableString(element, "name"),
                    DeserializeNullableString(element, "description"),
                    DeserializeInt(element, "version"),
                    DeserializeBoolean(element, "isLatest"),
                    DeserializeBoolean(element, "isPublished"),
                    DeserializeBoolean(element, "isSingleton"),
                    DeserializeBoolean(element, "isDisabled")
                );

            }

            return list;
        }

        private string DeserializeNullableString(JsonElement obj, string fieldName)
        {
            if (!obj.TryGetProperty(fieldName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return (string)_stringSerializer.Deserialize(value.GetString());
        }

        private int DeserializeInt(JsonElement obj, string fieldName)
        {
            JsonElement value = obj.GetProperty(fieldName);
            return (int)_intSerializer.Deserialize(value.GetInt32());
        }

        private bool DeserializeBoolean(JsonElement obj, string fieldName)
        {
            JsonElement value = obj.GetProperty(fieldName);
            return (bool)_booleanSerializer.Deserialize(value.GetBoolean());
        }
    }
}
