using System;

namespace XORM.CBase.Data.Common
{
    public class DataSet
    {
        public DataSet()
        {
            this._Tables = new DataTableCollection();
            this._DataSetName = "SET" + Guid.NewGuid().ToString().Replace("-", "");
        }
        public DataSet(string dataSetName)
        {
            this._Tables = new DataTableCollection();
            this._DataSetName = dataSetName;
        }
        private string _DataSetName = string.Empty;
        public string DataSetName { get; set; }
        private DataTableCollection _Tables = null;
        public DataTableCollection Tables { get { return _Tables; } }
        public void Clear()
        {
            this.Tables.Clear();
        }
    }
}