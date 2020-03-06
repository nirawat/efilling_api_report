using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelUserLogin
    {
        [Required(ErrorMessage = "userid is required.")]
        public string userid { get; set; }
        [Required(ErrorMessage = "password is required.")]
        public string passw { get; set; }
        public string registerid { get; set; }
    }
}
