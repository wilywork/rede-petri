using System;
using System.Collections.Generic;
using System.Threading;

namespace RedePetriSimulacao
{
    public class RedePetri
    {
        public Dictionary<string, Lugar> Lugares { get; set; }
        public List<Transicao> Transicoes { get; set; }

        public RedePetri()
        {
            Lugares = new Dictionary<string, Lugar>();
            Transicoes = new List<Transicao>();
        }

        public void AdicionarLugar(Lugar lugar)
        {
            Lugares[lugar.Nome] = lugar;
        }

        public void AdicionarTransicao(Transicao transicao)
        {
            Transicoes.Add(transicao);
        }

        public void Simular(int iteracoes)
        {
            for (int i = 0; i < iteracoes; i++)
            {
                List<Thread> threads = new List<Thread>();

                foreach (var transicao in Transicoes)
                {
                    Thread thread = new Thread(() => {
                        if (transicao.PodeDisparar(Lugares))
                        {
                            transicao.Disparar(Lugares);
                        }
                    });
                    threads.Add(thread);
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                Console.WriteLine($"Estado após iteração {i + 1}:");
                foreach (var lugar in Lugares)
                {
                    Console.WriteLine($"{lugar.Key}: {lugar.Value.Marcadores} marcadores");
                }
            }
        }
    }
}
