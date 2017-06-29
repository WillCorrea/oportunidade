using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FeedMinuto.Models
{
    public class FeedNoticias
    {
        public FeedNoticias()
        {
            this.ListaFeed = new List<DadosFeed>();
            this.ListaResumo = new List<ResumoFeed>();
        }

        public List<DadosFeed> ListaFeed { get; set; }
        public List<ResumoFeed> ListaResumo { get; set; }

        public FeedNoticias ObterFeedNoticias()
        {
            //Instancia a Classe de retono da Model
            FeedNoticias retornoFeed = new FeedNoticias();
            
            //Obtem a url do Feed localizado no web.config
            string PathFeed = System.Configuration.ConfigurationManager.AppSettings["FeedMinutoURL"].ToString();

            //Cria a variavel referente ao xml localizado na url em questao
            using (var reader = System.Xml.XmlReader.Create(PathFeed))
            {
                //Efetua a leitura do feed de noticias
                System.ServiceModel.Syndication.SyndicationFeed feed = System.ServiceModel.Syndication.SyndicationFeed.Load(reader);

                var Palavras = new List<string>();
                int QuantidadeFeeds = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumeroFeeds"].ToString());
                int PalavrasResumo = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumeroPalavrasResumo"].ToString());

                //Obtem os ultimos tópicos do feed de acordo com a configuracao no web.config (10)
                foreach (var item in feed.Items.Take(QuantidadeFeeds).OrderByDescending(x => x.PublishDate))
                {
                    //Obtem a lista de palavras desconsiderando as preposicoes
                    var ListaPalavas = ObtemPalavras(item.Summary.Text);

                    //Adiciona ao objeto a lista de palavras do topico
                    Palavras.AddRange(ListaPalavas);

                    //Adiciona ao objeto de retorno os feeds mais atuais
                    retornoFeed.ListaFeed.Add(new DadosFeed(item.Title.Text, item.PublishDate, item.Summary.Text, ListaPalavas.Count()));
                }

                //Faz uma consulta no objeto para quantificar as palavras utilizadas em todos os topicos
                var resultado = from p in Palavras
                                group p by p into g
                                select new { palavra = g.Key, contador = g.Count() };

                //Percorre o objeto de forma decrescente e obtem o numero de registros de acordo com a configuracao no web.config (10)
                foreach (var registo in resultado.OrderByDescending(x => x.contador).Take(PalavrasResumo))
                {
                    //Adiciona ao objeto de retorno o resumo das palavras mais utilizadas e quantas vezes se repetem
                    retornoFeed.ListaResumo.Add(new ResumoFeed(registo.palavra, registo.contador));
                }

            }

            return retornoFeed;
        }

        private static string RetiraTagsHTML(string stringHtml)
        {
            return Regex.Replace(System.Net.WebUtility.HtmlDecode(stringHtml), "<.*?>", String.Empty);
        }

        private static List<string> ObtemPalavras(string texto)
        {
            char[] delimitadores = new char[] { ' ', '\r', '\n' };
            var preposicoes = new string[] { "A", "ANTE", "APÓS", "ATÉ", "COM", "CONTRA", "DE", "DESDE", "EM", "ENTRE", "PARA", "PERANTE", "POR", "SEM", "SOB", "SOBRE", "TRÁS" };
            var artigos = new string[] { "O", "A", "OS", "AS", "UM", "UMA", "UNS", "UMAS" };

            texto = RetiraTagsHTML(texto);

            var palavars = texto.Split(delimitadores, StringSplitOptions.RemoveEmptyEntries);
            return palavars.Where(x => !preposicoes.Contains(x.ToUpper()) && !artigos.Contains(x.ToUpper())).ToList();
        }
    }
}