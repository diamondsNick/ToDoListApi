using System.ComponentModel.DataAnnotations;
using ToDoListApi.Entities;

namespace ToDoListApi.Models
{
    public class StatusDTO 
    {
        public long Id { get; set; }
        [MinLength(3)]
        public required string Name { get; set; }
    }
}
