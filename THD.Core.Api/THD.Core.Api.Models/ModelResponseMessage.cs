using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using THD.Core.Api.Models.ReportModels;

namespace THD.Core.Api.Models
{
    public class ModelResponseMessage
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
    }

    public class ModelResponseMessageRegisterUser
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string RegisterId { get; set; }
        public string TokenKey { get; set; }
    }

    public class ModelResponseMessageRegisterActive
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class ModelResponseMessageUpdateUserRegister
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }

    public class ModelResponseMessageLogin
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ModelResponseMessageLoginData Data { get; set; }
    }

    public class ModelResponseMessageLoginData
    {
        public string Guid { get; set; }
        public string Token { get; set; }
        public string RegisterId { get; set; } //Base64
        public string FullName { get; set; }
        public string PositionName { get; set; }
    }

    public class ModelResponseMessageAddDocB1
    {
        public bool Status { get; set; }
        public string DocNumber { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseMessageAddDocB1_2
    {
        public bool Status { get; set; }
        public string DocNumber { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseMessageAddDocB2
    {
        public bool Status { get; set; }
        public string DocNumber { get; set; }
        public string Message { get; set; }
    }

    public class ModelResponseC1Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string EmailArray { get; set; }
        // Report Return --------------
        public string filename_1 { get; set; }
        public string filebase_1_64 { get; set; }
        public string filename_2 { get; set; }
        public string filebase_2_64 { get; set; }
        public string filename_3 { get; set; }
        public string filebase_3_64 { get; set; }
    }

    public class ModelResponseC12Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string EmailArray { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


    public class ModelResponseC2Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string EmailArray { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


    public class ModelResponseC22Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string EmailArray { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


    public class ModelResponseMessageReportMeeting
    {
        public bool Status { get; set; }
        public string DocNumber { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        public IList<ModelResponseDataForSendMail> list_reasearch { get; set; }
        public IList<ModelResponseDataForSendMail> list_attendees { get; set; }

        // Report File
        public string rpt_14_filename { get; set; }
        public string rpt_14_filebase64 { get; set; }
    }

    public class ModelResponseMessageReportAgenda
    {
        public bool Status { get; set; }
        public string DocNumber { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseDataForSendMail
    {
        public string ProjectNumber { get; set; }
        public string ProjectNameThai { get; set; }
        public string ProjectNameEng { get; set; }
        public string ReceiveName { get; set; }
        public string ReceiveEmail { get; set; }

        public string rpt_filename { get; set; }
        public string rpt_filebase64 { get; set; }
    }





    public class ModelResponseA1Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename1and2 { get; set; }
        public string filebase1and264 { get; set; }
        public string filename16 { get; set; }
        public string filebase1664 { get; set; }
    }

    public class ModelResponseA2Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseA3Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseA4Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseA5Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseA6Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseA7Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC3Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC31Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC32Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC33Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC34Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseC35Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }


    public class ModelResponseD1Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

    public class ModelResponseD2Message
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int DocId { get; set; }
        // Report Return --------------
        public string filename { get; set; }
        public string filebase64 { get; set; }
    }

}
