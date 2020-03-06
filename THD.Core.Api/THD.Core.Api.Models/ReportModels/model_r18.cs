using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_18_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_18_report
    {
        public string labLocation { get; set; }
        public string labboy { get; set; }
        public string tel_no { get; set; }
        public string assessment_agent { get; set; }
        public string assessment_date { get; set; }
        public string projecttype { get; set; }

    }
}
