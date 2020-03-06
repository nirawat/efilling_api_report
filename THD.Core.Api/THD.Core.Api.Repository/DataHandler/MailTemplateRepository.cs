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

namespace THD.Core.Api.Repository.DataHandler
{
    public class MailTemplateRepository : IMailTemplateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string ConnectionString;
        private readonly IDocMenuB1Repository _IDocMenuB1Repository;
        private readonly IEmailHelper _EmailHelper;

        public MailTemplateRepository(
            IConfiguration configuration, IDocMenuB1Repository DocMenuB1Repository, IEmailHelper EmailHelper)
        {
            _configuration = configuration;
            ConnectionString = Encoding.UTF8.GetString(Convert.FromBase64String(_configuration.GetConnectionString("SqlConnection")));
            _IDocMenuB1Repository = DocMenuB1Repository;
            _EmailHelper = EmailHelper;
        }

        #region "Mail Template 1"

        public async Task<bool> MailTemplate1Async(int DocId, string rptBase64)
        {
            string mail_subject = "คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอแจ้งผลการตรวจสอบข้อเสนอโครงการวิจัยเรื่อง ";

            ModelMail_Template1 data = await GetData_MailTemplate1_Async(DocId);

            if (data != null)
            {
                string mail_body = "<h3>เรียน " + data.fullname + "</h3>" + Environment.NewLine +
                       "<h3>" + mail_subject + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + data.project_number + "</p>" + Environment.NewLine +
                       "<p>" + data.project_name_thai + "</p>" + Environment.NewLine +
                       "<p>พร้อมแจ้งผลการพิจารณาตามบันทึกแนบนี้</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : แจ้งผลการตรวจสอบข้อเสนอโครงการ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template1> GetData_MailTemplate1_Async(int DocId)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_request_id='" + DocId + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template1 e = new ModelMail_Template1();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 2"

        public async Task<bool> MailTemplate2Async(string ProjectNumber, string rptBase64)
        {
            string mail_subject = "คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอส่งสำเนาใบรับรองโครงการวิจัยเรื่อง ";

            ModelMail_Template2 data = await GetData_MailTemplate2_Async(ProjectNumber);

            if (data != null)
            {
                string mail_body = "<h3>เรียน " + data.fullname + "</h3>" + Environment.NewLine +
                       "<h3>" + mail_subject + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + data.project_number + "</p>" + Environment.NewLine +
                       "<p>" + data.project_name_thai + "</p>" + Environment.NewLine +
                       "<p>แนบมาพร้อม e-mail นี้  ท่านสามารถรับต้นฉบับจริงได้ด้วยตนเอง ณ งานจัดการมาตรฐานและเครือข่าย กองการวิจัยและนวัตกรรม (อาคารเอกาทศรถ) มหาวิทยาลัยนเรศวร</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : ใบรับรองด้านความปลอดภัยทางชีวภาพ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template2> GetData_MailTemplate2_Async(string ProjectNumber)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_number='" + ProjectNumber + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template2 e = new ModelMail_Template2();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 3"

        public async Task<bool> MailTemplate3Async(ModelMenuC1 model, string rptBase64)
        {
            string mail_subject = "คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอความอนุเคราะห์จากท่านได้อ่านและพิจารณาการรับรองโครงการ ";

            IList<ModelMail_Template3> email_to = await GetCommittee_MailTemplate3_Async(model);

            if (email_to != null && email_to.Count > 0)
            {
                foreach (var item in email_to)
                {
                    string mail_body = "<h3>เรียน " + item.fullname + "</h3>" + Environment.NewLine +
                           "<h3>" + mail_subject + "</h3>" + Environment.NewLine +
                           "</br>" + Environment.NewLine +
                           "<p>หมายเลขโครงการ " + model.projectnumber + "</p>" + Environment.NewLine +
                           "<p>" + model.projectnamethai + "</p>" + Environment.NewLine +
                           "<p>เพื่อขอความอนุเคราะห์จากท่านได้อ่านและพิจารณาการรับรองโครงการดังกล่าว ตามบันทึกแนบนี้  ซึ่งท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ” เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องได้ตั้งแต่บัดนี้เป็นต้นไป</p>" + Environment.NewLine +
                           "</br>" + Environment.NewLine +
                           "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                           "<h3>มหาวิทยาลัยนเรศวร</h3>";

                    await _EmailHelper.SentGmail(item.email, "NUIBC : ขอความอนุเคราะห์อ่านโครงการ", mail_body, rptBase64);
                }

                return true;
            }
            else return false;

        }

        public async Task<IList<ModelMail_Template3>> GetCommittee_MailTemplate3_Async(ModelMenuC1 model)
        {
            string multi_user = "";

            if (model.boardcodearray != null && model.boardcodearray.Count > 0)
            {
                foreach (var item in model.boardcodearray)
                {
                    multi_user += Encoding.UTF8.GetString(Convert.FromBase64String(item.value)) + "','";
                }
            }

            string sql = "SELECT email, (first_name + full_name) as full_name " +
                         "FROM [dbo].[RegisterUser] " +
                         "WHERE register_id IN ('" + multi_user + "')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMail_Template3> e = new List<ModelMail_Template3>();
                        while (await reader.ReadAsync())
                        {
                            ModelMail_Template3 item = new ModelMail_Template3();
                            item.fullname = reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 4"

        public async Task<bool> MailTemplate4Async(string ProjectNumber, string rptBase64)
        {
            string mail_subject = "คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ ";

            ModelMail_Template4 data = await GetData_MailTemplate4_Async(ProjectNumber);

            if (data != null)
            {
                string mail_body = "<h3>เรียน " + data.fullname + "</h3>" + Environment.NewLine +
                       "<h3>" + mail_subject + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + ProjectNumber + "</p>" + Environment.NewLine +
                       "<p>กองการวิจัยและนวัตกรรม ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ โดยมีมติในโครงการวิจัยของท่านเรื่อง <h2>" + data.project_name_thai + "</h2> พร้อมแจ้งผลการพิจารณาตามบันทึกแนบนี้</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : แจ้งผลการพิจารณา โดยให้แก้ไขโครงการ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template4> GetData_MailTemplate4_Async(string ProjectNumber)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_number='" + ProjectNumber + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template4 e = new ModelMail_Template4();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 5"

        public async Task<bool> MailTemplate5Async(string ProjectNumber, string rptBase64)
        {
            string mail_subject = "คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ ";

            ModelMail_Template5 data = await GetData_MailTemplate5_Async(ProjectNumber);

            if (data != null)
            {
                string mail_body = "<h3>เรียน " + data.fullname + "</h3>" + Environment.NewLine +
                       "<h3>" + mail_subject + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + ProjectNumber + "</p>" + Environment.NewLine +
                       "<p>กองการวิจัยและนวัตกรรม ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ โดยมีมติในโครงการวิจัยของท่านเรื่อง <h3>" + data.project_name_thai + "</h3> พร้อมแจ้งผลการพิจารณาตามบันทึกแนบนี้</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : แจ้งผลการพิจารณา รับรองโครงการ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template5> GetData_MailTemplate5_Async(string ProjectNumber)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_number='" + ProjectNumber + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template5 e = new ModelMail_Template5();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 6"

        public async Task<bool> MailTemplate6Async(string round, string year, string rptBase64)
        {
            IList<ModelMail_Template6> email_to = await GetUserMeeting_MailTemplate6_Async(round, year);

            if (email_to != null && email_to.Count > 0)
            {
                foreach (var item in email_to)
                {
                    string mail_body = "<h3>เรียน " + item.fullname + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอส่งระเบียบวาระการประชุมครั้งที่ <h3>" + round + " / " + year + "</h3></p>" + Environment.NewLine +
                       "<p>ตามระเบียบวาระการประชุมแนบ ท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ” เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องกับการประชุมได้ตั้งแต่บัดนี้เป็นต้นไป</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                    await _EmailHelper.SentGmail(item.email, "NUIBC : ขอนำส่งระเบียบวาระการประชุม", mail_body, rptBase64);
                }

                return true;
            }
            else return false;

        }

        public async Task<IList<ModelMail_Template6>> GetUserMeeting_MailTemplate6_Async(string round, string year)
        {
            string multi_user = "";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql1 = "SELECT TOP(1) meeting_user_code_array " +
                              "FROM Doc_MenuC3 " +
                              "WHERE meeting_round='" + round + "' AND year_of_meeting='" + year + "' ";

                using (SqlCommand command = new SqlCommand(sql1, conn))
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

                // --------------------------------------------------------------------------


                string sql2 = "SELECT email, (first_name + full_name) as full_name " +
                             "FROM [dbo].[RegisterUser] " +
                             "WHERE register_id IN ('" + multi_user.Replace(",", "','") + "')";

                using (SqlCommand command = new SqlCommand(sql2, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMail_Template6> e = new List<ModelMail_Template6>();
                        while (await reader.ReadAsync())
                        {
                            ModelMail_Template6 item = new ModelMail_Template6();
                            item.fullname = reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                    reader.Close();
                }

                conn.Close();
            }
            return null;

        }


        #endregion

        #region "Mail Template 7"

        public async Task<bool> MailTemplate7Async(string round, string year, string rptBase64)
        {
            IList<ModelMail_Template7> email_to = await GetUserMeeting_MailTemplate7_Async(round, year);

            if (email_to != null && email_to.Count > 0)
            {
                foreach (var item in email_to)
                {
                    string mail_body = "<h3>เรียน " + item.fullname + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอส่งรายงานการประชุมครั้งที่ <h3>" + round + " / " + year + "</h3></p>" + Environment.NewLine +
                       "<p>ตามระเบียบวาระการประชุมแนบ ท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ” เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องกับการประชุมได้ตั้งแต่บัดนี้เป็นต้นไป </p> " + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                    await _EmailHelper.SentGmail(item.email, "NUIBC : ขอนำส่งรายงานการประชุม", mail_body, rptBase64);
                }

                return true;
            }
            else return false;

        }

        public async Task<IList<ModelMail_Template7>> GetUserMeeting_MailTemplate7_Async(string round, string year)
        {
            string multi_user = "";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql1 = "SELECT TOP(1) meeting_user_code_array " +
                              "FROM Doc_MenuC3 " +
                              "WHERE meeting_round='" + round + "' AND year_of_meeting='" + year + "' ";

                using (SqlCommand command = new SqlCommand(sql1, conn))
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

                // --------------------------------------------------------------------------


                string sql2 = "SELECT email, (first_name + full_name) as full_name " +
                             "FROM [dbo].[RegisterUser] " +
                             "WHERE register_id IN ('" + multi_user.Replace(",", "','") + "')";

                using (SqlCommand command = new SqlCommand(sql2, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        IList<ModelMail_Template7> e = new List<ModelMail_Template7>();
                        while (await reader.ReadAsync())
                        {
                            ModelMail_Template7 item = new ModelMail_Template7();
                            item.fullname = reader["full_name"].ToString();
                            item.email = reader["email"].ToString();
                            e.Add(item);
                        }
                        return e;
                    }
                    reader.Close();
                }

                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 8"

        public async Task<bool> MailTemplate8Async(string ProjectNumber, string rptBase64)
        {
            ModelMail_Template8 data = await GetData_MailTemplate8_Async(ProjectNumber);

            if (data != null)
            {
                string mail_body = "<h3>เรียน กรรมการผู้อ่านและพิจารณาโครงการ</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + ProjectNumber + "</p>" + Environment.NewLine +
                       "<p>ข้าพเจ้าขอส่งรายงานการแก้ไขโครงการวิจัยเรื่อง <h2>" + data.project_name_thai + "</h2> เพื่อขอความอนุเคราะห์จากท่านได้อ่านและพิจารณาการรับรองโครงการดังกล่าว ตามบันทึกแนบนี้  ซึ่งท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ”เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องได้ตั้งแต่บัดนี้เป็นต้นไป</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : ขอส่งรายงานการแก้ไขโครงการ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template8> GetData_MailTemplate8_Async(string ProjectNumber)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_number='" + ProjectNumber + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template8 e = new ModelMail_Template8();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion

        #region "Mail Template 9"

        public async Task<bool> MailTemplate9Async(string ProjectNumber, string rptBase64)
        {

            ModelMail_Template9 data = await GetData_MailTemplate9_Async(ProjectNumber);

            if (data != null)
            {
                string mail_body = "<h3>เรียน กรรมการผู้อ่านและพิจารณาโครงการ</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>หมายเลขโครงการ " + ProjectNumber + "</p>" + Environment.NewLine +
                       "<p>ข้าพเจ้าขอส่งรายงานการแก้ไขโครงการตามมติคณะกรรมการ โครงการวิจัยเรื่อง <h2>" + data.project_name_thai + "</h2> เพื่อขอความอนุเคราะห์จากท่านได้อ่านและพิจารณาการรับรองโครงการดังกล่าว ตามบันทึกแนบนี้  ซึ่งท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ”เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องได้ตั้งแต่บัดนี้เป็นต้นไป</p>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                await _EmailHelper.SentGmail(data.email, "NUIBC : ขอส่งรายงานการแก้ไขโครงการตามมติคณะกรรมการ", mail_body, rptBase64);

                return true;
            }
            else return false;

        }

        public async Task<ModelMail_Template9> GetData_MailTemplate9_Async(string ProjectNumber)
        {
            string sql = "SELECT (B.first_name + B.full_name) as project_by_name, " +
                        "B.email, A.project_number, A.project_name_thai " +
                        "FROM [dbo].[Doc_Process] A " +
                        "LEFT OUTER JOIN [dbo].[RegisterUser] B " +
                        "ON A.project_by = B.register_id " +
                        "WHERE A.project_number='" + ProjectNumber + "'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        ModelMail_Template9 e = new ModelMail_Template9();
                        while (await reader.ReadAsync())
                        {
                            e.project_number = reader["project_number"].ToString();
                            e.project_name_thai = reader["project_name_thai"].ToString();
                            e.fullname = reader["project_by_name"].ToString();
                            e.email = reader["email"].ToString();
                        }
                        return e;
                    }
                }
                conn.Close();
            }
            return null;

        }

        #endregion



        #region "Send Mail Meeting Complete"

        public async Task<bool> MailMeetingCompleteAsync(string round, string year, ModelResponseMessageReportMeeting e)
        {
            if (e != null && e.list_attendees.Count > 0)
            {
                foreach (var item in e.list_attendees)
                {
                    string mail_body = "<h3>เรียน " + item.ReceiveName + "</h3>" + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<p>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอส่งรายงานการประชุมครั้งที่ <h3>" + round + " / " + year + "</h3></p>" + Environment.NewLine +
                       "<p>ตามระเบียบวาระการประชุมแนบ ท่านสามารถล็อกอินเข้า “ระบบรับรองโครงการ” เพื่อดาวน์โหลดเอกสารที่เกี่ยวข้องกับการประชุมได้ตั้งแต่บัดนี้เป็นต้นไป </p> " + Environment.NewLine +
                       "</br>" + Environment.NewLine +
                       "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                       "<h3>มหาวิทยาลัยนเรศวร</h3>";

                    await _EmailHelper.SentGmail(item.ReceiveEmail, "NUIBC : ขอนำส่งรายงานการประชุม", mail_body, e.rpt_14_filebase64);
                }
            }

            if (e != null && e.list_reasearch.Count > 0)
            {
                foreach (var item in e.list_reasearch)
                {
                    string mail_body = "<h3>เรียน " + item.ReceiveName + "</h3>" + Environment.NewLine +
                               "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                               "</br>" + Environment.NewLine +
                               "<p>หมายเลขโครงการ " + item.ProjectNumber + "</p>" + Environment.NewLine +
                               "<p>กองการวิจัยและนวัตกรรม ขอแจ้งผลการพิจารณาของคณะกรรมการเพื่อความปลอดภัยทางชีวภาพ โดยมีมติในโครงการวิจัยของท่านเรื่อง <h3>" + item.ProjectNameThai + "</h3> พร้อมแจ้งผลการพิจารณาตามบันทึกแนบนี้</p>" + Environment.NewLine +
                               "</br>" + Environment.NewLine +
                               "<h3>คณะกรรมการควบคุมความปลอดภัยทางชีวภาพ</h3>" + Environment.NewLine +
                               "<h3>มหาวิทยาลัยนเรศวร</h3>";

                    await _EmailHelper.SentGmail(item.ReceiveEmail, "NUIBC : แจ้งผลการพิจารณา รับรองโครงการ", mail_body, item.rpt_filebase64);
                }
            }

            return true;

        }


        #endregion


    }
}
