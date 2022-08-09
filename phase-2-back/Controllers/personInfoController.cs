using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace phase_2_back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class personInfoController : ControllerBase
    {
        private readonly HttpClient _client;
        private IConfiguration _configuration;

        private static List<personInfo> personInfoList = new List<personInfo> { };

        public personInfoController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("ipInfo");
            _configuration = configuration;
        }

        /// <summary>
        /// return IP info directly from external API
        /// </summary>
        /// <returns>A JSON object representing info around an IP</returns>
        [HttpGet]
        [Route("raw")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetIpInfo(String ipString)
        {
            var res = await _client.GetAsync("/" + ipString + "?token=" + _configuration.GetConnectionString("apiKey"));
            var content = await res.Content.ReadAsStringAsync();
            return Ok(content);
        }


        /// <summary>
        /// return list of all stored personInfo
        /// </summary>
        /// <returns>A JSON object representing all personInfo</returns>
        [HttpGet]
        [Route("getPersonInfoList")]
        [ProducesResponseType(200)]
        public IActionResult getPersonInfoList()
        {
            return Ok(personInfoList);
        }

        /// <summary>
        /// get personInfo with Id
        /// </summary>
        /// <returns>A 200 ok Response with personInfo, 400 BadRequest if id does not exist</returns>
        [HttpGet]
        [Route("getPersonInfo")]
        public IActionResult getPersonInfo(int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange == null)
            {
                return BadRequest("Id not found.");
            }
            else
            {
                return Ok(infoToChange);
            }
        }

        /// <summary>
        /// Post person info, automatically fills in physical location tied to IP.
        /// </summary>
        /// <returns>A 200 OK response or 400 error if external api call fails</returns>
        [HttpPost]
        [Route("addPersonInfo")]
        public async Task<IActionResult> addPersonInfo(String ipString, String name, int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange != null)
            {
                return BadRequest("Id already exist. Change or use updatePersonInfo instead.");
            }

            var result = await _client.GetAsync("/" + ipString + "?token=" + _configuration.GetConnectionString("apiKey"));
            var content = await result.Content.ReadAsStringAsync();
            
            var parsedContent = JObject.Parse(content);       

            try { 
                parsedContent["loc"].ToString(); }
            catch (Exception e) { 
                return BadRequest(result); }
            
            var newPersonInfo = new personInfo
            {
                name = name,
                id = id,
                ip = ipString,
                location = parsedContent["loc"].ToString()
            };
            personInfoList.Add(newPersonInfo);
            return Ok(newPersonInfo);
             
        }


        /// <summary>
        /// Update person info, automatically fills in physical location tied to IP. use id to specify element to be changed.
        /// </summary>
        /// <returns>A 200 OK response or 400 error if external api call fails</returns>
        [HttpPut]
        [Route("updatePersonInfo")]
        public async Task<IActionResult> updatePersonInfo(String ipString, String name, int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange == null)
            {
                return BadRequest("Id not found. Use post addPersonInfo instead.");
            }
            var result = await _client.GetAsync("/" + ipString + "?token=" + _configuration.GetConnectionString("apiKey"));
            var content = await result.Content.ReadAsStringAsync();

            var parsedContent = JObject.Parse(content);

            try
            {
                infoToChange.location = parsedContent["loc"].ToString();
            }
            catch (Exception e)
            {
                return BadRequest(result);
            }

            infoToChange.name = name;
            infoToChange.ip = ipString;
            
                
            return Ok(infoToChange);
        }


        /// <summary>
        /// delete personInfo from list with Id
        /// </summary>
        /// <returns>A 204 No Content Response: item deleted, 400 BadRequest if id does not exist</returns>
        [HttpDelete]
        [Route("deletePersonInfo")]
        public IActionResult deletePersonInfo(int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange == null)
            {
                return BadRequest("Id not found.");
            } else
            {
                personInfoList.Remove(infoToChange);
                return NoContent();
            }
        }

    }
}
