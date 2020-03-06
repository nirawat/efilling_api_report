using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_16_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_16_report
    {
        public string title_report { get; set; }
        public string project_head { get; set; }
        public string work_location { get; set; }
        public string telephone_no { get; set; }
        public string fax_no { get; set; }
        public string email_address { get; set; }
        public string project_namethai { get; set; }
        public string project_nameeng { get; set; }
        public string fund_source { get; set; }
        public decimal fund_amount { get; set; }
        public string fund_amount_thaicharacter { get; set; }
        public bool research_group1_type1 { get; set; }
        public bool research_group1_type2 { get; set; }
        public bool research_group1_type3 { get; set; }
        public bool research_group1_type4 { get; set; }
        public bool research_group1_type5 { get; set; }
        public string research_group1_other { get; set; }
        public bool research_group2_type1 { get; set; }
        public bool research_group2_type2 { get; set; }
        public bool research_group2_type3 { get; set; }
        public bool research_group2_type4 { get; set; }
        public bool research_group2_type5 { get; set; }
        public string research_group2_other { get; set; }
        public bool risk_group_1 { get; set; }
        public bool risk_group_1_1 { get; set; }
        public bool risk_group_1_2 { get; set; }
        public bool risk_group_1_3 { get; set; }
        public bool risk_group_1_4 { get; set; }
        public bool risk_group_1_5 { get; set; }
        public string risk_group_1_5_other { get; set; }
        public bool risk_group_2 { get; set; }
        public bool risk_group_2_1 { get; set; }
        public bool risk_group_2_2 { get; set; }
        public bool risk_group_2_3 { get; set; }
        public bool risk_group_2_4 { get; set; }
        public bool risk_group_2_5 { get; set; }
        public bool risk_group_3 { get; set; }
        public bool risk_group_3_1 { get; set; }
        public bool risk_group_3_2 { get; set; }
        public bool risk_group_3_3 { get; set; }
        public bool risk_group_3_4 { get; set; }
        public bool risk_group_3_5 { get; set; }
        public bool risk_group_4 { get; set; }
        public bool risk_group_4_1 { get; set; }
        public bool risk_group_4_2 { get; set; }
        public bool risk_group_4_3 { get; set; }
        public bool risk_group_4_4 { get; set; }
        public bool risk_group_4_5 { get; set; }
    }
}
