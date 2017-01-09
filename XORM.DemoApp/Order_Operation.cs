using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XORM.CBase;
using XORM.CBase.Attr;

namespace XORM.DemoApp
{
    [DbSource("EBS")]
    public class Order_Operation : ModelBase<Order_Operation>
    {
        #region 字段、属性
        /// <summary>
        /// 
        /// </summary>
        [AutoInCrement, DBCol]
        public Int32 ID
        {
            get { return this._ID; }
            set { this._ID = value; ModifiedColumns.Add("[ID]"); }
        }
        private Int32 _ID = 0;
        /// <summary>
        /// 订单号
        /// </summary>
        [PrimaryKey, DBCol]
        public string OrderId
        {
            get { return this._OrderId; }
            set { this._OrderId = value; ModifiedColumns.Add("[ORDERID]"); }
        }
        private string _OrderId = "";
        /// <summary>
        /// 订单总金额
        /// </summary>
        [DBCol]
        public decimal Amount
        {
            get { return this._Amount; }
            set { this._Amount = value; ModifiedColumns.Add("[AMOUNT]"); }
        }
        private decimal _Amount = 0M;
        /// <summary>
        /// 已支付总额
        /// </summary>
        [DBCol]
        public decimal Paid
        {
            get { return this._Paid; }
            set { this._Paid = value; ModifiedColumns.Add("[PAID]"); }
        }
        private decimal _Paid = 0M;
        /// <summary>
        /// 未支付总额
        /// </summary>
        [DBCol]
        public decimal Unpay
        {
            get { return this._Unpay; }
            set { this._Unpay = value; ModifiedColumns.Add("[UNPAY]"); }
        }
        private decimal _Unpay = 0M;
        /// <summary>
        /// 
        /// </summary>
        [DBCol]
        public Int32 IsDel
        {
            get { return this._IsDel; }
            set { this._IsDel = value; ModifiedColumns.Add("[ISDEL]"); }
        }
        private Int32 _IsDel = 0;
        /// <summary>
        /// 
        /// </summary>
        [DBCol]
        public DateTime CreateTime
        {
            get { return this._CreateTime; }
            set { this._CreateTime = value; ModifiedColumns.Add("[CREATETIME]"); }
        }
        private DateTime _CreateTime = DateTime.Now;
        #endregion
    }
}