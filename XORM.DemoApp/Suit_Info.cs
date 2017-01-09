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
    public class Suit_Info : ModelBase<Suit_Info>
    {
        [PrimaryKey, DBCol, AutoInCrement]
        public int ID { get; set; }
        [DBCol]
        public string SuitName { get; set; }
        [DBCol]
        public string CityName { get; set; }
        [DBCol]
        public DateTime CreateTime { get; set; }
        [DBCol]
        public decimal SinglePrice { get; set; }
        [DBCol]
        public decimal MinArea { get; set; }
    }
}