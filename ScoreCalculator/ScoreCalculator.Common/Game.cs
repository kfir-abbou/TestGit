using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;

namespace ScoreCalculator.EF
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("FirstPlayer")]
        public int FirstPlayer { get; set; }
        [ForeignKey("SecondPlayer")] 
        public int SecondPlayer { get; set; }
    }
}
