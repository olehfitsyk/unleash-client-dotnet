﻿using System;
using System.Threading;
using NUnit.Framework;
using Unleash.Communication;
using Unleash.Serialization;

namespace Unleash.Tests.Communication
{
    public abstract class BaseUnleashApiClientTest
    {
        private static IUnleashApiClient CreateApiClient()
        {
            var apiUri = new Uri("http://unleash.herokuapp.com/");

            var jsonSerializer = new DynamicNewtonsoftJsonSerializer();
            jsonSerializer.TryLoad();

            var httpClientFactory = new DefaultHttpClientFactory();

            var requestHeaders = new UnleashApiClientRequestHeaders
            {
                AppName = "api-test-client",
                InstanceTag = "instance1",
                CustomHttpHeaders = null
            };

            var httpClient = httpClientFactory.Create(apiUri);
            var client = new UnleashApiClient(httpClient, jsonSerializer, requestHeaders);
            return client;
        }

        internal IUnleashApiClient api
        {
            get => TestContext.CurrentContext.Test.Properties.Get("api") as IUnleashApiClient;
            set => TestContext.CurrentContext.Test.Properties.Set("api", value);
        }

        [SetUp]
        public void SetupTest()
        {
            api = CreateApiClient();
        }
    }
}