using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace IntegrationWEBAPI.Controllers
{
    public class WagesController : ApiController
    {
        private readonly WagesDbContext _dbContext;
        private string filePath = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/Source/Integraatioharjoitus - Valuuttakurssi.xml";
        public WagesController()
        {
            _dbContext = new WagesDbContext();
        }
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync()
        {
            byte[] requestBody = await Request.Content.ReadAsByteArrayAsync();
            string requestBodyString = Encoding.UTF8.GetString(requestBody);
            requestBodyString = requestBodyString.Trim();

            string processedXmlToJSON = PreprocessXml(requestBodyString);
            var ratesFile = File.ReadAllText(filePath);

            var rate = GetRate(ratesFile, "USD");
            string wageAdjusted = convertEuroToUSD(processedXmlToJSON, rate);

            const string jsonContentType = "application/json";
            return new HttpResponseMessage
            {
                Content = new StringContent(wageAdjusted, Encoding.UTF8, jsonContentType),
                StatusCode = HttpStatusCode.OK
            };
    
        }
        private string PreprocessXml(string xmlData)
        {
            var xmlDoc = XDocument.Parse(xmlData);
            var json = JsonConvert.SerializeXNode(xmlDoc, Formatting.Indented);

            JObject jsonObject = JObject.Parse(json);
            JArray palkkaArray = (JArray)jsonObject["palkanlaskenta"]["palkka"];
            List<Wage> employees = new List<Wage>();

            foreach (JToken item in palkkaArray)
            {

                string personName = (string)item["nimi"];
                decimal monthlySalary = (decimal)item["kuukausittain"];
                string hireDate = (string)item["työsuhdealkoi"];

                SalaryInfo salaryInfo = new SalaryInfo
                {
                    Monthly = monthlySalary.ToString()
                };

                Wage employee = new Wage
                {
                    PersonName = personName,
                    Salary = salaryInfo,
                    HireDate = hireDate
                };
                employees.Add(employee);
            }
            string newJson = JsonConvert.SerializeObject(employees, Formatting.Indented);
            return newJson;
        }
        static string convertEuroToUSD(string json, decimal rate)
        {
            var employees = JsonConvert.DeserializeObject<Wage[]>(json);

            foreach (var employee in employees)
            {

                if (decimal.TryParse(employee.Salary.Monthly, out decimal monthly))
                {
                    monthly *= rate;
                    employee.Salary.Monthly = monthly.ToString();
                }
            }

            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
        static decimal GetRate(string ratesFile, string currency)
        {
            XDocument xmlDoc = XDocument.Parse(ratesFile);
            XNamespace cubeNamespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

            var cubeElement = xmlDoc.Descendants(cubeNamespace + "Cube").FirstOrDefault();
            var cubeChildren = cubeElement.Elements();
            var moreChildren = cubeChildren.Elements();
            foreach (var child in moreChildren)
            {
                XAttribute currencyAttribute = child.Attribute("currency");
                XAttribute rateAttribute = child.Attribute("rate");
                if (currencyAttribute.Value == currency)
                {
                    decimal d = Convert.ToDecimal(rateAttribute.Value, CultureInfo.InvariantCulture);
                    return d;
                }
            }
            return 0m;
        }

    }
}


