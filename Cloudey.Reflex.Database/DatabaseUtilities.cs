using Microsoft.EntityFrameworkCore;

namespace Cloudey.Reflex.Database;

public static class DatabaseUtilities
{
	public static async Task DropAllTables (this DbContext dbContext)
	{
		await dbContext.Database.ExecuteSqlRawAsync(
			@"
                DO $$ DECLARE
                    r RECORD;
                BEGIN
                    FOR r IN (SELECT tableName FROM pg_tables WHERE schemaName = current_schema()) LOOP
                        EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tableName) || ' CASCADE';
                    END LOOP;
                END $$;"
		);
	}
}