using System.Collections.Generic;

namespace Adai.DbContext
{
	/// <summary>
	/// 筛选
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFilter<T> where T : class
	{
		/// <summary>
		/// 结果
		/// </summary>
		ICollection<T> Results { get; set; }

		/// <summary>
		/// 结果数
		/// </summary>
		int Count { get; set; }

		/// <summary>
		/// 本身
		/// </summary>
		T Self { get; set; }

		/// <summary>
		/// Skip
		/// </summary>
		int Skip { get; set; }

		/// <summary>
		/// Take
		/// </summary>
		int Take { get; set; }

		/// <summary>
		/// 排序字段
		/// </summary>
		string SortName { get; set; }

		/// <summary>
		/// 排序规则（1-倒序，0-正序）
		/// </summary>
		Config.SortType SortType { get; set; }
	}
}
