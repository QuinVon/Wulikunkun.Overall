using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wulikunkun.StockStatistic.Models
{
    [Table("stockbasicinfo")]
    public class StockBasicInfo
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Ts_Code { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string Area { get; set; }
        [MaxLength(256)]
        public string Industry { get; set; }

        [MaxLength(256)]
        public string List_Date { get; set; }
    }
}
