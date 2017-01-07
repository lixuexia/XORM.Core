using System.Collections.Generic;
using System.Linq;

namespace XORM.CBase.Data.Common
{
    public class DataRow
    {
        public object[] ItemArray
        {
            get { return obj.Values.ToArray(); }
        }

        private Dictionary<string, object> obj = new Dictionary<string, object>();

        public List<string> ColumnNames
        {
            get
            {
                List<string> Cols = new List<string>();
                foreach (string ColName in obj.Keys)
                {
                    Cols.Add(ColName.ToUpper());
                }
                return Cols;
            }
        }

        public bool ContainsColumn(string ColName)
        {
            foreach(string Col in this.obj.Keys)
            {
                if(Col.ToUpper()==ColName.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(string key, object value)
        {
            obj.Add(key.ToUpper(), value);
        }

        public bool IsNull(int columnIndex)
        {
            if (this[columnIndex] == null || System.DBNull.Value == this[columnIndex])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object this[string columnName]
        {
            get
            {
                return obj[columnName.ToUpper()];
            }
            set
            {
                obj[columnName.ToUpper()] = value;
            }
        }
        public object this[int ColumnIndex]
        {
            get
            {
                int i = 0;
                object reval = null;
                foreach (var item in obj)
                {
                    if (i == ColumnIndex)
                    {
                        reval = item.Value;
                        break;
                    }
                    i++;
                }
                return reval;
            }
        }

        public bool ContainsKey(string columnName)
        {
            if (this.obj == null) return false;
            return (this.obj.ContainsKey(columnName.ToUpper()));
        }

        public void AddColumn(string ColName)
        {
            if(!obj.ContainsKey(ColName.ToUpper()))
            {
                obj.Add(ColName.ToUpper(), "");
            }
        }
    }
}