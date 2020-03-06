using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_11_file
    {
        public string message { get; set; }
        public string filename_1 { get; set; }
        public string filebase_1_64 { get; set; }
        public string filename_2 { get; set; }
        public string filebase_2_64 { get; set; }
        public string filename_3 { get; set; }
        public string filebase_3_64 { get; set; }
    }

    public class model_rpt_11_report
    {
        public string docno { get; set; }
        public string doc_head_4 { get; set; }
        public string round { get; set; }
        public string project_qty { get; set; }
        public string certificate_meeting { get; set; }
        public string certificate_date { get; set; }
        public string approve_date { get; set; }
        public string meet_date { get; set; }
        public string meet_month { get; set; }
        public string meet_year { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public int year { get; set; }
        public string assign { get; set; }
        public string line_head1 { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }
        public IList<ModelSelectOption> boardcodearray { get; set; }
        public string signature_name_1 { get; set; }
    }
}
