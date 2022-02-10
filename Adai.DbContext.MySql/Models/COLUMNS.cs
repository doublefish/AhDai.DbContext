using Adai.DbContext.Attributes;

namespace Adai.DbContext.MySql.Models
{
	/// <summary>
	/// 列
	/// </summary>
	[Table("COLUMNS")]
	public class COLUMNS
	{
		/// <summary>
		/// TABLE_CATALOG
		/// </summary>
		[Column("TABLE_CATALOG")]
		public string TABLE_CATALOG { get; set; }
		/// <summary>
		/// TABLE_SCHEMA
		/// </summary>
		[Column("TABLE_SCHEMA")]
		public string TABLE_SCHEMA { get; set; }
		/// <summary>
		/// TABLE_NAME
		/// </summary>
		[Column("TABLE_NAME")]
		public string TABLE_NAME { get; set; }
		/// <summary>
		/// COLUMN_NAME
		/// </summary>
		[Column("COLUMN_NAME")]
		public string COLUMN_NAME { get; set; }
		/// <summary>
		/// ORDINAL_POSITION
		/// </summary>
		[Column("ORDINAL_POSITION")]
		public long ORDINAL_POSITION { get; set; }
		/// <summary>
		/// COLUMN_DEFAULT
		/// </summary>
		[Column("COLUMN_DEFAULT")]
		public string COLUMN_DEFAULT { get; set; }
		/// <summary>
		/// IS_NULLABLE
		/// </summary>
		[Column("IS_NULLABLE")]
		public string IS_NULLABLE { get; set; }
		/// <summary>
		/// DATA_TYPE
		/// </summary>
		[Column("DATA_TYPE")]
		public string DATA_TYPE { get; set; }
		/// <summary>
		/// CHARACTER_MAXIMUM_LENGTH
		/// </summary>
		[Column("CHARACTER_MAXIMUM_LENGTH")]
		public long? CHARACTER_MAXIMUM_LENGTH { get; set; }
		/// <summary>
		/// CHARACTER_OCTET_LENGTH
		/// </summary>
		[Column("CHARACTER_OCTET_LENGTH")]
		public long? CHARACTER_OCTET_LENGTH { get; set; }
		/// <summary>
		/// NUMERIC_PRECISION
		/// </summary>
		[Column("NUMERIC_PRECISION")]
		public long? NUMERIC_PRECISION { get; set; }
		/// <summary>
		/// NUMERIC_SCALE
		/// </summary>
		[Column("NUMERIC_SCALE")]
		public long? NUMERIC_SCALE { get; set; }
		/// <summary>
		/// DATETIME_PRECISION
		/// </summary>
		[Column("DATETIME_PRECISION")]
		public long? DATETIME_PRECISION { get; set; }
		/// <summary>
		/// CHARACTER_SET_NAME
		/// </summary>
		[Column("CHARACTER_SET_NAME")]
		public string CHARACTER_SET_NAME { get; set; }
		/// <summary>
		/// COLLATION_NAME
		/// </summary>
		[Column("COLLATION_NAME")]
		public string COLLATION_NAME { get; set; }
		/// <summary>
		/// COLUMN_TYPE
		/// </summary>
		[Column("COLUMN_TYPE")]
		public string COLUMN_TYPE { get; set; }
		/// <summary>
		/// COLUMN_KEY
		/// </summary>
		[Column("COLUMN_KEY")]
		public string COLUMN_KEY { get; set; }
		/// <summary>
		/// EXTRA
		/// </summary>
		[Column("EXTRA")]
		public string EXTRA { get; set; }
		/// <summary>
		/// PRIVILEGES
		/// </summary>
		[Column("PRIVILEGES")]
		public string PRIVILEGES { get; set; }
		/// <summary>
		/// COLUMN_COMMENT
		/// </summary>
		[Column("COLUMN_COMMENT")]
		public string COLUMN_COMMENT { get; set; }
		/// <summary>
		/// GENERATION_EXPRESSION
		/// </summary>
		[Column("GENERATION_EXPRESSION")]
		public string GENERATION_EXPRESSION { get; set; }
	}
}
