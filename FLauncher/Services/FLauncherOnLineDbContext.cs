﻿using FLauncher.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Services
{
    public class FLauncherOnLineDbContext : DbContext
    {
        private static DbContextOptions<FLauncherOnLineDbContext> _cachedOptions;

        public FLauncherOnLineDbContext()
        {
        }

        public FLauncherOnLineDbContext(DbContextOptions<FLauncherOnLineDbContext> options)
            : base(options)
        {
        }

        public static FLauncherOnLineDbContext Create(IMongoDatabase database)
        {
            if (_cachedOptions == null)
            {
                _cachedOptions = new DbContextOptionsBuilder<FLauncherOnLineDbContext>()
                    .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                    .Options;
            }
            return new FLauncherOnLineDbContext(_cachedOptions);
        }
        public DbSet<TrackingRecords> TrackingsTime { get; init; }
        public DbSet<TrackingPlayers> TrackingPlayers { get; init; }
        public DbSet<Achivement> Achivements { get; init; }
        public DbSet<UnlockAchivement> UnlockAchivements { get; init; }
        public DbSet<Buy> Bills { get; init; }
        public DbSet<GamePublisher> GamePublishers { get; init; }
        public DbSet<Game> Games { get; init; }
        public DbSet<Gamer> Gamers { get; init; }
        public DbSet<Message> Messages { get; init; }
        public DbSet<User> Users { get; init; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrackingRecords>();

            modelBuilder.Entity<TrackingPlayers>();
            modelBuilder.Entity<Achivement>();
            modelBuilder.Entity<UnlockAchivement>();
            modelBuilder.Entity<Buy>();

            modelBuilder.Entity<GamePublisher>();

            modelBuilder.Entity<Game>();

            modelBuilder.Entity<Gamer>();
            modelBuilder.Entity<User>();
            
                 modelBuilder.Entity<Message>();

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        }

    }
}
