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
    public class DocMenuD1Repository : IDocMenuD1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;
        public DocMenuD1Repository(
            IConfiguration configuration,
            IRegisterUserRepository IRegisterUserRepository,
            IDocMenuReportRepository DocMenuReportRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
        }

        public async Task<ModelMenuD1_InterfaceData> MenuD1InterfaceDataAsync(string RegisterId)
        {
            ModelMenuD1_InterfaceData resp = new ModelMenuD1_InterfaceData();

            resp.ListProjectNumber = new List<ModelSelectOption>();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M020");

            if (resp.UserPermission != null && resp.UserPermission.alldata == true)
            {
                resp.ListProjectNumber = await GetAllProjectForD1Async("", "");
            }
            else
            {
                resp.ListProjectNumber = await GetAllProjectForD1Async(RegisterId, "");
            }
            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectForD1Async(string AssignerCode, string DocProcess)
        {

            string sql = "SELECT * FROM [dbo].[Doc_Process] " +
                        "WHERE project_number IS NOT NULL AND project_type='PROJECT' AND doc_process_to IN('A3,A5,A6,A7') AND is_hold=0 ";

            if (!string.IsNullOrEmpty(AssignerCode))
            {
                sql += " AND project_by='" + Encoding.UTF8.GetString(Convert.FromBase64String(AssignerCode)) + "' ";
            }

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

        public async Task<ModelMenuD1ProjectNumberData> GetProjectNumberWithDataD1Async(string project_number)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_getdata_for_d1", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            ModelMenuD1ProjectNumberData e = new ModelMenuD1ProjectNumberData();
                            while (await reader.ReadAsync())
                            {
                                e.projectnamethai = reader[1].ToString();
                                e.projectnameeng = reader[2].ToString();
                                e.projectheadname = reader[3].ToString();
                                e.facultyname = reader[4].ToString();
                                e.positionname = reader[5].ToString();
                                e.certificatetype = reader[6].ToString();
                                e.acceptprojectno = reader[8].ToString();
                                e.advisorsnamethai = "";
                                e.accepttypenamethai = reader[6].ToString();
                                e.dateofapproval = Convert.ToDateTime(reader[7]).ToString("dd/MM/yyyy");
                            }
                            e.listRenewDate = new List<ModelMenuD1RenewTable>();
                            e.listRenewDate = await GetListRenewDateAsync(project_number);
                            return e;
                        }

                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;

        }

        public async Task<IList<ModelMenuD1RenewTable>> GetListRenewDateAsync(string project_number)
        {

            string sql = "SELECT * FROM [dbo].[Doc_MenuD1] WHERE project_number='" + project_number + "' ORDER BY doc_id ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMenuD1RenewTable> e = new List<ModelMenuD1RenewTable>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuD1RenewTable item = new ModelMenuD1RenewTable();
                            item.renewround = Convert.ToInt32(reader["RenewRound"]);
                            item.acceptdate = Convert.ToDateTime(reader["AcceptDate"]).ToString("dd/MM/yyyy");
                            item.expiredate = Convert.ToDateTime(reader["ExpireDate"]).ToString("dd/MM/yyyy");
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseD1Message> AddDocMenuD1Async(ModelMenuD1 model)
        {

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseD1Message resp = new ModelResponseD1Message();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_d1", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_date", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@accept_type_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.accepttypenamethai);
                        cmd.Parameters.Add("@advisorsNameThai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.advisorsnamethai);
                        cmd.Parameters.Add("@acceptProjectNo", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.acceptprojectno);
                        cmd.Parameters.Add("@acceptResult", SqlDbType.Int).Value = model.acceptresult;
                        cmd.Parameters.Add("@acceptCondition", SqlDbType.Int).Value = model.acceptcondition;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        // จริงๆไม่ต้องใช้แล้ว ------------------------------------------------------------------------------
                        cmd.Parameters.Add("@acceptDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                        cmd.Parameters.Add("@expireDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                        //---------------------------------------------------------------------------------------------

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

                            model_rpt_9_file rpt = await _IDocMenuReportRepository.GetReportR9Async((int)cmd.Parameters["@rDocId"].Value);

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
                Console.WriteLine(ex.Message);
            }

            return resp;
        }


        #region "Edit"
        public async Task<ModelMenuD1_InterfaceData> MenuD1EditInterfaceDataAsync(string UserId, string ProjectNumber)
        {
            ModelMenuD1_InterfaceData resp = new ModelMenuD1_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M020");

            resp.editdata = new ModelMenuD1();
            resp.editdata = await GetMenuD1DataEditAsync(ProjectNumber, UserId, resp.UserPermission);

            resp.ListProjectNumber = new List<ModelSelectOption>();
            ModelSelectOption project_name_default = new ModelSelectOption()
            {
                value = resp.editdata.projectnumber,
                label = resp.editdata.projectnumber + " : " + resp.editdata.projectnamethai,
            };
            resp.ListProjectNumber.Add(project_name_default);

            return resp;
        }

        private async Task<ModelMenuD1> GetMenuD1DataEditAsync(string ProjectNumber, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            string sql = "SELECT TOP(1) A.*, B.name_thai as accept_result_name, " +
                        "(CASE WHEN A.acceptCondition = 1 THEN 'แบบปีต่อปี' ELSE 'ไม่มีวันหมอายุ' END) as accept_condition_name " +
                        "FROM Doc_MenuD1 A " +
                        "LEFT OUTER JOIN MST_AcceptResult B ON A.acceptResult = B.id " +
                        "WHERE project_number = '" + ProjectNumber + "' ORDER BY doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuD1 e = new ModelMenuD1();
                        while (await reader.ReadAsync())
                        {
                            e.docid = reader["doc_id"].ToString();
                            e.projectnumber = reader["project_number"].ToString();
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                            e.advisorsnamethai = reader["advisorsNameThai"].ToString();
                            e.acceptprojectno = reader["acceptProjectNo"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.accepttypenamethai = reader["accept_type_name"].ToString();
                            e.acceptresult = Convert.ToInt16(reader["acceptResult"]);
                            e.acceptresultname = reader["accept_result_name"].ToString();
                            e.acceptcondition = Convert.ToInt16(reader["acceptCondition"]);
                            e.acceptconditionname = reader["accept_condition_name"].ToString();
                            e.acceptdate = Convert.ToDateTime(reader["AcceptDate"]).ToString("dd/MM/yyyy");
                            e.createby = reader["create_by"].ToString();
                        }
                        e.listRenewDate = new List<ModelMenuD1RenewTable>();
                        e.listRenewDate = await GetListRenewDateAsync(ProjectNumber);

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (user_id == e.createby)
                            {
                                e.editenable = true;
                            }
                        }

                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseD1Message> UpdateDocMenuD1EditAsync(ModelMenuD1 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseD1Message resp = new ModelResponseD1Message();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_d1_edit", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                    cmd.Parameters.Add("@doc_date", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                    cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                    cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                    cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                    cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                    cmd.Parameters.Add("@accept_type_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.accepttypenamethai);
                    cmd.Parameters.Add("@advisorsNameThai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.advisorsnamethai);
                    cmd.Parameters.Add("@acceptProjectNo", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.acceptprojectno);
                    cmd.Parameters.Add("@acceptResult", SqlDbType.Int).Value = model.acceptresult;
                    cmd.Parameters.Add("@acceptCondition", SqlDbType.Int).Value = model.acceptcondition;

                    cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                    // จริงๆไม่ต้องใช้แล้ว ------------------------------------------------------------------------------
                    cmd.Parameters.Add("@acceptDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                    cmd.Parameters.Add("@expireDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                    //---------------------------------------------------------------------------------------------

                    SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                    rStatus.Direction = ParameterDirection.Output;
                    SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                    rMessage.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    if ((int)cmd.Parameters["@rStatus"].Value > 0)
                    {
                        resp.Status = true;

                        model_rpt_9_file rpt = await _IDocMenuReportRepository.GetReportR9Async(Convert.ToInt32(model.docid));

                        resp.filename = rpt.filename;
                        resp.filebase64 = rpt.filebase64;
                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }

        #endregion

    }
}
