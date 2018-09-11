using System;
using System.IO;

namespace SmallArgon2d
{
    /// <summary>
    /// An implementation of Blake2b HMAC per RFC-7693
    /// </summary>
    public class HMACBlake2B
    {
        public string HashName { get; set; }

        /// <summary>
        /// Construct an HMACBlake2B without a key
        /// </summary>
        /// <param name="hashSize">The hash size in bits</param>
        public HMACBlake2B(int hashSize)
        {
            HashName = "Konscious.Security.Cryptography.HMACBlake2B";

            if ((hashSize % 8) > 0)
            {
                throw new ArgumentException("Hash Size must be byte aligned", nameof(hashSize));
            }

            if (hashSize < 8 || hashSize > 512)
            {
                throw new ArgumentException("Hash Size must be between 8 and 512", nameof(hashSize));
            }

            HashSize = hashSize;
            CreateImpl = CreateImplementation;
            Key = new byte[0];
        }

        /// <summary>
        /// Construct an HMACBlake2B
        /// </summary>
        /// <param name="keyData">The key for the HMAC</param>
        /// <param name="hashSize">The hash size in bits</param>
        public HMACBlake2B(byte[] keyData, int hashSize)
            : this(hashSize)
        {
            if (keyData == null)
            {
                keyData = new byte[0];
            }

            if (keyData.Length > 64)
            {
                throw new ArgumentException("Key needs to be between 0 and 64 bytes", nameof(keyData));
            }

            Key = keyData;
        }

        internal HMACBlake2B(byte[] keyData, int hashSize, Func<Blake2bBase> baseCreator)
            : this(keyData, hashSize)
        {
            CreateImpl = baseCreator;
        }

        /// <summary>
        /// Implementation of HashSize <seealso cref="System.Security.Cryptography.HashAlgorithm"/>
        /// </summary>
        /// <returns>The hash</returns>
        public int HashSize { get; private set; }

        /// <summary>
        /// Overridden key to enforce size
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Implementation of Initialize - initializes the HMAC buffer
        /// </summary>
        public void Initialize()
        {
            Implementation = CreateImplementation();
            Implementation.Initialize(Key);
        }

        /// <summary>
        /// Implementation of HashCore
        /// </summary>
        /// <param name="data">The data to hash</param>
        /// <param name="offset">The offset to start hashing from</param>
        /// <param name="size">The amount of data in the hash to consume</param>
        protected void HashCore(byte[] data, int offset, int size)
        {
            if (Implementation == null)
                Initialize();

            Implementation.Update(data, offset, size);
        }

        /// <summary>
        /// Finish hashing and return the final hash
        /// </summary>
        /// <returns>The final hash from HashCore</returns>
        protected byte[] HashFinal()
        {
            return Implementation.Final();
        }

        private Blake2bBase CreateImplementation()
        {
            return new Blake2bNormal(HashSize / 8);
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            byte[] Data = new byte[0x1000];
            int DataLength = inputStream.Read(Data, 0, Data.Length);
            HashCore(Data, 0, DataLength);

            byte[] Result = HashFinal();
            Initialize();

            return Result;
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            HashCore(buffer, 0, buffer.Length);

            byte[] Result = HashFinal();
            Initialize();

            return Result;
        }

        private Blake2bBase Implementation;
        private Func<Blake2bBase> CreateImpl;
    }
}
