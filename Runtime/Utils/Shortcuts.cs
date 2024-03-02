using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Accounts;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        /// <summary>
        ///   Several shortcuts to ease contract interaction. It includes the notion of
        ///   delegation which, given an account and the data, it computes:
        ///   1. stamp = seconds of (currentData - unixEpoch).
        ///   2. hash = keccak256(encodePacked(argsHash, stamp)).
        ///   3. signature = sign(account, hash).
        ///   4. result = encode(account.Address, stamp, hash, signature).
        ///   Validation involves checking the signature against the hash, the current stamp,
        ///   and the arguments hash, and also the validity of the stamp.
        /// </summary>
        public static class Shortcuts
        {
            // The keccak256 function in local.
            public static byte[] Keccak256(string value)
            {
                return Nethereum.Util.Sha3Keccack.Current.CalculateHash(value).HexToByteArray();
            }

            /// <summary>
            ///   Retrieves the account.
            /// </summary>
            /// <param name="client">The Web3 client</param>
            /// <returns>The associated account</returns>
            /// <exception cref="InvalidOperationException">The Web3 client is not authenticated</exception>
            public static IAccount GetAccount(Web3 client)
            {
                IAccount account = client.TransactionManager?.Account;
                if (account == null)
                {
                    throw new InvalidOperationException(
                        "This operations requires a web3 client with an account (i.e. an associated " +
                        "address and ability to sign with it)"
                    );
                }

                return account;
            }

            /// <summary>
            ///   Properly ABI-encodes (perhaps applying keccak256 and/or packing) the values.
            /// </summary>
            /// <param name="packed">Whether to apply packing (abi.encodePacked in Solidity)</param>
            /// <param name="keccak256">Whether to apply keccak256 (keccak256 in solidity)</param>
            /// <param name="values">The values to encode</param>
            /// <returns>The encoded values</returns>
            public static byte[] Encode(bool packed, bool keccak256, params object[] values)
            {
                ABIEncode encoder = new ABIEncode();
                if (packed)
                {
                    return keccak256 ? encoder.GetSha3ABIEncodedPacked(values) : encoder.GetABIEncodedPacked(values);
                }
                return keccak256 ? encoder.GetSha3ABIEncoded(values) : encoder.GetABIEncoded(values);
            }

            /// <summary>
            ///   Properly ABI-encodes (perhaps applying keccak256 and/or packing) the values.
            /// </summary>
            /// <param name="packed">Whether to apply packing (abi.encodePacked in Solidity)</param>
            /// <param name="keccak256">Whether to apply keccak256 (keccak256 in solidity)</param>
            /// <param name="values">The values to encode</param>
            /// <returns>The encoded values</returns>
            public static byte[] Encode(bool packed, bool keccak256, params ABIValue[] values)
            {
                ABIEncode encoder = new ABIEncode();
                if (packed)
                {
                    return keccak256 ? encoder.GetSha3ABIEncodedPacked(values) : encoder.GetABIEncodedPacked(values);
                }
                return keccak256 ? encoder.GetSha3ABIEncoded(values) : encoder.GetABIEncoded(values);
            }

            /// <summary>
            ///   Makes a delegation. This is done by signing and making all
            ///   the relevant data fit into the delegation.
            /// </summary>
            /// <param name="account">The account to use for signing</param>
            /// <param name="argsHash">The hash of the encodePacked-arguments to use</param>
            /// <returns>The delegation</returns>
            /// <exception cref="ArgumentException">The args hash has invalid length</exception>
            /// <exception cref="ArgumentNullException">The args hash is null</exception>
            public static async Task<byte[]> MakeDelegation(IAccount account, byte[] argsHash)
            {
                // First, calculate the current stamp.
                BigInteger stamp = (DateTime.UtcNow - DateTime.UnixEpoch).Seconds;
                
                // Second, the expected sender will be set from the account.
                string expectedSigner = account.Address;
                
                // Third, validate the arguments' hash's presence and length.
                if (argsHash == null)
                {
                    throw new ArgumentNullException(nameof(argsHash));
                }
                if (argsHash.Length != 32)
                {
                    throw new ArgumentException("The length of the args hash must be 32 bytes");
                }
                
                // Fourth, make the final hash.
                byte[] hash = Encode(
                    true, true,
                    new ABIValue("bytes32", argsHash),
                    new ABIValue("uint256", stamp)
                );
                
                // Fifth, compute the signature of that final hash.
                byte[] signature = await Sign(account, hash);

                // Finally, encode the delegation.
                return Encode(
                    false, false,
                    new ABIValue("address", expectedSigner),
                    new ABIValue("uint256", stamp),
                    new ABIValue("bytes32", hash),
                    new ABIValue("bytes", signature)
                );
            }

            /// <summary>
            ///   Makes a delegation. This is done by signing and making all
            ///   the relevant data fit into the delegation.
            /// </summary>
            /// <param name="account">The account to use for signing</param>
            /// <param name="args">The arguments to build the delegation with</param>
            /// <returns>The delegation</returns>
            public static Task<byte[]> MakeDelegation(IAccount account, params ABIValue[] args)
            {
                return MakeDelegation(account, Encode(true, true, args));
            }

            /// <summary>
            ///   Makes a delegation. This is done by signing and making all
            ///   the relevant data fit into the delegation.
            /// </summary>
            /// <param name="client">The web3 client to use for signing. Its account will be used</param>
            /// <param name="argsHash">The hash of the encodePacked-arguments to use</param>
            /// <returns>The delegation</returns>
            /// <exception cref="ArgumentException">The args hash has invalid length</exception>
            /// <exception cref="ArgumentNullException">The args hash is null</exception>
            public static Task<byte[]> MakeDelegation(Web3 client, byte[] argsHash)
            {
                return MakeDelegation(GetAccount(client), argsHash);
            }
            
            /// <summary>
            ///   Makes a delegation. This is done by signing and making all
            ///   the relevant data fit into the delegation.
            /// </summary>
            /// <param name="client">The web3 client to use for signing. Its account will be used</param>
            /// <param name="args">The arguments to build the delegation with</param>
            /// <returns>The delegation</returns>
            public static Task<byte[]> MakeDelegation(Web3 client, params ABIValue[] args)
            {
                return MakeDelegation(GetAccount(client), args);
            }

            /// <summary>
            ///   Signs a message using the PersonalSign protocol.
            /// </summary>
            /// <param name="account">The account to use for signing</param>
            /// <param name="message">The message to sign</param>
            /// <returns>The signature for the message</returns>
            public static async Task<byte[]> Sign(IAccount account, byte[] message)
            {
                var result = await account.AccountSigningService.PersonalSign.SendRequestAsync(message);
                return result.HexToByteArray();
            }
        }
    }
}