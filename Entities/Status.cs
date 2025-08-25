using ToDoListApi.Models;

namespace ToDoListApi.Entities
{
    public class Status
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public List<Entities.Assignment> Assignments { get; set; }
    }
}
