namespace ToDoListApi.Models
{
    public class PagedResult<T>
    {
        public int TotalAmount { get; set; }
        public List<T> Items { get; set; }
    }
}
