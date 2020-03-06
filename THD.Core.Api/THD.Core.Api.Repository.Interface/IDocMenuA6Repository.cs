using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA6Repository
    {
        Task<ModelMenuA6_InterfaceData> MenuA6InterfaceDataAsync(string RegisterId);
        Task<ModelMenuA6ProjectNumberData> GetProjectNumberWithDataA6Async(string project_number);
        Task<ModelResponseA6Message> AddDocMenuA6Async(ModelMenuA6 model);


        Task<ModelMenuA6_InterfaceData> MenuA6EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelMenuA6_FileDownload> GetA6DownloadFileByIdAsync(int DocId, int Id);
        Task<ModelResponseA6Message> UpdateDocMenuA6EditAsync(ModelMenuA6 model);
    }
}
