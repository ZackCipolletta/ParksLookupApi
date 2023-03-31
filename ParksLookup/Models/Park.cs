using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ParksLookupApi.Models
{
  public class Park
  {

    public int ParkId { get; set; }
    public string Type { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public virtual List<Review> Reviews { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int? ReviewCount { get; set; }
    public void UpdateReviewCount()
    {
        ReviewCount = Reviews?.Count ?? 0;
    }
  }
}