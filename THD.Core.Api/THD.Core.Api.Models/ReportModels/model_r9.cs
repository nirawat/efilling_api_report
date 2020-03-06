using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_9_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_9_report
    {
        public string projecttype { get; set; }
        public string project_namethai { get; set; }
        public string project_nameeng { get; set; }
        public string researcher { get; set; }
        public string advisor { get; set; }
        public string faculty { get; set; }
        public string project_no { get; set; }
        public string certificate_no { get; set; }
        public string certificate_type { get; set; }
        public string round { get; set; }
        public string certificate_date { get; set; }
        public string certificate_month { get; set; }
        public int certificate_year { get; set; }
        public string expire_date { get; set; }
        public string expire_month { get; set; }
        public int expire_year { get; set; }
        public string President { get; set; }
        public string note { get; set; }
        public string remark { get; set; }
        public string signature_name_1 { get; set; }
    }
}
