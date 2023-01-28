using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLEntryMVC.Entities
{
    public class UrlTbl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UrlLink { get; set; }
        public string? DomainLink { get; set; }
    }
}
