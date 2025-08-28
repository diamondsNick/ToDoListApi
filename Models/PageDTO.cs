using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Models
{
    public class PageDTO
    {
        public long Id { get; set; }
        [Required]
        public required string PageName { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        [Required]
        public long UserId { get; set; }
    }
}
