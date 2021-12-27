using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MusicManager.Data.Model;
using MusicManager.Data.Model.ManyToManyRelationModels;

namespace MusicManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistTrack>().HasKey(pt => new {pt.PlaylistId, pt.TrackId});
            modelBuilder.Entity<TrackGenre>().HasKey(pt => new {pt.GenreId, pt.TrackId});

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(a => a.Tracks)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Author)
                .WithMany(a => a.Tracks)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Track>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Album>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Author>()
                .HasIndex(u => u.Name)
                .IsUnique();


            modelBuilder.Entity<Playlist>()
                .HasIndex(u => new {u.Name, u.OwnerId})
                .IsUnique();

            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminLogin = "admin";
            string adminPassword = "123";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Login = adminLogin, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        //many-to-many relations
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<TrackGenre> TrackGenres { get; set; }
    }
}   
