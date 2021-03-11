using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ExceptionServices;

namespace ScoreCalculator.EF
{
    public class Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public TennisPlayer Winner { get; set; }
        public virtual ICollection<TennisSet> Sets { get; set; }
        public TennisPlayer FirstPlayer  { get; set; }
        public TennisPlayer SecondPlayer  { get; set; }
    }
}
