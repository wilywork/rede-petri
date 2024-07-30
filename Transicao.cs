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
        private static readonly object lockObj = new object();

        public Transicao(string nome)
        {
            Nome = nome;
            PreCondicoes = new Dictionary<string, int>();
            PosCondicoes = new Dictionary<string, int>();
        }

        public bool PodeDisparar(Dictionary<string, Lugar> lugares)
        {
            lock (lockObj)
            {
                if (PreCondicoes.Count > 0)
                {
                    foreach (var preCondicao in PreCondicoes)
                    {
                        //Console.WriteLine($"PodeDisparar {preCondicao.Key} = {lugares[preCondicao.Key].Marcadores} < {preCondicao.Value} ? {!(lugares[preCondicao.Key].Marcadores == 0 || lugares[preCondicao.Key].Marcadores <= preCondicao.Value)} ");
                        if (lugares[preCondicao.Key].Marcadores == 0 || lugares[preCondicao.Key].Marcadores <= preCondicao.Value)
                        {
                            return false;
                        }

                    }
                    //Console.WriteLine($"!!!!!!!!!PodeDisparar true");
                    return true;
                }
                else
                {
                    // Console.WriteLine($"PodeDisparar true");
                    return true;
                }

            }
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
                        lugares[posCondicao.Key].AdicionarMarcadores(posCondicao.Value);
                    }

                    Console.WriteLine($"Transição {Nome} disparada.");

                    //// Lógica específica para decolagem de avião
                    //if (Nome.StartsWith("Decolar_"))
                    //{
                    //    var lugarPassageiros = Nome.EndsWith("A") ? "Passageiro_A" : "Passageiro_B";
                    //    var pistaVaga = Nome.EndsWith("A") ? "Pista_vaga_A" : "Pista_vaga_B";
                    //    lugares[lugarPassageiros].ZerarMarcadores();
                    //    lugares[pistaVaga].AdicionarMarcadores(1);
                    //    Console.WriteLine($"Avião decolou de {Nome}, passageiros de {lugarPassageiros} foram zerados e a {pistaVaga} foi liberada.");
                    //}
                }
            }
        }
    }
}
