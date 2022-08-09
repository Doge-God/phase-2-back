﻿using Microsoft.AspNetCore.Mvc;
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
        /// return IP info directly from external API
        /// </summary>
        /// <returns>A JSON object representing info around an IP</returns>
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
        /// return list of all stored personInfo
        /// </summary>
        /// <returns>A JSON object representing all personInfo</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> getPersonInfoList()
        {
            return Ok(personInfoList);
        }

        /// <summary>
        /// Post person info, automatically fills in physical location tied to IP.
        /// </summary>
        /// <returns>A 200 OK response or 400 error if external api call fails</returns>
        [HttpPost]
        
        public async Task<IActionResult> addPersonInfo(String ipString, String name, int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange != null)
            {
                return BadRequest("Id already exist. Change or use updatePersonInfo instead.");
            }

            var result = await _client.GetAsync("/" + ipString + "?token=54370b3dbd9873");
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
        public async Task<IActionResult> updatePersonInfo(String ipString, String name, int id)
        {
            var infoToChange = personInfoList.Find(x => x.id == id);
            if (infoToChange == null)
            {
                return BadRequest("Id not found. Use post addPersonInfo instead.");
            }
            var result = await _client.GetAsync("/" + ipString + "?token=54370b3dbd9873");
            var content = await result.Content.ReadAsStringAsync();

            var parsedContent = JObject.Parse(content);


            infoToChange.name = name;
            infoToChange.ip = ipString;
            try { 
                infoToChange.location = parsedContent["loc"].ToString(); }
            catch (Exception e) { 
                return BadRequest(result); }
                
            return Ok(infoToChange);
        }


        /// <summary>
        /// delete personInfo from list with Id
        /// </summary>
        /// <returns>A 204 No Content Response: item deleted, 400 BadRequest if id does not exist</returns>
        [HttpDelete]
        [ProducesResponseType(204)]
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
