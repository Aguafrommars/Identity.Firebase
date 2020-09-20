# [3.2.0](https://github.com/Aguafrommars/Identity.Firebase/compare/3.1.0...3.2.0) (2020-09-20)


### Bug Fixes

* update packages ([6733313](https://github.com/Aguafrommars/Identity.Firebase/commit/673331312456883022ee5486ead0742913104809))


### Features

* firestore auth options from auth file ([1e4b8d5](https://github.com/Aguafrommars/Identity.Firebase/commit/1e4b8d52a1e0eeb737097f752a84fb2b9d120664))
* fix tests ([1c08d06](https://github.com/Aguafrommars/Identity.Firebase/commit/1c08d069454e47e38c0b7b82937e932e62c9b6cb))

# [3.0.0](https://github.com/aguacongas/Identity.Firebase/compare/2.2.1...3.0.0) (2019-09-26)


### Build System

* update packages ([b1d2a00](https://github.com/aguacongas/Identity.Firebase/commit/b1d2a00))


### BREAKING CHANGES

* APS.Net Core 3.0

## [2.2.1](https://github.com/aguacongas/Identity.Firebase/compare/2.2.0...2.2.1) (2018-12-19)


### Bug Fixes

* nugets packages version ([bb948bb](https://github.com/aguacongas/Identity.Firebase/commit/bb948bb))

# [2.2.0](https://github.com/aguacongas/Identity.Firebase/compare/2.1.1...2.2.0) (2018-12-16)

### Features

* .net 2.2 ([2cc92b9](https://github.com/aguacongas/Identity.Firebase/commit/2cc92b9))
* **ci:** use githubflow and semantic-release ([79b0537](https://github.com/aguacongas/Identity.Firebase/commit/79b0537))

# 2.1.0 (Jul. 28 2018)

# Feature

* ASP.Net Core 2.1 update

# 2.0.0 (Jun. 6 2018)

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
