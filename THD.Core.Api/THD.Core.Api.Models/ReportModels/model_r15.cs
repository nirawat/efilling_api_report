using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_15_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_15_report
    {
        public string Doc_head_1 { get; set; }
        public string Doc_head_2 { get; set; }
        public string Doc_head_3 { get; set; }
        public string Doc_head_4 { get; set; }
        public string Doc_head_5 { get; set; }

        // วาระที่ 1 ----------------------------
        public string title_1 { get; set; }

        public string subject_1_1 { get; set; }
        public IList<model_rpt_15_items> list_item_1_1 { get; set; }

        public string subject_1_2 { get; set; }
        public IList<model_rpt_15_items> list_item_1_2 { get; set; }

        // วาระที่ 2 ----------------------------
        public string title_2 { get; set; }

        public string subject_2_1 { get; set; }
        public IList<model_rpt_15_items> list_item_2_1 { get; set; }


        // วาระที่ 3 ----------------------------
        public string title_3 { get; set; }

        public string subject_3_1 { get; set; }
        public string subject_3_1_qty { get; set; }
        public IList<model_rpt_15_items> list_item_3_1 { get; set; }

        public string subject_3_2 { get; set; }
        public IList<model_rpt_15_items> list_item_3_2 { get; set; }



        // วาระที่ 4 ----------------------------
        public string title_4 { get; set; }
        public string subject_4_1 { get; set; }
        public IList<model_rpt_15_4> list_item_4_1 { get; set; }


        // วาระที่ 5 ----------------------------
        public string title_5 { get; set; }

        public string subject_5_1 { get; set; }
        public IList<model_rpt_15_items> list_item_5_1 { get; set; }

    }

    public class model_rpt_15_items
    {
        public string item { get; set; } = "ไม่มี";
    }

    public class model_rpt_15_4
    {
        public string item_4_1_1 { get; set; }
        public string item_4_1_2 { get; set; }
        public string item_4_1_3 { get; set; }
        public string item_4_1_4 { get; set; }
        public string item_4_1_5 { get; set; }
        public string item_4_1_6 { get; set; }
        public string item_4_1_7 { get; set; }
        public string item_4_1_8 { get; set; }
        public string item_4_1_9 { get; set; }

        public string item_4_1_1_qty { get; set; }
        public string item_4_1_2_qty { get; set; }
        public string item_4_1_3_qty { get; set; }
        public string item_4_1_4_qty { get; set; }
        public string item_4_1_5_qty { get; set; }
        public string item_4_1_6_qty { get; set; }
        public string item_4_1_7_qty { get; set; }
        public string item_4_1_8_qty { get; set; }
        public string item_4_1_9_qty { get; set; }
    }

}
