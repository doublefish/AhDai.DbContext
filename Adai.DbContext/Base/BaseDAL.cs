using Adai.DbContext.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Adai.DbContext
{
	/// <summary>
	/// BaseDAL
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public abstract class BaseDAL<Model> where Model : BaseModel, new()
	{
		string selectSql;
		/// <summary>
		/// 实体特性
		/// </summary>
		public Attributes.TableAttribute TableAttribute { get; private set; }

		/// <summary>
		/// 数据库名
		/// </summary>
		public string DbName { get; protected set; }
		/// <summary>
		/// 数据表名
		/// </summary>
		public string TableName { get; protected set; }
		/// <summary>
		/// 主键名称
		/// </summary>
		public string PrimaryKey { get; private set; }

		/// <summary>
		/// DbContext
		/// </summary>
		protected IDbContext DbContext { get; private set; }

		/// <summary>
		/// 查询语句
		/// </summary>
		protected string SelectSql
		{
			get
			{
				if (string.IsNullOrEmpty(selectSql))
				{
					selectSql = InitSelectSql();
				}
				return selectSql;
			}
		}

		/// <summary>
		/// 别名
		/// </summary>
		protected string Alias { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseDAL() : this(null, null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbName">数据库名</param>
		/// <param name="tableName">数据表名</param>
		public BaseDAL(string dbName, string tableName)
		{
			DbName = dbName;
			TableName = tableName;
			Alias = "t";
			TableAttribute = DbHelper.GetTableAttribute<Model>();
			if (TableAttribute == null)
			{
				throw new Exception("未设置表特性");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				TableName = TableAttribute.Name;
			}
			var primaryAttr = TableAttribute.ColumnAttributes.Where(o => o.Type == Attributes.ColumnType.Primary).FirstOrDefault();
			if (primaryAttr != null)
			{
				PrimaryKey = primaryAttr.Name;
			}
			DbContext = InitDbContext();
		}

		/// <summary>
		/// InitDbContext
		/// </summary>
		/// <returns></returns>
		protected abstract IDbContext InitDbContext();

		/// <summary>
		/// 初始化查询语句
		/// </summary>
		protected virtual string InitSelectSql()
		{
			return $"SELECT {Alias}.* FROM {TableName} {Alias} WHERE 1=1";
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual Model GetById(string id)
		{
			return GetByColumn(PrimaryKey, id);
		}

		/// <summary>
		/// 根据Id查询
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public virtual ICollection<Model> ListByIds(params string[] ids)
		{
			return ListByColumn(PrimaryKey, ids);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual ICollection<Model> List<T>(T filter) where T : IFilter<Model>
		{
			var sql = SelectSql;
			var paras = new List<IDbDataParameter>();
			sql += GenerateExtendQueryCondition(filter, out var paras1);
			if (paras1 != null)
			{
				paras.AddRange(paras1);
			}
			sql += DbContext.GenerateQueryCondition(filter, Alias, out var paras0);
			if (paras0 != null)
			{
				paras.AddRange(paras0);
			}
			sql += DbContext.GenerateSortCondition(filter, Alias);
			if (filter.Take > 0)
			{
				var countSql = $"SELECT COUNT(1) FROM ({sql}) t";
				var objCount = DbContext.ExecuteScalar(DbName, countSql, paras.ToArray());
				if (objCount != null && objCount != DBNull.Value)
				{
					filter.Count = Convert.ToInt32(objCount);
				}
				sql = DbContext.GeneratePaginationCondition(filter.Take, filter.Skip, sql);
				filter.Results = GetList(sql, paras.ToArray());
			}
			else
			{
				filter.Results = GetList(sql, paras.ToArray());
				filter.Count = filter.Results.Count;
			}
			return filter.Results;
		}

		/// <summary>
		/// 生成扩展查询条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected virtual string GenerateExtendQueryCondition<T>(T filter, out IDbDataParameter[] parameters) where T : IFilter<Model>
		{
			parameters = null;
			return null;
		}

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public virtual int Add(Model data)
		{
			var cmd = DbContext.GenerateInsertCommand(data, TableName);
			return DbContext.ExecuteNonQuery(DbName, cmd);
		}

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="data"></param>
		/// <param name="updateColumns"></param>
		/// <param name="whereColumns">可为空，读取实体特性主键</param>
		/// <returns></returns>
		public virtual int Update(Model data, string[] updateColumns, params string[] whereColumns)
		{
			if (whereColumns == null || whereColumns.Length == 0)
			{
				whereColumns = new string[] { PrimaryKey };
			}
			var cmd = DbContext.GenerateUpdateCommand(data, TableName, updateColumns, whereColumns);
			return DbContext.ExecuteNonQuery(DbName, cmd);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column">列名或对应的属性名</param>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual Model GetByColumn<T>(string column, T value)
		{
			var columnAttr = DbHelper.GetColumnAttribute<Model>(column);
			var columnName = columnAttr != null ? columnAttr.Name : column;
			var sql = $"{SelectSql} AND {Alias}.{columnName}=@{column} LIMIT 0,1";
			var para = DbContext.CreateParameter(column, value);
			return GetList(sql, para).FirstOrDefault();
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column">列名或对应的属性名</param>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual ICollection<Model> ListByColumn<T>(string column, T value)
		{
			var columnAttr = DbHelper.GetColumnAttribute<Model>(column);
			var columnName = columnAttr != null ? columnAttr.Name : column;
			var sql = $"{SelectSql} AND {Alias}.{columnName}=@{column}";
			var para = DbContext.CreateParameter(column, value);
			return GetList(sql, para);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column">列名或对应的属性名</param>
		/// <param name="values"></param>
		/// <returns></returns>
		public virtual ICollection<Model> ListByColumn<T>(string column, T[] values)
		{
			var columnAttr = DbHelper.GetColumnAttribute<Model>(column);
			var columnName = columnAttr != null ? columnAttr.Name : column;
			var sql_in = DbHelper.GenerateInSql($"{Alias}.{columnName}", values);
			var sql = $"{SelectSql} AND {sql_in}";
			return GetList(sql);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dbName"></param>
		/// <param name="tableNames"></param>
		/// <param name="column">列名或对应的属性名</param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ICollection<Model> ListByColumn<T>(string dbName, string[] tableNames, string column, T value)
		{
			var columnAttr = DbHelper.GetColumnAttribute<Model>(column);
			var columnName = columnAttr != null ? columnAttr.Name : column;
			var sqls = new StringBuilder();
			foreach (var tableName in tableNames)
			{
				sqls.AppendLine($"SELECT * FROM {tableName} WHERE {columnName}=@{column};");
			}
			var sql = sqls.ToString();
			var para = DbContext.CreateParameter(column, value);
			var ds = DbContext.GetDataSet(dbName, sql, para);
			var mappings = TableAttribute.ColumnAttributes.GetMappings();
			var list = new List<Model>();
			for (var i = 0; i < ds.Tables.Count; i++)
			{
				var _list = ds.Tables[i].ToList<Model>(mappings);
				foreach (var data in _list)
				{
					data.DbName = dbName;
					data.TableName = tableNames[i];
					list.Add(data);
				}
			}
			return list;
		}

		/// <summary>
		/// 跨库跨表查询
		/// </summary>
		/// <param name="dict">dbName-tableName-values</param>
		/// <param name="column">列名或对应的属性名</param>
		/// <returns></returns>
		protected ICollection<Model> ListByColumn<T>(IDictionary<string, IDictionary<string, ICollection<T>>> dict, string column)
		{
			var columnAttr = DbHelper.GetColumnAttribute<Model>(column);
			var columnName = columnAttr != null ? columnAttr.Name : column;
			var mappings = TableAttribute.ColumnAttributes.GetMappings();
			var list = new List<Model>();
			foreach (var kv in dict)
			{
				var dbName = kv.Key;
				var sqls = new StringBuilder();
				foreach (var _kv in kv.Value)
				{
					var tableName = _kv.Key;
					var values = _kv.Value;
					var sql_in = DbHelper.GenerateInSql(columnName, values.ToArray());
					sqls.AppendLine($"SELECT * FROM {tableName} WHERE {sql_in};");
				}
				var sql = sqls.ToString();
				var ds = DbContext.GetDataSet(dbName, sql);
				for (var i = 0; i < ds.Tables.Count; i++)
				{
					var _list = ds.Tables[i].ToList<Model>(mappings);
					foreach (var data in _list)
					{
						data.DbName = dbName;
						data.TableName = kv.Value.ElementAt(i).Key;
						list.Add(data);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected ICollection<Model> GetList(string sql, params IDbDataParameter[] parameters)
		{
			var list = DbContext.GetList<Model>(DbName, sql, parameters);
			foreach (var data in list)
			{
				data.DbName = DbName;
				data.TableName = TableName;
			}
			return list;
		}
	}
}
