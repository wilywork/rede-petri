using System;

namespace RedePetriSimulacao
{
    public class Lugar
    {
        public string Nome { get; set; }
        public int Marcadores { get; set; }
        public int Limite { get; set; }

        public Lugar(string nome, int marcadores, int limite = int.MaxValue)
        {
            Nome = nome;
            Marcadores = marcadores;
            Limite = limite;
        }

        public void AdicionarMarcadores(int quantidade)
        {
            Marcadores += quantidade;
        }

        public void RemoverMarcadores(int quantidade)
        {
            Marcadores -= quantidade;
        }

        public void ZerarMarcadores()
        {
            Marcadores = 0;
        }
    }
}
