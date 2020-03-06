using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuF2Repository
    {
        #region Menu F2
        Task<ModelMenuF2_InterfaceData> MenuF2InterfaceDataAsync(string RegisterId, string UserGroup);
        Task<IList<ModelMenuF2Report>> GetAllReportDataF2Async(ModelMenuF2_InterfaceData search_data);
        Task<ModelMenuF2Edit> GetUserEditPermissionF2Async(string UserGroup, string MenuCode);
        Task<ModelResponseMessageUpdateUserRegister> UpdatePermissionGroupAsync(ModelMenuF2Edit model);

        #endregion

    }
}
