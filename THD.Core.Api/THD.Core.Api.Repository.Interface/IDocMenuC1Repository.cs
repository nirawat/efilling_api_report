using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuC1Repository
    {

        #region Menu C1
        Task<ModelMenuC1_InterfaceData> MenuC1InterfaceDataAsync(string userid, string username);

        Task<ModelMenuC1Data> GetProjectNumberWithDataC1Async(string project_number);

        Task<ModelRegisterData> GetRegisterUserDataAsync(string register_id);

        Task<ModelResponseC1Message> AddDocMenuC1Async(ModelMenuC1 model);

        #endregion

        #region Menu C1 Edit

        Task<ModelMenuC1_InterfaceData> MenuC1InterfaceDataEditAsync(string project_number, string RegisterId);

        Task<ModelResponseC1Message> UpdateDocMenuC1EditAsync(ModelMenuC1 model);

        #endregion

        #region Menu C2

        Task<ModelMenuC12_InterfaceData> MenuC12InterfaceDataAsync(string RegisterId);

        Task<ModelMenuC12Data> GetProjectNumberWithDataC12Async(string project_number);

        Task<ModelRegisterDataC12> GetRegisterUserDataC12Async(string register_id);

        Task<ModelResponseC12Message> AddDocMenuC12Async(ModelMenuC12 model);

        #endregion

    }
}
