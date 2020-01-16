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
        public string FullName { get; set; }
        [MaxLength(256)]
        public string EnName { get; set; }
        [MaxLength(256)]
        public string Market { get; set; }
        [MaxLength(256)]
        public string Exchange { get; set; }
        [MaxLength(256)]
        public string Curr_Type { get; set; }
        [MaxLength(256)]
        public string List_Status { get; set; }
        [MaxLength(256)]
        public string List_Date { get; set; }
        [MaxLength(256)]
        public string Delist_Date { get; set; }
        public bool Is_Hs { get; set; }
    }
}
