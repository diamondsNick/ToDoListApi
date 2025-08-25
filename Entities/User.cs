using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ToDoListApi.Entities
{
    public class User
    {
        public long Id { get; set; }
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
        [AllowNull]
        public List<Page> Pages { get; set; }
    }
}
