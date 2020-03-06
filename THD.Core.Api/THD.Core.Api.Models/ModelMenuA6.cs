using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuA6_InterfaceData
    {
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public string editdatamessage { get; set; }
        public ModelMenuA6 editdata { get; set; }
    }
    public class ModelMenuA6
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string positionnamethai { get; set; }
        public string facultyname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string accepttypenamethai { get; set; }
        public string conclusiondate { get; set; }
        public int renewround { get; set; }
        public string file1name { get; set; }
        public string file1base64 { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }
    public class ModelMenuA6ProjectNumberData
    {
        public bool isprojectgroup { get; set; }
        public string projectheadname { get; set; }
        public string positionname { get; set; }
        public string facultyname { get; set; }
        public string projectname1 { get; set; }
        public string projectname2 { get; set; }
        public string certificatetype { get; set; }
        public string dateofapproval { get; set; }
    }
}
