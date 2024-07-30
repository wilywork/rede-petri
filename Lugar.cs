using System;

namespace RedePetriSimulacao
{
    public class Lugar
    {
        public string Nome { get; set; }
        public int Marcadores { get; set; }
        public int Limite { get; set; }
        public Action AcaoQuandoLimiteAtingido { get; set; }

        public Lugar(string nome, int marcadores, int limite = int.MaxValue, Action acaoQuandoLimiteAtingido = null)
        {
            Nome = nome;
            Marcadores = marcadores;
            Limite = limite;
            AcaoQuandoLimiteAtingido = acaoQuandoLimiteAtingido;
        }

        public void AdicionarMarcadores(int quantidade)
        {
            Marcadores += quantidade;
            if (Marcadores >= Limite && AcaoQuandoLimiteAtingido != null)
            {
                AcaoQuandoLimiteAtingido();
            }
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
