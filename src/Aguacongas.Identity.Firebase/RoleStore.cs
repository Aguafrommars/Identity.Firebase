using Aguacongas.Firebase;
using Microsoft.AspNetCore.Identity;
using System;
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
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TRole> : RoleStore<TRole, IdentityUserRole<string>, IdentityRoleClaim<string>>
        where TRole : IdentityRole<string>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole}"/>.
        /// </summary>
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(IFirebaseClient client, IdentityErrorDescriber describer = null) : base(client, describer) { }
    }


    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type of the class representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    [SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Follow EF implementation")]
    [SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "Follow EF implementation")]
    [SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Follow EF implementation")]
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Follow EF implementation")]
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Follow EF implementation")]
    public class RoleStore<TRole, TUserRole, TRoleClaim> :
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : IdentityRole<string>
        where TUserRole : IdentityUserRole<string>, new()
        where TRoleClaim : IdentityRoleClaim<string>, new()
    {
        private const string RolesTableName = "roles";
        private const string RoleClaimsTableName = "role-claims";

        protected const string RulePath = ".settings/rules.json";

        private readonly IFirebaseClient _client;
        private bool _disposed;

        /// <summary>
        /// A navigation property for the roles the store contains.
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get
            {
                var response = _client.GetAsync<Dictionary<string, object>>(GetFirebasePath(RolesTableName), queryString: "shallow=true").GetAwaiter().GetResult();
                var roleDictionary = response.Data;
                if (roleDictionary == null)
                {
                    return new List<TRole>().AsQueryable();
                }

                var taskList = new List<Task<TRole>>();
                foreach (var key in roleDictionary.Keys)
                {
                    taskList.Add(FindByIdAsync(key));
                }

                var results = Task.WhenAll(taskList).GetAwaiter().GetResult();

                return results.Where(r => r != null).AsQueryable();
            }
        }


        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="client">The <see cref="IFirebaseClient"/>.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(IFirebaseClient client, IdentityErrorDescriber describer = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            var response = await _client.PostAsync(GetFirebasePath(RolesTableName), role, cancellationToken, true)
                .ConfigureAwait(false);

            role.Id = response.Data;
            role.ConcurrencyStamp = response.Etag;

            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            try
            {
                var response = await _client.PutAsync(GetFirebasePath(RolesTableName, role.Id), role, cancellationToken, true, role.ConcurrencyStamp)
                    .ConfigureAwait(false);
                role.ConcurrencyStamp = response.Etag;
            }
            catch (FirebaseException e)
                when(e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }


            return IdentityResult.Success;
        }


        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public async virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            try
            {
                await _client.DeleteAsync(GetFirebasePath(RolesTableName, role.Id), cancellationToken, true, role.ConcurrencyStamp)
                    .ConfigureAwait(false);
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
        /// Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.Id);
        }

        /// <summary>
        /// Gets the name of a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.Name);
        }

        /// <summary>
        /// Sets the name of a role in the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var response = await _client.GetAsync<TRole>(GetFirebasePath(RolesTableName, roleId), cancellationToken, true)
                .ConfigureAwait(false);

            var role = response.Data;
            if (role != null)
            {
                role.Id = roleId;
                role.ConcurrencyStamp = response.Etag;
            }

            return role;
        }

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            Dictionary<string, TRole> data;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TRole>>(GetFirebasePath(RolesTableName), cancellationToken, false, $"orderBy=\"NormalizedName\"&equalTo=\"{normalizedRoleName}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }
            catch (FirebaseException e)
               when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(RolesTableName, new RoleIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TRole>>(GetFirebasePath(RolesTableName), cancellationToken, false, $"orderBy=\"NormalizedName\"&equalTo=\"{normalizedRoleName}\"")
                    .ConfigureAwait(false);
                data = response.Data;
            }

            if (data != null && data.Any())
            {
                return await FindByIdAsync(data.First().Key, cancellationToken)
                    .ConfigureAwait(false);
            }
            return null;

        }

        /// <summary>
        /// Get a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        /// <summary>
        /// Set a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be set.</param>
        /// <param name="normalizedName">The normalized name to set</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }


        /// <summary>
        /// Dispose the stores
        /// </summary>
        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Follow Ef implementation")]
        public void Dispose() => _disposed = true;

        /// <summary>
        /// Get the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public async virtual Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            try
            {
                var response = await _client.GetAsync<Dictionary<string, TRoleClaim>>(GetFirebasePath(RoleClaimsTableName), cancellationToken, false, $"orderBy=\"RoleId\"&equalTo=\"{role.Id}\"")
                    .ConfigureAwait(false);
                return response.Data.Values.Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();
            }
            catch (FirebaseException e)
               when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(RoleClaimsTableName, new RoleClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TRoleClaim>>(GetFirebasePath(RoleClaimsTableName), cancellationToken, false, $"orderBy=\"RoleId\"&equalTo=\"{role.Id}\"")
                    .ConfigureAwait(false);

                return response.Data.Values.Select(rc => new Claim(rc.ClaimType, rc.ClaimValue)).ToList();
            }
        }

        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));
            AssertNotNull(claim, nameof(claim));

            await _client.PostAsync(GetFirebasePath(RoleClaimsTableName), CreateRoleClaim(role, claim), cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async virtual Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));
            AssertNotNull(claim, nameof(claim));

            Dictionary<string, TRoleClaim> claims;
            try
            {
                var response = await _client.GetAsync<Dictionary<string, TRoleClaim>>(GetFirebasePath(RoleClaimsTableName), cancellationToken, false, $"orderBy=\"RoleId\"&equalTo=\"{role.Id}\"")
                    .ConfigureAwait(false);
                claims = response.Data;
            }
            catch (FirebaseException e)
                when (e.FirebaseError != null && e.FirebaseError.Error.StartsWith("Index"))
            {
                await SetIndex(RoleClaimsTableName, new RoleClaimIndex(), cancellationToken)
                    .ConfigureAwait(false);

                var response = await _client.GetAsync<Dictionary<string, TRoleClaim>>(GetFirebasePath(RoleClaimsTableName), cancellationToken, false, $"orderBy=\"RoleId\"&equalTo=\"{role.Id}\"")
                    .ConfigureAwait(false);
                claims = response.Data;
            }

            if (claims != null)
            {
                foreach(var kv in claims)
                {
                    var roleClaim = kv.Value;
                    if (roleClaim.ClaimType == claim.Type && roleClaim.ClaimValue == claim.Value)
                    {
                        await _client.DeleteAsync(GetFirebasePath(RoleClaimsTableName, kv.Key), cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new TRoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };

        protected virtual string GetFirebasePath(params string[] objectPath)
        {
            return string.Join("/", objectPath);
        }

        protected virtual void SetIndex(Dictionary<string, object> rules, string key, object index)
        {
            rules[key] = index;
        }

        private async Task SetIndex(string onTable, object index, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync<FirebaseRules>(RulePath, cancellationToken)
                .ConfigureAwait(false);
            var rules = response.Data ?? new FirebaseRules();
            SetIndex(rules.Rules, onTable, index);
            await _client.PutAsync(RulePath, rules, cancellationToken);
        }

        private void AssertNotNull(object p, string pName)
        {
            if (p == null)
            {
                throw new ArgumentNullException(pName);
            }
        }
    }
}
