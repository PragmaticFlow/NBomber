using Newtonsoft.Json;
using WebAppSimulator.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CSharpProd.HTTP.SimpleBookstore
{
    public class InitSimpleBookstore
    {
        public void SingUpUser()
        {
            //var user = new UserSingup
            //{
            //    FirstName = "Jon",
            //    LastName = "Smith",
            //    Email = "asd@gmail.com",
            //    Password = "fdsuHS12=",
            //};

            var user = new UserLogin
            {
                Email = "asd@gmail.com",
                Password = "fdsuHS12="
            };

            var httpClient = new HttpClient();
            //var url = "http://localhost:5195/api/bookstoreusers/singup";
            var url = "http://localhost:5195/api/bookstoreusers/login";

            var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = httpClient.SendAsync(message).Result;
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jwt = response.Content.ReadFromJsonAsync<Response<string>>().Result;
                Console.WriteLine(jwt.Data);
            }
        }
    }
}
