using Chris.Mongodb.Demo.Entities;
using MongoDB.Driver;

namespace Chris.Mongodb.Demo.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(MongoDbContext mongoDbContext)
        {
            _userCollection = mongoDbContext.GetCollection<User>("users");
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        public async Task AddUsersAsync(List<User> users)
        {
            await _userCollection.InsertManyAsync(users);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var filter = Builders<User>.Filter.Empty;
            return await _userCollection.Find(filter).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersByAgeAsync(int minAge)
        {
            var filter = Builders<User>.Filter.Gt(u => u.Age, minAge);
            return await _userCollection.Find(filter)
                .SortByDescending(u => u.Age)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(string id, User updateUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.Name, updateUser.Name)
                .Set(u => u.Age, updateUser.Age)
                .Set(u => u.Gender, updateUser.Gender)
                .Set(u => u.Hobby, updateUser.Hobby);

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var result = await _userCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<long> DeleteUsersByAgeAsync(int maxAge)
        {
            var filter = Builders<User>.Filter.Lt(u => u.Age, maxAge);
            var result = await _userCollection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }
    }
}
