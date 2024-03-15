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

            DefineKeys(modelBuilder);

            // Explicitly define the precision and scale for the decimal property in the Goal entity
            modelBuilder.Entity<Goal>()
                .Property(goal => goal.Time)
                .HasPrecision(5, 2);

            DefineRelationships(modelBuilder);
        }

        private static void DefineRelationships(ModelBuilder modelBuilder)
        {
            // Player (many) -> Team (one)
            modelBuilder.Entity<Player>()
                .HasOne(player => player.Team)
                .WithMany(team => team.Players)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // LeagueTeam (many) -> Team (one)
            modelBuilder.Entity<LeagueTeam>()
                .HasOne(leagueTeam => leagueTeam.Team)
                .WithMany(team => team.LeagueTeams)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // LeagueTeam (many) -> League (one)
            modelBuilder.Entity<LeagueTeam>()
                .HasOne(leagueTeam => leagueTeam.League)
                .WithMany(league => league.LeagueTeams)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // Manager (one) -> Team (one)
            modelBuilder.Entity<Manager>()
                .HasOne(manager => manager.Team)
                .WithOne(team => team.Manager)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // TeamStadium (one) -> Team (one)
            modelBuilder.Entity<TeamStadium>()
                .HasOne(teamStadium => teamStadium.Team)
                .WithOne(team => team.TeamStadium)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // TeamStadium (many) -> Stadium (one)
            modelBuilder.Entity<TeamStadium>()
                .HasOne(teamStadium => teamStadium.Stadium)
                .WithMany(stadium => stadium.TeamStadiums)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // Match (many) -> Stadium (one)
            /*
             * The Database logic: If stadium is deleted, then the match is deleted
             * 
             * ***TODO***: The Business Logic: The stadium cannot be deleted, but instead, a property (i.e., "demolished" or "active") should be set to true
             */
            modelBuilder.Entity<Match>()
                .HasOne(match => match.Stadium)
                .WithMany(stadium => stadium.Matches)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // Goal (many) -> Match (one)
            modelBuilder.Entity<Goal>()
                .HasOne(goal => goal.Match)
                .WithMany(match => match.Goals)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // 2 foreign key relationships for Match (many) -> Team (one)
            /*
             * Set IsRequired to false & DeleteBehaviour to NoAction,
             * becase when dealing with relationships involving 2 foreign keys from a single entity,
             * in order to prevent an error related to "cycle".
             */
            modelBuilder.Entity<Match>()
                .HasOne(match => match.HomeTeam)
                .WithMany(homeTeam => homeTeam.HomeMatches)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Match>()
                .HasOne(match => match.AwayTeam)
                .WithMany(awayTeam => awayTeam.AwayMatches)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Goal (many) -> Player (one)
            modelBuilder.Entity<Goal>()
                .HasOne(goal => goal.Player)
                .WithMany(player => player.Goals)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            // Player (many) -> Position (one)
            modelBuilder.Entity<Player>()
                .HasOne(player => player.Position)
                .WithMany(pos => pos.Players)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private static void DefineKeys(ModelBuilder modelBuilder)
        {
            // Using Fluent API

            // Primary Key: Goal
            modelBuilder.Entity<Goal>()
                .HasKey(goal => goal.Id);

            // Primary Key: League
            modelBuilder.Entity<League>()
                .HasKey(league => league.Id);

            // Primary Key: LeagueTeam
            modelBuilder.Entity<LeagueTeam>()
                .HasKey(leagueTeam => leagueTeam.Id);

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

            // Alternate Key: Position
            modelBuilder.Entity<Position>()
                .HasAlternateKey(position => position.Name);

            // Primary Key: Stadium
            modelBuilder.Entity<Stadium>()
                .HasKey(stadium => stadium.Id);

            // Primary Key: Team
            modelBuilder.Entity<Team>()
                .HasKey(team => team.Id);

            // Primary Key: TeamStadium
            modelBuilder.Entity<TeamStadium>()
                .HasKey(teamStadium => teamStadium.Id);
        }
    }
}
