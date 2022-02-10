using Adai.DbContext.Extensions;
using System.Data;

namespace Adai.DbContext
{
	/// <summary>
	/// DbContext
	/// </summary>
	public abstract class DbContext : IDbContext
	{
		/// <summary>
		/// 数据库类型
		/// </summary>
		public Config.DbType DbType { get; set; }

		/// <summary>
		/// 数据库连接
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbType"></param>
		public DbContext(Config.DbType dbType) : this(dbType, null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="connectionString"></param>
		public DbContext(Config.DbType dbType, string connectionString)
		{
			DbType = dbType;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// CreateConnection
		/// </summary>
		/// <returns></returns>
		public abstract IDbConnection CreateConnection();

		/// <summary>
		/// CreateConnection
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public virtual IDbConnection CreateConnection(string connectionString)
		{
			var conn = CreateConnection();
			conn.ConnectionString = connectionString;
			return conn;
		}

		/// <summary>
		/// CreateDataAdapter
		/// </summary>
		/// <returns></returns>
		public abstract IDbDataAdapter CreateDataAdapter();

		/// <summary>
		/// CreateParameter
		/// </summary>
		/// <returns></returns>
		public abstract IDbDataParameter CreateParameter();

		/// <summary>
		/// CreateParameter
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual IDbDataParameter CreateParameter(string name, object value)
		{
			var para = CreateParameter();
			para.ParameterName = name;
			para.Value = value;
			return para;
		}

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <returns></returns>
		public abstract IDbCommand CreateCommand();

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public virtual IDbCommand CreateCommand(string sql, params IDbDataParameter[] parameters)
		{
			var cmd = CreateCommand();
			cmd.CommandText = sql;
			if (parameters?.Length > 0)
			{
				cmd.Parameters.AddRange(parameters);
			}
			return cmd;
		}

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public virtual IDbCommand CreateCommand(IDbConnection connection, string sql, params IDbDataParameter[] parameters)
		{
			var cmd = connection.CreateCommand();
			cmd.CommandText = sql;
			if (parameters?.Length > 0)
			{
				cmd.Parameters.AddRange(parameters);
			}
			return cmd;
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public virtual DataSet GetDataSet(string sql, params IDbDataParameter[] parameters)
		{
			var conn = CreateConnection(ConnectionString);
			try
			{
				conn.Open();
				var adapter = CreateDataAdapter();
				adapter.SelectCommand = CreateCommand(conn, sql, parameters);
				BeforeExecute(adapter.SelectCommand);
				var ds = new DataSet();
				adapter.Fill(ds);
				return ds;
			}
			finally
			{
				conn.Dispose();
			}
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object ExecuteScalar(string sql, params IDbDataParameter[] parameters)
		{
			var conn = CreateConnection(ConnectionString);
			try
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = sql;
				if (parameters?.Length > 0)
				{
					cmd.Parameters.AddRange(parameters);
				}
				BeforeExecute(cmd);
				return cmd.ExecuteScalar();
			}
			finally
			{
				conn.Dispose();
			}
		}

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public virtual int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters)
		{
			var cmd = CreateCommand(sql, parameters);
			return ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public virtual int ExecuteNonQuery(IDbCommand command)
		{
			var conn = CreateConnection(ConnectionString);
			try
			{
				conn.Open();
				command.Connection = conn;
				BeforeExecute(command);
				return command.ExecuteNonQuery();
			}
			finally
			{
				conn.Dispose();
			}
		}

		/// <summary>
		/// 批量执行
		/// </summary>
		/// <param name="commands"></param>
		/// <returns></returns>
		public int ExecuteNonQuery(IDbCommand[] commands)
		{
			var result = 0;
			var conn = CreateConnection(ConnectionString);
			IDbTransaction tran = null;
			try
			{
				conn.Open();
				tran = conn.BeginTransaction();
				foreach (var command in commands)
				{
					command.Connection = conn;
					command.Transaction = tran;
					BeforeExecute(command);
					result += command.ExecuteNonQuery();
				}
				tran.Commit();
			}
			catch
			{
				if (tran != null)
				{
					tran.Rollback();
				}
				throw;
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
				{
					conn.Close();
				}
			}
			return result;
		}

		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="command"></param>
		public abstract void BeforeExecute(IDbCommand command);
	}
}
