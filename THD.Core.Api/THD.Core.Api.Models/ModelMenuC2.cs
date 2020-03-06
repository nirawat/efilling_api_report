using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC2_InterfaceData
    {
        public IList<ModelSelectOption> ListAssigner { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public string default_assigner_name { get; set; }
        public string default_assigner_seq { get; set; }
        public IList<ModelSelectOption> ListSafetyType { get; set; }
        public IList<ModelSelectOption> ListApprovalType { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public int defaultround { get; set; }
        public int defaultyear { get; set; }

        public ModelMenuC2 editdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelMenuC2
    {
        public string docid { get; set; }
        public string assignercode { get; set; }
        public string assignername { get; set; }
        public string positionname { get; set; }
        public string assignerseq { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string safetytype { get; set; }
        public string safetytypename { get; set; }
        public string approvaltype { get; set; }
        public string approvaltypename { get; set; }
        public string commentconsider { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }

    public class ModelMenuC2Data
    {
        public string registerid { get; set; }
        public string fullname { get; set; }
        public string positionname { get; set; }
        public string projectheadname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string facultyname { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
    }



}
