using MySql.Data.MySqlClient;
using System.Data;

namespace Adai.DbContext.MySql
{
	/// <summary>
	/// MySqlDbContext
	/// </summary>
	public sealed class MySqlDbContext : DbContext, IDbContext
	{
		/// <summary>
		/// 事件Id
		/// </summary>
		public string EventId { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId">事件Id</param>
		public MySqlDbContext(string eventId) : this(eventId, null)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="eventId">事件Id</param>
		/// <param name="connectionString"></param>
		public MySqlDbContext(string eventId, string connectionString) : base(Config.DbType.MySQL, connectionString)
		{
			EventId = eventId;
		}

		/// <summary>
		/// CreateConnection
		/// </summary>
		/// <returns></returns>
		public override IDbConnection CreateConnection()
		{
			return new MySqlConnection();
		}

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <returns></returns>
		public override IDbCommand CreateCommand()
		{
			return new MySqlCommand();
		}

		/// <summary>
		/// CreateDataAdapter
		/// </summary>
		/// <returns></returns>
		public override IDbDataAdapter CreateDataAdapter()
		{
			return new MySqlDataAdapter();
		}

		/// <summary>
		/// CreateParameter
		/// </summary>
		/// <returns></returns>
		public override IDbDataParameter CreateParameter()
		{
			return new MySqlParameter();
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
