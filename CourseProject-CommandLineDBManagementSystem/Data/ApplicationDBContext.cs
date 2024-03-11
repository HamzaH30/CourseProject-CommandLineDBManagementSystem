using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseProject_CommandLineDBManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject_CommandLineDBManagementSystem.Data
{
    internal class ApplicationDBContext : DbContext
    {
        public virtual DbSet<Goal> Goals { get; set; }
        public virtual DbSet<League> Leagues { get; set; }
        public virtual DbSet<LeagueTeam> LeagueTeams { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Stadium> Stadiums { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamStadium> TeamStadiums { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = "Data Source=localhost\\SQLEXPRESS;database=SoccerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Using Fluent API

            // Primary Key: Goal
            modelBuilder.Entity<Goal>()
                .HasKey(goal => goal.Id);

            // Primary Key: League
            modelBuilder.Entity<League>()
                .HasKey(league => league.Id);

            // Primary Key: LeagueTeam
            modelBuilder.Entity<LeagueTeam>()
                .HasKey(leagueTeam => new { leagueTeam.LeagueId, leagueTeam.TeamId });

            // Primary Key: Manager
            modelBuilder.Entity<Manager>()
                .HasKey(manager => manager.Id);

            // Primary Key: Match
            modelBuilder.Entity<Match>()
                .HasKey(match => match.Id);

            // Primary Key: Player
            modelBuilder.Entity<Player>()
                .HasKey(player => player.Id);

            // Primary Key: Position
            modelBuilder.Entity<Position>()
                .HasKey(position => position.Id);

            // Primary Key: Stadium
            modelBuilder.Entity<Stadium>()
                .HasKey(stadium => stadium.Id);

            // Primary Key: Team
            modelBuilder.Entity<Team>()
                .HasKey(team => team.Id);

            // Primary Key: TeamStadium
            modelBuilder.Entity<TeamStadium>()
                .HasKey(teamStadium => new {teamStadium.StadiumId, teamStadium.TeamId });
        }
    }
}
