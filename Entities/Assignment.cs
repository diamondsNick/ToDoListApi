using System.Diagnostics.CodeAnalysis;

namespace ToDoListApi.Entities
{
    public class Assignment
    {
        public long Id { get; set; }
        public required string Text { get; set; }
        public required long StatusId { get; set; }
        public required DateTime CreationDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime CompletionDeadline { get; set; }
        public required Status CurStatus { get; set; }
        public List<AssigmentPage> AssigmentsPages { get; set; }
    }
}
