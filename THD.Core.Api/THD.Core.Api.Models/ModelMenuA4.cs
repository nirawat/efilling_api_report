using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuA4_InterfaceData
    {
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public string editdatamessage { get; set; }
        public ModelMenuA4 editdata { get; set; }
    }
    public class ModelMenuA4
    {
        public string docid { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string conclusiondate { get; set; }
        public string file1name { get; set; }
        public string file1base64 { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }


    public class ModelMenuA4ProjectNumberData
    {
        public bool isprojectgroup { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string projectname1 { get; set; }
        public string projectname2 { get; set; }
        public string positionname { get; set; }
        public string dateofapproval { get; set; }
    }
}
