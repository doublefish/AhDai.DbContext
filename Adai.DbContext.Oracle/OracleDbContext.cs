using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Adai.DbContext.Oracle
{
	/// <summary>
	/// SqlDbContext
	/// </summary>
	public sealed class SqlDbContext : DbContext, IDbContext
	{
		/// <summary>
		/// 事件Id
		/// </summary>
		public string EventId { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId">事件Id</param>
		public SqlDbContext(string eventId) : this(eventId, null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId">事件Id</param>
		/// <param name="connectionString"></param>
		public SqlDbContext(string eventId, string connectionString) : base(Config.DbType.Oracle, connectionString)
		{
			EventId = eventId;
		}

		/// <summary>
		/// CreateConnection
		/// </summary>
		/// <returns></returns>
		public override IDbConnection CreateConnection()
		{
			return new OracleConnection();
		}

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <returns></returns>
		public override IDbCommand CreateCommand()
		{
			return new OracleCommand();
		}

		/// <summary>
		/// CreateDataAdapter
		/// </summary>
		/// <returns></returns>
		public override IDbDataAdapter CreateDataAdapter()
		{
			return new OracleDataAdapter();
		}

		/// <summary>
		/// CreateParameter
		/// </summary>
		/// <returns></returns>
		public override IDbDataParameter CreateParameter()
		{
			return new OracleParameter();
		}

		/// <summary>
		/// 执行之前
		/// </summary>
		/// <param name="command"></param>
		public override void BeforeExecute(IDbCommand command)
		{
			DbHelper.BeforeExecuteAction?.Invoke(EventId, command);
		}
	}
}
