using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Drug : Base
    {
        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Price")]
        public float Price { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "This Filed is Required")]
        [DisplayName("Description")]
        public String Description { get; set; }

        [DisplayName("Image Drug")]
        public String ImageName { get; set; }

    }
}
