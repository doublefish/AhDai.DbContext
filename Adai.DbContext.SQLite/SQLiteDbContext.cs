using System.Data;
using System.Data.SQLite;

namespace Adai.DbContext.SQLite
{
	/// <summary>
	/// SQLiteDbContext
	/// </summary>
	public class SQLiteDbContext : DbContext, IDbContext
	{
		/// <summary>
		/// 事件Id
		/// </summary>
		public string EventId { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId"></param>
		public SQLiteDbContext(string eventId) : this(eventId, null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId">事件Id</param>
		/// <param name="connectionString"></param>
		public SQLiteDbContext(string eventId, string connectionString) : base(Config.DbType.MySQL, connectionString)
		{
			EventId = eventId;
		}

		/// <summary>
		/// CreateConnection
		/// </summary>
		/// <returns></returns>
		public override IDbConnection CreateConnection()
		{
			return new SQLiteConnection();
		}

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <returns></returns>
		public override IDbCommand CreateCommand()
		{
			return new SQLiteCommand();
		}

		/// <summary>
		/// CreateDataAdapter
		/// </summary>
		/// <returns></returns>
		public override IDbDataAdapter CreateDataAdapter()
		{
			return new SQLiteDataAdapter();
		}

		/// <summary>
		/// CreateParameter
		/// </summary>
		/// <returns></returns>
		public override IDbDataParameter CreateParameter()
		{
			return new SQLiteParameter();
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
