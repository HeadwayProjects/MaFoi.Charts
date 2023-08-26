using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using MaFoi.Charts.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using System.Collections;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.AspNetCore.Cors;
using Microsoft.Ajax.Utilities;
using static System.Net.WebRequestMethods;

namespace MaFoi.Charts.Controllers
{
      [EnableCors("AllowSpecificOrigin")]
    public class AuditController : Controller
    {
        // GET: Audit
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("GetChart")]
        public async Task<FileContentResult> GetChart(ToDoFilterCriteria criteria)
        {
            var reportData = await GetDataForChart(criteria);
            return File(await Chart(reportData), "image/png");
        }
        class Employee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private Byte[] Chart(string chartTitle, List<dynamic> chartData, SeriesChartType chartType)
        {
            if (SeriesChartType.Pie.Equals(chartType))
                chartData = chartData.FindAll(x => x.Count > 0);
            else
            {
                if (chartTitle.Equals("Overall Rule score %"))
                    chartData = chartData.FindAll(x => x.Ycounts[0] > 0);
                else
                    chartData = chartData.FindAll(x => x.Ycounts[0] > 0 || x.Ycounts[1] > 0);

            }

            var chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
            };

            chart.Titles.Add(chartTitle);
            chart.Titles[0].Font =new Font("Arial", 14f,System.Drawing.FontStyle.Bold);

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


            //chart.Series.Add("");
            // chart.Series[0].ChartType = chartType;

            // List<string> GreenColorLegends = new List<string> { "Compliant", "Audited" };

            if (SeriesChartType.Pie.Equals(chartType))
            {
                chart.Series.Add(new Series("data"));
                chart.Legends.Add(new Legend("Stores"));
                chart.Series[0].ChartType = SeriesChartType.Pie;
                //chart.Series[0]["PieLabelStyle"] = "Outside";
                //chart.Series[0]["PieLineColor"] = "Black";

                chart.Series[0].Label = "(" + "#VALY" + ")" + "#PERCENT{P0}";
                // chart.Series[0].Label = "#PERCENT{P0}";
                chart.Series[0].Points.DataBindXY(
                    chartData.Select(data => data.Legend.ToString()).ToArray(),
                    chartData.Select(data => data.Count).ToArray()
                    );


                chart.Series[0]["PieLabelStyle"] = "inside";
                chart.Series[0].Legend = "Stores";
                chart.Series[0].LegendText = "#VALX";
                chart.Legends["Stores"].Docking = Docking.Right;

                foreach (Series charts in chart.Series)
                {
                    foreach (DataPoint point in charts.Points)
                    {

                        switch (point.AxisLabel)
                        {
                            case "Compliant": point.Color = Color.Green; break;
                            case "Non-Compliance": point.Color = Color.Orange; break;
                            case "Not-Applicable": point.Color = Color.Gray; break;
                            case "Rejected": point.Color = Color.Red; break;
                        }
                        //point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

                    }
                }

                //foreach (var q in chartData)
                //{
                //    chart.Series.Add(new Series(q.Legend));
                //    switch (q.Legend)
                //    {

                //        case "Compliant":
                //            {
                //                chart.Series[q.Legend].Color = Color.Green;
                //                chart.Series[q.Legend].Label = q.Count + "(" + q.Percantage + ")";
                //                chart.Series[q.Legend].Points.DataBind( chartData, q.Legend, Convert.ToDouble(q.Count),q.Percantage);

                //                //chart.Series[0].XValueMember=q.Legend;
                //                //chart.Series[0].Points.Add(Convert.ToDouble(q.Count));
                //                break;
                //            }
                //        case "Non-Compliance":
                //            {
                //                chart.Series[q.Legend].Color = Color.Yellow;
                //                chart.Series[q.Legend].Label = q.Count + "(" + q.Percantage + ")";
                //                chart.Series[q.Legend].Points.DataBind(chartData, q.Legend, Convert.ToDouble(q.Count), q.Percantage);

                //                //chart.Series[0].LabelToolTip = q.Legend;
                //                //chart.Series[0].Points.Add(Convert.ToDouble(q.Count));
                //                break;
                //            }
                //        case "Not-Applicable":
                //            {
                //                chart.Series[q.Legend].Color = Color.Gray;
                //                chart.Series[q.Legend].Label = q.Count + "(" + q.Percantage + ")";
                //                chart.Series[q.Legend].Points.DataBind(chartData, q.Legend, Convert.ToDouble(q.Count), q.Percantage);

                //                //chart.Series[0].LabelToolTip = q.Legend;
                //                //chart.Series[0].Points.AddY(Convert.ToDouble(q.Count));
                //                break;
                //            }
                //        case "Rejected":
                //            {
                //                chart.Series[q.Legend].Color = Color.Red;
                //                chart.Series[q.Legend].Label = q.Count + "(" + q.Percantage + ")";
                //                chart.Series[q.Legend].Points.DataBind(chartData, q.Legend, Convert.ToDouble(q.Count), q.Percantage);

                //                //chart.Series[0].LabelToolTip = q.Legend;
                //                //chart.Series[0].Points.AddY(Convert.ToDouble(q.Count));
                //                break;
                //            }
                //    }
                //    //chart.Series[0].Color = GreenColorLegends.Contains(q.Legend) ? Color.Green : Color.Red;
                //    //chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
                //}
            }
            else
            {

                chart.Legends.Add(new Legend("Stores"));
                if (chartData.Count > 0)
                {
                    foreach (var q in chartData)
                    {
                        chart.Series.Add(new Series(q.Legend));

                        chart.Series[q.Legend].IsValueShownAsLabel = true;
                        chart.Series[q.Legend].ChartType = SeriesChartType.StackedBar;
                        chart.Series[q.Legend].LabelToolTip = q.Legend.ToString();
                        // chart.Series[p].Label = p.ToString();                   
                        chart.Series[q.Legend].Label = "(" + "#VALY" + ") " + "#PERCENT{P0}";
                        chart.Series[q.Legend].LabelAngle = 45;
                        chart.Series[q.Legend].Color = GetColor1(q.Legend.ToString());
                        chart.Series[q.Legend].Points.DataBindXY(q.Xcounts, q.Ycounts);

                        chart.Series[q.Legend]["PieLabelStyle"] = "inside";
                        chart.Series[q.Legend].Legend = "Stores";
                        chart.Series[q.Legend].LegendText = q.Legend.ToString();

                    }
                    chart.Legends["Stores"].Docking = Docking.Right;
                }
                else
                {
                    TextAnnotation ta = new TextAnnotation();
                    ta.Text = "No Data Found";
                    ta.X = 30;  // % of the..
                    ta.Y = 45;  // chart size 
                    ta.Font = new Font("Consolas", 10f);
                    chart.Annotations.Add(ta);
                }
                
            }
            using (var chartimage = new MemoryStream())///////////////////
            {
                chart.SaveImage(chartimage, ChartImageFormat.Png);
                return chartimage.GetBuffer();
            }
        }

        [HttpPost]
        [Route("GetAuditReport")]
        public async Task<FilePathResult> GetAuditReport(ToDoFilterCriteria criteria)
        {
            var doc = new Document(PageSize.LETTER, 2f, 2f, 2f, 2f);
            var pdf = Server.MapPath("Chart") + "/Chart" + Guid.NewGuid() + ".pdf";
            string imagepath = Server.MapPath("Images") + "\\logo.jpg";
            //if (System.IO.File.Exists(pdf))
            //    System.IO.File.Delete(pdf);
            PdfWriter pdfwriter = PdfWriter.GetInstance(doc, new System.IO.FileStream(pdf, System.IO.FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            imagepath = imagepath.Replace("\\Audit", "");
            Image logoimage = Image.GetInstance(imagepath);
            logoimage.Alignment = Image.ALIGN_CENTER;
            logoimage.ScalePercent(40f);
           

            doc.Add(logoimage);
            var reportData = await GetDataForChart(criteria);
        
            List<InternalCompliancesMapping> internalCompliances = new List<InternalCompliancesMapping>();

            
            foreach (var todo in reportData.ToDoList.Where(t=>t.AuditStatus!="Compliant"))
            {
                InternalCompliancesMapping mapping1 = new InternalCompliancesMapping();
                mapping1.InternalCompliance = todo.Act.Name;
                internalCompliances.Add(mapping1);
                InternalCompliancesMapping mapping = new InternalCompliancesMapping();
                mapping.InternalCompliance = todo.Rule.Name;
                    mapping.Risk = "NA";
                    mapping.Status = todo.Status;
                    mapping.Auditstatus = todo.Auditted == "No Audit" ? "No" : "Yes";
                    mapping.Nature = todo.Activity.Type;
                    mapping.Critical = "No";
                    internalCompliances.Add(mapping);

            }
            internalCompliances = internalCompliances.DistinctBy(t => new { t.InternalCompliance, t.Nature, t.Auditstatus, t.Status,t.Risk }).ToList();
            
                

            #region Top Summary            

            PdfPTable tblTopSummary = new PdfPTable(1);
            tblTopSummary.SpacingBefore = 10f;
            tblTopSummary.SpacingAfter = 10f;

            var reportHeading = String.Format("Audit Report for {0}, {1}", reportData.AuditReportSummary.Month, reportData.AuditReportSummary.Year);

            PdfPCell cellHeading = new PdfPCell(new Phrase(reportHeading, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD| iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellHeading.HorizontalAlignment = 1;
            cellHeading.VerticalAlignment = 1;
            cellHeading.Border = 0;
            tblTopSummary.AddCell(cellHeading);

            doc.Add(tblTopSummary);
            #endregion Top Summary
      
            #region Left Summary            

            List<KeyValuePair<string, string>> leftSummaryData = new List<KeyValuePair<string, string>>();
            leftSummaryData.Add(new KeyValuePair<string, string>("Company Name", reportData.AuditReportSummary.Company));
            leftSummaryData.Add(new KeyValuePair<string, string>("Associate Company Name", reportData.AuditReportSummary.AssociateCompany));
            leftSummaryData.Add(new KeyValuePair<string, string>("Location", reportData.AuditReportSummary.Location));
            leftSummaryData.Add(new KeyValuePair<string, string>("Month & Year", string.Format("{0} ({1})", reportData.AuditReportSummary.Month, reportData.AuditReportSummary.Year)));

            float[] tblLeftSummaryWidths = new float[] { 40f, 60f };

            PdfPTable tblLeftSummary = new PdfPTable(tblLeftSummaryWidths);
            tblLeftSummary.SpacingBefore = 20f;
            tblLeftSummary.SpacingAfter = 10f;

            PdfPCell cellLeftSummaryHeading = new PdfPCell(new Phrase("Audit Submitted To", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD |iTextSharp.text.Font.UNDERLINE,  BaseColor.BLACK)));
            cellLeftSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellLeftSummaryHeading.PaddingBottom = 4f;
            cellLeftSummaryHeading.HorizontalAlignment = 1;
            cellLeftSummaryHeading.VerticalAlignment = 1;
            cellLeftSummaryHeading.Colspan = 2;
            cellLeftSummaryHeading.Border = 0;
            tblLeftSummary.AddCell(cellLeftSummaryHeading);

            foreach (var kvp in leftSummaryData)
            {
                PdfPCell cellLeftSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellLeftSummaryContent.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellLeftSummaryContent.HorizontalAlignment = 0;
                cellLeftSummaryContent.VerticalAlignment = 1;
                cellLeftSummaryContent.Border = 0;
                tblLeftSummary.AddCell(cellLeftSummaryContent);

                PdfPCell cellLeftSummaryContent1 = new PdfPCell(new Phrase(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cellLeftSummaryContent1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellLeftSummaryContent1.HorizontalAlignment = 0;
                cellLeftSummaryContent1.VerticalAlignment = 1;
                cellLeftSummaryContent1.Border = 0;
                tblLeftSummary.AddCell(cellLeftSummaryContent1);
            }
            //doc.Add(tblLeftSummary);
            #endregion Left Summary            

            #region Rigbht Summary            

            List<KeyValuePair<string, string>> RightSummaryData = new List<KeyValuePair<string, string>>();
            RightSummaryData.Add(new KeyValuePair<string, string>("Audited", reportData.AuditReportSummary.Audited.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Non-Compliance", reportData.AuditReportSummary.NonCompliance.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Rejected", reportData.AuditReportSummary.Rejected.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Compliant", reportData.AuditReportSummary.Compliant.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Not-Applicable", reportData.AuditReportSummary.NotApplicable.ToString()));

            float[] tblRightSummaryWidths = new float[] { 60f, 40f };

            PdfPTable tblRightSummary = new PdfPTable(tblRightSummaryWidths);
            tblRightSummary.SpacingBefore = 20f;
            tblRightSummary.SpacingAfter = 10f;

            PdfPCell cellRightSummaryHeading = new PdfPCell(new Phrase("Audit Summary", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD|iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellRightSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellRightSummaryHeading.HorizontalAlignment = 1;
            cellRightSummaryHeading.VerticalAlignment = 1;
            cellRightSummaryHeading.Colspan = 2;
            cellRightSummaryHeading.Border = 0;
            cellRightSummaryHeading.PaddingBottom = 4f;
            tblRightSummary.AddCell(cellRightSummaryHeading);

            foreach (var kvp in RightSummaryData)
            {
                PdfPCell cellRightSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellRightSummaryContent.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellRightSummaryContent.HorizontalAlignment = 0;
                cellRightSummaryContent.VerticalAlignment = 1;
                cellRightSummaryContent.Border = 0;
                cellRightSummaryContent.BackgroundColor = GetColor(kvp.Key);
                tblRightSummary.AddCell(cellRightSummaryContent);

                PdfPCell cellRightSummaryContent1 = new PdfPCell(new Phrase(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cellRightSummaryContent1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellRightSummaryContent1.HorizontalAlignment = 0;
                cellRightSummaryContent1.VerticalAlignment = 1;
                cellRightSummaryContent1.Border = 0;
                cellRightSummaryContent1.BackgroundColor = GetColor(kvp.Key);
                tblRightSummary.AddCell(cellRightSummaryContent1);
            }

            #endregion Right Summary            

            PdfPTable tblChartAndSummary = new PdfPTable(2);
            tblChartAndSummary.SpacingBefore = 20f;
            tblChartAndSummary.SpacingAfter = 10f;

            PdfPCell cellLeftSummary = new PdfPCell(tblLeftSummary);
            cellLeftSummary.Border = 0;
            tblChartAndSummary.AddCell(cellLeftSummary);

            PdfPCell cellRightSummary = new PdfPCell(tblRightSummary);
            cellRightSummary.Border = 0;
            cellRightSummary.HorizontalAlignment = 2;
            tblChartAndSummary.AddCell(cellRightSummary);

            doc.Add(tblChartAndSummary);

            var chartData = new List<dynamic>();

            //chartData.Add(new
            //{
            //    Legend = "Audited",
            //    Count = reportData.AuditorPerformance.Audited
            //});
            //chartData.Add(new
            //{
            //    Legend = "Not Audited",
            //    Count = reportData.AuditorPerformance.NotAudited
            //});
            int compliantcount = reportData.ToDoList.Count(l => l.AuditStatus == "Compliant");
            int noncompliantcount = reportData.ToDoList.Count(l => l.AuditStatus == "Non-Compliance");
            int nonapplicablecount = reportData.ToDoList.Count(l => l.AuditStatus == "Not-Applicable");
            int rejectcount = reportData.ToDoList.Count(l => l.Status == "Rejected");
            int total = compliantcount + noncompliantcount + nonapplicablecount + rejectcount;
            chartData.Add(new
            {
                Legend = "Compliant",
                Percantage = ((compliantcount * 100) / total).ToString() + "%",
                Count = reportData.ToDoList.Count(l => l.AuditStatus == "Compliant")

            });
            chartData.Add(new
            {
                Legend = "Non-Compliance",
                Percantage = ((noncompliantcount * 100) / total).ToString() + "%",
                Count = reportData.ToDoList.Count(l => l.AuditStatus == "Non-Compliance")

            });
            chartData.Add(new
            {
                Legend = "Not-Applicable",
                Percantage = ((nonapplicablecount * 100) / total).ToString() + "%",
                Count = reportData.ToDoList.Count(l => l.AuditStatus == "Not-Applicable")
            });
            chartData.Add(new
            {
                Legend = "Rejected",
                Percantage = ((rejectcount * 100) / total).ToString() + "%",
                Count = reportData.ToDoList.Count(l => l.Status == "Rejected")
            });


            var image = Image.GetInstance(Chart("Overall Compliance score %", chartData, SeriesChartType.Pie));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);


            //activity bar chart
            chartData = new List<dynamic>();
            string[] typelst = reportData.ToDoList.Select(t => t.Activity.Type).Distinct().ToArray();
            List<string> auditstatus = new List<string>()
            {
                "Compliant",   "Non-Compliance",     "Not-Applicable",       "Rejected"
            };
            foreach (var p in auditstatus)
            {
                List<int> yvalues = new List<int>();
                List<string> xvalues = new List<string>();

                foreach (var item in typelst)
                {
                    xvalues.Add(item);
                    if (p.Equals("Rejected"))
                        yvalues.Add(reportData.ToDoList.Count(l => l.Activity.Type.Equals(item) && l.Status.Equals(p)));
                    else
                        yvalues.Add(reportData.ToDoList.Count(l => l.Activity.Type.Equals(item) && l.AuditStatus.Equals(p)));
                }
                chartData.Add(new
                {
                    Legend = p,
                    Xcounts = xvalues,
                    Ycounts = yvalues
                });

            }
     

            //chartData.Add(new
            //{
            //    Legend = "Compliant",
            //    Count = reportData.ToDoList.Count(l => l.AuditStatus == "Compliant")
            //});
            //chartData.Add(new
            //{
            //    Legend = "Non Compliant",
            //    Count = reportData.ToDoList.Count(l => l.AuditStatus != "Compliant")
            //});

            image = Image.GetInstance(Chart("Overall Activity score %", chartData, SeriesChartType.StackedBar));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);

            //rule bar chart
            var risklst = reportData.RuleComplianceDetails.Select(r => r.Risk).Distinct().ToList();
            if (risklst.Count == 0)
            {
                risklst = new List<string>() { "Low" };
            }
            chartData = new List<dynamic>();
            foreach (var p in auditstatus)
            {
                List<int> yrulvalues = new List<int>();
                List<string> xRulevalues = new List<string>();
                foreach (var rl in risklst)
                {
                    int i = 0;
                    xRulevalues.Add(rl);
                    if (p.Equals("Rejected"))
                    {
                        var count1 = from t in reportData.ToDoList
                                         // ForumID part removed from both sides: LINQ should do that for you.
                                         // Added "into postsInForum" to get a group join
                                     join r in reportData.RuleComplianceDetails on t.RuleId equals r.RuleId into postsInForum
                                     select new { postcount = postsInForum.Count(r => r.Risk.Equals(rl) && t.Status.Equals(p)) };

                        foreach (var item in count1)
                        {
                            i += item.postcount;
                        }
                        yrulvalues.Add(i);

                    }
                    else
                    {
                        var count = from t in reportData.ToDoList
                                        // ForumID part removed from both sides: LINQ should do that for you.
                                        // Added "into postsInForum" to get a group join
                                    join r in reportData.RuleComplianceDetails on t.RuleId equals r.RuleId into postsInForum
                                    select new { postcount = postsInForum.Count(r => r.Risk.Equals(rl) && t.AuditStatus.Equals(p)) };

                        foreach (var item in count)
                        {
                            i += item.postcount;
                        }
                        yrulvalues.Add(i);
                    }
                }
                chartData.Add(new
                {
                    Legend = p,
                    Xcounts = xRulevalues,
                    Ycounts = yrulvalues
                });

            }

            if (risklst.Count > 0)
            {
                image = Image.GetInstance(Chart("Overall Rule score %", chartData, SeriesChartType.StackedBar));
                image.Alignment = 1;
                image.ScalePercent(75f);
                doc.Add(image);
            }

            ////
            #region ToDos Table

            var columnHeaders = new string[] {"S.No", "Act",
            "Rule",
            "Forms / Registers & Returns",
            "Audit Due Date",
            "Vendor Submitted Date",
            "Status",
            "Compliant",
            "NonCompliance",
            "Not Applicable",
            "Remarks, if any" };

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            PdfPTable tblAuditSummary = new PdfPTable(1);
            tblAuditSummary.SpacingBefore = 60f;
            tblAuditSummary.SpacingAfter = 10f;

            PdfPCell cellaudit = new PdfPCell(new Phrase("Compliance Audit Report", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellaudit.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellaudit.HorizontalAlignment = 1;
            cellaudit.VerticalAlignment = 1;
            cellaudit.Border = 0;
            tblAuditSummary.AddCell(cellaudit);
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(tblAuditSummary);

            float[] tableWidths = new float[] { 40f, 150f, 150f, 150f, 70f, 70f, 60f, 70f, 80f, 70f, 120f };
            PdfPTable table = new PdfPTable(tableWidths);
            table.SpacingBefore = 10f;
            //table.WidthPercentage = 100; //table width to 100per
            //table.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });// width of each column
            foreach (string header in columnHeaders)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                //cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
                //cell.BorderWidthBottom = 1f;
                //cell.BorderWidthTop = 1f;
                //cell.PaddingBottom = 2f;
                //cell.PaddingLeft = 2f;
                //cell.PaddingTop = 2f;
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                table.AddCell(cell);
            }

            ///

            //var resultHtml = "";
            int serialNo = 0;
            foreach (var todo in reportData.ToDoList)
            {

                PdfPCell cCell = new PdfPCell(new Phrase((serialNo + 1).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.Act.Name.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.Rule.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.Activity.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.DueDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.Auditted == "No Audit" ?"NA" : todo.AuditedDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.Status, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((todo.AuditStatus == "Compliant" ? "Y" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((todo.AuditStatus == "Non-Compliance" ? "Y" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((todo.AuditStatus == "Not-Applicable" ? "Y" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((!string.IsNullOrEmpty(todo.AuditRemarks) ? todo.AuditRemarks : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);

                serialNo++;
            }
            
            //table.AddCell("<b>Thank you for visiting</b>");
            //return resultHtml;
          
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(table);
            #endregion ToDos Table

            #region Observations / Recommendations Table

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            PdfPTable tblRecommendations = new PdfPTable(2);
            tblRecommendations.SpacingBefore = 10f;


            PdfPCell cell1 = new PdfPCell(new Phrase("Summary Of Audit Observations", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cell1.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cell1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cell1.HorizontalAlignment = 1;
            cell1.VerticalAlignment = 1;
            cell1.Colspan = 2;
            tblRecommendations.AddCell(cell1);

            PdfPCell cell2 = new PdfPCell(new Phrase("Auditor Observation", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cell2.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cell2.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cell2.HorizontalAlignment = 1;
            cell2.VerticalAlignment = 1;
            cell2.Colspan = 1;
            tblRecommendations.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Auditor Recommendation", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cell3.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cell3.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cell3.HorizontalAlignment = 1;
            cell3.VerticalAlignment = 1;
            cell3.Colspan = 1;
            tblRecommendations.AddCell(cell3);

            ////
            var recommendationsString = string.Join(",\n", reportData.ToDoRecommendations.Select(r => r.Recommendations).ToArray());
            recommendationsString = Regex.Replace(recommendationsString, "<.*?>", string.Empty);
            int sNo = 0;
            var observationsString = "";
            foreach (var todo in reportData.ToDoList.Where(t => t.Status == "Rejected"))
            {
                if (!string.IsNullOrEmpty(todo.AuditRemarks))
                    observationsString += (sNo + 1).ToString() + ") " + todo.AuditRemarks + "\n";
            }
            observationsString = Regex.Replace(observationsString, "<.*?>", string.Empty);
            ///
            PdfPCell cellObservations = new PdfPCell(new Phrase(observationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellObservations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellObservations.HorizontalAlignment = 0;
            cellObservations.Colspan = 1;
            tblRecommendations.AddCell(cellObservations);

            PdfPCell cellRecommendations = new PdfPCell(new Phrase(recommendationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellRecommendations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellRecommendations.HorizontalAlignment = 0;
            cellRecommendations.Colspan = 1;
            tblRecommendations.AddCell(cellRecommendations);

            doc.Add(tblRecommendations);
            #endregion ToDos Table

            //#region Observations / Recommendations Table

            #region List Of Internal Compliances Table

            var internalHeaders = new string[] {"Internal Compliance", "Critical",
            "Risk",
            "Nature",
            "Consider For Score",
            "Status"
             };
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            float[] internaltableWidths = new float[] {250f, 70f, 60f, 100f, 100f, 150f};
            PdfPTable internaltable = new PdfPTable(internaltableWidths);
            internaltable.SpacingBefore = 60f;
            PdfPCell cellinternal = new PdfPCell(new Phrase("List of Activities", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cellinternal.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cellinternal.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellinternal.HorizontalAlignment = 1;
            cellinternal.VerticalAlignment = 1;
            cellinternal.Colspan = 6;
            internaltable.AddCell(cellinternal);
            //table.WidthPercentage = 100; //table width to 100per
            //table.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });// width of each column
            foreach (string header in internalHeaders)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 1;
                internaltable.AddCell(cell);
            }
            foreach (var internalCompliances1 in internalCompliances)
            {
                if (string.IsNullOrEmpty(internalCompliances1.Risk))
                {
                    PdfPCell cCell1 = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell1.Colspan = 6;
                    internaltable.AddCell(cCell1);

                }
                else
                {
                    PdfPCell cCell = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Critical, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Risk, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Nature, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Auditstatus, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Status, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                }

            }
           // doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(internaltable);
            #endregion List Of Internal Compliances Table


            #region Signature

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            PdfPTable tblSignature = new PdfPTable(1);
            tblSignature.SpacingBefore = 10f;
            //tblSignature.HorizontalAlignment = 2;

            PdfPCell cellSignature = new PdfPCell(new Phrase("Audit Submitted By" + reportData.AuditReportSummary.AuditorName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellSignature.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellSignature.HorizontalAlignment = 2;
            cellSignature.VerticalAlignment = 1;
            cellSignature.Border = 0;
            tblSignature.AddCell(cellSignature);

            PdfPCell cellAuditorName = new PdfPCell(new Phrase(reportData.AuditReportSummary.AuditorName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellAuditorName.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellAuditorName.HorizontalAlignment = 2;
            cellAuditorName.VerticalAlignment = 1;
            cellAuditorName.Border = 0;
            tblSignature.AddCell(cellAuditorName);

            doc.Add(tblSignature);

            #endregion Signature

           
            doc.Close();


            return File(pdf, "application/pdf", "Chart.pdf");

        }

        public async Task<AuditReportData> GetDataForChart(ToDoFilterCriteria criteria)
        {
            string Baseurl ="https://apipro.ezycomp.com/api/Auditor/GetAuditReportData"; //"https://localhost:7221/api/Auditor/GetAuditReportData"; 
            AuditReportData reportData = new AuditReportData();
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage() { RequestUri = new Uri(Baseurl) };
                var payload = new ToDoFilterCriteria()
                {
                    Company = criteria.Company,
                    AssociateCompany = criteria.AssociateCompany,
                    Location = criteria.Location,
                    Month = criteria.Month,
                    Year = criteria.Year,
                    Statuses = criteria.Statuses

                    //Company = new Guid("310e6064-510f-4736-acd5-ed82eaca4765"),
                    //AssociateCompany = new Guid("dc93767a-337d-4049-a3ca-21cc0b544afa"),
                    //Location = new Guid("fd2494e1-99ce-48e7-a27c-625c800c3260"),
                    //Month = "April",
                    //Year = 2023,
                    //Statuses = new string[] { "" }
                    //AuditorId = new Guid("a95d7a63-1d19-48c2-8b74-4a28204f15c2")
                };
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var EmpResponse = response.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    reportData = JsonConvert.DeserializeObject<AuditReportData>(EmpResponse);
                }

                ////Passing service base url
                //client.BaseAddress = new Uri(Baseurl);
                //client.DefaultRequestHeaders.Clear();
                ////Define request data format
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ////Sending request to find web api REST service resource GetAllEmployees using HttpClient
                //HttpResponseMessage Res = await client.GetAsync("api/Employee/GetAllEmployees");
                ////Checking the response is successful or not which is sent using HttpClient
                //if (Res.IsSuccessStatusCode)
                //{
                //    //Storing the response details recieved from web api
                //    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                //    //Deserializing the response recieved from web api and storing into the Employee list
                //    toDoList = JsonConvert.DeserializeObject<List<dynamic>>(EmpResponse);
                //}
                //returning the employee list to view
                return reportData;
            }
        }

        public Color GetColor1(string legend)
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
                    legendColor = Color.Orange;
                    break;
            }
            return legendColor;
        }
        public BaseColor GetColor(string legend)
        {
            BaseColor legendColor = BaseColor.BLACK;
            switch (legend)
            {
                case "Audited":
                    legendColor = new BaseColor(Color.Green);
                    break;
                case "Non-Compliance":
                    legendColor = new BaseColor(Color.Gray);
                    break;
                case "Rejected":
                    legendColor = new BaseColor(Color.Red);
                    break;
                case "Compliant":
                    legendColor = new BaseColor(Color.ForestGreen);
                    break;
                case "Not-Applicable":
                    legendColor = new BaseColor(Color.Red);
                    break;
            }
            return legendColor;
        }
        private async Task<Byte[]> Chart(AuditReportData reportData)
        {
            var query = new List<dynamic>();
            query.Add(new
            {
                Legend = "Compliant",
                Count = reportData.ToDoList.Count(l => l.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = reportData.ToDoList.Count(l => l.AuditStatus != "Compliant")
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
                var fileUploadPayload = new UploadFilePathObject()
                {
                    BlobContainerName = "Audit",
                    CompanyId = reportData.AuditReportSummary.Company.ToString(),
                    AssociateCompanyId = reportData.AuditReportSummary.AssociateCompany.ToString(),
                    LocationId = reportData.AuditReportSummary.Location.ToString(),
                    Year = reportData.AuditReportSummary.Year,
                    Month = reportData.AuditReportSummary.Month,
                    FileName = Guid.NewGuid().ToString()
                };
                var awsProvider = new AwsS3BucketProvider();
                var fileName = await awsProvider.UploadFile(chartimage, fileUploadPayload);
                return chartimage.GetBuffer();
            }
        }

        
    }
   

}