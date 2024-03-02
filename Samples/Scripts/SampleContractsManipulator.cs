using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts;
using AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts.TokensContractComponents.Events;
using AlephVault.Unity.EVMGames.Contracts.Types;
using Nethereum.ABI;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using UnityEngine;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples
{
    public class SampleContractsManipulator : MonoBehaviour
    {
        private const string GAME_ADDRESS = "0xbD004d9048C9b9e5C4B5109c68dd569A65c47CF9";
        private const string USER_ADDRESS = "0x11BdE3126f46Cfb3851a9102c60b510B1305aF5b";

        private const string GAME_PRIVKEY = "0x91fd4e8a060cceff00ae5cde99d5b167179f724d9a424e24672e4200c7679c98";
        private const string USER_PRIVKEY = "0x6926ce01074106dcf4456a4e2cd744cf21fb062901494cc85141c9082659a7b1";

        private Web3 web3Game;
        private Web3 web3User;
        private ITransactionGasSetter transactionGasSetterBoth = null;

        private BridgeContract bridge;
        private TokensContract tokens;

        private void Awake()
        {
            // Address will be: http://localhost:8545.
            web3Game = new Web3(new Account(new EthECKey(GAME_PRIVKEY)));
            web3User = new Web3(new Account(new EthECKey(USER_PRIVKEY)));
            transactionGasSetterBoth = null;
        }

        // Start is called before the first frame update
        private async void Start()
        {
            bridge = new BridgeContract(web3Game, transactionGasSetterBoth);
            tokens = await bridge.Economy(web3User, transactionGasSetterBoth);

            BigInteger balance = await tokens.BalanceOf(web3Game.TransactionManager.Account.Address, new BigInteger(1));
            Debug.Log($"Balance of user in tokens: {balance}");
            
            var worker = tokens.MakeTransferSingleEventsWorker((@event, parameter) =>
            {
                return @event.CreateFilterInput(null, null, new []{ USER_ADDRESS, GAME_ADDRESS }, parameter, null);
            });
            foreach (var @event in await worker.GetEvents())
            {
                Debug.Log($"Event: from={@event.Event.From} to={@event.Event.To} id={@event.Event.Id} value={@event.Event.Value}");
            }
        }
    }
}
