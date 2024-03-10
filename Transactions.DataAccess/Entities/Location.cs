using System.ComponentModel.DataAnnotations;

namespace Transactions.DataAccess.Entities;

public struct Location
{
    [Key]
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public override string ToString() =>  $"{Latitude}, {Longitude}";
}