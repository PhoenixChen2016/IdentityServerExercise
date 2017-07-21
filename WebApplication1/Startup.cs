using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddIdentityServer()
			.AddTemporarySigningCredential()
			.AddInMemoryIdentityResources(new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			})
			.AddInMemoryApiResources(new[] {
				new ApiResource("api1", "My API")
			})
			.AddInMemoryClients(new[] {
				new Client{
					ClientId = "client",
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					AllowedScopes =
					{
						"api1"
					}
				},
				new Client
				{
					ClientId = "ro.client",
					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					AllowedScopes = { "api1" }
				},
				new Client
				{
					ClientId = "mvc",
					ClientName = "MVC Client",
					AllowedGrantTypes = GrantTypes.Implicit,

					// where to redirect to after login
					RedirectUris = { "http://localhost:5002/signin-oidc" },

					// where to redirect to after logout
					PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
					},
					AllowOfflineAccess = true
				},
				new Client
				{
					ClientId = "mvc1",
					ClientName = "MVC Client",
					AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},

					// where to redirect to after login
					RedirectUris = { "http://localhost:5003/signin-oidc" },

					// where to redirect to after logout
					PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"api1"
					},
					AllowOfflineAccess = true
				}
			})
			.AddTestUsers(new List<TestUser>
			{
				new TestUser
				{
					SubjectId = "1",
					Username = "alice",
					Password = "123",
					Claims = new []
					{
						new Claim("name", "Alice"),
						new Claim("website", "https://alice.com")
					}
				},
				new TestUser
				{
					SubjectId = "2",
					Username = "bob",
					Password = "123",
					Claims = new []
					{
						new Claim("name", "Bob"),
						new Claim("website", "https://bob.com")
					}
				}
			});

			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseIdentityServer();

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();
		}
	}
}
