using Adai.DbContext.Attributes;
using System;

namespace Adai.DbContext.MySql.Models
{
	/// <summary>
	/// 表
	/// </summary>
	[Table("TABLES")]
	public class TABLES
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
		/// TABLE_TYPE
		/// </summary>
		[Column("TABLE_TYPE")]
		public string TABLE_TYPE { get; set; }
		/// <summary>
		/// ENGINE
		/// </summary>
		[Column("ENGINE")]
		public string ENGINE { get; set; }
		/// <summary>
		/// VERSION
		/// </summary>
		[Column("VERSION")]
		public long? VERSION { get; set; }
		/// <summary>
		/// ROW_FORMAT
		/// </summary>
		[Column("ROW_FORMAT")]
		public string ROW_FORMAT { get; set; }
		/// <summary>
		/// TABLE_ROWS
		/// </summary>
		[Column("TABLE_ROWS")]
		public long? TABLE_ROWS { get; set; }
		/// <summary>
		/// AVG_ROW_LENGTH
		/// </summary>
		[Column("AVG_ROW_LENGTH")]
		public long? AVG_ROW_LENGTH { get; set; }
		/// <summary>
		/// DATA_LENGTH
		/// </summary>
		[Column("DATA_LENGTH")]
		public long? DATA_LENGTH { get; set; }
		/// <summary>
		/// MAX_DATA_LENGTH
		/// </summary>
		[Column("MAX_DATA_LENGTH")]
		public long? MAX_DATA_LENGTH { get; set; }
		/// <summary>
		/// INDEX_LENGTH
		/// </summary>
		[Column("INDEX_LENGTH")]
		public long? INDEX_LENGTH { get; set; }
		/// <summary>
		/// DATA_FREE
		/// </summary>
		[Column("DATA_FREE")]
		public long? DATA_FREE { get; set; }
		/// <summary>
		/// AUTO_INCREMENT
		/// </summary>
		[Column("AUTO_INCREMENT")]
		public long? AUTO_INCREMENT { get; set; }
		/// <summary>
		/// CREATE_TIME
		/// </summary>
		[Column("CREATE_TIME")]
		public DateTime? CREATE_TIME { get; set; }
		/// <summary>
		/// UPDATE_TIME
		/// </summary>
		[Column("UPDATE_TIME")]
		public DateTime? UPDATE_TIME { get; set; }
		/// <summary>
		/// CHECK_TIME
		/// </summary>
		[Column("CHECK_TIME")]
		public DateTime? CHECK_TIME { get; set; }
		/// <summary>
		/// TABLE_COLLATION
		/// </summary>
		[Column("TABLE_COLLATION")]
		public string TABLE_COLLATION { get; set; }
		/// <summary>
		/// CHECKSUM
		/// </summary>
		[Column("CHECKSUM")]
		public long? CHECKSUM { get; set; }
		/// <summary>
		/// CREATE_OPTIONS
		/// </summary>
		[Column("CREATE_OPTIONS")]
		public string CREATE_OPTIONS { get; set; }
		/// <summary>
		/// TABLE_COMMENT
		/// </summary>
		[Column("TABLE_COMMENT")]
		public string TABLE_COMMENT { get; set; }
	}
}
