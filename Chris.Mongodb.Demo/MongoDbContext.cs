using MongoDB.Driver;

namespace Chris.Mongodb.Demo
{
    /// <summary>
    /// MongoDB上下文（封装客户端和数据库连接）
    /// </summary>
    public class MongoDbContext
    {
        // MongoDB数据库实例
        private readonly IMongoDatabase _database;

        /// <summary>
        /// 构造函数：从配置读取连接信息，初始化客户端和数据库
        /// </summary>
        /// <param name="configuration">配置对象（ASP.NET Core内置）</param>
        public MongoDbContext(IConfiguration configuration)
        {
            // 1. 读取配置
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            // 2. 创建Mongo客户端（全局单例，不要频繁创建）
            var client = new MongoClient(connectionString);

            // 3. 获取数据库实例（不存在则插入数据时自动创建）
            _database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// 获取指定集合（泛型方法，适配不同实体）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="collectionName">集合名（对应MongoDB的Collection）</param>
        /// <returns>集合操作实例</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
