using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ScoreCalculator.EF
{
    public class GameScores
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Game")] 
        public int GameId { get; set; }
        
        [ForeignKey("Player")]
        public int ScoringPlayerId { get; set; }
    }
}
