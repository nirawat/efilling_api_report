using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA5Repository
    {
        Task<ModelMenuA5_InterfaceData> MenuA5InterfaceDataAsync(string RegisterId);
        Task<ModelMenuA5ProjectNumberData> GetProjectNumberWithDataA5Async(string project_number);
        Task<ModelResponseA5Message> AddDocMenuA5Async(ModelMenuA5 model);


        Task<ModelMenuA5_InterfaceData> MenuA5EditInterfaceDataAsync(string UserId, string ProjectNumber);

        Task<ModelMenuA5_FileDownload> GetA5DownloadFileByIdAsync(int DocId, int Id);
        Task<ModelResponseA5Message> UpdateDocMenuA5EditAsync(ModelMenuA5 model);
    }
}
