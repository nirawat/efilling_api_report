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

namespace THD.Core.Api.Repository.DataHandler
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly DataContextProvider _DataContextProvider;
        public RegisterUserRepository(IConfiguration configuration, DataContextProvider DataContextProvider)
        {
            _configuration = configuration;
            _DataContextProvider = DataContextProvider;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
        }

        public async Task<ModelRegisterActive_InterfaceData> ActiveUserAccountInterfaceAsync(string RegisterId)
        {
            ModelRegisterActive_InterfaceData resp = new ModelRegisterActive_InterfaceData();

            resp.listfirstname = await GetAllFirstNameAsync();

            resp.listfaculty = await GetAllFacultyAsync();

            resp.UserAccount = await GetUserAccountInfoByRegisterId(RegisterId);

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

        public async Task<ModelRegisterActive> GetUserAccountInfoByRegisterId(string RegisterId)
        {
            //string register_id = Encoding.UTF8.GetString(Convert.FromBase64String(RegisterId));

            string sql = "SELECT * FROM RegisterUser WHERE register_id ='" + RegisterId + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelRegisterActive item = new ModelRegisterActive();
                        while (await reader.ReadAsync())
                        {
                            item.registerid = reader["register_id"].ToString();
                            item.email = reader["email"].ToString();
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;
        }



        public async Task<ModelResponseMessageRegisterUser> AddRegisterUserAsync(EntityRegisterUser entity_model)
        {
            ModelResponseMessageRegisterUser resp = new ModelResponseMessageRegisterUser();

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var LicenseKey = GenerateToken.GetToken();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_register_user", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 100).Value = DateTime.Now.ToString("yyyyMMdd");
                    cmd.Parameters.Add("@LicenseKey", SqlDbType.NVarChar).Value = LicenseKey;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 200).Value = entity_model.userid;
                    cmd.Parameters.Add("@Passw", SqlDbType.NVarChar, 500).Value = entity_model.passw;
                    cmd.Parameters.Add("@ConfirmPassw", SqlDbType.NVarChar, 500).Value = entity_model.confirmpassw;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 250).Value = entity_model.email;
                    cmd.Parameters.Add("@RegisterDate", SqlDbType.DateTime).Value = entity_model.register_date;
                    cmd.Parameters.Add("@ExpireDate", SqlDbType.DateTime).Value = entity_model.register_expire;

                    SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                    rStatus.Direction = ParameterDirection.Output;
                    SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                    rMessage.Direction = ParameterDirection.Output;
                    SqlParameter rRegisterId = cmd.Parameters.Add("@rRegisterId", SqlDbType.VarChar, 100);
                    rRegisterId.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    if ((int)cmd.Parameters["@rStatus"].Value > 0)
                    {
                        resp.Status = true;
                        resp.RegisterId = cmd.Parameters["@rRegisterId"].Value.ToString();
                    }
                    else resp.Message = cmd.Parameters["@rMessage"].Value.ToString();
                }
                conn.Close();
            }
            return resp;
        }

        public async Task<ModelResponseMessageRegisterActive> AddRegisterActiveAsync(EntityRegisterUser model)
        {
            ModelResponseMessageRegisterActive resp = new ModelResponseMessageRegisterActive();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_register_active", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 100).Value = model.register_id;
                    cmd.Parameters.Add("@FirstName1", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.first_name_1);
                    cmd.Parameters.Add("@FirstName2", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.first_name_2);
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.first_name);
                    cmd.Parameters.Add("@FullName", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.full_name);
                    cmd.Parameters.Add("@Position", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.position);
                    cmd.Parameters.Add("@Faculty", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.faculty);
                    cmd.Parameters.Add("@WorkPhone", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.work_phone);
                    cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.mobile);
                    cmd.Parameters.Add("@Fax", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.fax);
                    cmd.Parameters.Add("@Education", SqlDbType.VarChar, 2).Value = model.education;
                    cmd.Parameters.Add("@Character", SqlDbType.VarChar, 2).Value = model.character;
                    cmd.Parameters.Add("@Note1", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note1);
                    cmd.Parameters.Add("@Note2", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note2);
                    cmd.Parameters.Add("@Note3", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.note3);
                    cmd.Parameters.Add("@ConfirmDate", SqlDbType.DateTime).Value = model.confirm_date;
                    cmd.Parameters.Add("@MemberExpire", SqlDbType.DateTime).Value = model.member_expire;

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

        public async Task<EntityRegisterUser> GetRegisterUserActiveAsync(string RegisterId)
        {
            return await _DataContextProvider.RegisterUsers.FirstOrDefaultAsync(e => e.register_id.Equals(RegisterId) && e.isactive == true);
        }

        public async Task<ModelRegisterActive> GetFullRegisterUserByIdAsync(string RegisterId)
        {

            string sql = "SELECT A.*, (B.name_thai) AS position_name_thai, " +
                        "(C.name_thai) AS faculty_name_thai, (D.name_thai) AS education_name_thai, " +
                        "(E.name_thai) AS character_name_thai " +
                        "FROM RegisterUser A " +
                        "LEFT OUTER JOIN MST_Position B ON A.position = B.id " +
                        "LEFT OUTER JOIN MST_Faculty C ON A.faculty = C.id " +
                        "LEFT OUTER JOIN MST_Education D ON A.education = D.id " +
                        "LEFT OUTER JOIN MST_Character E ON A.character = E.id " +
                        "WHERE A.IsActive = '1' AND register_id ='" + RegisterId + "' ORDER BY full_name ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelRegisterActive item = new ModelRegisterActive();
                        while (await reader.ReadAsync())
                        {
                            item.registerid = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["register_id"].ToString()));
                            item.confirmpassw = reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            item.firstname = reader["first_name"].ToString();
                            item.fullname = reader["full_name"].ToString();
                            item.faculty = reader["faculty"].ToString();
                            item.facultyname = reader["faculty_name_thai"].ToString();
                            item.position = reader["position"].ToString();
                            item.positionname = reader["position_name_thai"].ToString();
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
                            item.isactive = (bool)reader["isactive"];
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<EntityRegisterUser> GetRegisterUserInActiveAsync(string RegisterId)
        {
            return await _DataContextProvider.RegisterUsers.FirstOrDefaultAsync(e => e.register_id.Equals(RegisterId) && e.isactive != true);
        }

        public async Task<ModelPermissionPage> GetPermissionPageAsync(string RegisterId, string PageCode)
        {
            string userid = Encoding.UTF8.GetString(Convert.FromBase64String(RegisterId));

            string sql = "SELECT A.register_id, (A.first_name + A.full_name) as full_name, D.* " +
                        "FROM [dbo].[RegisterUser] A " +
                        "INNER JOIN[dbo].[SYS_UserRole] B ON A.character = B.code " +
                        "INNER JOIN[dbo].[SYS_UserGroup] C ON B.usergroup = C.code " +
                        "INNER JOIN[dbo].[SYS_Permission] D ON C.code = D.user_group_code " +
                        "WHERE A.register_id='" + userid + "' AND D.menu_page_code = '" + PageCode + "' AND A.IsActive=1";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelPermissionPage item = new ModelPermissionPage();
                        while (await reader.ReadAsync())
                        {
                            item.registerid = reader["register_id"].ToString();
                            item.fullname = reader["full_name"].ToString();
                            item.groupcode = reader["user_group_code"].ToString();
                            item.pagecode = reader["menu_page_code"].ToString();
                            item.view = (bool)reader["pm_view"];
                            item.insert = (bool)reader["pm_insert"];
                            item.edit = (bool)reader["pm_update"];
                            item.print = (bool)reader["pm_print"];
                            item.alldata = (bool)reader["pm_all_data"];
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseMessageUpdateUserRegister> ResetPasswordAsync(ModelResetPassword model)
        {
            ModelResponseMessageUpdateUserRegister resp = new ModelResponseMessageUpdateUserRegister();

            string userid = Encoding.UTF8.GetString(Convert.FromBase64String(model.registerid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_reset_password", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 100).Value = userid;

                    string encrypt_old_passw = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.oldpassw));

                    cmd.Parameters.Add("@OldPassword", SqlDbType.VarChar, 100).Value = encrypt_old_passw;

                    string encrypt_passw = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.newpassw));

                    cmd.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = encrypt_passw;

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

    }
}
