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
using THD.Core.Api.Models.ReportModels;
using static THD.Core.Api.Helpers.ServerDirectorys;
using THD.Core.Api.Models.Config;
using System.IO;

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuC3Repository : IDocMenuC3Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        private readonly IDocMeetingRoundRepository _IDocMeetingRoundRepository;
        private readonly IDocMenuReportRepository _IDocMenuReportRepository;
        private readonly IEnvironmentConfig _IEnvironmentConfig;

        public DocMenuC3Repository(
            IConfiguration configuration,
            IDropdownListRepository DropdownListRepository,
            IRegisterUserRepository IRegisterUserRepository,
            IDocMenuReportRepository DocMenuReportRepository,
            IDocMeetingRoundRepository DocMeetingRoundRepository,
            IEnvironmentConfig EnvironmentConfig)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
            _IDocMeetingRoundRepository = DocMeetingRoundRepository;
            _IDocMenuReportRepository = DocMenuReportRepository;
            _IEnvironmentConfig = EnvironmentConfig;
        }



        // บันทึกการประชุม ------------------------------------------------------------------------------

        public async Task<ModelMenuC3_InterfaceData> MenuC3InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC3_InterfaceData resp = new ModelMenuC3_InterfaceData();

            //คณะกรรมการ
            resp.ListCommittees = new List<ModelSelectOption>();
            resp.ListCommittees = await GetAllCommitteesAsync();

            //ผู้ร่วมประชุม
            resp.ListAttendees = new List<ModelSelectOption>();
            resp.ListAttendees = await GetAllAttendeesAsync();

            int thai_year = CommonData.GetYearOfPeriod();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            resp.ListYearOfProject = await GetListYearOfC3Async();
            ModelSelectOption year_current = new ModelSelectOption() { value = "", label = "" };
            resp.ListYearOfProject.Add(year_current);
            resp.defaultyear = (resp.ListYearOfProject != null) ? resp.ListYearOfProject[0].value : "";

            ModelCountOfYear count_of_year = new ModelCountOfYear();
            count_of_year = await _IDocMeetingRoundRepository.GetMeetingRoundOfProjectAsync(Convert.ToInt32(resp.defaultyear));
            resp.defaultround = count_of_year.count.ToString();

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            resp.defaultmeetingdate = "16/" + DateTime.Now.ToString("MM/yyyy");

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M013");

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllCommitteesAsync()
        {

            string sql = "SELECT register_id, (first_name + full_name) as full_name FROM RegisterUser WHERE IsActive='1' AND Character IN ('2','5','6','7','8') ORDER BY full_name ASC";

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
                            item.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["register_id"].ToString()));
                            item.label = reader["full_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllAttendeesAsync()
        {

            string sql = "SELECT register_id, (first_name + full_name) as full_name " +
                         "FROM RegisterUser WHERE IsActive='1' AND Character IN ('2','5','6','7','8') " +
                         "ORDER BY full_name ASC";

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
                            item.value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader["register_id"].ToString()));
                            item.label = reader["full_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelMenuC3_History>> GetAllHistoryDataC3Async()
        {

            string sql = "SELECT doc_id,year_of_meeting,meeting_round,B.name_thai, " +
                        "meeting_date, meeting_location,committees_array " +
                        "FROM Doc_MenuC3 A " +
                        "LEFT OUTER JOIN MST_MeetingRecordType B " +
                        "ON A.meeting_record_id = B.id " +
                        "ORDER BY doc_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMenuC3_History> e = new List<ModelMenuC3_History>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuC3_History item = new ModelMenuC3_History();
                            item.docid = reader["doc_id"].ToString();
                            item.meetingdate = Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy");
                            item.meetinglocation = reader["meeting_location"].ToString();
                            item.meetingrecordname = reader["name_thai"].ToString();
                            item.meetinground = reader["meeting_round"].ToString();
                            item.yearofmeeting = reader["year_of_meeting"].ToString();
                            item.committeesarray = "";
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;


        }

        private async Task<IList<ModelSelectOption>> GetListYearOfC3Async()
        {
            string sql = "SELECT meeting_year " +
                        "FROM Doc_MeetingRound_Project " +
                        "WHERE isClose = 0 " +
                        "ORDER BY meeting_round ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelSelectOption> e = new List<ModelSelectOption>();
                        while (await reader.ReadAsync())
                        {
                            ModelSelectOption item = new ModelSelectOption();
                            item.value = reader["meeting_year"].ToString();
                            item.label = reader["meeting_year"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelCountOfYearC3> GetDefaultRoundC3Async(int yearof)
        {
            ModelCountOfYearC3 rest = new ModelCountOfYearC3() { count = "" };

            string sql = "SELECT round_of_meeting, meeting_date " +
                         "FROM Doc_MenuC1 " +
                         "WHERE round_of_closed=0 " +
                         "AND year_of_meeting='" + yearof + "'" +
                         "GROUP BY round_of_meeting, meeting_date ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            rest.count = reader["round_of_meeting"].ToString();
                            rest.meetingdate = Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy");
                        }
                    }
                }
                conn.Close();
            }
            return rest;

        }

        public async Task<ModelResponseC3Message> AddDocMenuC3Async(ModelMenuC3 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC3Message resp = new ModelResponseC3Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);
                        cmd.Parameters.Add("@meeting_record_id", SqlDbType.Int).Value = model.meetingrecordid;
                        cmd.Parameters.Add("@meeting_round", SqlDbType.Int).Value = model.meetinground;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@meeting_location", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetinglocation);
                        cmd.Parameters.Add("@meeting_start", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetingstart);
                        cmd.Parameters.Add("@meeting_close", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetingclose);
                        cmd.Parameters.Add("@committees_array", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.committeesarray);
                        cmd.Parameters.Add("@attendees_array", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.attendeesarray);

                        int seq = 1;
                        StringBuilder meeting_user_code_array = new StringBuilder();
                        if (model.committeesarray != null && model.committeesarray.Count > 0)
                        {
                            foreach (var item in model.committeesarray)
                            {
                                meeting_user_code_array.AppendLine(Encoding.UTF8.GetString(Convert.FromBase64String(item.value.Trim())) + ",");
                                seq++;
                            }
                        }
                        cmd.Parameters.Add("@meeting_user_code_array", SqlDbType.NVarChar).Value = meeting_user_code_array.ToString();

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
                            resp.DocId = (int)cmd.Parameters["@rDocId"].Value;

                            model_rpt_15_file rpt = await _IDocMenuReportRepository.GetReportR15Async((int)cmd.Parameters["@rDocId"].Value);

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


        // บันทึกการประชุม 3 แก้ไข

        public async Task<ModelMenuC3_InterfaceData> MenuC3EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC3_InterfaceData resp = new ModelMenuC3_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M013");

            resp.editdata = new ModelMenuC3();
            resp.editdata = await GetMenuC3DataEditAsync(ProectNumber, UserId, resp.UserPermission);

            //คณะกรรมการ
            resp.ListCommittees = new List<ModelSelectOption>();
            resp.ListCommittees = await GetAllCommitteesAsync();

            //ผู้ร่วมประชุม
            resp.ListAttendees = new List<ModelSelectOption>();
            resp.ListAttendees = await GetAllAttendeesAsync();

            resp.ListYearOfProject = new List<ModelSelectOption>();
            ModelSelectOption year_current = new ModelSelectOption()
            {
                value = resp.editdata.yearofmeeting,
                label = resp.editdata.yearofmeeting,
            };
            resp.ListYearOfProject.Add(year_current);

            return resp;
        }

        private async Task<ModelMenuC3> GetMenuC3DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC3 e = new ModelMenuC3();
                        while (await reader.ReadAsync())
                        {
                            e.docid = reader["doc_id"].ToString();
                            e.meetingrecordid = Convert.ToInt32(reader["meeting_type_id"]);
                            e.meetinground = reader["meeting_round"].ToString();
                            e.yearofmeeting = reader["year_of_meeting"].ToString();
                            e.meetingdate = Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy");
                            e.meetinglocation = reader["meeting_location"].ToString();
                            e.meetingstart = reader["meeting_start"].ToString();
                            e.meetingclose = reader["meeting_close"].ToString();
                            e.committeesarray = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["committees_array"].ToString());
                            e.attendeesarray = JsonConvert.DeserializeObject<List<ModelSelectOption>>(reader["attendees_array"].ToString());
                            e.createby = reader["create_by"].ToString();
                            e.meetingresolution = reader["meeting_resolution"].ToString();
                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (string.IsNullOrEmpty(e.meetingresolution))
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC3Message> UpdateDocMenuC3EditAsync(ModelMenuC3 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC3Message resp = new ModelResponseC3Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@meeting_date", SqlDbType.DateTime).Value = Convert.ToDateTime(model.meetingdate);
                        cmd.Parameters.Add("@meeting_record_id", SqlDbType.Int).Value = model.meetingrecordid;
                        cmd.Parameters.Add("@meeting_round", SqlDbType.Int).Value = model.meetinground;
                        cmd.Parameters.Add("@year_of_meeting", SqlDbType.Int).Value = model.yearofmeeting;
                        cmd.Parameters.Add("@meeting_location", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetinglocation);
                        cmd.Parameters.Add("@meeting_start", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetingstart);
                        cmd.Parameters.Add("@meeting_close", SqlDbType.NVarChar).Value = ParseDataHelper.ConvertDBNull(model.meetingclose);
                        cmd.Parameters.Add("@committees_array", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.committeesarray);
                        cmd.Parameters.Add("@attendees_array", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.attendeesarray);

                        int seq = 1;
                        StringBuilder meeting_user_code_array = new StringBuilder();
                        if (model.committeesarray != null && model.committeesarray.Count > 0)
                        {
                            foreach (var item in model.committeesarray)
                            {
                                meeting_user_code_array.AppendLine(Encoding.UTF8.GetString(Convert.FromBase64String(item.value.Trim())) + ",");
                                seq++;
                            }
                        }
                        cmd.Parameters.Add("@meeting_user_code_array", SqlDbType.NVarChar).Value = meeting_user_code_array.ToString();

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            model_rpt_15_file rpt = await _IDocMenuReportRepository.GetReportR15Async(Convert.ToInt32(model.docid));

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




        // ระเบียบวาระที่ 1 ------------------------------------------------------------------------------
        public async Task<ModelMenuC31_InterfaceData> MenuC31InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC31_InterfaceData resp = new ModelMenuC31_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            if (resp.ListMeetingId != null && resp.ListMeetingId.Count > 0)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M014");

            return resp;
        }

        public async Task<ModelResponseC31Message> AddDocMenuC31Async(ModelMenuC31 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC31Message resp = new ModelResponseC31Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_1", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        //Tab 1 Group All
                        IList<ModelMenuC31Tab1GroupAll> list_Tab1_group_all = new List<ModelMenuC31Tab1GroupAll>();

                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq1Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq2Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq3Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }

                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq1Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq2Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq3Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }

                        string tab_1_group_all_json = JsonConvert.SerializeObject(list_Tab1_group_all);

                        cmd.Parameters.Add("@tab_1_group_all_json", SqlDbType.VarChar).Value = tab_1_group_all_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }


        // ระเบียบวาระที่ 1 แก้ไข ------------------------------------------------------------------------------
        public async Task<ModelMenuC31_InterfaceData> MenuC31EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC31_InterfaceData resp = new ModelMenuC31_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M014");

            resp.editdata = new ModelMenuC31();
            resp.editdata = await GetMenuC31DataEditAsync(ProectNumber, UserId, resp.UserPermission);

            resp.ListMeetingId = new List<ModelSelectOption>();
            resp.ListMeetingId = await GetMeetingIdAsync(resp.editdata.meetingid.ToString());

            if (resp.ListMeetingId != null)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            return resp;
        }

        private async Task<ModelMenuC31> GetMenuC31DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3_1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC31 e = new ModelMenuC31();
                        while (await reader.ReadAsync())
                        {
                            e.meetingid = Convert.ToInt32(reader["meeting_id"]);

                            if (reader["group_data"].ToString() == "1.1")
                            {
                                if (Convert.ToInt32(reader["seq"]) == 1)
                                {
                                    e.tab1Group1Seq1Input1 = reader["input1"].ToString();
                                    e.tab1Group1Seq1Input2 = reader["input2"].ToString();
                                    e.tab1Group1Seq1Input3 = reader["input3"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 2)
                                {
                                    e.tab1Group1Seq2Input1 = reader["input1"].ToString();
                                    e.tab1Group1Seq2Input2 = reader["input2"].ToString();
                                    e.tab1Group1Seq2Input3 = reader["input3"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 3)
                                {
                                    e.tab1Group1Seq3Input1 = reader["input1"].ToString();
                                    e.tab1Group1Seq3Input2 = reader["input2"].ToString();
                                    e.tab1Group1Seq3Input3 = reader["input3"].ToString();
                                }
                            }

                            if (reader["group_data"].ToString() == "1.2")
                            {
                                if (Convert.ToInt32(reader["seq"]) == 1)
                                {
                                    e.tab1Group2Seq1Input1 = reader["input1"].ToString();
                                    e.tab1Group2Seq1Input2 = reader["input2"].ToString();
                                    e.tab1Group2Seq1Input3 = reader["input3"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 2)
                                {
                                    e.tab1Group2Seq2Input1 = reader["input1"].ToString();
                                    e.tab1Group2Seq2Input2 = reader["input2"].ToString();
                                    e.tab1Group2Seq2Input3 = reader["input3"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 3)
                                {
                                    e.tab1Group2Seq3Input1 = reader["input1"].ToString();
                                    e.tab1Group2Seq3Input2 = reader["input2"].ToString();
                                    e.tab1Group2Seq3Input3 = reader["input3"].ToString();
                                }
                            }

                            e.createby = reader["create_by"].ToString();
                            e.meetingresolution = reader["meeting_resolution"].ToString();
                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (string.IsNullOrEmpty(e.meetingresolution))
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC31Message> UpdateDocMenuC31EditAsync(ModelMenuC31 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC31Message resp = new ModelResponseC31Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_1_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        //Tab 1 Group All
                        IList<ModelMenuC31Tab1GroupAll> list_Tab1_group_all = new List<ModelMenuC31Tab1GroupAll>();

                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq1Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq2Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab1Group1Seq3Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group1Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }

                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq1Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq2Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab1Group2Seq3Input1))
                                    {
                                        list_Tab1_group_all.Add(new ModelMenuC31Tab1GroupAll
                                        {
                                            groupdata = "1.2",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab1Group2Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }

                        string tab_1_group_all_json = JsonConvert.SerializeObject(list_Tab1_group_all);

                        cmd.Parameters.Add("@tab_1_group_all_json", SqlDbType.VarChar).Value = tab_1_group_all_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }



        // ระเบียบวาระที่ 2 ------------------------------------------------------------------------------
        public async Task<ModelMenuC32_InterfaceData> MenuC32InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC32_InterfaceData resp = new ModelMenuC32_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            resp.isFileAttachment = false;

            if (resp.ListMeetingId != null && resp.ListMeetingId.Count > 0)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
                resp.isFileAttachment = await MenuC32CheckAttachmentAsync(Convert.ToInt32(resp.ListMeetingId.FirstOrDefault().value));
            }

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M015");

            return resp;
        }

        public async Task<bool> MenuC32CheckAttachmentAsync(int meetingid)
        {
            string sql = "SELECT COUNT(input2) AS IsAttachment FROM Doc_MenuC3_Tab2 WHERE meeting_id = '" + meetingid + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            if ((int)reader["IsAttachment"] >= 1) return true;
                        }
                    }
                }
                conn.Close();
            }
            return false;
        }

        public async Task<ModelMenuC32_DownloadFileName> MenuC32DownloadAttachmentNameAsync(int meetingid)
        {

            string sql = "SELECT id,input2 FROM Doc_MenuC3_Tab2 WHERE meeting_id='" + meetingid + "' ORDER BY id ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC32_DownloadFileName e = new ModelMenuC32_DownloadFileName();
                        int rows = 1;
                        while (await reader.ReadAsync())
                        {
                            if (rows == 1 && !string.IsNullOrEmpty(reader["input2"].ToString()))
                            {
                                e.file1name = reader["input2"].ToString();
                            }
                            if (rows == 2 && !string.IsNullOrEmpty(reader["input2"].ToString()))
                            {
                                e.file2name = reader["input2"].ToString();
                            }
                            if (rows == 3 && !string.IsNullOrEmpty(reader["input2"].ToString()))
                            {
                                e.file3name = reader["input2"].ToString();
                            }
                            rows += 1;
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelMenuC32_DownloadFile> GetC32DownloadFileByIdAsync(int meetingid, int id)
        {

            string sql = "SELECT id,input2 FROM Doc_MenuC3_Tab2 WHERE meeting_id='" + meetingid + "' AND seq='" + id + "' ORDER BY id ASC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC32_DownloadFile e = new ModelMenuC32_DownloadFile();
                        while (await reader.ReadAsync())
                        {
                            e.filebase64 = ServerDirectorys.ReadFileToBase64(_IEnvironmentConfig.PathDocument, FolderDocument.menuC3Tab2, reader["input2"].ToString());
                            e.filename = "file_download_" + id + Path.GetExtension(reader["input2"].ToString());
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<ModelResponseC32Message> AddDocMenuC32Async(ModelMenuC32 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC32Message resp = new ModelResponseC32Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        IList<ModelMenuC32Tab2Group1> list_Tab2_group_1 = new List<ModelMenuC32Tab2Group1>();
                        for (int i = 0; i < 10; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq1Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input4).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq2Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input4).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq3Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input4).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }
                        string tab_2_group_1_json = JsonConvert.SerializeObject(list_Tab2_group_1);

                        cmd.Parameters.Add("@tab_2_group_1_json", SqlDbType.VarChar).Value = tab_2_group_1_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }


        // ระเบียบวาระที่ 2 แก้ไข ------------------------------------------------------------------------------
        public async Task<ModelMenuC32_InterfaceData> MenuC32EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC32_InterfaceData resp = new ModelMenuC32_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M015");

            resp.editdata = new ModelMenuC32();
            resp.editdata = await GetMenuC32DataEditAsync(ProectNumber, UserId, resp.UserPermission);

            resp.ListMeetingId = new List<ModelSelectOption>();
            resp.ListMeetingId = await GetMeetingIdAsync(resp.editdata.meetingid.ToString());

            if (resp.ListMeetingId != null)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            return resp;
        }

        private async Task<ModelMenuC32> GetMenuC32DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3_2", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC32 e = new ModelMenuC32();
                        while (await reader.ReadAsync())
                        {
                            e.meetingid = Convert.ToInt32(reader["meeting_id"]);

                            if (reader["group_data"].ToString() == "2.1")
                            {
                                if (Convert.ToInt32(reader["seq"]) == 1)
                                {
                                    e.tab2Group1Seq1Input1 = reader["input1"].ToString();
                                    e.tab2Group1Seq1FileInput2 = reader["input2"].ToString();
                                    e.tab2Group1Seq1Input3 = reader["input3"].ToString();
                                    e.tab2Group1Seq1Input4 = reader["input4"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 2)
                                {
                                    e.tab2Group1Seq2Input1 = reader["input1"].ToString();
                                    e.tab2Group1Seq2FileInput2 = reader["input2"].ToString();
                                    e.tab2Group1Seq2Input3 = reader["input3"].ToString();
                                    e.tab2Group1Seq2Input4 = reader["input4"].ToString();
                                }
                                if (Convert.ToInt32(reader["seq"]) == 3)
                                {
                                    e.tab2Group1Seq3Input1 = reader["input1"].ToString();
                                    e.tab2Group1Seq3FileInput2 = reader["input2"].ToString();
                                    e.tab2Group1Seq3Input3 = reader["input3"].ToString();
                                    e.tab2Group1Seq3Input4 = reader["input4"].ToString();
                                }
                            }

                            e.createby = reader["create_by"].ToString();
                            e.meetingresolution = reader["meeting_resolution"].ToString();
                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (string.IsNullOrEmpty(e.meetingresolution))
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC32Message> UpdateDocMenuC32EditAsync(ModelMenuC32 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC32Message resp = new ModelResponseC32Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_2_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        IList<ModelMenuC32Tab2Group1> list_Tab2_group_1 = new List<ModelMenuC32Tab2Group1>();
                        for (int i = 0; i < 10; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq1Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq1Input4).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq2Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq2Input4).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab2Group1Seq3Input1))
                                    {
                                        list_Tab2_group_1.Add(new ModelMenuC32Tab2Group1
                                        {
                                            groupdata = "2.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3FileInput2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input3).ToString(),
                                            input4 = ParseDataHelper.ConvertDBNull(model.tab2Group1Seq3Input4).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }
                        string tab_2_group_1_json = JsonConvert.SerializeObject(list_Tab2_group_1);

                        cmd.Parameters.Add("@tab_2_group_1_json", SqlDbType.VarChar).Value = tab_2_group_1_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }




        // ระเบียบวาระที่ 3 ------------------------------------------------------------------------------
        public async Task<ModelMenuC33_InterfaceData> MenuC33InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC33_InterfaceData resp = new ModelMenuC33_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            if (resp.ListMeetingId != null && resp.ListMeetingId.Count > 0)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M016");

            if (resp.UserPermission != null && resp.UserPermission.alldata == true)
            {
                resp.ListProjectNumberTab3 = await GetAllProjectForC3Tab3Async("");
            }
            else
            {
                resp.ListProjectNumberTab3 = await GetAllProjectForC3Tab3Async(RegisterId);
            }

            return resp;
        }

        public async Task<ModelResponseC33Message> AddDocMenuC33Async(ModelMenuC33 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC33Message resp = new ModelResponseC33Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;
                        cmd.Parameters.Add("@agenda_3_project_count", SqlDbType.Int).Value = model.agenda3projectcount;
                        cmd.Parameters.Add("@agenda_3_project_number", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnumber);
                        cmd.Parameters.Add("@agenda_3_project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnamethai);
                        cmd.Parameters.Add("@agenda_3_project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnameeng);
                        cmd.Parameters.Add("@agenda_3_conclusion", SqlDbType.Int).Value = model.agenda3Conclusion;
                        cmd.Parameters.Add("@agenda_3_conclusion_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3ConclusionName);
                        cmd.Parameters.Add("@agenda_3_suggestion", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3Suggestion);
                        cmd.Parameters.Add("@safety_type", SqlDbType.Int).Value = model.safetytype;
                        cmd.Parameters.Add("@isClose", SqlDbType.Bit).Value = (model.agenda3Conclusion == "4") ? true : false;

                        cmd.Parameters.Add("@comment_1_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment1title);
                        cmd.Parameters.Add("@comment_1_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment1comittee);
                        cmd.Parameters.Add("@comment_1_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment1note);
                        cmd.Parameters.Add("@comment_2_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment2title);
                        cmd.Parameters.Add("@comment_2_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment2comittee);
                        cmd.Parameters.Add("@comment_2_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment2note);
                        cmd.Parameters.Add("@comment_3_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment3title);
                        cmd.Parameters.Add("@comment_3_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment3comittee);
                        cmd.Parameters.Add("@comment_3_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment3note);

                        cmd.Parameters.Add("@sequel_1_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel1title);
                        cmd.Parameters.Add("@sequel_1_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel1summary);
                        cmd.Parameters.Add("@sequel_1_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel1note);
                        cmd.Parameters.Add("@sequel_2_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel2title);
                        cmd.Parameters.Add("@sequel_2_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel2summary);
                        cmd.Parameters.Add("@sequel_2_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel2note);
                        cmd.Parameters.Add("@sequel_3_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel3title);
                        cmd.Parameters.Add("@sequel_3_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel3summary);
                        cmd.Parameters.Add("@sequel_3_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel3note);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        DateTime dtAlertDate = Convert.ToDateTime(DateTime.Now).AddDays(335);
                        cmd.Parameters.Add("@alert_date", SqlDbType.VarChar, 50).Value = dtAlertDate.ToString("dd/MM/yyyy");

                        DateTime dtExpireDate = Convert.ToDateTime(DateTime.Now).AddDays(365);
                        cmd.Parameters.Add("@certificate_expire_date", SqlDbType.VarChar, 50).Value = dtExpireDate.ToString("dd/MM/yyyy");

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

                            if (model.agenda3Conclusion == "1" || model.agenda3Conclusion == "5")
                            {
                                model_rpt_13_file rpt = await _IDocMenuReportRepository.GetReportR13Async((int)cmd.Parameters["@rDocId"].Value, 3);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
                            if (model.agenda3Conclusion == "2" || model.agenda3Conclusion == "3" || model.agenda3Conclusion == "4")
                            {
                                model_rpt_12_file rpt = await _IDocMenuReportRepository.GetReportR12Async((int)cmd.Parameters["@rDocId"].Value, 3);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }

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

        public async Task<IList<ModelSelectOption>> GetAllProjectForC3Tab3Async(string AssignerCode)
        {

            string sql = "SELECT MAX(B.safety_type) AS first_approval, A.project_number, " +
                        "A.project_name_thai, A.project_name_eng " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[Doc_MenuC2] B " +
                        "ON A.project_number = B.project_number " +
                        "WHERE A.doc_process_to='C2' " +
                        "AND A.project_type='PROJECT' " +
                        "AND A.revert_type='Edit.A4' " +
                        "AND A.user_comment_array IS NOT NULL " +
                        "GROUP BY A.project_number, A.project_name_thai, A.project_name_eng  ";

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

        public async Task<ModelMenuC33Data> GetProjectNumberWithDataC3Tab3Async(string project_number)
        {

            string sql = "SELECT * FROM [dbo].[Doc_Process] " +
                         "WHERE project_number ='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC33Data e = new ModelMenuC33Data();
                        while (await reader.ReadAsync())
                        {
                            e.projectnamethai = reader["project_name_thai"].ToString();
                            e.projectnameeng = reader["project_name_eng"].ToString();
                        }
                        GetProjectDataMenuC2ForTab3Async(ref e, project_number);
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public void GetProjectDataMenuC2ForTab3Async(ref ModelMenuC33Data e, string project_number)
        {
            string sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                             "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                            "FROM Doc_Process Doc " +
                            "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a4_revision = C2.project_revision " +
                            "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                            "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                            "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                            "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'Edit.A4' " +
                            "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                            "ORDER BY C2.doc_id ASC ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand(sql_data, conn))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    int ir = 1;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            switch (ir)
                            {
                                case 1:
                                    e.comment1title = reader["approve_name"].ToString();
                                    e.comment1comittee = reader["name_thai"].ToString();
                                    e.comment1note = reader["comment_consider"].ToString();
                                    break;
                                case 2:
                                    e.comment2title = reader["approve_name"].ToString();
                                    e.comment2comittee = reader["name_thai"].ToString();
                                    e.comment2note = reader["comment_consider"].ToString();
                                    break;
                                case 3:
                                    e.comment3title = reader["approve_name"].ToString();
                                    e.comment3comittee = reader["name_thai"].ToString();
                                    e.comment3note = reader["comment_consider"].ToString();
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }
                            ir++;
                        }
                    }
                }
                conn.Close();
            }
        }

        public async Task<IList<ModelSelectOption>> GetAllApprovalTypeByProjectC2ForTab3Async(string project_number)
        {
            string sql = "SELECT A.project_number, A.approval_type, (B.name_thai + ' ' + A.safety_type + ' ' + B.name_thai_sub) AS approval_name " +
                         "FROM Doc_MenuC2 A INNER JOIN MST_ApprovalType B ON A.approval_type = B.id " +
                         "WHERE A.project_number='" + project_number + "'";

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
                            item.value = reader["approval_type"].ToString();
                            item.label = reader["approval_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelMenuC33HistoryData>> GetAllHistoryDataC3Tab3Async()
        {

            string sql = "SELECT meeting_id, " +
                "('วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) + ' ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + ' ปี ' + CONVERT(VARCHAR, year_of_meeting)) as rptMeetingTitle, " +
                "('3.1.' + CONVERT(VARCHAR, ROW_NUMBER() OVER(PARTITION BY meeting_id ORDER BY id ASC))) AS rptAgenda31, " +
                 "agenda_3_project_count, agenda_3_project_number, " +
                 "agenda_3_project_name_thai, agenda_3_project_name_eng, " +
                 "agenda_3_conclusion_name, agenda_3_suggestion " +
                "FROM Doc_MenuC3_Tab3 A " +
                "INNER JOIN Doc_MenuC3 B " +
                "ON A.meeting_id = B.doc_id " +
                "WHERE group_data = '3.1' " +
                "GROUP BY id,meeting_id, " +
                "agenda_3_project_count,agenda_3_project_number, " +
                "agenda_3_project_name_thai,agenda_3_project_name_eng, " +
                "agenda_3_conclusion_name,agenda_3_suggestion, " +
                "B.doc_id, B.meeting_date, B.meeting_round, B.year_of_meeting " +
                "ORDER BY meeting_id DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMenuC33HistoryData> e = new List<ModelMenuC33HistoryData>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuC33HistoryData item = new ModelMenuC33HistoryData();
                            item.rptMeetingId = reader["meeting_id"].ToString();
                            item.rptMeetingTitle = reader["rptMeetingTitle"].ToString();
                            item.rptAgenda31 = reader["rptAgenda31"].ToString();
                            item.rptProjectCount = reader["agenda_3_project_count"].ToString();
                            item.rptProjectNumber = reader["agenda_3_project_number"].ToString();
                            item.rptProjectNameThai = reader["agenda_3_project_name_thai"].ToString();
                            item.rptProjectNameEng = reader["agenda_3_project_name_eng"].ToString();
                            item.rptConclusionName = reader["agenda_3_conclusion_name"].ToString();
                            item.rptSuggestionName = reader["agenda_3_suggestion"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }



        // ระเบียบวาระที่ 3 แก้ไข ------------------------------------------------------------------------------
        public async Task<ModelMenuC33_InterfaceData> MenuC33EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC33_InterfaceData resp = new ModelMenuC33_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M016");

            resp.editdata = new ModelMenuC33();
            resp.editdata = await GetMenuC33DataEditAsync(ProectNumber, UserId, resp.UserPermission);

            resp.ListMeetingId = new List<ModelSelectOption>();
            resp.ListMeetingId = await GetMeetingIdAsync(resp.editdata.meetingid.ToString());

            if (resp.ListMeetingId != null)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            return resp;
        }

        private async Task<ModelMenuC33> GetMenuC33DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3_3", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC33 e = new ModelMenuC33();
                        while (await reader.ReadAsync())
                        {
                            e.docid = Convert.ToInt32(reader["id"]);
                            e.meetingid = Convert.ToInt32(reader["meeting_id"]);
                            e.agenda3projectcount = Convert.ToInt32(reader["agenda_3_project_count"]);
                            e.agenda3projectnumber = reader["agenda_3_project_number"].ToString();
                            e.agenda3projectnamethai = reader["agenda_3_project_name_thai"].ToString();
                            e.agenda3projectnameeng = reader["agenda_3_project_name_eng"].ToString();
                            e.agenda3Conclusion = reader["agenda_3_Conclusion"].ToString();
                            e.agenda3ConclusionName = reader["agenda_3_Conclusion_Name"].ToString();
                            e.agenda3Suggestion = reader["agenda_3_Suggestion"].ToString();
                            e.createby = reader["create_by"].ToString();
                            e.docprocessfrom = reader["doc_process_from"].ToString();
                            e.safetytype = Convert.ToInt32(reader["safety_type"]);
                            e.safetytypename = reader["safety_type_name"].ToString();

                            e.comment1title = reader["comment_1_title"].ToString();
                            e.comment1comittee = reader["comment_1_comittee"].ToString();
                            e.comment1note = reader["comment_1_note"].ToString();
                            e.comment2title = reader["comment_2_title"].ToString();
                            e.comment2comittee = reader["comment_2_comittee"].ToString();
                            e.comment2note = reader["comment_2_note"].ToString();
                            e.comment3title = reader["comment_3_title"].ToString();
                            e.comment3comittee = reader["comment_3_comittee"].ToString();
                            e.comment3note = reader["comment_3_note"].ToString();

                            e.sequel1title = reader["sequel_1_title"].ToString();
                            e.sequel1summary = reader["sequel_1_summary"].ToString();
                            e.sequel1note = reader["sequel_1_note"].ToString();
                            e.sequel2title = reader["sequel_2_title"].ToString();
                            e.sequel2summary = reader["sequel_2_summary"].ToString();
                            e.sequel2note = reader["sequel_2_note"].ToString();
                            e.sequel3title = reader["sequel_3_title"].ToString();
                            e.sequel3summary = reader["sequel_3_summary"].ToString();
                            e.sequel3note = reader["sequel_3_note"].ToString();

                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (!string.IsNullOrEmpty(e.docprocessfrom) && e.docprocessfrom == "C33")
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC33Message> UpdateDocMenuC33EditAsync(ModelMenuC33 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC33Message resp = new ModelResponseC33Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_3_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;
                        cmd.Parameters.Add("@agenda_3_project_count", SqlDbType.Int).Value = model.agenda3projectcount;
                        cmd.Parameters.Add("@agenda_3_project_number", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnumber);
                        cmd.Parameters.Add("@agenda_3_project_name_thai", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnamethai);
                        cmd.Parameters.Add("@agenda_3_project_name_eng", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3projectnameeng);
                        cmd.Parameters.Add("@agenda_3_conclusion", SqlDbType.Int).Value = model.agenda3Conclusion;
                        cmd.Parameters.Add("@agenda_3_conclusion_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda3ConclusionName);
                        cmd.Parameters.Add("@agenda_3_suggestion", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.agenda3Suggestion);
                        cmd.Parameters.Add("@safety_type", SqlDbType.Int).Value = model.safetytype;
                        cmd.Parameters.Add("@isClose", SqlDbType.Bit).Value = (model.agenda3Conclusion == "4") ? true : false;

                        cmd.Parameters.Add("@comment_1_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment1title);
                        cmd.Parameters.Add("@comment_1_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment1comittee);
                        cmd.Parameters.Add("@comment_1_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment1note);
                        cmd.Parameters.Add("@comment_2_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment2title);
                        cmd.Parameters.Add("@comment_2_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment2comittee);
                        cmd.Parameters.Add("@comment_2_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment2note);
                        cmd.Parameters.Add("@comment_3_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.comment3title);
                        cmd.Parameters.Add("@comment_3_comittee", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment3comittee);
                        cmd.Parameters.Add("@comment_3_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.comment3note);

                        cmd.Parameters.Add("@sequel_1_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel1title);
                        cmd.Parameters.Add("@sequel_1_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel1summary);
                        cmd.Parameters.Add("@sequel_1_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel1note);
                        cmd.Parameters.Add("@sequel_2_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel2title);
                        cmd.Parameters.Add("@sequel_2_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel2summary);
                        cmd.Parameters.Add("@sequel_2_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel2note);
                        cmd.Parameters.Add("@sequel_3_title", SqlDbType.VarChar, 500).Value = ParseDataHelper.ConvertDBNull(model.sequel3title);
                        cmd.Parameters.Add("@sequel_3_summary", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel3summary);
                        cmd.Parameters.Add("@sequel_3_note", SqlDbType.VarChar).Value = ParseDataHelper.ConvertDBNull(model.sequel3note);

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        DateTime dtAlertDate = Convert.ToDateTime(DateTime.Now).AddDays(335);
                        cmd.Parameters.Add("@alert_date", SqlDbType.VarChar, 50).Value = dtAlertDate.ToString("dd/MM/yyyy");

                        DateTime dtExpireDate = Convert.ToDateTime(DateTime.Now).AddDays(365);
                        cmd.Parameters.Add("@certificate_expire_date", SqlDbType.VarChar, 50).Value = dtExpireDate.ToString("dd/MM/yyyy");

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            if (model.agenda3Conclusion == "1" || model.agenda3Conclusion == "5")
                            {
                                model_rpt_13_file rpt = await _IDocMenuReportRepository.GetReportR13Async(model.docid, 3);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
                            if (model.agenda3Conclusion == "2" || model.agenda3Conclusion == "3" || model.agenda3Conclusion == "4")
                            {
                                model_rpt_12_file rpt = await _IDocMenuReportRepository.GetReportR12Async(model.docid, 3);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }

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



        // ระเบียบวาระที่ 4 ------------------------------------------------------------------------------
        public async Task<ModelMenuC34_InterfaceData> MenuC34InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC34_InterfaceData resp = new ModelMenuC34_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            if (resp.ListMeetingId != null && resp.ListMeetingId.Count > 0)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }
            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M017");

            return resp;
        }

        public async Task<ModelResponseC34Message> AddDocMenuC34Async(ModelMenuC34 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC34Message resp = new ModelResponseC34Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_4", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;
                        cmd.Parameters.Add("@comment_1_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input1);
                        cmd.Parameters.Add("@comment_1_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input2);
                        cmd.Parameters.Add("@comment_1_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input3);
                        cmd.Parameters.Add("@comment_2_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input1);
                        cmd.Parameters.Add("@comment_2_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input2);
                        cmd.Parameters.Add("@comment_2_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input3);
                        cmd.Parameters.Add("@comment_3_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input1);
                        cmd.Parameters.Add("@comment_3_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input2);
                        cmd.Parameters.Add("@comment_3_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input3);
                        cmd.Parameters.Add("@agenda_4_term", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.agenda4term);
                        cmd.Parameters.Add("@agenda_4_project_number", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectnumber);
                        cmd.Parameters.Add("@agenda_4_project_name_1", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectname1);
                        cmd.Parameters.Add("@agenda_4_project_name_2", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectname2);
                        cmd.Parameters.Add("@agenda_4_conclusion", SqlDbType.Int).Value = model.agenda4Conclusion;
                        cmd.Parameters.Add("@agenda_4_conclusion_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4ConclusionName);
                        cmd.Parameters.Add("@agenda_4_suggestion", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4Suggestion);
                        cmd.Parameters.Add("@safety_type", SqlDbType.Int).Value = model.safetytype;
                        cmd.Parameters.Add("@file1name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.file1name);
                        cmd.Parameters.Add("@isClose", SqlDbType.Bit).Value = (model.agenda4Conclusion == "4") ? true : false;
                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));


                        DateTime dtAlertDate = Convert.ToDateTime(DateTime.Now).AddDays(335);
                        cmd.Parameters.Add("@alert_date", SqlDbType.VarChar, 50).Value = dtAlertDate.ToString("dd/MM/yyyy");

                        DateTime dtExpireDate = Convert.ToDateTime(DateTime.Now).AddDays(365);
                        cmd.Parameters.Add("@certificate_expire_date", SqlDbType.VarChar, 50).Value = dtExpireDate.ToString("dd/MM/yyyy");


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

                            if (model.agenda4Conclusion == "1" || model.agenda4Conclusion == "5")
                            {
                                model_rpt_13_file rpt = await _IDocMenuReportRepository.GetReportR13Async((int)cmd.Parameters["@rDocId"].Value, 4);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
                            if (model.agenda4Conclusion == "2" || model.agenda4Conclusion == "3" || model.agenda4Conclusion == "4")
                            {
                                model_rpt_12_file rpt = await _IDocMenuReportRepository.GetReportR12Async((int)cmd.Parameters["@rDocId"].Value, 4);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
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

        public async Task<IList<ModelSelectOption>> GetAllProjectNumberTab4Async(int type)
        {
            string sql = "";

            switch (type)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 9:
                    sql = "SELECT * FROM (SELECT MAX(B.safety_type) AS first_approval, A.project_number, " +
                         "A.project_name_thai, A.project_name_eng " +
                         "FROM [dbo].[Doc_Process] A " +
                         "LEFT OUTER JOIN [dbo].[Doc_MenuC2] B " +
                         "ON A.project_number = B.project_number " +
                         "WHERE A.doc_process_to='C2' " +
                         "AND A.project_type='PROJECT' " +
                         "AND A.revert_type='New' " +
                         "GROUP BY A.project_number, A.project_name_thai, A.project_name_eng) AS db " +
                         "WHERE db.first_approval='" + type + "'";
                    break;
                case 8:
                    sql = "SELECT MIN(doc_id) AS first_approval, A.project_number, " +
                          "A.project_name_thai, A.project_name_eng,B.safety_type " +
                          "FROM [dbo].[Doc_Process] A " +
                          "LEFT OUTER JOIN [dbo].[Doc_MenuC2] B " +
                          "ON A.project_number = B.project_number " +
                          "WHERE A.doc_process_to='C22' " +
                          "AND A.project_type='LAB' " +
                          "GROUP BY A.project_number, A.project_name_thai, A.project_name_eng, B.safety_type";
                    break;
                case 5:
                    sql = "SELECT * FROM [dbo].[Doc_Process] " +
                          "WHERE doc_process_from='A6' AND doc_process_to='C34' AND revert_type='Renew.A6' " + // A5 แก้ไขโครงการที่ผ่านการรับรอง
                          "AND project_type='PROJECT' ";
                    break;
                case 6:
                    sql = "SELECT * FROM [dbo].[Doc_Process] " +
                          "WHERE doc_process_from='A5' AND doc_process_to='C34' AND revert_type='Renew.A5' " + // A5 แก้ไขโครงการที่ผ่านการรับรอง
                          "AND project_type='PROJECT' ";
                    break;
                case 7:
                    sql = "SELECT * FROM [dbo].[Doc_Process] " +
                          "WHERE doc_process_from='A7' AND doc_process_to='C34' AND revert_type='Renew.A7' " + // A7 ปิดโครงการ
                          "AND project_type='PROJECT' ";
                    break;
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

        public async Task<ModelMenuC34Tab4Data> GetProjectNumberWithDataC3Tab4Async(int type, string project_number)
        {

            string sql = "SELECT * FROM [dbo].[Doc_Process] WHERE project_number ='" + project_number + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC34Tab4Data e = new ModelMenuC34Tab4Data();

                        string revert_type = "";
                        int a4_revision = 0;
                        int a5_revision = 0;
                        int a6_revision = 0;
                        int a7_revision = 0;
                        while (await reader.ReadAsync())
                        {
                            e.agenda4ProjectName1 = reader["project_name_thai"].ToString();
                            e.agenda4ProjectName2 = reader["project_name_eng"].ToString();

                            revert_type = reader["revert_type"].ToString();
                            a4_revision = Convert.ToInt32(reader["a4_revision"]);
                            a5_revision = Convert.ToInt32(reader["a5_revision"]);
                            a6_revision = Convert.ToInt32(reader["a6_revision"]);
                            a7_revision = Convert.ToInt32(reader["a7_revision"]);
                        }
                        GetProjectDataMenuC22ForTab4Async(ref e, type, project_number, revert_type, a4_revision, a5_revision, a6_revision, a7_revision);
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public void GetProjectDataMenuC22ForTab4Async(ref ModelMenuC34Tab4Data e, int type, string project_number, string revert_type, int a4_revision, int a5_revision, int a6_revision, int a7_revision)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql_data = "";

                switch (type)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                                        "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                                       "FROM Doc_Process Doc " +
                                       "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a4_revision = C2.project_revision " +
                                       "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                                       "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                                       "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                                       "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'New' " +
                                       "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                                       "ORDER BY C2.doc_id ASC ";
                        break;
                    case 5:
                        sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                                        "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                                       "FROM Doc_Process Doc " +
                                       "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a6_revision = C2.project_revision " +
                                       "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                                       "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                                       "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                                       "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'Renew.A6' AND C2.project_revision='" + a6_revision + "' " +
                                       "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                                       "ORDER BY C2.doc_id ASC ";
                        break;
                    case 6:
                        sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                                        "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                                       "FROM Doc_Process Doc " +
                                       "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a5_revision = C2.project_revision " +
                                       "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                                       "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                                       "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                                       "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'Renew.A5' AND C2.project_revision='" + a5_revision + "' " +
                                       "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                                       "ORDER BY C2.doc_id ASC ";
                        break;
                    case 7:
                        sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                                        "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                                       "FROM Doc_Process Doc " +
                                       "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a7_revision = C2.project_revision " +
                                       "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                                       "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                                       "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                                       "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'Renew.A7' AND C2.project_revision='" + a7_revision + "' " +
                                       "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                                       "ORDER BY C2.doc_id ASC ";
                        break;
                    case 8:
                        sql_data = "SELECT D.full_name as approve_name, (C.name_thai + ' ' + C.name_thai_sub) AS name_thai, A.comment_consider " +
                                "FROM Doc_MenuC2_2 A " +
                                "INNER JOIN MST_ApprovalType C ON A.approval_type = C.id " +
                                "INNER JOIN RegisterUser D ON A.assigner_code = D.register_id " +
                                "WHERE project_number='" + project_number + "' ORDER BY doc_id ASC ";
                        break;
                    case 9:
                        sql_data = "SELECT MAX(C2.project_revision),(D.first_name + D.full_name) as approve_name, " +
                                    "(C.name_thai + ' ' + B.name_thai + ' ' + C.name_thai_sub) AS name_thai,C2.comment_consider " +
                                   "FROM Doc_Process Doc " +
                                   "INNER JOIN Doc_MenuC2 C2 ON Doc.project_number = C2.project_number AND Doc.a4_revision = C2.project_revision " +
                                   "INNER JOIN MST_Safety B ON C2.safety_type = B.id " +
                                   "INNER JOIN MST_ApprovalType C ON C2.approval_type = C.id " +
                                   "INNER JOIN RegisterUser D ON C2.assigner_code = D.register_id " +
                                   "WHERE C2.project_number = '" + project_number + "' AND C2.project_revert_type = 'New' " +
                                   "GROUP BY C2.doc_id,C2.project_revision,D.first_name,D.full_name,C.name_thai,B.name_thai,C.name_thai_sub,C2.comment_consider " +
                                   "ORDER BY C2.doc_id ASC ";
                        break;
                }

                using (SqlCommand command = new SqlCommand(sql_data, conn))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    int ir = 1;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            switch (ir)
                            {
                                case 1:
                                    e.tab4Group1Seq1Input1 = reader["approve_name"].ToString();
                                    e.tab4Group1Seq1Input2 = reader["name_thai"].ToString();
                                    e.tab4Group1Seq1Input3 = reader["comment_consider"].ToString();
                                    break;
                                case 2:
                                    e.tab4Group1Seq2Input1 = reader["approve_name"].ToString();
                                    e.tab4Group1Seq2Input2 = reader["name_thai"].ToString();
                                    e.tab4Group1Seq2Input3 = reader["comment_consider"].ToString();
                                    break;
                                case 3:
                                    e.tab4Group1Seq3Input1 = reader["approve_name"].ToString();
                                    e.tab4Group1Seq3Input2 = reader["name_thai"].ToString();
                                    e.tab4Group1Seq3Input3 = reader["comment_consider"].ToString();
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }
                            ir++;
                        }
                    }
                }
                conn.Close();
            }
        }

        public async Task<ModelMenuC34_DownloadFile> GetC34DownloadFileByIdAsync(int docid)
        {

            string sql = "SELECT id,file1name FROM Doc_MenuC3_Tab4 WHERE id='" + docid + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC34_DownloadFile e = new ModelMenuC34_DownloadFile();
                        while (await reader.ReadAsync())
                        {
                            e.filebase64 = ServerDirectorys.ReadFileToBase64(_IEnvironmentConfig.PathDocument, FolderDocument.menuC3Tab4, reader["file1name"].ToString());
                            e.filename = "เอกสารที่เกี่ยวข้อง" + Path.GetExtension(reader["file1name"].ToString());
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        // ระเบียบวาระที่ 4 แก้ไข ------------------------------------------------------------------------------
        public async Task<ModelMenuC34_InterfaceData> MenuC34EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC34_InterfaceData resp = new ModelMenuC34_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M017");

            resp.editdata = new ModelMenuC34();
            resp.editdata = await GetMenuC34DataEditAsync(ProectNumber, UserId, resp.UserPermission);

            resp.ListMeetingId = new List<ModelSelectOption>();
            resp.ListMeetingId = await GetMeetingIdAsync(resp.editdata.meetingid.ToString());

            if (resp.ListMeetingId != null)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            return resp;
        }

        private async Task<ModelMenuC34> GetMenuC34DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3_4", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC34 e = new ModelMenuC34();
                        while (await reader.ReadAsync())
                        {
                            e.docid = Convert.ToInt32(reader["id"]);
                            e.meetingid = Convert.ToInt32(reader["meeting_id"]);

                            e.agenda4term = reader["agenda_4_term"].ToString();

                            switch (e.agenda4term)
                            {
                                case "1":
                                    e.agenda4termname = "1.โครงการใหม่ที่เป็น-ความเสี่ยงประเภท 1";
                                    break;
                                case "2":
                                    e.agenda4termname = "2.โครงการใหม่ที่เป็น-ความเสี่ยงประเภท 2";
                                    break;
                                case "3":
                                    e.agenda4termname = "3.โครงการใหม่ที่เป็น-ความเสี่ยงประเภท 3";
                                    break;
                                case "4":
                                    e.agenda4termname = "4.โครงการใหม่ที่เป็น-ความเสี่ยงประเภท 4";
                                    break;
                                case "5":
                                    e.agenda4termname = "5.โครงการที่เป็น-แจ้งขอต่ออายุใบรับรอง";
                                    break;
                                case "6":
                                    e.agenda4termname = "6.โครงการที่เป็น-แก้ไขโครงการที่ผ่านการรับรองแล้ว";
                                    break;
                                case "7":
                                    e.agenda4termname = "7.โครงการที่เป็น-แจ้งปิดโครงการ";
                                    break;
                                case "8":
                                    e.agenda4termname = "8.คำขอประเมินห้องปฏิบัติการ";
                                    break;
                                case "9":
                                    e.agenda4termname = "9.ผลการตรวจเยี่ยมติดตามโครงการ";
                                    break;
                            }

                            e.agenda4projectnumber = reader["agenda_4_project_number"].ToString();
                            e.agenda4projectname1 = reader["agenda_4_project_name_1"].ToString();
                            e.agenda4projectname2 = reader["agenda_4_project_name_2"].ToString();
                            e.agenda4Conclusion = reader["agenda_4_Conclusion"].ToString();
                            e.agenda4ConclusionName = reader["agenda_4_Conclusion_Name"].ToString();
                            e.agenda4Suggestion = reader["agenda_4_Suggestion"].ToString();
                            e.file1name = reader["file1name"].ToString();
                            e.tab4Group1Seq1Input1 = reader["comment_1_title"].ToString();
                            e.tab4Group1Seq1Input2 = reader["comment_1_comittee"].ToString();
                            e.tab4Group1Seq1Input3 = reader["comment_1_note"].ToString();
                            e.tab4Group1Seq2Input1 = reader["comment_2_title"].ToString();
                            e.tab4Group1Seq2Input2 = reader["comment_2_comittee"].ToString();
                            e.tab4Group1Seq2Input3 = reader["comment_2_note"].ToString();
                            e.tab4Group1Seq3Input1 = reader["comment_3_title"].ToString();
                            e.tab4Group1Seq3Input2 = reader["comment_3_comittee"].ToString();
                            e.tab4Group1Seq3Input3 = reader["comment_3_note"].ToString();
                            e.createby = reader["create_by"].ToString();
                            e.docprocessfrom = reader["doc_process_from"].ToString();
                            e.safetytype = Convert.ToInt32(reader["safety_type"]);
                            e.safetytypename = reader["safety_type_name"].ToString();
                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (!string.IsNullOrEmpty(e.docprocessfrom) && e.docprocessfrom == "C34")
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC34Message> UpdateDocMenuC34EditAsync(ModelMenuC34 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC34Message resp = new ModelResponseC34Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_4_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@doc_id", SqlDbType.Int).Value = model.docid;
                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;
                        cmd.Parameters.Add("@comment_1_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input1);
                        cmd.Parameters.Add("@comment_1_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input2);
                        cmd.Parameters.Add("@comment_1_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq1Input3);
                        cmd.Parameters.Add("@comment_2_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input1);
                        cmd.Parameters.Add("@comment_2_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input2);
                        cmd.Parameters.Add("@comment_2_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq2Input3);
                        cmd.Parameters.Add("@comment_3_title", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input1);
                        cmd.Parameters.Add("@comment_3_comittee", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input2);
                        cmd.Parameters.Add("@comment_3_note", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.tab4Group1Seq3Input3);
                        cmd.Parameters.Add("@agenda_4_term", SqlDbType.VarChar, 2).Value = ParseDataHelper.ConvertDBNull(model.agenda4term);
                        cmd.Parameters.Add("@agenda_4_project_number", SqlDbType.VarChar, 50).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectnumber);
                        cmd.Parameters.Add("@agenda_4_project_name_1", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectname1);
                        cmd.Parameters.Add("@agenda_4_project_name_2", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4projectname2);
                        cmd.Parameters.Add("@agenda_4_conclusion", SqlDbType.Int).Value = model.agenda4Conclusion;
                        cmd.Parameters.Add("@agenda_4_conclusion_name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4ConclusionName);
                        cmd.Parameters.Add("@agenda_4_suggestion", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.agenda4Suggestion);
                        cmd.Parameters.Add("@safety_type", SqlDbType.Int).Value = model.safetytype;
                        cmd.Parameters.Add("@file1name", SqlDbType.VarChar, 200).Value = ParseDataHelper.ConvertDBNull(model.file1name);
                        cmd.Parameters.Add("@isClose", SqlDbType.Bit).Value = (model.agenda4Conclusion == "4") ? true : false;
                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

                        DateTime dtAlertDate = Convert.ToDateTime(DateTime.Now).AddDays(335);
                        cmd.Parameters.Add("@alert_date", SqlDbType.VarChar, 50).Value = dtAlertDate.ToString("dd/MM/yyyy");

                        DateTime dtExpireDate = Convert.ToDateTime(DateTime.Now).AddDays(365);
                        cmd.Parameters.Add("@certificate_expire_date", SqlDbType.VarChar, 50).Value = dtExpireDate.ToString("dd/MM/yyyy");

                        SqlParameter rStatus = cmd.Parameters.Add("@rStatus", SqlDbType.Int);
                        rStatus.Direction = ParameterDirection.Output;
                        SqlParameter rMessage = cmd.Parameters.Add("@rMessage", SqlDbType.NVarChar, 500);
                        rMessage.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        if ((int)cmd.Parameters["@rStatus"].Value > 0)
                        {
                            resp.Status = true;

                            if (model.agenda4Conclusion == "1" || model.agenda4Conclusion == "5")
                            {
                                model_rpt_13_file rpt = await _IDocMenuReportRepository.GetReportR13Async(model.docid, 4);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
                            if (model.agenda4Conclusion == "2" || model.agenda4Conclusion == "3" || model.agenda4Conclusion == "4")
                            {
                                model_rpt_12_file rpt = await _IDocMenuReportRepository.GetReportR12Async(model.docid, 4);

                                resp.filename = rpt.filename;
                                resp.filebase64 = rpt.filebase64;
                            }
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



        // ระเบียบวาระที่ 5 ------------------------------------------------------------------------------
        public async Task<ModelMenuC35_InterfaceData> MenuC35InterfaceDataAsync(string RegisterId)
        {
            ModelMenuC35_InterfaceData resp = new ModelMenuC35_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            if (resp.ListMeetingId != null && resp.ListMeetingId.Count > 0)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }
            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M018");

            return resp;
        }

        public async Task<ModelResponseC35Message> AddDocMenuC35Async(ModelMenuC35 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC35Message resp = new ModelResponseC35Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_5", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        //Tab 5 Group 1
                        IList<ModelMenuC35Tab5Group1> list_tab5_group_1 = new List<ModelMenuC35Tab5Group1>();
                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq1Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq2Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq3Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }
                        string tab_5_group_1_json = JsonConvert.SerializeObject(list_tab5_group_1);

                        cmd.Parameters.Add("@tab_5_group_1_json", SqlDbType.VarChar).Value = tab_5_group_1_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }


        // ระเบียบวาระที่ 5 แก้ไข ------------------------------------------------------------------------------
        public async Task<ModelMenuC35_InterfaceData> MenuC35EditInterfaceDataAsync(string UserId, string ProectNumber)
        {
            ModelMenuC35_InterfaceData resp = new ModelMenuC35_InterfaceData();

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(UserId, "M018");

            resp.editdata = new ModelMenuC35();
            resp.editdata = await GetMenuC35DataEditAsync(ProectNumber, UserId, resp.UserPermission);


            resp.ListMeetingId = new List<ModelSelectOption>();
            resp.ListMeetingId = await GetMeetingIdAsync(resp.editdata.meetingid.ToString());

            if (resp.ListMeetingId != null)
            {
                resp.meetingId = resp.ListMeetingId.FirstOrDefault().value;
                resp.meetingName = resp.ListMeetingId.FirstOrDefault().label;
            }

            return resp;
        }

        private async Task<ModelMenuC35> GetMenuC35DataEditAsync(string project_number, string userid, ModelPermissionPage permission)
        {
            string user_id = Encoding.UTF8.GetString(Convert.FromBase64String(userid));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_getdata_for_c3_5", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@project_number", SqlDbType.VarChar, 50).Value = project_number;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMenuC35 e = new ModelMenuC35();
                        while (await reader.ReadAsync())
                        {
                            e.meetingid = Convert.ToInt32(reader["meeting_id"]);

                            if (Convert.ToInt32(reader["seq"]) == 1)
                            {
                                e.tab5Group1Seq1Input1 = reader["input1"].ToString();
                                e.tab5Group1Seq1Input2 = reader["input2"].ToString();
                                e.tab5Group1Seq1Input3 = reader["input3"].ToString();
                            }
                            if (Convert.ToInt32(reader["seq"]) == 2)
                            {
                                e.tab5Group1Seq2Input1 = reader["input1"].ToString();
                                e.tab5Group1Seq2Input2 = reader["input2"].ToString();
                                e.tab5Group1Seq2Input3 = reader["input3"].ToString();
                            }
                            if (Convert.ToInt32(reader["seq"]) == 3)
                            {
                                e.tab5Group1Seq3Input1 = reader["input1"].ToString();
                                e.tab5Group1Seq3Input2 = reader["input2"].ToString();
                                e.tab5Group1Seq3Input3 = reader["input3"].ToString();
                            }

                            e.createby = reader["create_by"].ToString();
                            e.meetingresolution = reader["meeting_resolution"].ToString();
                        }

                        //Default Edit False
                        e.editenable = false;
                        if (permission.edit == true)
                        {
                            if (string.IsNullOrEmpty(e.meetingresolution))
                            {
                                if (user_id == e.createby)
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

        public async Task<ModelResponseC35Message> UpdateDocMenuC35EditAsync(ModelMenuC35 model)
        {
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            ModelResponseC35Message resp = new ModelResponseC35Message();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_doc_menu_c3_5_edit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@meeting_id", SqlDbType.Int).Value = model.meetingid;

                        //Tab 5 Group 1
                        IList<ModelMenuC35Tab5Group1> list_tab5_group_1 = new List<ModelMenuC35Tab5Group1>();
                        for (int i = 0; i < 3; i++)
                        {
                            string seq = (i + 1).ToString();
                            switch (i + 1)
                            {
                                case 1:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq1Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq1Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 2:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq2Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq2Input3).ToString(),
                                        });
                                    }
                                    break;
                                case 3:
                                    if (!string.IsNullOrEmpty(model.tab5Group1Seq3Input1))
                                    {
                                        list_tab5_group_1.Add(new ModelMenuC35Tab5Group1
                                        {
                                            groupdata = "5.1",
                                            seq = seq,
                                            input1 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input1).ToString(),
                                            input2 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input2).ToString(),
                                            input3 = ParseDataHelper.ConvertDBNull(model.tab5Group1Seq3Input3).ToString(),
                                        });
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }

                        }
                        string tab_5_group_1_json = JsonConvert.SerializeObject(list_tab5_group_1);

                        cmd.Parameters.Add("@tab_5_group_1_json", SqlDbType.VarChar).Value = tab_5_group_1_json;

                        cmd.Parameters.Add("@create_by", SqlDbType.VarChar, 50).Value = Encoding.UTF8.GetString(Convert.FromBase64String(model.createby));

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }


        //พิมพ์วาระการประชุม -------------------------------------------------------
        public async Task<ModelResponseMessageReportAgenda> PrintReportAgendaDraftAsync(int DocId, int Round, int Year)
        {
            ModelResponseMessageReportAgenda resp = new ModelResponseMessageReportAgenda();
            try
            {
                resp.Status = true;

                model_rpt_15_file rpt = await _IDocMenuReportRepository.GetReportR15Async(DocId);

                resp.filename = rpt.filename;

                resp.filebase64 = rpt.filebase64;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        public async Task<ModelResponseMessageReportAgenda> PrintReportAgendaRealAsync(ModelPrintMeeting model)
        {
            ModelResponseMessageReportAgenda resp = new ModelResponseMessageReportAgenda();
            try
            {

                resp.Status = true;

                model_rpt_15_file rpt = await _IDocMenuReportRepository.GetReportR15Async(model.docid);

                resp.filename = rpt.filename;
                resp.filebase64 = rpt.filebase64;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }


        //พิมพ์รายงานการประชุม -------------------------------------------------------

        public async Task<ModelResponseMessageReportMeeting> PrintReportMeetingDraftAsync(int DocId, int Round, int Year)
        {
            ModelResponseMessageReportMeeting resp = new ModelResponseMessageReportMeeting();
            try
            {

                resp.Status = true;

                model_rpt_14_file rpt14 = await _IDocMenuReportRepository.GetReportR14Async(DocId);

                resp.rpt_14_filename = rpt14.filename;

                resp.rpt_14_filebase64 = rpt14.filebase64;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        public async Task<ModelResponseMessageReportMeeting> PrintReportMeetingRealAsync(ModelPrintMeeting model)
        {
            ModelResponseMessageReportMeeting resp = new ModelResponseMessageReportMeeting();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    resp.list_reasearch = new List<ModelResponseDataForSendMail>();
                    resp.list_attendees = new List<ModelResponseDataForSendMail>();

                    string sql = "";

                    // Get Project Header for send mail ------------------------------------------
                    sql = "SELECT Doc.docid_of_meeting,Doc.doc_process_from,Doc.project_number, A1.project_head,Doc.project_name_thai, Doc.project_name_eng, " +
                          "(Regis.first_name + ' ' + Regis.full_name) AS project_header_name, Regis.email, " +
                          "Trans.consider_code " +
                          "FROM [dbo].[Doc_Process] Doc " +
                          "LEFT OUTER JOIN [dbo].[Transaction_Document] Trans ON Doc.project_number = Trans.project_number " +
                          "LEFT OUTER JOIN [dbo].[Doc_MenuA1] A1 ON Doc.project_request_id = A1.doc_id " +
                          "LEFT OUTER JOIN [dbo].[RegisterUser] Regis ON A1.project_head = Regis.register_id " +
                          "WHERE Doc.is_hold=1";

                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                ModelResponseDataForSendMail item = new ModelResponseDataForSendMail();
                                item.ReceiveName = reader["project_header_name"].ToString();
                                item.ReceiveEmail = reader["email"].ToString();
                                item.ProjectNumber = reader["project_number"].ToString();
                                item.ProjectNameThai = reader["project_name_thai"].ToString();
                                item.ProjectNameEng = reader["project_name_eng"].ToString();

                                string doc_process_from = reader["doc_process_from"].ToString();
                                int docid_of_meeting = Convert.ToInt32(reader["docid_of_meeting"]);
                                int conclustion = Convert.ToInt32(reader["consider_code"]);

                                if (conclustion == 1 || conclustion == 5)
                                {
                                    model_rpt_13_file rpt = await _IDocMenuReportRepository.GetReportR13Async(docid_of_meeting, (doc_process_from == "C34" ? 4 : 3));
                                    item.rpt_filename = rpt.filename;
                                    item.rpt_filebase64 = rpt.filebase64;
                                }
                                if (conclustion == 2 || conclustion == 3 || conclustion == 4)
                                {
                                    model_rpt_12_file rpt = await _IDocMenuReportRepository.GetReportR12Async(docid_of_meeting, (doc_process_from == "C34" ? 4 : 3));
                                    item.rpt_filename = rpt.filename;
                                    item.rpt_filebase64 = rpt.filebase64;
                                }

                                resp.list_reasearch.Add(item);
                            }
                        }
                        reader.Close();
                    }

                    // Get Project Header for send mail ------------------------------------------
                    string multi_user = "";

                    sql = "SELECT TOP(1) meeting_user_code_array " +
                          "FROM Doc_MenuC3 " +
                          "WHERE meeting_round='" + model.meetingofround + "' AND year_of_meeting='" + model.meetingofyear + "' ";

                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                multi_user += reader["meeting_user_code_array"].ToString();
                            }
                        }
                        reader.Close();
                    }


                    if (!string.IsNullOrEmpty(multi_user))
                    {
                        string multi_user_code = multi_user.Replace(",\r\n", "','");

                        sql = "SELECT email, (first_name + full_name) as full_name " +
                             "FROM [dbo].[RegisterUser] " +
                             "WHERE register_id IN ('" + multi_user_code + "')";

                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            SqlDataReader reader = await command.ExecuteReaderAsync();

                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    ModelResponseDataForSendMail item = new ModelResponseDataForSendMail();
                                    item.ReceiveName = reader["full_name"].ToString();
                                    item.ReceiveEmail = reader["email"].ToString();
                                    resp.list_attendees.Add(item);
                                }
                            }
                            reader.Close();
                        }
                    }


                    // Close Jobs ----------------------------------------------------------------
                    using (SqlCommand cmd = new SqlCommand("sp_print_report_meeting_real", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@YearOfClose", SqlDbType.Int).Value = model.meetingofyear;
                        cmd.Parameters.Add("@RoundOfClose", SqlDbType.Int).Value = model.meetingofround;

                        string project_number_json = JsonConvert.SerializeObject(resp.list_reasearch);
                        cmd.Parameters.Add("@ProjectNumberJson", SqlDbType.NVarChar).Value = project_number_json;

                        int current_year = CommonData.GetYearOfPeriod();
                        cmd.Parameters.Add("@YearOfNew", SqlDbType.Int).Value = current_year;

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

                            // Get Report ---------------------------------------------------------------------------------------------
                            model_rpt_14_file rpt14 = await _IDocMenuReportRepository.GetReportR14Async(model.docid); // ใช้เลขที่เอกสารเดิม
                            resp.rpt_14_filename = rpt14.filename;
                            resp.rpt_14_filebase64 = rpt14.filebase64;
                            //---------------------------------------------------------------------------------------------------------
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



        // ใช้ร่วมกัน --------------------------------------------------------------------------

        public async Task<IList<ModelSelectOption>> GetAllMeetingIdAsync()
        {

            string sql = "SELECT TOP(5) doc_id as meeting_id, " +
                        "'ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + " +
                        "' ปี ' + CONVERT(VARCHAR, year_of_meeting) + " +
                        "' วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) as meeting_name " +
                        "FROM Doc_MenuC3 " +
                        "WHERE isClosed=0 " +
                        "ORDER BY meeting_id DESC";

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
                            item.value = reader["meeting_id"].ToString();
                            item.label = reader["meeting_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetMeetingIdAsync(string meeting_id)
        {

            string sql = "SELECT TOP(5) doc_id as meeting_id, " +
                        "'ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + " +
                        "' ปี ' + CONVERT(VARCHAR, year_of_meeting) + " +
                        "' วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) as meeting_name " +
                        "FROM Doc_MenuC3 " +
                        "WHERE doc_id='" + meeting_id + "' " +
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
                            ModelSelectOption item = new ModelSelectOption();
                            item.value = reader["meeting_id"].ToString();
                            item.label = reader["meeting_name"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

    }

}
