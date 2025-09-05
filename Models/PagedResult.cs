namespace ToDoListApi.Models
{
    public class PagedResult<T>
    {
        public int TotalAmount { get; set; }
        public int PageAmount { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<T>? Items { get; set; }
    }
}
