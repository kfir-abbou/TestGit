using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreManagerService.Server
{
    public static class Utils
    {
        public static string ScoreToStringConverter(int p_score)
        {
            return p_score switch
            {
                0 => "LOVE",
                1 => "15",
                2 => "30",
                3 => "40",
                4 => "Head",
                5 => "2Head",
                _ => "0"
            };
        }
    }
}
