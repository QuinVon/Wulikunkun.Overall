using System;
using System.ComponentModel.DataAnnotations;

namespace Wulikunkun.Web.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime LogTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}