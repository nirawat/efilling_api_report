using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{

    #region Menu C1

    public class ModelMenuC1_InterfaceData
    {
        public IList<ModelSelectOption> ListAssigner { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public IList<ModelSelectOption> ListBoard { get; set; }
        public IList<ModelSelectOption> ListSpecialList { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public int defaultround { get; set; }
        public int defaultyear { get; set; }
        public string default_assigner_name { get; set; }
        public ModelMenuC1 editdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuC1
    {
        public string docid { get; set; }
        public string assignercode { get; set; }
        public string assignername { get; set; }
        public string positionname { get; set; }
        public string accepttype { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public string meetingdate { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
        public IList<ModelSelectOption> boardcodearray { get; set; }
        public IList<ModelSelectOption> speciallistcodearray { get; set; }
    }

    public class ModelMenuC1Data
    {
        public string positionname { get; set; }
        public string projectheadname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string facultyname { get; set; }
    }

    public class ModelRegisterData
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

    public class ModelCountOfYear
    {
        public int count { get; set; }
    }

    #endregion

}
