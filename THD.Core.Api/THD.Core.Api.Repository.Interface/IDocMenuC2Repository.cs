using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuC2Repository
    {

        #region C2

        Task<ModelMenuC2_InterfaceData> MenuC2InterfaceDataAsync(string userid, string username);

        Task<ModelMenuC2Data> GetProjectNumberWithDataC2Async(string project_number);

        Task<IList<ModelSelectOption>> GetAllAssignerUserAsync();

        Task<ModelMenuC2Data> GetRegisterUserDataC2Async(string register_id);

        Task<ModelResponseC2Message> AddDocMenuC2Async(ModelMenuC2 model);

        #endregion

        #region C2 Edit

        Task<ModelMenuC2_InterfaceData> MenuC2InterfaceDataEditAsync(int docid, string userid, string username);

        Task<ModelResponseC2Message> UpdateDocMenuC2EditAsync(ModelMenuC2 model);

        #endregion

        #region C2_2

        Task<ModelMenuC22_InterfaceData> MenuC22InterfaceDataAsync(string userid, string username);

        Task<ModelMenuC22Data> GetProjectNumberWithDataC22Async(string project_number);

        Task<IList<ModelSelectOption>> GetAllAssignerUserC22Async();

        Task<ModelMenuC22Data> GetRegisterUserDataC22Async(string register_id);

        Task<ModelResponseC22Message> AddDocMenuC22Async(ModelMenuC22 model);

        #endregion


        Task<IList<ModelSelectOption>> GetAllProjectAsync(string AssignerCode, string DocProcess);

        Task<IList<ModelSelectOption>> GetAllProjectLabAsync(string AssignerCode, string DocProcess);
    }
}
