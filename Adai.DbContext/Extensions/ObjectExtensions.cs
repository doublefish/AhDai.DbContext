using System;
using System.Collections.Generic;
using System.Linq;

namespace Adai.DbContext.Extensions
{
	/// <summary>
	/// ObjectExtensions
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// 克隆
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="obj"></param>
		/// <param name="ignores">忽略的属性</param>
		public static T Clone<T>(this T obj, params string[] ignores) where T : class, new()
		{
			var type = typeof(T);
			var data = new T();
			foreach (var pi in type.GetProperties())
			{
				if (ignores.Contains(pi.Name) || pi.CanRead == false)
				{
					continue;
				}
				var targetPi = type.GetProperty(pi.Name);
				if (targetPi.CanWrite == false)
				{
					continue;
				}
				var value = pi.GetValue(obj, null);
				targetPi.SetValue(data, value, null);
			}
			return data;
		}

		/// <summary>
		/// 设置默认值为对应类型的最小值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="values">例外的默认值，格式：属性名称-值</param>
		public static void SetMinValue(this object obj, IDictionary<string, object> values = null)
		{
			var propertyInfos = obj.GetType().GetProperties();
			foreach (var pi in propertyInfos)
			{
				if (pi.CanWrite == false)
				{
					continue;
				}
				if (values != null && values.TryGetValue(pi.Name, out var value))
				{
					pi.SetValue(obj, value);
					continue;
				}
				var min = obj.GetMinValue(pi.PropertyType);
				pi.SetValue(obj, min);
			}
		}

		/// <summary>
		/// 是否最小值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsMinValue(this object obj, Type type = null)
		{
			var min = obj.GetMinValue(type);
			return obj.Equals(min);
		}

		/// <summary>
		/// 获取最小值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetMinValue(this object obj, Type type = null)
		{
			type ??= obj.GetType();
			return type.FullName switch
			{
				"System.Byte" => byte.MinValue,
				"System.SByte" => sbyte.MinValue,
				"System.Char" => char.MinValue,
				"System.String" => string.Empty,
				"System.Int16" => short.MinValue,
				"System.Int32" => int.MinValue,
				"System.Int64" => long.MinValue,
				"System.UInt16" => ushort.MinValue,
				"System.UInt32" => uint.MinValue,
				"System.UInt64" => ulong.MinValue,
				"System.Boolean" => false,
				"System.Single" => float.MinValue,
				"System.Double" => double.MinValue,
				"System.Decimal" => decimal.MinValue,
				"System.DateTime" => DateTime.MinValue,
				_ => null,
			};
		}
	}
}
