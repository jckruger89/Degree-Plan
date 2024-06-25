using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegreePlan.Models
{
    [Table("Term")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string? Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string DateRange => $"{StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";

        [Ignore]
        public List<Course>? Courses { get; set; }
    }
}