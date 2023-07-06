using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.DataVisualization.Charting;
using MaFoi.Charts.Models;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
namespace MaFoi.Charts.Controllers
{
    public class ChartController : ApiController
    {
        [HttpGet]
        [Route("GetBarChart")]
        public MemoryStream GetBarChart()//List<ToDo> toDoList)
        {
            var query = new List<dynamic>();
            query.Add(new
            {
                Legend = "Compliant",
                Count = 25//toDoList.Count(l => l.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = 10//toDoList.Count(l => l.AuditStatus != "Compliant")
            });

            var chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
            };

            chart.Titles.Add("Overall compliance score %");
            chart.Titles[0].Font = new Font("Arial", 14f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisY.Title = "";
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Bar;

            foreach (var q in query)
            {
                chart.Series[0].Color = (q.Legend == "Compliant") ? Color.Green : Color.Red;
                chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
            }
            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                //var fileUploadPayload = new UploadFilePathObject()
                //{
                //    BlobContainerName = "Audit",
                //    CompanyId = "",//reportData.AuditReportSummary.Company.ToString(),
                //    AssociateCompanyId = "",//reportData.AuditReportSummary.AssociateCompany.ToString(),
                //    LocationId = "",//reportData.AuditReportSummary.Location.ToString(),
                //    Year = 0,//reportData.AuditReportSummary.Year,
                //    Month = "",//reportData.AuditReportSummary.Month,
                //    FileName = Guid.NewGuid().ToString()
                //};
                //var awsProvider = new AwsS3BucketProvider();
                //var fileName = await awsProvider.UploadFile(chartimage, fileUploadPayload);
                //return File(chartimage.GetBuffer(),"image/png");
                return chartimage;
            }
        }

        [HttpGet]
        [Route("GetPieChart")]
        public MemoryStream GetPieChart()//List<ToDo> toDoList)
        {
            var query = new List<dynamic>();
            query.Add(new
            {
                Legend = "Compliant",
                Count = 25//toDoList.Count(l => l.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = 10//toDoList.Count(l => l.AuditStatus != "Compliant")
            });

            var chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add("Overall compliance score %");
            chart.Titles[0].Font = new Font("Arial", 14f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisY.Title = "";
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Pie;

            foreach (var q in query)
            {
                chart.Series[0].Color = (q.Legend == "Compliant") ? Color.Green : Color.Red;
                chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
            }
            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                //var fileUploadPayload = new UploadFilePathObject()
                //{
                //    BlobContainerName = "Audit",
                //    CompanyId = "",//reportData.AuditReportSummary.Company.ToString(),
                //    AssociateCompanyId = "",//reportData.AuditReportSummary.AssociateCompany.ToString(),
                //    LocationId = "",//reportData.AuditReportSummary.Location.ToString(),
                //    Year = 0,//reportData.AuditReportSummary.Year,
                //    Month = "",//reportData.AuditReportSummary.Month,
                //    FileName = Guid.NewGuid().ToString()
                //};
                //var awsProvider = new AwsS3BucketProvider();
                //var fileName = await awsProvider.UploadFile(chartimage, fileUploadPayload);
                //return File(chartimage.GetBuffer(), "image/png");
                return chartimage;
            }
        }

        [HttpPost]
        [Route("Chart/GenerateCharts")]
        public async Task<string> GenerateCharts(List<ToDo> toDoList)
        {
            var fileName = Guid.NewGuid().ToString();
            var query = new List<dynamic>();
            query.Add(new
            {
                Legend = "Compliant",
                Count = toDoList.Count(l => l.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = toDoList.Count(l => l.AuditStatus != "Compliant")
            });

            var chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add("Overall compliance score %");
            chart.Titles[0].Font = new Font("Arial", 14f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisY.Title = "";
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Doughnut;

            foreach (var q in query)
            {
                chart.Series[0].Color = (q.Legend == "Compliant") ? Color.Green : Color.Red;
                chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
            }
            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                var fileUploadPayload = new UploadFilePathObject()
                {
                    BlobContainerName = "Audit",
                    FileName = fileName + "_pie"
                };
                var awsProvider = new AwsS3BucketProvider();
                var fileName1 = await awsProvider.UploadFile(chartimage, fileUploadPayload);
                //return File(chartimage.GetBuffer(), "image/png");
            }

            ///
            /// Bar Chart
            ///
            query = new List<dynamic>();
            query.Add(new
            {
                Legend = "Compliant",
                Count = toDoList.Count(l => l.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = toDoList.Count(l => l.AuditStatus != "Compliant")
            });

            chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add("Overall compliance score %");
            chart.Titles[0].Font = new Font("Arial", 14f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisY.Title = "";
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Bar;

            foreach (var q in query)
            {
                chart.Series[0].Color = (q.Legend == "Compliant") ? Color.Green : Color.Red;
                chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
            }
            using (var chartimage = new MemoryStream())
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                var fileUploadPayload = new UploadFilePathObject()
                {
                    BlobContainerName = "Audit",
                    FileName = fileName + "_bar"
                };
                var awsProvider = new AwsS3BucketProvider();
                fileName = await awsProvider.UploadFile(chartimage, fileUploadPayload);
                return fileName;//File(chartimage.GetBuffer(), "image/png");
            }
        }
    }
}
