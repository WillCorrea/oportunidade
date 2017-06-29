using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedMinuto.Models
{
    public class ResumoFeed
    {
        public ResumoFeed(string palavra, int contador)
        {
            this.Palavra = palavra;
            this.ContPalavrasResumo = contador;
        }
        public string Palavra { get; private set; }
        public int ContPalavrasResumo { get; private set; }
    }
}