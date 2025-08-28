using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Models
{
    public class AssignmentPageDTO
    {
        [Required]
        public long AssignmentId { get; set; }
        [Required]
        public long PageId { get; set; }
    }
}
