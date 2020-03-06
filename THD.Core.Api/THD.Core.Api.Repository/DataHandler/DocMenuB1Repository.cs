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
using THD.Core.Api.Models.ReportModels;

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuB1Repository : IDocMenuB1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMeetingRoundRepository _IDocMeetingRoundRepository;
        public DocMenuB1Repository(IConfiguration configuration,
                                   IDropdownListRepository DropdownListRepository,
                                   IRegisterUserRepository IRegisterUserRepository,
                                   IDocMenuReportRepository DocMenuReportRepository,
                                   IDocMeetingRoundRepository DocMeetingRoundRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
            _IDocMeetingRoundRepository = DocMeetingRoundRepository;
        }

        // B1 -----------------------------------------------------------------------------------

        public async Task<ModelMenuB1_InterfaceData> MenuB1InterfaceDataAsync(string userid, string username)
        {

            ModelMenuB1_InterfaceData resp = new ModelMenuB1_InterfaceData();

            resp.ListProjectHead = new List<ModelSelectOption>();
            resp.ListProjectHead = await GetAllProjectHeadAsync();

            if (resp.ListProjectHead == null)
                resp.ListProjectHead = new List<ModelSelectOption>();

            ModelSelectOption all_project_head = new ModelSelectOption();
            all_project_head.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("all"));
            all_project_head.label = "ทั้งหมด";
            resp.ListProjectHead.Add(all_project_head);

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            resp.defaultyear = thai_year;
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYearOfProject.Add(year_current);

            ModelCountOfYear round_of_year = new ModelCountOfYear();
            round_of_year = await _IDocMeetingRoundRepository.GetMeetingRoundOfProjectAsync(resp.defaultyear);
            resp.defaultround = round_of_year.count;

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M010");


            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectHeadAsync()
        {
            string sql = "SELECT A.project_head, B.first_name, B.full_name " +
                        "FROM Transaction_Document A " +
                        "INNER JOIN RegisterUser B " +
                        "ON A.project_head = B.register_id " +
                        "WHERE A.project_number IS NULL " +
                        "GROUP BY A.project_head, B.first_name, B.full_name " +
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
                            item.label = reader["first_name"].ToString() + " " + reader["full_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllProjectNameThaiAsync(string project_head)
        {

            string sql = "SELECT project_head, doc_id, project_name_thai " +
                         "FROM Doc_MenuA1 WHERE 1=1 AND IsClosed=0 ";

            if (!string.IsNullOrEmpty(project_head) && project_head != "all")
            {
                sql += "AND project_head='" + project_head + "'";
            }

            sql += " ORDER BY project_name_thai ASC";

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
                            item.value = reader["doc_id"].ToString();
                            item.label = reader["project_name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllDownloadFileByProjectIdAsync(string project_Id)
        {

            string sql = "SELECT file1name,file2name,file3name,file4name,file5name FROM Doc_MenuA1 WHERE doc_id='" + project_Id + "'";

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
                            if (!string.IsNullOrEmpty(reader["file1name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file1name"].ToString();
                                item.label = "แบบเสนอเพื่อขอรับการพิจารณารับรองด้านความปลอดภัย";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file2name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file2name"].ToString();
                                item.label = "โครงการวิจัยฉบับสมบูรณ์";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file3name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file3name"].ToString();
                                item.label = "เอกสารชี้แจงรายละเอียดของเชื้อที่ใช้/แบบฟอร์มข้อตกลงการใช้ตัวอย่างชีวภาพ";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file4name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file4name"].ToString();
                                item.label = "หนังสือรับรองและอนุมัติให้ใช้สถานะที่";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file5name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file5name"].ToString();
                                item.label = "อื่นๆ";
                                e.Add(item);
                            }
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuB1_GetDataByProjectNameThai> GetDataByProjectNameThaiAsync(int project_id)
        {

            string sql = "SELECT project_name_thai,project_name_eng,file1name,file2name,file3name,file4name,file5name FROM Doc_MenuA1 WHERE doc_id='" + project_id + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuB1_GetDataByProjectNameThai e = new ModelMenuB1_GetDataByProjectNameThai();
                        e.ListDownloadFile = new List<ModelSelectOption>();
                        while (await reader.ReadAsync())
                        {
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();

                            if (!string.IsNullOrEmpty(reader["file1name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file1name"].ToString();
                                item.label = "แบบเสนอเพื่อขอรับการพิจารณารับรองด้านความปลอดภัย";
                                e.ListDownloadFile.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file2name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file2name"].ToString();
                                item.label = "โครงการวิจัยฉบับสมบูรณ์";
                                e.ListDownloadFile.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file3name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file3name"].ToString();
                                item.label = "เอกสารชี้แจงรายละเอียดของเชื้อที่ใช้/แบบฟอร์มข้อตกลงการใช้ตัวอย่างชีวภาพ";
                                e.ListDownloadFile.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file4name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file4name"].ToString();
                                item.label = "หนังสือรับรองและอนุมัติให้ใช้สถานะที่";
                                e.ListDownloadFile.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file5name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file5name"].ToString();
                                item.label = "อื่นๆ";
                                e.ListDownloadFile.Add(item);
                            }
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuB1Data> GetProjectNumberWithDataAsync(string project_number)
        {

            string sql = "SELECT A.project_id, A.accept_type, A.initial_result, A.acronyms, " +
                        "C.full_name AS project_head_name, A.project_key_number, " +
                        "B.project_head, B.faculty_name, B.project_name_thai, B.project_name_eng, " +
                        "D.name_thai AS position_name_thai, E.name_thai AS accepttype_name_thai,  " +
                        "F.name_thai AS initial_name_thai, A.file_download_name, " +
                        "A.round_of_meeting, A.year_of_meeting, A.meeting_date as meeting_set_date, " +
                        "G.meeting_date as conclusion_date " +
                        "FROM Doc_MenuB1 A " +
                        "INNER JOIN Doc_MenuA1 B ON A.project_id = B.doc_id " +
                        "INNER JOIN RegisterUser C ON B.project_head = C.register_id " +
                        "INNER JOIN MST_Position D ON C.position = D.id " +
                        "LEFT JOIN MST_AcceptType E ON A.accept_type = E.id " +
                        "LEFT JOIN MST_InitialResult F ON A.initial_result = F.id " +
                        "INNER JOIN Doc_MenuC3 G ON A.project_key_number = G.agenda_3_project_number " +
                        "WHERE A.IsClosed='0' AND A.project_key_number='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuB1Data e = new ModelMenuB1Data();
                        while (await reader.ReadAsync())
                        {
                            e.accepttype = reader["accept_type"].ToString();
                            e.accepttypenamethai = reader["accepttype_name_thai"].ToString();
                            e.projecthead = reader["project_head"].ToString();
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.projectid = reader["project_id"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.positionnamethai = "หัวหน้าโครงการ";// reader["position_name_thai"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                            e.acronyms = reader["acronyms"].ToString();
                            e.initialresult = reader["initial_result"].ToString();
                            e.initialnamethai = reader["initial_name_thai"].ToString();
                            e.filedownloadname = reader["file_download_name"].ToString();
                            e.projectkeynumber = reader["project_key_number"].ToString();
                            e.roundofmeeting = Convert.ToInt32(reader["round_of_meeting"]);
                            e.yearofmeeting = Convert.ToInt32(reader["year_of_meeting"]);
                            e.meetingsetdate = Convert.ToDateTime(reader["meeting_set_date"]).ToString("dd/MM/yyyy");
                            e.conclusiondate = Convert.ToDateTime(reader["conclusion_date"]).ToString("dd/MM/yyyy");
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseMessageAddDocB1> AddDocMenuB1Async(ModelMenuB1 model)
        {

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseMessageAddDocB1 resp = new ModelResponseMessageAddDocB1();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_b1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string project_head = Encoding.UTF8.GetString(Convert.FromBase64String(model.projecthead));

                    cmd.Parameters.Add("@doc_date", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                    cmd.Parameters.Add("@project_head", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(project_head);
                    cmd.Parameters.Add("@project_id", SqlDbType.Int).Value = ParseDataHelper.ConvertDBNull(model.projectid);
                    cmd.Parameters.Add("@project_name_thai", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                    cmd.Parameters.Add("@project_name_eng", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                    cmd.Parameters.Add("@acronyms", SqlDbType.VarChar, 3).Value = ParseDataHelper.ConvertDBNull(model.acronyms);
                    cmd.Parameters.Add("@initial_result", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.initialresult);
                    cmd.Parameters.Add("@file_download_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filedownloadname);
                    cmd.Parameters.Add("@project_key_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectkeynumber);
                    cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.notes);
                    cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                    cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                    cmd.Parameters.Add("@year_of_running", SqlDbType.Int).Value = model.defaultyear;
                    cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                    cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                    SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                    rStatus.Direction = ParameterDirection.Output;
                    SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                    rMessage.Direction = ParameterDirection.Output;
                    SqlParameter rDocId = cmd.Parameters.Add("@rDocId", SqlDbType.Int);
                    rDocId.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    if ((int)cmd.Parameters["@rStatus"].Value > 0)
                    {
                        resp.Status = true;
                        resp.DocNumber = (string)cmd.Parameters["@rMessage"].Value;
                        resp.DocId = (int)cmd.Parameters["@rDocId"].Value;

                        model_rpt_8_file rpt = await _IDocMenuReportRepository.GetReportR8Async((int)cmd.Parameters["@rDocId"].Value, "B1");

                        resp.filename = rpt.filename;
                        resp.filebase64 = rpt.filebase64;

                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }


        // Edit B1 Data Mode --------------------------------------------------------------------

        public async Task<ModelMenuB1_InterfaceData> MenuB1InterfaceDataEditAsync(string project_number, string userid, string username)
        {

            ModelMenuB1_InterfaceData resp = new ModelMenuB1_InterfaceData();

            // Edit Mode
            resp.editdata = new ModelMenuB1Edit();
            resp.editdata = await GetEditDataB1Async(project_number);

            resp.ListProjectHead = new List<ModelSelectOption>();
            ModelSelectOption user_login = new ModelSelectOption();
            user_login.value = resp.editdata.projecthead;
            user_login.label = resp.editdata.projectheadname;
            resp.ListProjectHead.Add(user_login);

            resp.defaultusername = resp.editdata.projectheadname;
            resp.defaultuserid = resp.editdata.projecthead;


            int thai_year = CommonData.GetYearOfPeriod();
            resp.ListYearOfProject = new List<ModelSelectOption>();
            resp.defaultyear = thai_year;
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYearOfProject.Add(year_current);


            resp.ListProjectNameThai = new List<ModelSelectOption>();
            ModelSelectOption project_name_default = new ModelSelectOption()
            {
                value = resp.editdata.projectid,
                label = resp.editdata.projectnamethai,
            };
            resp.ListProjectNameThai.Add(project_name_default);

            resp.ListDownloadFile = new List<ModelSelectOption>();
            resp.ListDownloadFile = await GetAllDownloadFileByProjectIdAsync(resp.editdata.projectid);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M010");

            return resp;
        }

        public async Task<ModelMenuB1Edit> GetEditDataB1Async(string project_number)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            string sql = "SELECT TOP(1)*, A1.project_name_thai, A1.committee_code_array, " +
                        "(MST1.name_thai) AS initial_result_name, (Users.first_name + ' ' + Users.full_name) as project_head_name " +
                        "FROM Doc_MenuB1 B1 " +
                        "LEFT OUTER JOIN Transaction_Document A1 ON B1.project_id = A1.project_request_id " +
                        "LEFT OUTER JOIN MST_InitialResult MST1 ON B1.initial_result = MST1.id " +
                        "LEFT OUTER JOIN RegisterUser Users ON B1.project_head = Users.register_id " +
                        "WHERE project_key_number='" + project_number + "' " +
                        "ORDER BY B1.doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuB1Edit e = new ModelMenuB1Edit();
                        while (await reader.ReadAsync())
                        {
                            e.docid = reader["doc_id"].ToString();
                            e.docdate = Convert.ToDateTime(reader["doc_date"]);
                            e.accepttype = reader["accept_type"].ToString();
                            e.projecthead = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["project_head"].ToString()));
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.projectid = reader["project_id"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.acronyms = reader["acronyms"].ToString();
                            e.initialresult = reader["initial_result"].ToString();
                            e.initialresultname = reader["initial_result_name"].ToString();
                            e.filedownloadnametitle = reader["file_download_name_title"].ToString();
                            e.filedownloadname = reader["file_download_name"].ToString();
                            e.projectkeynumber = reader["project_key_number"].ToString();
                            e.notes = reader["notes"].ToString();
                            e.roundofmeeting = reader["round_of_meeting"].ToString();
                            e.yearofmeeting = reader["year_of_meeting"].ToString();
                            e.defaultyear = reader["year_of_meeting"].ToString();
                            e.meetingdate = Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy");
                            e.editenable = (string.IsNullOrEmpty(reader["committee_code_array"].ToString()) ? true : false);
                            e.createby = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["create_by"].ToString()));
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseMessageAddDocB1> UpdateDocMenuB1Async(ModelMenuB1Edit model)
        {
            ModelResponseMessageAddDocB1 resp = new ModelResponseMessageAddDocB1();
            try
            {
                var cultureInfo = new CultureInfo("en-GB");
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_b1_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string project_head = Encoding.UTF8.GetString(Convert.FromBase64String(model.projecthead));

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@doc_date", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                        cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                        cmd.Parameters.Add("@project_head", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(project_head);
                        cmd.Parameters.Add("@project_id", SqlDbType.Int).Value = ParseDataHelper.ConvertDBNull(model.projectid);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@acronyms", SqlDbType.VarChar, 3).Value = ParseDataHelper.ConvertDBNull(model.acronyms);
                        cmd.Parameters.Add("@initial_result", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.initialresult);
                        cmd.Parameters.Add("@file_download_name_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.filedownloadnametitle);
                        cmd.Parameters.Add("@file_download_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filedownloadname);
                        cmd.Parameters.Add("@project_key_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectkeynumber);
                        cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.notes);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@year_of_running", SqlDbType.Int).Value = model.defaultyear;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;
                            resp.DocNumber = (string)cmd.Parameters["@rMessage"].Value;

                            model_rpt_8_file rpt = await _IDocMenuReportRepository.GetReportR8Async(Convert.ToInt32(model.docid), "B1");

                            resp.filename = rpt.filename;
                            resp.filebase64 = rpt.filebase64;
                        }
                        else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resp;
        }


        // B1_2 ----------------------------------------------------------------------------------
        public async Task<ModelMenuB1_2_InterfaceData> MenuB1_2InterfaceDataAsync(string userid, string username)
        {

            ModelMenuB1_2_InterfaceData resp = new ModelMenuB1_2_InterfaceData();

            resp.ListProjectNumber = new List<ModelSelectOption>();
            resp.ListProjectNumber = await GetAllProjectNumberAsync();

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            resp.defaultyear = thai_year;
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYearOfProject.Add(year_current);

            ModelCountOfYear round_of_year = new ModelCountOfYear();
            round_of_year = await _IDocMeetingRoundRepository.GetMeetingRoundOfProjectAsync(resp.defaultyear);
            resp.defaultround = round_of_year.count;

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M038");


            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectNumberAsync()
        {
            string sql = "SELECT project_number, project_name_thai, project_name_eng " +
                         "FROM [dbo].[Doc_Process] " +
                         "WHERE doc_process_from='A4' AND doc_process_to='B1_2'";

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
                            item.value = reader["project_number"].ToString();
                            item.label = reader["project_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuB1_2_GetDataByProjectNumber> GetB1_2ProjectNumberDataAsync(string project_number)
        {
            string sql = "SELECT TOP(1) MAX(doc_id) as doc_id, project_name_thai, project_name_eng, file1name " +
                         "FROM [dbo].[Doc_MenuA4] " +
                         "WHERE project_number ='" + project_number + "' " +
                         "GROUP BY project_name_thai, project_name_eng, file1name";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuB1_2_GetDataByProjectNumber e = new ModelMenuB1_2_GetDataByProjectNumber();
                        while (await reader.ReadAsync())
                        {
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();

                            if (!string.IsNullOrEmpty(reader["file1name"].ToString()))
                            {
                                e.ListDownloadFile = new List<ModelSelectOption>();
                                e.ListDownloadFile.Add(new ModelSelectOption() { value = reader["file1name"].ToString(), label = "เอกสารแนบคำขอชี้แจง/แก้ไขโครงการตามมติคณะกรรมการ" });
                            }
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseMessageAddDocB1_2> AddDocMenuB1_2Async(ModelMenuB1_2 model)
        {

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseMessageAddDocB1_2 resp = new ModelResponseMessageAddDocB1_2();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_b1_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@initial_result", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.initialresult);
                        cmd.Parameters.Add("@file_download_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filedownloadname);
                        cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.notes);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;
                        SqlParameter rDocId = cmd.Parameters.Add("@rDocId", SqlDbType.Int);
                        rDocId.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;
                            resp.DocNumber = (string)cmd.Parameters["@rMessage"].Value;
                            resp.DocId = (int)cmd.Parameters["@rDocId"].Value;

                            //model_rpt_8_file rpt = await _IDocMenuReportRepository.GetReportR8Async((int)cmd.Parameters["@rDocId"].Value, "B1_2");

                            //resp.filename = rpt.filename;
                            //resp.filebase64 = rpt.filebase64;

                        }
                        else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }
    }
}
