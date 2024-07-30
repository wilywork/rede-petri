using RedePetriSimulacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedePetriSimulacao
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Caminho para o arquivo XML
            string caminhoArquivoXML = "Aeroporto.xpn";

            // Parse e criação da rede de Petri
            RedePetri redePetri = ParserRedePetri.Parse(caminhoArquivoXML);

            // Definir o número de iterações para a simulação
            int iteracoes = 5; // Pode ser lido de uma entrada do usuário

             Console.WriteLine("Iniciando simulação da Rede de Petri...");
            // Simulação
             redePetri.Simular(iteracoes);
             Console.WriteLine("Simulação concluída.");
            while (true)
            {

            }
        }
    }
}