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
using System.IO;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Native.Web;
using THD.Core.Api.Repository.ReportFiles;
using THD.Core.Api.Models.ReportModels;
using DevExpress.DataAccess.ObjectBinding;
using THD.Core.Api.Models.Config;
using static THD.Core.Api.Models.ReportModels.common_model;

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuReportRepository : IDocMenuReportRepository
    {
        private readonly IEnvironmentConfig _IEnvironmentConfig;
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;

        public DocMenuReportRepository(IEnvironmentConfig EnvironmentConfig, IConfiguration configuration, IDropdownListRepository DropdownListRepository)
        {
            _IEnvironmentConfig = EnvironmentConfig;
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
        }


        // Report 1-2-16 --------------------------------------------------------------------
        public async Task<model_rpt_1_file> GetReportR1_2Async(int doc_id)
        {
            model_rpt_1_file resp = new model_rpt_1_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            try
            {
                model_rpt_1_report rptData1_2 = new model_rpt_1_report();
                model_rpt_16_report rptData16 = new model_rpt_16_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_report_1_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData1_2.doccode = reader["project_type"].ToString() == "1" ? "NUIBC01-2" : "NUIBC02-2";
                                rptData1_2.projecttype = reader["project_type"].ToString();
                                rptData1_2.title = reader["name_thai"].ToString();
                                rptData1_2.Doc_head_2 = reader["doc_number"].ToString();
                                rptData1_2.Doc_head_4 = Convert.ToDateTime(reader["doc_date"]).ToString("dd/MM/yyyy");
                                string presentname = "ข้าพเจ้า " + reader["project_head_name"].ToString() + " สถานะภาพ ";
                                rptData1_2.Presenter_name = ParseDataHelper.ReportTextToHtml(presentname);
                                rptData1_2.Position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData1_2.Position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData1_2.Position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData1_2.Position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData1_2.Position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData1_2.Job_Position = "";
                                rptData1_2.Faculty_name = reader["faculty_name"].ToString();
                                rptData1_2.Research_name_thai = "( " + reader["project_name_thai"].ToString() + " )";
                                rptData1_2.Research_name_eng = "( " + reader["project_name_eng"].ToString() + " )";
                                rptData1_2.Faculty_name = reader["faculty_name"].ToString();

                                rptData1_2.Advisor_fullname = reader["project_consult_name"].ToString();
                                rptData1_2.HeadofResearch_fullname = reader["project_head_name"].ToString();
                                string line1 = reader["faculty_name"].ToString() + "ขอเสนอโครงการวิจัยเรื่อง" + " (" + reader["project_name_thai"].ToString() + ")" + " " + "(" + reader["project_name_eng"].ToString() + ")" + " เพื่อขอรับการพิจารณาด้านความปลอดภัยทางชีวภาพ และได้แนบเอกสารประกอบการพิจารณา ดังนี้";
                                ParseDataHelper.ReportTextToHtml(presentname);
                                rptData1_2.line1 = ParseDataHelper.ReportTextToHtml(line1);

                                MemberProject member1json = new MemberProject();
                                member1json = JsonConvert.DeserializeObject<MemberProject>(reader["member_project_1"].ToString());
                                if (member1json != null)
                                {
                                    var member1 = await GetRegisterInforAsync(member1json.projecthead);
                                    if (member1 != null && member1.Count > 0)
                                        rptData1_2.co_research_fullname1 = member1.FirstOrDefault().name;
                                }

                                MemberProject member2json = new MemberProject();
                                member2json = JsonConvert.DeserializeObject<MemberProject>(reader["member_project_2"].ToString());
                                if (member2json != null)
                                {
                                    var member2 = await GetRegisterInforAsync(member2json.projecthead);
                                    if (member2 != null && member2.Count > 0)
                                        rptData1_2.co_research_fullname2 = member2.FirstOrDefault().name;
                                }

                                MemberProject member3json = new MemberProject();
                                member3json = JsonConvert.DeserializeObject<MemberProject>(reader["member_project_3"].ToString());
                                if (member3json != null)
                                {
                                    var member3 = await GetRegisterInforAsync(member3json.projecthead);
                                    if (member3 != null && member3.Count > 0)
                                        rptData1_2.co_research_fullname3 = member3.FirstOrDefault().name;
                                }

                            }
                        }
                        reader.Close();
                    }


                    using (SqlCommand cmd = new SqlCommand("sp_report_16", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData16.title_report = reader["project_type"].ToString();
                                rptData16.project_head = reader["project_headname"].ToString();
                                rptData16.work_location = reader["labroom"].ToString();
                                rptData16.project_namethai = "(" + reader["project_name_thai"].ToString() + ")";
                                rptData16.project_nameeng = "(" + reader["project_name_eng"].ToString() + " )";
                                rptData16.telephone_no = reader["work_phone"].ToString() + '/' + reader["mobile"].ToString();
                                rptData16.fax_no = reader["fax"].ToString();
                                rptData16.email_address = reader["email"].ToString();
                                rptData16.fund_source = reader["money_supply"].ToString();
                                rptData16.fund_amount = Convert.ToDecimal(reader["budget"]);

                                rptData16.research_group1_type1 = (reader["project_according_type_method"].ToString()) == "1" ? true : false;
                                rptData16.research_group1_type2 = (reader["project_according_type_method"].ToString()) == "2" ? true : false;
                                rptData16.research_group1_type3 = (reader["project_according_type_method"].ToString()) == "3" ? true : false;
                                rptData16.research_group1_type4 = (reader["project_according_type_method"].ToString()) == "4" ? true : false;
                                rptData16.research_group1_other = (reader["project_according_type_method"].ToString()) == "4" ? reader["project_according_other"].ToString() : "";

                                rptData16.research_group2_type1 = (reader["according_type_method"].ToString()) == "1" ? true : false;
                                rptData16.research_group2_type2 = (reader["according_type_method"].ToString()) == "2" ? true : false;
                                rptData16.research_group2_type3 = (reader["according_type_method"].ToString()) == "3" ? true : false;
                                rptData16.research_group2_type4 = (reader["according_type_method"].ToString()) == "4" ? true : false;
                                rptData16.research_group2_type5 = (reader["according_type_method"].ToString()) == "5" ? true : false;
                                rptData16.research_group2_other = (reader["project_according_type_method"].ToString()) == "5" ? reader["project_other"].ToString() : "";

                                rptData16.risk_group_1 = Convert.ToBoolean(reader["risk_group_1"]);
                                rptData16.risk_group_1_1 = Convert.ToBoolean(reader["risk_group_1_1"]);
                                rptData16.risk_group_1_2 = Convert.ToBoolean(reader["risk_group_1_2"]);
                                rptData16.risk_group_1_3 = Convert.ToBoolean(reader["risk_group_1_3"]);
                                rptData16.risk_group_1_4 = Convert.ToBoolean(reader["risk_group_1_4"]);
                                rptData16.risk_group_1_5 = Convert.ToBoolean(reader["risk_group_1_5"]);
                                rptData16.risk_group_1_5_other = (Convert.ToBoolean(reader["risk_group_1_5"]) == true ? reader["risk_group_1_5_other"].ToString() : "");

                                rptData16.risk_group_2 = Convert.ToBoolean(reader["risk_group_2"]);
                                rptData16.risk_group_2_1 = Convert.ToBoolean(reader["risk_group_2_1"]);
                                rptData16.risk_group_2_2 = Convert.ToBoolean(reader["risk_group_2_2"]);
                                rptData16.risk_group_2_3 = Convert.ToBoolean(reader["risk_group_2_3"]);
                                rptData16.risk_group_2_4 = Convert.ToBoolean(reader["risk_group_2_4"]);
                                rptData16.risk_group_2_5 = Convert.ToBoolean(reader["risk_group_2_5"]);

                                rptData16.risk_group_3 = Convert.ToBoolean(reader["risk_group_3"]);
                                rptData16.risk_group_3_1 = Convert.ToBoolean(reader["risk_group_3_1"]);
                                rptData16.risk_group_3_2 = Convert.ToBoolean(reader["risk_group_3_2"]);
                                rptData16.risk_group_3_3 = Convert.ToBoolean(reader["risk_group_3_3"]);
                                rptData16.risk_group_3_4 = Convert.ToBoolean(reader["risk_group_3_4"]);
                                rptData16.risk_group_3_5 = Convert.ToBoolean(reader["risk_group_3_5"]);

                                rptData16.risk_group_4 = Convert.ToBoolean(reader["risk_group_4"]);
                                rptData16.risk_group_4_1 = Convert.ToBoolean(reader["risk_group_4_1"]);
                                rptData16.risk_group_4_2 = Convert.ToBoolean(reader["risk_group_4_2"]);
                                rptData16.risk_group_4_3 = Convert.ToBoolean(reader["risk_group_4_3"]);
                                rptData16.risk_group_4_4 = Convert.ToBoolean(reader["risk_group_4_4"]);
                                rptData16.risk_group_4_5 = Convert.ToBoolean(reader["risk_group_4_5"]);

                            }
                        }
                        reader.Close();
                    }

                    conn.Close();
                }


                rptR1 rpt1 = new rptR1();

                ObjectDataSource ds1_2 = new ObjectDataSource();
                ds1_2.Constructor = new ObjectConstructorInfo();
                ds1_2.DataSource = rptData1_2;

                string report_nameR1_2 = "R1_2_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_pathR1_2 = _IEnvironmentConfig.PathReport + report_nameR1_2;

                rpt1.DataSource = ds1_2;
                rpt1.ExportToPdf(report_full_pathR1_2);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_pathR1_2))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_pathR1_2));
                    fBase64 = readFileByte;
                }
                resp.filename1and2 = report_nameR1_2;
                resp.filebase1and264 = fBase64;



                rptR16 rpt16 = new rptR16();
                ObjectDataSource ds16 = new ObjectDataSource();
                ds16.Constructor = new ObjectConstructorInfo();
                ds16.DataSource = rptData16;

                string report_nameR16 = "R16_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_pathR16 = _IEnvironmentConfig.PathReport + report_nameR16;
                rpt16.DataSource = ds16;
                rpt16.ExportToPdf(report_full_pathR16);
                if (File.Exists(report_full_pathR16))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_pathR16));
                    fBase64 = readFileByte;
                }
                resp.filename16 = report_nameR16;
                resp.filebase1664 = fBase64;


            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 3 --------------------------------------------------------------------------
        public async Task<model_rpt_3_file> GetReportR3Async(int doc_id)
        {
            model_rpt_3_file resp = new model_rpt_3_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_3_report rptData = new model_rpt_3_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                //rptData.projecttype = reader["project_type"].ToString();
                                //rptData.Doc_head_2 = reader["doc_number"].ToString();
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                string present_name = "ข้าพเจ้า  " + reader["project_head_name"].ToString() + "  สถานภาพ  ";
                                rptData.Presenter_name = ParseDataHelper.ReportTextToHtml(present_name);
                                rptData.Position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.Position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.Position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.Position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.Position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData.Job_Position = "";
                                rptData.Faculty_name = reader["faculty_name"].ToString();
                                rptData.HeadofResearch_fullname = reader["project_head_name"].ToString();
                                rptData.certificate_date = reader["Certificate_date"].ToString();
                                rptData.certificate_month = reader["Certificate_month"].ToString();
                                rptData.certificate_year = Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["Certificate_year"])));
                                string line1 = "คณะ  " + reader["faculty_name"].ToString() + " ขอปรับแก้โครงการวิจัยเรื่อง " + " (" + reader["project_name_thai"] + ")" + "  (" + reader["project_name_eng"].ToString() + ") " + Environment.NewLine +
                               "ได้ผ่านการรับรองแบบ/ระดับ                  " + "จากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร เมื่อวันที่  " + reader["Certificate_date"].ToString() + " เดือน  " + reader["Certificate_month"].ToString() + " พ.ศ. " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["Certificate_year"])));
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);

                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR3 rpt = new rptR3();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R3_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 4 --------------------------------------------------------------------------
        public async Task<model_rpt_4_file> GetReportR4Async(int doc_id)
        {
            model_rpt_4_file resp = new model_rpt_4_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_4_report rptData = new model_rpt_4_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_4", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                //rptData.projecttype = reader["project_type"].ToString();
                                //rptData.Doc_head_2 = reader["doc_number"].ToString();
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                string presenter_name = "ข้าพเจ้า  " + reader["project_head_name"].ToString() + "  สถานภาพ  ";
                                rptData.presenter_name = ParseDataHelper.ReportTextToHtml(presenter_name);
                                rptData.position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData.Job_Position = "";
                                string line1 = "คณะ  " + reader["faculty_name"].ToString() + " ขอรายงานความก้าวหน้าโครงการวิจัยเรื่อง " + " (" + reader["project_name_thai"] + ")" + "  (" + reader["project_name_eng"].ToString() + ") " + Environment.NewLine +
                               "ได้ผ่านการรับรองงานวิจัยประเภทที่                  " + "จากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร เมื่อวันที่  " + reader["Certificate_date"].ToString() + " เดือน  " + reader["Certificate_month"].ToString() + " พ.ศ. " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["Certificate_year"])));
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);

                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR4 rpt = new rptR4();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R4_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 5 --------------------------------------------------------------------------
        public async Task<model_rpt_5_file> GetReportR5Async(int doc_id)
        {
            model_rpt_5_file resp = new model_rpt_5_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_5_report rptData = new model_rpt_5_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_5", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                //rptData.projecttype = reader["project_type"].ToString();
                                //rptData.Doc_head_2 = reader["doc_number"].ToString();
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                rptData.month = reader["doc_month"].ToString();
                                rptData.year = CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                string presenter_name = "ข้าพเจ้า  " + reader["project_head_name"].ToString() + "  สถานภาพ  ";
                                rptData.presenter_name = ParseDataHelper.ReportTextToHtml(presenter_name);
                                rptData.position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData.Job_Position = "";
                                rptData.faculty_name = reader["faculty_name"].ToString();
                                rptData.research_name_thai = "ขอรายงานความก้าวหน้าโครงการวิจัยเรื่อง " + "( " + reader["project_name_thai"].ToString() + " )";
                                rptData.research_name_eng = "( " + reader["project_name_eng"].ToString() + " )";
                                rptData.headofresearch_fullname = reader["project_head_name"].ToString();
                                rptData.certificate_type = reader["accept_type_name"].ToString();
                                string line1 = "คณะ  " + reader["faculty_name"].ToString() + " ขอรายงานความก้าวหน้าโครงการวิจัยเรื่อง " + " (" + reader["project_name_thai"] + ")" + "  (" + reader["project_name_eng"].ToString() + ") " + Environment.NewLine +
                               "ได้ผ่านการรับรองงานวิจัยประเภทที่                  " + "จากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร เมื่อวันที่  " + reader["Certificate_date"].ToString() + " เดือน  " + reader["Certificate_month"].ToString() + " พ.ศ. " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["Certificate_year"])));
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);

                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR5 rpt = new rptR5();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R5_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 6 --------------------------------------------------------------------------
        public async Task<model_rpt_6_file> GetReportR6Async(int doc_id)
        {
            model_rpt_6_file resp = new model_rpt_6_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_6_report rptData = new model_rpt_6_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_6", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                //rptData.projecttype = reader["project_type"].ToString();
                                //rptData.Doc_head_2 = reader["doc_number"].ToString();
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                string presenter_name = "ข้าพเจ้า  " + reader["project_head_name"].ToString() + "  สถานภาพ  ";
                                rptData.presenter_name = ParseDataHelper.ReportTextToHtml(presenter_name);
                                rptData.position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData.Job_Position = "";
                                rptData.faculty_name = reader["faculty_name"].ToString();
                                rptData.headofresearch_fullname = reader["project_head_name"].ToString();
                                string line1 = "คณะ  " + reader["faculty_name"].ToString() + " ขอปรับแก้โครงการวิจัยเรื่อง " + " (" + reader["project_name_thai"] + ")" + "  (" + reader["project_name_eng"].ToString() + ") " + Environment.NewLine +
                               "ได้รับการพิจรณาจากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร ครั้งที่          " + " เมื่อวันที่  " + reader["conclusion_date"].ToString() + " เดือน  " + reader["conclusion_month"].ToString() + " พ.ศ. " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["conclusion_year"])));
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);

                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR6 rpt = new rptR6();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R6_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 7 --------------------------------------------------------------------------
        public async Task<model_rpt_7_file> GetReportR7Async(int doc_id)
        {
            model_rpt_7_file resp = new model_rpt_7_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = "";
            int year = System.DateTime.Now.Year;

            try
            {
                model_rpt_7_report rptData = new model_rpt_7_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_7", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.doc = report_no.PadLeft(4, '0');
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                string presenter_name = "ข้าพเจ้า  " + reader["project_head_name"].ToString() + "  สถานภาพ  ";
                                rptData.presenter_name = ParseDataHelper.ReportTextToHtml(presenter_name);
                                rptData.position_1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.position_2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.position_3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.position_4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.position_5 = (reader["check_value"].ToString()) == "5" ? true : false;
                                rptData.Job_Position = "";
                                rptData.faculty_name = reader["faculty_name"].ToString();
                                rptData.headofresearch_fullname = reader["project_head_name"].ToString();
                                string line1 = "คณะ  " + reader["faculty_name"].ToString() + " ขอต่ออายุโครงการวิจัยเรื่อง " + " (" + reader["project_name_thai"] + ")" + "  (" + reader["project_name_eng"].ToString() + ") " + Environment.NewLine +
                               "เลขที่โครงการ NUIBC no " + reader["project_number"].ToString() + "ซึ่งได้ผ่านการรับรองแบบ  " + reader["accept_type_name"].ToString() + " จากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ เมื่อการประชุมครั้งที่ " + reader["renew_round"].ToString() + " และได้แนบเอกสารประกอบการพิจรณา ดังนี้ ";
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);

                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR7 rpt = new rptR7();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R7_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 8 --------------------------------------------------------------------------
        public async Task<model_rpt_8_file> GetReportR8Async(int doc_id, string type)
        {
            model_rpt_8_file resp = new model_rpt_8_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = "";
            int year = System.DateTime.Now.Year;

            try
            {
                model_rpt_8_report rptData = new model_rpt_8_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(type == "B1_2" ? "sp_report_8_1" : "sp_report_8", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                report_no = await CreateReportNumberAsync(year, "R8", doc_id, "Doc_MenuB1");
                                rptData.docno = report_no.PadLeft(4, '0');
                                rptData.day = reader["doc_date"].ToString() + "  " + reader["doc_month"].ToString() + "  " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                rptData.faculty_name = reader["faculty_name"].ToString();
                                rptData.headofresearch_fullname = reader["project_head"].ToString();
                                string line1 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; ตามที่  ท่านได้ส่งเอกสารโครงการวิจัยเพื่อขอรับการพิจารณารับรองด้านความปลอดภัยทาง   ชีวภาพ เรื่อง " + " (" + reader["project_name_thai"].ToString() + ")" + "   " + "(" + reader["project_name_eng"].ToString() + ") " + "  " + reader["project_key_number"].ToString();
                                string line2 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; ในการนี้ คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ได้รับเอกสารโครงการวิจัยของท่านเป็นที่เรียบร้อยแล้ว จึงขอแจ้งให้ทราบ ดังนี้";
                                string line3 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; 1. หมายเลขสำคัญโครงการ (NUIBC No.) คือ " + reader["project_key_number"].ToString() + " ซึ่งในกรณีที่มีการส่งเอกสารใดๆ ที่เกี่ยวข้องกับโครงการนี้ กรุณาระบุหมายเลขสำคัญโครงการ (NUIBC No.) และหากมีการติดตามและสอบถาม รายละเอียดเกี่ยวกับโครงการวิจัยขอให้แจ้งหมายเลขสำคัญโครงการ (NUIBC No.) ดังกล่าวทุกครั้ง ";
                                string line4 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; 2. โครงการวิจัยของท่านจะนำเข้ารับการพิจารณาในรอบการประชุมคณะกรรมการเพื่อความปลอดภัย ทางชีวภาพ ครั้งที่ " + reader["round_of_meeting"].ToString() + '/' + reader["year_of_meeting"].ToString() + " ในเดือน " + reader["meeting_month"].ToString() + " " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["meeting_year"])));

                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);
                                rptData.line3 = ParseDataHelper.ReportTextToHtml(line3);
                                rptData.line4 = ParseDataHelper.ReportTextToHtml(line4);
                            }
                        }
                        reader.Close();

                        rptData.signature_name_1 = await GetChairmanOfBoardAsync();
                    }
                    conn.Close();
                }

                rptR8 rpt = new rptR8();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R8_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 9 --------------------------------------------------------------------------
        public async Task<model_rpt_9_file> GetReportR9Async(int doc_id)
        {
            model_rpt_9_file resp = new model_rpt_9_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_9_report rptData = new model_rpt_9_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_9", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.project_namethai = reader["project_name_thai"].ToString();
                                rptData.project_nameeng = reader["project_name_eng"].ToString();
                                rptData.researcher = reader["project_head_name"].ToString();
                                rptData.advisor = reader["advisorsNameThai"].ToString();
                                rptData.faculty = reader["faculty_name"].ToString();
                                rptData.project_no = reader["project_number"].ToString();
                                rptData.certificate_no = reader["acceptProjectNo"].ToString();
                                rptData.round = reader["RenewRound"].ToString();
                                rptData.certificate_date = reader["accept_date"].ToString();
                                rptData.certificate_month = reader["accept_month"].ToString();
                                rptData.certificate_year = CommonData.ConvertYearToThai(Convert.ToInt32(reader["accept_year"]));
                                rptData.expire_date = reader["expire_date"].ToString() + " " + reader["expire_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["expire_year"]));
                                rptData.expire_month = reader["expire_month"].ToString();
                                rptData.expire_year = CommonData.ConvertYearToThai(Convert.ToInt32(reader["expire_year"]));
                                rptData.projecttype = "งานประเภทที่ " + reader["acceptResult"].ToString();
                                rptData.note = "ข้อเสนอการวิจัยนี้ได้ผ่านการพิจารณาและรับรองจากคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร" +
                                               " ครั้งที่ " + reader["RenewRound"].ToString() + " วันที่ " + Convert.ToDateTime(reader["doc_date"]).ToString("dd MMMM") + " " + CommonData.ConvertYearToThai(Convert.ToDateTime(reader["doc_date"]).Year) +
                                               " เห็นว่ามีความสอดคล้องกับแนวทางปฏิบัติเพื่อความปลอดภัยทางชีวภาพ จึงเห็นควรให้ดำเนินการวิจัยด้านความปลอดภัยทางชีวภาพ ตามข้อเสนอวิจัยนี้ได้";
                                rptData.remark = "ใบรับรองฉบับนี้ใช้ควบคู่กับหนังสือราขการ เล่มที่ ศธ 0527.01.13(1)/ว " +
                                    DateTime.Now.ToString("dd MMMM") + " " + CommonData.ConvertYearToThai(DateTime.Now.Year);
                            }
                        }
                        reader.Close();

                        rptData.signature_name_1 = await GetChairmanOfBoardAsync();
                    }
                    conn.Close();
                }

                rptR9 rpt = new rptR9();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R9_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 10 --------------------------------------------------------------------------
        public async Task<model_rpt_10_file> GetReportR10Async(int doc_id)
        {
            model_rpt_10_file resp = new model_rpt_10_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = await CreateReportNumberAsync(2563, "R8", doc_id, "B1");

            try
            {
                model_rpt_10_report rptData = new model_rpt_10_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_10", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {

                                rptData.projectnamethai = reader["project_name_thai"].ToString();
                                rptData.projectnameeng = reader["project_name_eng"].ToString();
                                rptData.projectheadname = reader["project_head_name"].ToString();
                                rptData.advisor = reader["consult_name"].ToString();
                                rptData.facultyname = reader["faculty_name"].ToString();
                                rptData.chkbox1 = (reader["check_value"].ToString()) == "1" ? true : false;
                                rptData.chkbox2 = (reader["check_value"].ToString()) == "2" ? true : false;
                                rptData.chkbox3 = (reader["check_value"].ToString()) == "3" ? true : false;
                                rptData.chkbox4 = (reader["check_value"].ToString()) == "4" ? true : false;
                                rptData.approvetype1 = (reader["approval_type"].ToString()) == "1" ? reader["approval_type"].ToString() : "-";
                                rptData.approvetype2 = (reader["approval_type"].ToString()) == "2" ? reader["approval_type"].ToString() : "-";
                                rptData.approvetype3 = (reader["approval_type"].ToString()) == "3" ? reader["approval_type"].ToString() : "-";
                                rptData.assignername = reader["assign_name"].ToString();
                                rptData.commentconsider1 = (reader["check_value"].ToString()) == "1" ? reader["comment_consider"].ToString() : "-";
                                rptData.commentconsider2 = (reader["check_value"].ToString()) == "2" ? reader["comment_consider"].ToString() : "-";
                                rptData.commentconsider3 = (reader["check_value"].ToString()) == "3" ? reader["comment_consider"].ToString() : "-";
                                rptData.commentconsider4 = (reader["check_value"].ToString()) == "4" ? reader["comment_consider"].ToString() : "-";
                                rptData.day = "วันที่ " + reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                            }
                            reader.Close();
                        }
                        conn.Close();
                    }

                    rptR10 rpt = new rptR10();

                    ObjectDataSource dataSource = new ObjectDataSource();
                    dataSource.Constructor = new ObjectConstructorInfo();
                    dataSource.DataSource = rptData;

                    string report_name = "R10_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                    string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                    rpt.DataSource = dataSource;
                    rpt.ExportToPdf(report_full_path);

                    string fBase64 = string.Empty;
                    if (File.Exists(report_full_path))
                    {
                        string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                        fBase64 = readFileByte;
                    }

                    resp.filename = report_name;

                    resp.filebase64 = fBase64;
                }
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 11 --------------------------------------------------------------------------
        public async Task<model_rpt_11_file> GetReportR11Async(int doc_id)
        {
            model_rpt_11_file resp = new model_rpt_11_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = "";
            int year = System.DateTime.Now.Year;

            try
            {
                model_rpt_11_report rptData = new model_rpt_11_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_report_11", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.boardcodearray = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["board_code_array"].ToString());

                                report_no = await CreateReportNumberAsync(year, "R11", doc_id, "Doc_MenuC1");
                                rptData.docno = report_no.PadLeft(4, '0');
                                rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                rptData.meet_year = Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["meet_year"])));

                                string linehead1 = "ขอความอนุเคราะห์พิจารณาโครงการวิจัยด้านความปลอดภัยทางชีวภาพ ครั้งที่ " + (reader["round_of_meeting"].ToString() + '/' + reader["year_of_meeting"].ToString());
                                string line1 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; ตามที่ คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร ได้มีกำหนดการประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร ครั้งที่ " + (reader["round_of_meeting"].ToString() + '/' + reader["year_of_meeting"].ToString()) + " ในวันที่ " + reader["meet_date"].ToString() + " " + reader["meet_month"].ToString() + " " + Convert.ToString(CommonData.ConvertYearToThai(Convert.ToInt32(reader["meet_year"]))) + " เพื่อลงมติผลการพิจารณารับรองโครงการวิจัยด้านความปลอดภัยทางชีวภาพ นั้น ";
                                string line2 = "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; ในการนี้ เพื่อลดระยะเวลาในการประชุม จึงขอความอนุเคราะห์ท่านพิจารณาโครงการวิจัย  ที่ขอรับรองด้านความปลอดภัยทางชีวภาพ <bold> จำนวน 1 โครงการ  คือ " + "หมายเลขโครงการ " + reader["project_number"].ToString() + " (" + reader["project_name_thai"].ToString() + ") " + " (" + reader["project_name_eng"].ToString() + ") " + " </bold> ทั้งนี้ ขอความกรุณาพิจารณาและส่งผลการพิจารณา <bold> ภายใน ๑ สัปดาห์หลังได้รับอีเมลล์ </bold> เพื่อฝ่ายเลขานุการดำเนินการสรุปผลการพิจารณาโครงการเข้าวาระการประชุมดังกล่าวฯ ต่อไป";

                                rptData.line_head1 = linehead1;
                                rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);

                            }
                        }
                        reader.Close();

                        rptData.signature_name_1 = await GetChairmanOfBoardAsync();
                    }

                    conn.Close();
                }

                if (rptData.boardcodearray != null && rptData.boardcodearray.Count > 0)
                {
                    int count = 1;
                    foreach (var item in rptData.boardcodearray)
                    {

                        rptData.assign = item.label;

                        rptR11 rpt = new rptR11();

                        ObjectDataSource dataSource = new ObjectDataSource();
                        dataSource.Constructor = new ObjectConstructorInfo();
                        dataSource.DataSource = rptData;

                        string report_name = "R11_" + doc_id + "_" + item.value + "_" + DateTime.Now.ToString("_yyMMddHHmmss").ToString() + ".pdf";
                        string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                        rpt.DataSource = dataSource;
                        rpt.ExportToPdf(report_full_path);

                        string fBase64 = string.Empty;
                        if (File.Exists(report_full_path))
                        {
                            string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                            fBase64 = readFileByte;
                        }

                        if (count == 1)
                        {
                            resp.filename_1 = report_name;
                            resp.filebase_1_64 = fBase64;
                        }
                        if (count == 2)
                        {
                            resp.filename_2 = report_name;
                            resp.filebase_2_64 = fBase64;
                        }
                        if (count == 3)
                        {
                            resp.filename_3 = report_name;
                            resp.filebase_3_64 = fBase64;
                        }

                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 12 --------------------------------------------------------------------------
        public async Task<model_rpt_12_file> GetReportR12Async(int doc_id, int type)
        {
            model_rpt_12_file resp = new model_rpt_12_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = "";
            int year = System.DateTime.Now.Year;

            string line1 = "";
            string line2 = "";
            string line3 = "";
            string line4 = "";
            string line5 = "";
            string line6 = "";
            string line7 = "";
            string line8 = "";
            string line9 = "";
            string line10 = "";

            string store_name = "";

            if (type == 3)
            {
                store_name = "sp_report_12";
            }
            else if (type == 4)
            {
                store_name = "sp_report_12_2";
            }


            try
            {
                model_rpt_12_report rptData = new model_rpt_12_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(store_name, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {

                                if (type == 3)
                                {

                                    report_no = await CreateReportNumberAsync(year, "R12", doc_id, "Doc_MenuC3_Tab3");
                                    rptData.docno = report_no.PadLeft(4, '0');
                                    rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                    rptData.month = reader["doc_month"].ToString();
                                    rptData.year = CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                    rptData.line_head1 = "ผลการประเมินด้านความปลอดภัยทางชีวภาพของการประชุมครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"];
                                    rptData.assignname = reader["sender"].ToString();
                                    line1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ตามที่ท่านได้เสนอโครงการวิจัย" + " (" + reader["agenda_3_project_name_thai"].ToString() + ") (" + reader["agenda_3_project_name_eng"].ToString() + ") " + "หมายเลขสำคัญโครงการ " + reader["agenda_3_project_number"].ToString() + " มายังคณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร เพื่อขอรับการพิจารณารับรองความปลอดภัยทางชีวภาพ นั้น";
                                    line2 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ในการนี้ที่ประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString() + " ได้พิจรณาโครงการวิจัยดังกล่าวแล้ว และได้มีมติ ";
                                    line3 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + reader["comment_1_comittee"].ToString() + reader["comment_1_note"].ToString() + "<br>";
                                    line4 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + reader["comment_2_note"].ToString();

                                    rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                    rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);
                                    rptData.line3 = ParseDataHelper.ReportTextToHtml(line3);
                                    rptData.line4 = ParseDataHelper.ReportTextToHtml(line4);

                                    line5 = "จึงเรียนมาเพื่อโปรดทราบ และดำเนินการตามมติของคณะกรรมการฯ พร้อมจัดทำเอกสาร ดังต่อไปนี้";
                                    line6 = "บันทึกข้อความขอชี้แจง/แก้ไข (NUIBC06) โครงการวิจัยตามมติของคณะกรรมการฯ ดังกล่าว เป็นรายข้อ จำนวน 1 ชุด";
                                    line7 = "เอกสารการขอรับรองฯ ฉบับแก้ไข ตามมติของคณะกรรมการฯ และขีดเส้นใต้ข้อความที่มีการแก้ไขในเอกสาร จำนวน 1 ชุด";
                                    line8 = "สแกนบันทึกข้อความและเอกสารการขอรับรองฉบับแก้ไข (ในรูปแบบ pdf file เท่านั้น) และบันทึกข้อมูลลงใน CD/DVD จำนวน 1 แผ่น";
                                    line9 = "ส่งเอกสารฉบับแก้ไขมายัง คณะกรรมการฯ ภายใน 45 วัน หากพ้นระยะเวลาที่กำหนดท่านจะต้องดำเนินการยื่นขอรับรองฯ ใหม่อีกครั้ง";
                                    line10 = "ทั้งนี้ท่านสามารถติดต่อสอบถามรายละเอียดเพิ่มเติมได้ที่ นางสาวปรางทิพย์  แก้วประสิทธิ์ กองการวิจัยและนวัตกรรม อาคารเอกาทศรถ ชั้น 3 โทรศัพท์ 0 5596 8604 โทรสาร 0 5596 8637 หรือผ่านจดหมายอิเล็กทรอนิกส์ prangtipk@nu.ac.th";

                                    rptData.line5 = ParseDataHelper.ReportTextToHtml(line5);
                                    rptData.line6 = ParseDataHelper.ReportTextToHtml(line6);
                                    rptData.line7 = ParseDataHelper.ReportTextToHtml(line7);
                                    rptData.line8 = ParseDataHelper.ReportTextToHtml(line8);
                                    rptData.line9 = ParseDataHelper.ReportTextToHtml(line9);
                                    rptData.line10 = ParseDataHelper.ReportTextToHtml(line10);


                                }
                                else if (type == 4)
                                {
                                    report_no = await CreateReportNumberAsync(year, "R12", doc_id, "Doc_MenuC3_Tab4");
                                    rptData.docno = report_no.PadLeft(4, '0');
                                    rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));

                                    rptData.line_head1 = "ผลการประเมินด้านความปลอดภัยทางชีวภาพของการประชุมครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"];
                                    rptData.assignname = reader["sender"].ToString();
                                    line1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ตามที่ท่านได้เสนอโครงการวิจัย" + " (" + reader["agenda_4_project_name_1"].ToString() + ") (" + reader["agenda_4_project_name_2"].ToString() + ") " + "หมายเลขสำคัญโครงการ " + reader["agenda_4_project_number"].ToString() + " มายังคณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร เพื่อขอรับการพิจารณารับรองความปลอดภัยทางชีวภาพ นั้น";
                                    line2 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ในการนี้ที่ประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString() + " ได้พิจรณาโครงการวิจัยดังกล่าวแล้ว และได้มีมติ ";
                                    line3 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + reader["comment_1_comittee"].ToString() + reader["comment_1_note"].ToString() + "<br>";
                                    line4 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + reader["comment_2_note"].ToString();

                                    rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                    rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);
                                    rptData.line3 = ParseDataHelper.ReportTextToHtml(line3);
                                    rptData.line4 = ParseDataHelper.ReportTextToHtml(line4);

                                    line5 = "จึงเรียนมาเพื่อโปรดทราบ และดำเนินการตามมติของคณะกรรมการฯ พร้อมจัดทำเอกสาร ดังต่อไปนี้";
                                    line6 = "บันทึกข้อความขอชี้แจง/แก้ไข (NUIBC06) โครงการวิจัยตามมติของคณะกรรมการฯ ดังกล่าว เป็นรายข้อ จำนวน 1 ชุด";
                                    line7 = "เอกสารการขอรับรองฯ ฉบับแก้ไข ตามมติของคณะกรรมการฯ และขีดเส้นใต้ข้อความที่มีการแก้ไขในเอกสาร จำนวน 1 ชุด";
                                    line8 = "สแกนบันทึกข้อความและเอกสารการขอรับรองฉบับแก้ไข (ในรูปแบบ pdf file เท่านั้น) และบันทึกข้อมูลลงใน CD/DVD จำนวน 1 แผ่น";
                                    line9 = "ส่งเอกสารฉบับแก้ไขมายัง คณะกรรมการฯ ภายใน 45 วัน หากพ้นระยะเวลาที่กำหนดท่านจะต้องดำเนินการยื่นขอรับรองฯ ใหม่อีกครั้ง";
                                    line10 = "ทั้งนี้ท่านสามารถติดต่อสอบถามรายละเอียดเพิ่มเติมได้ที่ นางสาวปรางทิพย์  แก้วประสิทธิ์ กองการวิจัยและนวัตกรรม อาคารเอกาทศรถ ชั้น 3 โทรศัพท์ 0 5596 8604 โทรสาร 0 5596 8637 หรือผ่านจดหมายอิเล็กทรอนิกส์ prangtipk@nu.ac.th";

                                    rptData.line5 = ParseDataHelper.ReportTextToHtml(line5);
                                    rptData.line6 = ParseDataHelper.ReportTextToHtml(line6);
                                    rptData.line7 = ParseDataHelper.ReportTextToHtml(line7);
                                    rptData.line8 = ParseDataHelper.ReportTextToHtml(line8);
                                    rptData.line9 = ParseDataHelper.ReportTextToHtml(line9);
                                    rptData.line10 = ParseDataHelper.ReportTextToHtml(line10);
                                }

                            }
                        }
                        reader.Close();

                        rptData.signature_name_1 = await GetChairmanOfBoardAsync();
                    }
                    conn.Close();
                }

                rptR12 rpt = new rptR12();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R12_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 13 --------------------------------------------------------------------------
        public async Task<model_rpt_13_file> GetReportR13Async(int doc_id, int type)
        {
            model_rpt_13_file resp = new model_rpt_13_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = "";
            int year = System.DateTime.Now.Year;
            string line1 = "";
            string line2 = "";
            string line3 = "";

            string store_name = "";

            if (type == 3)
            {
                store_name = "sp_report_13";
            }
            else if (type == 4)
            {
                store_name = "sp_report_13_2";
            }

            try
            {
                model_rpt_13_report rptData = new model_rpt_13_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(store_name, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                if (type == 3)
                                {
                                    report_no = await CreateReportNumberAsync(year, "R13", doc_id, "Doc_MenuC3_Tab3");
                                    rptData.docno = report_no.PadLeft(4, '0');
                                    rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                    rptData.researcher = reader["sender"].ToString();
                                    rptData.linehead = "ผลการประเมินด้านความปลอดภัยทางชีวภาพของการประชุมครั้งที่ " + (reader["meeting_round"].ToString() + '/' + reader["year_of_meeting"].ToString());
                                    line1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ตามที่ท่านได้เสนอโครงการวิจัย" + "&nbsp;" + reader["agenda_3_project_number"] + " (" + reader["agenda_3_project_name_thai"].ToString() + ") (" + reader["agenda_3_project_name_eng"].ToString() + ") " + "มายังคณะกรรมการควบคุมความปลอดภัยทางชีวภาพมหาวิทยาลัยนเรศวร เพื่อขอรับการพิจารณารับรองความปลอดภัยทางชีวภาพนั้น";
                                    line2 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ในการนี้ที่ประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString() + " ได้พิจารณาโครงการวิจัย ดังกล่าวฯแล้วและมีมติ" + "&nbsp;" + "<b>" + reader["agenda_3_conclusion_name"].ToString() + " ประเภทที่ " + reader["safety_type"].ToString() + "</b> โดยให้ท่านดำเนินงานวิจัยตามประกาศมหาวิทยาลัยนเรศวร เรื่องการกำหนดวิธีดำเนินงานตามแนวทางปฏิบัติเพื่อความปลอดภัยทางชีวภาพในกำกับของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพมหาวิทยาลัยนเรศวรประกาศ&nbsp;ณ&nbsp;วันที่&nbsp;12&nbsp;ธันวาคม&nbsp;2555&nbsp;ทั้งนี้ทางคณะกรรมการฯ&nbsp;จะดำเนินการจัดส่งใบรับรองด้านความปลอด&nbsp;ภัยทางชีวภาพฉบับจริงไปที่หน่วยงานของท่านอีก";
                                    line3 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;จึงเรียนมาเพื่อโปรดทราบ&nbsp;ทั้งนี้ท่านสามารถติดต่อสอบถามรายละเอียดเพิ่มเติมได้ที่&nbsp;นางสาวปรางทิพย์&nbsp;แก้วประสิทธิ์&nbsp;กองการวิจัยและนวัตกรรม&nbsp;อาคารเอกาทศรถ&nbsp;ชั้น&nbsp;3&nbsp;โทรศัพท์&nbsp;0&nbsp;5596&nbsp;8604&nbsp;โทรสาร&nbsp;0&nbsp;5596&nbsp;8637&nbsp;หรือผ่านจดหมายอิเล็กทรอนิกส์&nbsp;prangtipk@nu.ac.th";
                                    rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                    rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);
                                    rptData.line3 = ParseDataHelper.ReportTextToHtml(line3);

                                }
                                else if (type == 4)
                                {
                                    report_no = await CreateReportNumberAsync(year, "R13", doc_id, "Doc_MenuC3_Tab4");
                                    rptData.docno = report_no.PadLeft(4, '0');
                                    rptData.day = reader["doc_date"].ToString() + " " + reader["doc_month"].ToString() + " " + CommonData.ConvertYearToThai(Convert.ToInt32(reader["doc_year"]));
                                    rptData.researcher = reader["sender"].ToString();
                                    rptData.linehead = "ผลการประเมินด้านความปลอดภัยทางชีวภาพของการประชุมครั้งที่ " + (reader["meeting_round"].ToString() + '/' + reader["year_of_meeting"].ToString());
                                    line1 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ตามที่ท่านได้เสนอโครงการวิจัย" + "&nbsp;" + reader["agenda_4_project_number"] + " (" + reader["agenda_4_project_name_1"].ToString() + ") (" + reader["agenda_4_project_name_2"].ToString() + ") " + "มายังคณะกรรมการควบคุมความปลอดภัยทางชีวภาพมหาวิทยาลัยนเรศวร เพื่อขอรับการพิจารณารับรองความปลอดภัยทางชีวภาพนั้น";
                                    line2 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ในการนี้ที่ประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString() + " ได้พิจารณาโครงการวิจัย ดังกล่าวฯแล้วและมีมติ" + "&nbsp;" + "<b>" + reader["agenda_4_conclusion_name"].ToString() + " ประเภทที่ " + reader["safety_type"].ToString() + "</b> โดยให้ท่านดำเนินงานวิจัยตามประกาศมหาวิทยาลัยนเรศวร เรื่องการกำหนดวิธีดำเนินงานตามแนวทางปฏิบัติเพื่อความปลอดภัยทางชีวภาพในกำกับของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ&nbsp;มหาวิทยาลัยนเรศวร&nbsp;ประกาศ&nbsp;ณ&nbsp;วันที่&nbsp;12&nbsp;ธันวาคม&nbsp;2555&nbsp;ทั้งนี้ทางคณะกรรมการฯ&nbsp;จะดำเนินการจัดส่งใบรับรองด้านความปลอด&nbsp;ภัยทางชีวภาพฉบับจริงไปที่หน่วยงานของท่านอีก";
                                    line3 = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;จึงเรียนมาเพื่อโปรดทราบ&nbsp;ทั้งนี้ท่านสามารถติดต่อสอบถามรายละเอียดเพิ่มเติมได้ที่&nbsp;นางสาวปรางทิพย์&nbsp;แก้วประสิทธิ์&nbsp;กองการวิจัยและนวัตกรรม&nbsp;อาคารเอกาทศรถ&nbsp;ชั้น&nbsp;3&nbsp;โทรศัพท์&nbsp;0&nbsp;5596&nbsp;8604&nbsp;โทรสาร&nbsp;0&nbsp;5596&nbsp;8637&nbsp;หรือผ่านจดหมายอิเล็กทรอนิกส์&nbsp;prangtipk@nu.ac.th";
                                    rptData.line1 = ParseDataHelper.ReportTextToHtml(line1);
                                    rptData.line2 = ParseDataHelper.ReportTextToHtml(line2);
                                    rptData.line3 = ParseDataHelper.ReportTextToHtml(line3);
                                }
                            }
                        }
                        reader.Close();

                        rptData.signature_name_1 = await GetChairmanOfBoardAsync();
                    }
                    conn.Close();
                }

                rptR13 rpt = new rptR13();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R13_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt.DataSource = dataSource;
                rpt.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }


        // Report 14 --------------------------------------------------------------------------

        public async Task<model_rpt_14_file> GetReportR14Async(int doc_id)
        {
            model_rpt_14_file resp = new model_rpt_14_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_14_report rptData = new model_rpt_14_report();

                //Default ----------------------------------------------------------
                rptData.Doc_head_1 = "รายงานการประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร";
                rptData.Doc_head_2 = "ครั้งที่ - ";
                rptData.Doc_head_3 = "วันที่ -";
                rptData.Doc_head_4 = "เวลา -";
                rptData.Doc_head_5 = "สถานที่ -";

                rptData.list_person_meeting = new List<model_list_person_meeting>();
                rptData.list_person_out_of_meeting = new List<model_list_person_meeting>();
                rptData.list_agenda_1_1 = new List<model_list_agenda_1_1>();
                rptData.list_agenda_1_2 = new List<model_list_agenda_1_2>();
                rptData.list_agenda_2 = new List<model_list_agenda_2>();
                rptData.list_agenda_3 = new List<model_list_agenda_3>();
                rptData.list_agenda_4_1 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_2 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_3 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_4 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_5 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_6 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_7 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_8 = new List<model_list_agenda_4>();
                rptData.list_agenda_4_9 = new List<model_list_agenda_4>();
                rptData.list_agenda_5 = new List<model_list_agenda_5>();
                //--------------------------------------------------------------------

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // ส่วนหัวเอกสาร
                    using (SqlCommand cmd = new SqlCommand("sp_report_14", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.Doc_head_1 = "รายงานการประชุมคณะกรรมการควบคุมความปลอดภัยทางชีวภาพ มหาวิทยาลัยนเรศวร";
                                rptData.Doc_head_2 = "ครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString();

                                int year_meeting = Convert.ToInt32(Convert.ToDateTime(reader["meeting_date"]).ToString("yyyy"));
                                rptData.Doc_head_3 = "วัน" + Convert.ToDateTime(reader["meeting_date"]).ToString("dddd dd MMMM ") + CommonData.ConvertYearToThai(year_meeting);

                                rptData.Doc_head_4 = "เวลา " + ParseDataHelper.ReportEmptyValue(reader["meeting_start"].ToString()) + " เป็นต้นไป";
                                rptData.Doc_head_5 = ParseDataHelper.ReportEmptyValue(reader["meeting_location"].ToString());
                                rptData.Doc_head_6 = "เวลา " + ParseDataHelper.ReportEmptyValue(reader["meeting_close"].ToString());

                                var temp_list_person_meeting = JsonConvert.DeserializeObject<IList<model_list_person_meeting>>(reader["committees_array"].ToString());
                                if (temp_list_person_meeting != null && temp_list_person_meeting.Count > 0)
                                {
                                    int seq = 0;
                                    rptData.list_person_meeting = temp_list_person_meeting;
                                    foreach (var item in rptData.list_person_meeting)
                                    {
                                        var personal = await GetRegisterInforAsync(item.value);
                                        item.seq = (seq + 1).ToString() + ".";
                                        item.position = personal != null ? personal.FirstOrDefault().position : "";
                                        item.department = personal != null ? personal.FirstOrDefault().department : "";
                                        seq++;
                                    }
                                }
                                else
                                {
                                    model_list_person_meeting null_item = new model_list_person_meeting();
                                    null_item.seq = "-";
                                    rptData.list_person_meeting.Add(null_item);
                                }

                                var temp_list_person_out_of_meeting = JsonConvert.DeserializeObject<IList<model_list_person_meeting>>(reader["attendees_array"].ToString());
                                if (temp_list_person_out_of_meeting != null && temp_list_person_out_of_meeting.Count > 0)
                                {
                                    int seq = 0;
                                    rptData.list_person_out_of_meeting = temp_list_person_out_of_meeting;
                                    foreach (var item in rptData.list_person_out_of_meeting)
                                    {
                                        var personal = await GetRegisterInforAsync(item.value);
                                        item.seq = (seq + 1).ToString() + ".";
                                        item.position = personal != null ? personal.FirstOrDefault().position : "";
                                        item.department = personal != null ? personal.FirstOrDefault().department : "";
                                        seq++;
                                    }
                                }
                                else
                                {
                                    model_list_person_meeting null_item = new model_list_person_meeting();
                                    null_item.seq = "-";
                                    rptData.list_person_out_of_meeting.Add(null_item);
                                }

                            }
                        }
                        reader.Close();
                    }

                    // ระเบียบวารที่ 1
                    using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_1", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                string seq = reader["seq"].ToString();
                                string title = "ระเบียบวาระที่ " + (reader["group_data"].ToString() == "1.1" ? "1.1." + seq : "1.2." + seq) + " ";

                                if (reader["group_data"].ToString() == "1.1")
                                {
                                    model_list_agenda_1_1 agenda_1_1 = new model_list_agenda_1_1()
                                    {
                                        subject = title + ParseDataHelper.ReportEmptyValue(reader["input1"].ToString()),
                                        detail_summary = ParseDataHelper.ReportEmptyValue(reader["input2"].ToString()),
                                        detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["input3"].ToString()),
                                    };
                                    rptData.list_agenda_1_1.Add(agenda_1_1);
                                }

                                if (reader["group_data"].ToString() == "1.2")
                                {
                                    model_list_agenda_1_2 agenda_1_2 = new model_list_agenda_1_2()
                                    {
                                        subject = title + ParseDataHelper.ReportEmptyValue(reader["input1"].ToString()),
                                        detail_summary = ParseDataHelper.ReportEmptyValue(reader["input2"].ToString()),
                                        detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["input3"].ToString()),
                                    };
                                    rptData.list_agenda_1_2.Add(agenda_1_2);
                                }
                            }
                        }
                        else
                        {
                            rptData.list_agenda_1_1.Add(new model_list_agenda_1_1());
                            rptData.list_agenda_1_2.Add(new model_list_agenda_1_2());
                        }

                        reader.Close();
                    }

                    // ระเบียบวารที่ 2
                    using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                string seq = reader["seq"].ToString();
                                string title = "ระเบียบวาระที่ 2.1." + seq + " ";

                                model_list_agenda_2 agenda_2 = new model_list_agenda_2()
                                {
                                    subject = title + ParseDataHelper.ReportEmptyValue(reader["input1"].ToString()),
                                    detail_summary = ParseDataHelper.ReportEmptyValue(reader["input3"].ToString()),
                                    detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["input4"].ToString()),
                                };
                                if (reader["group_data"].ToString() == "2.1")
                                    rptData.list_agenda_2.Add(agenda_2);

                            }
                        }
                        else rptData.list_agenda_2.Add(new model_list_agenda_2());

                        reader.Close();

                    }

                    // ระเบียบวารที่ 3 (3.1)
                    using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            int project_count = 0;
                            int seq = 0;
                            while (await reader.ReadAsync())
                            {
                                model_list_agenda_3 agenda_3 = new model_list_agenda_3();

                                project_count += Convert.ToInt32(reader["count_3"]);

                                agenda_3.subject = (seq + 1).ToString() + ". เรื่อง :";
                                agenda_3.project_number = ParseDataHelper.ReportEmptyValue(reader["agenda_3_project_number"].ToString());
                                agenda_3.project_name_thai = ParseDataHelper.ReportEmptyValue(reader["agenda_3_project_name_thai"].ToString());
                                agenda_3.project_name_eng = ParseDataHelper.ReportEmptyValue(reader["agenda_3_project_name_eng"].ToString());
                                agenda_3.project_safety_type = ParseDataHelper.ReportEmptyValue(reader["safety_type"].ToString());
                                agenda_3.list_researchers = ParseDataHelper.ReportEmptyValue(reader["project_head_name"].ToString());
                                agenda_3.consultant_name = ParseDataHelper.ReportEmptyValue(reader["consultant_name"].ToString());
                                agenda_3.comment_1_name = "1. " + ParseDataHelper.ReportEmptyValue(reader["comment_1_title"].ToString());
                                agenda_3.comment_1_desc = ParseDataHelper.ReportEmptyValue(reader["comment_1_note"].ToString());
                                agenda_3.comment_2_name = "2. " + ParseDataHelper.ReportEmptyValue(reader["comment_2_title"].ToString());
                                agenda_3.comment_2_desc = ParseDataHelper.ReportEmptyValue(reader["comment_2_note"].ToString());
                                agenda_3.comment_3_name = "3. " + ParseDataHelper.ReportEmptyValue(reader["comment_3_title"].ToString());
                                agenda_3.comment_3_desc = ParseDataHelper.ReportEmptyValue(reader["comment_3_note"].ToString());
                                agenda_3.conclusion_name = ParseDataHelper.ReportEmptyValue(reader["agenda_3_conclusion_name"].ToString());
                                agenda_3.detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["agenda_3_suggestion"].ToString());

                                agenda_3.list_agenda_3_2 = new List<model_list_agenda_3_2>();

                                if (!string.IsNullOrEmpty(reader["sequel_1_title"].ToString()))
                                {
                                    model_list_agenda_3_2 item_3_2_1 = new model_list_agenda_3_2()
                                    {
                                        sequel_title = "3.2.1 " + ParseDataHelper.ReportEmptyValue(reader["sequel_1_title"].ToString()),
                                        sequel_detail_summary = ParseDataHelper.ReportEmptyValue(reader["sequel_1_summary"].ToString()),
                                        sequel_detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["sequel_1_note"].ToString()),
                                    };
                                    agenda_3.list_agenda_3_2.Add(item_3_2_1);

                                    if (!string.IsNullOrEmpty(reader["sequel_2_title"].ToString()))
                                    {
                                        model_list_agenda_3_2 item_3_2_2 = new model_list_agenda_3_2()
                                        {
                                            sequel_title = "3.2.2 " + ParseDataHelper.ReportEmptyValue(reader["sequel_2_title"].ToString()),
                                            sequel_detail_summary = ParseDataHelper.ReportEmptyValue(reader["sequel_2_summary"].ToString()),
                                            sequel_detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["sequel_2_note"].ToString()),
                                        };
                                        agenda_3.list_agenda_3_2.Add(item_3_2_2);
                                    }

                                    if (!string.IsNullOrEmpty(reader["sequel_3_title"].ToString()))
                                    {
                                        model_list_agenda_3_2 item_3_2_3 = new model_list_agenda_3_2()
                                        {
                                            sequel_title = "3.2.3 " + ParseDataHelper.ReportEmptyValue(reader["sequel_3_title"].ToString()),
                                            sequel_detail_summary = ParseDataHelper.ReportEmptyValue(reader["sequel_3_summary"].ToString()),
                                            sequel_detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["sequel_3_note"].ToString()),
                                        };
                                        agenda_3.list_agenda_3_2.Add(item_3_2_3);
                                    }

                                }
                                else agenda_3.list_agenda_3_2.Add(new model_list_agenda_3_2());

                                rptData.list_agenda_3.Add(agenda_3);
                                seq++;
                            }

                            if (rptData.list_agenda_3 != null && rptData.list_agenda_3.Count > 0)
                            {
                                string title = "โครงการวิจัยที่รับรองหลังจากปรับปรุง/แก้ไข จำนวน ";
                                foreach (var item in rptData.list_agenda_3)
                                {
                                    item.title = title + ParseDataHelper.ReportEmptyValueInt(project_count) + " โครงการ";
                                }
                            }
                        }
                        else
                        {
                            IList<model_list_agenda_3_2> list_agenda_3_2 = new List<model_list_agenda_3_2>();
                            list_agenda_3_2.Add(new model_list_agenda_3_2());
                            rptData.list_agenda_3.Add(new model_list_agenda_3() { list_agenda_3_2 = list_agenda_3_2 });
                        }

                        reader.Close();

                    }

                    // ระเบียบวารที่ 4
                    rptData.list_agenda_4_1 = await GetAgenda4Type1To4(conn, doc_id, 1);
                    rptData.list_agenda_4_2 = await GetAgenda4Type1To4(conn, doc_id, 2);
                    rptData.list_agenda_4_3 = await GetAgenda4Type1To4(conn, doc_id, 3);
                    rptData.list_agenda_4_4 = await GetAgenda4Type1To4(conn, doc_id, 4);
                    rptData.list_agenda_4_5 = await GetAgenda4Type5To9(conn, doc_id, 5);
                    rptData.list_agenda_4_6 = await GetAgenda4Type5To9(conn, doc_id, 6);
                    rptData.list_agenda_4_7 = await GetAgenda4Type5To9(conn, doc_id, 7);
                    rptData.list_agenda_4_8 = await GetAgenda4Type5To9(conn, doc_id, 8);
                    rptData.list_agenda_4_9 = await GetAgenda4Type5To9(conn, doc_id, 9);

                    // ระเบียบวารที่ 5
                    using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_5", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                string seq = reader["seq"].ToString();
                                string title = "ระเบียบวาระที่ 5. " + seq + " ";

                                model_list_agenda_5 agenda_5 = new model_list_agenda_5()
                                {
                                    subject = title + ParseDataHelper.ReportEmptyValue(reader["input1"].ToString()),
                                    detail_summary = ParseDataHelper.ReportEmptyValue(reader["input2"].ToString()),
                                    detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["input3"].ToString()),
                                };
                                if (reader["group_data"].ToString() == "5.1")
                                    rptData.list_agenda_5.Add(agenda_5);

                            }
                        }
                        else rptData.list_agenda_5.Add(new model_list_agenda_5());

                        reader.Close();
                    }

                    rptData.signature_name_1 = await GetChairmanOfBoardAsync();

                    conn.Close();
                }

                rptR14 rpt14 = new rptR14();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R14_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt14.DataSource = dataSource;
                rpt14.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        private async Task<IList<model_list_agenda_4>> GetAgenda4Type1To4(SqlConnection conn, int doc_id, int safety_type)
        {
            IList<model_list_agenda_4> list_agenda_4 = new List<model_list_agenda_4>();

            using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_4_type_1_4", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;
                cmd.Parameters.Add("@safety_type", SqlDbType.Int).Value = safety_type;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                string title = "โครงการวิจัยใหม่ที่เข้าข่ายการพิจารณางานประเภทที่ " + safety_type + " จำนวน ";

                if (reader.HasRows)
                {
                    int seq = 0;
                    while (await reader.ReadAsync())
                    {
                        model_list_agenda_4 agenda_4 = new model_list_agenda_4();

                        agenda_4.title = title + ParseDataHelper.ReportEmptyValueInt(Convert.ToInt32(reader["count_4"])) + " โครงการ ดังนี้";
                        agenda_4.subject = (seq + 1).ToString() + ". เรื่อง :";
                        agenda_4.project_number = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_number"].ToString());
                        agenda_4.project_name_thai = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_name_1"].ToString());
                        agenda_4.project_name_eng = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_name_2"].ToString());
                        agenda_4.project_safety_type = ParseDataHelper.ReportEmptyValue(reader["safety_type"].ToString());
                        agenda_4.list_researchers = ParseDataHelper.ReportEmptyValue(reader["project_head_name"].ToString());
                        agenda_4.consultant_name = ParseDataHelper.ReportEmptyValue(reader["consultant_name"].ToString());
                        agenda_4.comment_1_name = "1. " + (string.IsNullOrEmpty(reader["comment_1_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_1_title"].ToString().Trim()));
                        agenda_4.comment_1_desc = ParseDataHelper.ReportEmptyValue(reader["comment_1_note"].ToString());
                        agenda_4.comment_2_name = "2. " + (string.IsNullOrEmpty(reader["comment_2_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_2_title"].ToString().Trim()));
                        agenda_4.comment_2_desc = ParseDataHelper.ReportEmptyValue(reader["comment_2_note"].ToString());
                        agenda_4.comment_3_name = "3. " + (string.IsNullOrEmpty(reader["comment_3_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_3_title"].ToString().Trim()));
                        agenda_4.comment_3_desc = ParseDataHelper.ReportEmptyValue(reader["comment_3_note"].ToString());
                        agenda_4.conclusion_name = ParseDataHelper.ReportEmptyValue(reader["agenda_4_conclusion_name"].ToString());
                        agenda_4.detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["agenda_4_suggestion"].ToString());

                        list_agenda_4.Add(agenda_4);
                        seq++;
                    }
                }
                else list_agenda_4.Add(new model_list_agenda_4() { title = title + "0 โครงการ ดังนี้" });

                reader.Close();
            }

            return list_agenda_4;
        }

        private async Task<IList<model_list_agenda_4>> GetAgenda4Type5To9(SqlConnection conn, int doc_id, int type)
        {
            IList<model_list_agenda_4> list_agenda_4 = new List<model_list_agenda_4>();

            using (SqlCommand cmd = new SqlCommand("sp_report_14_detail_4_type_5_9", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;
                cmd.Parameters.Add("@type", SqlDbType.Int).Value = type;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                string title = "";

                if (type == 5) title = "โครงการวิจัยที่แจ้งขอต่ออายุใบรับรอง จำนวน ";
                if (type == 6) title = "โครงการวิจัยที่ขอแก้ไขโครงการหลังผ่านการรับรองแล้ว จำนวน ";
                if (type == 7) title = "โครงการวิจัยที่ขอแจ้งปิดโครงการ จำนวน ";
                if (type == 8) title = "คําขอประเมิณห้องปฏิบัติการ จำนวน ";
                if (type == 9) title = "ผลการตรวจเยี่ยมติดตามโครงการ จำนวน ";

                if (reader.HasRows)
                {
                    int seq = 0;
                    while (await reader.ReadAsync())
                    {

                        model_list_agenda_4 agenda_4 = new model_list_agenda_4();

                        agenda_4.title = title + ParseDataHelper.ReportEmptyValueInt(Convert.ToInt32(reader["count_4"])) + " โครงการ ดังนี้";
                        agenda_4.subject = (seq + 1).ToString() + ". เรื่อง :";
                        agenda_4.project_number = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_number"].ToString());
                        agenda_4.project_name_thai = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_name_1"].ToString());
                        agenda_4.project_name_eng = ParseDataHelper.ReportEmptyValue(reader["agenda_4_project_name_2"].ToString());
                        agenda_4.project_safety_type = ParseDataHelper.ReportEmptyValue(reader["safety_type"].ToString());
                        agenda_4.list_researchers = ParseDataHelper.ReportEmptyValue(reader["project_head_name"].ToString());
                        agenda_4.consultant_name = ParseDataHelper.ReportEmptyValue(reader["consultant_name"].ToString());
                        agenda_4.comment_1_name = "1. " + (string.IsNullOrEmpty(reader["comment_1_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_1_title"].ToString().Trim()));
                        agenda_4.comment_1_desc = ParseDataHelper.ReportEmptyValue(reader["comment_1_note"].ToString());
                        agenda_4.comment_2_name = "2. " + (string.IsNullOrEmpty(reader["comment_2_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_2_title"].ToString().Trim()));
                        agenda_4.comment_2_desc = ParseDataHelper.ReportEmptyValue(reader["comment_2_note"].ToString());
                        agenda_4.comment_3_name = "3. " + (string.IsNullOrEmpty(reader["comment_3_note"].ToString()) ? "ไม่มี" : ParseDataHelper.ReportEmptyValue(reader["comment_3_title"].ToString().Trim()));
                        agenda_4.comment_3_desc = ParseDataHelper.ReportEmptyValue(reader["comment_3_note"].ToString());
                        agenda_4.conclusion_name = ParseDataHelper.ReportEmptyValue(reader["agenda_4_conclusion_name"].ToString());
                        agenda_4.detail_conclusion = ParseDataHelper.ReportEmptyValue(reader["agenda_4_suggestion"].ToString());

                        list_agenda_4.Add(agenda_4);
                        seq++;
                    }
                }
                else list_agenda_4.Add(new model_list_agenda_4() { title = title + "0 โครงการ ดังนี้" });

                reader.Close();
            }

            return list_agenda_4;
        }

        public async Task<model_rpt_15_file> GetReportR15Async(int doc_id)
        {
            model_rpt_15_file resp = new model_rpt_15_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {
                model_rpt_15_report rptData = new model_rpt_15_report();

                //Default ----------------------------------------------------------
                rptData.Doc_head_1 = "ระเบียบวารการประชุมคณะกรรมการควบคุมความปลอดภัยทาชีวภาพ มหาวิทยาลัยนเรศวร";
                rptData.Doc_head_2 = "ครั้งที่ - ";
                rptData.Doc_head_3 = "วันที่ -";
                rptData.Doc_head_4 = "เวลา -";
                rptData.Doc_head_5 = "สถานที่ -";

                rptData.subject_1_1 = "1.1 เรื่องที่ประธานแจ้งให้ที่ประชุมทราบ";
                rptData.subject_1_2 = "1.2 เรื่องที่ฝ่ายเลขานุการแจ้งให้ที่ประชุมทราบ";
                rptData.subject_3_1 = "3.1 โครงการวิจัยที่รับรองหลังจากปรับปรุง/แก้ไข";
                rptData.subject_3_1_qty = "จำนวน " + 0 + " โครงการ";
                rptData.subject_3_2 = "3.2 เรื่องสืบเนื่อง";


                rptData.list_item_1_1 = new List<model_rpt_15_items>();
                rptData.list_item_1_2 = new List<model_rpt_15_items>();
                rptData.list_item_2_1 = new List<model_rpt_15_items>();
                rptData.list_item_3_2 = new List<model_rpt_15_items>();
                rptData.list_item_4_1 = new List<model_rpt_15_4>();
                rptData.list_item_5_1 = new List<model_rpt_15_items>();

                model_rpt_15_4 item_4_1 = new model_rpt_15_4()
                {
                    item_4_1_1 = "4.1 โครงการวิจัยใหม่ที่เข้าข่ายการพิจารณางานประเภทที่ 1",
                    item_4_1_2 = "4.2 โครงการวิจัยใหม่ที่เข้าข่ายการพิจารณางานประเภทที่ 2",
                    item_4_1_3 = "4.3 โครงการวิจัยใหม่ที่เข้าข่ายการพิจารณางานประเภทที่ 3",
                    item_4_1_4 = "4.4 โครงการวิจัยใหม่ที่เข้าข่ายการพิจารณางานประเภทที่ 4",
                    item_4_1_5 = "4.5 โครงการวิจัยที่แจ้งขอต่ออายุใบรับรอง",
                    item_4_1_6 = "4.6 โครงการวิจัยที่ขอแก้ไขโครงการหลังผ่านการรับรองแล้ว",
                    item_4_1_7 = "4.7 โครงการวิจัยที่ขอแจ้งปิดโครงการ",
                    item_4_1_8 = "4.8 คำขอประเมิณห้องปฏิบัติการ",
                    item_4_1_9 = "4.9 ผลการตรวจเยี่ยมติดตามโครงการ",
                    item_4_1_1_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_2_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_3_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_4_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_5_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_6_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_7_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_8_qty = "จำนวน " + 0 + " โครงการ",
                    item_4_1_9_qty = "จำนวน " + 0 + " โครงการ",
                };

                //--------------------------------------------------------------------

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // ส่วนหัวเอกสาร
                    using (SqlCommand cmd = new SqlCommand("sp_report_15", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.Doc_head_1 = "ระเบียบวารการประชุมคณะกรรมการควบคุมความปลอดภัยทาชีวภาพ มหาวิทยาลัยนเรศวร";
                                rptData.Doc_head_2 = "ครั้งที่ " + reader["meeting_round"].ToString() + "/" + reader["year_of_meeting"].ToString();

                                int year_meeting = Convert.ToInt32(Convert.ToDateTime(reader["meeting_date"]).ToString("yyyy"));
                                rptData.Doc_head_3 = "วัน" + Convert.ToDateTime(reader["meeting_date"]).ToString("dddd dd MMMM ") + CommonData.ConvertYearToThai(year_meeting);

                                rptData.Doc_head_4 = "เวลา " + ParseDataHelper.ReportEmptyValue(reader["meeting_start"].ToString()) + " เป็นต้นไป";
                                rptData.Doc_head_5 = ParseDataHelper.ReportEmptyValue(reader["meeting_location"].ToString());

                            }
                        }
                        reader.Close();
                    }

                    // ระเบียบวารที่ 1
                    using (SqlCommand cmd = new SqlCommand("sp_report_15_detail_1", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                string string_value = ParseDataHelper.ReportEmptyValue(reader["input1"].ToString());

                                rptData.subject_1_1 = "1.1 เรื่องที่ประธานแจ้งให้ที่ประชุมทราบ";
                                if (reader["group_data"].ToString() == "1.1")
                                {
                                    model_rpt_15_items agenda_1_1 = new model_rpt_15_items()
                                    {
                                        item = "1.1." + reader["seq"].ToString() + " " + string_value,
                                    };
                                    rptData.list_item_1_1.Add(agenda_1_1);
                                };

                                rptData.subject_1_2 = "1.2 เรื่องที่ฝ่ายเลขานุการแจ้งให้ที่ประชุมทราบ";
                                if (reader["group_data"].ToString() == "1.2")
                                {
                                    model_rpt_15_items agenda_1_2 = new model_rpt_15_items()
                                    {
                                        item = "1.2." + reader["seq"].ToString() + " " + string_value,
                                    };
                                    rptData.list_item_1_2.Add(agenda_1_2);
                                };
                            }
                        }
                        else
                        {
                            rptData.list_item_1_1.Add(new model_rpt_15_items());
                            rptData.list_item_1_2.Add(new model_rpt_15_items());
                        }
                        reader.Close();
                    }

                    // ระเบียบวารที่ 2
                    using (SqlCommand cmd = new SqlCommand("sp_report_15_detail_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["group_data"].ToString() == "2.1")
                                {
                                    string string_value = ParseDataHelper.ReportEmptyValue(reader["input1"].ToString());
                                    if (reader["group_data"].ToString() == "2.1")
                                    {
                                        model_rpt_15_items agenda_2_1 = new model_rpt_15_items()
                                        {
                                            item = "2.1." + reader["seq"].ToString() + " " + string_value,
                                        };
                                        rptData.list_item_2_1.Add(agenda_2_1);
                                    };
                                }
                            }
                        }
                        else rptData.list_item_2_1.Add(new model_rpt_15_items());

                        reader.Close();
                    }

                    // ระเบียบวารที่ 3
                    using (SqlCommand cmd = new SqlCommand("sp_report_15_detail_3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            int project_count = 0;
                            while (await reader.ReadAsync())
                            {
                                rptData.subject_3_1 = "3.1 โครงการวิจัยที่รับรองหลังจากปรับปรุง/แก้ไข";
                                if (reader["group_data"].ToString() == "3.1")
                                {
                                    project_count += Convert.ToInt32(reader["count_3"]);

                                    rptData.subject_3_1_qty = "จำนวน " + ParseDataHelper.ReportEmptyValueInt(project_count) + " โครงการ";
                                }

                                rptData.subject_3_2 = "3.2 เรื่องสืบเนื่อง";

                                if (!string.IsNullOrEmpty(reader["sequel_1_title"].ToString()))
                                {
                                    model_rpt_15_items agenda_3_2_1 = new model_rpt_15_items()
                                    {
                                        item = "3.2." + reader["seq"].ToString() + " " + ParseDataHelper.ReportEmptyValue(reader["sequel_1_title"].ToString()),
                                    };
                                    rptData.list_item_3_2.Add(agenda_3_2_1);

                                    if (!string.IsNullOrEmpty(reader["sequel_2_title"].ToString()))
                                    {
                                        model_rpt_15_items agenda_3_2_2 = new model_rpt_15_items()
                                        {
                                            item = "3.2." + reader["seq"].ToString() + " " + ParseDataHelper.ReportEmptyValue(reader["sequel_2_title"].ToString()),
                                        };
                                        rptData.list_item_3_2.Add(agenda_3_2_2);
                                    }

                                    if (!string.IsNullOrEmpty(reader["sequel_3_title"].ToString()))
                                    {
                                        model_rpt_15_items agenda_3_2_3 = new model_rpt_15_items()
                                        {
                                            item = "3.2." + reader["seq"].ToString() + " " + ParseDataHelper.ReportEmptyValue(reader["sequel_3_title"].ToString()),
                                        };
                                        rptData.list_item_3_2.Add(agenda_3_2_3);
                                    }

                                }
                                else rptData.list_item_3_2.Add(new model_rpt_15_items());
                            }
                        }
                        else rptData.list_item_3_2.Add(new model_rpt_15_items());

                        reader.Close();
                    }

                    // ระเบียบวารที่ 4
                    using (SqlCommand cmd = new SqlCommand("sp_report_15_detail_4", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                string string_qty = "จำนวน " + ParseDataHelper.ReportEmptyValueInt(Convert.ToInt32(reader["count_4"])) + " โครงการ";

                                switch (reader["agenda_4_term"].ToString())
                                {
                                    case "1":
                                        item_4_1.item_4_1_1_qty = string_qty;
                                        break;
                                    case "2":
                                        item_4_1.item_4_1_2_qty = string_qty;
                                        break;
                                    case "3":
                                        item_4_1.item_4_1_3_qty = string_qty;
                                        break;
                                    case "4":
                                        item_4_1.item_4_1_4_qty = string_qty;
                                        break;
                                    case "5":
                                        item_4_1.item_4_1_5_qty = string_qty;
                                        break;
                                    case "6":
                                        item_4_1.item_4_1_6_qty = string_qty;
                                        break;
                                    case "7":
                                        item_4_1.item_4_1_7_qty = string_qty;
                                        break;
                                    case "8":
                                        item_4_1.item_4_1_8_qty = string_qty;
                                        break;
                                    case "9":
                                        item_4_1.item_4_1_9_qty = string_qty;
                                        break;
                                }
                            }
                        }
                        reader.Close();

                        rptData.list_item_4_1.Add(item_4_1);
                    }

                    // ระเบียบวารที่ 5
                    using (SqlCommand cmd = new SqlCommand("sp_report_15_detail_5", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["group_data"].ToString() == "5.1")
                                {
                                    string string_value = ParseDataHelper.ReportEmptyValue(reader["input1"].ToString());

                                    model_rpt_15_items agenda_5_1 = new model_rpt_15_items()
                                    {
                                        item = "5." + reader["seq"].ToString() + " " + string_value,
                                    };
                                    rptData.list_item_5_1.Add(agenda_5_1);
                                }
                            }
                        }
                        else rptData.list_item_5_1.Add(new model_rpt_15_items());

                        reader.Close();
                    }

                    conn.Close();
                }

                rptR15 rpt15 = new rptR15();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R15_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                rpt15.DataSource = dataSource;
                rpt15.ExportToPdf(report_full_path);

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        // Report 17-18 --------------------------------------------------------------------------

        public async Task<model_rpt_17_file> GetReportR17_18Async(int doc_id)
        {
            model_rpt_17_file resp = new model_rpt_17_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            string report_no = await CreateReportNumberAsync(2563, "R8", doc_id, "B1");

            try
            {
                model_rpt_17_report rptData = new model_rpt_17_report();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_report_17_18", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.VarChar, 50).Value = doc_id;

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                rptData.projecttype = reader["project_according_type_method"].ToString();
                                rptData.assessment_agent = "";
                                rptData.labLocation = reader["build_location"].ToString();
                                rptData.labboy_name = reader["responsible_person"].ToString();
                                rptData.room_no = reader["room_tel"].ToString();
                                rptData.assessment_date = Convert.ToDateTime(reader["doc_date"]).ToString("dd/MM/yyyy");
                            }
                        }
                        reader.Close();
                    }
                    conn.Close();
                }

                rptR17 rpt17 = new rptR17();
                rptR18 rpt18 = new rptR18();

                ObjectDataSource dataSource = new ObjectDataSource();
                dataSource.Constructor = new ObjectConstructorInfo();
                dataSource.DataSource = rptData;

                string report_name = "R17_18_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path = _IEnvironmentConfig.PathReport + report_name;

                if (rptData != null && rptData.projecttype == "1")
                {
                    rpt17.DataSource = dataSource;
                    rpt17.ExportToPdf(report_full_path);
                }

                if (rptData != null && rptData.projecttype == "2")
                {
                    rpt18.DataSource = dataSource;
                    rpt18.ExportToPdf(report_full_path);
                }

                string fBase64 = string.Empty;
                if (File.Exists(report_full_path))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path));
                    fBase64 = readFileByte;
                }

                resp.filename = report_name;

                resp.filebase64 = fBase64;
            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        public async Task<IList<ModelMenuR1RegisterInfo>> GetRegisterInforAsync(string user_id)
        {

            string sql = "SELECT A.register_id,A.email, " +
                        "(A.first_name + A.full_name) as full_name, B.name_thai as position, C.name_thai as faculty " +
                        "FROM RegisterUser A " +
                        "LEFT OUTER JOIN MST_Position B ON A.position = B.id " +
                        "LEFT OUTER JOIN MST_Faculty C ON A.faculty = C.id " +
                        "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(user_id))
            {
                string userid = Encoding.UTF8.GetString(Convert.FromBase64String(user_id));

                sql += " AND A.register_id ='" + userid + "'";
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMenuR1RegisterInfo> list = new List<ModelMenuR1RegisterInfo>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuR1RegisterInfo e = new ModelMenuR1RegisterInfo();
                            e.registerid = reader["register_id"].ToString();
                            e.email = reader["email"].ToString();
                            e.name = reader["full_name"].ToString();
                            e.position = reader["position"].ToString();
                            e.department = reader["faculty"].ToString();
                            e.faculty = reader["faculty"].ToString();
                            list.Add(e);
                        }
                        return list;
                    }
                }
                conn.Close();
            }
            return null;

        }

        // รายงานหลังจากบันทึกมติที่ประชุมครบเรียบร้อย -----------------------------------------------------

        public async Task<model_rpt_meeting_file> GetAllReportMeetingAsync(int doc_id)
        {
            model_rpt_meeting_file resp = new model_rpt_meeting_file();

            var cultureInfo = new CultureInfo("th-TH");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            try
            {

                string fBase64 = string.Empty;

                // Report 9 ----------------------------------------------------------------------------------------------------------
                rptR9 rpt9 = new rptR9();
                ObjectDataSource ds9 = new ObjectDataSource();
                ds9.Constructor = new ObjectConstructorInfo();
                //ds9.DataSource = rptData;
                string report_name_9 = "meeting_r9_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path_9 = _IEnvironmentConfig.PathReport + report_name_9;
                rpt9.DataSource = ds9;
                rpt9.ExportToPdf(report_full_path_9);
                if (File.Exists(report_full_path_9))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path_9));
                    fBase64 = readFileByte;
                }
                resp.filename9 = report_name_9;
                resp.filebase964 = fBase64;



                // Report 12 ----------------------------------------------------------------------------------------------------------
                rptR12 rpt12 = new rptR12();
                ObjectDataSource ds12 = new ObjectDataSource();
                ds12.Constructor = new ObjectConstructorInfo();
                //ds12.DataSource = rptData;

                string report_name_12 = "meeting_r12_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path_12 = _IEnvironmentConfig.PathReport + report_name_12;

                rpt12.DataSource = ds12;
                rpt12.ExportToPdf(report_full_path_12);

                if (File.Exists(report_full_path_12))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path_12));
                    fBase64 = readFileByte;
                }
                resp.filename12 = report_name_12;
                resp.filebase1264 = fBase64;


                // Report 13 ----------------------------------------------------------------------------------------------------------
                rptR13 rpt13 = new rptR13();
                ObjectDataSource ds13 = new ObjectDataSource();
                ds13.Constructor = new ObjectConstructorInfo();
                //ds13.DataSource = rptData;

                string report_name_13 = "meeting_r13_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path_13 = _IEnvironmentConfig.PathReport + report_name_13;

                rpt13.DataSource = ds13;
                rpt13.ExportToPdf(report_full_path_13);

                if (File.Exists(report_full_path_13))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path_13));
                    fBase64 = readFileByte;
                }
                resp.filename13 = report_name_13;
                resp.filebase1364 = fBase64;

                // Report 14 ----------------------------------------------------------------------------------------------------------
                rptR14 rpt14 = new rptR14();
                ObjectDataSource ds14 = new ObjectDataSource();
                ds14.Constructor = new ObjectConstructorInfo();
                //ds13.DataSource = rptData;

                string report_name_14 = "meeting_r14_" + doc_id + DateTime.Now.ToString("_yyyyMMddHHmmss").ToString() + ".pdf";
                string report_full_path_14 = _IEnvironmentConfig.PathReport + report_name_14;

                rpt14.DataSource = ds14;
                rpt14.ExportToPdf(report_full_path_14);

                if (File.Exists(report_full_path_14))
                {
                    string readFileByte = "data:application/pdf;base64," + Convert.ToBase64String(File.ReadAllBytes(report_full_path_14));
                    fBase64 = readFileByte;
                }
                resp.filename14 = report_name_14;
                resp.filebase1464 = fBase64;

            }
            catch (Exception ex)
            {
                resp.message = ex.Message.ToString();
            }
            return resp;
        }

        private async Task<string> CreateReportNumberAsync(int year, string report_type, int doc_id, string doc_type)
        {

            string report_no = "-";

            switch (doc_type)
            {
                case "B1":
                case "C1":
                case "C3":
                case "C4":
                case "A6":
                    goto GenerateReportNo;
                default:
                    break;
            }

            switch (report_type)
            {
                case "R8":
                case "R11":
                case "R12":
                case "R13":
                case "R7":
                    goto GenerateReportNo;
                default:
                    break;
            }

            GenerateReportNo:
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_create_report_number", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@report_year", SqlDbType.Int).Value = year;
                    cmd.Parameters.Add("@report_type", SqlDbType.VarChar, 20).Value = report_type;
                    cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = doc_id;
                    cmd.Parameters.Add("@doc_type", SqlDbType.VarChar, 20).Value = doc_type;

                    SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                    rStatus.Direction = ParameterDirection.Output;
                    SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                    rMessage.Direction = ParameterDirection.Output;
                    SqlParameter rDocId = cmd.Parameters.Add("@rReportNo", SqlDbType.VarChar, 10);
                    rDocId.Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();

                    if ((int)cmd.Parameters["@rStatus"].Value > 0)
                    {
                        report_no = (string)cmd.Parameters["@rReportNo"].Value;
                    }
                }
                conn.Close();
            }

            return report_no;
        }

        public async Task<string> GetChairmanOfBoardAsync()
        {
            string sql = "SELECT register_id,(first_name + full_name) as chairman_name FROM [dbo].[RegisterUser] WHERE character=5";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<chair_of_board> list = new List<chair_of_board>();
                        while (await reader.ReadAsync())
                        {
                            chair_of_board e = new chair_of_board();
                            e.registerid = reader["register_id"].ToString();
                            e.fullname = reader["chairman_name"].ToString();
                            list.Add(e);
                        }
                        return list.FirstOrDefault().fullname;
                    }
                }
                conn.Close();
            }
            return null;

        }

    }

}
