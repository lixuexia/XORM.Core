using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XORM.CBase.Data.Common
{
    public class DataColumnCollection : IEnumerable
    {
        public DataColumnCollection()
        {
            this._list = new List<DataColumn>();
        }
        public DataColumn this[int index] { get { return this._list[index]; } }
        public DataColumn this[string name]
        {
            get
            {
                foreach (var col in this._list)
                {
                    if (name == col.ColumnName)
                    {
                        return col;
                    }
                }
                return null;
            }
        }

        public int Count
        {
            get
            {
                return this._list.Count();
            }
        }

        private List<DataColumn> _list = null;
        protected List<DataColumn> List { get { return _list; } }

        public DataColumn Add()
        {
            DataColumn col = new DataColumn();
            this._list.Add(col);
            return col;
        }
        public void Add(DataColumn column)
        {
            this._list.Add(column);
        }
        public DataColumn Add(string columnName)
        {
            DataColumn col = new DataColumn(columnName);
            this._list.Add(col);
            return col;
        }

        internal bool ContainsKey(string name)
        {
            foreach (var col in this._list)
            {
                if (col.ColumnName == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddRange(DataColumn[] columns)
        {
            this._list.AddRange(columns);
        }
        public void Clear()
        {
            this._list.Clear();
        }
        public bool Contains(string name)
        {
            foreach (var col in this._list)
            {
                if (col.ColumnName == name)
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(DataColumn column)
        {
            for (int i = 0; i < this._list.Count; i++)
            {
                if (this._list[i] == column)
                {
                    return i;
                }
            }
            return -1;
        }
        public int IndexOf(string columnName)
        {
            for (int i = 0; i < this._list.Count; i++)
            {
                if (this._list[i].ColumnName == columnName)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Remove(DataColumn column)
        {
            int index = IndexOf(column);
            this.List.RemoveAt(index);
        }
        public void Remove(string name)
        {
            int index = IndexOf(name);
            this._list.RemoveAt(index);
        }
        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return this._list.GetEnumerator();
        }
    }
}