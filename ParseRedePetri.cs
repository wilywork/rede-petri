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

            foreach (var lugarElement in doc.Descendants("place"))
            {
                string nome = lugarElement.Element("properties").Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "0.default.name")
                                ?.Attribute("name").Value;
                int marcadores = int.Parse(lugarElement.Element("properties").Elements("property")
                                   .FirstOrDefault(p => p.Attribute("id").Value == "default.marking")
                                   ?.Attribute("marking").Value);

                Lugar lugar = new Lugar(nome, marcadores);
                redePetri.AdicionarLugar(lugar);
            }

            foreach (var transicaoElement in doc.Descendants("transition"))
            {
                string nome = transicaoElement.Element("properties").Elements("property")
                                .FirstOrDefault(p => p.Attribute("id").Value == "0.default.name")
                                ?.Attribute("name").Value;
                Transicao transicao = new Transicao(nome);

                foreach (var arco in doc.Descendants("arc"))
                {
                    string de = arco.Attribute("from").Value;
                    string para = arco.Attribute("to").Value;

                    if (de == transicaoElement.Attribute("uuid").Value)
                    {
                        var nomeLugar = doc.Descendants("place")
                            .FirstOrDefault(p => p.Attribute("uuid").Value == para)
                            ?.Element("properties")
                            .Elements("property")
                            .FirstOrDefault(pr => pr.Attribute("id").Value == "0.default.name")
                            ?.Attribute("name").Value;

                        if (nomeLugar != null)
                        {
                            transicao.PosCondicoes[nomeLugar] = 1; // Assumindo peso 1 para simplicidade
                        }
                    }
                    else if (para == transicaoElement.Attribute("uuid").Value)
                    {
                        var nomeLugar = doc.Descendants("place")
                            .FirstOrDefault(p => p.Attribute("uuid").Value == de)
                            ?.Element("properties")
                            .Elements("property")
                            .FirstOrDefault(pr => pr.Attribute("id").Value == "0.default.name")
                            ?.Attribute("name").Value;

                        if (nomeLugar != null)
                        {
                            transicao.PreCondicoes[nomeLugar] = 1; // Assumindo peso 1 para simplicidade
                        }
                    }
                }

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

            return redePetri;
        }
    }
}
