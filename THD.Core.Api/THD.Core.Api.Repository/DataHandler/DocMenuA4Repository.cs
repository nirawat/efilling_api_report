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
using System.Globalization;
using THD.Core.Api.Models.ReportModels;
using THD.Core.Api.Models.Config;
using static THD.Core.Api.Helpers.ServerDirectorys;

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuA4Repository : IDocMenuA4Repository
    {
        private readonly IConfiguration _configuration;
        private readonly IEnvironmentConfig _IEnvironmentConfig;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMenuC2Repository _IDocMenuC2Repository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;

        public DocMenuA4Repository(IConfiguration configuration,
            IEnvironmentConfig EnvironmentConfig,
            IDropdownListRepository DropdownListRepository,
            IRegisterUserRepository IRegisterUserRepository,
            IDocMenuReportRepository DocMenuReportRepository,
            IDocMenuC2Repository DocMenuC2Repository)
        {
            _configuration = configuration;
            _IEnvironmentConfig = EnvironmentConfig;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMenuC2Repository = DocMenuC2Repository;
            _IDocMenuReportRepository = DocMenuReportRepository;
        }

        public async Task<ModelMenuA4_InterfaceData> MenuA4InterfaceDataAsync(string RegisterId)
        {
            ModelMenuA4_InterfaceData resp = new ModelMenuA4_InterfaceData();

            resp.ListProjectNumber = new List<ModelSelectOption>();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M006");

            if (resp.UserPermission != null && resp.UserPermission.alldata == true)
            {
                resp.ListProjectNumber = await GetAllProjectForA4Async("", "A4");
            }
            else
            {
                resp.ListProjectNumber = await GetAllProjectForA4Async(RegisterId, "A4");
            }

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllProjectForA4Async(string AssignerCode, string DocProcess)
        {

            string sql = "SELECT * FROM [dbo].[Doc_Process] " +
                        "WHERE project_type='PROJECT' AND is_hold=0 AND doc_process_to='" + DocProcess + "' ";

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

        public async Task<ModelMenuA4ProjectNumberData> GetProjectNumberWithDataA4Async(string project_number)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_getdata_for_a4", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            ModelMenuA4ProjectNumberData e = new ModelMenuA4ProjectNumberData();
                            while (await reader.ReadAsync())
                            {
                                e.projectname1 = reader[1].ToString();
                                e.projectname2 = reader[2].ToString();
                                e.projectheadname = reader[3].ToString();
                                e.facultyname = reader[4].ToString();
                                e.positionname = reader[5].ToString();
                                e.dateofapproval = reader[7].ToString();
                            }
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

        public async Task<ModelResponseA4Message> AddDocMenuA4Async(ModelMenuA4 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseA4Message resp = new ModelResponseA4Message();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_a4", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                    cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                    cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                    cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                    cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                    cmd.Parameters.Add("@conclusion_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.conclusiondate);
                    cmd.Parameters.Add("@file1name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.file1name);

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

                        model_rpt_6_file rpt = await _IDocMenuReportRepository.GetReportR6Async((int)cmd.Parameters["@rDocId"].Value);

                        resp.filename = rpt.filename;
                        resp.filebase64 = rpt.filebase64;
                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }

        #region "Edit"
        public async Task<ModelMenuA4_InterfaceData> MenuA4EditInterfaceDataAsync(string UserId, string ProjectNumber)
        {
            ModelMenuA4_InterfaceData resp = new ModelMenuA4_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M006");

            resp.editdata = new ModelMenuA4();
            resp.editdata = await GetMenuA4DataEditAsync(ProjectNumber, UserId, resp.UserPermission);

            resp.ListProjectNumber = new List<ModelSelectOption>();
            ModelSelectOption project_name_default = new ModelSelectOption()
            {
                value = resp.editdata.projectnumber,
                label = resp.editdata.projectnumber + " : " + resp.editdata.projectnamethai,
            };
            resp.ListProjectNumber.Add(project_name_default);

            return resp;
        }

        private async Task<ModelMenuA4> GetMenuA4DataEditAsync(string ProjectNumber, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            string sql = "SELECT TOP(1)* FROM Doc_MenuA4 " +
                         "WHERE project_number='" + ProjectNumber + "' ORDER BY doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuA4 e = new ModelMenuA4();
                        while (await reader.ReadAsync())
                        {
                            e.docid = reader["doc_id"].ToString();
                            e.projectnumber = reader["project_number"].ToString();
                            e.projectheadname = reader["project_head_name"].ToString();
                            e.facultyname = reader["faculty_name"].ToString();
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                            e.conclusiondate = Convert.ToDateTime(reader["conclusion_date"]).ToString("dd/MM/yyyy");
                            e.file1name = reader["file1name"].ToString();
                            e.createby = reader["create_by"].ToString();

                            //Default Edit False
                            e.editenable = false;
                            if (permission.edit == true)
                            {
                                if (user_id == reader["create_by"].ToString())
                                {
                                    e.editenable = true;
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

        public async Task<ModelMenuA4_FileDownload> GetA4DownloadFileByIdAsync(int DocId, int Id)
        {

            string sql = "SELECT TOP(1) file1name FROM Doc_MenuA4 WHERE doc_id='" + DocId + "' ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuA4_FileDownload e = new ModelMenuA4_FileDownload();
                        while (await reader.ReadAsync())
                        {
                            if (Id == 1)
                            {
                                e.filebase64 = ServerDirectorys.ReadFileToBase64(_IEnvironmentConfig.PathDocument, FolderDocument.menuA4, reader["file1name"].ToString());
                                e.filename = "คำขอชี้แจง_แก้ไขโครงการตามมติคณะกรรมการ";
                            }
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseA4Message> UpdateDocMenuA4EditAsync(ModelMenuA4 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseA4Message resp = new ModelResponseA4Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_a4_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectnumber);
                        cmd.Parameters.Add("@project_head_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectheadname);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyname);
                        cmd.Parameters.Add("@project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnamethai);
                        cmd.Parameters.Add("@project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.projectnameeng);
                        cmd.Parameters.Add("@conclusion_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.conclusiondate);
                        cmd.Parameters.Add("@file1name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.file1name);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            model_rpt_6_file rpt = await _IDocMenuReportRepository.GetReportR6Async(Convert.ToInt32(model.docid));

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
    }
}

