using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        /// <summary>
        ///   This class has many checks for valid values
        ///   or names and stuff like that.
        /// </summary>
        public static class Sanitizer
        {
            // An expression validating names being also class names.
            private static Regex classNameRegex = new Regex("^[A-Z][A-Za-z0-9]*$");

            // An expression validating an ABI event name.
            private static Regex identifierRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
            
            // An expression validating a recommended function argument.
            private static Regex recommendedArgumentRegex = new Regex(@"^[a-z_][A-Za-z0-9_]*$");

            /// <summary>
            ///   Validates a contract name.
            /// </summary>
            /// <param name="name">The contract name</param>
            /// <returns>The sanitized contract name, suitable for C# class names</returns>
            /// <exception cref="ArgumentException">The contract name is not valid</exception>
            public static string SanitizeContractName(string name)
            {
                // The contract must obey PascalCase.
                // Otherwise, the ABI structure itself is wrong.
                // Since this name is chosen by the user, we can
                // enforce this rule here.
                var match = classNameRegex.Match(name);
                if (!match.Success)
                {
                    throw new ArgumentException(
                        "Invalid contract name. It must start with A-Z characters and " +
                        "continue with A-Z or a-z characters, or digits", nameof(name)
                    );
                }

                // Trivial return for the same name.
                // We might need to change this in the future.
                return name;
            }

            /// <summary>
            ///   Validates an event name, and cleans it. This is done by
            ///   removing _ signs from the event name.
            /// </summary>
            /// <param name="name">The event name</param>
            /// <returns>The cleaned event name, suitable for C# class names</returns>
            /// <exception cref="ArgumentException">The event name is not valid</exception>
            public static string SanitizeContractEventName(string name)
            {
                // The contract event name must obey [A-Za-z_][A-Za-z0-9_]*,
                // but also should obey [A-Z][A-Za-z0-9]*, or a warning will
                // be issued and also corrections will be done in the end.
                var match = identifierRegex.Match(name);
                if (!match.Success)
                {
                    throw new ArgumentException(
                        "Invalid event name. It must start with A-Z, a-z or _ characters and " +
                        "continue with A-Z, a-z, _ characters, or digits", nameof(name)
                    );
                }

                match = classNameRegex.Match(name);
                if (!match.Success)
                {
                    Debug.LogWarning(
                        $"The name '{name}' does not match a C# class name. It will be properly fixed"
                    );
                    name = name.Replace("_", "");
                    name = name.Substring(0, 1).ToUpper() + name.Substring(1);
                }

                return name;
            }

            /// <summary>
            ///   Validates an event argument name, and cleans it. This is
            ///   done by removing _ signs from the arg name.
            /// </summary>
            /// <param name="name">The argument name</param>
            /// <returns>The cleaned argument name, suitable for C# property names</returns>
            /// <exception cref="NotImplementedException">The argument name is not valid</exception>
            public static string SanitizeContractEventOrFunctionArgName(string name)
            {
                // The contract event arg name must obey [A-Za-z_][A-Za-z0-9_]*,
                // but also should obey [a-z][A-Za-z0-9]*, or a warning will be
                // issued and more corrections will be done.
                var match = identifierRegex.Match(name);
                if (!match.Success)
                {
                    throw new ArgumentException(
                        "Invalid argument name. It must start with A-Z, a-z or _ characters and " +
                        "continue with A-Z, a-z, _ characters, or digits", nameof(name)
                    );
                }

                name = name.Replace("_", "");
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);
                return name;
            }

            /// <summary>
            ///   Validates a function name, and cleans it. This is done
            ///   be removing _ signs from the function name.
            /// </summary>
            /// <param name="name">The function name</param>
            /// <returns>The cleaned function name, suitable for methods</returns>
            /// <exception cref="NotImplementedException">The function name is not valid</exception>
            public static string SanitizeContractFunctionName(string name)
            {
                var match = identifierRegex.Match(name);
                if (!match.Success)
                {
                    throw new ArgumentException(
                        "Invalid function name. It must start with A-Z, a-z or _ characters and " +
                        "continue with A-Z, a-z, _ characters, or digits", nameof(name)
                    );
                }

                name = name.Replace("_", "");
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);
                return name;
            }
        }
    }
}