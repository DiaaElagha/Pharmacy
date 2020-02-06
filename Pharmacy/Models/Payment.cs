using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Payment : Base
    {
        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Drug")]
        public int DrugId { get; set; }
        [ForeignKey("DrugId")]
        public Drug Drug { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("pharmacist")]
        public String UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Total")]
        public float Total { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Quntity")]
        public float Quntity { get; set; }
    }
}
