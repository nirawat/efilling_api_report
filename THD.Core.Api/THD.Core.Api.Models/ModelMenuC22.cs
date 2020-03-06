using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC22_InterfaceData
    {
        public IList<ModelSelectOption> ListAssigner { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public string default_assigner_name { get; set; }
        public string default_assigner_seq { get; set; }
        public IList<ModelSelectOption> ListSafetyType { get; set; }
        public IList<ModelSelectOption> ListApprovalType { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelMenuC22
    {
        public string docid { get; set; }
        public string assignercode { get; set; }
        public string assignername { get; set; }
        public string positionname { get; set; }
        public string assignerseq { get; set; }
        public string projectnumber { get; set; }
        public string labtypename { get; set; }
        public string facultyname { get; set; }
        public string safetytype { get; set; }
        public string approvaltype { get; set; }
        public string commentconsider { get; set; }
    }

    public class ModelMenuC22Data
    {
        public string registerid { get; set; }
        public string fullname { get; set; }
        public string positionname { get; set; }
        public string labtypename { get; set; }
        public string facultyname { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
    }



}
