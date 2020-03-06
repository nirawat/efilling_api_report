using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuD2_InterfaceData
    {
        public int defaultyear { get; set; }
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public IList<ModelSelectOption> listYearOfMeeting { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuD2_FileDownload
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }
    public class ModelMenuD2
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string accepttypenamethai { get; set; }
        public string filedownloadname { get; set; }
        public int agendanumber { get; set; }
        public int yearofmeeting { get; set; }
        public string agendameetingdate { get; set; }
        public string acceptTypeNameThai { get; set; }
        public string remarkApproval { get; set; }
        public string conclusiondate { get; set; }

        public string createby { get; set; }
        public bool editenable { get; set; }

    }


    public class ModelMenuD2ProjectNumberData
    {
        public bool isprojectgroup { get; set; }
        public string projectheadname { get; set; }
        public string positionname { get; set; }
        public string facultyname { get; set; }
        public string projectname1 { get; set; }
        public string projectname2 { get; set; }
        public string certificatetype { get; set; }
        public string remarkapproval { get; set; }
        public string dateofapproval { get; set; }

        public IList<ModelSelectOption> ListDownloadFile { get; set; }
    }

}
