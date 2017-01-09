using XORM.CBase.Attr;

namespace XORM.DemoApp
{
    [DbSource("EBS")]
    public class Admin_UserInfo : XORM.CBase.ModelBase<Admin_UserInfo>
    {
        [DBCol,NoAdd]
        public int Id { get; set; }

        [DBCol]
        public string SoufunName { get; set; }

        public string TrueName { get; set; }
    }
}