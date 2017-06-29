using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedMinuto.Models
{
    public class DadosFeed
    {
        public DadosFeed(string Titulo, DateTimeOffset Data, string Texto, int ContPalavras)
        {
            this.TituloFeed = Titulo;
            this.DataPublicacao = Data;
            this.TextoFeed = Texto;
            this.ContPalavrasFeed = ContPalavras;

        }

        public string TituloFeed { get; set; }
        public DateTimeOffset DataPublicacao { get; set; }
        public string TextoFeed { get; set; }
        public int ContPalavrasFeed { get; set; }
    }

}