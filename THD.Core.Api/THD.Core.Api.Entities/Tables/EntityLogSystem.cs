using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace THD.Core.Api.Entities.Tables
{
    [Table("LogSystem")]
    public class EntityLogSystem
    {
        public string register_id { get; set; }
        public string userid { get; set; }
        public string passw { get; set; }
        public string log_event { get; set; }
        public DateTime log_date { get; set; }
    }
}
