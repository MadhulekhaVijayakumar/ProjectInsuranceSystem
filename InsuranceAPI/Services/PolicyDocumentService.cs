using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using QuestPDF.Drawing;

namespace InsuranceAPI.Services
{
    public class PolicyDocumentService : IPolicyDocumentService
    {
        public byte[] GeneratePolicyDocument(Insurance insurance)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    // HEADER
                    page.Header().AlignCenter().Text($"Your Vehicle Insurance Policy\n#{insurance.InsurancePolicyNumber}")
                        .FontSize(18).Bold().FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        // CLIENT DATA
                        col.Item().Text("INSURANT DATA").Bold().FontSize(13).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text($"Name: {insurance.Client.Name}");
                            table.Cell().Text($"Email: {insurance.Client.Email}");

                            table.Cell().Text($"Address: {insurance.Client.Address}");
                            //table.Cell().Text($"City: {insurance.Client.City}");

                            table.Cell().Text($"Birthdate: {insurance.Client.DateOfBirth:MM/dd/yyyy}");
                            table.Cell().Text($"Gender: {insurance.Client.Gender}");
                        });

                        // VEHICLE DATA
                        col.Item().Text("VEHICLE DATA").Bold().FontSize(13).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            table.Cell().Text($"Type: {insurance.Vehicle.VehicleType}");
                            table.Cell().Text($"Number: {insurance.Vehicle.VehicleNumber}");

                            table.Cell().Text($"Make: {insurance.Vehicle.MakerName}");
                            table.Cell().Text($"Fuel Type: {insurance.Vehicle.FuelType}");

                            table.Cell().Text($"Date of Manufacture: {insurance.Vehicle.RegistrationDate:MM/dd/yyyy}");
                            table.Cell().Text($"Seats: {insurance.Vehicle.SeatCapacity}");
                        });

                        // PRODUCT DATA
                        col.Item().Text("PRODUCT DATA").Bold().FontSize(13).Underline();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            table.Cell().Text($"Insurance Start Date: {insurance.InsuranceStartDate:MM/dd/yyyy}");
                            table.Cell().Text($"Insurance Sum: ₹{insurance.InsuranceSum}");

                            //table.Cell().Text($"Coverage Type: {insurance.InsuranceDetail.LiabilityOption}");
                            //table.Cell().Text($"Price Option: {insurance.PriceOptio}");
                        });

                        // PRICING
                        col.Item().Text("PRICING").Bold().FontSize(13).Underline();
                        col.Item().Text($"Premium Amount: ₹{insurance.PremiumAmount} p.a.");
                        col.Item().Text("Subject to 10% VAT added to the above amount.");
                        col.Item().Text("40% upon agreement - 60% upon delivery.");

                        // SIGNATURE
                        col.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeColumn().Text($"Date: {DateTime.Today:MM/dd/yyyy}");
                            row.RelativeColumn().AlignRight().Text("Signee: ______________________");
                        });
                    });

                    // FOOTER
                    page.Footer().AlignCenter().Text("Powered by Automobile Insurance Co. | www.automobile-insurance.com").FontSize(9);

                });
            });

            return document.GeneratePdf();
        }


    }

}
