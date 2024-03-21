using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;

class Employee
{
    public required string PersonName { get; set; }
    public required SalaryInfo Salary { get; set; }
    public string? HireDate { get; set; }
}

class SalaryInfo
{
    public required string Monthly { get; set; }
}

class Program
{

    static void Main(string[] args)
    {
        string filePath = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/Source/Integraatioharjoitus - Palkanlaskenta.xml";
        string fileContent = ReadFile(filePath);

        filePath = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/Source/Integraatioharjoitus - Valuuttakurssi.xml";
        string ratesFile = ReadFile(filePath);

        string newFilePath = "C:/Users/pjetu/Documents/IntegraatioHarjoitus/In/converted.json";

        if (fileContent != null && ratesFile != null)
        {
            XDocument xmlDoc = XDocument.Parse(fileContent);
            xmlDoc.Declaration = null;
            var xmlToString = xmlDoc.ToString();

            var rate = GetRate(ratesFile, "USD");

            string converted = convertXmlToJSON(xmlToString);
            string modifiedJson = convertEuroToUSD(converted, rate);

            File.WriteAllText(newFilePath, modifiedJson);
            Console.WriteLine($"File '{newFilePath}' has been created with the converted JSON.");
        }
        else
        {
            Console.WriteLine($"File '{filePath}' does not exist or could not be read.");
        }
    }

    static string convertXmlToJSON(string fileContent)
    {
        var xmlDoc = XDocument.Parse(fileContent);
        var json = JsonConvert.SerializeXNode(xmlDoc, Formatting.Indented);

        JObject jsonObject = JObject.Parse(json);
        JArray palkkaArray = (JArray)jsonObject["palkanlaskenta"]["palkka"];
        List<Employee> employees = new List<Employee>();

        foreach (JToken item in palkkaArray)
        {

            string personName = (string)item["nimi"];
            decimal monthlySalary = (decimal)item["kuukausittain"];
            string hireDate = (string)item["työsuhdealkoi"];

            SalaryInfo salaryInfo = new SalaryInfo
            {
                Monthly = monthlySalary.ToString()
            };

            Employee employee = new Employee
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

    static string convertEuroToUSD(string fileContent, decimal rate)
    {
        var employees = JsonConvert.DeserializeObject<Employee[]>(fileContent);

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


    static string ReadFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading the file:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
