using System.ComponentModel.DataAnnotations;

namespace Transactions.DataAccess.Entities;

public struct Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}