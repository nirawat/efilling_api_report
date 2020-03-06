using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuB1_InterfaceData
    {
        public IList<ModelSelectOption> ListProjectHead { get; set; }
        public IList<ModelSelectOption> ListProjectNameThai { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public IList<ModelSelectOption> ListDownloadFile { get; set; }
        public int defaultround { get; set; }
        public int defaultyear { get; set; }
        public string defaulttypeid { get; set; } = "1";
        public string defaulttypename { get; set; } = "ข้อเสนอโครงการ";
        public string defaultuserid { get; set; }
        public string defaultusername { get; set; }
        public ModelMenuB1Edit editdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuB1_GetDataByProjectNameThai
    {
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public IList<ModelSelectOption> ListDownloadFile { get; set; }
    }

    public class ModelMenuB1_FileDownload
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelMenuB1
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string accepttype { get; set; }
        public string projecthead { get; set; }
        public string projectid { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string acronyms { get; set; }
        public string initialresult { get; set; }
        public string filedownloadnametitle { get; set; }
        public string filedownloadname { get; set; }
        public string projectkeynumber { get; set; }
        public string notes { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public int defaultyear { get; set; }
        public string meetingdate { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }

    public class ModelMenuB1Edit
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string accepttype { get; set; }
        public string projecthead { get; set; }
        public string projectheadname { get; set; }
        public string projectid { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string acronyms { get; set; }
        public string initialresult { get; set; }
        public string initialresultname { get; set; }
        public string filedownloadnametitle { get; set; }
        public string filedownloadname { get; set; }
        public string projectkeynumber { get; set; }
        public string notes { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public string defaultyear { get; set; }
        public string meetingdate { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }

    public class ModelMenuB1Data
    {
        public string accepttype { get; set; }
        public string accepttypenamethai { get; set; }
        public string projecthead { get; set; }
        public string projectheadname { get; set; }
        public string projectid { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string positionnamethai { get; set; }
        public string facultyname { get; set; }
        public string acronyms { get; set; }
        public string initialresult { get; set; }
        public string initialnamethai { get; set; }
        public string filedownloadname { get; set; }
        public string projectkeynumber { get; set; }
        public int roundofmeeting { get; set; }
        public int yearofmeeting { get; set; }
        public string meetingsetdate { get; set; }
        public string conclusiondate { get; set; }
    }

}
