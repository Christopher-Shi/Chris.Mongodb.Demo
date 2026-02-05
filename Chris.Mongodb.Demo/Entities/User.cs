using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chris.Mongodb.Demo.Entities
{
    public class User
    {
        /// <summary>
        /// MongoDB默认主键（ObjectId类型）
        /// [BsonId] 标记为主键；[BsonRepresentation] 适配JSON序列化时的类型
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("name")] 
        public required string Name { get; set; }

        public int Age { get; set; }

        public required string Gender { get; set; }

        public List<string>? Hobby { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
