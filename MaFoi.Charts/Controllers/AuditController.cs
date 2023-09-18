using iTextSharp.text;
using iTextSharp.text.pdf;
using MaFoi.Charts.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
namespace MaFoi.Charts.Controllers
{

    [AllowCrossSite]
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
            if (SeriesChartType.Pie.Equals(chartType)  || SeriesChartType.Doughnut.Equals(chartType))
                chartData = chartData.FindAll(x => x.Count > 0);
            else
            {
                if (chartTitle.Equals("5.Overall Rule Score %"))
                    chartData = chartData.FindAll(x => x.Ycounts[0] > 0);
                else
                    chartData = chartData.FindAll(x => x.Ycounts[0] > 0 || x.Ycounts[1] > 0);

            }

            var chart = new Chart
            {
                Width = 800,
                Height = 300,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
            };

            chart.Titles.Add(chartTitle);
            chart.Titles[0].Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            chart.Titles[0].Font = new Font("Arial", 14f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisY.Title = "";
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 6f);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8f);
            chart.ChartAreas[0].AxisY.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;


            //chart.Series.Add("");
            // chart.Series[0].ChartType = chartType;

            // List<string> GreenColorLegends = new List<string> { "Compliant", "Audited" };

            if (SeriesChartType.Pie.Equals(chartType) || SeriesChartType.Doughnut.Equals(chartType))
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
                            case "Compliant": point.Color = Color.MediumSeaGreen; break;
                            case "Non-Compliance": point.Color = Color.Red; break;
                            case "Not-Applicable": point.Color = Color.Gray; break;
                            case "Rejected": point.Color = Color.Yellow; break;
                        }
                        //point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

                    }
                }

                
            }
            else
            {

                chart.Legends.Add(new Legend("Stores"));
                if (chartData.Count > 0)
                {
                    chart.Width = 800;
                    chart.Height = 500;
                    

                    foreach (var q in chartData)
                    {
                        chart.Series.Add(new Series(q.Legend));

                        chart.Series[q.Legend].IsValueShownAsLabel = true;
                        chart.Series[q.Legend].ChartType = SeriesChartType.Bar;
                        chart.Series[q.Legend].LabelToolTip = q.Legend.ToString();
                        // chart.Series[p].Label = p.ToString();                   
                        chart.Series[q.Legend].Label = "(" + "#VALY" + ") " + "#PERCENT{P0}";
                        //chart.Series[q.Legend].LabelAngle = 45;
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
            pdfwriter.PageEvent = new PageNumberEventHandler();

            doc.Open();

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));
            imagepath = imagepath.Replace("\\Audit", "");
            Image logoimage = Image.GetInstance(imagepath);
            logoimage.Alignment = Image.ALIGN_CENTER;
            logoimage.ScalePercent(15f);


            doc.Add(logoimage);
            var reportData = await GetDataForChart(criteria);

            
            
            
            List<InternalCompliancesMapping> internalCompliances = new List<InternalCompliancesMapping>();

            List<InternalCompliancesMapping> NoAuditinternalCompliances = new List<InternalCompliancesMapping>();
            foreach (var todo in reportData.ToDoList)
            {
                if(todo.ToDo.Auditted== "No Audit")
                {
                    InternalCompliancesMapping mapping1 = new InternalCompliancesMapping();
                    mapping1.InternalCompliance = todo.ToDo.Act.Name;
                    internalCompliances.Add(mapping1);
                    InternalCompliancesMapping mapping = new InternalCompliancesMapping();
                    mapping.InternalCompliance = todo.ToDo.Rule.Name;
                    mapping.Risk = todo.RuleComplianceDetails.Risk;
                    mapping.Status = todo.ToDo.AuditStatus;
                    mapping.Auditstatus = todo.ToDo.Auditted == "No Audit" ? "No" : "Yes";
                    mapping.Nature = todo.ToDo.Activity.Type;
                    mapping.AuditType = todo.ToDo.Auditted.ToString();
                    string Impriosonment = todo.RuleComplianceDetails.Impriosonment == true ? "Imprisonment" : "";
                    string ContinuingPenalty = todo.RuleComplianceDetails.ContinuingPenalty == true ? "ContinuingPenalty" : "";
                    string CancellationSuspensionOfLicense = todo.RuleComplianceDetails.CancellationSuspensionOfLicense == true ? "CancellationSuspensionOfLicense" : "";

                    Dictionary<string, string> myDictionary = new Dictionary<string, string>
                            {
            {"Complaince Description", todo.RuleComplianceDetails.ComplianceDescription },
                        { "Risk:" ,todo.RuleComplianceDetails.Risk },
                         {"AuditType" , todo.RuleComplianceDetails.AuditType },
                        {"StatutoryAuthority" ,todo.RuleComplianceDetails.StatutoryAuthority },
                        {"Proof of Complaince:" , todo.RuleComplianceDetails.ProofOfCompliance },
                        { "Penalty" , todo.RuleComplianceDetails.Penalty },
                        {"MaximumPenalityAmount" ,todo.RuleComplianceDetails.MaximumPenaltyAmount.ToString() },
                     { "CheckboxValues" , todo.RuleComplianceDetails.ComplianceName + "," + Impriosonment + "," + ContinuingPenalty + "," + CancellationSuspensionOfLicense }

        };
                    mapping.ImpectDetails = myDictionary;


                    NoAuditinternalCompliances.Add(mapping);
                }

                else if ((todo.ToDo.AuditStatus == "Non-Compliance" || todo.ToDo.Status == "Rejected") && todo.ToDo.Auditted!= "No Audit")
                {

                    InternalCompliancesMapping mapping1 = new InternalCompliancesMapping();
                    mapping1.InternalCompliance = todo.ToDo.Act.Name;
                    internalCompliances.Add(mapping1);
                    InternalCompliancesMapping mapping = new InternalCompliancesMapping();
                    mapping.InternalCompliance = todo.ToDo.Rule.Name;
                    mapping.Risk = todo.RuleComplianceDetails.Risk;
                    mapping.Status = todo.ToDo.AuditStatus;
                    mapping.Auditstatus = todo.ToDo.Auditted == "No Audit" ? "No" : "Yes";
                    mapping.Nature = todo.ToDo.Activity.Type;
                    mapping.AuditType = todo.ToDo.Auditted.ToString();
                    string Impriosonment = todo.RuleComplianceDetails.Impriosonment== true ? "Imprisonment" : "";
                    string ContinuingPenalty = todo.RuleComplianceDetails.ContinuingPenalty == true ? "ContinuingPenalty" : "";
                    string CancellationSuspensionOfLicense = todo.RuleComplianceDetails.CancellationSuspensionOfLicense == true ? "CancellationSuspensionOfLicense" : "";

                    Dictionary<string, string> myDictionary = new Dictionary<string, string>
                            {
            {"Complaince Description", todo.RuleComplianceDetails.ComplianceDescription },
                        { "Risk:" ,todo.RuleComplianceDetails.Risk },
                         {"AuditType" , todo.RuleComplianceDetails.AuditType },
                        {"StatutoryAuthority" ,todo.RuleComplianceDetails.StatutoryAuthority },
                        {"Proof of Complaince:" , todo.RuleComplianceDetails.ProofOfCompliance },
                        { "Penalty" , todo.RuleComplianceDetails.Penalty },
                        {"MaximumPenalityAmount" ,todo.RuleComplianceDetails.MaximumPenaltyAmount.ToString() },
                     { "CheckboxValues" , todo.RuleComplianceDetails.ComplianceName + "," + Impriosonment + "," + ContinuingPenalty + "," + CancellationSuspensionOfLicense }

        };
                    mapping.ImpectDetails = myDictionary;


                    internalCompliances.Add(mapping);
                }

            }
            internalCompliances = internalCompliances.DistinctBy(t => new { t.InternalCompliance, t.Nature, t.Auditstatus, t.Status, t.Risk }).ToList();



            #region Top Summary            

            PdfPTable tblTopSummary = new PdfPTable(1);
            tblTopSummary.SpacingBefore = 10f;
            tblTopSummary.SpacingAfter = 10f;

            var reportHeading = String.Format(reportData.AuditReportSummary.Company+" Audit Report for {0}, {1}", reportData.AuditReportSummary.Month, reportData.AuditReportSummary.Year);

            PdfPCell cellHeading = new PdfPCell(new Phrase(reportHeading, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellHeading.HorizontalAlignment = 1;
            cellHeading.VerticalAlignment = 1;
            cellHeading.Border = 0;
            tblTopSummary.AddCell(cellHeading);

            doc.Add(tblTopSummary);
            #endregion Top Summary

            #region Left Summary            

            Paragraph companydetails = new Paragraph(new Chunk("Associate Company Name:", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            companydetails.Add(new Chunk(reportData.AuditReportSummary.AssociateCompany, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            companydetails.Alignment = Element.ALIGN_LEFT;
            
            Paragraph location = new Paragraph(new Chunk("Location:", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            location.Add(new Chunk(reportData.AuditReportSummary.Location, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            location.Alignment = Element.ALIGN_RIGHT;
            location.PaddingTop=-20f;

           // doc.Add(location);
            //List<KeyValuePair<string, string>> leftSummaryData = new List<KeyValuePair<string, string>>();
            //leftSummaryData.Add(new KeyValuePair<string, string>("Company Name", reportData.AuditReportSummary.Company));
            //leftSummaryData.Add(new KeyValuePair<string, string>("Associate Company Name", reportData.AuditReportSummary.AssociateCompany));
            //leftSummaryData.Add(new KeyValuePair<string, string>("Location", reportData.AuditReportSummary.Location));
            //leftSummaryData.Add(new KeyValuePair<string, string>("Month & Year", string.Format("{0} ({1})", reportData.AuditReportSummary.Month, reportData.AuditReportSummary.Year)));

            float[] tblLeftSummaryWidths = new float[] { 400f,200f,200f, 300f };

            PdfPTable tblLeftSummary = new PdfPTable(tblLeftSummaryWidths);
            tblLeftSummary.SpacingBefore = 20f;
            tblLeftSummary.SpacingAfter = 10f;
            tblLeftSummary.DefaultCell.Border = 0;
            tblLeftSummary.AddCell(companydetails);
            tblLeftSummary.AddCell("");
            tblLeftSummary.AddCell("");
            tblLeftSummary.AddCell(location);
            doc.Add(tblLeftSummary);
            //PdfPCell cellLeftSummaryHeading = new PdfPCell(new Phrase("1.Audit Submitted To", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            //cellLeftSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cellLeftSummaryHeading.PaddingBottom = 4f;
            //cellLeftSummaryHeading.HorizontalAlignment = 1;
            //cellLeftSummaryHeading.VerticalAlignment = 1;
            //cellLeftSummaryHeading.Colspan = 2;
            //cellLeftSummaryHeading.Border = 0;
            //tblLeftSummary.AddCell(cellLeftSummaryHeading);

            //foreach (var kvp in leftSummaryData)
            //{
            //    PdfPCell cellLeftSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //    cellLeftSummaryContent.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //    cellLeftSummaryContent.HorizontalAlignment = 0;
            //    cellLeftSummaryContent.VerticalAlignment = 1;
            //    cellLeftSummaryContent.Border = 0;
            //    tblLeftSummary.AddCell(cellLeftSummaryContent);

            //    PdfPCell cellLeftSummaryContent1 = new PdfPCell(new Phrase(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //    cellLeftSummaryContent1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //    cellLeftSummaryContent1.HorizontalAlignment = 0;
            //    cellLeftSummaryContent1.VerticalAlignment = 1;
            //    cellLeftSummaryContent1.Border = 0;
            //    tblLeftSummary.AddCell(cellLeftSummaryContent1);
            //}
            //doc.Add(tblLeftSummary);
            #endregion Left Summary            

            #region Rigbht Summary            

            List<KeyValuePair<string, string>> RightSummaryData = new List<KeyValuePair<string, string>>();
            RightSummaryData.Add(new KeyValuePair<string, string>("Audited", reportData.AuditReportSummary.Audited.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Non-Compliance", reportData.AuditReportSummary.NonCompliance.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Rejected", reportData.AuditReportSummary.Rejected.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Compliant", reportData.AuditReportSummary.Compliant.ToString()));
            RightSummaryData.Add(new KeyValuePair<string, string>("Not-Applicable", reportData.AuditReportSummary.NotApplicable.ToString()));

            float[] tblRightSummaryWidths = new float[] { 150f, 50f };

            PdfPTable tblRightSummary = new PdfPTable(tblRightSummaryWidths);
            tblRightSummary.SpacingBefore = 20f;
            tblRightSummary.SpacingAfter = 5f;

            PdfPCell cellRightSummaryHeading = new PdfPCell(new Phrase("2.Audit Summary", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellRightSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellRightSummaryHeading.HorizontalAlignment = 1;
            cellRightSummaryHeading.VerticalAlignment = 1;
            cellRightSummaryHeading.Colspan = 2;
            cellRightSummaryHeading.Border = 0;
            cellRightSummaryHeading.PaddingBottom = 10f;
            tblRightSummary.AddCell(cellRightSummaryHeading);

            foreach (var kvp in RightSummaryData)
            {
                PdfPCell cellRightSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cellRightSummaryContent.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellRightSummaryContent.HorizontalAlignment = 0;
                cellRightSummaryContent.VerticalAlignment = 1;
                cellRightSummaryContent.Border = 0;
                cellRightSummaryContent.BackgroundColor = GetColor(kvp.Key);
                tblRightSummary.AddCell(cellRightSummaryContent);

                PdfPCell cellRightSummaryContent1 = new PdfPCell(new Phrase(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                cellRightSummaryContent1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cellRightSummaryContent1.HorizontalAlignment = 0;
                cellRightSummaryContent1.VerticalAlignment = 1;
                cellRightSummaryContent1.Border = 0;
                cellRightSummaryContent1.BackgroundColor = GetColor(kvp.Key);
                tblRightSummary.AddCell(cellRightSummaryContent1);
            }

            #endregion Right Summary            

            PdfPTable tblChartAndSummary = new PdfPTable(1);
            tblChartAndSummary.SpacingBefore = 20f;
            tblChartAndSummary.SpacingAfter = 10f;

            
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
            int compliantcount = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Compliant");
            int noncompliantcount = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Non-Compliance");
           // int nonapplicablecount = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Not-Applicable");
            int rejectcount = reportData.ToDoList.Count(l => l.ToDo.Status == "Rejected");
            int total = compliantcount + noncompliantcount  + rejectcount;



            chartData.Add(new
            {
                Legend = "Compliant",
                Percantage = Convert.ToDecimal(((double)(compliantcount * 100) / total)),
                Count = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Compliant")

            });
            chartData.Add(new
            {
                Legend = "Non-Compliance",
                Percantage = Convert.ToDecimal(((double)(noncompliantcount * 100) / total)),
                Count = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Non-Compliance")

            });
          
            chartData.Add(new
            {
                Legend = "Rejected",
                Percantage = Convert.ToDecimal(((double)(rejectcount * 100) / total)),
                Count = reportData.ToDoList.Count(l => l.ToDo.Status == "Rejected")
            });


            var image = Image.GetInstance(Chart("3.Overall Compliance Score %", chartData, SeriesChartType.Pie));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);
            doc.Add(new Paragraph(""));
            // Overall Compliance Score table
            PdfPTable overallcompliance = new PdfPTable(3);
            overallcompliance.SpacingBefore = 20f;
            overallcompliance.SpacingAfter = 10f;
            PdfPCell overallheading = new PdfPCell(new Phrase("Overall Compliance Score", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallheading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            overallheading.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
            overallheading.HorizontalAlignment = 1;
            overallheading.VerticalAlignment = 1;
            overallheading.Colspan = 3;
            //overallheading.Border = 0;
            overallheading.PaddingBottom = 10f;
            overallcompliance.AddCell(overallheading);
      string [] columnHeaderscompliance= new string[] { "Category", "Count", "Percentage" };
            foreach (string header in columnHeaderscompliance)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                overallcompliance.AddCell(cell);
            }
            decimal totalpercentage = 0;
            foreach (var data in chartData)
            {
                PdfPCell cCell = new PdfPCell(new Phrase(data.Legend, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcompliance.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(data.Count.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcompliance.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(Math.Round(data.Percantage,2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcompliance.AddCell(cCell);
                totalpercentage= totalpercentage+Convert.ToDecimal(data.Percantage);
            }
            PdfPCell cCell2 = new PdfPCell(new Phrase("Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcompliance.AddCell(cCell2);
            cCell2 = new PdfPCell(new Phrase(total.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcompliance.AddCell(cCell2);
            cCell2 = new PdfPCell(new Phrase(Math.Round(totalpercentage,2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcompliance.AddCell(cCell2);

            doc.Add(overallcompliance);
            doc.Add(new Paragraph(""));
            //Audited type  status -audit
            PdfPTable typeimages = new PdfPTable(3);
            typeimages.DefaultCell.BorderWidth = 0;
            PdfPCell auidtcell = new PdfPCell();
            PdfPCell physicalcell = new PdfPCell();
            PdfPCell noauditcell = new PdfPCell();
            int auditcompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted=="Audit" && l.ToDo.AuditStatus == "Compliant");
            int auditnoncompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Audit" && l.ToDo.AuditStatus == "Non-Compliance");
           // int auditnonapplicablecount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Audit" && l.ToDo.AuditStatus == "Not-Applicable");
            int auditrejectcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Audit" && l.ToDo.Status == "Rejected");
            int audittotal = auditcompliantcount + auditnoncompliantcount  + auditrejectcount;
            var auditchartdata =new List<dynamic>();
            auditchartdata.Add(new
            {
                Legend = "Compliant",
                Percantage = ((auditcompliantcount * 100) / audittotal).ToString() + "%",
                Count = auditcompliantcount

            });
            auditchartdata.Add(new
            {
                Legend = "Non-Compliance",
                Percantage = ((auditnoncompliantcount * 100) / audittotal).ToString() + "%",
                Count = auditnoncompliantcount

            });
           
            auditchartdata.Add(new
            {
                Legend = "Rejected",
                Percantage = ((auditrejectcount * 100) / audittotal).ToString() + "%",
                Count =auditrejectcount
            });


            var auditimage = Image.GetInstance(Chart("Audit Type:Audit", auditchartdata, SeriesChartType.Doughnut));
            auditimage.Alignment = 1;
            auditimage.ScalePercent(30f);
            //doc.Add(auditimage);
            auidtcell.AddElement(auditimage);
            auidtcell.BorderWidth = 0;
           // cell1.Width = 30f;
           // cell1.Colspan = 1;
            //Audited type  status -physical audit

            int Physicalauditcompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Physical Audit" && l.ToDo.AuditStatus == "Compliant");
            int physicalauditnoncompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Physical Audit" && l.ToDo.AuditStatus == "Non-Compliance");
          //  int physicalauditnonapplicablecount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Physical Audit" && l.ToDo.AuditStatus == "Not-Applicable");
            int physicalauditrejectcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "Physical Audit" && l.ToDo.Status == "Rejected");
            int physicalaudittotal = Physicalauditcompliantcount + physicalauditnoncompliantcount  + physicalauditrejectcount;
            var physicalauditchartdata = new List<dynamic>();
            physicalauditchartdata.Add(new
            {
                Legend = "Compliant",
                Percantage = ((Physicalauditcompliantcount * 100) / physicalaudittotal).ToString() + "%",
                Count = Physicalauditcompliantcount

            });
            physicalauditchartdata.Add(new
            {
                Legend = "Non-Compliance",
                Percantage = ((physicalauditnoncompliantcount * 100) / physicalaudittotal).ToString() + "%",
                Count = physicalauditnoncompliantcount

            });
           

            physicalauditchartdata.Add(new
            {
                Legend = "Rejected",
                Percantage = ((physicalauditrejectcount * 100) / physicalaudittotal).ToString() + "%",
                Count = physicalauditrejectcount
            });


            var physicalauditimage = Image.GetInstance(Chart("Audit Type:Physical Audit", physicalauditchartdata, SeriesChartType.Doughnut));
            physicalauditimage.Alignment = 1;
            physicalauditimage.ScalePercent(30f);
            // doc.Add(physicalauditimage);
            physicalcell.AddElement(physicalauditimage);
            physicalcell.BorderWidth = 0;
           // cell2.Width = 30f;
          //  cell2.Colspan = 1;
            //
            //Audited type  status -No audit

            int Noauditcompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "No Audit" && l.ToDo.AuditStatus == "Compliant");
            int Noauditnoncompliantcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "No Audit" && l.ToDo.AuditStatus == "Non-Compliance");
           // int Noauditnonapplicablecount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "No Audit" && l.ToDo.AuditStatus == "Not-Applicable");
            int Noauditrejectcount = reportData.ToDoList.Count(l => l.ToDo.Auditted == "No Audit" && l.ToDo.Status == "Rejected");
            int Noaudittotal = Noauditcompliantcount + Noauditnoncompliantcount  + auditrejectcount;
            var Noauditchartdata = new List<dynamic>();
            Noauditchartdata.Add(new
            {
                Legend = "Compliant",
                Percantage = ((Noauditcompliantcount * 100) / Noaudittotal).ToString() + "%",
                Count = Noauditcompliantcount

            });
            Noauditchartdata.Add(new
            {
                Legend = "Non-Compliance",
                Percantage = ((Noauditnoncompliantcount * 100) / Noaudittotal).ToString() + "%",
                Count = Noauditnoncompliantcount

            });
           
            Noauditchartdata.Add(new
            {
                Legend = "Rejected",
                Percantage = ((auditrejectcount * 100) / Noaudittotal).ToString() + "%",
                Count = auditrejectcount
            });


            var Noauditimage = Image.GetInstance(Chart("Audit Type:No Audit", Noauditchartdata, SeriesChartType.Doughnut));
            Noauditimage.Alignment = 1;
            Noauditimage.ScalePercent(30f);
            //doc.Add(Noauditimage);
            noauditcell.AddElement(Noauditimage);
            noauditcell.BorderWidth = 0;
           
            typeimages.AddCell(auidtcell);
            typeimages.AddCell(physicalcell);
            typeimages.AddCell(noauditcell);
            

            doc.Add(typeimages);
            //activity bar chart
            chartData = new List<dynamic>();
            string[] typelst = reportData.ToDoList.Select(t => t.ToDo.Activity.Type).Distinct().ToArray();
            List<string> auditstatus = new List<string>()
            {
                "Compliant",   "Non-Compliance",  "Rejected"
            };
            foreach (var p in auditstatus)
            {
                List<int> yvalues = new List<int>();
                List<string> xvalues = new List<string>();

                foreach (var item in typelst)
                {
                    xvalues.Add(item);
                    if (p.Equals("Rejected"))
                        yvalues.Add(reportData.ToDoList.Count(l => l.ToDo.Activity.Type.Equals(item) && l.ToDo.Status.Equals(p)));
                    else
                        yvalues.Add(reportData.ToDoList.Count(l => l.ToDo.Activity.Type.Equals(item) && l.ToDo.AuditStatus.Equals(p)));
                }
                chartData.Add(new
                {
                    Legend = p,
                    Xcounts = xvalues,
                    Ycounts = yvalues
                });

            }


            

            image = Image.GetInstance(Chart("4.Overall Activity Score %", chartData, SeriesChartType.Bar));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);
            #region Overall Activity Score table
            PdfPTable overallcactivity = new PdfPTable(3);
            overallcactivity.SpacingBefore = 20f;
            overallcactivity.SpacingAfter = 10f;
            PdfPCell activityheading = new PdfPCell(new Phrase("Overall Activity Score", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            activityheading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            activityheading.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
            activityheading.HorizontalAlignment = 1;
            activityheading.VerticalAlignment = 1;
            activityheading.Colspan = 3;
            //overallheading.Border = 0;
            activityheading.PaddingBottom = 10f;
            overallcactivity.AddCell(activityheading);
            
            foreach (string header in columnHeaderscompliance)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                overallcactivity.AddCell(cell);
            }
            double totalactivityper = 0.0;
            int totalactivity = 0;
            foreach (var data in chartData)
            {
                int SUM = 0;
                foreach(int VAL in data.Ycounts)
                {
                    SUM += VAL;
                }
                totalactivity = totalactivity + SUM;
            }

            foreach (var data in chartData)
            {
                PdfPCell cCell = new PdfPCell(new Phrase(data.Legend, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcactivity.AddCell(cCell);
                int sum = 0;
                foreach (int VAL in data.Ycounts)
                {
                    sum += VAL;
                }
                cCell = new PdfPCell(new Phrase(sum.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcactivity.AddCell(cCell);
                  double percent= ((double)(sum*100) / totalactivity);
                cCell = new PdfPCell(new Phrase(Math.Round(percent,2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                overallcactivity.AddCell(cCell);
                totalactivityper = totalactivityper + percent;
            }
            PdfPCell activitycell2 = new PdfPCell(new Phrase("Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcactivity.AddCell(activitycell2);
            activitycell2 = new PdfPCell(new Phrase(total.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcactivity.AddCell(activitycell2);
            activitycell2 = new PdfPCell(new Phrase(Math.Round(totalactivityper, 2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            overallcactivity.AddCell(activitycell2);

            doc.Add(overallcactivity);
            #endregion end overal activity
            //rule bar chart
            var risklst = reportData.ToDoList.Select(r => r.RuleComplianceDetails.Risk).Distinct().ToList();
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

                        var count1 = (from t in reportData.ToDoList.Where(r=>r.RuleComplianceDetails.Risk.Equals(rl) && r.ToDo.AuditStatus.Equals(p)) select t).Count();

                        //var count1 = from t in reportData.ToDoList
                        //                 // ForumID part removed from both sides: LINQ should do that for you.
                        //                 // Added "into postsInForum" to get a group join
                        //             join r in reportData.RuleComplianceDetails on t.RuleId equals r.RuleId into postsInForum
                        //             select new { postcount = postsInForum.Count(r => r.Risk.Equals(rl) && t.Status.Equals(p)) };

                       
                        yrulvalues.Add(count1);

                    }
                    else
                    {

                        var count = (from t in reportData.ToDoList.Where(r => r.RuleComplianceDetails.Risk.Equals(rl) && r.ToDo.AuditStatus.Equals(p)) select t).Count();


                        //var count = from t in reportData.ToDoList
                        //                // ForumID part removed from both sides: LINQ should do that for you.
                        //                // Added "into postsInForum" to get a group join
                        //             join r in reportData.RuleComplianceDetails on t.RuleId equals r.RuleId into postsInForum
                        //            select new { postcount = postsInForum.Count(r => r.Risk.Equals(rl) && t.AuditStatus.Equals(p)) };

                        yrulvalues.Add(count);
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
                doc.NewPage();

                image = Image.GetInstance(Chart("5.Overall Rule Score %", chartData, SeriesChartType.Bar));
                image.Alignment = 1;
                image.ScalePercent(75f);
                doc.Add(image);

                #region Overall Rule Score table
                PdfPTable overallrule = new PdfPTable(3);
                overallrule.SpacingBefore = 20f;
                overallrule.SpacingAfter = 10f;
                PdfPCell ruleheading = new PdfPCell(new Phrase("Overall Rule Score", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                ruleheading.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                ruleheading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                ruleheading.HorizontalAlignment = 1;
                ruleheading.VerticalAlignment = 1;
                ruleheading.Colspan = 3;
                //overallheading.Border = 0;
                ruleheading.PaddingBottom = 10f;
                overallrule.AddCell(ruleheading);

                foreach (string header in columnHeaderscompliance)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                    cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    overallrule.AddCell(cell);
                }
                double totalruleper = 0.0;
                int totalrule = 0;
                foreach (var data in chartData)
                {
                    int SUM = 0;
                    foreach (int VAL in data.Ycounts)
                    {
                        SUM += VAL;
                    }
                    totalrule = totalrule + SUM;
                }

                foreach (var data in chartData)
                {
                    PdfPCell cCell = new PdfPCell(new Phrase(data.Legend, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    overallrule.AddCell(cCell);
                    int sum = 0;
                    foreach (int VAL in data.Ycounts)
                    {
                        sum += VAL;
                    }
                    cCell = new PdfPCell(new Phrase(sum.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    overallrule.AddCell(cCell);
                    double percent = ((double)(sum * 100) / totalrule);
                    cCell = new PdfPCell(new Phrase(Math.Round(percent, 2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    overallrule.AddCell(cCell);
                    totalruleper = totalruleper + percent;
                }
                PdfPCell rulecell2 = new PdfPCell(new Phrase("Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                overallrule.AddCell(rulecell2);
                rulecell2 = new PdfPCell(new Phrase(total.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                overallrule.AddCell(rulecell2);
                rulecell2 = new PdfPCell(new Phrase(Math.Round(totalruleper, 2).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                overallrule.AddCell(rulecell2);

                doc.Add(overallrule);
                #endregion end overal Rule


            }

            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            ////
            #region ToDos Table
       
            PdfPTable tblAuditSummary = new PdfPTable(1);
            tblAuditSummary.SpacingBefore =20f;
            tblAuditSummary.SpacingAfter = 10f;

            PdfPCell cellaudit = new PdfPCell(new Phrase("6.Compliance Audit Report", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellaudit.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellaudit.HorizontalAlignment = 1;
            cellaudit.VerticalAlignment = 1;
            cellaudit.Border = 0;
            tblAuditSummary.AddCell(cellaudit);

            doc.NewPage();
            doc.Add(new Paragraph(" "));
            


            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(tblAuditSummary);
            var columnHeaders = new string[] {"S.No", "Act",
            "Rule",
            "Activities",
            "Audit Due Date",
            "Vendor Submitted Date",
            "Compliant",
            "NonCompliance",
            "Not Applicable",
            "Audit Type",
            "Status",

            "Remarks, if any" };
            float[] tableWidths = new float[] { 50f, 200f, 200f, 150f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 120f };
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
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                table.AddCell(cell);
            }
            int serialNo = 0;
            foreach (var todo in reportData.ToDoList)
            {

                PdfPCell cCell = new PdfPCell(new Phrase((serialNo + 1).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Act.Name.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Rule.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Activity.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.DueDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Auditted == "No Audit" ? "NA" : todo.ToDo.AuditedDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);

                cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Compliant" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Non-Compliance" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Not-Applicable" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Auditted, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase(todo.ToDo.Status, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);
                cCell = new PdfPCell(new Phrase((!string.IsNullOrEmpty(todo.ToDo.AuditRemarks) ? todo.ToDo.AuditRemarks : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                table.AddCell(cCell);

                serialNo++;
            }
            table.HeaderRows = 1;
            doc.Add(table);


            #endregion ToDos Table

            #region Observations / Recommendations Table

            doc.NewPage();
            doc.Add(new Paragraph(" "));
           
            PdfPTable tblRecommendations = new PdfPTable(1);
            tblRecommendations.SpacingBefore = 10f;


            PdfPCell cell1 = new PdfPCell(new Phrase("Summary Of Audit Observations", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cell1.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cell1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cell1.HorizontalAlignment = 1;
            cell1.VerticalAlignment = 1;
           
            tblRecommendations.AddCell(cell1);

      

            PdfPCell cell3 = new PdfPCell(new Phrase("Auditor Recommendation", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cell3.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            cell3.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cell3.HorizontalAlignment = 1;
            cell3.VerticalAlignment = 1;
            
            tblRecommendations.AddCell(cell3);

            ////
            var recommendationsString = string.Join(",\n", reportData.ToDoRecommendations.Select(r => r.Recommendations).ToArray());
            recommendationsString = Regex.Replace(recommendationsString, "<.*?>", string.Empty);
            int sNo = 0;
            var observationsString = "";
            foreach (var todo in reportData.ToDoList.Where(t => t.ToDo.Status == "Rejected"))
            {
                if (!string.IsNullOrEmpty(todo.ToDo.AuditRemarks))
                    observationsString += (sNo + 1).ToString() + ") " + todo.ToDo.AuditRemarks + "\n";
            }
            observationsString = Regex.Replace(observationsString, "<.*?>", string.Empty);
            ///
            //PdfPCell cellObservations = new PdfPCell(new Phrase(observationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //cellObservations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cellObservations.HorizontalAlignment = 0;
            //cellObservations.Colspan = 1;
            //tblRecommendations.AddCell(cellObservations);

            PdfPCell cellRecommendations = new PdfPCell(new Phrase(recommendationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellRecommendations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellRecommendations.HorizontalAlignment = 0;
            tblRecommendations.AddCell(cellRecommendations);
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(tblRecommendations);
            #endregion ToDos Table

            //#region Observations / Recommendations Table

            #region List Of Internal Compliances Table

            var internalHeaders = new string[] {"Internal Compliance", "Impact Details", "Audit Type",
            "Risk",
            "Nature",
            "Consider For Score"
           
           
             };
            doc.NewPage();
            doc.Add(new Paragraph(""));
            PdfPTable tblnoncomp = new PdfPTable(1);
            tblnoncomp.SpacingBefore = 20f;
            tblnoncomp.SpacingAfter = 10f;

            float[] internaltableWidths = new float[] { 250f, 550f, 100f, 100f, 100f,100f };
            PdfPTable internaltable = new PdfPTable(internaltableWidths);
            internaltable.SpacingBefore = 10f;
            PdfPCell cellinternal = new PdfPCell(new Phrase("7.List of Non-complaince Details and Impact\r\n", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellinternal.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellinternal.HorizontalAlignment = 1;
            cellinternal.VerticalAlignment = 1;
            cellinternal.Border = 0;
            tblnoncomp.AddCell(cellinternal);
            doc.Add(tblnoncomp);
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
                    PdfPCell cCell1 = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cCell1.Colspan = 6;
                    internaltable.AddCell(cCell1);

                }
                else
                {
                 
                    Paragraph cell1Text = new Paragraph();
                    foreach (var kvp in internalCompliances1.ImpectDetails)
                    {
                        cell1Text.Add(new Chunk(kvp.Key.ToString() + ":", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                        cell1Text.Add(new Chunk(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    }
                    // Create another paragraph for the second cell
                    Paragraph cell2Text = new Paragraph("Plain Text", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));


                    PdfPCell cCell = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(cell1Text);
                    cCell.Colspan = 1;
                    internaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.AuditType, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
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
                   
                    
                }

            }
            // doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(internaltable);
            #endregion List Of Internal Compliances Table

            #region List Of No Audiit Internal Compliances Table

            var NoAuditinternalHeaders = new string[] {"Internal Compliance", "Impact Details", "Audit Type",
            "Risk",
            "Nature",
            "Consider For Score"


             };
            doc.NewPage();
            doc.Add(new Paragraph(""));
            PdfPTable tblNoAuditnoncomp = new PdfPTable(1);
            tblNoAuditnoncomp.SpacingBefore = 20f;
            tblNoAuditnoncomp.SpacingAfter = 10f;

            float[] NoAuditinternaltableWidths = new float[] { 250f, 550f, 100f, 100f, 100f, 100f };
            PdfPTable NoAuditinternaltable = new PdfPTable(NoAuditinternaltableWidths);
            NoAuditinternaltable.SpacingBefore = 10f;
            PdfPCell NoAuditcellinternal = new PdfPCell(new Phrase("8.List of No Audit Activity Details and Impact\r\n", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            NoAuditcellinternal.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            NoAuditcellinternal.HorizontalAlignment = 1;
            NoAuditcellinternal.VerticalAlignment = 1;
            NoAuditcellinternal.Border = 0;
            tblNoAuditnoncomp.AddCell(NoAuditcellinternal);
            doc.Add(tblNoAuditnoncomp);
            //table.WidthPercentage = 100; //table width to 100per
            //table.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });// width of each column
            foreach (string header in NoAuditinternalHeaders)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);

                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 1;
                NoAuditinternaltable.AddCell(cell);
            }
            foreach (var internalCompliances1 in NoAuditinternalCompliances)
            {
                if (string.IsNullOrEmpty(internalCompliances1.Risk))
                {
                    PdfPCell cCell1 = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                    cCell1.Colspan = 6;
                    NoAuditinternaltable.AddCell(cCell1);

                }
                else
                {

                    Paragraph cell1Text = new Paragraph();
                    foreach (var kvp in internalCompliances1.ImpectDetails)
                    {
                        cell1Text.Add(new Chunk(kvp.Key.ToString() + ":", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                        cell1Text.Add(new Chunk(kvp.Value, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    }
                    // Create another paragraph for the second cell
                    Paragraph cell2Text = new Paragraph("Plain Text", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));


                    PdfPCell cCell = new PdfPCell(new Phrase(internalCompliances1.InternalCompliance.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);
                    cCell = new PdfPCell(cell1Text);
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.AuditType, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Risk, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Nature, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);
                    cCell = new PdfPCell(new Phrase(internalCompliances1.Auditstatus, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    cCell.Colspan = 1;
                    NoAuditinternaltable.AddCell(cCell);


                }

            }
            // doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            doc.Add(NoAuditinternaltable);
            #endregion List Of Internal Compliances Table
            #region Signature

            doc.Add(new Paragraph(" "));
           

            doc.Add(new Paragraph(" "));

            PdfPTable tblSignature = new PdfPTable(1);
            tblSignature.SpacingBefore = 10f;
            //tblSignature.HorizontalAlignment = 2;

            PdfPCell cellSignature = new PdfPCell(new Phrase("Audit Submitted By" + reportData.AuditReportSummary.AuditorName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            cellSignature.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellSignature.HorizontalAlignment = 2;
            cellSignature.VerticalAlignment = 1;
            cellSignature.Border = 0;
            tblSignature.AddCell(cellSignature);

            PdfPCell cellAuditorName = new PdfPCell(new Phrase(string.IsNullOrEmpty(reportData.AuditReportSummary.AuditorName) ? "NA" : reportData.AuditReportSummary.AuditorName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            cellAuditorName.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellAuditorName.HorizontalAlignment = 2;
            cellAuditorName.VerticalAlignment = 1;
            cellAuditorName.Border = 0;
            tblSignature.AddCell(cellAuditorName);

            doc.Add(tblSignature);
            Paragraph paragraph = new Paragraph(new Chunk("This report is generated by using Ezycomp,No signature required. " + DateTime.Now, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLUE)));
            paragraph.Alignment = Element.ALIGN_CENTER;
            doc.Add(paragraph);
            #endregion Signature


            doc.Close();


            return File(pdf, "application/pdf", "Chart.pdf");

        }

        public async Task<AuditReportData> GetDataForChart(ToDoFilterCriteria criteria)
        {
            string Baseurl = ConfigurationManager.AppSettings["apiurl"]; // "https://apipro.ezycomp.com/api/Auditor/GetAuditReportData"; //"https://localhost:7221/api/Auditor/GetAuditReportData";
                                                                                         //  //"
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

                    //Company = new Guid("a7078dec-b017-4868-90e3-8ad2f7c888e7"),
                    //AssociateCompany = new Guid("c7496a2c-2aac-45c3-9ee2-d124b96115c2"),
                    //Location = new Guid("4c23e85b-f74c-4b67-a570-e459d301a236"),
                    //Month = "June",
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
                    legendColor = Color.LightBlue;
                    break;
                case "Non-Compliance":
                    legendColor = Color.Red;
                    break;
                case "Rejected":
                    legendColor = Color.Yellow;
                    break;
                case "Compliant":
                    legendColor = Color.MediumSeaGreen;
                    break;
                case "Not-Applicable":
                    legendColor = Color.Gray;
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
                    legendColor = new BaseColor(Color.LightBlue);
                    break;
                case "Non-Compliance":
                    legendColor = new BaseColor(Color.Red);
                    break;
                case "Rejected":
                    legendColor = new BaseColor(Color.Yellow);
                    break;
                case "Compliant":
                    legendColor = new BaseColor(Color.MediumSeaGreen);
                    break;
                case "Not-Applicable":
                    legendColor = new BaseColor(Color.Gray);
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
                Count = reportData.ToDoList.Count(l => l.ToDo.AuditStatus == "Compliant")
            });
            query.Add(new
            {
                Legend = "Non Compliant",
                Count = reportData.ToDoList.Count(l => l.ToDo.AuditStatus != "Compliant")
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
        private PdfPTable CreateHeaderTable()
        {
            var columnHeaders = new string[] {"S.No", "Act",
            "Rule",
            "Activities",
            "Audit Due Date",
            "Vendor Submitted Date",
            "Compliant",
            "NonCompliance",
            "Not Applicable",
            "Audit Type",
            "Status",

            "Remarks, if any" };
            float[] tableWidths = new float[] { 50f, 200f, 200f, 150f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 120f };
            PdfPTable headerTable = new PdfPTable(tableWidths);
            headerTable.SpacingBefore = 10f;
            //table.WidthPercentage = 100; //table width to 100per
            //table.SetTotalWidth(new float[] { iTextSharp.text.PageSize.A4.Rotate().Width - 25 });// width of each column
            foreach (string header in columnHeaders)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightBlue);
                cell.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
                //cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                headerTable.AddCell(cell);
            }

            return headerTable;
        }

        private PdfPTable CreateBodyTable(AuditReportData reportData)
        {
            float[] tableWidths = new float[] { 50f, 200f, 200f, 150f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 120f };
            PdfPTable table = new PdfPTable(tableWidths);
            // Generate table content

            int serialNo = 0;
            foreach (var todo in reportData.ToDoList)
            {                           

                        PdfPCell cCell = new PdfPCell(new Phrase((serialNo + 1).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Act.Name.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Rule.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Activity.Name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.DueDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Auditted == "No Audit" ? "NA" : todo.ToDo.AuditedDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);

                        cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Compliant" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Non-Compliance" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase((todo.ToDo.AuditStatus == "Not-Applicable" ? "Yes" : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Auditted, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase(todo.ToDo.Status, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);
                        cCell = new PdfPCell(new Phrase((!string.IsNullOrEmpty(todo.ToDo.AuditRemarks) ? todo.ToDo.AuditRemarks : ""), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(cCell);

                        serialNo++;
                    }            
           
            return table;
        }


    }
    public class AllowCrossSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");

            base.OnActionExecuting(filterContext);
        }
    }
    public class PageNumberEventHandler : PdfPageEventHelper
    { 
    public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // Create a PdfPTable to hold the page number
            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = document.PageSize.Width;

            // Create a PdfPCell to hold the page number text
            PdfPCell cell = new PdfPCell(new Phrase("Page " + writer.PageNumber));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = PdfPCell.NO_BORDER;

            // Add the cell to the table
            table.AddCell(cell);

            // Add the table to the document
            table.WriteSelectedRows(0, -2, document.LeftMargin-30, document.BottomMargin +20, writer.DirectContent);
        }
    }


}