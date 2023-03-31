using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParksLookupApi.Models
{
  public class Review
  {

    public int ReviewId { get; set; }
    [ForeignKey("Park")]
    public int ParkId { get; set; }
    
    [StringLength(120)]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public string UserName { get; set; }
  }
}

