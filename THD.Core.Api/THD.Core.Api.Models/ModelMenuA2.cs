using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuA2_InterfaceData
    {
        public IList<ModelSelectOption> ListLaboratoryRoom { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelMenuA2
    {
        public string docid { get; set; }
        public DateTime docdate { get; set; }
        public string projectaccordingtypemethod { get; set; }
        public string facultylaboratory { get; set; }
        public string department { get; set; }
        public string laboratoryaddress { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string roomnumber { get; set; }
        public string labothername { get; set; }
        public string telephone { get; set; }
        public string responsibleperson { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string filename1 { get; set; }
        public string filename1base64 { get; set; }
        public string filename2 { get; set; }
        public string filename2base64 { get; set; }
        public string createby { get; set; }
        public bool editenable { get; set; }
    }



}
