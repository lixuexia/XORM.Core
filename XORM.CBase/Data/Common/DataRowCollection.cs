using System.Collections;
using System.Collections.Generic;

namespace XORM.CBase.Data.Common
{
    public class DataRowCollection:IEnumerable
    {
        public DataRowCollection()
        {
            this.Rows = new List<DataRow>();
        }
        public DataRow this[int thisIndex]
        {
            get
            {
                return Rows[thisIndex];
            }
        }

        private int index = -1;
        private List<DataRow> Rows = null;
        public int Count
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows.Count;
            }
        }

        public object Current
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows[index];
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            index++;
            var isNext = index < Rows.Count;
            if (!isNext)
                Reset();
            return isNext;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Add(DataRow daRow)
        {
            if (Rows == null)
            {
                Rows = new List<DataRow>();
            }
            Rows.Add(daRow);
        }

        public IEnumerator GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }
    }
}