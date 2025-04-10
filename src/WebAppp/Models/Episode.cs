using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppp.Models
{
    [Table("episodes")]
    public class Episode
    {
        [Key]
        [Column("id")]
        [StringLength(20)]
        public string? Id { get; set; }

        [Required]
        [Column("season_number")]
        public int SeasonNumber { get; set; }

        [Required]
        [Column("episode_number")]
        public int EpisodeNumber { get; set; }

        [Column("title")]
        [StringLength(255)]
        public string? Title { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("subtitle_path")]
        [StringLength(255)]
        public string? SubtitlePath { get; set; }

        [Column("audio_path")]
        [StringLength(255)]
        public string? AudioPath { get; set; }
    }
} 