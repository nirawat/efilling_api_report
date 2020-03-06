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
    public class DocMenuC1Repository : IDocMenuC1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMeetingRoundRepository _IDocMeetingRoundRepository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;
        public DocMenuC1Repository(
            IConfiguration configuration,
            IDropdownListRepository DropdownListRepository,
            IRegisterUserRepository IRegisterUserRepository,
            IDocMenuReportRepository DocMenuReportRepository,
            IDocMeetingRoundRepository DocMeetingRoundRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMeetingRoundRepository = DocMeetingRoundRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
        }

        #region Menu C1

        public async Task<ModelMenuC1_InterfaceData> MenuC1InterfaceDataAsync(string userid, string username)
        {

            ModelMenuC1_InterfaceData resp = new ModelMenuC1_InterfaceData();

            resp.ListAssigner = new List<ModelSelectOption>();
            ModelSelectOption assigner_login = new ModelSelectOption();
            assigner_login.value = userid;
            assigner_login.label = username + " (เช้าสู่ระบบ)";
            resp.default_assigner_name = assigner_login.label;
            resp.ListAssigner.Add(assigner_login);

            resp.ListBoard = new List<ModelSelectOption>();
            resp.ListBoard = await GetAllRegisterUserByCharacterAsync("");

            resp.ListSpecialList = new List<ModelSelectOption>();
            resp.ListSpecialList = await GetAllRegisterUserByCharacterAsync("");

            int thai_year = CommonData.GetYearOfPeriod();
            resp.ListYearOfProject = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.defaultyear = (thai_year);
            resp.ListYearOfProject.Add(year_current);

            ModelCountOfYear round_of_year = new ModelCountOfYear();
            round_of_year = await _IDocMeetingRoundRepository.GetMeetingRoundOfProjectAsync(resp.defaultyear);
            resp.defaultround = round_of_year.count;

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M011");

            resp.ListProjectNumber = new List<ModelSelectOption>();

            if (resp.UserPermission != null && resp.UserPermission.alldata == true)
            {
                resp.ListProjectNumber = await GetAllProjectNumberC1Async(null);
            }
            else
            {
                resp.ListProjectNumber = await GetAllProjectNumberC1Async(userid);
            }


            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectNumberC1Async(string project_header)
        {
            string sql = "SELECT A.project_key_number, B.project_name_thai " +
                        "FROM Doc_MenuB1 A " +
                        "INNER JOIN Doc_Process B " +
                        "ON A.project_key_number = B.project_number " +
                        "WHERE 1=1 AND A.IsIssue=0 ";

            if (!string.IsNullOrEmpty(project_header))
            {
                sql += " AND A.project_head='" + project_header + "' ";
            }

            sql += "GROUP BY A.doc_id, A.project_key_number, A.project_head, B.project_name_thai " +
                    "ORDER BY A.doc_id ASC";

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
                            item.value = reader["project_key_number"].ToString();
                            item.label = reader["project_key_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuC1Data> GetProjectNumberWithDataC1Async(string project_number)
        {

            string sql = "SELECT (C.first_name + C.full_name) AS project_head_name, B.faculty_name, B.project_name_thai, B.project_name_eng " +
                        "FROM Doc_MenuB1 A " +
                        "INNER JOIN Doc_MenuA1 B ON A.project_id = B.doc_id " +
                        "INNER JOIN RegisterUser C ON B.project_head = C.register_id " +
                        "LEFT JOIN MST_AcceptType D ON A.accept_type = D.id " +
                        "WHERE A.project_key_number='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC1Data e = new ModelMenuC1Data();
                        while (await reader.ReadAsync())
                        {
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllProjectHeadAsync()
        {
            string sql = "SELECT A.project_head, B.first_name, B.full_name " +
                        "FROM Doc_MenuB1 A " +
                        "INNER JOIN RegisterUser B " +
                        "ON A.project_head = B.register_id " +
                        "WHERE A.IsIssue = 0 " +
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

        public async Task<IList<ModelSelectOption>> GetAllRegisterUserByCharacterAsync(string RegisterId)
        {

            string sql = "SELECT register_id, (first_name + full_name) as full_name FROM RegisterUser WHERE 1=1 AND IsActive='1' ";

            if (string.IsNullOrEmpty(RegisterId)) sql += "AND Character IN ('2') ";

            if (!string.IsNullOrEmpty(RegisterId)) sql += "AND Character IN ('" + RegisterId + "') ";

            sql += " ORDER BY full_name ASC";

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
                            item.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["register_id"].ToString()));
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

        public async Task<ModelRegisterData> GetRegisterUserDataAsync(string register_id)
        {

            string register_id_ = Encoding.UTF8.GetString(Convert.FromBase64String(register_id));

            string sql = "SELECT A.register_id, A.first_name, A.full_name, A.work_phone, A.mobile, B.name_thai as position_name " +
                         "FROM RegisterUser A INNER JOIN MST_Character B ON A.position = B.id " +
                         "WHERE register_id='" + register_id_ + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelRegisterData e = new ModelRegisterData();
                        while (await reader.ReadAsync())
                        {
                            e.registerid = reader["register_id"].ToString();
                            e.fullname = reader["first_name"].ToString() + " " + reader["full_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<string> GetEmailUserAsync(ModelMenuC1 model)
        {
            if (model != null && model.boardcodearray != null && model.boardcodearray.Count > 0)
            {
                string register_id = string.Empty;

                foreach (var item in model.boardcodearray)
                    register_id += Encoding.UTF8.GetString(Convert.FromBase64String(item.value)) + "','";

                if (!string.IsNullOrEmpty(register_id))
                    register_id = register_id.Remove(register_id.Length - 3, 3).ToString();

                string sql = "SELECT email FROM RegisterUser WHERE register_id IN('" + register_id + "')";

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            string email_list = "";
                            while (await reader.ReadAsync())
                            {
                                email_list += reader["email"].ToString() + ",";
                            }
                            if (!string.IsNullOrEmpty(email_list))
                                email_list = email_list.Remove(email_list.Length - 1, 1).ToString();
                            return email_list;
                        }
                    }
                    conn.Close();
                }
            }
            return string.Empty;

        }

        public async Task<ModelResponseC1Message> AddDocMenuC1Async(ModelMenuC1 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC1Message resp = new ModelResponseC1Message();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                    cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                    cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                    cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                    cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                    cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                    cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                    cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                    cmd.Parameters.Add("@board_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.boardcodearray);
                    cmd.Parameters.Add("@speciallist_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.speciallistcodearray);
                    cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                    cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                    cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                    cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                    int seq = 1;
                    StringBuilder list_committee_code_array = new StringBuilder();
                    StringBuilder list_committee_name_array = new StringBuilder();
                    if (model.boardcodearray != null && model.boardcodearray.Count > 0)
                    {
                        foreach (var item in model.boardcodearray)
                        {
                            list_committee_code_array.AppendLine(Encoding.UTF8.GetString(Convert.FromBase64String(item.value.Trim())) + ", ");
                            list_committee_name_array.AppendLine(seq.ToString() + ". " + item.label.Trim());
                            seq++;
                        }
                    }
                    cmd.Parameters.Add("@committee_code_array", SqlDbType.NVarChar).Value = list_committee_code_array.ToString();
                    cmd.Parameters.Add("@committee_name_array", SqlDbType.NVarChar).Value = list_committee_name_array.ToString();

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

                        resp.EmailArray = await GetEmailUserAsync(model);

                        model_rpt_11_file rpt = await _IDocMenuReportRepository.GetReportR11Async((int)cmd.Parameters["@rDocId"].Value);

                        // คนที่ 1 ----------------------------------------
                        resp.filename_1 = rpt.filename_1;
                        resp.filebase_1_64 = rpt.filebase_1_64;

                        // คนที่ 2 ----------------------------------------
                        resp.filename_2 = rpt.filename_2;
                        resp.filebase_2_64 = rpt.filebase_2_64;

                        // คนที่ 3 ----------------------------------------
                        resp.filename_3 = rpt.filename_3;
                        resp.filebase_3_64 = rpt.filebase_3_64;

                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }

            return resp;
        }

        #endregion

        #region Menu C1 Edit

        public async Task<ModelMenuC1_InterfaceData> MenuC1InterfaceDataEditAsync(string project_number, string RegisterId)
        {
            ModelMenuC1_InterfaceData resp = new ModelMenuC1_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M011");

            resp.editdata = new ModelMenuC1();
            resp.editdata = await GetMenuC1DataEditAsync(project_number, RegisterId, resp.UserPermission);

            resp.ListAssigner = new List<ModelSelectOption>();
            ModelSelectOption assigner_login = new ModelSelectOption();
            assigner_login.value = resp.editdata.assignercode;
            assigner_login.label = resp.editdata.assignername;
            resp.default_assigner_name = assigner_login.label;
            resp.ListAssigner.Add(assigner_login);

            resp.ListBoard = new List<ModelSelectOption>();
            resp.ListBoard = await GetAllRegisterUserByCharacterAsync("");

            resp.ListSpecialList = new List<ModelSelectOption>();
            resp.ListSpecialList = await GetAllRegisterUserByCharacterAsync("");

            resp.ListProjectNumber = new List<ModelSelectOption>();
            ModelSelectOption project_name_default = new ModelSelectOption()
            {
                value = resp.editdata.projectnumber,
                label = resp.editdata.projectnumber + " : " + resp.editdata.projectnamethai,
            };
            resp.ListProjectNumber.Add(project_name_default);

            resp.ListYearOfProject = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = resp.editdata.yearofmeeting;
            year_current.label = resp.editdata.yearofmeeting;
            resp.defaultyear = Convert.ToInt32(resp.editdata.yearofmeeting);
            resp.ListYearOfProject.Add(year_current);

            return resp;
        }

        private async Task<ModelMenuC1> GetMenuC1DataEditAsync(string project_number, string RegisterId, ModelPermissionPage permission)
        {
            string sql = "SELECT TOP(1)* , (b.first_name + B.full_name) AS assigner_name, C.committee_comment_date " +
                        "FROM Doc_MenuC1 A " +
                        "LEFT OUTER JOIN RegisterUser B ON A.assigner_code = B.register_id " +
                        "LEFT OUTER JOIN Transaction_Document C ON A.project_number = C.project_number " +
                        "WHERE A.project_number='" + project_number + "' ORDER BY A.doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC1 e = new ModelMenuC1();
                        while (await reader.ReadAsync())
                        {
                            string assigner_code = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["assigner_code"].ToString()));

                            e.docid = reader["doc_id"].ToString();
                            e.assignercode = assigner_code;
                            e.assignername = reader["assigner_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                            e.accepttype = reader["accept_type"].ToString();
                            e.projectnumber = reader["project_number"].ToString();
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.roundofmeeting = reader["round_of_meeting"].ToString();
                            e.yearofmeeting = reader["year_of_meeting"].ToString();
                            e.meetingdate = Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy");
                            e.createby = reader["create_by"].ToString();

                            //Default Edit False
                            e.editenable = false;
                            if (permission.edit == true)
                            {
                                if (string.IsNullOrEmpty(reader["committee_comment_date"].ToString()))
                                {
                                    string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(RegisterId));
                                    if (user_id == reader["create_by"].ToString())
                                    {
                                        e.editenable = true;
                                    }
                                }
                            }

                            e.boardcodearray = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());
                            e.speciallistcodearray = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["speciallist_code_array"].ToString());

                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseC1Message> UpdateDocMenuC1EditAsync(ModelMenuC1 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC1Message resp = new ModelResponseC1Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c1_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                        cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                        cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@board_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.boardcodearray);
                        cmd.Parameters.Add("@speciallist_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.speciallistcodearray);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        int seq = 1;
                        StringBuilder list_committee_code_array = new StringBuilder();
                        StringBuilder list_committee_name_array = new StringBuilder();
                        if (model.boardcodearray != null && model.boardcodearray.Count > 0)
                        {
                            foreach (var item in model.boardcodearray)
                            {
                                list_committee_code_array.AppendLine(Encoding.UTF8.GetString(Convert.FromBase64String(item.value.Trim())) + ", ");
                                list_committee_name_array.AppendLine(seq.ToString() + ". " + item.label.Trim());
                                seq++;
                            }
                        }
                        cmd.Parameters.Add("@committee_code_array", SqlDbType.NVarChar).Value = list_committee_code_array.ToString();
                        cmd.Parameters.Add("@committee_name_array", SqlDbType.NVarChar).Value = list_committee_name_array.ToString();

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            resp.EmailArray = await GetEmailUserAsync(model);

                            model_rpt_11_file rpt = await _IDocMenuReportRepository.GetReportR11Async(Convert.ToInt32(model.docid));

                            // คนที่ 1 ----------------------------------------
                            resp.filename_1 = rpt.filename_1;
                            resp.filebase_1_64 = rpt.filebase_1_64;

                            // คนที่ 2 ----------------------------------------
                            resp.filename_2 = rpt.filename_2;
                            resp.filebase_2_64 = rpt.filebase_2_64;

                            // คนที่ 3 ----------------------------------------
                            resp.filename_3 = rpt.filename_3;
                            resp.filebase_3_64 = rpt.filebase_3_64;

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

        #endregion

        #region Menu C1_2

        public async Task<ModelMenuC12_InterfaceData> MenuC12InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC12_InterfaceData resp = new ModelMenuC12_InterfaceData();

            resp.ListAssigner = new List<ModelSelectOption>();

            resp.ListAssigner = await GetAllRegisterUserByCharacterC12Async("2");

            resp.ListBoard = new List<ModelSelectOption>();

            resp.ListBoard = await GetAllRegisterUserByCharacterC12Async("2");

            resp.ListProjectNumber = new List<ModelSelectOption>();

            resp.ListProjectNumber = await GetAllProjectNumberC12Async();

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.defaultyear = (thai_year);
            resp.ListYearOfProject.Add(year_current);

            ModelCountOfYear round_of_year = new ModelCountOfYear();
            round_of_year = await _IDocMeetingRoundRepository.GetMeetingRoundOfProjectAsync(resp.defaultyear);
            resp.defaultround = round_of_year.count;

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M011");

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectNumberC12Async()
        {

            string sql = "SELECT project_number, project_name_thai FROM Doc_Process WHERE doc_process_to='C12'";

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

        public async Task<ModelMenuC12Data> GetProjectNumberWithDataC12Async(string project_number)
        {

            string sql = "SELECT project_number, project_name_thai, project_name_eng " +
                         "FROM Doc_Process WHERE project_number='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC12Data e = new ModelMenuC12Data();
                        while (await reader.ReadAsync())
                        {
                            e.labtypename = reader["project_name_thai"].ToString();
                            e.facultyname = reader["project_name_eng"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllRegisterUserByCharacterC12Async(string RegisterId)
        {

            string sql = "SELECT register_id, first_name, full_name FROM RegisterUser WHERE 1=1 AND IsActive='1' ";

            if (string.IsNullOrEmpty(RegisterId)) sql += "AND Character IN ('1','2') ";

            if (!string.IsNullOrEmpty(RegisterId)) sql += "AND Character IN ('" + RegisterId + "') ";

            sql += " ORDER BY full_name ASC";

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
                            item.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["register_id"].ToString()));
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

        public async Task<ModelRegisterDataC12> GetRegisterUserDataC12Async(string register_id)
        {

            string register_id_ = Encoding.UTF8.GetString(Convert.FromBase64String(register_id));

            string sql = "SELECT A.register_id, A.first_name, A.full_name, A.work_phone, A.mobile, B.name_thai as position_name " +
                         "FROM RegisterUser A INNER JOIN MST_Character B ON A.position = B.id " +
                         "WHERE register_id='" + register_id_ + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelRegisterDataC12 e = new ModelRegisterDataC12();
                        while (await reader.ReadAsync())
                        {
                            e.registerid = reader["register_id"].ToString();
                            e.fullname = reader["first_name"].ToString() + " " + reader["full_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<string> GetEmailUserC12Async(ModelMenuC12 model)
        {
            if (model != null && model.boardcodearray != null && model.boardcodearray.Count > 0)
            {
                string register_id = string.Empty;

                foreach (var item in model.boardcodearray)
                    register_id += Encoding.UTF8.GetString(Convert.FromBase64String(item.value)) + "','";

                if (!string.IsNullOrEmpty(register_id))
                    register_id = register_id.Remove(register_id.Length - 3, 3).ToString();

                string sql = "SELECT email FROM RegisterUser WHERE register_id IN('" + register_id + "')";

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            string email_list = "";
                            while (await reader.ReadAsync())
                            {
                                email_list += reader["email"].ToString() + ",";
                            }
                            if (!string.IsNullOrEmpty(email_list))
                                email_list = email_list.Remove(email_list.Length - 1, 1).ToString();
                            return email_list;
                        }
                    }
                    conn.Close();
                }
            }
            return string.Empty;

        }

        public async Task<ModelResponseC12Message> AddDocMenuC12Async(ModelMenuC12 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC12Message resp = new ModelResponseC12Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c1_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                        cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                        cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                        cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@lab_type_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.labtypename);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@board_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.boardcodearray);
                        cmd.Parameters.Add("@speciallist_code_array", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(model.speciallistcodearray);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            resp.EmailArray = await GetEmailUserC12Async(model);

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

        #endregion




    }
}
