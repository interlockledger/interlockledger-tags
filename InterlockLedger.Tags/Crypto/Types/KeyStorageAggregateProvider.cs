// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;
public sealed class KeyStorageAggregateProvider : IKeyStorageProvider, IKeyPhasedCreationProvider
{
    public KeyStorageAggregateProvider(IEnumerable<IKeyStorageImplementationProvider> providers) {
        if (providers.None())
            throw new ArgumentException("Must supply at least one KeyStorageProvider!");
        foreach (var provider in providers)
            AddProvider(provider);
    }

    public string AppKeysFolderPath => _providers.Values.Select(ksp => ksp.AppKeysFolderPath).JoinedBy(":");
    public IEnumerable<InterlockSigningKeyData> Keys => _providers.Values.SelectMany(ksp => ksp.Keys);

    public IEnumerable<Algorithm> SupportedAlgorithms => _providers.Values.SelectMany(ksp => ksp.SupportedAlgorithms).Distinct().Order().ToArray();

    public void AddProvider(IKeyStorageImplementationProvider provider) {
        provider.Required();
        _providers.Add(provider.Name, provider);
    }

    public InterlockSigningKeyData Create(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, Algorithm algorithm, KeyStrength strength, string name, string description, string password)
        => FindProviderFor(algorithm, ksp => ksp.SupportsKeyCreation, "and key creation").Create(purposes, permissions, algorithm, strength, name, description, password);

    public X509Certificate2 CreateCertificate(string name, string password, string file, Algorithm algorithm, KeyStrength strength)
        => FindProviderFor(algorithm, ksp => ksp.SupportsCertificateImport, "certificate creation").CreateCertificate(name, password, file, algorithm, strength);

    IKeyParameters IKeyPhasedCreationProvider.CreateKeyParameters(Algorithm algorithm, KeyStrength strength) => FindPhasedProvider(algorithm).CreateKeyParameters(algorithm, strength);

    InterlockSigningKeyData IKeyPhasedCreationProvider.CreateUsing(IKeyParameters emergencyKeyParameters, KeyStrength strength, BaseKeyId identity, string name, string description, string password, IEnumerable<AppPermissions> permissions, params KeyPurpose[] purposes)
        => FindPhasedProvider(emergencyKeyParameters.PublicKey.Algorithm).CreateUsing(emergencyKeyParameters, strength, identity, name, description, password, permissions, purposes);

    public ExportedKeyFile? ExportKeyFile(string name) {
        foreach (var provider in _providers.Values) if (provider.Keys.Any(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return provider.ExportKeyFile(name);
        return null;
    }

    public InterlockSigningKeyData Import(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, byte[] certificateBytes, string password)
        => FindProviderFor(ksp => ksp.SupportsCertificateImport, "certificate import").Import(purposes, permissions, certificateBytes, password);

    public InterlockSigningKey Open(InterlockSigningKeyData key, string password)
        => key is null
            ? throw new ArgumentNullException(nameof(key))
            : FindProviderFor(key.PublicKey.Algorithm, _ => true).Open(key, password);

    public InterlockSigningKey? Resolve(string name, string password) {
        foreach (var provider in _providers.Values) if (provider.Keys.Any(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return provider.Resolve(name, password);
        return null;
    }

    private readonly Dictionary<string, IKeyStorageImplementationProvider> _providers = new();

    private IKeyPhasedCreationProvider FindPhasedProvider(Algorithm algorithm)
        => (IKeyPhasedCreationProvider)FindProviderFor(algorithm, ksp => ksp.SupportsKeyCreation && ksp is IKeyPhasedCreationProvider, "and phased key creation");

    private IKeyStorageImplementationProvider FindProviderFor(Algorithm algorithm, Func<IKeyStorageImplementationProvider, bool> filter, string? messageComplement = null)
        => FindProviderFor(ksp => ksp.SupportsAlgorithm(algorithm) && filter(ksp), $"the algorithm {algorithm} {messageComplement}");

    private IKeyStorageImplementationProvider FindProviderFor(Func<IKeyStorageImplementationProvider, bool> filter, string? messageComplement = null)
        => _providers.Values.FirstOrDefault(filter.Required()) ?? throw new InvalidOperationException($"No key storage provider found that supports {messageComplement}");
}