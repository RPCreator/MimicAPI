using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _db;
        public PalavraRepository(MimicContext db)
        {
            _db = db;
        }
        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();

            var item = _db.Palavras.FromSqlRaw("Select * from Palavras");

            //Obter palavras a partir de determinada data
            if (query.Data != null)
            {
                item = _db.Palavras.FromSqlRaw($"Select * from Palavras where DataCriacao >'{query.Data}';");
            }

            //Criar Paginação
            if (query.PagNum.HasValue)
            {
                var qtdtotal = item.Count();

                item = item.Skip((query.PagNum.Value - 1) * query.PagRegisto.Value).Take(query.PagRegisto.Value);
                var paginacao = new Pagination();
                paginacao.NumPagina = query.PagNum.Value;
                paginacao.RegistoPorPagina = query.PagRegisto.Value;
                paginacao.TotalRegistos = qtdtotal;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)paginacao.TotalRegistos / query.PagRegisto.Value);

                lista.Pagination = paginacao;
            }
            lista.AddRange(item.ToList());
            return lista;
        }

        public Palavra Obter(int id)
        {
           return _db.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Inserir(Palavra palavra)
        {
            _db.Palavras.Add(palavra);
            _db.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _db.Palavras.Update(palavra);
            _db.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _db.Palavras.Update(_db.Palavras.Find(palavra));
            _db.SaveChanges();
        }   

    }
}
