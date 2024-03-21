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
    public class CustomersController : ApiController
    {
        private readonly CustomersDbContext _dbContext;
        public CustomersController()
        {
            _dbContext = new CustomersDbContext();
        }

        // GET: api/Customers
        public IHttpActionResult Get()
        {
            var customers = _dbContext.GetCustomers();
            return Ok(customers);
        }

        // GET: api/Customers/5
        public IHttpActionResult Get(int id)
        {
            var customer = _dbContext.GetCustomer(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync()
        {
            byte[] requestBody = await Request.Content.ReadAsByteArrayAsync();
            string requestBodyString = Encoding.UTF8.GetString(requestBody);

            var customerObject = JsonConvert.DeserializeObject<Customer>(requestBodyString);

            var customer = _dbContext.CreateCustomerById(customerObject);

            string jsonResponse = JsonConvert.SerializeObject(customer);

            const string jsonContentType = "application/json";
            return new HttpResponseMessage
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, jsonContentType),
                StatusCode = HttpStatusCode.OK
            };
        }

        // PUT: api/Customers/5
        [HttpPut]
        public async Task<HttpResponseMessage> PutAsync()
        {
            byte[] requestBody = await Request.Content.ReadAsByteArrayAsync();
            string requestBodyString = Encoding.UTF8.GetString(requestBody);

            var customerObject = JsonConvert.DeserializeObject<Customer>(requestBodyString);

            var customer = _dbContext.UpdateCustomer(customerObject);

            string jsonResponse = JsonConvert.SerializeObject(customer);

            const string jsonContentType = "application/json";
            return new HttpResponseMessage
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, jsonContentType),
                StatusCode = HttpStatusCode.OK
            };

        }

        // DELETE: api/Customers/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var customer = _dbContext.DeleteCustomer(id);
            if (customer)
            return Ok();
            else return BadRequest();
        }
    }
}
