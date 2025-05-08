using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppp.Models
{
    [Table("words")]
    public class Word
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("word")]
        public string WordText { get; set; }

        [Column("episode_id")]
        public string EpisodeId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}