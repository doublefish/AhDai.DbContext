// See https://aka.ms/new-console-template for more information


using Adai.DbContext;
using Adai.DbContext.MySql;
using Adai.DbContext.Test;
using System.Data;

Console.WriteLine("Hello, World!");

var type = typeof(Program);
Console.WriteLine(type.FullName);

Class1.InitDbConfig();

var eventId = Guid.NewGuid().ToString();
var dbContext = new MySqlDbContext(eventId);

var sql = $"select * from User";
var list = dbContext.GetList<Adai.DbContext.Test.Models.User>("db", sql);

Console.WriteLine(list.Count);

TestDbTrans();


static void TestDbTrans()
{
	var eventId = Guid.NewGuid().ToString("N");
	var dbContext = new MySqlDbContext(eventId);

	var sql = $"INSERT INTO `User` (Username,Nickname,Roles,SecretKey,Status,CreateTime) VALUES ('test02','test02','','',0,'{DateTime.Now:yyyy-MM-dd HH:mm:ss}');";
	var sql0 = $"INSERT INTO `User` (Username,Nickname,Roles,SecretKey,Status,CreateTime) VALUES ('test02','test02','',NULL,0,'{DateTime.Now:yyyy-MM-dd HH:mm:ss}');";

	var cmds = new List<IDbCommand>() {
		dbContext.CreateCommand(sql),
		//dbContext.CreateCommand(sql0)
	};

	var cmds0 = new List<IDbCommand>() {
		//dbContext.CreateCommand(sql),
		dbContext.CreateCommand(sql0)
	};

	var dictCmds = new Dictionary<string, ICollection<IDbCommand>>() {
		{ "db", cmds },
		{ "db", cmds0 }
	};

	dbContext.ExecuteNonQuery(dictCmds);
}