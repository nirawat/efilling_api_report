using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC33_InterfaceData
    {
        public string meetingId { get; set; }
        public string meetingName { get; set; }
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public IList<ModelSelectOption> ListProjectNumberTab3 { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public ModelMenuC33 editdata { get; set; }
    }
    public class ModelMenuC33
    {
        public int docid { get; set; }
        public int meetingid { get; set; } //เอกสารอ้างอิงจากหน้าบันทึกประชุม
        public int agenda3projectcount { get; set; }
        public string agenda3projectnumber { get; set; }
        public string agenda3projectnamethai { get; set; }
        public string agenda3projectnameeng { get; set; }
        public string agenda3Conclusion { get; set; }
        public string agenda3ConclusionName { get; set; }
        public string agenda3Suggestion { get; set; }
        public string docprocessfrom { get; set; }
        public int safetytype { get; set; }
        public string safetytypename { get; set; }

        //Tab 3 Group 1
        //public string tab3Group1Seq1Input1 { get; set; }
        //public string tab3Group1Seq1Input2 { get; set; }
        //public string tab3Group1Seq1Input3 { get; set; }
        //public string tab3Group1Seq2Input1 { get; set; }
        //public string tab3Group1Seq2Input2 { get; set; }
        //public string tab3Group1Seq2Input3 { get; set; }
        //public string tab3Group1Seq3Input1 { get; set; }
        //public string tab3Group1Seq3Input2 { get; set; }
        //public string tab3Group1Seq3Input3 { get; set; }


        //Tab 3 Group 2
        //public string tab3Group2Seq1Input1 { get; set; }
        //public string tab3Group2Seq1Input2 { get; set; }
        //public string tab3Group2Seq1Input3 { get; set; }
        //public string tab3Group2Seq2Input1 { get; set; }
        //public string tab3Group2Seq2Input2 { get; set; }
        //public string tab3Group2Seq2Input3 { get; set; }
        //public string tab3Group2Seq3Input1 { get; set; }
        //public string tab3Group2Seq3Input2 { get; set; }
        //public string tab3Group2Seq3Input3 { get; set; }


        public string comment1title { get; set; }
        public string comment1comittee { get; set; }
        public string comment1note { get; set; }
        public string comment2title { get; set; }
        public string comment2comittee { get; set; }
        public string comment2note { get; set; }
        public string comment3title { get; set; }
        public string comment3comittee { get; set; }
        public string comment3note { get; set; }

        public string sequel1title { get; set; }
        public string sequel1summary { get; set; }
        public string sequel1note { get; set; }
        public string sequel2title { get; set; }
        public string sequel2summary { get; set; }
        public string sequel2note { get; set; }
        public string sequel3title { get; set; }
        public string sequel3summary { get; set; }
        public string sequel3note { get; set; }


        public string createby { get; set; }
        public bool editenable { get; set; }
        public string meetingresolution { get; set; }
    }

    public class ModelMenuC33Data
    {
        public bool isprojectgroup { get; set; }
        public string projectnamethai { get; set; }
        public string projectnameeng { get; set; }
        public IList<ModelSelectOption> ListAssignerUser { get; set; }
        public IList<ModelSelectOption> ListApprovalType { get; set; }


        public string comment1title { get; set; }
        public string comment1comittee { get; set; }
        public string comment1note { get; set; }
        public string comment2title { get; set; }
        public string comment2comittee { get; set; }
        public string comment2note { get; set; }
        public string comment3title { get; set; }
        public string comment3comittee { get; set; }
        public string comment3note { get; set; }
    }

    public class ModelMenuC33Tab3GroupAll
    {
        public string groupdata { get; set; }
        public string seq { get; set; }
        public string input1 { get; set; }
        public string input2 { get; set; }
        public string input3 { get; set; }
    }

    public class ModelMenuC33HistoryData
    {
        public string rptMeetingId { get; set; }
        public string rptMeetingTitle { get; set; }
        public string rptAgenda31 { get; set; }
        public string rptProjectCount { get; set; }
        public string rptProjectNumber { get; set; }
        public string rptProjectNameThai { get; set; }
        public string rptProjectNameEng { get; set; }
        public string rptConclusionName { get; set; }
        public string rptSuggestionName { get; set; }
    }



}
