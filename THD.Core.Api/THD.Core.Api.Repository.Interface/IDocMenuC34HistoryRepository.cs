using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuC34HistoryRepository
    {
        //บันทึกการประชุม
        Task<ModelMenuC3Tab4_InterfaceData_History> MenuC3Tab4InterfaceHistoryDataAsync();
        Task<IList<ModelMenuC3Tab4_Data>> GetAllReportHistoryDataC3Tab4Async(ModelMenuC3Tab4_InterfaceData_History search);

    }
}
