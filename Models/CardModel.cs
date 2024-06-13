using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationToSellThings.APIs.Models
{
    public class CardModel
    {
        [Key]
        public Guid CardId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Cvv { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
