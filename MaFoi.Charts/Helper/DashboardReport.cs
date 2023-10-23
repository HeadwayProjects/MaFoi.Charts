using MaFoi.Charts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MaFoi.Charts.Helper
{
    public class DashboardReport
    {
        private string JwtToken;
        public DashboardReport(string jwt)
        {
            JwtToken = jwt;
        }
        public async Task<DashboardStatus> GetDataSashboardChartStatus(dashparams searchParams)
        {
            string Baseurl = ConfigurationManager.AppSettings["apidashstatusurl"]; // "https://apipro.ezycomp.com/api/Auditor/GetAuditReportData"; //"https://localhost:7221/api/Auditor/GetAuditReportData";
                                                                         //  //"
            DashboardStatus reportstatus = new DashboardStatus();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Set the authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",JwtToken);

           //var request = new HttpRequestMessage() { RequestUri = new Uri(Baseurl) };
                var payload = new SearchParams()
                {
                    Search= string.IsNullOrEmpty(searchParams.Search)?"":searchParams.Search,
                    Filters=searchParams.Filters,
                    Pagination=searchParams.Pagination,
                    Sort=searchParams.Sort,
                    IncludeCentral=searchParams.IncludeCentral
                };
         // request.Method = HttpMethod.Post;
                // var Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                //var content = new form(pairs);
                HttpResponseMessage response = await client.PostAsJsonAsync(Baseurl, payload);
                response.EnsureSuccessStatusCode();

                //var response = await client.PostAsync(request,payload);

                if (response.IsSuccessStatusCode)
                {
                    var EmpResponse = response.Content.ReadAsStringAsync().Result;
    //Deserializing the response recieved from web api and storing into the Employee list
    reportstatus = JsonConvert.DeserializeObject<DashboardStatus>(EmpResponse);
                }


return reportstatus;
            }
        }

        public async Task<List<DashboardCategory>> GetDataSashboardChartCategory(dashparams searchParams,string category)
        {
            string Baseurl = ConfigurationManager.AppSettings["apidashcategoryurl"] +category; // "https://apipro.ezycomp.com/api/Auditor/GetAuditReportData"; //"https://localhost:7221/api/Auditor/GetAuditReportData";
                                                                                               //  //"
            List<DashboardCategory> reportcategory = new List<DashboardCategory>();
            using (var client = new HttpClient())
            {
                // Set the authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", JwtToken);

                var request = new HttpRequestMessage() { RequestUri = new Uri(Baseurl) };
                var payload = new SearchParams()
                {
                    Search = string.IsNullOrEmpty(searchParams.Search)?"":searchParams.Search,
                    Filters = searchParams.Filters,
                   Pagination = searchParams.Pagination,
                    Sort = searchParams.Sort,
                    IncludeCentral = searchParams.IncludeCentral
                };
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var EmpResponse = response.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    reportcategory = JsonConvert.DeserializeObject<List<DashboardCategory>>(EmpResponse);
                }


                return reportcategory;
            }
        }

        public async Task<DashBoardreport> GetDashboardReport(dashparams searchParams)
        {
            string Baseurl = ConfigurationManager.AppSettings["apidashboardreporturl"]; // "https://apipro.ezycomp.com/api/Auditor/GetAuditReportData"; //"https://localhost:7221/api/Auditor/GetAuditReportData";
                                                                                        //  //"
            DashBoardreport report = new DashBoardreport();
            using (var client = new HttpClient())
            {
                Paging paging = new Paging();
                paging.PageSize = 500;
                paging.PageNumber = 1;
                searchParams.Pagination = paging;
                // Set the authorization header with the JWT token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", JwtToken);

                var request = new HttpRequestMessage() { RequestUri = new Uri(Baseurl) };
                var payload = new SearchParams()
                {
                    Search = string.IsNullOrEmpty(searchParams.Search) ? "" : searchParams.Search,
                    Filters = searchParams.Filters,
                   Pagination = searchParams.Pagination,
                    Sort = searchParams.Sort,
                    IncludeCentral = searchParams.IncludeCentral
                };
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var EmpResponse = response.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    report = JsonConvert.DeserializeObject<DashBoardreport>(EmpResponse);
                }


                return report;
            }
        }
    }
}