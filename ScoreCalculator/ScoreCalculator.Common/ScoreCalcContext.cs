using Microsoft.EntityFrameworkCore;


namespace ScoreCalculator.EF
{
    public sealed class ScoreCalcContext : DbContext, IScoreCalcContext
    {
        public ScoreCalcContext(DbContextOptions<ScoreCalcContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TennisPlayer> Players { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameScores> GameScores { get; set; }
        public DbSet<TennisSet> TennisSet { get; set; }
        public DbSet<Match> Match { get; set; }
        public void SaveData()
        {
            this.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=L-P-KFIRABB-WWN\SQLEXPRESS;Database=TennisScoreDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TennisPlayer>()
                .HasKey(p => new { p.Id });
            modelBuilder.Entity<GameScores>()
                .HasKey(p => new { p.Id });
            modelBuilder.Entity<Game>()
                .HasKey(p => new { p.Id });
            modelBuilder.Entity<TennisSet>()
                .HasKey(p => new { p.Id });
            modelBuilder.Entity<Match>()
                .HasKey(p => new { p.Id });
        }
    }

    public interface IScoreCalcContext
    {
        DbSet<TennisPlayer> Players { get; }
        DbSet<Game> Game { get; }
        DbSet<GameScores> GameScores { get; }
        DbSet<TennisSet> TennisSet { get; }
        DbSet<Match> Match { get; }

        void SaveData();

    }
}

