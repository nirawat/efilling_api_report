using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC34_InterfaceData
    {
        public string meetingId { get; set; }
        public string meetingName { get; set; }
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public IList<ModelSelectOption> ListProjectNumberTab4 { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
        public ModelMenuC34 editdata { get; set; }
    }
    public class ModelMenuC34
    {
        public int docid { get; set; }
        public int meetingid { get; set; } //เอกสารอ้างอิงจากหน้าบันทึกประชุม

        public string agenda4term { get; set; }
        public string agenda4termname { get; set; }
        public string agenda4projectnumber { get; set; }
        public string agenda4projectname1 { get; set; }
        public string agenda4projectname2 { get; set; }
        public string agenda4Conclusion { get; set; }
        public string agenda4ConclusionName { get; set; }
        public string agenda4Suggestion { get; set; }
        public int safetytype { get; set; }
        public string safetytypename { get; set; }
        public string file1name { get; set; }
        public string file1base64 { get; set; }


        //Tab 4 Group 1
        public string tab4Group1Seq1Input1 { get; set; }
        public string tab4Group1Seq1Input2 { get; set; }
        public string tab4Group1Seq1Input3 { get; set; }
        public string tab4Group1Seq2Input1 { get; set; }
        public string tab4Group1Seq2Input2 { get; set; }
        public string tab4Group1Seq2Input3 { get; set; }
        public string tab4Group1Seq3Input1 { get; set; }
        public string tab4Group1Seq3Input2 { get; set; }
        public string tab4Group1Seq3Input3 { get; set; }



        public string createby { get; set; }
        public bool editenable { get; set; }
        public string docprocessfrom { get; set; }
    }

    public class ModelMenuC34Tab4Data
    {
        public bool isprojectgroup { get; set; }
        public string agenda4ProjectName1 { get; set; }
        public string agenda4ProjectName2 { get; set; }
        public IList<ModelSelectOption> ListApprovalType { get; set; }

        public string tab4Group1Seq1Input1 { get; set; }
        public string tab4Group1Seq1Input2 { get; set; }
        public string tab4Group1Seq1Input3 { get; set; }
        public string tab4Group1Seq2Input1 { get; set; }
        public string tab4Group1Seq2Input2 { get; set; }
        public string tab4Group1Seq2Input3 { get; set; }
        public string tab4Group1Seq3Input1 { get; set; }
        public string tab4Group1Seq3Input2 { get; set; }
        public string tab4Group1Seq3Input3 { get; set; }
    }

    public class ModelMenuC34Tab4GroupAll
    {
        public string groupdata { get; set; }
        public string seq { get; set; }
        public string input1 { get; set; }
        public string input2 { get; set; }
        public string input3 { get; set; }
    }

    public class ModelMenuC34HistoryData
    {

        public string rptMeetingId { get; set; }
        public string rptMeetingTitle { get; set; }
        public string rptAgenda41 { get; set; }
        public string rptAgendaName { get; set; }
        public string rptProjectNumber { get; set; }
        public string rptProjectName1 { get; set; }
        public string rptProjectName2 { get; set; }
        public string rptConclusionName { get; set; }
        public string rptSuggestionName { get; set; }
    }

    public class ModelMenuC34ResultNote
    {
        public string resultNote { get; set; }
    }

    public class ModelMenuC34_DownloadFile
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

}
