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
    public class DocMenuF1Repository : IDocMenuF1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuF1Repository(IConfiguration configuration, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
        }

        #region Menu F1

        public async Task<ModelMenuF1_InterfaceData> MenuF1InterfaceDataAsync(string RegisterId)
        {
            ModelMenuF1_InterfaceData resp = new ModelMenuF1_InterfaceData();

            resp.listdata = await GetAllReportDataF1Async(null);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M023");

            return resp;
        }

        public async Task<IList<ModelMenuF1Report>> GetAllReportDataF1Async(ModelMenuF1_InterfaceData search_data)
        {
            string sql = "SELECT A.*, (B.name_thai) AS position_name_thai, " +
                        "(C.name_thai) AS faculty_name_thai, (D.name_thai) AS education_name_thai, " +
                        "(E.name_thai) AS character_name_thai " +
                        "FROM RegisterUser A " +
                        "INNER JOIN MST_Position B ON A.position = B.id " +
                        "INNER JOIN MST_Faculty C ON A.faculty = C.id " +
                        "INNER JOIN MST_Education D ON A.education = D.id " +
                        "INNER JOIN MST_Character E ON A.character = E.id " +
                        "WHERE 1=1 ";

            if (search_data != null && !string.IsNullOrEmpty(search_data.searchkey))
            {
                sql += " AND (register_id LIKE '%" + search_data.searchkey + "%' OR email LIKE '%" + search_data.searchkey + "%' OR full_name LIKE '%" + search_data.searchkey + "%') ";
            }

            sql += " ORDER BY full_name ASC ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        int row_count = 1;
                        IList<ModelMenuF1Report> e = new List<ModelMenuF1Report>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuF1Report item = new ModelMenuF1Report();
                            item.registerid = reader["register_id"].ToString();
                            item.userid = reader["userid"].ToString();
                            item.fullname = reader["first_name"].ToString() + reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            item.registerdate = Convert.ToDateTime(reader["register_date"]).ToString("dd/MM/yyyy");
                            item.registerexpire = Convert.ToDateTime(reader["register_expire"]).ToString("dd/MM/yyyy");
                            item.positionname = reader["position_name_thai"].ToString();
                            item.facultyname = reader["faculty_name_thai"].ToString();
                            item.educationname = reader["education_name_thai"].ToString();
                            item.charactername = reader["character_name_thai"].ToString();
                            item.workphone = reader["work_phone"].ToString();
                            item.mobile = reader["mobile"].ToString();
                            item.fax = reader["fax"].ToString();
                            item.isactive = reader["IsActive"].ToString();

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

        #endregion

        #region Menu F1 Edit

        public async Task<ModelMenuF1Edit_InterfaceData> MenuF1EditInterfaceDataAsync(string RegisterId, string UserId)
        {
            ModelMenuF1Edit_InterfaceData resp = new ModelMenuF1Edit_InterfaceData();

            resp.listfirstname = await GetAllFirstNameAsync();

            resp.listfaculty = await GetAllFacultyAsync();

            resp.UserData = await GetEditDataByRegisterId(UserId);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M023");

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllFirstNameAsync()
        {

            string sql = "SELECT * FROM MST_FirstName ORDER BY id ASC";

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
                            item.value = reader["name_thai"].ToString();
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

        public async Task<IList<ModelSelectOption>> GetAllFacultyAsync()
        {

            string sql = "SELECT * FROM MST_Faculty ORDER BY id ASC";

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

        public async Task<ModelRegisterEdit> GetEditDataByRegisterId(string UserId)
        {
            string sql = "SELECT A.*, (B.name_thai) AS position_name_thai, " +
                        "(C.name_thai) AS faculty_name_thai, (D.name_thai) AS education_name_thai, " +
                        "(E.name_thai) AS character_name_thai " +
                        "FROM RegisterUser A " +
                        "INNER JOIN MST_Position B ON A.position = B.id " +
                        "INNER JOIN MST_Faculty C ON A.faculty = C.id " +
                        "INNER JOIN MST_Education D ON A.education = D.id " +
                        "INNER JOIN MST_Character E ON A.character = E.id " +
                        "WHERE register_id ='" + UserId + "' ORDER BY full_name ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelRegisterEdit item = new ModelRegisterEdit();
                        while (await reader.ReadAsync())
                        {
                            item.registerid = reader["register_id"].ToString();
                            item.email = reader["email"].ToString();
                            item.userid = reader["userid"].ToString();
                            item.passw = reader["passw"].ToString();
                            item.confirmpassw = reader["confirmpassw"].ToString();
                            item.firstname1 = reader["first_name_1"].ToString();
                            item.firstname2 = reader["first_name_2"].ToString();
                            item.firstname = reader["first_name"].ToString();
                            item.fullname = reader["full_name"].ToString();
                            item.position = reader["position"].ToString();
                            item.positionname = reader["position_name_thai"].ToString();
                            item.faculty = reader["faculty"].ToString();
                            item.facultyname = reader["faculty_name_thai"].ToString();
                            item.workphone = reader["work_phone"].ToString();
                            item.mobile = reader["mobile"].ToString();
                            item.fax = reader["fax"].ToString();
                            item.education = reader["education"].ToString();
                            item.educationname = reader["education_name_thai"].ToString();
                            item.character = reader["character"].ToString();
                            item.charactername = reader["character_name_thai"].ToString();
                            item.note1 = reader["note1"].ToString();
                            item.note2 = reader["note2"].ToString();
                            item.note3 = reader["note3"].ToString();
                            item.isactive = Convert.ToBoolean(reader["isActive"]);
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;
        }

        public async Task<ModelResponseMessageUpdateUserRegister> UpdateUserRegisterAsync(ModelRegisterEdit model)
        {
            ModelResponseMessageUpdateUserRegister resp = new ModelResponseMessageUpdateUserRegister();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_user_register", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 100).Value = model.registerid;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.email);
                    cmd.Parameters.Add("@FirstName1", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname1);
                    cmd.Parameters.Add("@FirstName2", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname2);
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname);
                    cmd.Parameters.Add("@FullName", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.fullname);
                    cmd.Parameters.Add("@WorkPhone", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.workphone);
                    cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.mobile);
                    cmd.Parameters.Add("@Fax", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.fax);
                    cmd.Parameters.Add("@Position", SqlDbType.VarChar, 2).Value = model.position;
                    cmd.Parameters.Add("@Faculty", SqlDbType.VarChar, 2).Value = model.faculty;
                    cmd.Parameters.Add("@Education", SqlDbType.VarChar, 2).Value = model.education;
                    cmd.Parameters.Add("@Character", SqlDbType.VarChar, 2).Value = model.character;
                    cmd.Parameters.Add("@Note1", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note1);
                    cmd.Parameters.Add("@Note2", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note2);
                    cmd.Parameters.Add("@Note3", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note3);
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.isactive;

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
            return resp;
        }

        #endregion

        #region Menu Account User

        public async Task<ModelMenuFAccount_InterfaceData> MenuAccountInterfaceDataAsync(string RegisterId)
        {
            ModelMenuFAccount_InterfaceData resp = new ModelMenuFAccount_InterfaceData();

            resp.listfirstname = await GetAllFirstNameAsync();

            resp.listfaculty = await GetAllFacultyAsync();

            resp.account = await GetEditDataAccountAsync(RegisterId);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M037");

            return resp;
        }

        private async Task<ModelMenuAccountUser> GetEditDataAccountAsync(string registerid)
        {
            string userid = Encoding.UTF8.GetString(Convert.FromBase64String(registerid));

            string sql = "SELECT A.*, (B.name_thai) AS position_name_thai, " +
                        "(C.name_thai) AS faculty_name_thai, (D.name_thai) AS education_name_thai, " +
                        "(E.name_thai) AS character_name_thai " +
                        "FROM RegisterUser A " +
                        "INNER JOIN MST_Position B ON A.position = B.id " +
                        "INNER JOIN MST_Faculty C ON A.faculty = C.id " +
                        "INNER JOIN MST_Education D ON A.education = D.id " +
                        "INNER JOIN MST_Character E ON A.character = E.id " +
                        "WHERE 1=1 AND register_id='" + userid + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuAccountUser item = new ModelMenuAccountUser();
                        while (await reader.ReadAsync())
                        {
                            item.registerid = reader["register_id"].ToString();
                            item.userid = reader["userid"].ToString();
                            item.firstname1 = reader["first_name_1"].ToString();
                            item.firstname2 = reader["first_name_2"].ToString();
                            item.firstname = reader["first_name"].ToString();
                            item.fullname = reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            item.registerdate = Convert.ToDateTime(reader["register_date"]).ToString("dd/MM/yyyy");
                            item.registerexpire = Convert.ToDateTime(reader["register_expire"]).ToString("dd/MM/yyyy");
                            item.position = reader["position"].ToString();
                            item.positionname = reader["position_name_thai"].ToString();
                            item.faculty = reader["faculty"].ToString();
                            item.facultyname = reader["faculty_name_thai"].ToString();
                            item.education = reader["education"].ToString();
                            item.educationname = reader["education_name_thai"].ToString();
                            item.character = reader["character"].ToString();
                            item.charactername = reader["character_name_thai"].ToString();
                            item.workphone = reader["work_phone"].ToString();
                            item.mobile = reader["mobile"].ToString();
                            item.fax = reader["fax"].ToString();
                            item.note1 = reader["note1"].ToString();
                            item.note2 = reader["note2"].ToString();
                            item.note3 = reader["note3"].ToString();
                            item.isactive = reader["IsActive"].ToString();
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;
        }

        public async Task<ModelResponseMessageUpdateUserRegister> UpdateUserAccountAsync(ModelUpdateAccountUser model)
        {
            ModelResponseMessageUpdateUserRegister resp = new ModelResponseMessageUpdateUserRegister();

            string userid = Encoding.UTF8.GetString(Convert.FromBase64String(model.registerid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_user_account", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 100).Value = userid;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.email);
                    cmd.Parameters.Add("@FirstName1", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname1);
                    cmd.Parameters.Add("@FirstName2", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname2);
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.firstname);
                    cmd.Parameters.Add("@FullName", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.fullname);
                    cmd.Parameters.Add("@WorkPhone", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.workphone);
                    cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.mobile);
                    cmd.Parameters.Add("@Fax", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.fax);
                    cmd.Parameters.Add("@Position", SqlDbType.VarChar, 2).Value = model.position;
                    cmd.Parameters.Add("@Faculty", SqlDbType.VarChar, 2).Value = model.faculty;
                    cmd.Parameters.Add("@Education", SqlDbType.VarChar, 2).Value = model.education;
                    cmd.Parameters.Add("@Note1", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note1);
                    cmd.Parameters.Add("@Note2", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note2);
                    cmd.Parameters.Add("@Note3", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note3);

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
            return resp;
        }
        #endregion

    }
}
