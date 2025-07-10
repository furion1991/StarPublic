using Microsoft.AspNetCore.Mvc;

namespace CasesService.Controllers;

[ApiController]
[Route("/cases")]
public class CategoryController : ControllerBase
{
    [HttpPost("category/{name}")]
    public async Task<IActionResult> CreateCategory([FromForm] string name, IFormFile file)
    {
        return Ok();
    }
}