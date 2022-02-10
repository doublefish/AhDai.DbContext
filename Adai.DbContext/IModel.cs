namespace Adai.DbContext
{
	/// <summary>
	/// 实体
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// 数据库名称
		/// </summary>
		string DbName { get; set; }
		/// <summary>
		/// 数据表名称
		/// </summary>
		string TableName { get; set; }
	}
}
