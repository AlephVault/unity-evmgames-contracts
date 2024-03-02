using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Models
        {
            /// <summary>
            ///   This utility properly maps the types between the
            ///   ABI and the C# interface.
            /// </summary>
            public static class ABITypes
            {
                /**
                 * An explanation of this all:
                 *
                 * 1. The ABI has its own types. We'll be considering 101
                 *    types, being 32 byteX types, 32 uintX types, and
                 *    32 intX types. The other types are: bytes, string,
                 *    bool and address. Finally, the compound "tuple"
                 *    type. There are also variants of these types: one
                 *    being a dynamic array, and one being a static array,
                 *    having a fixed length.
                 *
                 * 2. We'll associate a marshalling C# type(s). As an example,
                 *    consider that byte[] will be used for bytes and byteX
                 *    types, while byte[][] will be used for bytes[] and byteX[].
                 */
                
                // These are the mapped types.
                private static Dictionary<string, string> abiToCSharp = new Dictionary<string, string>()
                {
                    { "int8", "sbyte" },
                    { "int16", "short" },
                    { "int24", "int" },
                    { "int32", "int" },
                    { "int40", "long" },
                    { "int48", "long" },
                    { "int56", "long" },
                    { "int64", "long" },
                    { "int72", "BigInteger" },
                    { "int80", "BigInteger" },
                    { "int88", "BigInteger" },
                    { "int96", "BigInteger" },
                    { "int104", "BigInteger" },
                    { "int112", "BigInteger" },
                    { "int120", "BigInteger" },
                    { "int128", "BigInteger" },
                    { "int136", "BigInteger" },
                    { "int144", "BigInteger" },
                    { "int152", "BigInteger" },
                    { "int160", "BigInteger" },
                    { "int168", "BigInteger" },
                    { "int176", "BigInteger" },
                    { "int184", "BigInteger" },
                    { "int192", "BigInteger" },
                    { "int200", "BigInteger" },
                    { "int208", "BigInteger" },
                    { "int216", "BigInteger" },
                    { "int224", "BigInteger" },
                    { "int232", "BigInteger" },
                    { "int240", "BigInteger" },
                    { "int248", "BigInteger" },
                    { "int256", "BigInteger" },
                    { "int", "BigInteger" }, // Alias of int256
                    { "uint8", "byte" },
                    { "uint16", "ushort" },
                    { "uint24", "uint" },
                    { "uint32", "uint" },
                    { "uint40", "ulong" },
                    { "uint48", "ulong" },
                    { "uint56", "ulong" },
                    { "uint64", "ulong" },
                    { "uint72", "BigInteger" },
                    { "uint80", "BigInteger" },
                    { "uint88", "BigInteger" },
                    { "uint96", "BigInteger" },
                    { "uint104", "BigInteger" },
                    { "uint112", "BigInteger" },
                    { "uint120", "BigInteger" },
                    { "uint128", "BigInteger" },
                    { "uint136", "BigInteger" },
                    { "uint144", "BigInteger" },
                    { "uint152", "BigInteger" },
                    { "uint160", "BigInteger" },
                    { "uint168", "BigInteger" },
                    { "uint176", "BigInteger" },
                    { "uint184", "BigInteger" },
                    { "uint192", "BigInteger" },
                    { "uint200", "BigInteger" },
                    { "uint208", "BigInteger" },
                    { "uint216", "BigInteger" },
                    { "uint224", "BigInteger" },
                    { "uint232", "BigInteger" },
                    { "uint240", "BigInteger" },
                    { "uint248", "BigInteger" },
                    { "uint256", "BigInteger" },
                    { "uint", "BigInteger" }, // Alias of uint256
                    { "bytes1", "byte[]" },
                    { "bytes2", "byte[]" },
                    { "bytes3", "byte[]" },
                    { "bytes4", "byte[]" },
                    { "bytes5", "byte[]" },
                    { "bytes6", "byte[]" },
                    { "bytes7", "byte[]" },
                    { "bytes8", "byte[]" },
                    { "bytes9", "byte[]" },
                    { "bytes10", "byte[]" },
                    { "bytes11", "byte[]" },
                    { "bytes12", "byte[]" },
                    { "bytes13", "byte[]" },
                    { "bytes14", "byte[]" },
                    { "bytes15", "byte[]" },
                    { "bytes16", "byte[]" },
                    { "bytes17", "byte[]" },
                    { "bytes18", "byte[]" },
                    { "bytes19", "byte[]" },
                    { "bytes20", "byte[]" },
                    { "bytes21", "byte[]" },
                    { "bytes22", "byte[]" },
                    { "bytes23", "byte[]" },
                    { "bytes24", "byte[]" },
                    { "bytes25", "byte[]" },
                    { "bytes26", "byte[]" },
                    { "bytes27", "byte[]" },
                    { "bytes28", "byte[]" },
                    { "bytes29", "byte[]" },
                    { "bytes30", "byte[]" },
                    { "bytes31", "byte[]" },
                    { "bytes32", "byte[]" },
                    { "bytes", "byte[]" },
                    { "string", "string" },
                    { "bool", "bool" },
                    { "address", "string" },
                    // The actual value of "tuple" will not be used.
                    // This, because it is a composite type, and they
                    // are mapped differently.
                    { "tuple", "tuple" }
                };

                /// <summary>
                ///   Gets the corresponding C# type from the ABI base type.
                /// </summary>
                /// <param name="abiBaseType">The ABI base type</param>
                /// <returns>The corresponding C# type</returns>
                /// <exception cref="ArgumentException"></exception>
                public static string GetCorrespondingCSharpBaseType(string abiBaseType)
                {
                    try
                    {
                        return abiToCSharp[abiBaseType];
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException(
                            $"Invalid or unsupported base ABI type: {abiBaseType}. Only string, bytes, " +
                            $"address, int8 to int256, uint8 to uint256, bytes1 to bytes32, bool and tuple " +
                            $"are supported", nameof(abiBaseType)
                        );
                    }
                }
                
                /// <summary>
                ///   Parses an ABI type.
                /// </summary>
                /// <param name="abiType">The ABI type</param>
                /// <returns>A tuple (baseType, dimensions) with the type details</returns>
                /// <exception cref="ArgumentException">Invalid ABI type</exception>
                public static (string baseType, int[] dimensions) ParseABIType(string abiType)
                {
                    // Regular expression to match the base type and array dimensions
                    var regex = new Regex(@"^([a-zA-Z\d]+)((\[\d*\])*)$");
                    var match = regex.Match(abiType);

                    // Test the match properly.
                    if (!match.Success)
                    {
                        throw new ArgumentException(
                            "Invalid ABI type format", nameof(abiType)
                        );
                    }

                    // Extract base type and bit size.
                    var baseType = match.Groups[1].Value;
                    if (!abiToCSharp.ContainsKey(baseType))
                    {
                        throw new ArgumentException(
                            $"Invalid or unsupported base ABI type: {baseType}. Only string, bytes, " +
                            $"address, int8 to int256, uint8 to uint256, bytes1 to bytes32, bool and tuple " +
                            $"are supported", nameof(baseType)
                        );
                    }

                    // Extract dimensions.
                    var dimensionsPart = match.Groups[2].Value;
                    var dimensionMatches = Regex.Matches(dimensionsPart, @"\[(\d*)\]");
                    var dimensions = new List<int>();

                    foreach (Match dimMatch in dimensionMatches)
                    {
                        // If dimension is empty, it's a dynamic array (-1).
                        // Otherwise, parse the dimension size.
                        dimensions.Add(
                            string.IsNullOrEmpty(dimMatch.Groups[1].Value)
                                ? -1
                                : int.Parse(dimMatch.Groups[1].Value)
                        );
                    }

                    return (baseType, dimensions.ToArray());
                }
            }
        }
    }
}
