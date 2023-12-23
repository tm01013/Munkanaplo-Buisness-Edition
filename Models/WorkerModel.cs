using System;
using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
    public class WorkerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string User { get; set; }

        public int MoneyPerHour { get; set; }
    }
}

