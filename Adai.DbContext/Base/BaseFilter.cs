using System.Collections.Generic;

namespace Adai.DbContext
{
	/// <summary>
	/// BaseFilter
	/// </summary>
	public class BaseFilter<T> : IFilter<T> where T : class, new()
	{
		/// <summary>
		/// 结果
		/// </summary>
		public ICollection<T> Results { get; set; }

		/// <summary>
		/// 结果数
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Model
		/// </summary>
		public T Self { get; set; }

		/// <summary>
		/// Skip
		/// </summary>
		public int Skip { get; set; }

		/// <summary>
		/// Take
		/// </summary>
		public int Take { get; set; }

		/// <summary>
		/// 排序字段
		/// </summary>
		public string SortName { get; set; }

		/// <summary>
		/// 排序规则（1-倒序，0-正序）
		/// </summary>
		public Config.SortType SortType { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseFilter() : this(null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="defaultValues">Self的特殊默认值</param>
		public BaseFilter(IDictionary<string, object> defaultValues = null)
		{
			Self = new T();
			Extensions.ObjectExtensions.SetMinValue(Self, defaultValues);
			SortType = Config.SortType.DESC;
		}
	}
}
