using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuB1_2_InterfaceData
    {
        public IList<ModelSelectOption> ListProjectNumber { get; set; }
        public IList<ModelSelectOption> ListYearOfProject { get; set; }
        public int defaultround { get; set; }
        public int defaultyear { get; set; }
        public string defaultuserid { get; set; }
        public string defaultusername { get; set; }
        public ModelMenuB1Edit editdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuB1_2_GetDataByProjectNumber
    {
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public IList<ModelSelectOption> ListDownloadFile { get; set; }
    }

    public class ModelMenuB1_2_FileDownload
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelMenuB1_2
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string projectnumber { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string initialresult { get; set; }
        public string filedownloadnametitle { get; set; }
        public string filedownloadname { get; set; }
        public string notes { get; set; }
        public string roundofmeeting { get; set; }
        public string yearofmeeting { get; set; }
        public int defaultyear { get; set; }
        public string meetingdate { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }

}
