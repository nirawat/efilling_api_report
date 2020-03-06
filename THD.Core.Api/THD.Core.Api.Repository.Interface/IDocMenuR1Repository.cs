using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuR1Repository
    {
        //บันทึกการประชุม
        Task<ModelMenuR1_InterfaceData> MenuR1InterfaceDataAsync(string RegisterId);
        Task<IList<ModelMenuR1Data>> GetAllReportHistoryDataR1Async(ModelMenuR1_InterfaceData search);

    }
}
