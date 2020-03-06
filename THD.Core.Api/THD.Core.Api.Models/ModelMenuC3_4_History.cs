using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuC3Tab4_InterfaceData_History
    {
        public IList<ModelSelectOption> ListMeetingId { get; set; }
        public IList<ModelSelectOption> ListConsiderType { get; set; }
        public IList<ModelSelectOption> ListApprovalType { get; set; }
        public IList<ModelMenuC3Tab4_Data> ListReportData { get; set; }
        public string meetingid { get; set; }
        public string projectnumber { get; set; }
        public string considertypeid { get; set; }
        public string approvaltypeid { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuC3Tab4_Data
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


}
