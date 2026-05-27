using Microsoft.AspNetCore.Mvc;

namespace Bifrost.Request;

using System.ComponentModel.DataAnnotations;

public class PaginationQueryRequest
{
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue, ErrorMessage = "page must be greater than 0")]
    public int Page { get; set; } = 1;
    
    [FromQuery(Name = "size")]
    [Range(1, int.MaxValue, ErrorMessage = "size must be greater than 0")]
    public int Size { get; set; } = 10;
}