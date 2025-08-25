namespace ToDoListApi.Entities
{
    public class AssigmentPage
    {
        public long AssigmentId { get; set; }
        public long PageId { get; set; }
        public required Assignment Assignment { get; set; }
        public required Page Page { get; set; }

    }
}