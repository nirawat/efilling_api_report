using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA7Repository
    {
        Task<ModelMenuA7_InterfaceData> MenuA7InterfaceDataAsync(string RegisterId);
        Task<ModelMenuA7ProjectNumberData> GetProjectNumberWithDataA7Async(string project_number);
        Task<ModelResponseA7Message> AddDocMenuA7Async(ModelMenuA7 model);


        Task<ModelMenuA7_InterfaceData> MenuA7EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelMenuA7_FileDownload> GetA7DownloadFileByIdAsync(int DocId, int Id);
        Task<ModelResponseA7Message> UpdateDocMenuA7EditAsync(ModelMenuA7 model);
    }
}
