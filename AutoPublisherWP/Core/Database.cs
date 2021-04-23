using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPublisherWP.Core
{
    class Database
    {
        private readonly string dbpath = Path.Combine(AppContext.BaseDirectory, "wpauto.db");

        public SqliteConnection GetConnection()
        {
            var db = new SqliteConnection($"Filename={dbpath}");
            db.Open();
            return db;
        }

        public async Task<string> CheckDatabase()
        {
            await CheckIfExistsAsync();
            using (var db = GetConnection())
            {
                var version = await db.QueryFirstOrDefaultAsync<int>(
@"select *
from version");
                if (version == 0)
                {
                    await db.ExecuteAsync(
@"CREATE TABLE ""sites"" (
    ""id""    INTEGER,
	""URL""   TEXT NOT NULL,
	""User""  TEXT NOT NULL,
	""Pass""  TEXT NOT NULL,
	PRIMARY KEY(""id"" AUTOINCREMENT)
); 
update version set number = 1;");
                    return "base de datos elevada a version 1";
                }
                else
                {
                    return "base de datos en version 1";
                }
            }
        }

        private async Task CheckIfExistsAsync()
        {
            if (!File.Exists(dbpath))
            {
                string DbBase = Path.Combine(AppContext.BaseDirectory, "base.db");
                using (FileStream Source = File.Open(DbBase,FileMode.Open))
                {
                    using (FileStream Destination = File.Create(dbpath))
                    {
                        await Source.CopyToAsync(Destination);
                    }
                }
            }
        }
    }
}
