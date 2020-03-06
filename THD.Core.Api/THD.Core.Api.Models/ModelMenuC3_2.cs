using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC32_InterfaceData
    {
        public string meetingId { get; set; }
        public string meetingName { get; set; }
        public bool isFileAttachment { get; set; }
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public ModelMenuC32 editdata { get; set; }
    }
    public class ModelMenuC32
    {
        public int meetingid { get; set; } //เอกสารอ้างอิงจากหน้าบันทึกประชุม

        public string tab2Group1Seq1Input1 { get; set; }
        public string tab2Group1Seq1FileInput2 { get; set; }
        public string tab2Group1Seq1FileInput2Base64 { get; set; }
        public string tab2Group1Seq1Input3 { get; set; }
        public string tab2Group1Seq1Input4 { get; set; }
        public string tab2Group1Seq2Input1 { get; set; }
        public string tab2Group1Seq2FileInput2 { get; set; }
        public string tab2Group1Seq2FileInput2Base64 { get; set; }
        public string tab2Group1Seq2Input3 { get; set; }
        public string tab2Group1Seq2Input4 { get; set; }
        public string tab2Group1Seq3Input1 { get; set; }
        public string tab2Group1Seq3FileInput2 { get; set; }
        public string tab2Group1Seq3FileInput2Base64 { get; set; }
        public string tab2Group1Seq3Input3 { get; set; }
        public string tab2Group1Seq3Input4 { get; set; }

        public string createby { get; set; }
        public bool editenable { get; set; }
        public string meetingresolution { get; set; }
    }


    public class ModelMenuC32Tab2Group1
    {
        public string groupdata { get; set; }
        public string seq { get; set; }
        public string input1 { get; set; }
        public string input2 { get; set; }
        public string input3 { get; set; }
        public string input4 { get; set; }
    }

    public class ModelMenuC32_DownloadFileName
    {
        public string file1name { get; set; }
        public string file2name { get; set; }
        public string file3name { get; set; }
    }

    public class ModelMenuC32_DownloadFile
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


}
