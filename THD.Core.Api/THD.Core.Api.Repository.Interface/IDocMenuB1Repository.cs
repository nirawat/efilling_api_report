using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuB1Repository
    {
        Task<ModelMenuB1_InterfaceData> MenuB1InterfaceDataAsync(string userid, string username);
        Task<IList<ModelSelectOption>> GetAllProjectNameThaiAsync(string project_head);
        Task<IList<ModelSelectOption>> GetAllDownloadFileByProjectIdAsync(string project_id);
        Task<ModelMenuB1_GetDataByProjectNameThai> GetDataByProjectNameThaiAsync(int project_id);
        Task<ModelMenuB1Data> GetProjectNumberWithDataAsync(string project_number);
        Task<ModelResponseMessageAddDocB1> AddDocMenuB1Async(ModelMenuB1 model);


        Task<ModelMenuB1_InterfaceData> MenuB1InterfaceDataEditAsync(string project_number, string userid, string username);
        Task<ModelResponseMessageAddDocB1> UpdateDocMenuB1Async(ModelMenuB1Edit model);


        Task<ModelMenuB1_2_InterfaceData> MenuB1_2InterfaceDataAsync(string userid, string username);
        Task<ModelMenuB1_2_GetDataByProjectNumber> GetB1_2ProjectNumberDataAsync(string project_number);
        Task<ModelResponseMessageAddDocB1_2> AddDocMenuB1_2Async(ModelMenuB1_2 model);
    }
}
