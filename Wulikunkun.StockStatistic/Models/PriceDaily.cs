using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wulikunkun.StockStatistic.Models
{
    class PriceDaily
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Ts_Code { get; set; }
        [MaxLength(50)]
        public string Trade_Date { get; set; }
        public decimal Open { get; set; }
        public decimal Hight { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Pre_Close { get; set; }
        public decimal Change { get; set; }
        public decimal Pct_Chg { get; set; }
        public decimal Vol { get; set; }
        public decimal Amount { get; set; }
    }
}
