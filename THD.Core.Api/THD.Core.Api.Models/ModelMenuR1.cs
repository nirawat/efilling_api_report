using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuR1_InterfaceData
    {
        public IList<ModelSelectOption> ListMeetingType { get; set; }
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public IList<ModelMenuR1Data> ListReportData { get; set; }
        public string meetingid { get; set; }
        public string meetingTypeId { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuR1Data
    {
        public string docid { get; set; }
        public string meetinground { get; set; }
        public string yearofmeeting { get; set; }
        public string meetingdate { get; set; }
        public string meetinglocation { get; set; }
        public string meetingstart { get; set; }
        public string meetingclose { get; set; }
        public bool isclosed { get; set; }
    }

    public class ModelMenuR1RegisterInfo
    {
        public string registerid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string faculty { get; set; }
    }

    public class ModelMenuR1ReportFile
    {
        public string message { get; set; }
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


}
