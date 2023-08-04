using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.DataVisualization.Charting;
using iTextSharp.text;
using MaFoi.Charts.Models;
using static iTextSharp.text.pdf.AcroFields;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
//using Plotly.NET;
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
                Legend = "Non-Compliance",
                Count = toDoList.Count(l => l.AuditStatus == "Non-Compliance")
            });
            query.Add(new
            {
                Legend = "Not-Applicable",
                Count = toDoList.Count(l => l.AuditStatus == "Not-Applicable")
            });
            query.Add(new
            {
                Legend = "Rejected",
                Count = toDoList.Count(l => l.Status == "Rejected")
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

                switch (q.Legend)
                {

                    case "Compliant":
                        {
                            chart.Series[0].Color = Color.Green;
                            chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
                            //chart.Series[0].XValueMember=q.Legend;
                            //chart.Series[0].Points.Add(Convert.ToDouble(q.Count));
                            break;
                        }
                    case "Non-Compliance":
                        {
                            chart.Series[0].Color = Color.Yellow;
                            chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
                            //chart.Series[0].LabelToolTip = q.Legend;
                            //chart.Series[0].Points.Add(Convert.ToDouble(q.Count));
                            break;
                        }
                    case "Not-Applicable":
                        {
                            chart.Series[0].Color = Color.Gray;
                            chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
                            //chart.Series[0].LabelToolTip = q.Legend;
                            //chart.Series[0].Points.AddY(Convert.ToDouble(q.Count));
                            break;
                        }
                    case "Rejected":
                        {
                            chart.Series[0].Color = Color.Red;
                            chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
                            //chart.Series[0].LabelToolTip = q.Legend;
                            //chart.Series[0].Points.AddY(Convert.ToDouble(q.Count));
                            break;
                        }
                }

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
            var typelst = toDoList.Select(t => t.Activity.Type).Distinct().ToList();
            //foreach (var item in typelst)
            //{

            //    query.Add(new
            //    {
            //        Legend = item,
            //        Count = new[] { Convert.ToDouble(toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Compliant")),
            //                        Convert.ToDouble(toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Non-Compliance")),
            //                       Convert.ToDouble( toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Not-Applicable")),
            //                        Convert.ToDouble(toDoList.Count(l => l.Activity.Type.Equals(item) && l.Status == "Rejected"))
            //        }
            //    });

            //}
            //query.Add(new
            //{
            //    Legend = "occurace",
            //    Count = new[] { Convert.ToDouble(toDoList.Count(l => l.AuditStatus == "Compliant")),
            //                        Convert.ToDouble(toDoList.Count(l =>l.AuditStatus == "Non-Compliance")),
            //                       Convert.ToDouble( toDoList.Count(l => l.AuditStatus == "Not-Applicable")),
            //                        Convert.ToDouble(toDoList.Count(l => l.Status == "Rejected"))
            //        }
            //});


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

            List<string> auditstatus = new List<string>()
            {
                "Compliant",
                "Non-Compliance",
                "Not-Applicable",
                "Rejected"
            };
            ////chart.Series.Add("");
            //foreach (var q in query)
            //{
            //    chart.Series.Add(new Series(q.Legend));
            //    chart.Series[q.Legend].IsValueShownAsLabel = true;
            //    chart.Series[q.Legend].ChartType = SeriesChartType.StackedBar;
            //    chart.Series[q.Legend].Points.DataBindXY(auditstatus, q.Count);
            //}


            // chart.Series[0].ChartType = SeriesChartType.Bar;


            //string[] typelst = toDoList.Select(t => t.Activity.Type).Distinct().ToArray();

            foreach (var p in auditstatus)
            {
                List<int> yvalues = new List<int>();
                List<string> xvalues = new List<string>();

                foreach (var item in typelst)
                {
                    xvalues.Add(item);
                    if (p.Equals("Rejected"))
                        yvalues.Add(toDoList.Count(l => l.Activity.Type.Equals(item) && l.Status.Equals(p)));
                    else
                        yvalues.Add(toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus.Equals(p)));
                }

                chart.Series.Add(new Series(p));
                chart.Series[p].IsValueShownAsLabel = true;
                chart.Series[p].ChartType = SeriesChartType.StackedBar;
                chart.Series[p].LabelToolTip = p.ToString();
                // chart.Series[p].Label = p.ToString();
                chart.Series[p].Color = GetColor(p.ToString());
                chart.Series[p].Points.DataBindXY(xvalues, yvalues);

            }
            //foreach (var item in auditstatus)
            //{


            //    string[] x = typelst;
            //    //Get the Total of Orders for each Country.
            //    int[] y = new[] {
            //        toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Compliant"),
            //        toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Non-Compliance"),
            //        toDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus == "Not-Applicable"),
            //        toDoList.Count(l => l.Activity.Type.Equals(item) && l.Status == "Rejected")
            //    };


            //Add Series to the Chart.
            //   chart.Series.Add(new Series(item));
            //chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].ChartType = SeriesChartType.StackedBar;
            //chart.Series[0].Points.DataBind(query,xField:);
            //chart.Series[q.Legend].Color = Color.Red;
            //chart.Series[q.Legend].Points.AddXY(q.Legend,q.Count);


            // }
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

        public Color GetColor(string legend)
        {
            Color legendColor = Color.White;
            switch (legend)
            {
                case "Audited":
                    legendColor = Color.Green;
                    break;
                case "Non-Compliance":
                    legendColor = Color.Gray;
                    break;
                case "Rejected":
                    legendColor = Color.Red;
                    break;
                case "Compliant":
                    legendColor = Color.ForestGreen;
                    break;
                case "Not-Applicable":
                    legendColor = Color.Yellow;
                    break;
            }
            return legendColor;
        }
    }
}
