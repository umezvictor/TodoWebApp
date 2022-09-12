using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Web.Client.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        // GET: api/<TodoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TodoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
