using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuHomeRepository
    {
        Task<ModelMenuHome1_InterfaceData> MenuHome1InterfaceDataAsync(string RegisterId);

        Task<ModelMenuA1> GetFileDownloadHome1Async(string project_number);

        Task<IList<ResultCommentNote>> GetResultNoteHome1Async(string project_number, string user_id);

        Task<IList<ModelMenuHome1ReportData>> GetAllReportDataHome1Async(ModelMenuHome1_InterfaceData search_data);

        Task<ModelMenuHome2_InterfaceData> MenuHome2InterfaceDataAsync(string RegisterId);

        Task<ModelMenuA2> GetFileDownloadHome2Async(string project_number);

        Task<IList<ModelMenuHome2ReportData>> GetAllReportDataHome2Async(ModelMenuHome2_InterfaceData search_data);
    }
}
