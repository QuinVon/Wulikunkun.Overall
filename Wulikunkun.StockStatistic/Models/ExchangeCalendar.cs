using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockStatistics.Models
{
    [Table("exchangecalendar")]
    class ExchangeCalendar
    {
        public int Id { get; set; }
        public string Exchange { get; set; }

        public DateTime Cal_Date { get; set; }

        public bool Is_Open { get; set; }
    }
}
