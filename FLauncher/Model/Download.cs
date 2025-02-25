﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
namespace FLauncher.Model
{
    [Collection("Download")]
    public class Download
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("Gamer_id")] // Matches the JSON key "Gamer_id"
        public string GamerId { get; set; }

    [BsonElement("Game_id")] // Matches the JSON key "Game_id"
    public string GameId { get; set; }

        [BsonElement("TimeDownload")]
        public string TimeDownloadString { get; set; } // Stores the date as a string in MongoDB

        [BsonIgnore]
        public DateTime TimeDownload
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeDownloadString))
                    throw new InvalidOperationException("TimeDownloadString is null or empty.");

                return DateTime.ParseExact(TimeDownloadString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            set
            {
                TimeDownloadString = value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        [BsonElement("Storage")] // Matches the JSON key "Storage"
    public string Storage { get; set; }

    [BsonElement("Directory")] // Matches the JSON key "Directory"
    public string Directory { get; set; }

    }
}
