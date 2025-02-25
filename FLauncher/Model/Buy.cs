﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
namespace FLauncher.Model
{
    [Collection("Buy")]
    public class Buy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Represents the MongoDB _id
        [BsonElement("ID_Bill")]
        public string BillId { get; set; }

        [BsonElement("ID_Gamer")]
        public string GamerId { get; set; }

        [BsonElement("ID_Game")]
        public string GameId { get; set; }

        [BsonElement("Buy_time")]
        public string BuyTimeString { get; set; }  // Store as string in MongoDB

        [BsonIgnore]
        public DateTime BuyTime
        {
            get => DateTime.ParseExact(BuyTimeString, "yyyy-MM-dd HH:mm:ss", null);  // Convert string to DateTime
            set => BuyTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");  // Convert DateTime to string
        }

        [BsonElement("Buy_price")]
        public double BuyPrice { get; set; }
    }
}
