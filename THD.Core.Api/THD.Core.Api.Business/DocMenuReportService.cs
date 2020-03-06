using System;
using System.Collections.Generic;
using THD.Core.Api.Business.Interface;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using THD.Core.Api.Models;
using THD.Core.Api.Repository.Interface;
using System.Text;
using THD.Core.Api.Helpers;
using System.IO;
using static THD.Core.Api.Helpers.ServerDirectorys;
using THD.Core.Api.Models.Config;
using System.Globalization;
using THD.Core.Api.Repository.DataHandler;
using THD.Core.Api.Models.ReportModels;

namespace THD.Core.Api.Business
{
    public class DocMenuReportService : IDocMenuReportService
    {
        private readonly IEnvironmentConfig _IEnvironmentConfig;
        private readonly IDocMenuReportRepository _IDocMenReportRepository;


        public DocMenuReportService(
            IEnvironmentConfig EnvironmentConfig,
            IDocMenuReportRepository DocMenuReportRepository)
        {
            _IEnvironmentConfig = EnvironmentConfig;
            _IDocMenReportRepository = DocMenuReportRepository;
        }

        public async Task<model_rpt_1_file> GetReportR1_2Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR1_2Async(doc_id);
        }
        public async Task<model_rpt_3_file> GetReportR3Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR3Async(doc_id);
        }
        public async Task<model_rpt_4_file> GetReportR4Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR4Async(doc_id);
        }
        public async Task<model_rpt_5_file> GetReportR5Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR5Async(doc_id);
        }
        public async Task<model_rpt_6_file> GetReportR6Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR6Async(doc_id);
        }
        public async Task<model_rpt_7_file> GetReportR7Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR7Async(doc_id);
        }
        public async Task<model_rpt_8_file> GetReportR8Async(int doc_id, string type)
        {
            return await _IDocMenReportRepository.GetReportR8Async(doc_id, type);
        }
        public async Task<model_rpt_9_file> GetReportR9Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR9Async(doc_id);
        }
        public async Task<model_rpt_10_file> GetReportR10Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR10Async(doc_id);
        }
        public async Task<model_rpt_11_file> GetReportR11Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR11Async(doc_id);
        }
        public async Task<model_rpt_12_file> GetReportR12Async(int doc_id, int type)
        {
            return await _IDocMenReportRepository.GetReportR12Async(doc_id, type);
        }
        public async Task<model_rpt_13_file> GetReportR13Async(int doc_id, int type)
        {
            return await _IDocMenReportRepository.GetReportR13Async(doc_id, type);
        }
        public async Task<model_rpt_14_file> GetReportR14Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR14Async(doc_id);
        }
        public async Task<model_rpt_15_file> GetReportR15Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR15Async(doc_id);
        }
        public async Task<model_rpt_17_file> GetReportR17_18Async(int doc_id)
        {
            return await _IDocMenReportRepository.GetReportR17_18Async(doc_id);
        }
        public async Task<model_rpt_meeting_file> GetAllReportMeetingAsync(int doc_id)
        {
            return await _IDocMenReportRepository.GetAllReportMeetingAsync(doc_id);
        }

    }
}
