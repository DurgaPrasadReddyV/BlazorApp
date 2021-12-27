using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Host.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
}