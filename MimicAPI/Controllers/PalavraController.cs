using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]  
    public class PalavraController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        public PalavraController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {

            var item = _repository.ObterPalavras(query);

            if (query.PagNum > item.Pagination.TotalPaginas)
            {
                return NotFound();
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Pagination));

            return Ok(item.ToList());
        }



        //web --/api/palavras/1
        //Obter palavra especifica
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
                return NotFound();

            return Ok(obj);
        }


       

        //--/api/palavras (Post: id:nome, ativo, ...)
        [Route("")]
        [HttpPost]
        public ActionResult InserirPalavra([FromBody]Palavra palavra)
        {
            _repository.Inserir(palavra);
            return Created($"/api/Palavras/{palavra.Id}", palavra);
        }

        //--/api/palavras/id (Post: id:nome, ativo, ...)
        [Route("{id}")]
        [HttpPut]
        public ActionResult UpdatePalavra(int id, [FromBody]Palavra palavra)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
                return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult DeletePalavra(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
                return NotFound();
            _repository.Eliminar(id);
          
            return NoContent();
        }
    }
}