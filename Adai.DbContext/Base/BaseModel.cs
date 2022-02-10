namespace Adai.DbContext
{
	/// <summary>
	/// 基类
	/// </summary>
	public class BaseModel : IModel
	{
		#region 扩展
		/// <summary>
		/// 数据库名称
		/// </summary>
		public string DbName { get; set; }
		/// <summary>
		/// 数据表名称
		/// </summary>
		public string TableName { get; set; }
		#endregion
	}
}
