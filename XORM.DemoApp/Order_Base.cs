using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XORM.CBase;
using XORM.CBase.Attr;

namespace XORM.DemoApp
{
    /// <summary>
    /// 数据实体类:Order_Base
    /// </summary>
    [DbSource("EBS")]
    public class Order_Base : ModelBase<Order_Base>
    {
        #region 字段、属性
        /// <summary>
        /// 订单号
        /// </summary>
        [AutoInCrement, DBCol]
        public Int32 ID
        {
            get { return this._ID; }
            set { this._ID = value; ModifiedColumns.Add("[ID]"); }
        }
        private Int32 _ID = 0;
        /// <summary>
        /// 
        /// </summary>
        [PrimaryKey, DBCol]
        public string OrderId
        {
            get { return this._OrderId; }
            set { this._OrderId = value; ModifiedColumns.Add("[ORDERID]"); }
        }
        private string _OrderId = "";
        /// <summary>
        /// 来源编码
        /// </summary>
        [DBCol]
        public Int32 SourceId
        {
            get { return this._SourceId; }
            set { this._SourceId = value; ModifiedColumns.Add("[SOURCEID]"); }
        }
        private Int32 _SourceId = 0;
        /// <summary>
        /// 来源名称
        /// </summary>
        [DBCol]
        public string SourceName
        {
            get { return this._SourceName; }
            set { this._SourceName = value; ModifiedColumns.Add("[SOURCENAME]"); }
        }
        private string _SourceName = "";
        /// <summary>
        /// 状态编码
        /// </summary>
        [DBCol]
        public Int32 StatusId
        {
            get { return this._StatusId; }
            set { this._StatusId = value; ModifiedColumns.Add("[STATUSID]"); }
        }
        private Int32 _StatusId = 0;
        /// <summary>
        /// 状态名称
        /// </summary>
        [DBCol]
        public string StatusName
        {
            get { return this._StatusName; }
            set { this._StatusName = value; ModifiedColumns.Add("[STATUSNAME]"); }
        }
        private string _StatusName = "";
        /// <summary>
        /// 城市编码
        /// </summary>
        [DBCol]
        public Int32 CityId
        {
            get { return this._CityId; }
            set { this._CityId = value; ModifiedColumns.Add("[CITYID]"); }
        }
        private Int32 _CityId = 0;
        /// <summary>
        /// 城市名称
        /// </summary>
        [DBCol]
        public string CityName
        {
            get { return this._CityName; }
            set { this._CityName = value; ModifiedColumns.Add("[CITYNAME]"); }
        }
        private string _CityName = "";
        /// <summary>
        /// 业主姓名
        /// </summary>
        [DBCol]
        public string TrueName
        {
            get { return this._TrueName; }
            set { this._TrueName = value; ModifiedColumns.Add("[TRUENAME]"); }
        }
        private string _TrueName = "";
        /// <summary>
        /// 业主电话
        /// </summary>
        [DBCol]
        public string Phone
        {
            get { return this._Phone; }
            set { this._Phone = value; ModifiedColumns.Add("[PHONE]"); }
        }
        private string _Phone = "";
        /// <summary>
        /// 性别，-1未选择，0女，1男
        /// </summary>
        [DBCol]
        public Int32 Sex
        {
            get { return this._Sex; }
            set { this._Sex = value; ModifiedColumns.Add("[SEX]"); }
        }
        private Int32 _Sex = -1;
        /// <summary>
        /// 创建时间
        /// </summary>
        [DBCol]
        public DateTime CreateTime
        {
            get { return this._CreateTime; }
            set { this._CreateTime = value; ModifiedColumns.Add("[CREATETIME]"); }
        }
        private DateTime _CreateTime = DateTime.Now;
        /// <summary>
        /// 伪删除标记,0否,1是
        /// </summary>
        [DBCol]
        public Int32 IsDel
        {
            get { return this._IsDel; }
            set { this._IsDel = value; ModifiedColumns.Add("[ISDEL]"); }
        }
        private Int32 _IsDel = 0;
        #endregion
    }
}