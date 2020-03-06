using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuC3Repository
    {
        //บันทึกการประชุม
        Task<ModelMenuC3_InterfaceData> MenuC3InterfaceDataAsync(string RegisterId);
        Task<ModelCountOfYearC3> GetDefaultRoundC3Async(int yearof);
        Task<IList<ModelMenuC3_History>> GetAllHistoryDataC3Async();
        Task<ModelResponseC3Message> AddDocMenuC3Async(ModelMenuC3 model);

        //บันทึกการประชุม แก้ไข
        Task<ModelMenuC3_InterfaceData> MenuC3EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC3Message> UpdateDocMenuC3EditAsync(ModelMenuC3 model);



        //ระเบียบวาระที่ 1 ----------------------------------------------------------------------------------------
        Task<ModelMenuC31_InterfaceData> MenuC31InterfaceDataAsync(string RegisterId);
        Task<ModelResponseC31Message> AddDocMenuC31Async(ModelMenuC31 model);

        //ระเบียบวาระที่ 1 แก้ไข
        Task<ModelMenuC31_InterfaceData> MenuC31EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC31Message> UpdateDocMenuC31EditAsync(ModelMenuC31 model);




        //ระเบียบวาระที่ 2 ----------------------------------------------------------------------------------------
        Task<ModelMenuC32_InterfaceData> MenuC32InterfaceDataAsync(string RegisterId);
        Task<bool> MenuC32CheckAttachmentAsync(int meetingid);
        Task<ModelMenuC32_DownloadFileName> MenuC32DownloadAttachmentNameAsync(int meetingid);
        Task<ModelMenuC32_DownloadFile> GetC32DownloadFileByIdAsync(int meetingid, int Id);
        Task<ModelResponseC32Message> AddDocMenuC32Async(ModelMenuC32 model);


        //ระเบียบวาระที่ 2 แก้ไข
        Task<ModelMenuC32_InterfaceData> MenuC32EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC32Message> UpdateDocMenuC32EditAsync(ModelMenuC32 model);



        //ระเบียบวาระที่ 3 ----------------------------------------------------------------------------------------
        Task<ModelMenuC33_InterfaceData> MenuC33InterfaceDataAsync(string RegisterId);
        Task<ModelResponseC33Message> AddDocMenuC33Async(ModelMenuC33 model);
        Task<ModelMenuC33Data> GetProjectNumberWithDataC3Tab3Async(string project_number);
        Task<IList<ModelSelectOption>> GetAllApprovalTypeByProjectC2ForTab3Async(string project_number);
        Task<IList<ModelMenuC33HistoryData>> GetAllHistoryDataC3Tab3Async();

        //ระเบียบวาระที่ 3 แก้ไข
        Task<ModelMenuC33_InterfaceData> MenuC33EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC33Message> UpdateDocMenuC33EditAsync(ModelMenuC33 model);



        //ระเบียบวาระที่ 4 ----------------------------------------------------------------------------------------
        Task<ModelMenuC34_InterfaceData> MenuC34InterfaceDataAsync(string RegisterId);
        Task<ModelResponseC34Message> AddDocMenuC34Async(ModelMenuC34 model);
        Task<ModelMenuC34Tab4Data> GetProjectNumberWithDataC3Tab4Async(int type, string project_number);
        Task<IList<ModelSelectOption>> GetAllProjectNumberTab4Async(int type);
        Task<ModelMenuC34_DownloadFile> GetC34DownloadFileByIdAsync(int docid);

        //ระเบียบวาระที่ 4 แก้ไข
        Task<ModelMenuC34_InterfaceData> MenuC34EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC34Message> UpdateDocMenuC34EditAsync(ModelMenuC34 model);



        //ระเบียบวาระที่ 5 ----------------------------------------------------------------------------------------
        Task<ModelMenuC35_InterfaceData> MenuC35InterfaceDataAsync(string RegisterId);
        Task<ModelResponseC35Message> AddDocMenuC35Async(ModelMenuC35 model);


        //ระเบียบวาระที่ 5 แก้ไข
        Task<ModelMenuC35_InterfaceData> MenuC35EditInterfaceDataAsync(string UserId, string ProjectNumber);
        Task<ModelResponseC35Message> UpdateDocMenuC35EditAsync(ModelMenuC35 model);


        //พิมพ์วาระการประชุม -------------------------------------------------------
        Task<ModelResponseMessageReportAgenda> PrintReportAgendaDraftAsync(int DocId, int Round, int Year);

        Task<ModelResponseMessageReportAgenda> PrintReportAgendaRealAsync(ModelPrintMeeting model);


        //พิมพ์รายงานการประชุม -------------------------------------------------------

        Task<ModelResponseMessageReportMeeting> PrintReportMeetingDraftAsync(int DocId, int Round, int Year);

        Task<ModelResponseMessageReportMeeting> PrintReportMeetingRealAsync(ModelPrintMeeting model);

    }
}
