using Adai.DbContext.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Adai.DbContext
{
	/// <summary>
	/// DbContextExtensions
	/// </summary>
	public static class DbContextExtensions
	{
		/// <summary>
		/// 获取连接字符串
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <returns></returns>
		static string GetConnectionString(this IDbContext dbContext, string dbName)
		{
			var dbConfig = DbHelper.GetDbConfig(dbContext.DbType, dbName);
			if (dbConfig == null || string.IsNullOrEmpty(dbConfig.ConnectionString))
			{
				throw new ArgumentException($"未配置类型是{dbContext.DbType.ToName()}，别名是{dbName}的数据库配置");
			}
			return dbConfig.ConnectionString;
		}

		/// <summary>
		/// 变更连接字符串
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <returns></returns>
		static void ChangeConnectionString(this IDbContext dbContext, string dbName)
		{
			var connStr = dbContext.GetConnectionString(dbName);
			dbContext.ConnectionString = connStr;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static DataSet GetDataSet(this IDbContext dbContext, string dbName, string sql, params IDbDataParameter[] parameters)
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.GetDataSet(sql, parameters);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static T GetSingle<T>(this IDbContext dbContext, string dbName, string sql, params IDbDataParameter[] parameters) where T : class, new()
		{
			return dbContext.GetList<T>(dbName, sql, parameters).FirstOrDefault();
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static ICollection<T> GetList<T>(this IDbContext dbContext, string sql, params IDbDataParameter[] parameters) where T : class, new()
		{
			var ds = dbContext.GetDataSet(sql, parameters);
			var list = new List<T>();
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				for (var i = 0; i < ds.Tables.Count; i++)
				{
					var _list = ds.Tables[i].ToList<T>();
					list.AddRange(_list);
				}
			}
			else
			{
				var mappings = tableAttr.ColumnAttributes.GetMappings();
				for (var i = 0; i < ds.Tables.Count; i++)
				{
					var _list = ds.Tables[i].ToList<T>(mappings);
					list.AddRange(_list);
				}
			}
			return list;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static ICollection<T> GetList<T>(this IDbContext dbContext, string dbName, string sql, params IDbDataParameter[] parameters) where T : class, new()
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.GetList<T>(sql, parameters);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static object ExecuteScalar(this IDbContext dbContext, string dbName, string sql, params IDbDataParameter[] parameters)
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.ExecuteScalar(sql, parameters);
		}

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static int ExecuteNonQuery(this IDbContext dbContext, string dbName, string sql, params IDbDataParameter[] parameters)
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.ExecuteNonQuery(sql, parameters);
		}

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		public static int ExecuteNonQuery(this IDbContext dbContext, string dbName, IDbCommand command)
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.ExecuteNonQuery(command);
		}

		/// <summary>
		/// 批量执行
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dbName"></param>
		/// <param name="commands"></param>
		/// <returns></returns>
		public static int ExecuteNonQuery(this IDbContext dbContext, string dbName, IDbCommand[] commands)
		{
			dbContext.ChangeConnectionString(dbName);
			return dbContext.ExecuteNonQuery(commands);
		}

		/// <summary>
		/// 跨库执行
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dictCmds">connStr-cmds</param>
		/// <returns></returns>
		public static int ExecuteNonQuery(this IDbContext dbContext, IDictionary<string, ICollection<IDbCommand>> dictCmds)
		{
			var result = 0;
			var conns = new List<IDbConnection>();
			var trans = new List<IDbTransaction>();
			try
			{
				foreach (var kv in dictCmds)
				{
					var conn = dbContext.CreateConnection();
					conn.ConnectionString = kv.Key;
					conn.Open();
					conns.Add(conn);
					var cmds = kv.Value;
					var tran = conn.BeginTransaction();
					trans.Add(tran);
					foreach (var cmd in cmds)
					{
						cmd.Connection = conn;
						cmd.Transaction = tran;
						dbContext.BeforeExecute(cmd);
						result += cmd.ExecuteNonQuery();
					}
				}
				foreach (var tran in trans)
				{
					tran.Commit();
				}
			}
			catch
			{
				foreach (var tran in trans)
				{
					if (tran.Connection.State == ConnectionState.Open)
					{
						tran.Rollback();
					}
				}
				throw;
			}
			finally
			{
				foreach (var conn in conns)
				{
					if (conn.State == ConnectionState.Open)
					{
						conn.Close();
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 跨库执行
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="dictCmds">dbName-cmds</param>
		/// <returns></returns>
		public static int ExecuteNonQueryByDbName(this IDbContext dbContext, IDictionary<string, ICollection<IDbCommand>> dictCmds)
		{
			var _dict = new Dictionary<string, ICollection<IDbCommand>>();
			foreach (var kv in dictCmds)
			{
				var connStr = dbContext.GetConnectionString(kv.Key);
				_dict.Add(connStr, kv.Value);
			}
			return dbContext.ExecuteNonQuery(_dict);
		}

		/// <summary>
		/// 生成查询条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="filter"></param>
		/// <param name="alias"></param>
		/// <param name="parameters"></param>
		/// <returns>查询条件部分SQL语句</returns>
		public static string GenerateQueryCondition<T>(this IDbContext dbContext, IFilter<T> filter, string alias, out IDbDataParameter[] parameters) where T : class, new()
		{
			var tableAttr = DbHelper.GetTableAttribute(filter.Self.GetType());
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			var columns = tableAttr.ColumnAttributes;
			var builder = new StringBuilder();
			var paras = new List<IDbDataParameter>();
			foreach (var column in columns)
			{
				if (column.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				var pi = column.Property;
				var value = pi.GetValue(filter.Self);
				if (value == null)
				{
					continue;
				}
				if (value.IsMinValue())
				{
					continue;
				}
				builder.Append($" AND {alias}.{column.Name}=@{column.Name}");
				paras.Add(dbContext.CreateParameter(column.Name, value));
			}
			parameters = paras.ToArray();
			return builder.ToString();
		}

		/// <summary>
		/// 生成排序条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="filter"></param>
		/// <param name="alias"></param>
		/// <returns>排序条件部分SQL语句</returns>
		public static string GenerateSortCondition<T>(this IDbContext dbContext, IFilter<T> filter, string alias) where T : class, new()
		{
			if (dbContext == null)
			{
				throw new Exception("未初始化");
			}
			var tableAttr = DbHelper.GetTableAttribute(filter.Self.GetType());
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			var columns = tableAttr.ColumnAttributes;
			var builder = new StringBuilder();
			if (!string.IsNullOrEmpty(filter.SortName))
			{
				var sortName = filter.SortName;
				if (!sortName.Contains('.'))
				{
					var sortColumn = columns.Find(filter.SortName);
					sortName = $"{alias}.{sortColumn.Name}";
				}
				else
				{
					// 主表外的字段需要是实际的字段名，后面有空看怎么扩展成自动获取的吧
				}
				var sortType = filter.SortType == Config.SortType.DESC ? "DESC" : "ASC";
				builder.Append($" ORDER BY {sortName} {sortType}");
			}
			return builder.ToString();
		}

		/// <summary>
		/// 生成排序条件
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="take"></param>
		/// <param name="skip"></param>
		/// <param name="sql"></param>
		/// <returns>完整SQL语句</returns>
		public static string GeneratePaginationCondition(this IDbContext dbContext, int take, int skip, string sql)
		{
			var builder = new StringBuilder(sql);
			switch (dbContext.DbType)
			{
				case Config.DbType.MSSQL:
					// 2012版本以上
					builder.Append($" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY");
					break;
				case Config.DbType.MySQL:
				case Config.DbType.SQLite:
					//sql.Append($" LIMIT {filter.Skip},{filter.Take}");
					builder.Append($" LIMIT {take} OFFSET {skip}");
					break;
				case Config.DbType.Oracle:
					//select a.* from ( select t.*,rownum rowno from test t where rownum <= 20 ) a where a.rowno >= 11;
					builder.Insert(0, $"SELECT t1.* FROM (SELECT t0.*,ROWNUM ROWNO FROM (");
					builder.Append($") t0 WHERE ROWNUM <= {take + skip} ) t1 WHERE t1.ROWNO > {skip}");
					break;
				default:
					throw new Exception("无法识别的数据库类型");
			}
			return builder.ToString();
		}

		/// <summary>
		/// 生成InsertCommand
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="data"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <returns></returns>
		public static IDbCommand GenerateInsertCommand<T>(this IDbContext dbContext, T data, string tableName = null) where T : class, new()
		{
			var sql = dbContext.GenerateInsertSql(data, tableName, out var paras);
			var cmd = dbContext.CreateCommand();
			cmd.CommandText = sql;
			if (paras?.Length > 0)
			{
				cmd.Parameters.AddRange(paras);
			}
			return cmd;
		}

		/// <summary>
		/// 生成UpdateCommand
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="data"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <param name="updateColumns"></param>
		/// <param name="whereColumns"></param>
		/// <returns></returns>
		public static IDbCommand GenerateUpdateCommand<T>(this IDbContext dbContext, T data, string tableName, string[] updateColumns, params string[] whereColumns) where T : class, new()
		{
			var sql = dbContext.GenerateUpdateSql(data, tableName, updateColumns, whereColumns, out var paras);
			var cmd = dbContext.CreateCommand();
			cmd.CommandText = sql;
			if (paras?.Length > 0)
			{
				cmd.Parameters.AddRange(paras);
			}
			return cmd;
		}

		/// <summary>
		/// 生成Insert语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="data"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string GenerateInsertSql<T>(this IDbContext dbContext, T data, string tableName, out IDbDataParameter[] parameters) where T : class, new()
		{
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				tableName = tableAttr.Name;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			var columns = new StringBuilder();
			var values = new StringBuilder();
			var paras = new List<IDbDataParameter>();
			foreach (var columnAttr in columnAttrs)
			{
				if (columnAttr.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				var value = columnAttr.Property.GetValue(data);
				columns.Append($",{columnAttr.Name}");
				values.Append($",@{columnAttr.Name}");
				paras.Add(dbContext.CreateParameter(columnAttr.Name, value));
			}
			columns = columns.Remove(0, 1);
			values = values.Remove(0, 1);

			parameters = paras.ToArray();
			var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
			return sql;
		}

		/// <summary>
		/// 生成Insert语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="data"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <returns></returns>
		public static string GenerateInsertSql<T>(this IDbContext dbContext, T data, string tableName) where T : class, new()
		{
			if (dbContext.DbType == Config.DbType.MSSQL || dbContext.DbType == Config.DbType.MySQL)
			{
			}
			else
			{
				throw new NotImplementedException("此方法不支持当前数据库类型");
			}
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				tableName = tableAttr.Name;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			var columns = new StringBuilder();
			var values = new StringBuilder();
			foreach (var columnAttr in columnAttrs)
			{
				if (columnAttr.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				var value = columnAttr.Property.GetValue(data);
				columns.Append($",{columnAttr.Name}");
				values.Append($",'{value}'");
			}
			columns = columns.Remove(0, 1);
			values = values.Remove(0, 1);

			var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
			return sql;
		}

		/// <summary>
		/// 生成Insert语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="datas"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string GenerateInsertSql<T>(this IDbContext dbContext, T[] datas, string tableName, out IDbDataParameter[] parameters) where T : class, new()
		{
			if (dbContext.DbType == Config.DbType.MSSQL || dbContext.DbType == Config.DbType.MySQL)
			{
			}
			else
			{
				throw new NotImplementedException("此方法不支持当前数据库类型");
			}
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				tableName = tableAttr.Name;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			var columns = new StringBuilder();
			var values = new StringBuilder();
			var paras = new List<IDbDataParameter>();
			foreach (var columnAttr in columnAttrs)
			{
				if (columnAttr.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				columns.Append($",{columnAttr.Name}");
				values.Append($",@{columnAttr.Name}_{{0}}");
			}
			columns = columns.Remove(0, 1);
			values = values.Remove(0, 1);

			var format = values.ToString();
			values = new StringBuilder();
			for (var i = 0; i < datas.Length; i++)
			{
				values.AppendLine($",({string.Format(format, i)})");
				var data = datas[i];
				foreach (var columnAttr in columnAttrs)
				{
					if (columnAttr.Type == Attributes.ColumnType.External)
					{
						continue;
					}
					var value = columnAttr.Property.GetValue(data);
					paras.Add(dbContext.CreateParameter($"{columnAttr.Name}_{i}", value));
				}
			}
			values = values.Remove(0, 1);

			parameters = paras.ToArray();
			var sql = $"INSERT INTO {tableName} ({columns}) VALUES {values}";
			return sql;
		}

		/// <summary>
		/// 生成Insert语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="datas"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <returns></returns>
		public static string GenerateInsertSql<T>(this IDbContext dbContext, T[] datas, string tableName) where T : class, new()
		{
			if (dbContext.DbType == Config.DbType.MSSQL || dbContext.DbType == Config.DbType.MySQL)
			{
			}
			else
			{
				throw new NotImplementedException("此方法不支持当前数据库类型");
			}
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				tableName = tableAttr.Name;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			var columns = new StringBuilder();
			foreach (var columnAttr in columnAttrs)
			{
				if (columnAttr.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				columns.Append($",{columnAttr.Name}");
			}
			columns = columns.Remove(0, 1);

			var values = new StringBuilder();
			for (var i = 0; i < datas.Length; i++)
			{
				var data = datas[i];
				var builder = new StringBuilder();
				foreach (var columnAttr in columnAttrs)
				{
					if (columnAttr.Type == Attributes.ColumnType.External)
					{
						continue;
					}
					var value = columnAttr.Property.GetValue(data);
					builder.Append($",'{value}'");
				}
				builder = builder.Remove(0, 1);
				values.AppendLine($",({builder})");
			}
			values = values.Remove(0, 1);

			var sql = $"INSERT INTO {tableName} ({columns}) VALUES {values}";
			return sql;
		}

		/// <summary>
		/// 生成Update语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="data"></param>
		/// <param name="tableName">可为空，读取实体特性</param>
		/// <param name="updateColumns"></param>
		/// <param name="whereColumns"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string GenerateUpdateSql<T>(this IDbContext dbContext, T data, string tableName, string[] updateColumns, string[] whereColumns, out IDbDataParameter[] parameters) where T : class, new()
		{
			var tableAttr = DbHelper.GetTableAttribute<T>();
			if (tableAttr == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				tableName = tableAttr.Name;
			}
			var columnAttrs = tableAttr.ColumnAttributes;
			if (whereColumns == null || whereColumns.Length == 0)
			{
				throw new ArgumentNullException(nameof(whereColumns));
			}
			var set = new StringBuilder();
			var where = new StringBuilder();
			var paras = new List<IDbDataParameter>();
			foreach (var updateColumn in updateColumns)
			{
				var columnAttr = columnAttrs.Find(updateColumn);
				if (columnAttr == null || columnAttr.Type == Attributes.ColumnType.External)
				{
					continue;
				}
				var value = columnAttr.Property.GetValue(data);
				set.Append($",{columnAttr.Name}=@{columnAttr.Name}");
				paras.Add(dbContext.CreateParameter(columnAttr.Name, value));
			}
			set = set.Remove(0, 1);

			foreach (var whereColumn in whereColumns)
			{
				var columnAttr = columnAttrs.Find(whereColumn);
				if (columnAttr == null || columnAttr.Type == Attributes.ColumnType.External)
				{
					throw new ArgumentException($"找不到{whereColumn}对应的列");
				}
				var value = columnAttr.Property.GetValue(data);
				where.Append($" AND {columnAttr.Name}=@{columnAttr.Name}");
				paras.Add(dbContext.CreateParameter(columnAttr.Name, value));
			}
			where = where.Remove(0, 5);

			parameters = paras.ToArray();
			var sql = $"UPDATE {tableName} SET {set} WHERE {where}";
			return sql;
		}

		#region 私有方法

		#endregion
	}
}
