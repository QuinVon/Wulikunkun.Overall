using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wulikunkun.StockStatistic.Models
{
    [Table("exchangecalendar")]
    class ExchangeCalendar
    {
        public int Id { get; set; }
        [MaxLength(256)]
        public string Exchange { get; set; }

        public string Cal_Date { get; set; }

        public bool Is_Open { get; set; }
    }
}
