using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using THD.Core.Api.Models;
using THD.Core.Api.Models.ReportModels;

namespace THD.Core.Api.Business.Interface
{
    public interface IDocMenuReportService
    {
        Task<model_rpt_1_file> GetReportR1_2Async(int doc_id);
        Task<model_rpt_3_file> GetReportR3Async(int doc_id);
        Task<model_rpt_4_file> GetReportR4Async(int doc_id);
        Task<model_rpt_5_file> GetReportR5Async(int doc_id);
        Task<model_rpt_6_file> GetReportR6Async(int doc_id);
        Task<model_rpt_7_file> GetReportR7Async(int doc_id);
        Task<model_rpt_8_file> GetReportR8Async(int doc_id, string type);
        Task<model_rpt_9_file> GetReportR9Async(int doc_id);
        Task<model_rpt_10_file> GetReportR10Async(int doc_id);
        Task<model_rpt_11_file> GetReportR11Async(int doc_id);
        Task<model_rpt_12_file> GetReportR12Async(int doc_id, int type);
        Task<model_rpt_13_file> GetReportR13Async(int doc_id, int type);
        Task<model_rpt_14_file> GetReportR14Async(int doc_id);
        Task<model_rpt_15_file> GetReportR15Async(int doc_id);
        Task<model_rpt_17_file> GetReportR17_18Async(int doc_id);
        Task<model_rpt_meeting_file> GetAllReportMeetingAsync(int doc_id);
    }
}
