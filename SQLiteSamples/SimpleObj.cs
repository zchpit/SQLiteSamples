using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLiteSamples
{
    public class SimpleObj
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    [Table("highscores")]
    public class EfSimpleObj
    {
        [Key]
        public string Name { get; set; }
        public int Score { get; set; }
    }
}