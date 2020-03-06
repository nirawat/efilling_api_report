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

namespace THD.Core.Api.Repository.DataHandler
{
    public class DropdownListRepository : IDropdownListRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly DataContextProvider _DataContextProvider;
        public DropdownListRepository(IConfiguration configuration, DataContextProvider DataContextProvider)
        {
            _configuration = configuration;
            _DataContextProvider = DataContextProvider;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
        }

        public async Task<IList<ModelSelectOption>> GetAllRegisterUserByCharacterAsync(string character)
        {

            string sql = "SELECT register_id, full_name FROM RegisterUser WHERE IsActive='1' ";

            string condition = (!string.IsNullOrEmpty(character) ? "AND Character='" + character + "'" : "");

            sql += condition + " ORDER BY full_name ASC";

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

        //public async Task<IList<ModelSelectOption>> GetAllProjectNumberWithNameAsync(string projectnumber)
        //{

        //    string sql = "SELECT A.project_key_number, (A.project_key_number + ' : ' + B.project_name_thai) AS project_name_thai " +
        //                "FROM Doc_MenuB1 A " +
        //                "INNER JOIN Doc_MenuA1 B ON A.project_id = B.doc_id " +
        //                "INNER JOIN Doc_MenuC3 C ON A.project_key_number = C.agenda_3_project_number " +
        //                "WHERE 1 = 1 AND B.IsClosed = 0 ";

        //    string condition = (!string.IsNullOrEmpty(projectnumber) ? "AND project_key_number='" + projectnumber + "'" : "");

        //    sql += condition + " GROUP BY A.project_key_number, (A.project_key_number + ' : ' + B.project_name_thai)";

        //    using (SqlConnection conn = new SqlConnection(ConnectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand command = new SqlCommand(sql, conn))
        //        {
        //            SqlDataReader reader = await command.ExecuteReaderAsync();

        //            if (reader.HasRows)
        //            {
        //                IList<ModelSelectOption> e = new List<ModelSelectOption>();
        //                while (await reader.ReadAsync())
        //                {
        //                    ModelSelectOption item = new ModelSelectOption();
        //                    item.value = reader["project_key_number"].ToString();
        //                    item.label = reader["project_name_thai"].ToString();
        //                    e.Add(item);
        //                }
        //                return e;
        //            }
        //        }
        //        conn.Close();
        //    }
        //    return null;

        //}

    }
}
