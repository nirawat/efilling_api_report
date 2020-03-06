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
    public class DocMenuB2Repository : IDocMenuB2Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuB2Repository(IConfiguration configuration,
                                   IDropdownListRepository DropdownListRepository, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
        }

        public async Task<ModelMenuB2_InterfaceData> MenuB2InterfaceDataAsync(string RegisterId)
        {
            ModelMenuB2_InterfaceData resp = new ModelMenuB2_InterfaceData();

            resp.ListLabNumber = new List<ModelSelectOption>();
            resp.ListLabNumber = await GetAllLabNumberAsync();

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            resp.defaultyear = thai_year;
            ModelSelectOption year_current = new ModelSelectOption();
            year_current.value = (thai_year).ToString();
            year_current.label = (thai_year).ToString();
            resp.ListYearOfProject.Add(year_current);

            //for (int i = 1; i < 5; i++)
            //{
            //    ModelSelectOption year_next = new ModelSelectOption();
            //    year_next.value = (thai_year + i).ToString();
            //    year_next.label = (thai_year + i).ToString();
            //    resp.ListYearOfProject.Add(year_next);
            //}

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M010");

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllLabNumberAsync()
        {
            string sql = "SELECT doc_id, (room_number + ' : ' + ISNULL(labothername,'') + ' ' + faculty_laboratory) as name_thai FROM Doc_MenuA2 WHERE IsUsed=0 ";

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

        public async Task<ModelMenuB2Data> GetLabNumberWithDataB2Async(string project_id)
        {

            string sql = "SELECT A.doc_id, B.name_thai, A.faculty_laboratory FROM Doc_MenuA2 A " +
                        "INNER JOIN MST_LabMethodType B " +
                        "ON A.project_according_type_method = B.id " +
                        "WHERE A.doc_id='" + project_id + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuB2Data e = new ModelMenuB2Data();
                        while (await reader.ReadAsync())
                        {
                            e.labTypeName = reader["name_thai"].ToString();
                            e.facultyName = reader["faculty_laboratory"].ToString();
                        }
                        e.ListDownloadFile = await GetAllDownloadFileByLabNumberB2Async(project_id);
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllDownloadFileByLabNumberB2Async(string project_id)
        {

            string sql = "SELECT filename1,filename2 FROM Doc_MenuA2 WHERE doc_id='" + project_id + "'";

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
                            if (!string.IsNullOrEmpty(reader["filename1"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["filename1"].ToString();
                                item.label = "แนบไฟล์แบบคำขอ (NUIBC01)";
                                e.Add(item);
                            }
                            if (!string.IsNullOrEmpty(reader["filename2"].ToString()))
                            {
                                ModelSelectOption item = new ModelSelectOption();
                                item.value = reader["filename2"].ToString();
                                item.label = "แนบไฟล์แบบประเมินเบื้องต้น";
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

        public async Task<ModelResponseMessageAddDocB2> AddDocMenuB2Async(ModelMenuB2 model)
        {

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseMessageAddDocB2 resp = new ModelResponseMessageAddDocB2();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_b2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_date", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                        cmd.Parameters.Add("@accept_type", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.accepttype);
                        cmd.Parameters.Add("@project_id", SqlDbType.Int).Value = model.projectid;

                        char[] spearator = { ':' };
                        string[] lab_number = model.labNumber.Split(spearator, System.StringSplitOptions.RemoveEmptyEntries);

                        cmd.Parameters.Add("@lab_number", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(lab_number[0].ToString().Trim());
                        cmd.Parameters.Add("@lab_type_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.labTypeName);
                        cmd.Parameters.Add("@faculty_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.facultyName);
                        cmd.Parameters.Add("@initial_result", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.initialresult);
                        cmd.Parameters.Add("@file_download_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filedownloadname);
                        cmd.Parameters.Add("@project_key_number", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.projectkeynumber);
                        cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.notes);
                        cmd.Parameters.Add("@acronyms", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.acronyms);
                        cmd.Parameters.Add("@round_of_meeting", SqlDbType.Int).Value = model.roundofmeeting;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@year_of_running", SqlDbType.Int).Value = model.defaultyear;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);

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
