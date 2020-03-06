using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC12_InterfaceData
    {
        public IList<ModelSelectOption> ListAssigner { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public IList<ModelSelectOption> ListBoard { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public int defaultround { get; set; }
        public int defaultyear { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelMenuC12
    {
        public string docid { get; set; }
        public string assignercode { get; set; }
        public string positionname { get; set; }
        public string accepttype { get; set; }
        public string projectnumber { get; set; }
        public string labtypename { get; set; }
        public string facultyname { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public string meetingdate { get; set; }
        public IList<ModelSelectOption> boardcodearray { get; set; }
        public IList<ModelSelectOption> speciallistcodearray { get; set; }
    }

    public class ModelMenuC12Data
    {
        public string labtypename { get; set; }
        public string facultyname { get; set; }
    }

    public class ModelRegisterDataC12
    {
        public string registerid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string fullname { get; set; }
        public string faculty { get; set; }
        public string facultyname { get; set; }
        public string position { get; set; }
        public string positionname { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
    }


}
