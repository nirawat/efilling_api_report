using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_10_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_10_report
    {
        public string docid { get; set; }
        public string assignercode { get; set; }
        public string assignername { get; set; }
        public string positionname { get; set; }
        public string assignerseq { get; set; }
        public string projectnumber { get; set; }
        public string projectheadname { get; set; }
        public string facultyname { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public string safetytype { get; set; }
        public string safetytypename { get; set; }
        public string approvaltype { get; set; }
        public string approvaltypename { get; set; }
        public string commentconsider1 { get; set; }
        public string commentconsider2 { get; set; }
        public string commentconsider3 { get; set; }
        public string commentconsider4 { get; set; }
        public string advisor { get; set; }
        public string doc_date { get; set; }
        public bool chkbox1 { get; set; }
        public bool chkbox2 { get; set; }
        public bool chkbox3 { get; set; }
        public bool chkbox4 { get; set; }
        public bool chkbox5 { get; set; }
        public string comment { get; set; }
        public string approvetype1 { get; set; }
        public string approvetype2 { get; set; }
        public string approvetype3 { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public int year { get; set; }
    }
}
