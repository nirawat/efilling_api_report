using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelRegisterUser
    {
        public string registerid { get; set; }
        [Required(ErrorMessage = "userid is required.")]
        public string userid { get; set; }
        [Required(ErrorMessage = "password is required.")]
        //[StringLength(6, MinimumLength = 10, ErrorMessage = "password is required 6-10 character.")]
        public string passw { get; set; }
        [Required(ErrorMessage = "confirm password is required.")]
        //[StringLength(6, MinimumLength = 10, ErrorMessage = "confirm password is required 6-10 character.")]
        public string confirmpassw { get; set; }
        [Required(ErrorMessage = "email is required.")]
        public string email { get; set; }
        public DateTime registerdate { get; set; }
        public DateTime registerexpire { get; set; }
    }

}
