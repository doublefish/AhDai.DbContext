# 数据库访问基础类库

# 初始化
建议在程序启动时调用初始化数据库连接信息方法
建议引用命名空间，因为AhDai.DbContext有许多扩展方法在命名空间AhDai.DbContext下

方式1：适用于AhDai.DbContext.数据类型的包，例如：AhDai.DbContext.MySql

AhDai.DbContext.DbHelper.Init(ICollection<Models.DbConfig> dbConfigs, Action<string, IDbCommand> beforeExecute = null);

参数说明：

dbConfigs 数据库配置

beforeExecute 执行之前执行，可用于记录SQL，第一个参数是初始化时传入的EventId
