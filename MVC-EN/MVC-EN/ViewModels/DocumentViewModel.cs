using System.ComponentModel.DataAnnotations;

namespace MVC_EN.ViewModels
{
  public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        [Display(Name = "Type"), Required]
        public string DocumentType { get; set; }

        [Display(Name = "Doc. No"), Required]
        public int DocumentNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        [Required]
        public DateTime DocumentDate { get; set; }

        [Display(Name = "Partner"), Required]
        public int? PartnerId { get; set; }
        public string PartnerName { get; set; }

        [Display(Name = "Previous document")]
        public int? PreviousDocumentId { get; set; }
        public string PreviousDocumentName { get; set; }

        [Display(Name = "VAT (in %)")]
        [Range(0, 100, ErrorMessage = "Express VAT rate as integer percentage from 0 to 100")]        
        public int VatAsInt { get; set; }

        public decimal VAT
        {
            get
            {
                return VatAsInt / 100m;
            }
            set
            {
                VatAsInt = (int) (100m * value);
            }
        }

        public decimal Amount { get; set; }
        public IEnumerable<ItemViewModel> Items { get; set; }       

        public DocumentViewModel()
        {
            this.Items = new List<ItemViewModel>();
        }
    }
}
