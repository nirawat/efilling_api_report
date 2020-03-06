using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_12_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_12_report
    {
        public string docno { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public int year { get; set; }
        public string doc_head_4 { get; set; }
        public string projectno { get; set; }
        public string researchname_thai { get; set; }
        public string researchname_eng { get; set; }
        public string sender { get; set; }
        public string round { get; set; }
        public string comment1 { get; set; }
        public string comment2 { get; set; }
        public string nuibc { get; set; }
        public string researcher { get; set; }
        public string research_name_thai { get; set; }
        public string research_name_eng { get; set; }
        public string project_qty { get; set; }
        public string certificate_date { get; set; }
        public string approve_date { get; set; }
        public string approvetype { get; set; }
        public string assignname { get; set; }
        public string line_head1 { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }

        public string line5 { get; set; }
        public string line6 { get; set; }
        public string line7 { get; set; }
        public string line8 { get; set; }
        public string line9 { get; set; }
        public string line10 { get; set; }

        public string signature_name_1 { get; set; }
    }
}
