# Identity.Firebase
[ASP.NET Identity](https://github.com/aspnet/AspNetCore/tree/master/src/Identity) Firebase Providers

[![Build status](https://ci.appveyor.com/api/projects/status/h3n8dna94b156o58/branch/develop?svg=true)](https://ci.appveyor.com/project/aguacongas/identity-firebase) 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aguacongas_Identity.Firebase&metric=alert_status)](https://sonarcloud.io/dashboard?id=aguacongas_Identity.Firebase)

Nuget packages
--------------
|Aguacongas.Firebase|Aguacongas.Identity.Firebase|Aguacongas.Identity.Firestore|
|:------:|:------:|:------:|
[![][Aguacongas.Firebase-badge]][Aguacongas.Firebase-nuget]|[![][Aguacongas.Identity.Firebase-badge]][Aguacongas.Identity.Firebase-nuget]|[![][Aguacongas.Identity.Firestore-badge]][Aguacongas.Identity.Firestore-nuget]|
[![][Aguacongas.Firebase-downloadbadge]][Aguacongas.Firebase-nuget]|[![][Aguacongas.Identity.Firebase-downloadbadge]][Aguacongas.Identity.Firebase-nuget]|[![][Aguacongas.Identity.Firestore-downloadbadge]][Aguacongas.Identity.Firestore-nuget]|


[Aguacongas.Firebase-badge]: https://img.shields.io/nuget/v/Aguacongas.Firebase.svg
[Aguacongas.Firebase-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.Firebase.svg
[Aguacongas.Firebase-nuget]: https://www.nuget.org/packages/Aguacongas.Firebase/

[Aguacongas.Identity.Firebase-badge]: https://img.shields.io/nuget/v/Aguacongas.Identity.Firebase.svg
[Aguacongas.Identity.Firebase-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.Identity.Firebase.svg
[Aguacongas.Identity.Firebase-nuget]: https://www.nuget.org/packages/Aguacongas.Identity.Firebase/

[Aguacongas.Identity.Firestore-badge]: https://img.shields.io/nuget/v/Aguacongas.Identity.Firestore.svg
[Aguacongas.Identity.Firestore-downloadbadge]: https://img.shields.io/nuget/dt/Aguacongas.Identity.Firestore.svg
[Aguacongas.Identity.Firestore-nuget]: https://www.nuget.org/packages/Aguacongas.Identity.Firestore/

## Setup

Read [Authentication](https://github.com/aguacongas/Identity.Firebase/wiki/Authentication) wiki page to setup stores.

## Samples

Samples are copies of [IdentitySample.Mvc](https://github.com/aspnet/Identity/tree/dev/samples/IdentitySample.Mvc) sample using a firebase database (Realtime or Firestore).  

## Tests

Thoses libraries are tested using [Microsoft.AspNetCore.Identity.Specification.Tests](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.Specification.Tests/), the shared test suite for Asp.Net Identity Core store implementations.  
