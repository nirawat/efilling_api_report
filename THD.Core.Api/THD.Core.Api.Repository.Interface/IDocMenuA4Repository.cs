using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA4Repository
    {
        Task<ModelMenuA4_InterfaceData> MenuA4InterfaceDataAsync(string RegisterId);
        Task<ModelMenuA4ProjectNumberData> GetProjectNumberWithDataA4Async(string project_number);
        Task<ModelResponseA4Message> AddDocMenuA4Async(ModelMenuA4 model);


        Task<ModelMenuA4_InterfaceData> MenuA4EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelMenuA4_FileDownload> GetA4DownloadFileByIdAsync(int DocId, int Id);
        Task<ModelResponseA4Message> UpdateDocMenuA4EditAsync(ModelMenuA4 model);


    }
}
