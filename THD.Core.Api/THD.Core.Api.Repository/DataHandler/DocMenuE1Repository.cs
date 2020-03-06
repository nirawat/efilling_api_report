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
    public class DocMenuE1Repository : IDocMenuE1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly ISystemRepository _ISystemRepository;
        public DocMenuE1Repository(IConfiguration configuration, IRegisterUserRepository IRegisterUserRepository, ISystemRepository SystemRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IRegisterUserRepository = IRegisterUserRepository;
            _ISystemRepository = SystemRepository;
        }

        public async Task<ModelMenuE1_InterfaceData> MenuE1InterfaceDataAsync(string RegisterId, string Passw)
        {
            ModelMenuE1_InterfaceData resp = new ModelMenuE1_InterfaceData();

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            EntityLogSystem user_login = new EntityLogSystem();

            user_login.register_id = RegisterId;
            user_login.userid = "customer";
            user_login.passw = Passw;
            user_login.log_event = "Get Link Login";
            user_login.log_date = DateTime.Now;

            ModelResponseMessageLogin get_login = await _ISystemRepository.LogIn(user_login);

            // Default
            resp.Status = false;
            resp.Message = "ไม่พบสิทธิ์การใช้งานในระบบ!";
            resp.docDate = DateTime.Now;
            resp.docNumber = "";

            if (get_login != null && get_login.Status == true)
            {
                resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M022");

                if (resp.UserPermission != null)
                {
                    resp.Status = true;
                    resp.Message = "ยินดีต้อนรับเข้าสู่ระบบ.";
                    resp.docDate = DateTime.Now;
                    resp.docNumber = DateTime.Now.ToString("yyMM-XXX");
                }
            }

            resp.listfaculty = await GetAllFacultyAsync();

            return resp;
        }

        public async Task<ModelMenuE1_InterfaceReportData> MenuE1InterfaceReportDataAsync(ModelMenuE1_InterfaceReportData search)
        {
            ModelMenuE1_InterfaceReportData resp = new ModelMenuE1_InterfaceReportData();

            resp.listfaculty = await GetAllFacultyAsync();

            resp.listreportdata = await GetAllReportDataE1Async(search);

            return resp;
        }

        private async Task<IList<ModelMenuE1Report>> GetAllReportDataE1Async(ModelMenuE1_InterfaceReportData search)
        {
            string sql = "SELECT A.*, " +
                        "B.name_thai as group_1_risk_human_name,  " +
                        "C.name_thai as group_1_risk_animal_name,  " +
                        "D.name_thai as group_1_pathogens_name,  " +
                        "E.name_thai as group_2_risk_human_name,  " +
                        "F.name_thai as group_2_risk_animal_name,  " +
                        "G.name_thai as group_2_pathogens_name, " +
                        "H.name_thai as faculty_name " +
                        "FROM Doc_MenuE1 A " +
                        "LEFT OUTER JOIN MST_Risk_Human B ON A.group_1_risk_human = B.id " +
                        "LEFT OUTER JOIN MST_Risk_Animal C ON A.group_1_risk_animal = C.id " +
                        "LEFT OUTER JOIN MST_Risk_Pathogens D ON A.group_1_pathogens = D.id " +
                        "LEFT OUTER JOIN MST_Risk_Human E ON A.group_2_risk_human = E.id " +
                        "LEFT OUTER JOIN MST_Risk_Animal F ON A.group_2_risk_animal = F.id " +
                        "LEFT OUTER JOIN MST_Risk_Pathogens G ON A.group_2_pathogens = G.id " +
                        "LEFT OUTER JOIN MST_Faculty H ON A.faculty = H.id " +
                        "WHERE 1=1 ";

            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.docnumber))
                    sql += " AND A.doc_number LIKE '%" + search.docnumber + "%'";

                if (!string.IsNullOrEmpty(search.sectionname))
                    sql += " AND A.section_name LIKE '%" + search.sectionname + "%'";

                if (!string.IsNullOrEmpty(search.faculty))
                    sql += " AND A.faculty LIKE '%" + search.faculty + "%'";

                if (!string.IsNullOrEmpty(search.group1riskhuman))
                    sql += " AND A.group_1_risk_human ='" + search.group1riskhuman + "'";

                if (!string.IsNullOrEmpty(search.group1riskanimal))
                    sql += " AND A.group_1_risk_animal ='" + search.group1riskanimal + "'";

                if (!string.IsNullOrEmpty(search.group1pathogens))
                    sql += " AND A.group_1_pathogens ='" + search.group1pathogens + "'";

                if (!string.IsNullOrEmpty(search.group2riskhuman))
                    sql += " AND A.group_2_risk_human ='" + search.group2riskhuman + "'";

                if (!string.IsNullOrEmpty(search.group2riskanimal))
                    sql += " AND A.group_2_risk_animal ='" + search.group2riskanimal + "'";

                if (!string.IsNullOrEmpty(search.group2pathogens))
                    sql += " AND A.group_2_pathogens ='" + search.group2pathogens + "'";

            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        int row_count = 1;
                        IList<ModelMenuE1Report> e = new List<ModelMenuE1Report>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuE1Report item = new ModelMenuE1Report();
                            item.docDate = Convert.ToDateTime(reader["doc_date"]).ToString("dd/MM/yyyy");
                            item.docNumber = reader["doc_number"].ToString();
                            item.sectionName = reader["section_name"].ToString();
                            item.facultyName = reader["faculty_name"].ToString();
                            item.departmentName = reader["department_name"].ToString();
                            item.phone = reader["phone"].ToString();
                            item.fax = reader["fax"].ToString();
                            item.email = reader["email"].ToString();
                            item.group1genus = reader["group_1_genus"].ToString();
                            item.group1species = reader["group_1_species"].ToString();
                            item.group1riskHumanName = reader["group_1_risk_human_name"].ToString();
                            item.group1riskAnimalName = reader["group_1_risk_animal_name"].ToString();
                            item.group1pathogensName = reader["group_1_pathogens_name"].ToString();
                            item.group1virus = reader["group_1_virus"].ToString();
                            item.group1bacteria = reader["group_1_bacteria"].ToString();
                            item.group1paraSit = reader["group_1_paraSit"].ToString();
                            item.group1mold = reader["group_1_mold"].ToString();
                            item.group1protein = reader["group_1_protein"].ToString();
                            item.group2genus = reader["group_2_genus"].ToString();
                            item.group2species = reader["group_2_species"].ToString();
                            item.group2riskHumanName = reader["group_2_risk_human_name"].ToString();
                            item.group2riskAnimalName = reader["group_2_risk_animal_name"].ToString();
                            item.group2pathogensName = reader["group_2_pathogens_name"].ToString();
                            item.group2virus = reader["group_2_virus"].ToString();
                            item.group2bacteria = reader["group_2_bacteria"].ToString();
                            item.group2paraSit = reader["group_2_paraSit"].ToString();
                            item.group2mold = reader["group_2_mold"].ToString();
                            item.group2protein = reader["group_2_protein"].ToString();
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

        public async Task<ModelResponseMessage> AddDocMenuE1Async(ModelMenuE1 model)
        {

            ModelResponseMessage resp = new ModelResponseMessage();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_doc_menu_e1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@section_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.sectionName);
                    cmd.Parameters.Add("@faculty", SqlDbType.Int).Value = model.faculty;
                    cmd.Parameters.Add("@department_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.departmentName);
                    cmd.Parameters.Add("@phone", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.phone);
                    cmd.Parameters.Add("@fax", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.fax);
                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.email);
                    cmd.Parameters.Add("@group_1_genus", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1genus);
                    cmd.Parameters.Add("@group_1_species", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1species);
                    cmd.Parameters.Add("@group_1_risk_human", SqlDbType.Int).Value = model.group1riskHuman;
                    cmd.Parameters.Add("@group_1_risk_animal", SqlDbType.Int).Value = model.group1riskAnimal;
                    cmd.Parameters.Add("@group_1_pathogens", SqlDbType.Int).Value = model.group1pathogens;
                    cmd.Parameters.Add("@group_1_virus", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1virus);
                    cmd.Parameters.Add("@group_1_bacteria", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1bacteria);
                    cmd.Parameters.Add("@group_1_parasit", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1paraSit);
                    cmd.Parameters.Add("@group_1_mold", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1mold);
                    cmd.Parameters.Add("@group_1_protein", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group1protein);
                    cmd.Parameters.Add("@group_2_genus", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2genus);
                    cmd.Parameters.Add("@group_2_species", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2species);
                    cmd.Parameters.Add("@group_2_risk_human", SqlDbType.Int).Value = model.group2riskHuman;
                    cmd.Parameters.Add("@group_2_risk_animal", SqlDbType.Int).Value = model.group2riskAnimal;
                    cmd.Parameters.Add("@group_2_pathogens", SqlDbType.Int).Value = model.group2pathogens;
                    cmd.Parameters.Add("@group_2_virus", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2virus);
                    cmd.Parameters.Add("@group_2_bacteria", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2bacteria);
                    cmd.Parameters.Add("@group_2_parasit", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2paraSit);
                    cmd.Parameters.Add("@group_2_mold", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2mold);
                    cmd.Parameters.Add("@group_2_protein", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.group2protein);

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
