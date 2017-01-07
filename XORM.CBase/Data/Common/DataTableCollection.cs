using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XORM.CBase.Data.Common
{
    public class DataTableCollection : IEnumerable
    {
        public DataTableCollection()
        {
            this.List = new List<DataTable>();
        }
        public DataTable this[string name]
        {
            get
            {
                foreach (var Tab in List)
                {
                    if (Tab.TableName == name)
                    {
                        return Tab;
                    }
                }
                throw new Exception("指定命名的DataTable不存在");
            }
        }
        public DataTable this[int index] { get { return List[index]; } }
        protected List<DataTable> List { get; }
        public DataTable Add()
        {
            DataTable dt = new DataTable();
            dt.TableName = Guid.NewGuid().ToString().Replace("-", "");
            this.List.Add(dt);
            return dt;
        }
        public void Add(DataTable table)
        {
            this.List.Add(table);
        }
        public DataTable Add(string name)
        {
            DataTable dt = new DataTable();
            dt.TableName = name;
            this.List.Add(dt);
            return dt;
        }
        public void AddRange(DataTable[] tables)
        {
            this.List.AddRange(tables);
        }
        public void Clear()
        {
            this.List.Clear();
        }
        public bool Contains(string name)
        {
            foreach (var Tab in List)
            {
                if (Tab.TableName == name)
                {
                    return true;
                }
            }
            return false;
        }
        public void CopyTo(DataTable[] array, int index)
        {
            for (int i = index; i < this.List.Count; i++)
            {
                array[i - index] = this.List[i];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        public int IndexOf(DataTable table)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if (table == this.List[i])
                {
                    return i;
                }
            }
            return -1;
        }
        public int IndexOf(string tableName)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if (tableName == this.List[i].TableName)
                {
                    return i;
                }
            }
            return -1;
        }
        public void Remove(DataTable table)
        {
            this.List.Remove(table);
        }
        public void Remove(string name)
        {
            int index = IndexOf(name);
            this.List.RemoveAt(index);
        }
        public void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }
        public int Count
        {
            get
            {
                return this.List.Count();
            }
        }
    }
}