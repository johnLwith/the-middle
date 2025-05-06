using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using Microsoft.Extensions.VectorData;

namespace WebAppp.Models
{
    [Table("episodes_embeddings")]
    public class EpisodeEmbedding
    {
        [Key]
        [Column("id")]
        [VectorStoreRecordKey(StoragePropertyName = "id")]
        public int Id { get; set; }

        [Column("episodes_id")]
        [VectorStoreRecordData(StoragePropertyName = "episodes_id")]
        public string EpisodeId { get; set; }

        [Column("content")]
        [VectorStoreRecordData(StoragePropertyName = "content")]
        public string Content { get; set; }

        [Column("embedding")]
        [VectorStoreRecordVector(Dimensions: 768, DistanceFunction.CosineDistance, StoragePropertyName = "embedding")]
        public ReadOnlyMemory<float> Embedding { get; set; }

        // Navigation property
        [ForeignKey("EpisodeId")]
        public Episode Episode { get; set; }
    }
}