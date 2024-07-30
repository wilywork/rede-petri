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

            foreach (var arco in doc.Descendants("arc"))
            {
                string de = arco.Attribute("from").Value;
                string para = arco.Attribute("to").Value;

                if (transicaoIds.ContainsKey(de) && lugarIds.ContainsKey(para))
                {
                    var nomeTransicao = transicaoIds[de];
                    var nomeLugar = lugarIds[para];
                    var transicao = redePetri.Transicoes.FirstOrDefault(t => t.Nome == nomeTransicao);
                    if (transicao != null)
                    {
                        transicao.PosCondicoes[nomeLugar] = 1; // Assumindo peso 1 para simplicidade
                    }
                }
                else if (lugarIds.ContainsKey(de) && transicaoIds.ContainsKey(para))
                {
                    var nomeTransicao = transicaoIds[para];
                    var nomeLugar = lugarIds[de];
                    var transicao = redePetri.Transicoes.FirstOrDefault(t => t.Nome == nomeTransicao);
                    if (transicao != null)
                    {
                        transicao.PreCondicoes[nomeLugar] = 1; // Assumindo peso 1 para simplicidade
                    }
                }
            }

            return redePetri;
        }
    }
}
