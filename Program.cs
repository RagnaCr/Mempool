using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mempool
{
    class Program
    {
        private static string result;
        static async Task Main(string[] args)
        {
            BlockchainRequest web = new BlockchainRequest { jsonrpc = "2.0", method = "txpool_content", id = 1 };
            web.@params = new string[] { };
            result = await Web3.Web3Method(web, false);
            string path = @"C:\Users\LPayr\Desktop\txpoolnew.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteAsync(result);
            }

            Dictionary<string, Dictionary<string, Dictionary<string, object>>> pending;
            pending = Web3.GetPending(result);

            TxpoolContent txpool = new TxpoolContent();

            foreach (var item in pending)
            {
                Console.WriteLine("High key - " + item.Key);
                txpool.Pending.Transactions.Add(item.Key, new Dictionary<string, Transaction>());
                foreach (var item1 in item.Value)
                {
                    Console.WriteLine("Middle key - " + item1.Key);
                    txpool.Pending.Transactions[item.Key].Add(item1.Key, new Transaction());
                    foreach (var item2 in item1.Value)
                    {
                        Console.Write(item2.Key + ": ");
                        Console.Write(item2.Value + "\n");
                        if (item2.Key == "blockhash")
                        {
                            txpool.Pending.Transactions[item.Key][item1.Key] = new Transaction
                            {
                                BlockHash = item2.Value.ToString()
                            };
                        }
                        if (item2.Key == "from")
                        {
                            txpool.Pending.Transactions[item.Key][item1.Key].From = item2.Value.ToString();
                        }
                    }
                }
            }
        }
        static async void GetGase()
        {
            BlockchainRequest web = new BlockchainRequest { jsonrpc = "2.0", method = "eth_gasPrice", id = 1 };
            web.@params = new string[] { };
            result = await Web3.Web3Method(web);
            long a = Convert.ToInt64(result.Substring(2), 16);
            Console.WriteLine($"{a}");
        }
    }
}
