using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {
        private IConfiguration config;

        private static readonly string AuthorityUrl = "https://login.windows.net/common/oauth2/authorize/";
        private static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";
        private string ApplicationId;
        private static readonly string ApiUrl = "https://api.powerbi.com";
        private string WorkspaceId;

        private string ApplicationSecret;
        private string Tenant;

        public PowerBIController(IConfiguration configuration)
        {
            config = configuration;

            ApplicationId = config["Data:powerBIAppId"];
            WorkspaceId = config["Data:demoWorkspace"];
            ApplicationSecret = config["Data:powerBIAppSecret"];
            Tenant = config["Data:tenantId"];
        }

        [HttpGet("tile")]
        public async Task<TileEmbedConfig> GetTileToken(string tileId)
        {
            var tokenCredentials = await GetTokenCredentials();

            using (var client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials))
            {
                var groups = await client.Groups.GetGroupsAsync();
                // Get a list of dashboards.
                var dashboards = await client.Dashboards.GetDashboardsInGroupAsync(WorkspaceId);

                // Get the first report in the workspace.
                var dashboard = dashboards.Value.FirstOrDefault();

                if (dashboard == null)
                {
                    Debug.WriteLine("No Dashboards Found");
                }

                var tiles = await client.Dashboards.GetTilesInGroupAsync(WorkspaceId, dashboard.Id);

                // Get the first tile in the workspace.
                var tile = tiles.Value.FirstOrDefault();

                // Generate Embed Token for a tile.
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = await client.Tiles.GenerateTokenInGroupAsync(WorkspaceId, dashboard.Id, tile.Id, generateTokenRequestParameters);

                if (tokenResponse == null)
                {
                    Debug.WriteLine("No TOKEN rECEIVED");
                }

                // Generate Embed Configuration.
                return new TileEmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = tile.EmbedUrl
                };
            }
        }

        private async Task<TokenCredentials> GetTokenCredentials()
        {

            // Authenticate using created credentials
            AuthenticationResult authenticationResult = null;
            try
            {
                var tenantSpecificURL = AuthorityUrl.Replace("common", Tenant);
                var authenticationContext = new AuthenticationContext(tenantSpecificURL);

                // Authentication using app credentials
                var credential = new ClientCredential(ApplicationId, ApplicationSecret);
                authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl, credential);
            }
            catch (AggregateException exc)
            {
                Debug.WriteLine(exc.Message);
            }

            if (authenticationResult == null)
            {
                Debug.WriteLine("Auth Falied");
            }

            return new TokenCredentials(authenticationResult.AccessToken, "Bearer");
        }
    }
}