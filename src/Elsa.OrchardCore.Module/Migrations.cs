using System;
using Elsa.OrchardCore.Indexes;
using OrchardCore.Data.Migration;

namespace Elsa.OrchardCore
{
    public class Migrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateMapIndexTable(nameof(WorkflowServerIndex), table => table
                .Column<string>("WorkflowServerId")
            );

            return 1;
        }
    }
}
