﻿// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.Identity.Firestore
{
    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TRole> : RoleStore<TRole, IdentityUserRole<string>, IdentityRoleClaim<string>>
        where TRole : IdentityRole<string>, new()
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole}"/>.
        /// </summary>
        /// <param name="db">The <see cref="FirestoreDb"/>.</param>
        /// <param name="tableNamesConfig"><see cref="FirestoreTableNamesConfig"/></param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(FirestoreDb db, FirestoreTableNamesConfig tableNamesConfig,  IdentityErrorDescriber describer = null) : base(db, tableNamesConfig, describer) { }
    }

    /// <summary>
    /// Creates a new instance of a persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type of the class representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Follow EF implementation")]
    [SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Follow EF implementation")]
    [SuppressMessage("Major Code Smell", "S2436:Types and methods should not have too many generic parameters", Justification = "Follow EF implementation")]
    [SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Follow EF implementation")]
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose")]
    public class RoleStore<TRole, TUserRole, TRoleClaim> :
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : IdentityRole<string>, new()
        where TUserRole : IdentityUserRole<string>, new()
        where TRoleClaim : IdentityRoleClaim<string>, new()
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _roles;
        private readonly CollectionReference _roleClaims;
        private bool _disposed;

        /// <summary>
        /// A navigation property for the roles the store contains.
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get
            {
                var documents = _roles.GetSnapshotAsync().GetAwaiter().GetResult();
                return documents.Select(d => Map.FromDictionary<TRole>(d.ToDictionary())).AsQueryable();

            }
        }


        /// <summary>
        /// Constructs a new instance of <see cref="RoleStore{TRole, TUserRole, TRoleClaim}"/>.
        /// </summary>
        /// <param name="db">The <see cref="FirestoreDb"/>.</param>
        /// <param name="tableNamesConfig"><see cref="FirestoreTableNamesConfig"/></param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber"/>.</param>
        public RoleStore(FirestoreDb db, FirestoreTableNamesConfig tableNamesConfig, IdentityErrorDescriber describer = null)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            tableNamesConfig = tableNamesConfig ?? throw new ArgumentNullException(nameof(tableNamesConfig));
            _roles = _db.Collection(tableNamesConfig.RolesTableName);
            _roleClaims = _db.Collection(tableNamesConfig.RoleClaimsTableName);
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

            var dictionary = Map.ToDictionary(role);
            await _roles.Document(role.Id).SetAsync(dictionary, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return IdentityResult.Success;

        }

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            var dictionary = Map.ToDictionary(role);
            return _db.RunTransactionAsync(async transaction =>
            {
                var roleRef = _roles.Document(role.Id);
                var snapShot = await transaction.GetSnapshotAsync(roleRef, cancellationToken)
                    .ConfigureAwait(false);
                if (snapShot.GetValue<string>("ConcurrencyStamp") != role.ConcurrencyStamp)
                {
                    return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
                }
                transaction.Update(roleRef, dictionary);
                return IdentityResult.Success;
            }, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
        public virtual Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));

            return _db.RunTransactionAsync(async transaction =>
            {
                var userRef = _roles.Document(role.Id);
                var snapShot = await transaction.GetSnapshotAsync(userRef, cancellationToken)
                    .ConfigureAwait(false);
                if (snapShot.GetValue<string>("ConcurrencyStamp") != role   .ConcurrencyStamp)
                {
                    return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
                }
                transaction.Delete(userRef);
                return IdentityResult.Success;
            }, cancellationToken: cancellationToken);

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
            AssertNotNullOrEmpty(roleId, nameof(roleId));

            var snapShot = await _roles.Document(roleId).GetSnapshotAsync(cancellationToken)
                .ConfigureAwait(false);
            if (snapShot != null)
            {
                return Map.FromDictionary<TRole>(snapShot.ToDictionary());
            }

            return null;
        }

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
        public virtual async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNullOrEmpty(normalizedRoleName, nameof(normalizedRoleName));

            var snapShot = await _roles.WhereEqualTo("NormalizedName", normalizedRoleName)
                .GetSnapshotAsync(cancellationToken)
                .ConfigureAwait(false);
            var document = snapShot.Documents
                .FirstOrDefault();
            if (document != null)
            {
                return Map.FromDictionary<TRole>(document.ToDictionary());
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
        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose")]
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

            var snapShot = await _roleClaims.WhereEqualTo("RoleId", role.Id)
                .GetSnapshotAsync(cancellationToken)
                .ConfigureAwait(false);
            return snapShot.Documents
                .Select(d => new Claim(d.GetValue<string>("Type"), d.GetValue<string>("Value")))
                .ToList();

        }

        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            AssertNotNull(role, nameof(role));
            AssertNotNull(claim, nameof(claim));
            

            Dictionary<string, object> dictionary = ClaimsToDictionary(role, claim);
            return _roleClaims.AddAsync(dictionary, cancellationToken: cancellationToken);

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

            var snapShot = await _roleClaims.WhereEqualTo("RoleId", role.Id)
                .WhereEqualTo("Type", claim.Type)
                .GetSnapshotAsync(cancellationToken)
                .ConfigureAwait(false);
            var document = snapShot.Documents.FirstOrDefault();
            if (document != null)
            {
                await _roleClaims.Document(document.Id).DeleteAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
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

        protected virtual Dictionary<string, object> ClaimsToDictionary(TRole role, Claim claim)
        {
            return new Dictionary<string, object>
                {
                    { "RoleId",  role.Id },
                    { "Type", claim.Type },
                    { "Value", claim.Value }
                };
        }

        private static void AssertNotNull(object p, string pName)
        {
            if (p == null)
            {
                throw new ArgumentNullException(pName);
            }
        }

        private static void AssertNotNullOrEmpty(string p, string pName)
        {
            if (string.IsNullOrWhiteSpace(p))
            {
                throw new ArgumentNullException(pName);
            }
        }
    }
}
