using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace IntegrationWEBAPI.Controllers
{
    public class Test { public string Value; }

    public class RequestDiag { public DateTime timestamp;  public int requestBodySize; public string savedFileName; }

    [Authorize]
    [Route("api/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync()
        {
            byte[] requestBody = await Request.Content.ReadAsByteArrayAsync();
            string requestBodyString = Encoding.UTF8.GetString(requestBody);
            System.Diagnostics.Debug.WriteLine("Received value: " + requestBodyString);

            DateTime timestamp = DateTime.Now;
            int requestBodySize = requestBody.Length;
            string fileName = "received_body_" + timestamp.ToString("yyyyMMddHHmmssfff") + ".txt";
            string filePath = Path.Combine("C:\\Users\\pjetu\\Documents\\IntegraatioHarjoitus\\In", fileName);

            File.WriteAllText(filePath, requestBodyString);
            System.Diagnostics.Debug.WriteLine("Body saved to file successfully.");

            string fileNameDiag = "received_request_" + timestamp.ToString("yyyyMMddHHmmssfff") + ".txt";
            string filePathDiag = Path.Combine("C:\\Users\\pjetu\\Documents\\IntegraatioHarjoitus\\In", fileNameDiag);

            var diag = new RequestDiag
            {
                timestamp = timestamp,
                requestBodySize = requestBodySize,
                savedFileName = fileName
            };

            string diagJson = JsonConvert.SerializeObject(diag);

            File.WriteAllText(filePathDiag, diagJson);

            const string jsonContentType = "application/json";
            return new HttpResponseMessage
            {
                Content = new StringContent("Saved body to a file!", Encoding.UTF8, jsonContentType),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
