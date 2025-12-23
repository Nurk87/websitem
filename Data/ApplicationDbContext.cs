using Microsoft.EntityFrameworkCore;
using MyTodoList.Models;

namespace MyTodoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> Todos { get; set; } // <--- BU SATIR EKLENMELİ
    }
}