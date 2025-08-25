using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Entities
{
    public class Page
    {
        public long Id { get; set; }
        [Required]
        public required string PageName { get; set; }
        public DateTime CreationDate { get; set; }
        public long UserId { get; set; }
        public required User Author { get; set; }
        public List<AssigmentPage> AssigmentsPages { get; set; }
    }
}
