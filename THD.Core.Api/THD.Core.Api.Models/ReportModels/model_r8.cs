using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_8_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_8_report
    {
        public string docno { get; set; }
        public string projecttype { get; set; }
        public string doc_head_1 { get; set; }
        public string doc_head_2 { get; set; }
        public string doc_head_3 { get; set; }
        public string doc_head_4 { get; set; }
        public string research_name_thai { get; set; }
        public string research_name_eng { get; set; }
        public string Job_Position { get; set; }
        public string faculty_name { get; set; }
        public string headofresearch_fullname { get; set; }
        public string nuibc_no { get; set; }
        public string renew_round { get; set; }
        public string month_project { get; set; }
        public string year_project { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public int year { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string line4 { get; set; }
        public string signature_name_1 { get; set; }
    }
}
