using System;
using System.IO;
using System.Windows.Forms;
using DishOfTheDay.Entity;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace DishOfTheDay.Editors
{
    public partial class StatisticsForm : Form
    {
        private StatisticsInfo info;
        
        public StatisticsForm()
        {
            InitializeComponent();
            
            info = DataLayer.Instance.GetStatistics();
            RtfBuilder result = new RtfBuilder();
            
            result.AppendBold("Інформація про страви").AppendLine()
                .AppendLine($"Усього страв: {info.TotalDishCount}")
                .AppendLine($"Страв із зображенням: {info.DishesWithImageCount}")
                .AppendLine($"Страв без зображення: {info.DishesWithoutImageCount}")
                .AppendLine($"Перша страва створена: {info.FirstCreated}")
                .AppendLine($"Остання страва створена: {info.LastCreated}");
            
            result.AppendLine().AppendBoldLine("Інформація про використання страв")
                .AppendLine($"Усього: {info.SumOfUsage}")
                .AppendLine($"Мінімальна кількість: {info.MinOfUsage}")
                .AppendLine($"Максимальна кількість: {info.MaxOfUsage}")
                .AppendLine($"Середня кількість: {info.AvgOfUsage}");

            result.AppendLine().AppendBoldLine("Найбільш популярний тип страви")
                .AppendLine($"{info.PopularDishType}: {info.PopularDishTypeCount} використання");

            result.AppendLine().AppendBoldLine("Найбільш популярна кухня")
                .AppendLine($"{info.PopularKitchen}: {info.PopularKitchenCount} використання");

            result.AppendLine().AppendBoldLine("Найбільш активні користувачі");
            foreach (var i in info.MostActive)
            {
                result.AppendLine($"{i.Name}: {i.Value} рейтингів виставлено");
            }

            result.AppendLine().AppendBoldLine("Найбільш голодні користувачі");
            foreach (var i in info.MostHungry)
            {
                result.AppendLine($"{i.Name}: {i.Value} страв використано");
            }

            txtInformation.Rtf = result.ToRtf();            //.Replace("\n", "\r\n");
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Data files (*.pdf)|*.pdf|All files|*.*";
            dlg.RestoreDirectory = true;
            dlg.DefaultExt = ".pdf";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var fontProgram = FontProgramFactory.CreateFont(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "verdana.ttf"));
                    var font = PdfFontFactory.CreateFont(fontProgram, "Windows-1251");
                    
                    var writer = new PdfWriter(dlg.FileName);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);
                    document.SetFont(font);
                    Paragraph newline = new Paragraph(new Text("\n"));

                    //header
                    var header = new Paragraph("СТАТИСТИКА ПО DISH OF THE DAY ")
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                    document.Add(header);
                    
                    // Line separator
                    var ls = new LineSeparator(new SolidLine());
                    document.Add(ls);
                    
                    //subheader
                    var subheader  = new Paragraph("Створено в DishOfTheDay " + DateTime.Now)
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(15);
                    document.Add(subheader);
                    
                    
                    //Information
                    var title1 = new Paragraph("Інформація про страви").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title1);
                    var totalDishCount = new Paragraph("Усього страв: " + info.TotalDishCount).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(totalDishCount);
                    var dishesWithImageCount = new Paragraph("Страв із зображенням: " + info.DishesWithImageCount).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(dishesWithImageCount);
                    var dishesWithoutImageCount = new Paragraph("Страв без зображення: " + info.DishesWithoutImageCount).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(dishesWithoutImageCount);
                    var firstCreated = new Paragraph("Перша страва створена: " + info.FirstCreated).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(firstCreated);
                    var lastCreated = new Paragraph("Остання страва створена: " + info.LastCreated).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(lastCreated);
                    document.Add(newline);
                    
                    var title2 = new Paragraph("Інформація про використання страв").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title2);
                    var sumOfUsage = new Paragraph("Усього: " + info.SumOfUsage).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(sumOfUsage);
                    var minOfUsage = new Paragraph("Мінімальна кількість: " + info.MinOfUsage).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(minOfUsage);
                    var maxOfUsage = new Paragraph("Максимальна кількість: " + info.MaxOfUsage).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(maxOfUsage);
                    var avgOfUsage = new Paragraph("Середня кількість: " + info.AvgOfUsage).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(avgOfUsage);
                    document.Add(newline);                    
                    
                    var title3 = new Paragraph("Найбільш популярний тип страви").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title3);
                    var popularDishType = new Paragraph(info.PopularDishType + ": " + info.PopularDishTypeCount + " використання").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(popularDishType);
                    document.Add(newline);   
                    
                    var title4 = new Paragraph("Найбільш популярна кухня").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title4);
                    var popularKitchen = new Paragraph(info.PopularKitchen + ": " + info.PopularKitchenCount + " використання").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12);
                    document.Add(popularKitchen);
                    document.Add(newline);                    

                    var title5 = new Paragraph("Найбільш активні користувачі").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title5);
                    Table table1 = new Table(2, false);
                    Cell cell11 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Ім'я"));
                    Cell cell12 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Кількість виставлених рейтингів"));
                    table1.AddCell(cell11);
                    table1.AddCell(cell12);
                    foreach (var i in info.MostActive)
                    {
                        Cell cell21 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.Name));
                        Cell cell22 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.Value.ToString()));
                        table1.AddCell(cell21);
                        table1.AddCell(cell22);
                    }
                    document.Add(table1);
                    document.Add(newline); 
                    
                    var title6 = new Paragraph("Найбільш активні користувачі").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12).SetBold();
                    document.Add(title6);
                    Table table2 = new Table(2, false);
                    Cell cell31 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Ім'я"));
                    Cell cell32 = new Cell(1, 1).SetBackgroundColor(ColorConstants.GRAY).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("Кількість використаних страв"));
                    table2.AddCell(cell31);
                    table2.AddCell(cell32);
                    foreach (var i in info.MostActive)
                    {
                        Cell cell41 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.Name));
                        Cell cell42 = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(i.Value.ToString()));
                        table2.AddCell(cell41);
                        table2.AddCell(cell42);
                    }
                    document.Add(table2);
                    document.Add(newline); 
                    
                    document.Add(newline);
                  
                    // Page numbers
                    var n = pdf.GetNumberOfPages();
                    for (var i = 1; i <= n; i++)
                    {
                        document.ShowTextAligned(new Paragraph(string.Format("page" + i + " of " + n)),
                            559, 806, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
                    }
                    
                    document.Close();
                    // MessageBox.Show("Данні успішно експортовано", "Information", MessageBoxButtons.OK,
                    //     MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Щось сталося", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }
    }
}
