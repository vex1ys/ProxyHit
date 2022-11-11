using System;
using System.Net;
using Leaf.xNet;
using Newtonsoft.Json;

namespace Proxy
{
    internal class Program
    {
        public static List<string> proxies = new List<string>();
        public static List<string> http = new List<string>();
        public static List<string> socks4 = new List<string>();
        public static List<string> socks5 = new List<string>();
        public static string[] proxiesw = new string[0];
        
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();
            for(int i = 0; i<int.MaxValue; i++)
            {
                
                using (StreamReader sr = new StreamReader(wc.OpenRead("https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all")))
                {
                    while (!sr.EndOfStream)
                    {
                        var du = sr.ReadLine();
                        if (du == null)
                        {

                        }
                        else
                        {
                          proxies.Add(du);
                          
                        }
                        
                    }
                }
                

                using (StreamReader sr = new StreamReader(wc.OpenRead("https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks5&timeout=10000&country=all")))
                {
                    while (!sr.EndOfStream)
                    {
                        var du = sr.ReadLine();
                        if (du == null)
                        {

                        }
                        else
                        {
                            proxies.Add(du);
                           
                        };
                    }
                }
                string count = Convert.ToString(proxies.Count);
                Console.WriteLine(count);
                Parallel.For(0, proxies.Count, delegate (int io)

                    {
                        string one = proxies[io].Split(':')[0];
                        string two = proxies[io].Split(':')[1];
                        HttpRequest req = new HttpRequest();
                        req.ConnectTimeout = 3000;
                        try
                        {
                            req.Proxy = new HttpProxyClient(proxies[io].Split(':')[0], Convert.ToInt32(two));
                            var req1 = req.Get("https://example.com/");
                            http.Add(proxies[io]);
                            Console.WriteLine("Http proxy: "+ proxies[io]);

                        }
                        catch (Exception)
                        {
                             try
                        {
                            req.Proxy = new Socks4ProxyClient(proxies[io].Split(':')[0], Convert.ToInt32(two));
                            var req1 = req.Get("https://example.com/");
                            socks4.Add(proxies[io]);
                            Console.WriteLine("Socks4 proxy: "+ proxies[io]);
                        }
                        catch (Exception)
                        {
                             try
                        {
                            req.Proxy = new Socks5ProxyClient(proxies[io].Split(':')[0], Convert.ToInt32(two));
                            var req1 = req.Get("https://example.com/");
                            socks5.Add(proxies[io]);
                            Console.WriteLine("Socks5 proxy: "+ proxies[io]);
                        }
                        catch (Exception)
                        {
                            
                        }
                        }
                        }
                    });
            File.WriteAllLines(@"http", http);
            File.WriteAllLines(@"socks4", socks4);
            File.WriteAllLines(@"socks5", socks5);

            Thread.Sleep(1000);
            proxies.Clear();
            http.Clear();
             socks4.Clear();
              socks5.Clear();
            }
             
        }
    }
}