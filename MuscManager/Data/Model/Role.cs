﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicManager.Data.Model
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}