using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{
    public class model_rpt_14_file
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class model_rpt_14_report
    {
        public string Doc_head_1 { get; set; }
        public string Doc_head_2 { get; set; }
        public string Doc_head_3 { get; set; }
        public string Doc_head_4 { get; set; }
        public string Doc_head_5 { get; set; }
        public string Doc_head_6 { get; set; } // ปิดประชุม


        // ผู้เข้าร่วมประชุม -----------------------
        public IList<model_list_person_meeting> list_person_meeting { get; set; }

        // ผู้ติดราชการ --------------------------
        public IList<model_list_person_meeting> list_person_out_of_meeting { get; set; }

        // ระเบียบวาระที่ 1 -----------------------
        public IList<model_list_agenda_1_1> list_agenda_1_1 { get; set; }
        public IList<model_list_agenda_1_2> list_agenda_1_2 { get; set; }

        // ระเบียบวาระที่ 2 -----------------------
        public IList<model_list_agenda_2> list_agenda_2 { get; set; }

        // ระเบียบวาระที่ 3 -----------------------
        public IList<model_list_agenda_3> list_agenda_3 { get; set; }

        // ระเบียบวาระที่ 4 -----------------------
        public IList<model_list_agenda_4> list_agenda_4_1 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_2 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_3 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_4 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_5 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_6 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_7 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_8 { get; set; }
        public IList<model_list_agenda_4> list_agenda_4_9 { get; set; }

        // ระเบียบวาระที่ 5 -----------------------
        public IList<model_list_agenda_5> list_agenda_5 { get; set; }

        public string signature_name_1 { get; set; }

    }

    public class model_list_person_meeting
    {
        public string seq { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string position { get; set; }
        public string department { get; set; }
    }

    public class model_list_agenda_1_1
    {
        public string title { get; set; } = "-";
        public string subject { get; set; } = "ระเบียบวาระที่ 1.1.1 ไม่มี";
        public string subject_summary { get; set; } = "สรุปเรื่อง";
        public string detail_summary { get; set; } = "ไม่มี";
        public string subject_conclusion { get; set; } = "มติ";
        public string detail_conclusion { get; set; } = "ไม่มี";
    }

    public class model_list_agenda_1_2
    {
        public string title { get; set; } = "-";
        public string subject { get; set; } = "ระเบียบวาระที่ 1.2.1 ไม่มี";
        public string subject_summary { get; set; } = "สรุปเรื่อง";
        public string detail_summary { get; set; } = "ไม่มี";
        public string subject_conclusion { get; set; } = "มติ";
        public string detail_conclusion { get; set; } = "ไม่มี";
    }

    public class model_list_agenda_2
    {
        public string title { get; set; } = "-";
        public string subject { get; set; } = "ระเบียบวาระที่ 2.1 ไม่มี";
        public string subject_summary { get; set; } = "สรุปเรื่อง";
        public string detail_summary { get; set; } = "ไม่มี";
        public string subject_conclusion { get; set; } = "มติ";
        public string detail_conclusion { get; set; } = "ไม่มี";
    }

    public class model_list_agenda_3
    {
        public string title { get; set; } = "โครงการวิจัยที่รับรองหลังจากปรับปรุง/แก้ไข จำนวน 0 โครงการ ดังนี้";
        public string subject { get; set; } = "เรื่อง :";
        public string project_number { get; set; } = "ไม่มี";
        public string project_name_thai { get; set; } = "ไม่มี";
        public string project_name_eng { get; set; } = "ไม่มี";
        public string project_safety_type { get; set; } = "ไม่มี";
        public string consultant_name { get; set; } = "ไม่มี";
        public string list_researchers { get; set; } = "ไม่มี";
        public string comment_1_name { get; set; } = "1. ไม่มี";
        public string comment_1_desc { get; set; } = "ไม่มี";
        public string comment_2_name { get; set; } = "2. ไม่มี";
        public string comment_2_desc { get; set; } = "ไม่มี";
        public string comment_3_name { get; set; } = "3. ไม่มี";
        public string comment_3_desc { get; set; } = "ไม่มี";
        public string conclusion_name { get; set; } = "ไม่มี";
        public string detail_conclusion { get; set; } = "ไม่มี";

        // เรื่องสืบเนื่อง ----------------------------------------
        public IList<model_list_agenda_3_2> list_agenda_3_2 { get; set; }
    }

    public class model_list_agenda_3_2
    {
        public string sequel_title { get; set; } = "3.2.1. ไม่มี";
        public string sequel_detail_summary { get; set; } = "ไม่มี";
        public string sequel_detail_conclusion { get; set; } = "ไม่มี";
    }

    public class model_personal
    {
        public string projecthead { get; set; }
        public string facultyname { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
    }

    public class model_list_agenda_4
    {
        public string title { get; set; } = "-";
        public string subject { get; set; } = "เรื่อง :";
        public string project_number { get; set; } = "ไม่มี";
        public string project_name_thai { get; set; } = "ไม่มี";
        public string project_name_eng { get; set; } = "ไม่มี";
        public string project_safety_type { get; set; } = "ไม่มี";
        public string consultant_name { get; set; } = "ไม่มี";
        public string list_researchers { get; set; } = "ไม่มี";
        public string comment_1_name { get; set; } = "1. ไม่มี";
        public string comment_1_desc { get; set; } = "ไม่มี";
        public string comment_2_name { get; set; } = "2. ไม่มี";
        public string comment_2_desc { get; set; } = "ไม่มี";
        public string comment_3_name { get; set; } = "3. ไม่มี";
        public string comment_3_desc { get; set; } = "ไม่มี";
        public string conclusion_name { get; set; } = "ไม่มี";
        public string detail_conclusion { get; set; } = "ไม่มี";

    }

    public class model_list_agenda_5
    {
        public string title { get; set; } = "-";
        public string subject { get; set; } = "ระเบียบวาระที่ 5.1 ไม่มี";
        public string subject_summary { get; set; } = "สรุปเรื่อง";
        public string detail_summary { get; set; } = "ไม่มี";
        public string subject_conclusion { get; set; } = "มติ";
        public string detail_conclusion { get; set; } = "ไม่มี";
    }

}
