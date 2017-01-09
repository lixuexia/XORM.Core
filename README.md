# XORM.Core 李雪侠
XORM跨平台版
提供一个简单方便的访问工具，支持lamda表达式条件，动态属性条件，动态属性条件，静态语句条件等灵活使用方式
在.NET Core上测试实用通过
（1）使用前建议用XORM.CTool生成代码，也可以自己定义代码
（2）目前提供对SQL Server和MySQL的支持，如需自己定制，可以自行实现接口IDbExecute
（3）因.NET Core的配置实用方式较灵活，使用前可以在Startup.cs的public Startup(IHostingEnvironment env)方法中添加语句：
XORM.CBase.Data.DBHelper.SetConfigurationService(Configuration);即可正常使用
（4）数据库字符串规则：XXX为数据源名，XXX_READ标识只读源，XXX_WRITE标识可写源，用于实现读写分离，代码使用过程中，可以显式指定数据源类型
