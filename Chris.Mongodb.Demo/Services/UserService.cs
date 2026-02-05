using Chris.Mongodb.Demo.Entities;
using MongoDB.Driver;

namespace Chris.Mongodb.Demo.Services
{
    /// <summary>
    /// 用户服务（封装MongoDB CRUD）
    /// </summary>
    public class UserService
    {
        // 用户集合实例
        private readonly IMongoCollection<User> _userCollection;

        /// <summary>
        /// 构造函数：注入MongoDB上下文，获取用户集合
        /// </summary>
        /// <param name="mongoDbContext">MongoDB上下文</param>
        public UserService(MongoDbContext mongoDbContext)
        {
            // 获取"user"集合（不存在则插入数据时自动创建）
            _userCollection = mongoDbContext.GetCollection<User>("users");
        }

        #region 新增
        /// <summary>
        /// 添加单个用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>新增后的用户（包含自动生成的Id）</returns>
        public async Task<User> AddUserAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        /// <summary>
        /// 批量添加用户
        /// </summary>
        /// <param name="users">用户列表</param>
        public async Task AddUsersAsync(List<User> users)
        {
            await _userCollection.InsertManyAsync(users);
        }
        #endregion

        #region 查询
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            // 空过滤条件 = 查询所有
            var filter = Builders<User>.Filter.Empty;
            return await _userCollection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// 根据Id查询用户
        /// </summary>
        /// <param name="id">MongoDB的ObjectId字符串</param>
        /// <returns>单个用户（不存在则返回null）</returns>
        public async Task<User> GetUserByIdAsync(string id)
        {
            // 构建过滤条件：Id等于指定值
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 条件查询（示例：年龄大于指定值）
        /// </summary>
        /// <param name="minAge">最小年龄</param>
        /// <returns>符合条件的用户列表</returns>
        public async Task<List<User>> GetUsersByAgeAsync(int minAge)
        {
            // 构建过滤条件：Age > minAge
            var filter = Builders<User>.Filter.Gt(u => u.Age, minAge);
            // 可选：排序（按年龄降序）
            return await _userCollection.Find(filter)
                .SortByDescending(u => u.Age)
                .ToListAsync();
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新用户（根据Id）
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="updateUser">更新后的用户信息</param>
        /// <returns>是否更新成功</returns>
        public async Task<bool> UpdateUserAsync(string id, User updateUser)
        {
            // 构建过滤条件：Id等于指定值
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            // 构建更新操作：只更新指定字段（避免覆盖整个文档）
            var update = Builders<User>.Update
                .Set(u => u.Name, updateUser.Name)
                .Set(u => u.Age, updateUser.Age)
                .Set(u => u.Gender, updateUser.Gender)
                .Set(u => u.Hobby, updateUser.Hobby);

            // 执行更新（只更新匹配的第一条）
            var result = await _userCollection.UpdateOneAsync(filter, update);
            // MatchedCount > 0 表示有匹配的文档，更新成功
            return result.MatchedCount > 0;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据Id删除用户
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteUserAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var result = await _userCollection.DeleteOneAsync(filter);
            // DeletedCount > 0 表示删除成功
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// 批量删除（示例：删除指定年龄以下的用户）
        /// </summary>
        /// <param name="maxAge">最大年龄</param>
        /// <returns>删除的条数</returns>
        public async Task<long> DeleteUsersByAgeAsync(int maxAge)
        {
            var filter = Builders<User>.Filter.Lt(u => u.Age, maxAge);
            var result = await _userCollection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }
        #endregion
    }
}
