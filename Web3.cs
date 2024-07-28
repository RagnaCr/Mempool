using System;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mempool
{
    //string data = $"{{\"method\":\"eth_gasPrice\",\"params\":[],\"id\":1,\"jsonrpc\":\"2.0\"}}"; формат запроса
    class Web3
    {
        private static readonly string Rpc1 = "https://bsc-dataseed4.binance.org/"; // bscmainnet
        private static readonly string Rpc2 = "https://newest-neat-tree.bsc-testnet.discover.quiknode.pro/063244829b4a7e11ffedd3b22107522ab5847174/"; // bsctestnet (quicknode)
        private static readonly string Rpc3 = "https://data-seed-prebsc-1-s1.binance.org:8545/"; // bsctestnet
        // НЕ ЗАБЫВАЙ МЕНЯТЬ RPC ВО ВСЕХ МЕТОДАХ
        private static readonly string RPC = Rpc2;
        public Web3()
        {

        }
        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> GetQueued(string txpool) // return dictionary of txpool json result
        {
            Dictionary<string, object> values = JsonSerializer.Deserialize<Dictionary<string, object>>(txpool);
            Dictionary<string, object> result = null;
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> queued = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            foreach (KeyValuePair<string, object> kvp in values)
            {
                if (kvp.Key == "result")
                {
                    result = JsonSerializer.Deserialize<Dictionary<string, object>>(kvp.Value.ToString());
                }
            }

            foreach (KeyValuePair<string, object> kvp in result)
            {
                if (kvp.Key == "queued")
                {
                    queued = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(kvp.Value.ToString());
                }
            }
            return queued;
        }
        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> GetPending(string txpool) // return dictionary of txpool json result
        {
            Dictionary<string, object> values = JsonSerializer.Deserialize<Dictionary<string, object>>(txpool);
            Dictionary<string, object> result = null;
            Dictionary<string, Dictionary<string, Dictionary<string, object>>> pending = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            foreach (KeyValuePair<string, object> kvp in values)
            {
                if (kvp.Key == "result")
                {
                    result = JsonSerializer.Deserialize<Dictionary<string, object>>(kvp.Value.ToString());
                }
            }
            foreach (KeyValuePair<string, object> kvp in result)
            {
                if (kvp.Key == "pending")
                {
                    pending = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(kvp.Value.ToString());
                }
            }
            return pending;
        }

        public static async Task<string> Web3Method(BlockchainRequest web, bool returnjson = false)
        {
            string jsonresult;
            WebRequest request = WebRequest.Create(RPC);
            request.Method = "POST";

            JsonContent content = JsonContent.Create(web);
            string json = await content.ReadAsStringAsync();
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json); // преобразования строки в поток байтов
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length); // отправляем запрос потоком байтов
            }
            WebResponse res = request.GetResponse();
            using (Stream stream = res.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonresult = reader.ReadToEnd(); // ответ сервера
                }
            }
            res.Close();
            Console.WriteLine("Запрос выполнен...");
            if (returnjson)
            {
                BlockchainResponse wres = JsonSerializer.Deserialize<BlockchainResponse>(jsonresult); // десериализация результата
                return wres.result;
            }
            else
            {
                return jsonresult;
            }
        }

        public static async Task<string> GetMempool(BlockchainRequest web)
        {
            string jsonresult;
            WebRequest request = WebRequest.Create(RPC);
            request.Method = "POST";
            
            JsonContent content = JsonContent.Create(web);
            string json = await content.ReadAsStringAsync();
            Console.WriteLine(json);

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json); // преобразования строки в поток байтов
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length); // отправляем запрос потоком байтов
            }

            WebResponse res = request.GetResponse();
            using (Stream stream = res.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonresult = reader.ReadToEnd(); // ответ сервера
                }
            }
            res.Close();
            Console.WriteLine("Mempool выполнен");
            return jsonresult;
        }
    }
    class BlockchainRequest
    {
        public string method { get; set; }
        public string[] @params { get; set; }
        public int id { get; set; }
        public string jsonrpc { get; set; }
    }

    public class TxpoolContent
    {
        public TxpoolContent()
        {
            Pending = new PendingTransactions();
            Queued = new QueuedTransactions();
        }
        public PendingTransactions Pending { get; set; }
        public QueuedTransactions Queued { get; set; }
    }

    public class PendingTransactions
    {
        public PendingTransactions()
        {
            Transactions = new Dictionary<string, Dictionary<string, Transaction>>();
        }
        public Dictionary<string, Dictionary<string, Transaction>> Transactions { get; set; }
        // ------------- address ----------- num ----- transaction ------------
    }

    public class QueuedTransactions
    {
        public QueuedTransactions()
        {
            Transactions = new Dictionary<string, Dictionary<string, Transaction>>();
        }
        public Dictionary<string, Dictionary<string, Transaction>> Transactions { get; set; }
    }

    public class Transaction
    {
        public Transaction() 
        {
            BlockHash = "";
            BlockNumber = "";
            From = "";
            Gas = "";
            GasPrice = "";
            Hash = "";
            Input = "";
            Nonce = "";
            To = "";
            TransactionIndex = "";
            Value = "";
            Type = "";
            V = "";
            R = "";
            S = "";
        }
        public string BlockHash { get; set; }
        public string BlockNumber { get; set; }
        public string From { get; set; }
        public string Gas { get; set; }
        public string GasPrice { get; set; }
        public string Hash { get; set; }
        public string Input { get; set; }
        public string Nonce { get; set; }
        public string To { get; set; }
        public string TransactionIndex { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string V { get; set; }
        public string R { get; set; }
        public string S { get; set; }
    }
    class BlockchainResponse
    {
        public int id { get; set; }
        public string jsonrpc { get; set; }
        public string result { get; set; }
    }    
}
