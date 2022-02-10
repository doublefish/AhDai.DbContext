using Adai.DbContext.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Adai.DbContext
{
	/// <summary>
	/// DbHelper
	/// </summary>
	public static class DbHelper
	{
		/// <summary>
		/// 表特性
		/// </summary>
		static readonly IDictionary<string, Attributes.TableAttribute> TableAttributes;
		static readonly object Locker;

		/// <summary>
		/// 数据库配置
		/// </summary>
		public static ICollection<Models.DbConfig> DbConfigs { get; private set; }

		/// <summary>
		/// 执行之前
		/// </summary>
		public static Action<string, IDbCommand> BeforeExecuteAction { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		static DbHelper()
		{
			TableAttributes = new Dictionary<string, Attributes.TableAttribute>();
			Locker = new object();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="dbConfigs">数据库配置</param>
		/// <param name="beforeExecute">执行之前执行，可用于记录SQL，第一个参数是初始化时传入的EventId</param>
		public static void Init(ICollection<Models.DbConfig> dbConfigs, Action<string, IDbCommand> beforeExecute = null)
		{
			var names = new HashSet<string>();
			foreach (var dbConfig in dbConfigs)
			{
				if (!names.Add(dbConfig.Name))
				{
					throw new ArgumentException($"存在重复的别名：{dbConfig.Name}", nameof(dbConfigs));
				}
			}
			DbConfigs = dbConfigs;
			BeforeExecuteAction = beforeExecute;
		}

		/// <summary>
		/// 获取表特性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Attributes.TableAttribute GetTableAttribute<T>() where T : class, new()
		{
			return GetTableAttribute(typeof(T));
		}

		/// <summary>
		/// 获取表特性
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Attributes.TableAttribute GetTableAttribute(Type type)
		{
			if (!TableAttributes.TryGetValue(type.FullName, out var data))
			{
				lock (Locker)
				{
					var typeA = typeof(Attributes.TableAttribute);
					var attrs = type.GetCustomAttributes(typeA, true);
					if (attrs != null && attrs.Length > 0)
					{
						data = attrs.FirstOrDefault() as Attributes.TableAttribute;
						data.ColumnAttributes = type.GetColumnAttributes<Attributes.ColumnAttribute>();
					}
					TableAttributes[type.FullName] = data;
				}
			}
			return data;
		}

		/// <summary>
		/// 获取列特性
		/// </summary>
		/// <param name="column">列名或对应的属性名</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Attributes.ColumnAttribute GetColumnAttribute<T>(string column) where T : class, new()
		{
			return GetColumnAttribute(column, typeof(T));
		}

		/// <summary>
		/// 获取列特性
		/// </summary>
		/// <param name="column">列名或对应的属性名</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Attributes.ColumnAttribute GetColumnAttribute(string column, Type type)
		{
			var tableAttr = GetTableAttribute(type);
			if (tableAttr == null)
			{
				return null;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			return columnAttrs.Find(column);
		}

		/// <summary>
		/// 获取数据库配置
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public static Models.DbConfig GetDbConfig(Config.DbType dbType, string dbName)
		{
			if (DbConfigs == null || DbConfigs.Count == 0)
			{
				throw new Exception("程序尚未初始化，请先执行Init");
			}
			if (string.IsNullOrEmpty(dbName))
			{
				throw new ArgumentNullException(nameof(dbName));
			}
			var dbConfig = DbConfigs.Where(o => o.DbType == dbType && o.Name == dbName).FirstOrDefault();
			return dbConfig;
		}

		/// <summary>
		/// 生成 In/ Not In 语句，一千个值一组
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <param name="values"></param>
		/// <param name="notIn"></param>
		/// <returns></returns>
		static string GenerateInSql<T>(string column, T[] values, bool notIn = false)
		{
			string key_word0, key_word1;
			if (notIn)
			{
				key_word0 = "AND";
				key_word1 = "NOT IN";
			}
			else
			{
				key_word0 = "OR";
				key_word1 = "IN";
			}
			var type = typeof(T);
			var builder = new StringBuilder();
			int skip = 0, take = 1000;
			if (type.FullName == "System.String")
			{
				while (skip < values.Length)
				{
					var _values = values.Skip(skip).Take(take);
					builder.Append($"{key_word0} {column} {key_word1} ('{string.Join("','", _values)}')");
					skip += take;
				}
			}
			else
			{
				while (skip < values.Length)
				{
					var _values = values.Skip(skip).Take(take);
					builder.Append($"{key_word0} {column} {key_word1} ({string.Join(",", _values)})");
					skip += take;
				}
			}
			var sql = "";
			if (builder.Length > 0)
			{
				sql = builder.Remove(0, key_word0.Length + 1).ToString();
				if (skip > 0)
				{
					sql = $"({sql})";
				}
			}
			return sql;
		}

		/// <summary>
		/// 生成 In 语句，一千个值一组
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column">列名</param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string GenerateInSql<T>(string column, T[] values)
		{
			return GenerateInSql(column, values, false);
		}

		/// <summary>
		/// 生成 Not In 语句，一千个值一组
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column">列名</param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string GenerateNotInSql<T>(string column, T[] values)
		{
			return GenerateInSql(column, values, true);
		}

		#region 复杂方法

		#endregion
	}
}
