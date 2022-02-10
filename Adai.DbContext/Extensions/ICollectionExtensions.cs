using System;
using System.Data;

namespace Adai.DbContext.Extensions
{
	/// <summary>
	/// ICollectionExtensions
	/// </summary>
	public static class ICollectionExtensions
	{
		/// <summary>
		/// AddRange
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="values"></param>
		public static void AddRange(this IDataParameterCollection collection, Array values)
		{
			foreach (var value in values)
			{
				collection.Add(value);
			}
		}
	}
}
