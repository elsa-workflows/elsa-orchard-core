using Elsa.OrchardCore.Indexes;
using OrchardCore.Data.Migration;
using YesSql.Sql;

namespace Elsa.OrchardCore
{
    public class Migrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateMapIndexTable<WorkflowServerRecordIndex>( table => table
                .Column<string>("WorkflowServerId")
            );

            return 1;
        }
    }
}
