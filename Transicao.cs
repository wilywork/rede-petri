using System;
using System.Collections.Generic;
using System.Threading;

namespace RedePetriSimulacao
{
    public class Transicao
    {
        public string Nome { get; set; }
        public Dictionary<string, int> PreCondicoes { get; set; }
        public Dictionary<string, int> PosCondicoes { get; set; }
        public Dictionary<string, string> MarkingUpdates { get; set; }
        public string EnablingFunction { get; set; }
        private static readonly object lockObj = new object();

        public Transicao(string nome)
        {
            Nome = nome;
            PreCondicoes = new Dictionary<string, int>();
            PosCondicoes = new Dictionary<string, int>();
            MarkingUpdates = new Dictionary<string, string>();
            EnablingFunction = string.Empty;
        }

        public bool PodeDisparar(Dictionary<string, Lugar> lugares)
        {
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(EnablingFunction))
                {
                    if (!AvaliarEnablingFunction(lugares))
                    {
                        return false;
                    }
                }

                foreach (var preCondicao in PreCondicoes)
                {
                    if (lugares[preCondicao.Key].Marcadores < preCondicao.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private bool AvaliarEnablingFunction(Dictionary<string, Lugar> lugares)
        {
            if (EnablingFunction.Contains(">="))
            {
                var partes = EnablingFunction.Split(new string[] { ">=" }, StringSplitOptions.None);
                var lugar = partes[0].Trim();
                var valor = int.Parse(partes[1].Trim());

                return lugares[lugar].Marcadores >= valor;
            }

            return true;
        }

        public void Disparar(Dictionary<string, Lugar> lugares)
        {
            lock (lockObj)
            {
                if (PodeDisparar(lugares))
                {
                    // Remover marcadores das precondições
                    foreach (var preCondicao in PreCondicoes)
                    {
                        lugares[preCondicao.Key].RemoverMarcadores(preCondicao.Value);
                    }

                    // Adicionar marcadores nas pós-condições
                    foreach (var posCondicao in PosCondicoes)
                    {
                        Console.WriteLine($"AdicionarMarcadores: {posCondicao.Key}, {posCondicao.Value}.");
                        lugares[posCondicao.Key].AdicionarMarcadores(posCondicao.Value);
                    }

                    // Aplicar marking-updates
                    foreach (var update in MarkingUpdates)
                    {
                        var partes = update.Value.Split('=');
                        if (partes.Length > 1)
                        {
                            var lugar = partes[0].Trim();
                            var valor = int.Parse(partes[1].Trim());
                            lugares[lugar].Marcadores = valor;
                        }
                    }

                    Console.WriteLine($">>>>>> Transição {Nome} disparada.");

                    // Lógica específica para pouso de avião
                    if (Nome.StartsWith("Pousar_"))
                    {
                        var aviaoDisponivel = Nome.EndsWith("A") ? "Aviao_disponivel_A" : "Aviao_disponivel_B";
                        lugares[aviaoDisponivel].AdicionarMarcadores(1);
                        Console.WriteLine($"Avião pousou de {Nome}, {aviaoDisponivel} atualizado.");
                    }
                }
            }
        }
    }
}
