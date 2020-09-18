using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using Newtonsoft.Json;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]  
    public class PalavraController : ControllerBase
    {
        private readonly MimicContext _db;
        public PalavraController(MimicContext db)
        {
            _db = db;
        }

        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas(string? data, int? pagNum, int? pagRegisto)
        {
            var item = _db.Palavras.FromSqlRaw("Select * from Palavras");

            //Obter palavras a partir de determinada data
            if (data != null)
            {               
                item = _db.Palavras.FromSqlRaw($"Select * from Palavras where DataCriacao >'{data}';");
            }

            //Criar Paginação
            if (pagNum.HasValue)
            {
                var qtdtotal = item.Count();

                item = item.Skip((pagNum.Value -1) * pagRegisto.Value).Take(pagRegisto.Value);
                var paginacao = new Pagination();
                paginacao.NumPagina = pagNum.Value;
                paginacao.RegistoPorPagina = pagRegisto.Value;
                paginacao.TotalRegistos = qtdtotal;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)paginacao.TotalRegistos/pagRegisto.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if(pagNum > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }

            return Ok(item);
        }



        //web --/api/palavras/1
        //Obter palavra especifica
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _db.Palavras.Find(id);

            if (obj == null)
                return NotFound();

            return Ok(_db.Palavras.Find(id));
        }


       

        //--/api/palavras (Post: id:nome, ativo, ...)
        [Route("")]
        [HttpPost]
        public ActionResult InserirPalavra([FromBody]Palavra palavra)
        {
            _db.Palavras.Add(palavra);   
            _db.SaveChanges();
            return Created($"/api/Palavras/{palavra.Id}", palavra);
        }

        //--/api/palavras/id (Post: id:nome, ativo, ...)
        [Route("{id}")]
        [HttpPut]
        public ActionResult UpdatePalavra(int id, [FromBody]Palavra palavra)
        {
            var obj = _db.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (obj == null)
                return NotFound();

            palavra.Id = id;
            _db.Palavras.Update(palavra);
            _db.SaveChanges();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult DeletePalavra(int id)
        {
            var obj = _db.Palavras.Find(id);

            if (obj == null)
                return NotFound();

            _db.Palavras.Remove(_db.Palavras.Find(id));
            _db.SaveChanges();
            return NoContent();
        }
    }
}