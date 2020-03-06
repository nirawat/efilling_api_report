using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{

    public class ModelGetLinkResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public DateTime docDate { get; set; }
        public string docNumber { get; set; }
    }

}
