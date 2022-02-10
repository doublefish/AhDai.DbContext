using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Adai.DbContext.Extensions
{
	/// <summary>
	/// DataTableExtensions
	/// </summary>
	public static class DataTableExtensions
	{
		/// <summary>
		/// ToModel
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		public static T ToModel<T>(this DataRow dataRow) where T : class, new()
		{
			var propertyInfos = typeof(T).GetProperties();
			return dataRow.ToModel<T>(propertyInfos);
		}

		/// <summary>
		/// ToModel
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataRow"></param>
		/// <param name="propertyInfos"></param>
		/// <returns></returns>
		public static T ToModel<T>(this DataRow dataRow, PropertyInfo[] propertyInfos) where T : class, new()
		{
			var data = Activator.CreateInstance<T>();
			foreach (DataColumn dataColumn in dataRow.Table.Columns)
			{
				var name = dataColumn.ColumnName;
				var value = dataRow[name];
				//var pi = propertyInfos.Where(o => o.Name == name).FirstOrDefault();
				// 不区分大小写
				var pi = propertyInfos.Where(o => string.Compare(o.Name, name, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
				if (pi == null || pi.CanWrite == false)
				{
					continue;
				}
				pi.SetValueExt(data, value);
			}
			return data;
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataTable"></param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this DataTable dataTable) where T : class, new()
		{
			var list = new List<T>();
			var propertyInfos = typeof(T).GetProperties();
			foreach (DataRow dataRow in dataTable.Rows)
			{
				var data = dataRow.ToModel<T>(propertyInfos);
				list.Add(data);
			}
			return list;
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataTable"></param>
		/// <param name="mappings">映射（列名-属性名）</param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this DataTable dataTable, IDictionary<string, string> mappings) where T : class, new()
		{
			var dict = GetMappings<T>(mappings);
			return dataTable.ToList<T>(dict);
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataTable"></param>
		/// <param name="mappings">映射（列名-属性）</param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this DataTable dataTable, IDictionary<string, PropertyInfo> mappings) where T : class, new()
		{
			var list = new List<T>();
			foreach (DataRow dataRow in dataTable.Rows)
			{
				var data = Activator.CreateInstance<T>();
				foreach (DataColumn dataColumn in dataTable.Columns)
				{
					var name = dataColumn.ColumnName;
					var value = dataRow[name];
					SetValue(data, name, value, mappings);
				}
				list.Add(data);
			}
			return list;
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataReader"></param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this IDataReader dataReader) where T : class, new()
		{
			var list = new List<T>();
			var properties = typeof(T).GetProperties();
			while (dataReader.Read())
			{
				var data = Activator.CreateInstance<T>();
				for (var i = 0; i < dataReader.FieldCount; i++)
				{
					var name = dataReader.GetName(i);
					var value = dataReader[name];
					var pi = properties.Where(o => o.Name == name).FirstOrDefault();
					if (pi == null || pi.CanWrite == false)
					{
						continue;
					}
					pi.SetValueExt(data, value);
				}
				list.Add(data);
			}
			return list;
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataReader"></param>
		/// <param name="mappings">映射（列名-属性名）</param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this IDataReader dataReader, IDictionary<string, string> mappings) where T : class, new()
		{
			var dict = GetMappings<T>(mappings);
			return dataReader.ToList<T>(dict);
		}

		/// <summary>
		/// ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataReader"></param>
		/// <param name="mappings">映射（列名-属性）</param>
		/// <returns></returns>
		public static ICollection<T> ToList<T>(this IDataReader dataReader, IDictionary<string, PropertyInfo> mappings) where T : class, new()
		{
			var list = new List<T>();
			while (dataReader.Read())
			{
				var data = Activator.CreateInstance<T>();
				for (var i = 0; i < dataReader.FieldCount; i++)
				{
					var name = dataReader.GetName(i);
					var value = dataReader[name];
					SetValue(data, name, value, mappings);
				}
				list.Add(data);
			}
			return list;
		}

		/// <summary>
		/// 赋值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="mappings">映射（列名-属性）</param>
		static void SetValue<T>(T data, string name, object value, IDictionary<string, PropertyInfo> mappings) where T : class
		{
			PropertyInfo pi = null;
			foreach (var kv in mappings)
			{
				if (string.Compare(kv.Key, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					pi = kv.Value;
					break;
				}
			}
			if (pi == null)
			{
				foreach (var kv in mappings)
				{
					if (string.Compare(kv.Value.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						pi = kv.Value;
						break;
					}
				}
			}
			if (pi == null)
			{
				return;
			}
			pi.SetValueExt(data, value);
		}

		/// <summary>
		/// 获取列名和属性的映射关系
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mappings"></param>
		/// <returns></returns>
		static IDictionary<string, PropertyInfo> GetMappings<T>(IDictionary<string, string> mappings)
		{
			var propertyInfos = typeof(T).GetProperties();
			var dict = new Dictionary<string, PropertyInfo>();
			foreach (var kv in mappings)
			{
				var pi = propertyInfos.Where(o => string.Compare(o.Name, kv.Value, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
				if (pi == null)
				{
					throw new Exception($"列名{kv.Value}没有匹配的属性");
				}
				dict.Add(kv.Key, pi);
			}
			return dict;
		}
	}
}
