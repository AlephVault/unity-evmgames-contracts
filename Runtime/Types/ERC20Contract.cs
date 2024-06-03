using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using UnityEngine;
using TransferFunction = Nethereum.Contracts.Standards.ERC20.ContractDefinition.TransferFunction;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        using Events;
        
        /// <summary>
        ///   An interface to the ERC20 contract.
        /// </summary>
        public class ERC20Contract : BaseContract
        {
            public ERC20Contract(Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null) : base(web3, address, transactionGasSetter)
            {
            }

            public Task<BigInteger> Allowance(string owner, string spender, BlockParameter blockParameter = null)
            {
                return Call<AllowanceFunction, BigInteger>(new AllowanceFunction
                {
                    Owner = owner,
                    Spender = spender,
                }, blockParameter);
            }

            public Task<TransactionReceipt> Approve(string spender, BigInteger value)
            {
                return Send(new ApproveFunction()
                {
                    Spender = spender,
                    Value = value,
                });
            }

            public Task<BigInteger> BalanceOf(string account, BlockParameter blockParameter = null)
            {
                return Call<BalanceOfFunction, BigInteger>(new BalanceOfFunction
                {
                    Owner = account,
                }, blockParameter);
            }

            public Task<byte> Decimals(BlockParameter blockParameter = null)
            {
                return Call<DecimalsFunction, byte>(new DecimalsFunction
                {
                }, blockParameter);
            }

            public Task<string> Name(BlockParameter blockParameter = null)
            {
                return Call<NameFunction, string>(new NameFunction
                {
                }, blockParameter);
            }

            public Task<string> Symbol(BlockParameter blockParameter = null)
            {
                return Call<SymbolFunction, string>(new SymbolFunction
                {
                }, blockParameter);
            }

            public Task<BigInteger> TotalSupply(BlockParameter blockParameter = null)
            {
                return Call<TotalSupplyFunction, BigInteger>(new TotalSupplyFunction
                {
                }, blockParameter);
            }

            public Task<TransactionReceipt> Transfer(string to, BigInteger value)
            {
                return Send(new TransferFunction()
                {
                    To = to,
                    Value = value,
                });
            }

            public Task<TransactionReceipt> TransferFrom(string from, string to, BigInteger value)
            {
                return Send(new TransferFromFunction()
                {
                    From = from,
                    To = to,
                    Value = value,
                });
            }

            public EventsWorker<ApprovalEventDTO> MakeApprovalEventsWorker(Func<Event<ApprovalEventDTO>, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            public EventsWorker<TransferEventDTO> MakeTransferEventsWorker(Func<Event<TransferEventDTO>, NewFilterInput> filterMaker, BlockParameter fromBlock = null)
            {
                return MakeEventsWorker(filterMaker, fromBlock);
            }

            /// <summary>
            ///   Returns an object which tracks the current value.
            /// </summary>
            /// <param name="account">The account to pivot</param>
            /// <param name="initialAmount">The initial amount</param>
            /// <param name="fromBlock">A starting block (optional)</param>
            /// <returns></returns>
            public ERC20EventsProcessorsSet MakeProcessorsSet(string account, BigInteger initialAmount = default, BlockParameter fromBlock = null)
            {
                if (initialAmount != 0 && (fromBlock == null || fromBlock.ParameterType == BlockParameter.BlockParameterType.earliest))
                {
                    Debug.LogWarning(
                        "You're specifying a non-zero initial amount and the earliest block. " +
                        "This does not make sense for ERC-20 contracts. Ensure you know what are you " +
                        "doing, because this is non-standard."
                    );
                }
                return new ERC20EventsProcessorsSet(this, account, initialAmount, fromBlock);
            }
        }
    }
}
