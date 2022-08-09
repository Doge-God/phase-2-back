using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace phase_2_back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class personInfoController : ControllerBase
    {
        private readonly HttpClient _client;

        private static List<personInfo> personInfoList = new List<personInfo> { };

        public personInfoController(IHttpClientFactory clientFactory)
        {
            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }
            _client = clientFactory.CreateClient("ipInfo");
        }

        /// <summary>
        /// Gets the raw JSON for the hot feed in reddit
        /// </summary>
        /// <returns>A JSON object representing the hot feed in reddit</returns>
        [HttpGet]
        [Route("raw")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetIpInfo(String ipString)
        {
            var res = await _client.GetAsync("/" + ipString + "?token=54370b3dbd9873");
            var content = await res.Content.ReadAsStringAsync();
            return Ok(content);
        }


        /// <summary>
        /// Gets the raw JSON for the hot feed in reddit
        /// </summary>
        /// <returns>A JSON object representing the hot feed in reddit</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> getPersonInfos()
        {
            return Ok(personInfoList);
        }

        /// <summary>
        /// Demonstrates posting action
        /// </summary>
        /// <returns>A 200 OK response</returns>
        [HttpPost]
        
        public async Task<IActionResult> PostPersonInfo(String ipString, String name, int id)
        {
            var result = await _client.GetAsync("/" + ipString + "?token=54370b3dbd9873");
            var content = await result.Content.ReadAsStringAsync();
            
            var parsedContent = JObject.Parse(content);

            if (!parsedContent.ContainsKey("status"))
            {
                personInfoList.Add(
                    new personInfo
                    {
                        name = name,
                        id = id,
                        ip = ipString,
                        location = parsedContent["loc"].ToString()
                    }
                    );
                return Ok(personInfoList);
            }

            return BadRequest(result);

            //personInfo newPersonInfo = new personInfo
             
        }
    }
}
