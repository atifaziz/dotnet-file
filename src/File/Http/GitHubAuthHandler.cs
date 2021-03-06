﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitHub;
using Microsoft.Git.CredentialManager;

namespace Microsoft.DotNet
{
    class GitHubAuthHandler : AuthHandler
    {
        ICredential? credential;

        public GitHubAuthHandler(HttpMessageHandler inner) : base(inner)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var creds = await GetCredentialAsync();
            var builder = new UriBuilder(request.RequestUri);
            builder.UserName = creds.Password;
            builder.Password = "x-auth-basic";

            // retry the request
            var retry = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            retry.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(creds.Password)));
            foreach (var etag in request.Headers.IfNoneMatch)
            {
                retry.Headers.IfNoneMatch.Add(etag);
            }

            return await base.SendAsync(retry, cancellationToken);
        }

        async Task<ICredential> GetCredentialAsync()
        {
            if (credential != null)
                return credential;

            var input = new InputArguments(new Dictionary<string, string>
            {
                ["protocol"] = "https",
                ["host"] = "github.com",
            });

            var provider = new GitHubHostProvider(new CommandContext());

            credential = await provider.GetCredentialAsync(input);
            return credential;
        }
    }
}
