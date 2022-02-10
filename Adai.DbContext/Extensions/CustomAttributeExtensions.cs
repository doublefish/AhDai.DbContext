using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Adai.DbContext.Extensions
{
	/// <summary>
	/// CustomAttributeExtensions
	/// </summary>
	public static class CustomAttributeExtensions
	{
		/// <summary>
		/// 读取类的属性的的特性
		/// </summary>
		/// <typeparam name="T">类的属性的特性的类型</typeparam>
		/// <param name="type">类的Type</param>
		/// <returns></returns>
		public static T[] GetColumnAttributes<T>(this Type type) where T : Attributes.ColumnAttribute
		{
			var properties = type.GetProperties();
			var list = new List<T>();
			var typeA = typeof(T);
			foreach (var pi in properties)
			{
				var attrs = pi.GetCustomAttributes(typeA, true);
				if (attrs.Length > 0)
				{
					var attr = attrs[0] as T;
					attr.Property = pi;
					list.Add(attr);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 获取列名和属性的映射关系
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static IDictionary<string, PropertyInfo> GetMappings<T>(this IEnumerable<T> attributes) where T : Attributes.ColumnAttribute
		{
			var dict = new Dictionary<string, PropertyInfo>();
			foreach (var attr in attributes)
			{
				dict.Add(attr.Name, attr.Property);
			}
			return dict;
		}

		/// <summary>
		/// 查询：匹配列名或者属性名
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="attributes"></param>
		/// <param name="name"></param>
		/// <param name="comparisonType"></param>
		/// <returns></returns>
		public static T Find<T>(this IEnumerable<T> attributes, string name, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) where T : Attributes.ColumnAttribute
		{
			var attribute = attributes.Where(o => string.Compare(o.Name, name, comparisonType) == 0).FirstOrDefault();
			if (attribute == null)
			{
				attribute = attributes.Where(o => string.Compare(o.Property.Name, name, comparisonType) == 0).FirstOrDefault();
			}
			return attribute;
		}
	}
}
