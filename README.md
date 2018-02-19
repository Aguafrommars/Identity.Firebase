# Identity.Firebase
[Identity](https://github.com/aspnet/identity) stores implementation for Firebase

[![Build status](https://ci.appveyor.com/api/projects/status/github/aguacongas/chatle?svg=true)](https://ci.appveyor.com/project/aguacongas/chatle)
[![Latest Code coveragre](https://aguacongas.github.io/Identity.Firebase/latest/badge_linecoverage.svg)](https://aguacongas.github.io/Identity.Firebase/latest)

## Setup

Create a user on your firebase database and configure an `EmailPasswordOptions` and `FirebaseOptions` in your Startup class according to the database user.  
And add firebase stores to Identity using `AddFirebaseStores`.

    services.Configure<EmailPasswordOptions>(options =>
        {
            Configuration.GetSection("EmailPasswordOptions").Bind(options);
        }).Configure<FirebaseOptions>(options =>
        {
            Configuration.GetSection("FirebaseOptions").Bind(options);
        }).AddIdentity<ApplicationUser, IdentityRole>()
        .AddFirebaseStores()
        .AddDefaultTokenProviders();

Your configuration file can look like this:

    {
        "EmailPasswordOptions: {
            Password": "the database user password",
            "Email": "the database user eMail",
            "ApiKey": "the database api key",
        }
        "FirebaseOptions: {
            "DatabaseUrl": "the database url"
        }
    }

## Sample

The sample is a copy of [IdentitySample.Mvc](https://github.com/aspnet/Identity/tree/dev/samples/IdentitySample.Mvc) sample using a firebase database.  
To run it you must configure an `EmailPasswordOptions` and `FirebaseOptions` by either modify the appsettings.json file, use the [secret manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio) or environments variables.  

