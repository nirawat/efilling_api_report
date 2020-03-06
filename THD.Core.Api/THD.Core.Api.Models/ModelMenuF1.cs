using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuF1_InterfaceData
    {
        public string searchkey { get; set; }
        public IList<ModelMenuF1Report> listdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuF1Report
    {
        public string registerid { get; set; }
        public string userid { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public string educationname { get; set; }
        public string charactername { get; set; }
        public string positionname { get; set; }
        public string facultyname { get; set; }
        public string registerdate { get; set; }
        public string registerexpire { get; set; }
        public string isactive { get; set; }

    }

}
