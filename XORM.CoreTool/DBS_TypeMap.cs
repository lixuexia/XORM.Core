using System.Collections.Generic;

namespace XORM.CoreTool
{
    public class DBS_TypeMap
    {
        private Dictionary<string, DBS_AtomType> DIC = new Dictionary<string, DBS_AtomType>();

        private List<string> KeyWordList = new List<string>();

        public DBS_AtomType this[string typeName]
        {
            get { return DIC[typeName]; }
        }

        public DBS_TypeMap()
        {
            #region 布尔型
            this.DIC.Add("bit", new DBS_AtomType("bit", "bool", "Convert.ToBoolean", "System.Data.SqlDbType.Bit"));
            #endregion

            #region 字节型
            this.DIC.Add("tinyint", new DBS_AtomType("tinyint", "int", "Convert.ToInt32", "System.Data.SqlDbType.TinyInt"));
            this.DIC.Add("varbinary", new DBS_AtomType("varbinary", "Byte[]", "(Byte[])", "System.Data.SqlDbType.VarBinary"));
            this.DIC.Add("timestamp", new DBS_AtomType("timestamp", "Byte[]", "(Byte[])", "System.Data.SqlDbType.Timestamp"));
            this.DIC.Add("image", new DBS_AtomType("image", "Byte[]", "(Byte[])", "System.Data.SqlDbType.Image"));
            this.DIC.Add("binary", new DBS_AtomType("binary", "Byte[]", "(Byte[])", "System.Data.SqlDbType.Binary"));
            #endregion

            #region 整型
            this.DIC.Add("smallint", new DBS_AtomType("smallint", "Int16", "Convert.ToInt16", "System.Data.SqlDbType.SmallInt"));
            this.DIC.Add("int", new DBS_AtomType("int", "Int32", "Convert.ToInt32", "System.Data.SqlDbType.Int"));
            this.DIC.Add("bigint", new DBS_AtomType("bigint", "Int64", "Convert.ToInt64", "System.Data.SqlDbType.BigInt"));
            #endregion

            #region 时间型
            this.DIC.Add("date", new DBS_AtomType("datetime", "DateTime", "Convert.ToDateTime", "System.Data.SqlDbType.DateTime"));
            this.DIC.Add("datetime", new DBS_AtomType("datetime", "DateTime", "Convert.ToDateTime", "System.Data.SqlDbType.DateTime"));
            this.DIC.Add("datetime2", new DBS_AtomType("datetime", "DateTime", "Convert.ToDateTime", "System.Data.SqlDbType.DateTime2"));
            this.DIC.Add("smalldatetime", new DBS_AtomType("smalldatetime", "int", "Convert.ToDateTime", "System.Data.SqlDbType.SmallDateTime"));
            #endregion
            
            #region 浮点型
            this.DIC.Add("decimal", new DBS_AtomType("decimal", "decimal", "Convert.ToDecimal", "System.Data.SqlDbType.Decimal"));
            this.DIC.Add("numeric", new DBS_AtomType("numeric", "decimal", "Convert.ToDecimal", "System.Data.SqlDbType.Decimal"));
            this.DIC.Add("float", new DBS_AtomType("float", "double", "Convert.ToDouble", "System.Data.SqlDbType.Float"));
            this.DIC.Add("money", new DBS_AtomType("money", "decimal", "Convert.ToDecimal", "System.Data.SqlDbType.Money"));
            this.DIC.Add("smallmoney", new DBS_AtomType("smallmoney", "decimal", "Convert.ToDecimal", "System.Data.SqlDbType.Decimal"));
            this.DIC.Add("real", new DBS_AtomType("real", "decimal", "Convert.ToDecimal", "System.Data.SqlDbType.Real"));
            #endregion

            #region 字符串类型
            this.DIC.Add("char", new DBS_AtomType("char", "string", "Convert.ToString", "System.Data.SqlDbType.Char"));
            this.DIC.Add("nchar", new DBS_AtomType("nchar", "string", "Convert.ToString", "System.Data.SqlDbType.NChar"));
            this.DIC.Add("text", new DBS_AtomType("text", "string", "Convert.ToString", "System.Data.SqlDbType.Text"));
            this.DIC.Add("ntext", new DBS_AtomType("ntext", "string", "Convert.ToString", "System.Data.SqlDbType.NText"));
            this.DIC.Add("varchar", new DBS_AtomType("varchar", "string", "Convert.ToString", "System.Data.SqlDbType.VarChar"));
            this.DIC.Add("nvarchar", new DBS_AtomType("nvarchar", "string", "Convert.ToString", "System.Data.SqlDbType.NVarChar"));
            #endregion

            this.DIC.Add("uniqueidentifier", new DBS_AtomType("uniqueidentifier", "System.Guid", "Convert.ToString", "System.Data.SqlDbType.UniqueIdentifier"));

            this.DIC.Add("xml", new DBS_AtomType("xml", "string", "Convert.ToString", "System.Data.SqlDbType.Xml"));

            string[] KeyWordArray = CodeKeyWord.Split('|');
            this.KeyWordList.Clear();
            this.KeyWordList.AddRange(KeyWordArray);
        }

        public bool IsKeyWord(string colName)
        {
            return this.KeyWordList.Contains(colName);
        }

        public const string CodeKeyWord =
@"abstract
|as
|base
|bool
|break
|byte
|case
|catch
|char
|checked
|class
|const
|continue
|decimal
|default
|delegate
|do
|double
|else
|enum
|event
|explicit
|extern
|false
|finally
|fixed
|float
|for
|foreach
|goto
|if
|implicit
|in
|int
|interface
|internal
|is
|lock
|long
|namespace
|new
|null
|operator
|out
|override
|params
|private
|protected
|public
|readonly
|ref
|return
|sbyte
|sealed
|short
|sizeof
|stackalloc
|static
|string
|struct
|switch
|this
|throw
|true
|try
|typeof
|uint
|ulong
|unchecked
|unsafe
|ushort
|using
|virtual
|void
|volatile
|while";
    }
}