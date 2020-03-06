using System;
using System.Collections.Generic;
using THD.Core.Api.Repository.DataContext;
using THD.Core.Api.Repository.Interface;
using Microsoft.Extensions.Configuration;
using THD.Core.Api.Entities.Tables;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THD.Core.Api.Helpers;
using System.Data;
using THD.Core.Api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuHomeRepository : IDocMenuHomeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuHomeRepository(IConfiguration configuration, IDropdownListRepository DropdownListRepository, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));

            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
        }

        public async Task<ModelMenuHome1_InterfaceData> MenuHome1InterfaceDataAsync(string RegisterId)
        {
            ModelMenuHome1_InterfaceData resp = new ModelMenuHome1_InterfaceData();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYear = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYear.Add(year_current);

            for (int i = 1; i < 5; i++)
            {
                ModelSelectOption year_next = new ModelSelectOption();
                year_next.value = (thai_year + i).ToString();
                year_next.label = (thai_year + i).ToString();
                resp.ListYear.Add(year_next);
            }
            ModelSelectOption all_year = new ModelSelectOption();
            all_year.value = "All";
            all_year.label = "เลือก...";
            resp.ListYear.Add(all_year);

            //------------------------------------------------------------------------------------------
            resp.ListProjectHead = await GetAllProjectHeadAsync();
            if (resp.ListProjectHead == null) resp.ListProjectHead = new List<ModelSelectOption>();
            ModelSelectOption all_project_head = new ModelSelectOption();
            all_project_head.value = "YWxs";
            all_project_head.label = "เลือก...";
            resp.ListProjectHead.Add(all_project_head);

            //------------------------------------------------------------------------------------------
            resp.ListAcceptType = new List<ModelSelectOption>();
            ModelSelectOption accept_type_1 = new ModelSelectOption();
            accept_type_1.value = "1";
            accept_type_1.label = "ขอรับการพิจารณารับรองด้านความปลอดภัยทางชีวภาพระดับห้องทดลอง";
            resp.ListAcceptType.Add(accept_type_1);
            ModelSelectOption accept_type_2 = new ModelSelectOption();
            accept_type_2.value = "2";
            accept_type_2.label = "ขอรับการพิจารณารับรองด้านความปลอดภัยทางชีวภาพระดับภาคสนาม";
            resp.ListAcceptType.Add(accept_type_2);
            ModelSelectOption accept_type_all = new ModelSelectOption();
            accept_type_all.value = "ALL";
            accept_type_all.label = "เลือก...";
            resp.ListAcceptType.Add(accept_type_all);

            //------------------------------------------------------------------------------------------

            resp.ListFaculty = await GetAllFacultyAsync();
            if (resp.ListFaculty == null) resp.ListFaculty = new List<ModelSelectOption>();
            ModelSelectOption faculty_all = new ModelSelectOption();
            faculty_all.value = "All";
            faculty_all.label = "เลือก...";
            resp.ListFaculty.Add(faculty_all);

            //------------------------------------------------------------------------------------------
            resp.ListAcronyms = await GetAllAcronymsAsync();
            if (resp.ListAcronyms == null) resp.ListAcronyms = new List<ModelSelectOption>();
            ModelSelectOption acronyms_all = new ModelSelectOption();
            acronyms_all.value = "ALL";
            acronyms_all.label = "เลือก...";
            resp.ListAcronyms.Add(acronyms_all);

            //------------------------------------------------------------------------------------------
            resp.ListRisk = new List<ModelSelectOption>();
            ModelSelectOption risk_1 = new ModelSelectOption();
            risk_1.value = "1";
            risk_1.label = "งานวิจัยประเภทที่ 1";
            resp.ListRisk.Add(risk_1);
            ModelSelectOption risk_2 = new ModelSelectOption();
            risk_2.value = "2";
            risk_2.label = "งานวิจัยประเภทที่ 2";
            resp.ListRisk.Add(risk_2);
            ModelSelectOption risk_3 = new ModelSelectOption();
            risk_3.value = "3";
            risk_3.label = "งานวิจัยประเภทที่ 3";
            resp.ListRisk.Add(risk_3);
            ModelSelectOption risk_4 = new ModelSelectOption();
            risk_4.value = "4";
            risk_4.label = "งานวิจัยประเภทที่ 4";
            resp.ListRisk.Add(risk_4);
            ModelSelectOption risk_all = new ModelSelectOption();
            risk_all.value = "ALL";
            risk_all.label = "เลือก...";
            resp.ListRisk.Add(risk_all);


            //------------------------------------------------------------------------------------------

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M001");

            resp.usergroup = (resp.UserPermission.groupcode == "G001" || resp.UserPermission.groupcode == "G005" ? 2 : 1);

            ModelMenuHome1_InterfaceData search_data = new ModelMenuHome1_InterfaceData() { userid = RegisterId };

            resp.ListReportData = new List<ModelMenuHome1ReportData>();

            resp.ListReportData = await GetAllReportDataHome1Async(search_data);

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectHeadAsync()
        {
            string sql = "SELECT A.project_head, B.full_name " +
                        "FROM Transaction_Document A " +
                        "INNER JOIN RegisterUser B " +
                        "ON A.project_head = B.register_id " +
                        "GROUP BY A.project_head, B.full_name " +
                        "ORDER BY full_name ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelSelectOption> e = new List<ModelSelectOption>();
                        while (await reader.ReadAsync())
                        {
                            ModelSelectOption item = new ModelSelectOption();
                            item.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["project_head"].ToString()));
                            item.label = reader["full_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllFacultyAsync()
        {
            string sql = "SELECT * FROM MST_Faculty ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelSelectOption> e = new List<ModelSelectOption>();
                        while (await reader.ReadAsync())
                        {
                            ModelSelectOption item = new ModelSelectOption();
                            item.value = reader["id"].ToString();
                            item.label = reader["name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllAcronymsAsync()
        {
            string sql = "SELECT * FROM MST_Acronyms ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelSelectOption> e = new List<ModelSelectOption>();
                        while (await reader.ReadAsync())
                        {
                            ModelSelectOption item = new ModelSelectOption();
                            item.value = reader["id"].ToString();
                            item.label = reader["name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuA1> GetFileDownloadHome1Async(string project_number)
        {
            string sql = "SELECT B.project_key_number, A.file1name, A.file2name, " +
                        "A.file3name, A.file4name, A.file5name " +
                        "FROM Doc_MenuA1 A INNER JOIN Doc_MenuB1 B " +
                        "ON A.doc_id = B.project_id " +
                        "WHERE B.project_key_number='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuA1 e = new ModelMenuA1();
                        while (await reader.ReadAsync())
                        {
                            e.file1name = reader["file1name"].ToString();
                            e.file2name = reader["file2name"].ToString();
                            e.file3name = reader["file3name"].ToString();
                            e.file4name = reader["file4name"].ToString();
                            e.file5name = reader["file5name"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ResultCommentNote>> GetResultNoteHome1Async(string project_number, string user_id)
        {

            ModelPermissionPage user_permission = await _IRegisterUserRepository.GetPermissionPageAsync(user_id, "M001");

            string sql = "SELECT A.doc_id, ROW_NUMBER() OVER(PARTITION BY A.project_number ORDER BY A.doc_id ASC) as seq, A.doc_date, " +
                        "A.assigner_code, (B.first_name + B.full_name) as full_name, A.comment_consider, C.name_thai, (D.name_thai + ' ' + D.name_thai_sub) as approval_name_thai " +
                        "FROM Doc_MenuC2 A " +
                        "LEFT OUTER JOIN RegisterUser B " +
                        "ON A.assigner_code = B.register_id " +
                        "LEFT OUTER JOIN MST_Safety C " +
                        "ON A.safety_type = C.id " +
                        "LEFT OUTER JOIN MST_ApprovalType D " +
                        "ON A.approval_type = D.id " +
                        "WHERE 1=1 " + (user_permission.groupcode == "G002" ? " AND assigner_code='" + user_permission.registerid + "' " : "") +
                        "AND A.project_number='" + project_number + "' " +
                        "ORDER BY A.doc_id ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ResultCommentNote> e = new List<ResultCommentNote>();
                        while (await reader.ReadAsync())
                        {
                            ResultCommentNote item = new ResultCommentNote();

                            item.docid = Convert.ToInt32(reader["doc_id"]);

                            item.xseq = "ลำดับที่:";
                            item.xdate = " วันที่:";
                            item.xassignName = "ชื่อกรรมการ:";
                            item.xriskName = "ประเภทความเสี่ยง:";
                            item.xapprovalName = "ความเห็นการรับรอง:";
                            item.xcommentDetail = "ความเห็นประกอบการพิจารณา:";

                            item.commentDetail = reader["comment_consider"].ToString();
                            item.seq = Convert.ToInt32(reader["seq"]).ToString();
                            item.date = Convert.ToDateTime(reader["doc_date"]).ToString("dd/MM/yyyy");
                            item.assignName = reader["full_name"].ToString();
                            item.riskName = reader["name_thai"].ToString();
                            item.approvalName = reader["approval_name_thai"].ToString();
                            item.commentDetail = reader["comment_consider"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<string> GetCommentDataAsync(string project_number, string user_group, string user_id)
        {
            string userid = Encoding.UTF8.GetString(Convert.FromBase64String(user_id));

            StringBuilder comment_date = new StringBuilder();

            string sql = "SELECT * " +
                        "FROM Doc_MenuC2 " +
                        "WHERE 1=1 " + (user_group == "G002" ? " AND assigner_code='" + userid + "' " : "") +
                        "AND project_number='" + project_number + "' " +
                        "ORDER BY doc_id ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            comment_date.AppendLine("วันที่: " + Convert.ToDateTime(reader["doc_date"]).ToString("dd/MM/yyyy"));
                        }
                    }
                }
                conn.Close();
            }
            return comment_date.ToString();

        }

        public async Task<IList<ModelMenuHome1ReportData>> GetAllReportDataHome1Async(ModelMenuHome1_InterfaceData search_data)
        {

            ModelPermissionPage user_permission = await _IRegisterUserRepository.GetPermissionPageAsync(search_data.userid, "M001");

            string sql = "SELECT A.*, B.faculty, (B.first_name + ' ' + B.full_name) AS project_head_name " +
                        "FROM Transaction_Document A " +
                        "INNER JOIN RegisterUser B ON A.project_head = B.register_id " +
                        "WHERE 1=1 ";

            if (user_permission != null && user_permission.alldata == false)
            {
                string userid = Encoding.UTF8.GetString(Convert.FromBase64String(search_data.userid));

                sql += " AND (A.project_by='" + userid + "' OR A.committee_code_array LIKE '%" + userid + "%') ";
            }

            if (search_data != null)
            {
                if (!string.IsNullOrEmpty(search_data.year) && search_data.year.ToLower() != "all")
                    sql += " AND A.year ='" + search_data.year + "'";

                if (!string.IsNullOrEmpty(search_data.projecthead) && search_data.projecthead != "YWxs")
                {
                    search_data.projecthead = Encoding.UTF8.GetString(Convert.FromBase64String(search_data.projecthead));
                    sql += " AND A.project_head ='" + search_data.projecthead + "'";
                }

                if (!string.IsNullOrEmpty(search_data.accepttype) && search_data.accepttype.ToLower() != "all")
                    sql += " AND A.project_type ='" + search_data.accepttype + "'";

                if (!string.IsNullOrEmpty(search_data.faculty) && search_data.faculty.ToLower() != "all")
                    sql += " AND B.faculty ='" + search_data.faculty + "'";

                if (!string.IsNullOrEmpty(search_data.acronyms) && search_data.acronyms.ToLower() != "all")
                    sql += " AND A.acronyms ='" + search_data.acronyms + "'";

                if (!string.IsNullOrEmpty(search_data.risk) && search_data.risk.ToLower() != "all")
                    sql += " AND A.risk_type LIKE'%" + search_data.risk + "%'";
            }

            sql += " ORDER BY A.trans_id ASC ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        int row_count = 1;
                        IList<ModelMenuHome1ReportData> e = new List<ModelMenuHome1ReportData>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuHome1ReportData item = new ModelMenuHome1ReportData();
                            item.project_request_id = reader["project_request_id"].ToString();
                            item.project_name_thai = reader["project_name_thai"].ToString();
                            item.project_name_eng = reader["project_name_eng"].ToString();
                            item.project_number = reader["project_number"].ToString();
                            item.project_head_name = reader["project_head_name"].ToString();
                            item.acronyms = reader["acronyms"].ToString();
                            item.risk_type = reader["risk_type"].ToString();
                            item.delivery_online_date = reader["delivery_online_date"].ToString();
                            item.review_request_date = reader["review_request_date"].ToString();
                            item.result_doc_review = reader["result_doc_review"].ToString();
                            item.committee_assign_date = reader["committee_assign_date"].ToString();
                            item.committee_name_array = (user_permission.groupcode == "G002" ? user_permission.fullname : reader["committee_name_array"].ToString());

                            string comment_date = "";
                            if (user_permission.groupcode == "G002")
                                comment_date = await GetCommentDataAsync(reader["project_number"].ToString(), user_permission.groupcode, search_data.userid);

                            item.committee_comment_date = (user_permission.groupcode == "G002" ? comment_date : reader["committee_comment_date"].ToString());
                            item.meeting_date = reader["meeting_date"].ToString();
                            item.meeting_approval_date = reader["meeting_approval_date"].ToString();

                            string consider_result = reader["consider_result"].ToString() + (!string.IsNullOrEmpty(reader["consider_result"].ToString()) ? " (" + reader["safety_type"].ToString() + ")" : "");
                            item.consider_result = (reader["safety_type"].ToString() == "5" ? "-" : consider_result);

                            item.alert_date = reader["alert_date"].ToString();
                            item.request_edit_meeting_date = reader["request_edit_meeting_date"].ToString(); /////
                            item.request_edit_date = reader["request_edit_date"].ToString();
                            item.report_status_date = reader["report_status_date"].ToString();
                            item.certificate_expire_date = reader["certificate_expire_date"].ToString();
                            item.request_renew_date = reader["request_renew_date"].ToString();
                            item.close_project_date = reader["close_project_date"].ToString(); /////
                            item.print_certificate_date = reader["print_certificate_date"].ToString(); /////
                            e.Add(item);
                            row_count++;
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }



        public async Task<ModelMenuHome2_InterfaceData> MenuHome2InterfaceDataAsync(string RegisterId)
        {
            ModelMenuHome2_InterfaceData resp = new ModelMenuHome2_InterfaceData();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYear = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYear.Add(year_current);

            for (int i = 1; i < 5; i++)
            {
                ModelSelectOption year_next = new ModelSelectOption();
                year_next.value = (thai_year + i).ToString();
                year_next.label = (thai_year + i).ToString();
                resp.ListYear.Add(year_next);
            }
            ModelSelectOption all_year = new ModelSelectOption();
            all_year.value = "All";
            all_year.label = "เลือก...";
            resp.ListYear.Add(all_year);

            //------------------------------------------------------------------------------------------
            resp.ListAcceptType = new List<ModelSelectOption>();
            ModelSelectOption accept_type_1 = new ModelSelectOption();
            accept_type_1.value = "1";
            accept_type_1.label = "แบบประเมินเบื้องต้นสำหรับห้องปฏิบัติการ";
            resp.ListAcceptType.Add(accept_type_1);
            ModelSelectOption accept_type_2 = new ModelSelectOption();
            accept_type_2.value = "2";
            accept_type_2.label = "แบบประเมินเบื้องต้นสำหรับโรงเรือนทดลองสำหรับพืชดัดแปลงพันธุกรรม";
            resp.ListAcceptType.Add(accept_type_2);
            ModelSelectOption accept_type_all = new ModelSelectOption();
            accept_type_all.value = "ALL";
            accept_type_all.label = "เลือก...";
            resp.ListAcceptType.Add(accept_type_all);

            //------------------------------------------------------------------------------------------
            resp.ListReportData = new List<ModelMenuHome2ReportData>();
            resp.ListReportData = await GetAllReportDataHome2Async(null);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M002");

            return resp;
        }

        public async Task<ModelMenuA2> GetFileDownloadHome2Async(string project_number)
        {
            string sql = "SELECT B.project_key_number, A.filename1, A.filename2 " +
                        "FROM Doc_MenuA2 A INNER JOIN Doc_MenuB2 B " +
                        "ON A.doc_id = B.project_id " +
                        "WHERE B.project_key_number='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuA2 e = new ModelMenuA2();
                        while (await reader.ReadAsync())
                        {
                            e.filename1 = reader["filename1"].ToString();
                            e.filename2 = reader["filename2"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelMenuHome2ReportData>> GetAllReportDataHome2Async(ModelMenuHome2_InterfaceData search_data)
        {
            string sql = "SELECT ROW_NUMBER() OVER (ORDER BY A.doc_id ASC) as col1, " +
                        "A.room_number as col2, A.faculty_laboratory as col3,  " +
                        "A.department as col4,A.laboratory_address as col5,A.building as col6, " +
                        "A.floor as col7, CONVERT(VARCHAR(10),A.doc_date,103) AS col8, " +
                        "CONVERT(VARCHAR(10),B.doc_date,103) AS col9, C.name_thai as col10, B.project_key_number as col11, " +
                        "CONVERT(VARCHAR(10),B.meeting_date,103) as col12, (F.name_thai + ' ' + E.safety_type + ' ' + F.name_thai_sub) as col13,  " +
                        "CONVERT(VARCHAR(10),D.doc_date,103) as col14, CONVERT(VARCHAR(10),E.doc_date,103) as col15, " +
                        "B.year_of_meeting, A.project_according_type_method " +
                        "FROM Doc_MenuA2 A " +
                        "LEFT OUTER JOIN Doc_MenuB2 B " +
                        "ON A.doc_id = B.project_id " +
                        "LEFT OUTER JOIN MST_InitialResult C " +
                        "ON B.initial_result = C.id " +
                        "LEFT OUTER JOIN Doc_MenuC1_2 D " +
                        "ON B.project_key_number = D.project_number " +
                        "LEFT OUTER JOIN Doc_MenuC2_2 E " +
                        "ON B.project_key_number = E.project_number " +
                        "LEFT OUTER JOIN MST_ApprovalType F " +
                        "ON E.approval_type = F.id " +
                        "WHERE 1=1 ";

            if (search_data != null)
            {
                if (!string.IsNullOrEmpty(search_data.defaultyear) && search_data.defaultyear.ToLower() != "all")
                    sql += " AND B.year_of_meeting ='" + search_data.defaultyear + "'";

                if (!string.IsNullOrEmpty(search_data.defaultaccepttype) && search_data.defaultaccepttype.ToLower() != "all")
                    sql += " AND A.project_according_type_method ='" + search_data.defaultaccepttype + "'";

            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        int row_count = 1;
                        IList<ModelMenuHome2ReportData> e = new List<ModelMenuHome2ReportData>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuHome2ReportData item = new ModelMenuHome2ReportData();
                            item.col1 = row_count.ToString();
                            item.col2 = reader["col2"].ToString();
                            item.col3 = reader["col3"].ToString();
                            item.col4 = reader["col4"].ToString();
                            item.col5 = reader["col5"].ToString();
                            item.col6 = reader["col6"].ToString();
                            item.col7 = reader["col7"].ToString();
                            item.col8 = reader["col8"].ToString();
                            item.col9 = reader["col9"].ToString();
                            item.col10 = reader["col10"].ToString();
                            item.col11 = reader["col11"].ToString();
                            item.col12 = reader["col12"].ToString();
                            item.col13 = reader["col13"].ToString();
                            item.col14 = reader["col14"].ToString();
                            item.col15 = reader["col15"].ToString();
                            e.Add(item);
                            row_count++;
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

    }
}
