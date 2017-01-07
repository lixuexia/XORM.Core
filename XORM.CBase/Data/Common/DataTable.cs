namespace XORM.CBase.Data.Common
{
    public class DataTable
    {
        public DataColumnCollection Columns { get; set; }

        public DataRowCollection Rows { get; set; }

        public string TableName { get; set; }

        public DataRow NewRow()
        {
            DataRow dr = new DataRow();
            foreach(DataColumn dc in Columns)
            {
                dr.AddColumn(dc.ColumnName);
            }
            return dr;
        }

        public DataTable()
        {
            Columns = new DataColumnCollection();
            Rows = new DataRowCollection();
            TableName = System.Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}