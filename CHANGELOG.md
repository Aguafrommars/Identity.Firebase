# 2.0.0-beta.1 (Mar. 18 2018)

Takes advantage of new [HttpClientFactory](https://github.com/aspnet/HttpClientFactory)

## Improvements

* Takes advantage of new [HttpClientFactory](https://github.com/aspnet/HttpClientFactory). The underlying HttpClient is created with a named instance
* New IServiceCollection extensions methods
* Better code documentation

## Breaking changes

* FirebaseClient constructor takes an HttpClient and a FirebaseOptions instances
* New DelegatingHandler, FirebaseAuthenticationHandler to manage authentication. Authentication is now manage by this handler
* AuthRequest renamed EmailPasswordAuthRequest
* AuthTokenManager renamed OAuthTokenManager
* AuthOptions renamed OAuthServiceAccountKey
* AuthTokenManager renamed OAuthTokenManager
* IFirebaseTokenManager doesn't extends IDisposable anymore

# 1.1.1 (Mar. 02 2018)

## Bug Fixe  

* [#1 entity cannot be updated/removed when retrieved throught IQueryable](https://github.com/aguacongas/Identity.Firebase/issues/1)

# 1.1.0 (Mar. 02 2018)

Implement `IQueryableRoleStore<>` and `IQueryableUserStore<>`  

# 1.0.0 (Feb. 21 2018)

Initial release