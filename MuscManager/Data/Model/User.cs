using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicManager.Data.Model
{
    public class User
    {
        public User()
        {
            Playlists = new List<Playlist>();
        }

        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsBlocked { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
