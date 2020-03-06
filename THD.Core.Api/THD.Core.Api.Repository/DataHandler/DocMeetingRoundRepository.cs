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
    public class DocMeetingRoundRepository : IDocMeetingRoundRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly DataContextProvider _DataContextProvider;
        public DocMeetingRoundRepository(IConfiguration configuration, DataContextProvider DataContextProvider)
        {
            _configuration = configuration;
            _DataContextProvider = DataContextProvider;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
        }

        #region Get Round Of Meeting

        public async Task<ModelCountOfYear> GetMeetingRoundOfProjectAsync(int year)
        {
            ModelCountOfYear round = new ModelCountOfYear() { count = 1 };

            string sql = "SELECT MAX(meeting_round) AS round_number " +
                         "FROM [EFILLING].[dbo].[Doc_MeetingRound_Project] " +
                         "WHERE meeting_year = '" + year + "'";

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
                            round.count = Convert.ToInt32(reader["round_number"]);
                        }
                    }
                }
                conn.Close();
            }
            return round;
        }

        //public async Task<ModelCountOfYear> GetDefaultRoundC1Async(int yearof)
        //{
        //    ModelCountOfYear rest = new ModelCountOfYear() { count = 1 };

        //    string sql = "SELECT round_of_meeting " +
        //                 "FROM Doc_MenuC1 " +
        //                 "WHERE round_of_closed=1 " +
        //                 "AND year_of_meeting='" + yearof + "'" +
        //                 "GROUP BY round_of_meeting ";

        //    using (SqlConnection conn = new SqlConnection(ConnectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand(sql, conn))
        //        {
        //            var round_no = await cmd.ExecuteScalarAsync();
        //            rest.count = (round_no != null) ? ((int)round_no + 1) : 1;
        //        }
        //        conn.Close();
        //    }
        //    return rest;

        //}

        #endregion

    }
}
