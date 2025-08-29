namespace ToDoListApi.Models
{
    public class AssignmentDTO
    {
        public long Id { get; set; }
        public required string Text { get; set; }
        public required long StatusId { get; set; }
        public required long PageId { get; set; }
        public required DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; }
        public DateTime? CompletionDeadline { get; set; }
    }
}
