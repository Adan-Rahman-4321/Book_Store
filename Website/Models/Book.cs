using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }


        [Required,
         MaxLength(length: 500)]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [DataType(DataType.Date),
         Display(Name = "Upload Date")]
        public DateTime UploadedDate { get; set; }

        [Required,
         DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Select Discount")]
        public DiscountType DiscountType { get; set; }

        [Range(0,
         99, ErrorMessage = "Please enter a valid discount amount")]
        public decimal DiscountAmount { get; set; }


        [Display(Name = "Final Price")]
        public decimal FinalPrice { get; set; }


        [Display(Name = "Image")]
        public string ImgUrl { get; set; }


    }
}
