using System;
using System.Collections.Generic;
using THD.Core.Api.Repository.DataContext;
using THD.Core.Api.Repository.Interface;
using Microsoft.Extensions.Configuration;
using THD.Core.Api.Entities.Tables;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THD.Core.Api.Models;
using THD.Core.Api.Helpers;

namespace THD.Core.Api.Repository.DataHandler
{
    public class SystemRepository : ISystemRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly DataContextProvider _DataContextProvider;
        public SystemRepository(IConfiguration configuration, DataContextProvider DataContextProvider)
        {
            _configuration = configuration;
            _DataContextProvider = DataContextProvider;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
        }

        public async Task<bool> LicenseKeyValidate(string LicenseKey)
        {
            int count = 0;

            string sql = "SELECT COUNT(LicenseKey) FROM LicenseUser WHERE IsActive='1' AND LicenseKey=@LicenseKey";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.Add("@LicenseKey", SqlDbType.NVarChar).Value = LicenseKey;
                    count = (int)await command.ExecuteScalarAsync();
                }
                conn.Close();
            }

            return (count > 0) ? true : false;
        }

        public async Task<ModelResponseMessageLogin> LogIn(EntityLogSystem entity_model)
        {
            ModelResponseMessageLogin resp = new ModelResponseMessageLogin();
            resp.Data = new ModelResponseMessageLoginData();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_login_system", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 50).Value = entity_model.userid;
                    cmd.Parameters.Add("@Passw", SqlDbType.VarChar, 50).Value = entity_model.passw;
                    cmd.Parameters.Add("@LogEvent", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(entity_model.log_event);
                    cmd.Parameters.Add("@LogDate", SqlDbType.DateTime).Value = entity_model.log_date;

                    SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                    rStatus.Direction = ParameterDirection.Output;
                    SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                    rMessage.Direction = ParameterDirection.Output;
                    SqlParameter rRegisterId = cmd.Parameters.Add("@rRegisterId", SqlDbType.NVarChar, 50);
                    rRegisterId.Direction = ParameterDirection.Output;
                    SqlParameter rFullName = cmd.Parameters.Add("@rFullName", SqlDbType.VarChar, 200);
                    rFullName.Direction = ParameterDirection.Output;
                    SqlParameter rPositionName = cmd.Parameters.Add("@rPositionName", SqlDbType.VarChar, 200);
                    rPositionName.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    if ((int)cmd.Parameters["@rStatus"].Value > 0)
                    {
                        resp.Status = true;
                        resp.Data.RegisterId = (string)cmd.Parameters["@rRegisterId"].Value;
                        resp.Data.FullName = (string)cmd.Parameters["@rFullName"].Value;
                        resp.Data.PositionName = (string)cmd.Parameters["@rPositionName"].Value;
                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }

        public async Task<ModelResponseMessageLogin> LogOut(EntityLogSystem entity_model)
        {
            ModelResponseMessageLogin resp = new ModelResponseMessageLogin();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_logout_system", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@RegisterId", SqlDbType.VarChar, 50).Value = entity_model.register_id;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 50).Value = entity_model.userid;
                    cmd.Parameters.Add("@LogEvent", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(entity_model.log_event);
                    cmd.Parameters.Add("@LogDate", SqlDbType.DateTime).Value = entity_model.log_date;

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
