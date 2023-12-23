using System;
using System.ComponentModel.DataAnnotations;

namespace Munkanaplo2.Models
{
    public class WorkModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public string StartTime { get; set; }

        public string EndTiem { get; set; }

        [Required]
        public bool isFinished { get; set; }

        [Required]
        public string User { get; set; }

    }
}

