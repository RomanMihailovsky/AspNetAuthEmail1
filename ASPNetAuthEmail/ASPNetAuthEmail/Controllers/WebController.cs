using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ASPNetAuthEmail.Controllers
{
    public class WebController : ApiController
    {
        //private ReservationRepository repo = ReservationRepository.Current;

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public string Create([FromBody]string value)
        {
            return "create";
        }

        // PUT api/<controller>/5
        [HttpPut]
        public string Update(int id, [FromBody]string value)
        {
            return "update";
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}