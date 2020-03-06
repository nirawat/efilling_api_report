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
    public class DocMenuC34HistoryRepository : IDocMenuC34HistoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        public DocMenuC34HistoryRepository(IConfiguration configuration, IDropdownListRepository DropdownListRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
        }

        // ระเบียบวาระที่ 4 ------------------------------------------------------------------------------
        public async Task<ModelMenuC3Tab4_InterfaceData_History> MenuC3Tab4InterfaceHistoryDataAsync()
        {
            ModelMenuC3Tab4_InterfaceData_History resp = new ModelMenuC3Tab4_InterfaceData_History();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            ModelSelectOption all_meeting = new ModelSelectOption() { value = "", label = "เลือก..." };
            resp.ListMeetingId.Add(all_meeting);

            resp.ListApprovalType = await GetAllApprovalTypeAsync();

            ModelSelectOption all_approval_type = new ModelSelectOption() { value = "", label = "เลือก..." };
            resp.ListApprovalType.Add(all_approval_type);

            resp.ListConsiderType = await GetAllConsiderTypeAsync();

            ModelSelectOption all_consider_type = new ModelSelectOption() { value = "", label = "เลือก..." };
            resp.ListConsiderType.Add(all_consider_type);

            resp.ListReportData = await GetAllReportHistoryDataC3Tab4Async(null);

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllApprovalTypeAsync()
        {
            string sql = "SELECT id, (name_thai + ' ' + name_thai_sub) AS approval_name FROM MST_ApprovalType ORDER BY id ASC";

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

        public async Task<IList<ModelSelectOption>> GetAllConsiderTypeAsync()
        {
            string sql = "SELECT id, name_thai FROM MST_Consider ORDER BY id ASC";

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

        public async Task<IList<ModelMenuC3Tab4_Data>> GetAllReportHistoryDataC3Tab4Async(ModelMenuC3Tab4_InterfaceData_History search)
        {

            string sql = "SELECT meeting_id, " +
                        "('วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) + " +
                        "' ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + " +
                        "' ปี ' + CONVERT(VARCHAR, year_of_meeting)) as rptMeetingTitle, " +
                        "('4.' + CONVERT(VARCHAR,agenda_4_term) + '.' + CONVERT(VARCHAR, ROW_NUMBER() OVER(PARTITION BY meeting_id, agenda_4_term ORDER BY A.id ASC))) AS rptAgenda41, " +
                        "C.name_thai as agenda_4_name,agenda_4_project_number, " +
                        "agenda_4_project_name_1,agenda_4_project_name_2, " +
                        "agenda_4_conclusion_name,agenda_4_suggestion " +
                        "FROM Doc_MenuC3_Tab4 A " +
                        "INNER JOIN Doc_MenuC3 B " +
                        "ON A.meeting_id = B.doc_id " +
                        "LEFT OUTER JOIN MST_Consider C " +
                        "ON A.agenda_4_term = C.id " +
                        "WHERE 1=1 AND group_data = '4.1' ";

            if (search != null && !string.IsNullOrEmpty(search.meetingid)) sql += " AND meeting_id='" + search.meetingid + "' ";

            if (search != null && !string.IsNullOrEmpty(search.projectnumber)) sql += " AND agenda_4_project_number='" + search.projectnumber + "' ";

            if (search != null && !string.IsNullOrEmpty(search.considertypeid)) sql += " AND agenda_4_term='" + search.considertypeid + "' ";

            if (search != null && !string.IsNullOrEmpty(search.approvaltypeid)) sql += " AND agenda_4_conclusion='" + search.approvaltypeid + "' ";

            sql += " GROUP BY A.id,meeting_id,agenda_4_term,C.name_thai, " +
                        "agenda_4_term, agenda_4_project_number, " +
                        "agenda_4_project_name_1,agenda_4_project_name_2, " +
                        "agenda_4_conclusion,agenda_4_conclusion_name,agenda_4_suggestion, " +
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
                        IList<ModelMenuC3Tab4_Data> e = new List<ModelMenuC3Tab4_Data>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuC3Tab4_Data item = new ModelMenuC3Tab4_Data();
                            item.rptMeetingId = reader["meeting_id"].ToString();
                            item.rptMeetingTitle = reader["rptMeetingTitle"].ToString();
                            item.rptAgenda41 = reader["rptAgenda41"].ToString();
                            item.rptAgendaName = reader["agenda_4_name"].ToString();
                            item.rptProjectNumber = reader["agenda_4_project_number"].ToString();
                            item.rptProjectName1 = reader["agenda_4_project_name_1"].ToString();
                            item.rptProjectName2 = reader["agenda_4_project_name_2"].ToString();
                            item.rptConclusionName = reader["agenda_4_conclusion_name"].ToString();
                            item.rptSuggestionName = reader["agenda_4_suggestion"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        public async Task<IList<ModelSelectOption>> GetAllMeetingIdAsync()
        {

            string sql = "SELECT TOP(5) doc_id as meeting_id, " +
                        "('วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) + " +
                        "' ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + " +
                        "' ปี ' + CONVERT(VARCHAR, year_of_meeting)) as meeting_name " +
                        "FROM Doc_MenuC3 " +
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

    }

}
