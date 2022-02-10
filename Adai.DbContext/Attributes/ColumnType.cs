namespace Adai.DbContext.Attributes
{
	/// <summary>
	/// 列类型
	/// </summary>
	public enum ColumnType
	{
		/// <summary>
		/// 普通
		/// </summary>
		Normal,
		/// <summary>
		/// 主键
		/// </summary>
		Primary,
		/// <summary>
		/// 外键
		/// </summary>
		Foreign,
		/// <summary>
		/// 扩展
		/// </summary>
		External
	}
}
