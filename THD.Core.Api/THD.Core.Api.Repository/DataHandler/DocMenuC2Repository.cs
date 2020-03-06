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
    public class DocMenuC2Repository : IDocMenuC2Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;
        private readonly IDocMeetingRoundRepository _IDocMeetingRoundRepository;

        public DocMenuC2Repository(
            IConfiguration configuration,
            IDropdownListRepository DropdownListRepository,
            IDocMenuReportRepository DocMenuReportRepository,
            IRegisterUserRepository IRegisterUserRepository,
            IDocMeetingRoundRepository DocMeetingRoundRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
            _IDocMeetingRoundRepository = DocMeetingRoundRepository;
        }

        #region C2

        public async Task<ModelMenuC2_InterfaceData> MenuC2InterfaceDataAsync(string userid, string username)
        {

            ModelMenuC2_InterfaceData resp = new ModelMenuC2_InterfaceData();

            resp.ListAssigner = new List<ModelSelectOption>();
            ModelSelectOption assigner_login = new ModelSelectOption();
            assigner_login.value = userid;
            assigner_login.label = username + " (เช้าสู่ระบบ)";
            resp.default_assigner_name = assigner_login.label;
            resp.default_assigner_seq = "0"; //Default 0 ไม่มีผล
            resp.ListAssigner.Add(assigner_login);

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

            resp.ListProjectNumber = new List<ModelSelectOption>();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M012");

            resp.ListProjectNumber = await GetAllProjectAsync(userid, "C2");

            resp.ListSafetyType = new List<ModelSelectOption>();

            resp.ListSafetyType = null;

            resp.ListApprovalType = new List<ModelSelectOption>();

            resp.ListApprovalType = null;

            return resp;
        }

        public async Task<ModelMenuC2Data> GetProjectNumberWithDataC2Async(string project_number)
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
                        ModelMenuC2Data e = new ModelMenuC2Data();
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

        public async Task<IList<ModelSelectOption>> GetAllAssignerUserAsync()
        {

            string sql = "SELECT board_code_array FROM Doc_MenuC1 ";

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
                            List<ModelSelectOption> item_list = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());

                            if (item_list != null && item_list.Count > 0)
                            {
                                int ir = 1;
                                foreach (var item in item_list)
                                {
                                    item.label = ir.ToString() + ". " + item.label;
                                    e.Add(item);
                                    ir++;
                                }
                            }

                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuC2Data> GetRegisterUserDataC2Async(string register_id)
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
                        ModelMenuC2Data e = new ModelMenuC2Data();
                        while (await reader.ReadAsync())
                        {
                            e.registerid = reader["register_id"].ToString();
                            e.fullname = reader["first_name"].ToString() + " " + reader["full_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                            e.ListProjectNumber = await GetAllProjectAsync(register_id, "C2,C34");
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseC2Message> AddDocMenuC2Async(ModelMenuC2 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC2Message resp = new ModelResponseC2Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    IList<ModelSelectOption> list_assign_seq = new List<ModelSelectOption>();
                    int assig_seq = 0;

                    string sqlA = "SELECT board_code_array FROM Doc_MenuC1 WHERE project_number='" + model.projectnumber + "'";

                    using (SqlCommand cmdA = new SqlCommand(sqlA, conn))
                    {
                        SqlDataReader reader = await cmdA.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                list_assign_seq = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());
                            }
                            if (list_assign_seq != null && list_assign_seq.Count > 0)
                            {
                                int seq = 1;
                                foreach (var item in list_assign_seq)
                                {
                                    if (item.value == model.assignercode) assig_seq = seq;
                                    seq++;
                                }
                            }
                        }
                        reader.Close();
                    }




                    StringBuilder array_comment_date = new StringBuilder();

                    string sqlB = "SELECT committee_comment_date FROM Transaction_Document WHERE project_number='" + model.projectnumber + "'";

                    using (SqlCommand cmdB = new SqlCommand(sqlB, conn))
                    {
                        SqlDataReader reader = await cmdB.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                array_comment_date.AppendLine(reader["committee_comment_date"].ToString().Trim());

                                array_comment_date.AppendLine("คนที่ " + assig_seq.ToString() + ". " + DateTime.Now.ToString("dd/MM/yyyy"));
                            }
                        }
                        reader.Close();
                    }




                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                        cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                        cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                        cmd.Parameters.Add("@assigner_seq", SqlDbType.Int).Value = ParseDataHelper.ConvertDBNull(model.assignerseq);
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@safety_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.safetytype);
                        cmd.Parameters.Add("@approval_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.approvaltype);
                        cmd.Parameters.Add("@comment_consider", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.commentconsider);
                        cmd.Parameters.Add("@committee_comment_date", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(array_comment_date.ToString());
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
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

                            model_rpt_10_file rpt = await _IDocMenuReportRepository.GetReportR10Async((int)cmd.Parameters["@rDocId"].Value);

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

        #endregion

        #region C2 Edit

        public async Task<ModelMenuC2_InterfaceData> MenuC2InterfaceDataEditAsync(int docid, string userid, string username)
        {

            ModelMenuC2_InterfaceData resp = new ModelMenuC2_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M012");

            resp.editdata = new ModelMenuC2();
            resp.editdata = await GetMenuC2DataEditAsync(docid, userid, resp.UserPermission);

            resp.ListAssigner = new List<ModelSelectOption>();

            ModelSelectOption assigner_login = new ModelSelectOption();
            assigner_login.value = resp.editdata.assignercode;
            assigner_login.label = resp.editdata.assignername;

            resp.default_assigner_name = assigner_login.label;
            resp.default_assigner_seq = "0"; //Default 0 ไม่มีผล
            resp.ListAssigner.Add(assigner_login);

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
            resp.defaultround = Convert.ToInt32(resp.editdata.roundofmeeting);
            resp.ListYearOfProject.Add(year_current);

            return resp;
        }

        private async Task<ModelMenuC2> GetMenuC2DataEditAsync(int docid, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            string sql = "SELECT TOP(1) A.*, (D.first_name + ' ' + D.full_name) AS assigner_name, B.name_thai AS safety_type_name, " +
                        "(C.name_thai + ' ' + C.name_thai_sub) AS approval_type_name, E.meeting_date " +
                        "FROM [dbo].[Doc_MenuC2] A " +
                        "LEFT OUTER JOIN[dbo].[MST_Safety] B ON A.safety_type = B.id " +
                        "LEFT OUTER JOIN[dbo].[MST_ApprovalType] C ON A.approval_type = C.id " +
                        "LEFT OUTER JOIN[dbo].[RegisterUser] D ON A.assigner_code = D.register_id " +
                        "LEFT OUTER JOIN Transaction_Document E ON A.project_number = E.project_number " +
                        "WHERE A.doc_id='" + docid + "' " +
                        (permission.alldata == true ? "" : " AND A.create_by = '" + user_id + "'") +
                        "ORDER BY A.doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC2 e = new ModelMenuC2();
                        while (await reader.ReadAsync())
                        {
                            string assigner_code = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["assigner_code"].ToString()));

                            e.docid = reader["doc_id"].ToString();
                            e.assignercode = assigner_code;
                            e.assignername = reader["assigner_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                            e.assignerseq = reader["assigner_seq"].ToString();
                            e.projectnumber = reader["project_number"].ToString();
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.safetytype = reader["safety_type"].ToString();
                            e.safetytypename = reader["safety_type_name"].ToString();
                            e.approvaltype = reader["approval_type"].ToString();
                            e.approvaltypename = reader["approval_type_name"].ToString();
                            e.commentconsider = reader["comment_consider"].ToString();
                            e.roundofmeeting = reader["round_of_meeting"].ToString();
                            e.yearofmeeting = reader["year_of_meeting"].ToString();
                            e.createby = reader["create_by"].ToString();

                            //Default Edit False
                            e.editenable = false;
                            if (permission.edit == true)
                            {
                                if (string.IsNullOrEmpty(reader["meeting_date"].ToString()))
                                {
                                    if (user_id == reader["create_by"].ToString())
                                    {
                                        e.editenable = true;
                                    }
                                }
                            }

                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseC2Message> UpdateDocMenuC2EditAsync(ModelMenuC2 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC2Message resp = new ModelResponseC2Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c2_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                        cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                        cmd.Parameters.Add("@assigner_seq", SqlDbType.Int).Value = ParseDataHelper.ConvertDBNull(model.assignerseq);
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@safety_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.safetytype);
                        cmd.Parameters.Add("@approval_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.approvaltype);
                        cmd.Parameters.Add("@comment_consider", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.commentconsider);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            model_rpt_10_file rpt = await _IDocMenuReportRepository.GetReportR10Async(Convert.ToInt32(model.docid));

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

        #endregion

        #region C2_2

        public async Task<ModelMenuC22_InterfaceData> MenuC22InterfaceDataAsync(string userid, string username)
        {
            ModelMenuC22_InterfaceData resp = new ModelMenuC22_InterfaceData();

            resp.ListAssigner = new List<ModelSelectOption>();

            ModelSelectOption assigner_login = new ModelSelectOption();
            int assigner_count = (resp.ListAssigner.Count + 1);
            assigner_login.value = userid;
            assigner_login.label = assigner_count.ToString() + ". " + username + " (เช้าสู่ระบบ)";
            resp.default_assigner_name = assigner_login.label;
            resp.default_assigner_seq = assigner_count.ToString();
            resp.ListAssigner.Add(assigner_login);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(userid, "M012");

            resp.ListProjectNumber = new List<ModelSelectOption>();

            resp.ListProjectNumber = await GetAllProjectLabAsync(userid, "C22");

            resp.ListSafetyType = new List<ModelSelectOption>();

            resp.ListSafetyType = null;

            resp.ListApprovalType = new List<ModelSelectOption>();

            resp.ListApprovalType = null;

            return resp;
        }

        public async Task<ModelMenuC22Data> GetProjectNumberWithDataC22Async(string project_number)
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
                        ModelMenuC22Data e = new ModelMenuC22Data();
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

        public async Task<IList<ModelSelectOption>> GetAllAssignerUserC22Async()
        {

            string sql = "SELECT board_code_array FROM Doc_MenuC1_2";

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
                            List<ModelSelectOption> item_list = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());

                            if (item_list != null && item_list.Count > 0)
                            {
                                int ir = 1;
                                foreach (var item in item_list)
                                {
                                    item.label = ir.ToString() + ". " + item.label;
                                    e.Add(item);
                                    ir++;
                                }
                            }

                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuC22Data> GetRegisterUserDataC22Async(string register_id)
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
                        ModelMenuC22Data e = new ModelMenuC22Data();
                        while (await reader.ReadAsync())
                        {
                            e.registerid = reader["register_id"].ToString();
                            e.fullname = reader["first_name"].ToString() + " " + reader["full_name"].ToString();
                            e.positionname = reader["position_name"].ToString();
                            e.ListProjectNumber = await GetAllProjectLabAsync(register_id, "C22,C34");
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseC22Message> AddDocMenuC22Async(ModelMenuC22 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC22Message resp = new ModelResponseC22Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c2_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        string assigner_code = Encoding.UTF8.GetString(Convert.FromBase64String(model.assignercode));

                        cmd.Parameters.Add("@assigner_code", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(assigner_code);
                        cmd.Parameters.Add("@position_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.positionname);
                        cmd.Parameters.Add("@assigner_seq", SqlDbType.Int).Value = ParseDataHelper.ConvertDBNull(model.assignerseq);
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@lab_type_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.labtypename);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@safety_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.safetytype);
                        cmd.Parameters.Add("@approval_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.approvaltype);
                        cmd.Parameters.Add("@comment_consider", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.commentconsider);

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;
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


        public async Task<IList<ModelSelectOption>> GetAllProjectAsync(string AssignerCode, string DocProcess)
        {
            string assign_code = Encoding.UTF8.GetString(Convert.FromBase64String(AssignerCode));

            string sql = "SELECT A.project_number, " +
                        "A.project_name_thai, A.project_name_eng, B.board_code_array " +
                        "FROM [dbo].[Doc_Process] A " +
                        "INNER JOIN [dbo].[Doc_MenuC1] B " +
                        "ON A.project_number = B.[project_number] " +
                        "WHERE A.project_type='PROJECT' " +
                        "AND ISNULL(A.user_comment_array,'') NOT LIKE '%" + assign_code + "%' " +
                        "AND A.doc_process_to='" + DocProcess + "'";

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
                            List<ModelSelectOption> item_list = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());

                            if (item_list != null && item_list.Count > 0)
                            {
                                if (string.IsNullOrEmpty(AssignerCode))
                                {
                                    ModelSelectOption item = new ModelSelectOption();
                                    item.value = reader["project_number"].ToString();
                                    item.label = reader["project_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                                    e.Add(item);
                                    //goto IsHasData;
                                }
                                else
                                {
                                    var get_assign = item_list.Where(ee => ee.value == AssignerCode).ToList();
                                    if (get_assign != null && get_assign.Count > 0)
                                    {
                                        ModelSelectOption item = new ModelSelectOption();
                                        item.value = reader["project_number"].ToString();
                                        item.label = reader["project_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                                        e.Add(item);
                                        //goto IsHasData;
                                    }
                                }
                            }
                        }
                        //IsHasData:
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllProjectLabAsync(string AssignerCode, string DocProcess)
        {
            string sql = "SELECT A.project_number, " +
                        "A.project_name_thai, A.project_name_eng, B.board_code_array " +
                        "FROM [dbo].[Doc_Process] A " +
                        "INNER JOIN [dbo].[Doc_MenuC1_2] B " +
                        "ON A.project_number = B.[project_number] " +
                        "WHERE A.project_type='LAB' AND doc_process_to='" + DocProcess + "'";

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
                            List<ModelSelectOption> item_list = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());

                            if (item_list != null && item_list.Count > 0)
                            {
                                if (string.IsNullOrEmpty(AssignerCode))
                                {
                                    ModelSelectOption item = new ModelSelectOption();
                                    item.value = reader["project_number"].ToString();
                                    item.label = reader["project_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                                    e.Add(item);
                                    goto IsHasData;
                                }
                                else
                                {
                                    var get_assign = item_list.Where(ee => ee.value == AssignerCode).ToList();
                                    if (get_assign != null && get_assign.Count > 0)
                                    {
                                        ModelSelectOption item = new ModelSelectOption();
                                        item.value = reader["project_number"].ToString();
                                        item.label = reader["project_number"].ToString() + " : " + reader["project_name_thai"].ToString();
                                        e.Add(item);
                                        goto IsHasData;
                                    }
                                }
                            }
                        }
                        IsHasData:
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }
    }
}
