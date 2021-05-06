# Aguacongas.Identity.Firestore
Google Firestore-based ASP.NET Core Identity user & role stores.

## Basic usage
1. Set `GOOGLE_APPLICATION_CREDENTIALS` environment variable to a path to the JSON file for GCP service account (see https://cloud.google.com/docs/authentication/getting-started#setting_the_environment_variable)
1. In `Startup.cs` of your ASP.NET Core project:
    ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
      // ... your other services 
       services
       .AddIdentity<ApplicationUser, IdentityRole>()
       .AddFirestoreStores("<YOUR GCP PROJECT ID>")
       .AddDefaultTokenProviders();
   
      // ... rest of your configuration
   }
    ```
### Using custom Firestore table names
Useful when you want to deploy multiple applications using Firestore-based Identity stores within the same GCP project.
 ```csharp
public void ConfigureServices(IServiceCollection services)
{
   // ... your other services
   
   services
   .AddIdentity<ApplicationUser, IdentityRole>()
   .AddFirestoreStores("<YOUR GCP PROJECT ID>", tableNames => {
      // Option 1 - set individual fields in tableNames (class Aguacongas.Identity.Firestore.FirestoreTableNamesConfig)
      tableNames.UsersTableName = "awesome-users-table-1";
      
      // Option 2 - add suffix to all table names at once (all table names will have default name + the specified suffix)
      tableNames.WithSuffix("-staging");
   })
   .AddDefaultTokenProviders();

   // ... rest of your configuration
}
```