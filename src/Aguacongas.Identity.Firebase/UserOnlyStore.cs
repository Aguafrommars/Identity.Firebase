using Aguacongas.Firebase;
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
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class UserOnlyStore<TUser> : UserOnlyStore<TUser, IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityUserToken<string>>
        where TUser : IdentityUser<string>
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
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserOnlyStore<TUser, TUserClaim, TUserLogin, TUserToken> :
        FirebaseUserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>,
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
        where TUser : IdentityUser<string>
        where TUserClaim : IdentityUserClaim<string>, new()
        where TUserLogin : IdentityUserLogin<string>, new()
        where TUserToken : IdentityUserToken<string>, new()
    {
        private const string UsersTableName = "users";

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
            var response = await _client.PostAsync(UsersTableName, user, cancellationToken);
            user.Id = response.Data;
            SetConcurrencyStamp(user, response.Etag);
            
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

            try
            {
                var response = await _client.PutAsync(GetFirebasePath(UsersTableName, user.Id), user, cancellationToken, true, GetEtag(user));
                SetConcurrencyStamp(user, response.Etag);
            }
            catch (FirebaseException e)
                when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

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
                await _client.DeleteAsync(GetFirebasePath(UsersTableName, user.Id), cancellationToken, true, GetEtag(user));
            }
            catch (FirebaseException e)
                when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
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
            var response = await _client.GetAsync<TUser>(GetFirebasePath(UsersTableName, userId), cancellationToken, true);
            return CreateUser(userId, response);
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
            try
            {
                var response = await _client.GetAsync<TUser>(UsersTableName, cancellationToken, queryString: $"orderBy=\"NormalizedUserName\"&equalTo=\"{normalizedUserName}\"");
                return CreateUser(response.Data.Id, response);
            }
            catch(FirebaseException e)
                when (e.FirebaseError.Error.StartsWith("Index"))
            {
                var rules = await _client.GetAsync<FirebaseRules>(".settings/rules.json", cancellationToken);
                AddUserIndexes(rules.Data.Rules);
                await _client.PutAsync(".settings", rules.Data, cancellationToken);

                var response = await _client.GetAsync<TUser>(UsersTableName, cancellationToken, queryString: $"orderBy=\"NormalizedUserName\"&equalTo=\"{normalizedUserName}\"");
                return CreateUser(response.Data.Id, response);
            }
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
            var taskList = new List<Task>(userClaims.Count + 1);
            taskList.Add(_client.PutAsync($"claims/{user.Id}", userClaims, cancellationToken));
            foreach(var uc in userClaims)
            {
                taskList.Add(IndexClaim(user, uc.ClaimType, uc.ClaimValue, cancellationToken));
            }

            await Task.WhenAll(taskList.ToArray());
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
            var userClaims = response.Data;
            if (userClaims != null)
            {
                var matchedClaims = userClaims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
                var taskList = new List<Task>((matchedClaims.Count() * 2) + 1);
                foreach (var matchedClaim in matchedClaims)
                {
                    taskList.Add(RemoveClaimIndex(user, matchedClaim.ClaimType, matchedClaim.ClaimValue, cancellationToken));
                    matchedClaim.ClaimValue = newClaim.Value;
                    matchedClaim.ClaimType = newClaim.Type;
                    taskList.Add(IndexClaim(user, matchedClaim.ClaimType, matchedClaim.ClaimValue, cancellationToken));
                }
                taskList.Add(_client.PutAsync($"claims/{user.Id}", userClaims, cancellationToken));

                await Task.WhenAll(taskList.ToArray());
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
            var userClaims = response.Data;
            if (userClaims != null)
            {
                foreach (var claim in claims)
                {
                    userClaims.RemoveAll(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
                }

                var taskList = new List<Task>(userClaims.Count + 1);
                taskList.Add(_client.PutAsync($"claims/{user.Id}", userClaims, cancellationToken));
                foreach (var uc in userClaims)
                {
                    taskList.Add(RemoveClaimIndex(user, uc.ClaimType, uc.ClaimValue, cancellationToken));
                }
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

            try
            {
                var response = await _client.GetAsync<TUser>(UsersTableName, cancellationToken, queryString: $"orderBy=\"NormalizedEmail\"&equalTo=\"{normalizedEmail}\"");
                return CreateUser(response.Data.Id, response);
            }
            catch (FirebaseException e)
                when (e.FirebaseError.Error.StartsWith("Index"))
            {
                var rules = await _client.GetAsync<FirebaseRules>(".settings/rules.json", cancellationToken);
                AddUserIndexes(rules.Data.Rules);
                await _client.PutAsync(".settings", rules.Data, cancellationToken);

                var response = await _client.GetAsync<TUser>(UsersTableName, cancellationToken, queryString: $"orderBy=\"NormalizedEmail\"&equalTo=\"{normalizedEmail}\"");
                return CreateUser(response.Data.Id, response);
            }
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

            var indexUrl = $"indexes/claims/{FirebaseLookupNormalizer.NormalizeKey(claim.Type)}/{FirebaseLookupNormalizer.NormalizeKey(claim.Value)}";
            var response = await _client.GetAsync<IEnumerable<string>>(indexUrl, cancellationToken);
            var userIds = response.Data;
            var users = new List<TUser>();
            if (userIds != null)
            {
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
        internal Task<TUserLogin> FindUserLoginInternalAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
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
        /// Get user tokens
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>User tokens.</returns>
        internal Task<List<TUserToken>> GetUserTokensInternalAsync(TUser user, CancellationToken cancellationToken)
        {
            return GetUserTokensAsync(user, cancellationToken);
        }

        /// <summary>
        /// Save user tokens.
        /// </summary>
        /// <param name="user">The tokens owner.</param>
        /// <param name="tokens">Tokens to save</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns></returns>
        internal Task SaveUserTokensInternalAsync(TUser user, IEnumerable<TUserToken> tokens, CancellationToken cancellationToken)
        {
            return SaveUserTokensAsync(user, tokens, cancellationToken);
        }
        
        /// <summary>
        /// Return a user with the matching userId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user if it exists.</returns>
        protected override Task<TUser> FindUserAsync(string userId, CancellationToken cancellationToken)
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
        protected override async Task<TUserLogin> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
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
            var response = await _client.GetAsync<string>($"indexes/provider-keys/{loginProvider}/{providerKey}", cancellationToken);
            var data = response.Data;
            if (data != null)
            {
                return await FindUserLoginAsync(data, loginProvider, providerKey, cancellationToken);
            }
            return null;
        }

        /// <summary>
        /// Get user tokens
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>User tokens.</returns>
        protected override async Task<List<TUserToken>> GetUserTokensAsync(TUser user, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<List<TUserToken>>($"users-tokens/{user.Id}", cancellationToken);
            return response.Data ?? new List<TUserToken>();
        }

        /// <summary>
        /// Save user tokens.
        /// </summary>
        /// <param name="user">The tokens owner.</param>
        /// <param name="tokens">Tokens to save</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns></returns>
        protected override async Task SaveUserTokensAsync(TUser user, IEnumerable<TUserToken> tokens, CancellationToken cancellationToken)
        {
            await _client.PutAsync($"users-tokens/{user.Id}", tokens, cancellationToken);
        }

        protected virtual void AddUserIndexes(Dictionary<string, object> rules)
        {
            rules.Add(UsersTableName, new UserIndex());
        }

        private TUser CreateUser(string userId, FirebaseResponse<TUser> response)
        {
            var user = response.Data;
            if (user != null)
            {
                user.Id = userId;
                SetConcurrencyStamp(user, response.Etag);
            }

            return user;
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

        private async Task IndexClaim(TUser user, string claimType, string claimValue, CancellationToken cancellationToken)
        {
            var indexUrl = $"indexes/claims/{FirebaseLookupNormalizer.NormalizeKey(claimType)}/{FirebaseLookupNormalizer.NormalizeKey(claimValue)}";
            var indexResponse = await _client.GetAsync<List<string>>(indexUrl, cancellationToken);
            var userList = indexResponse.Data ?? new List<string>(1);
            userList.Add(user.Id);
            await _client.PutAsync(indexUrl, userList, cancellationToken);
        }

        private async Task RemoveClaimIndex(TUser user, string claimType, string claimValue, CancellationToken cancellationToken)
        {
            var indexUrl = $"indexes/claims/{FirebaseLookupNormalizer.NormalizeKey(claimType)}/{FirebaseLookupNormalizer.NormalizeKey(claimValue)}";
            var indexResponse = await _client.GetAsync<List<string>>(indexUrl, cancellationToken);
            var userList = indexResponse.Data;
            if (userList != null)
            {
                userList.RemoveAll(u => u == user.Id);
                await _client.PutAsync(indexUrl, userList, cancellationToken);
            }
        }
    }
}
