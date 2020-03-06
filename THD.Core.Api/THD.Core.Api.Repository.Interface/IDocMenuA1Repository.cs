using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA1Repository
    {

        #region Menu A1

        Task<ModelMenuA1_InterfaceData> MenuA1InterfaceDataAsync(string userid, string username);
        Task<ModelResponseA1Message> AddDocMenuA1Async(ModelMenuA1 model);

        #endregion

        #region Menu A1 Edit

        Task<ModelMenuA1_InterfaceData> MenuA1InterfaceDataEditAsync(int doc_id, string userid, string username);
        Task<ModelMenuA1_FileDownload> GetA1DownloadFileByIdAsync(int DocId, int Id);
        Task<ModelResponseA1Message> UpdateDocMenuA1EditAsync(ModelMenuA1 model);


        #endregion
    }
}
