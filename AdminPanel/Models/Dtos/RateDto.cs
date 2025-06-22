using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models.Dtos;

public class RateDto
{
    public int Id { get; set; }

    [Range(0, int.MaxValue)]
    public decimal Value { get; set; }
}
