using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RedePetriSimulacao
{
    public static class ParserRedePetri
    {
        public static RedePetri Parse(string caminhoArquivo)
        {
            XDocument doc = XDocument.Load(caminhoArquivo);
            RedePetri redePetri = new RedePetri();
            Dictionary<string, string> lugarIds = new Dictionary<string, string>();
            Dictionary<string, string> transicaoIds = new Dictionary<string, string>();
            Dictionary<string, string> jointIds = new Dictionary<string, string>();

            // Parse lugares
            foreach (var lugarElement in doc.Descendants("place"))
            {
                string id = lugarElement.Attribute("uuid").Value;
                string nome = lugarElement.Element("properties").Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "0.default.name")
                                ?.Attribute("name").Value;
                int marcadores = int.Parse(lugarElement.Element("properties").Elements("property")
                                   .FirstOrDefault(p => p.Attribute("id").Value == "default.marking")
                                   ?.Attribute("marking").Value);

                Lugar lugar = new Lugar(nome, marcadores);
                redePetri.AdicionarLugar(lugar);
                lugarIds[id] = nome;
            }

            // Parse transições
            foreach (var transicaoElement in doc.Descendants("transition"))
            {
                string id = transicaoElement.Attribute("uuid").Value;
                string nome = transicaoElement.Element("properties").Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "0.default.name")
                                ?.Attribute("name").Value;
                Transicao transicao = new Transicao(nome);
                transicaoIds[id] = nome;

                var markingUpdateElement = transicaoElement.Element("properties")
                                .Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "11.default.markingUpdate");

                if (markingUpdateElement != null)
                {
                    var updates = markingUpdateElement.Attribute("marking-update").Value.Split(';');
                    foreach (var update in updates)
                    {
                        var partes = update.Split('=');
                        if (partes.Length > 1)
                        {
                            var lugar = partes[0].Trim();
                            var valor = partes[1].Trim();
                            transicao.MarkingUpdates[lugar] = update;
                        }
                    }
                }

                var enablingFunctionElement = transicaoElement.Element("properties")
                                .Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "10.default.enablingFunction");

                if (enablingFunctionElement != null)
                {
                    transicao.EnablingFunction = enablingFunctionElement.Attribute("enabling-function").Value;
                }

                redePetri.AdicionarTransicao(transicao);
            }

            // Parse joints
            foreach (var jointElement in doc.Descendants("joint"))
            {
                string id = jointElement.Attribute("uuid").Value;
                jointIds[id] = id; // Usamos o próprio ID como valor pois não há um nome específico
            }

            // Parse arcos e conectar lugares, transições e joints
            foreach (var arco in doc.Descendants("arc"))
            {
                string de = arco.Attribute("from").Value;
                string para = arco.Attribute("to").Value;

                // Resolver a origem e destino, lidando com joints
                string origem = ResolverOrigemDestino(doc, de, jointIds);
                string destino = ResolverOrigemDestino(doc, para, jointIds);

                if (transicaoIds.ContainsKey(origem) && lugarIds.ContainsKey(destino))
                {
                    var nomeTransicao = transicaoIds[origem];
                    var nomeLugar = lugarIds[destino];
                    var transicao = redePetri.Transicoes.FirstOrDefault(t => t.Nome == nomeTransicao);
                    if (transicao != null)
                    {
                        transicao.PosCondicoes[nomeLugar] = 1; // Assumindo peso 1
                    }
                }
                else if (lugarIds.ContainsKey(origem) && transicaoIds.ContainsKey(destino))
                {
                    var nomeTransicao = transicaoIds[destino];
                    var nomeLugar = lugarIds[origem];
                    var transicao = redePetri.Transicoes.FirstOrDefault(t => t.Nome == nomeTransicao);
                    if (transicao != null)
                    {
                        transicao.PreCondicoes[nomeLugar] = 1; // Assumindo peso 1
                    }
                }
            }

            return redePetri;
        }

        private static string ResolverOrigemDestino(XDocument doc, string id, Dictionary<string, string> jointIds)
        {
            while (jointIds.ContainsKey(id))
            {
                id = doc.Descendants("arc").FirstOrDefault(a => a.Attribute("from").Value == id)?.Attribute("to").Value;
            }
            return id;
        }
    }
}
