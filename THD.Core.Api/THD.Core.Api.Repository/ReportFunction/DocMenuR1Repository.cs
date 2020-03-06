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

namespace THD.Core.Api.Repository.DataHandler
{
    public class DocMenuR1Repository : IDocMenuR1Repository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDropdownListRepository _IDropdownListRepository;
        private readonly IRegisterUserRepository _IRegisterUserRepository;
        public DocMenuR1Repository(IConfiguration configuration, IDropdownListRepository DropdownListRepository, IRegisterUserRepository IRegisterUserRepository)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDropdownListRepository = DropdownListRepository;
            _IRegisterUserRepository = IRegisterUserRepository;
        }




        // ระเบียบวาระที่ 4 ------------------------------------------------------------------------------
        public async Task<ModelMenuR1_InterfaceData> MenuR1InterfaceDataAsync(string RegisterId)
        {
            ModelMenuR1_InterfaceData resp = new ModelMenuR1_InterfaceData();

            resp.ListMeetingId = new List<ModelSelectOption>();

            resp.ListMeetingId = await GetAllMeetingIdAsync();

            ModelSelectOption all_meeting = new ModelSelectOption() { value = "", label = "เลือก..." };
            resp.ListMeetingId.Add(all_meeting);

            resp.ListMeetingType = await GetAllMeetingTypeAsync();

            ModelSelectOption all_meeting_type = new ModelSelectOption() { value = "", label = "เลือก..." };
            resp.ListMeetingType.Add(all_meeting_type);

            resp.ListReportData = await GetAllReportHistoryDataR1Async(null);

            resp.UserPermission = await _IRegisterUserRepository.GetPermissionPageAsync(RegisterId, "M019");

            return resp;
        }

        public async Task<IList<ModelSelectOption>> GetAllMeetingTypeAsync()
        {
            string sql = "SELECT id, name_thai FROM MST_MeetingRecordType ORDER BY id ASC";

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

        public async Task<IList<ModelMenuR1Data>> GetAllReportHistoryDataR1Async(ModelMenuR1_InterfaceData search)
        {

            string sql = "SELECT B.doc_id, A.meeting_round, A.meeting_year, A.isClose, " +
                        "B.meeting_date, B.meeting_start, B.meeting_close, B.meeting_location, C.name_thai AS meeting_type_name " +
                        "FROM Doc_MeetingRound_Project A " +
                        "LEFT OUTER JOIN [dbo].[Doc_MenuC3] B " +
                        "ON A.meeting_year = B.year_of_meeting AND A.meeting_round = B.meeting_round " +
                        "LEFT OUTER JOIN [dbo].[MST_MeetingRecordType] C ON C.id = B.meeting_record_id " +
                        "WHERE 1=1 ";

            if (search != null && !string.IsNullOrEmpty(search.meetingid)) sql += " AND B.doc_id='" + search.meetingid + "' ";

            sql += " ORDER BY A.meeting_round DESC";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMenuR1Data> e = new List<ModelMenuR1Data>();
                        while (await reader.ReadAsync())
                        {
                            ModelMenuR1Data item = new ModelMenuR1Data();
                            item.docid = reader["doc_id"].ToString();
                            item.meetingdate = (string.IsNullOrEmpty(reader["meeting_date"].ToString()) ? "" : Convert.ToDateTime(reader["meeting_date"]).ToString("dd/MM/yyyy"));
                            item.meetinglocation = reader["meeting_location"].ToString();
                            item.meetingstart = reader["meeting_start"].ToString();
                            item.meetingclose = reader["meeting_close"].ToString();
                            item.meetinground = reader["meeting_round"].ToString();
                            item.yearofmeeting = reader["meeting_year"].ToString();
                            item.isclosed = Convert.ToBoolean(reader["isClose"]);
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
                        "'ครั้งที่ ' + CONVERT(VARCHAR, meeting_round) + " +
                        "' ปี ' + CONVERT(VARCHAR, year_of_meeting) + " +
                        " 'วันที่ ' + CONVERT(VARCHAR, meeting_date, 103) as meeting_name " +
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
