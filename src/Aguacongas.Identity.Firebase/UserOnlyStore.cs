using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Identity.Firebase
{
    /// <summary>
    /// Represents a new instance of a persistence store for <see cref="IdentityUser"/>.
    /// </summary>
    public class UserOnlyStore: UserOnlyStore<IdentityUser<string>>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="UserStore{TUser, TRole, TKey}"/>.
        /// </summary>
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public UserOnlyStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer) { }
    }

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
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
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
    [SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "Follow EF implementation")]
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
        private const string UserLoginsTableName = "user-logins";
        private const string UserClaimsTableName = "user-claims";
        private const string UserTokensTableName = "user-tokens";

        private readonly IFirebaseClient _client;

        /// <summary>
        /// A navigation property for the users the store contains.
        /// </summary>
        public override IQueryable<TUser> Users
        {
            get
            {
                var response = _client.GetAsync<Dictionary<string, object>>(GetFirebasePath(UsersTableName), queryString: "shallow=true").GetAwaiter().GetResult();
                var userDictionary = response.Data;
                if (userDictionary == null)
                {
                    return new List<TUser>(0).AsQueryable();
                }

                var taskList = new List<Task<TUser>>();
                foreach(var key in userDictionary.Keys)
                {
                    taskList.Add(FindByIdAsync(key));
                }

                Task.WaitAll(taskList.ToArray());
                var users = taskList.Where(t => t.Result != null)
                    .Select(t => t.Result)
                    .ToList();

                return users.AsQueryable();
            }
        }

        /// <summary>
        /// Creates a new instance of the store.
        /// </summary>
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/> used to describe store errors.</param>
        public UserOnlyStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(describer ?? new IdentityErrorDescriber())
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public async override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            var response = await _client.PostAsync(GetFirebasePath(UsersTableName), user, cancellationToken)
                .ConfigureAwait(false);

            user.Id = response.Data;
            user.ConcurrencyStamp = response.Etag;
            
            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public async override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            try
            {
                var response = await _client.PutAsync(GetFirebasePath(UsersTableName, user.Id), user, cancellationToken, true, user.ConcurrencyStamp)
                    .ConfigureAwait(false);
                user.ConcurrencyStamp = response.Etag;
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
        public async override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            try
            {
                await _client.DeleteAsync(GetFirebasePath(UsersTableName, user.Id), cancellationToken, true, user.ConcurrencyStamp)
                    .ConfigureAwait(false);
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
        public override async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var response = await _client.GetAsync<TUser>(GetFirebasePath(UsersTableName, userId), cancellationToken, true)
                .ConfigureAwait(false);

            var user = response.Data;
            if (user != null)
            {
                user.Id = userId;
                user.ConcurrencyStamp = response.Etag;
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
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUser>>(GetFirebasePath(UsersTableName), cancellationToken, false, $"orderBy=\"NormalizedUserName\"&equalTo=\"{normalizedUserName}\"")
                    .ConfigureAwait(false);

                var data = response.Data;
                if (data.Any())
                {
                    return await FindByIdAsync(data.First().Key, cancellationToken)
                        .ConfigureAwait(false);
                }
                return null;
                
            }
            catch(FirebaseException e)
               when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UsersTableName, new UserIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUser>>(GetFirebasePath(UsersTableName), cancellationToken, false, $"orderBy=\"NormalizedUserName\"&equalTo=\"{normalizedUserName}\"")
                    .ConfigureAwait(false);
                var data = response.Data;
                if (data.Any())
                {
                    return await FindByIdAsync(data.First().Key, cancellationToken)
                        .ConfigureAwait(false);
                }
                return null;
            }
        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public async override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            Dictionary<string, TUserClaim> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, false, $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserClaimsTableName, new UserClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, queryString: $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);

                data = response.Data;
            }
            
            if (data != null)
            {
                return data.Select(c => c.Value.ToClaim()).ToList();
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
        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claims, nameof(claims));

            List<TUserClaim> userClaims = claims.Select(c => CreateUserClaim(user, c)).ToList();
            var taskList = new List<Task>(userClaims.Count);
            foreach(var userClaim in userClaims)
            {
                taskList.Add(_client.PostAsync(GetFirebasePath(UserClaimsTableName), userClaim, cancellationToken));
            }

            await Task.WhenAll(taskList.ToArray())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claim, nameof(claim));
            AssertNotNull(newClaim, nameof(newClaim));

            Dictionary<string, TUserClaim> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, false, $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserClaimsTableName, new UserClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, queryString: $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);

                data = response.Data;
            }

            data = data ?? new Dictionary<string, TUserClaim>();
            foreach(var kv in data)
            {
                var uc = kv.Value;
                if (uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value)
                {
                    uc.ClaimType = newClaim.Type;
                    uc.ClaimValue = newClaim.Value;

                    await _client.PutAsync(GetFirebasePath(UserClaimsTableName, kv.Key), uc, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async override Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(claims, nameof(claims));

            Dictionary<string, TUserClaim> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, false, $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);

                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserClaimsTableName, new UserClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, queryString: $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);

                data = response.Data;
            }

            if (data != null)
            {
                var taskList = new List<Task>(claims.Count());
                foreach (var claim in claims)
                {
                    var match = data.SingleOrDefault(kv => kv.Value.ClaimType == claim.Type && kv.Value.ClaimValue == claim.Value);
                    if (match.Key != null)
                    {
                        taskList.Add(_client.DeleteAsync(GetFirebasePath(UserClaimsTableName, match.Key), cancellationToken));
                    }
                }

                await Task.WhenAll(taskList.ToArray())
                    .ConfigureAwait(false);
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
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));
            AssertNotNull(login, nameof(login));

            await _client.PostAsync(GetFirebasePath(UserLoginsTableName), CreateUserLogin(user, login), cancellationToken)
                .ConfigureAwait(false);
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
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            var data = await GetUserLoginsAsync(user.Id, cancellationToken)
                .ConfigureAwait(false);

            foreach (var kv in data)
            {
                var login = kv.Value;
                if (login.LoginProvider == loginProvider && login.ProviderKey == providerKey)
                {
                    await _client.DeleteAsync(GetFirebasePath(UserLoginsTableName, kv.Key), cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public async override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(user, nameof(user));

            Dictionary<string, TUserLogin> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, false, $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserLoginsTableName, new LoginIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, queryString: $"orderBy=\"UserId\"&equalTo=\"{user.Id}\"")
                    .ConfigureAwait(false);

                data = response.Data;
            }

            if (data != null)
            {
                return data.Values
                    .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName))
                    .ToList();
            }
            return new List<UserLoginInfo>(0);
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
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken)
                .ConfigureAwait(false);
            if (userLogin != null)
            {
                return await FindUserAsync(userLogin.UserId, cancellationToken)
                    .ConfigureAwait(false);
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
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUser>>(GetFirebasePath(UsersTableName), cancellationToken, false, $"orderBy=\"NormalizedEmail\"&equalTo=\"{normalizedEmail}\"")
                    .ConfigureAwait(false);

                var data = response.Data;
                if (data.Any())
                {
                    return await FindByIdAsync(data.First().Key, cancellationToken)
                        .ConfigureAwait(false);
                }

                return null;

            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UsersTableName, new UserIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUser>>(GetFirebasePath(UsersTableName), cancellationToken, false, $"orderBy=\"NormalizedEmail\"&equalTo=\"{normalizedEmail}\"")
                    .ConfigureAwait(false);
                var data = response.Data;
                if (data.Any())
                {
                    return await FindByIdAsync(data.First().Key, cancellationToken)
                        .ConfigureAwait(false);
                }
                return null;
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
        public async override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(claim, nameof(claim));

            Dictionary<string, TUserClaim> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, false, $"orderBy=\"ClaimType\"&equalTo=\"{claim.Type}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserClaimsTableName, new UserClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserClaim>>(GetFirebasePath(UserClaimsTableName), cancellationToken, queryString: $"orderBy=\"ClaimType\"&equalTo=\"{claim.Type}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }

            if (data == null)
            {
                return new List<TUser>(0);
            }

            var userIds = data.Values.Where(c => c.ClaimValue == claim.Value).Select(c => c.UserId);
            var users = new ConcurrentBag<TUser>();
            var taskList = new List<Task>(userIds.Count());
            foreach (var userId in userIds)
            {
                taskList.Add(Task.Run(async () => {
                    var user = await FindByIdAsync(userId, cancellationToken)
                        .ConfigureAwait(false);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }));
            }

            await Task.WhenAll(taskList.ToArray())
                .ConfigureAwait(false);

            return users.ToList();
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
            var data = await GetUserLoginsAsync(userId, cancellationToken)
                .ConfigureAwait(false);
            if (data != null)
            {
                return data.Values.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
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
            Dictionary<string, TUserLogin> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, false, $"orderBy=\"ProviderKey\"&equalTo=\"{providerKey}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserLoginsTableName, new LoginIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, queryString: $"orderBy=\"ProviderKey\"&equalTo=\"{providerKey}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }

            if (data != null)
            {
                return data.Values.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
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
            var response = await _client.GetAsync<List<TUserToken>>(GetFirebasePath(UserTokensTableName, user.Id), cancellationToken)
                .ConfigureAwait(false);
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
            await _client.PutAsync(GetFirebasePath(UserTokensTableName, user.Id), tokens, cancellationToken)
                .ConfigureAwait(false);
        }

        protected virtual async Task<Dictionary<string, TUserLogin>> GetUserLoginsAsync(string userId, CancellationToken cancellationToken)
        {
            Dictionary<string, TUserLogin> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, false, $"orderBy=\"UserId\"&equalTo=\"{userId}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(UserLoginsTableName, new LoginIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TUserLogin>>(GetFirebasePath(UserLoginsTableName), cancellationToken, queryString: $"orderBy=\"UserId\"&equalTo=\"{userId}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }

            return data;
        }


        protected virtual void SetIndex(Dictionary<string, object> rules, string key, object index)
        {
            rules[key] = index;
        }

        internal async Task SetIndex(string onTable, object index, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<FirebaseRules>(RulePath, cancellationToken)
                .ConfigureAwait(false);
            var rules = response.Data ?? new FirebaseRules();
            SetIndex(rules.Rules, onTable, index);
            await _client.PutAsync(RulePath, rules, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
