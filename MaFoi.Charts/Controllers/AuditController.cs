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

namespace MaFoi.Charts.Controllers
{
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
            var chart = new Chart
            {
                Width = 700,
                Height = 200,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
            };

            chart.Titles.Add(chartTitle);
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
            chart.Series[0].ChartType = chartType;

            List<string> GreenColorLegends = new List<string> { "Compliant", "Audited" };
            foreach (var q in chartData)
            {
                chart.Series[0].Color = GreenColorLegends.Contains(q.Legend) ? Color.Green : Color.Red;
                chart.Series[0].Points.AddXY(q.Legend, Convert.ToDouble(q.Count));
            }
            using (var chartimage = new MemoryStream())
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
            //if (System.IO.File.Exists(pdf))
            //    System.IO.File.Delete(pdf);
            PdfWriter.GetInstance(doc, new System.IO.FileStream(pdf, System.IO.FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            var reportData = await GetDataForChart(criteria);

            #region Top Summary            

            PdfPTable tblTopSummary = new PdfPTable(1);
            tblTopSummary.SpacingBefore = 10f;
            tblTopSummary.SpacingAfter = 10f;

            var reportHeading = String.Format("Audit Report for {0}, {1}", reportData.AuditReportSummary.Month, reportData.AuditReportSummary.Year);

            PdfPCell cellHeading = new PdfPCell(new Phrase(reportHeading, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
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

            float[] tblLeftSummaryWidths = new float[] { 50f, 50f };

            PdfPTable tblLeftSummary = new PdfPTable(tblLeftSummaryWidths);
            tblLeftSummary.SpacingBefore = 10f;
            tblLeftSummary.SpacingAfter = 10f;

            PdfPCell cellLeftSummaryHeading = new PdfPCell(new Phrase("Audit Submitted To", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellLeftSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellLeftSummaryHeading.PaddingBottom = 4f;
            cellLeftSummaryHeading.HorizontalAlignment = 1;
            cellLeftSummaryHeading.VerticalAlignment = 1;
            cellLeftSummaryHeading.Colspan = 2;
            cellLeftSummaryHeading.Border = 0;
            tblLeftSummary.AddCell(cellLeftSummaryHeading);

            foreach (var kvp in leftSummaryData)
            {
                PdfPCell cellLeftSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
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

            float[] tblRightSummaryWidths = new float[] { 50f, 50f };

            PdfPTable tblRightSummary = new PdfPTable(tblRightSummaryWidths);
            tblRightSummary.SpacingBefore = 10f;
            tblRightSummary.SpacingAfter = 10f;

            PdfPCell cellRightSummaryHeading = new PdfPCell(new Phrase("Audit Summary", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)));
            cellRightSummaryHeading.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            cellRightSummaryHeading.HorizontalAlignment = 1;
            cellRightSummaryHeading.VerticalAlignment = 1;
            cellRightSummaryHeading.Colspan = 2;
            cellRightSummaryHeading.Border = 0;
            cellRightSummaryHeading.PaddingBottom = 4f;
            tblRightSummary.AddCell(cellRightSummaryHeading);

            foreach (var kvp in RightSummaryData)
            {
                PdfPCell cellRightSummaryContent = new PdfPCell(new Phrase(kvp.Key, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
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
            tblChartAndSummary.SpacingBefore = 10f;
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

            chartData.Add(new
            {
                Legend = "Audited",
                Count = reportData.AuditorPerformance.Audited
            });
            chartData.Add(new
            {
                Legend = "Not Audited",
                Count = reportData.AuditorPerformance.NotAudited
            });

            var image = Image.GetInstance(Chart("Overall score %", chartData, SeriesChartType.Pie));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);


            chartData = new List<dynamic>();
            chartData.Add(new
            {
                Legend = "Compliant",
                Count = reportData.ToDoList.Count(l => l.AuditStatus == "Compliant")
            });
            chartData.Add(new
            {
                Legend = "Non Compliant",
                Count = reportData.ToDoList.Count(l => l.AuditStatus != "Compliant")
            });

            image = Image.GetInstance(Chart("Overall compliance score %", chartData, SeriesChartType.Bar));
            image.Alignment = 1;
            image.ScalePercent(75f);
            doc.Add(image);

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
            float[] tableWidths = new float[] { 40f, 150f, 150f, 90f, 70f, 70f, 60f, 60f, 60f, 60f, 60f };
            PdfPTable table = new PdfPTable(tableWidths);
            table.SpacingBefore = 10f;

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
                cCell = new PdfPCell(new Phrase(todo.SubmittedDate.ToString("d"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
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
            ///

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

            //doc.Add(new Paragraph(" "));
            //doc.Add(new Paragraph(" "));

            //PdfPTable tblRecommendations = new PdfPTable(2);
            //tblRecommendations.SpacingBefore = 10f;


            //PdfPCell cell1 = new PdfPCell(new Phrase("Summary Of Audit Observations", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //cell1.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            //cell1.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cell1.HorizontalAlignment = 1;
            //cell1.VerticalAlignment = 1;
            //cell1.Colspan = 2;
            //tblRecommendations.AddCell(cell1);

            //PdfPCell cell2 = new PdfPCell(new Phrase("Auditor Observation", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //cell2.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            //cell2.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cell2.HorizontalAlignment = 1;
            //cell2.VerticalAlignment = 1;
            //cell2.Colspan = 1;
            //tblRecommendations.AddCell(cell2);

            //PdfPCell cell3 = new PdfPCell(new Phrase("Auditor Recommendation", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            //cell3.BackgroundColor = new iTextSharp.text.BaseColor(Color.LightGreen);
            //cell3.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cell3.HorizontalAlignment = 1;
            //cell3.VerticalAlignment = 1;
            //cell3.Colspan = 1;
            //tblRecommendations.AddCell(cell3);

            //////
            //var recommendationsString = string.Join(",\n", reportData.ToDoRecommendations.Select(r => r.Recommendations).ToArray());
            //recommendationsString = Regex.Replace(recommendationsString, "<.*?>", string.Empty);
            //int sNo = 0;
            //var observationsString = "";
            //foreach (var todo in reportData.ToDoList.Where(t => t.Status == "Rejected"))
            //{
            //    if (!string.IsNullOrEmpty(todo.AuditRemarks))
            //        observationsString += (sNo + 1).ToString() + ") " + todo.AuditRemarks + "\n";
            //}
            //observationsString = Regex.Replace(observationsString, "<.*?>", string.Empty);
            /////
            //PdfPCell cellObservations = new PdfPCell(new Phrase(observationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //cellObservations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cellObservations.HorizontalAlignment = 0;
            //cellObservations.Colspan = 1;
            //tblRecommendations.AddCell(cellObservations);

            //PdfPCell cellRecommendations = new PdfPCell(new Phrase(recommendationsString, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
            //cellRecommendations.BorderColor = new iTextSharp.text.BaseColor(Color.Black);
            //cellRecommendations.HorizontalAlignment = 0;
            //cellRecommendations.Colspan = 1;
            //tblRecommendations.AddCell(cellRecommendations);

            //doc.Add(tblRecommendations);
            //#endregion ToDos Table

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
            string Baseurl = "https://apipro.ezycomp.com/api/Auditor/GetAuditReportData";
            AuditReportData reportData = new AuditReportData();
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage() { RequestUri = new Uri(Baseurl) };
                var payload = new ToDoFilterCriteria()
                {
                    Company = new Guid("310e6064-510f-4736-acd5-ed82eaca4765"),
                    AssociateCompany = new Guid("dc93767a-337d-4049-a3ca-21cc0b544afa"),
                    Location = new Guid("fd2494e1-99ce-48e7-a27c-625c800c3260"),
                    Month = "April",
                    Year = 2023,
                    Statuses = new string[] { "" }
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