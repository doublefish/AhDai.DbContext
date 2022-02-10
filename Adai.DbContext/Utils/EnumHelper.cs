using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Adai.DbContext.Utils
{
	/// <summary>
	/// EnumExtensions
	/// </summary>
	public static class EnumHelper
	{
		readonly static Type DescriptionType;

		/// <summary>
		/// 构造函数
		/// </summary>
		static EnumHelper()
		{
			DescriptionType = typeof(DescriptionAttribute);
		}

		/// <summary>
		/// 获取枚举值的描述
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetValueDescription<T>(T value) where T : Enum
		{
			var type = typeof(T);
			var values = Enum.GetValues(type);
			var description = string.Empty;
			foreach (var v in values)
			{
				if (v.Equals(value))
				{
					var attrs = type.GetField(v.ToString()).GetCustomAttributes(DescriptionType, true);
					if (attrs.Length > 0)
					{
						var attr = attrs.FirstOrDefault() as DescriptionAttribute;
						description = attr.Description;
					}
					break;
				}
			}
			return description;
		}

		/// <summary>
		/// 获取枚举的值的集合
		/// </summary>
		/// <returns></returns>
		public static T[] GetValues<T>() where T : Enum
		{
			var type = typeof(T);
			var values = Enum.GetValues(type);
			var array = new List<T>();
			foreach (var v in values)
			{
				array.Add((T)v);
			}
			return array.ToArray();
		}

		/// <summary>
		/// 获取枚举的值的集合
		/// </summary>
		/// <returns></returns>
		public static IDictionary<T, string> GetValuesAndDescriptions<T>() where T : Enum
		{
			var type = typeof(T);
			var values = Enum.GetValues(type);
			var dict = new Dictionary<T, string>();
			foreach (var v in values)
			{
				var description = string.Empty;
				var attrs = type.GetField(v.ToString()).GetCustomAttributes(DescriptionType, true);
				if (attrs.Length > 0)
				{
					var attr = attrs.FirstOrDefault() as DescriptionAttribute;
					description = attr.Description;
				}
				dict.Add((T)v, description);
			}
			return dict;
		}
	}
}
