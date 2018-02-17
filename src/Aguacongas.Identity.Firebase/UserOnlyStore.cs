using Aguacongas.Firebase;
using Aguacongas.Identity.Firebase.Internal;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Creates a new instance of a persistence store for the specified user type.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class UserOnlyStore<TUser> : UserOnlyStore<TUser, string> where TUser : IdentityUser<string>, new()
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserOnlyStore{TUser}"/>.
        /// </summary>
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserOnlyStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer) { }

    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    public abstract class UserOnlyStore<TUser, TKey> : UserOnlyStore<TUser, TKey, IdentityUserClaim<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TKey}"/>.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserOnlyStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer) { }
    }

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserOnlyStore<TUser, TKey, TUserClaim, TUserLogin, TUserToken> :
        UserStoreBase<TUser, TKey, TUserClaim, TUserLogin, TUserToken>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private readonly IFirebaseClient _client;

        /// <summary>
        /// Creates a new instance of the store.
        /// </summary>
        /// <param name="context">The context used to access the store.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserOnlyStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(describer ?? new IdentityErrorDescriber())
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            _client = client;
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public async override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var response = await _client.PostAsync("users", user, cancellationToken);
            user.Id = ConvertIdFromString(response.Data);
            SetConcurrencyStamp(user, response.Etag);

            // Create indexes
            var list = new List<Task>(2);
            list.Add(_client.PutAsync($"indexes/users-names/{user.NormalizedUserName}", user.Id, cancellationToken));
            if (!string.IsNullOrEmpty(user.NormalizedEmail))
            {
                list.Add(_client.PutAsync($"indexes/users-email/{user.NormalizedEmail}", user.Id, cancellationToken));
            }

            await Task.WhenAll(list.ToArray());
            
            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public async override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            string eTag = null;
            try
            {
                var response = await _client.PutAsync($"users/{user.Id}", user, cancellationToken, true, GetEtag(user));
                eTag = response.Etag;
            }
            catch (FirebaseException e)
            {
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
                }
                throw;
            }

            // Update indexes
            var list = new List<Task>();
            var state = user.ConcurrencyStamp.Split(';');

            var oldUserName = state[0];
            if (oldUserName != user.NormalizedUserName)
            {
                list.Add(_client.DeleteAsync($"indexes/users-names/{oldUserName}", cancellationToken));
                list.Add(_client.PutAsync($"indexes/users-names/{user.NormalizedUserName}", user.Id, cancellationToken));
            }

            var oldEmail = state[1];
            if (!string.IsNullOrEmpty(user.NormalizedEmail) && oldEmail != user.NormalizedEmail)
            {
                list.Add(_client.DeleteAsync($"indexes/users-email/{oldEmail}", cancellationToken));
                list.Add(_client.PutAsync($"indexes/users-email/{user.NormalizedEmail}", user.Id, cancellationToken));
            }

            await Task.WhenAll(list.ToArray());

            SetConcurrencyStamp(user, eTag);
            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="user"/> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public async override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                await _client.DeleteAsync($"users/{user.Id}", cancellationToken, true, GetEtag(user));
            }
            catch (FirebaseException e)
            {
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
                }
                throw;
            }
            return IdentityResult.Success;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var response = await _client.GetAsync<TUser>($"users/{userId}", cancellationToken, true);
            var user = response.Data;
            if (user != null)
            {
                user.Id = ConvertIdFromString(userId);
                SetConcurrencyStamp(user, response.Etag);
            }

            return user;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var response = await _client.GetAsync<string>($"indexes/users-names/{normalizedUserName}", cancellationToken);
            if (!string.IsNullOrEmpty(response.Data))
            {
                return await FindByIdAsync(response.Data, cancellationToken);
            }
            return default(TUser);
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public async override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var response = await _client.GetAsync<IEnumerable<TUserClaim>>($"claims/{user.Id}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                return data.Select(c => c.ToClaim()).ToList();
            }
            return new List<Claim>(0);
        }

        /// <summary>
        /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The claim to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            List<TUserClaim> userClaims = claims.Select(c => CreateUserClaim(user, c)).ToList();
            var response = await _client.GetAsync<IEnumerable<TUserClaim>>($"claims/{user.Id}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                userClaims.AddRange(data);
            }

            await _client.PutAsync($"claims/{user.Id}", userClaims, cancellationToken);
        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var response = await _client.GetAsync<IEnumerable<TUserClaim>>($"claims/{user.Id}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                var matchedClaims = data.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
                foreach (var matchedClaim in matchedClaims)
                {
                    matchedClaim.ClaimValue = newClaim.Value;
                    matchedClaim.ClaimType = newClaim.Type;
                }
                await _client.PutAsync($"claims/{user.Id}", data, cancellationToken);
            }
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var response = await _client.GetAsync<List<TUserClaim>>($"claims/{user.Id}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                foreach (var claim in claims)
                {
                    data.RemoveAll(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
                }
                await _client.PutAsync($"claims/{user.Id}", data, cancellationToken);
            }
        }

        /// <summary>
        /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task AddLoginAsync(TUser user, UserLoginInfo login,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var response = await _client.GetAsync<List<TUserLogin>>($"users-logins/{user.Id}", cancellationToken);
            var data = response.Data ?? new List<TUserLogin>(0);
            data.Add(CreateUserLogin(user, login));

            await _client.PutAsync($"users-logins/{user.Id}", data, cancellationToken);
            await _client.PutAsync($"indexes/provider-keys/{login.LoginProvider}/{login.ProviderKey}", user.Id, cancellationToken);
        }

        /// <summary>
        /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var response = await _client.GetAsync<List<TUserLogin>>($"users-logins/{user.Id}", cancellationToken);
            var data = response.Data ?? new List<TUserLogin>(0);
            data.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);

            await _client.PutAsync($"users-logins/{user.Id}", data, cancellationToken);
            await _client.DeleteAsync($"indexes/provider-keys/{loginProvider}/{providerKey}", cancellationToken);
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public async override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var response = await _client.GetAsync<List<TUserLogin>>($"users-logins/{user.Id}", cancellationToken);
            var data = response.Data ?? new List<TUserLogin>(0);

            return data.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList();
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key.
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        public async override Task<TUser> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
            {
                return await FindUserAsync(userLogin.UserId, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var response = await _client.GetAsync<TKey>($"indexes/users-email/{normalizedEmail}", cancellationToken);
            if (response.Data != null)
            {
                return await FindByIdAsync(response.Data.ToString(), cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Retrieves all users with the specified claim.
        /// </summary>
        /// <param name="claim">The claim whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim. 
        /// </returns>
        public async override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var response = await _client.GetAsync<IEnumerable<KeyValue<IEnumerable<TUserClaim>>>>($"claims", cancellationToken);
            var data = response.Data;
            var users = new List<TUser>();
            if (data != null)
            {
                var userIds = data.Where(uc => uc.Value.Any(c => c.ClaimType == claim.Type && c.ClaimValue == c.ClaimValue))
                    .Select(uc => uc.Key);

                foreach (var userId in userIds)
                {
                    var user = await FindByIdAsync(userId, cancellationToken);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Return a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        internal Task<TUserLogin> FindUserLoginInternalAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return FindUserLoginAsync(userId, loginProvider, providerKey, cancellationToken);
        }

        /// <summary>
        /// Return a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        internal Task<TUserLogin> FindUserLoginInternalAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
        }

        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        internal Task<TUserToken> FindTokenInternalAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return FindTokenAsync(user, loginProvider, name, cancellationToken);
        }

        /// <summary>
        /// Add a new user token.
        /// </summary>
        /// <param name="token">The token to be added.</param>
        /// <returns></returns>
        internal Task AddUserTokenInternalAsync(TUserToken token)
        {
            return AddUserTokenAsync(token);
        }


        /// <summary>
        /// Remove a new user token.
        /// </summary>
        /// <param name="token">The token to be removed.</param>
        /// <returns></returns>
        internal Task RemoveUserTokenInternalAsync(TUserToken token)
        {
            return RemoveUserTokenAsync(token);
        }
        
        /// <summary>
        /// Return a user with the matching userId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user if it exists.</returns>
        protected override Task<TUser> FindUserAsync(TKey userId, CancellationToken cancellationToken)
        {
            return FindByIdAsync(userId.ToString(), cancellationToken);
        }

        /// <summary>
        /// Return a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected override async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<IEnumerable<TUserLogin>>($"users-logins/{userId}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                return data.SingleOrDefault(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
            }
            return null;
        }

        /// <summary>
        /// Return a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<TKey>($"indexes/provider-keys/{loginProvider}/{providerKey}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                return await FindUserLoginAsync(data, loginProvider, providerKey, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected override async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<IEnumerable<KeyValue<KeyValue<string>>>>($"users-tokens/{user.Id}", cancellationToken);
            if (response.Data != null)
            {
                return response.Data
                    .Select(d => new TUserToken
                    {
                        UserId = user.Id,
                        LoginProvider = d.Key,
                        Name = d.Value.Key,
                        Value = d.Value.Value
                    })
                    .Where(t => t.LoginProvider == loginProvider && t.Name == name)
                    .FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Add a new user token.
        /// </summary>
        /// <param name="token">The token to be added.</param>
        /// <returns></returns>
        protected override async Task AddUserTokenAsync(TUserToken token)
        {
            await _client.PutAsync($"users-tokens/{token.UserId}/{token.LoginProvider}/{token.Name}", token);
        }


        /// <summary>
        /// Remove a new user token.
        /// </summary>
        /// <param name="token">The token to be removed.</param>
        /// <returns></returns>
        protected override async Task RemoveUserTokenAsync(TUserToken token)
        {
            await _client.DeleteAsync($"users-tokens/{token.UserId}/{token.LoginProvider}/{token.Name}");
        }

        private void SetConcurrencyStamp(TUser user, string eTag)
        {
            user.ConcurrencyStamp = $"{user.NormalizedUserName};{user.NormalizedEmail};{eTag}";
        }

        private string GetEtag(TUser user)
        {
            if (user.ConcurrencyStamp == null)
            {
                return null;
            }
            var state = user.ConcurrencyStamp.Split(';');
            return state.Length == 3 ? state[2] : null;
        }
    }
}
