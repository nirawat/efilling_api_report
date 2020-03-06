using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC35_InterfaceData
    {
        public string meetingId { get; set; }
        public string meetingName { get; set; }
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public ModelPermissionPage UserPermission { get; set; }

        public ModelMenuC35 editdata { get; set; }
    }
    public class ModelMenuC35
    {
        public int meetingid { get; set; }
        public string meetingName { get; set; }

        //Tab 5
        public string tab5Group1Seq1Input1 { get; set; }
        public string tab5Group1Seq1Input2 { get; set; }
        public string tab5Group1Seq1Input3 { get; set; }

        public string tab5Group1Seq2Input1 { get; set; }
        public string tab5Group1Seq2Input2 { get; set; }
        public string tab5Group1Seq2Input3 { get; set; }

        public string tab5Group1Seq3Input1 { get; set; }
        public string tab5Group1Seq3Input2 { get; set; }
        public string tab5Group1Seq3Input3 { get; set; }

        public string createby { get; set; }
        public bool editenable { get; set; }
        public string meetingresolution { get; set; }
    }

    public class ModelMenuC35Tab5Group1
    {
        public string groupdata { get; set; }
        public string seq { get; set; }
        public string input1 { get; set; }
        public string input2 { get; set; }
        public string input3 { get; set; }
    }

}
