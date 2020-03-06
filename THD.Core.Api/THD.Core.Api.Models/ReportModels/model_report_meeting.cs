using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Models.ReportModels
{

    public class model_rpt_meeting_file
    {
        public string message { get; set; }
        public string meetingRound { get; set; }
        public string meetingYear { get; set; }
        //Report R9 --------------------------------
        public string filename9 { get; set; }
        public string filebase964 { get; set; }
        //Report R12 --------------------------------
        public string filename12 { get; set; }
        public string filebase1264 { get; set; }
        //Report R13 --------------------------------
        public string filename13 { get; set; }
        public string filebase1364 { get; set; }
        //Report R14 --------------------------------
        public string filename14 { get; set; }
        public string filebase1464 { get; set; }
    }

}
