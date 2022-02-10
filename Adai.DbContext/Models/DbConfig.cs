namespace Adai.DbContext.Models
{
	/// <summary>
	/// 数据库配置
	/// </summary>
	public class DbConfig
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public DbConfig() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbType">数据库类型</param>
		/// <param name="name">别名</param>
		/// <param name="connectionString">连接字符串</param>
		public DbConfig(Config.DbType dbType, string name, string connectionString)
		{
			DbType = dbType;
			Name = name;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// 数据库类型
		/// </summary>
		public Config.DbType DbType { get; set; }
		/// <summary>
		/// 别名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 连接字符串
		/// </summary>
		public string ConnectionString { get; set; }
	}
}
