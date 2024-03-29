﻿// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.Firebase;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Aguacongas.Identity.Firebase.IntegrationTest
{
    public class FirebaseTestFixture
    {
        public IConfigurationRoot Configuration { get; private set; }
        public FirebaseOptions FirebaseOptions { get; private set; }

        public string TestDb { get; } = DateTime.Now.ToString("s");
        public FirebaseTestFixture()
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../../../../testsettings.json"))
                .Build();

            FirebaseOptions = new FirebaseOptions();
            Configuration.GetSection("FirebaseOptions").Bind(FirebaseOptions);
        }
    }
}
