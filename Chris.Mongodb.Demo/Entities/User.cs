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
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [BsonElement("name")] // 自定义MongoDB中的字段名（可选，默认与属性名一致）
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 爱好（数组类型，对应MongoDB的数组）
        /// </summary>
        public List<string> Hobby { get; set; }

        /// <summary>
        /// 创建时间（自动赋值）
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] // 处理时区问题
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
