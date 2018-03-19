using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ExN2.Datablock {
    class SqlTest {

        NpgsqlConnection conn;

        public void Test() {
            conn = new NpgsqlConnection(String.Format("Server={0};Port=5432;User Id={1};Password={2};Database={3}", "127.0.0.1", "postgres", "Nordit0276", "Test"));
            conn.Open();

            conn.Close();
        }
    }
}
