using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace THD.Core.Api.Entities.Tables
{
    [Table("RegisterUser")]
    public class EntityRegisterUser
    {
        [Key]
        public string email { get; set; }
        public string userid { get; set; }
        public string register_id { get; set; }
        public string passw { get; set; }
        public string confirmpassw { get; set; }
        public DateTime register_date { get; set; }
        public DateTime register_expire { get; set; }
        public string first_name_1 { get; set; }
        public string first_name_2 { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string position { get; set; }
        public string work_phone { get; set; }
        public string faculty { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public string education { get; set; }
        public string character { get; set; }
        public string note1 { get; set; }
        public string note2 { get; set; }
        public string note3 { get; set; }
        public DateTime? confirm_date { get; set; }
        public DateTime? member_expire { get; set; }
        public bool isactive { get; set; }
    }
}
