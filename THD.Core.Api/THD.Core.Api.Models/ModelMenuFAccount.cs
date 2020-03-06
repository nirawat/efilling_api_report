using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuFAccount_InterfaceData
    {

        public IList<ModelSelectOption> listfirstname { get; set; }
        public IList<ModelSelectOption> listfaculty { get; set; }
        public ModelMenuAccountUser account { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuAccountUser
    {
        public string registerid { get; set; }
        public string userid { get; set; }
        public string firstname1 { get; set; }
        public string firstname2 { get; set; }
        public string firstname { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public string note1 { get; set; }
        public string note2 { get; set; }
        public string note3 { get; set; }
        public string education { get; set; }
        public string educationname { get; set; }
        public string character { get; set; }
        public string charactername { get; set; }
        public string position { get; set; }
        public string positionname { get; set; }
        public string faculty { get; set; }
        public string facultyname { get; set; }
        public string registerdate { get; set; }
        public string registerexpire { get; set; }
        public string isactive { get; set; }
        public bool editenable { get; set; }

    }
}
