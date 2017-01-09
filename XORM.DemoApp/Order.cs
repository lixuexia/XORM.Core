using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XORM.CBase;
using XORM.CBase.Attr;

namespace XORM.DemoApp
{
    [DbSource("EBS")]
    public class Order : XORM.CBase.ModelBase<Order>
    {
        [DBCol]
        public string OrderId { get; set; }
        [DBCol]
        public string Amount { get; set; }
        [DBCol]
        public string UserId { get; set; }
    }
}