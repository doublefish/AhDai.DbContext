namespace Adai.DbContext.Extensions
{
	/// <summary>
	/// ConfigExtensions
	/// </summary>
	public static class ConfigExtensions
	{
		/// <summary>
		/// 转换为名称
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string ToName(this Config.DbType dbType)
		{
			var typeName = dbType switch
			{
				Config.DbType.MSSQL => "MSSQL",
				Config.DbType.MySQL => "MySQL",
				Config.DbType.Oracle => "Oracle",
				Config.DbType.SQLite => "SQLite",
				_ => null,
			};
			return typeName;
		}
	}
}
