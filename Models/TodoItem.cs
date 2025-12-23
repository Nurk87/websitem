using System.ComponentModel.DataAnnotations;

namespace MyTodoList.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        [Required]
        public string YapilacakIs { get; set; }

        public string Saat { get; set; } // "14:30" gibi saat bilgisi

        public bool IsCompleted { get; set; }

        public DateTime? TamamlanmaTarihi { get; set; } // Görev bittiğinde tarih atacağız

        public int UserId { get; set; }
    }
}