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

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuD2Repository : IDocMenuD2Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuD2Repository(IConfiguration configuration, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
        }

        public async Task<ModelMenuD2_InterfaceData> MenuD2InterfaceDataAsync(string RegisterId)
        {
            ModelMenuD2_InterfaceData resp = new ModelMenuD2_InterfaceData();

            resp.ListProjectNumber = new List<ModelSelectOption>();

            int thai_year = CommonData.GetYearOfPeriod();
            resp.defaultyear = thai_year;
            resp.listYearOfMeeting = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.listYearOfMeeting.Add(year_current);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M021");

            if (resp.UserPermission != null && resp.UserPermission.alldata == true)
            {
                resp.ListProjectNumber = await GetAllProjectForD2Async("");
            }
            else
            {
                resp.ListProjectNumber = await GetAllProjectForD2Async(RegisterId);
            }
            return resp;
        }


        public async Task<IList<ModelSelectOption>> GetAllProjectForD2Async(string AssignerCode)
        {

            string sql = "SELECT * FROM [dbo].[Doc_Process] " +
                        "WHERE project_type='PROJECT' AND doc_process_to='D2' AND revert_type='InClose.D2' AND is_hold=0";

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


        public async Task<ModelMenuD2ProjectNumberData> GetProjectNumberWithDataD2Async(string project_number)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_getdata_for_d2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            ModelMenuD2ProjectNumberData e = new ModelMenuD2ProjectNumberData();
                            while (await reader.ReadAsync())
                            {
                                e.projectname1 = reader[1].ToString();
                                e.projectname2 = reader[2].ToString();
                                e.projectheadname = reader[3].ToString();
                                e.facultyname = reader[4].ToString();
                                e.positionname = reader[5].ToString();
                                e.certificatetype = reader[6].ToString();
                                e.remarkapproval = reader[7].ToString();
                                e.dateofapproval = Convert.ToDateTime(reader[8]).ToString("dd/MM/yyyy");
                            }
                            e.ListDownloadFile = new List<ModelSelectOption>();
                            e.ListDownloadFile = await GetFileDownloadByProjectNumberAsync(project_number);
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

        public async Task<IList<ModelSelectOption>> GetFileDownloadByProjectNumberAsync(string project_number)
        {

            string sql = "SELECT TOP(1) file1name,file2name " +
                         "FROM Doc_MenuA7 WHERE project_number='" + project_number + "' " +
                         "ORDER BY doc_id DESC";

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
                                item.label = "รายงานผลสรุปและแจ้งปิดโครงการ";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["file2name"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["file2name"].ToString();
                                item.label = "ไฟล์ผลงานวิจัยฉบับสมบูรณ์";
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

        public async Task<ModelResponseD2Message> AddDocMenuD2Async(ModelMenuD2 model)
        {

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseD2Message resp = new ModelResponseD2Message();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_d2", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                    cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                    cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                    cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                    cmd.Parameters.Add("@file_download_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filedownloadname);
                    cmd.Parameters.Add("@agenda_number", SqlDbType.Int).Value = model.agendanumber;
                    cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                    cmd.Parameters.Add("@agenda_meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.agendameetingdate);
                    cmd.Parameters.Add("@suggestion", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.acceptTypeNameThai);
                    cmd.Parameters.Add("@remark_suggestion", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.remarkApproval);
                    cmd.Parameters.Add("@conclusion_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.conclusiondate);

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
                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }

    }
}
