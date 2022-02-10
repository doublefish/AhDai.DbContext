using Adai.DbContext;
using System.Data;

namespace Adai.DbContext.Test
{
	internal class Class1
	{
		public static void InitDbConfig()
		{
			// 读取数据库配置
			var dbConfigs = new List<Adai.DbContext.Models.DbConfig>()
			{
				new Adai.DbContext.Models.DbConfig() {
					DbType = Config.DbType.MySQL,
					Name = "db",
					ConnectionString = "server=127.0.0.1;port=3306;database=Basic;user=root;password=mysql@123456;"
				},
			};
			DbHelper.Init(dbConfigs, BeforeExecute);
		}

		/// <summary>
		/// 执行前执行
		/// </summary>
		/// <param name="eventId"></param>
		/// <param name="command"></param>
		static void BeforeExecute(string eventId, IDbCommand command)
		{
			var message = $"记录SQL=>{command.CommandText};Paras=>";
			foreach (IDbDataParameter para in command.Parameters)
			{
				message += $"{para.ParameterName}={para.Value},";
			}
			Console.WriteLine($"[{eventId}]{message}");
		}
	}
}
