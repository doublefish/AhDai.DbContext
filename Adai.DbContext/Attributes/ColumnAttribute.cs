using System;
using System.Reflection;

namespace Adai.DbContext.Attributes
{
	/// <summary>
	/// 列的特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class ColumnAttribute : CustomAttribute
	{
		/// <summary>
		/// 类型
		/// </summary>
		public ColumnType Type { get; set; }
		/// <summary>
		/// 排序（未启用）
		/// </summary>
		public int Order { get; set; }
		/// <summary>
		/// 特定数据类型（未启用）
		/// </summary>
		public string TypeName { get; set; }
		/// <summary>
		/// 属性
		/// </summary>
		public PropertyInfo Property { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="type">类型</param>
		public ColumnAttribute(string name, ColumnType type = ColumnType.Normal) : base(name)
		{
			Type = type;
		}
	}
}
