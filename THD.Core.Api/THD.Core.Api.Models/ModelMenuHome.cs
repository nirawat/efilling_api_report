using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuHome1_InterfaceData
    {
        public IList<ModelSelectOption> ListYear { get; set; }
        public IList<ModelSelectOption> ListProjectHead { get; set; }
        public IList<ModelSelectOption> ListAcceptType { get; set; }
        public IList<ModelSelectOption> ListFaculty { get; set; }
        public IList<ModelSelectOption> ListAcronyms { get; set; }
        public IList<ModelSelectOption> ListRisk { get; set; }
        public IList<ModelMenuHome1ReportData> ListReportData { get; set; }
        public string year { get; set; }
        public string projecthead { get; set; }
        public string accepttype { get; set; }
        public string acronyms { get; set; }
        public string faculty { get; set; }
        public string risk { get; set; }
        public string userid { get; set; }
        public int usergroup { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelMenuHome1_DownloadFile
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelMenuHome1_ResultNote
    {
        public IList<ResultCommentNote> listcomment { get; set; }
    }

    public class ResultCommentNote
    {
        public int docid { get; set; }

        public string xseq { get; set; }
        public string xdate { get; set; }
        public string xassignName { get; set; }
        public string xriskName { get; set; }
        public string xapprovalName { get; set; }
        public string xcommentDetail { get; set; }

        public string seq { get; set; }
        public string date { get; set; }
        public string assignName { get; set; }
        public string riskName { get; set; }
        public string approvalName { get; set; }
        public string commentDetail { get; set; }
    }

    public class ModelMenuHome1ReportData
    {
        public string project_request_id { get; set; }
        public string project_name_thai { get; set; }
        public string project_name_eng { get; set; }
        public string project_head_name { get; set; }
        public string project_number { get; set; }
        public string acronyms { get; set; }
        public string risk_type { get; set; }
        public string delivery_online_date { get; set; }
        public string review_request_date { get; set; }
        public string result_doc_review { get; set; }
        public string committee_assign_date { get; set; }
        public string committee_name_array { get; set; }
        public string committee_comment_date { get; set; }
        public string meeting_date { get; set; }
        public string meeting_approval_date { get; set; }
        public string consider_result { get; set; }
        public string alert_date { get; set; }
        public string request_edit_meeting_date { get; set; }
        public string request_edit_date { get; set; }
        public string report_status_date { get; set; }
        public string certificate_expire_date { get; set; }
        public string request_renew_date { get; set; }
        public string close_project_date { get; set; }
        public string print_certificate_date { get; set; }
    }


    public class ModelMenuHome2_InterfaceData
    {
        public IList<ModelSelectOption> ListYear { get; set; }
        public IList<ModelSelectOption> ListAcceptType { get; set; }
        public IList<ModelMenuHome2ReportData> ListReportData { get; set; }
        public string defaultyear { get; set; }
        public string defaultaccepttype { get; set; }

        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuHome2_DownloadFile
    {
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelMenuHome2ReportData
    {
        public string col1 { get; set; }
        public string col2 { get; set; }
        public string col3 { get; set; }
        public string col4 { get; set; }
        public string col5 { get; set; }
        public string col6 { get; set; }
        public string col7 { get; set; }
        public string col8 { get; set; }
        public string col9 { get; set; }
        public string col10 { get; set; }
        public string col11 { get; set; }
        public string col12 { get; set; }
        public string col13 { get; set; }
        public string col14 { get; set; }
        public string col15 { get; set; }
    }

}
