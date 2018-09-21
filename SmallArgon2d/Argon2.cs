using System;

namespace SmallArgon2d
{
    /// <summary>
    /// An implementation of Argon2 https://github.com/P-H-C/phc-winner-argon2
    /// </summary>
    public abstract class Argon2 : IDisposable
    {
        /// <summary>
        /// Create an Argon2 for encrypting the given password
        /// </summary>
        /// <param name="password"></param>
        public Argon2(byte[] password)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentException("Argon2 needs a password set", nameof(password));

            Password = password;
        }

        /// <summary>
        /// The version number
        /// </summary>
        public int Version { get { return 1; } }

        /// <summary>
        /// The password hashing salt
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// An optional secret to use while hashing the Password
        /// </summary>
        public byte[] KnownSecret { get; set; }

        /// <summary>
        /// Any extra associated data to use while hashing the password
        /// </summary>
        public int AssociatedUse { get; set; }

        /// <summary>
        /// The number of iterations to apply to the password hash
        /// </summary>
        public int Iterations { get; set; } = 3;

        /// <summary>
        /// The number of 1kB memory blocks to use while processing the hash
        /// </summary>
        public int MemorySize { get; set; } = 4096;

        /// <summary>
        /// Implementation of GetBytes
        /// </summary>
        public byte[] GetBytes(int bc)
        {
            ValidateParameters(bc);
            return GetBytesAsyncImpl(bc);
        }

        /// <summary>
        /// Implementation of GetSettings
        /// </summary>
        public string GetSettings()
        {
            return $"PHS={GetType().Name};Version={Version};Iterations={Iterations};MemorySize={MemorySize};AssociatedUse={AssociatedUse}";
        }

        /// <summary>
        /// Implementation of GetEncoded
        /// </summary>
        public string GetEncoded(string hash)
        {
            return $"{GetSettings()};Hash={hash}";
        }

        internal abstract Argon2Core BuildCore(int bc);

        private void ValidateParameters(int bc)
        {
            if (bc > 1024)
                throw new NotSupportedException("Current implementation of Argon2 only supports generating up to 1024 bytes");

            if (Iterations < 1)
                throw new InvalidOperationException("Cannot perform an Argon2 Hash with out at least 1 iteration");

            if (MemorySize < 4)
                throw new InvalidOperationException("Argon2 requires a minimum of 4kB of memory (MemorySize >= 4)");
        }

        private byte[] GetBytesAsyncImpl(int bc)
        {
            Argon2Core n = BuildCore(bc);
            n.Salt = Salt;
            n.Secret = KnownSecret;
            n.AssociatedData = BitConverter.GetBytes(AssociatedUse);
            n.Iterations = Iterations;
            n.MemorySize = MemorySize;

            return n.Hash(Password);
        }

        private byte[] Password;

        public void Dispose()
        {
            Array.Clear(Password, 0, Password.Length);
            Password = null;
        }
    }
}
