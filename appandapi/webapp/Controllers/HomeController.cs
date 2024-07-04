using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using webapp.Models;

namespace webapp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GraphServiceClient _graphServiceClient;
    private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
    private string[]? _graphScopes;

    public HomeController(
        ILogger<HomeController> logger,
        IConfiguration configuration,
        GraphServiceClient graphServiceClient,
        MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
        this._consentHandler = consentHandler;
        _graphScopes = configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
    }

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public IActionResult Index()
    {
        return View();
    }

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public async Task<IActionResult> Profile()
    {
        User? currentUser = null;
        currentUser = await _graphServiceClient.Me.Request().GetAsync();
        ViewData["Me"] = currentUser;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
