using MySql.Data.MySqlClient;

namespace XORM.CBase.Data.Common
{
    public class MySqlDataAdapter
    {
        private MySqlCommand command;
        private string sql;
        private MySqlConnection _sqlConnection;

        public MySqlDataAdapter(MySqlCommand command)
        {
            this.command = command;
        }

        public MySqlDataAdapter(string sql, MySqlConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        public MySqlCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new MySqlCommand(this.sql, this._sqlConnection);
                }
                return this.command;
            }
        }

        public void Fill(DataTable dt)
        {
            if (dt == null)
            {
                dt = new DataTable();
            }
            var columns = dt.Columns;
            var rows = dt.Rows;
            using (MySqlDataReader dr = command.ExecuteReader())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i).Trim();
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }
                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
                    }
                    dt.Rows.Add(daRow);
                }
            }
        }

        public void Fill(DataSet ds)
        {
            if (ds == null)
            {
                ds = new DataSet();
            }
            using (MySqlDataReader dr = command.ExecuteReader())
            {
                do
                {
                    DataTable dt = ds.Tables.Add();

                    var columns = dt.Columns;
                    var rows = dt.Rows;
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string name = dr.GetName(i).Trim();
                        if (!columns.ContainsKey(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    }
                    while (dr.Read())
                    {
                        DataRow daRow = new DataRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (!daRow.ContainsKey(columns[i].ColumnName))
                                daRow.Add(columns[i].ColumnName, dr.GetValue(i));
                        }
                        dt.Rows.Add(daRow);
                    }
                } while (dr.NextResult());
            }
        }
    }
}