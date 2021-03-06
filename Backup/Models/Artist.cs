﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgainArt.Models
{
    [Table("TbArtist")]
    public class Artist
    {
        [Key]
        [Required]
        public int Id
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Nome
        {
            get;
            set;
   
        }
        public Artist()
        {
            
    }
    }
}
