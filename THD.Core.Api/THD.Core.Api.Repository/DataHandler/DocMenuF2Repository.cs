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
    public class DocMenuF2Repository : IDocMenuF2Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuF2Repository(IConfiguration configuration, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
        }

        #region Menu F2

        public async Task<ModelMenuF2_InterfaceData> MenuF2InterfaceDataAsync(string RegisterId, string UserGroup)
        {
            ModelMenuF2_InterfaceData resp = new ModelMenuF2_InterfaceData();

            ModelMenuF2_InterfaceData search_data = new ModelMenuF2_InterfaceData();
            search_data.usergroup = UserGroup;
            resp.listdata = await GetAllReportDataF2Async(search_data);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M024");

            return resp;
        }

        public async Task<IList<ModelMenuF2Report>> GetAllReportDataF2Async(ModelMenuF2_InterfaceData search_data)
        {
            string sql = "SELECT B.name as group_name, C.*, A.* " +
                        "FROM [dbo].[SYS_Permission] A " +
                        "LEFT OUTER JOIN[dbo].[SYS_UserGroup] B ON A.user_group_code = B.code " +
                        "LEFT OUTER JOIN[dbo].[SYS_MenuPages] C ON A.menu_page_code = C.code " +
                        "WHERE 1=1 ";

            if (search_data != null && !string.IsNullOrEmpty(search_data.usergroup))
            {
                sql += " AND (user_group_code ='" + search_data.usergroup + "') ";
            }

            sql += " ORDER BY B.name, C.code ASC ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        int row_count = 1;
                        IList<ModelMenuF2Report> e = new List<ModelMenuF2Report>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuF2Report item = new ModelMenuF2Report();
                            item.usergroupname = reader["group_name"].ToString();
                            item.menupagecode = reader["menu_page_code"].ToString();
                            item.menupagename = reader["name"].ToString();
                            item.pmview = (Convert.ToBoolean(reader["pm_view"]) == true) ? "YES" : "";
                            item.pminsert = (Convert.ToBoolean(reader["pm_insert"]) == true) ? "YES" : "";
                            item.pmupdate = (Convert.ToBoolean(reader["pm_update"]) == true) ? "YES" : "";
                            item.pmprint = (Convert.ToBoolean(reader["pm_print"]) == true) ? "YES" : "";
                            item.pmalldata = (Convert.ToBoolean(reader["pm_all_data"]) == true) ? "YES" : "";
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

        public async Task<ModelMenuF2Edit> GetUserEditPermissionF2Async(string UserGroup, string MenuCode)
        {
            string sql = "SELECT * FROM [dbo].[SYS_Permission] WHERE user_group_code='" + UserGroup + "' AND menu_page_code='" + MenuCode + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuF2Edit item = new ModelMenuF2Edit();
                        while (await reader.ReadAsync())
                        {
                            item.pmview = Convert.ToBoolean(reader["pm_view"]);
                            item.pminsert = Convert.ToBoolean(reader["pm_insert"]);
                            item.pmupdate = Convert.ToBoolean(reader["pm_update"]);
                            item.pmprint = Convert.ToBoolean(reader["pm_print"]);
                            item.pmalldata = Convert.ToBoolean(reader["pm_all_data"]);
                        }
                        return item;
                    }
                }
                conn.Close();
            }
            return null;
        }

        public async Task<ModelResponseMessageUpdateUserRegister> UpdatePermissionGroupAsync(ModelMenuF2Edit model)
        {
            ModelResponseMessageUpdateUserRegister resp = new ModelResponseMessageUpdateUserRegister();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_permission_group", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserGroupCode", SqlDbType.VarChar, 100).Value = model.usergroup;
                    cmd.Parameters.Add("@MenuPageCode", SqlDbType.VarChar, 100).Value = model.menupagecode;
                    cmd.Parameters.Add("@PmView", SqlDbType.Bit).Value = Convert.ToBoolean(model.pmview);
                    cmd.Parameters.Add("@PmInsert", SqlDbType.Bit).Value = Convert.ToBoolean(model.pminsert);
                    cmd.Parameters.Add("@PmUpdate", SqlDbType.Bit).Value = Convert.ToBoolean(model.pmupdate);
                    cmd.Parameters.Add("@PmPrint", SqlDbType.Bit).Value = Convert.ToBoolean(model.pmprint);
                    cmd.Parameters.Add("@PmAllData", SqlDbType.Bit).Value = Convert.ToBoolean(model.pmalldata);

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
