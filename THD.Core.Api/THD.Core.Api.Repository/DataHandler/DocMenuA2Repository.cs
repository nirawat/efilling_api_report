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

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuA2Repository : IDocMenuA2Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;

        public DocMenuA2Repository(
            IConfiguration configuration,
            DataContextProvider DataContextProvider,
            IDocMenuReportRepository DocMenuReportRepository,
            IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
        }

        public async Task<ModelMenuA2_InterfaceData> MenuA2InterfaceDataAsync(string RegisterId)
        {
            ModelMenuA2_InterfaceData resp = new ModelMenuA2_InterfaceData();

            resp.ListLaboratoryRoom = new List<ModelSelectOption>();
            resp.ListLaboratoryRoom = await GetAllLaboratoryRoomAsync();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M004");

            return resp;

        }

        public async Task<IList<ModelSelectOption>> GetAllLaboratoryRoomAsync()
        {

            string sql = "SELECT * FROM MST_Laboratory ORDER BY id ASC";

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
                            item.value = reader["code"].ToString();
                            item.label = reader["code"].ToString() + " : " + reader["name_thai"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;


        }
        public async Task<ModelResponseA2Message> AddDocMenuA2Async(ModelMenuA2 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseA2Message resp = new ModelResponseA2Message();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_a2", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@DocDate", SqlDbType.DateTime).Value = model.docdate.ToString("yyyy-MM-dd");
                    cmd.Parameters.Add("@ProjectMethod", SqlDbType.VarChar, 2).Value = model.projectaccordingtypemethod;
                    cmd.Parameters.Add("@FacultyLaboratory", SqlDbType.VarChar, 200).Value = model.facultylaboratory;
                    cmd.Parameters.Add("@Department", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.department);
                    cmd.Parameters.Add("@LaboratoryAddress", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.laboratoryaddress);
                    cmd.Parameters.Add("@Building", SqlDbType.VarChar, 100).Value = ParseDataHelper.ConvertDBNull(model.building);
                    cmd.Parameters.Add("@Floor", SqlDbType.VarChar, 10).Value = ParseDataHelper.ConvertDBNull(model.floor);
                    cmd.Parameters.Add("@RoomNumber", SqlDbType.VarChar, 10).Value = ParseDataHelper.ConvertDBNull(model.roomnumber);
                    cmd.Parameters.Add("@LabOtherName", SqlDbType.VarChar, 10).Value = ParseDataHelper.ConvertDBNull(model.labothername);
                    cmd.Parameters.Add("@Telephone", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.telephone);
                    cmd.Parameters.Add("@ReponsiblePerson", SqlDbType.VarChar, 100).Value = ParseDataHelper.ConvertDBNull(model.responsibleperson);
                    cmd.Parameters.Add("@Workphone", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.workphone);
                    cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 20).Value = ParseDataHelper.ConvertDBNull(model.mobile);
                    cmd.Parameters.Add("@FileName1", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filename1);
                    cmd.Parameters.Add("@FileName2", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.filename2);
                    cmd.Parameters.Add("@create_by", SqlDbType.NVarChar).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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

                        model_rpt_17_file rpt = await _IDocMenuReportRepository.GetReportR17_18Async((int)cmd.Parameters["@rDocId"].Value);

                        resp.filename = rpt.filename;
                        resp.filebase64 = rpt.filebase64;
                    }
                    else resp.Message = (string)cmd.Parameters["@rMessage"].Value;
                }
                conn.Close();
            }
            return resp;
        }

    }
}
