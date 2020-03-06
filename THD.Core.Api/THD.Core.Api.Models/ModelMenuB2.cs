using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuB2_InterfaceData
    {
        public IList<ModelSelectOption> ListLabNumber { get; set; }
        public IList<ModelSelectOption> ListProjectNameThai { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public int defaultyear { get; set; }
        public string defaulttypeid { get; set; } = "2";
        public string defaulttypename { get; set; } = "ประเมิณห้องปฏิบัติการ";
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuB2_FileDownload
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelMenuB2
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string accepttype { get; set; }
        public string projectid { get; set; }
        public string labNumber { get; set; }
        public string labTypeName { get; set; }
        public string facultyName { get; set; }
        public string initialresult { get; set; }
        public string filedownloadname { get; set; }
        public string projectkeynumber { get; set; }
        public string notes { get; set; }
        public string acronyms { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public int defaultyear { get; set; }
        public string meetingdate { get; set; }
    }

    public class ModelMenuB2Data
    {
        public string labTypeName { get; set; }
        public string facultyName { get; set; }
        public IList<ModelSelectOption> ListDownloadFile { get; set; }
    }

}
