using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuD1_InterfaceData
    {
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public string editdatamessage { get; set; }
        public ModelMenuD1 editdata { get; set; }
    }

    public class ModelMenuD1
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string advisorsnamethai { get; set; }
        public string acceptprojectno { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string accepttypenamethai { get; set; }
        public int acceptresult { get; set; }
        public string acceptresultname { get; set; }
        public int acceptcondition { get; set; }
        public string acceptconditionname { get; set; }
        public string acceptdate { get; set; }

        public string createby { get; set; }
        public bool editenable { get; set; }

        public IList<ModelMenuD1RenewTable> listRenewDate { get; set; }
    }

    public class ModelMenuD1RenewTable
    {
        public int renewround { get; set; }
        public string acceptdate { get; set; }
        public string expiredate { get; set; }
    }

    public class ModelMenuD1ProjectNumberData
    {
        public string projectheadname { get; set; }
        public string positionname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string facultyname { get; set; }
        public string advisorsnamethai { get; set; }
        public string accepttypenamethai { get; set; }
        public string certificatetype { get; set; }
        public string acceptprojectno { get; set; }
        public string remarkapproval { get; set; }
        public string dateofapproval { get; set; }
        public IList<ModelMenuD1RenewTable> listRenewDate { get; set; }
    }

}
