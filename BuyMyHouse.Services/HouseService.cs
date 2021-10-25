using Aspose.Pdf;
using BuyMyHouse.Domain;
using BuyMyHouse.Domain.DTO;
using BuyMyHouse.Domain.Helpers;
using BuyMyHouse.Infrastructure;
using BuyMyHouse.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyMyHouse.Services
{
    public class HouseService : IHouseService
    {
        private TableStorageHouse db_House { get; set; }
        private BlobStorage blobStorage { get; set; }

        public HouseService(TableStorageHouse tableStorageHouse, BlobStorage blobStorage)
        {
            this.db_House = tableStorageHouse;
            this.blobStorage = blobStorage;
        }
        public async Task<HouseDTO> CreateHouse(HouseDTO houseDTO)
        {
            var houseDAL = HouseConversionHelper.toDAL(houseDTO);
            var result = await db_House.CreateEntity(houseDAL);
            var userDTO = HouseConversionHelper.toDTO(result);
            return userDTO;
        }

        public async Task<List<HouseDTO>> GetHouses(double min, double max)
        {
            var result = await db_House.GetHouses(min, max);
            var list = result.Select(x => HouseConversionHelper.toDTO(x)).ToList();
            return list;
        }

        public async Task<HouseDTO> GetEntity(string partioningKey, string rowKey)
        {
            var result = await db_House.GetEntityByPartitionKeyAndRowKey(partioningKey, rowKey);
            if (result is null) throw new NullReferenceException("No house found.");
            var houseDTO = HouseConversionHelper.toDTO(result);
            return houseDTO;
        }

        public async Task<HouseDTO> UpdateHouse(HouseDTO houseDTO)
        {
            var userDAL = await db_House.GetEntityByPartitionKeyAndRowKey(houseDTO.HouseId, houseDTO.Street + houseDTO.ZipCode);
            if (userDAL is null) throw new NullReferenceException($"No user found on with {houseDTO.HouseId}.");
            var result = await db_House.UpdateEntity(userDAL);
            var houseDto = HouseConversionHelper.toDTO(result);
            return houseDto;
        }

        public async Task<bool> UpdateHousePicture(string houseId, string street, string zipcode, Stream stream)
        {
            var imageReferenceName = $"{houseId}-{street}-{zipcode}";
            var result = await blobStorage.UploadImage(imageReferenceName, stream);
            var imagUrl = await blobStorage.GetImage(imageReferenceName);
            var house = await db_House.GetEntityByPartitionKeyAndRowKey(houseId, street+zipcode);

            house.ImageURL = imagUrl;
            await db_House.UpdateEntity(house);

            return result;
        }

        public async Task<bool> DeleteHouse(string partioningKey, string rowKey)
        {
            var result = await db_House.DeleteEntity(partioningKey, rowKey);
            return result;
        }

        public async Task CalculateMortgage(UserDTO userDTO)
        {
            var maxMortage = new double();
            var minMortage = new double();
            var pdfContent = "";
            var filename = "";

            if (userDTO.Age < 18 || !userDTO.HasPermanentContract)
            {
                maxMortage = 0;
                minMortage = 0;

                pdfContent = @$"Beste {userDTO.FirstName} {userDTO.LastName},
                                            You will not recieve any available houses. Please update your credentials.
                                            Mortag range: {minMortage} - {maxMortage}
                                            Available houses: None.";

                filename = $"{userDTO.UserId}-{userDTO.FirstName}-{userDTO.LastName}-document.txt";
                await blobStorage.UploadPdf(filename, new MemoryStream(Encoding.UTF8.GetBytes(pdfContent ?? "")));
            }

            var totalMortage = userDTO.IncomeYearly * 30;
            maxMortage = CalculateMax(totalMortage, userDTO.Debts);
            minMortage = CalculateMin(totalMortage, userDTO.Debts);

            var houses = await GetHouses(minMortage, maxMortage);

            pdfContent = @$"Beste {userDTO.FirstName} {userDTO.LastName},
                                                This is your mortage offer:
                                                Mortag range: {minMortage} - {maxMortage}
                                                Available houses:";

            foreach (var house in houses)
            {
                pdfContent += $"{house.Title}, {house.Decsription}. The house: <img src={house.ImageURL}>";
            }

            filename = $"{userDTO.UserId}-{userDTO.FirstName}-{userDTO.LastName}-document.txt";

            await blobStorage.UploadPdf(filename, new MemoryStream(Encoding.UTF8.GetBytes(pdfContent ?? "")));
        }

        public double CalculateMax(double totalMortage, double debts)
        {
            return totalMortage * 0.6 - debts * 0.95;
        }

        public double CalculateMin(double totalMortage, double debts)
        {
            return totalMortage * 0.4 - debts * 0.94;
        }

        public async Task SendMail(UserDTO userDTO)
        {
            if (userDTO.Email is null or "") return;

            var uri = await blobStorage.GetPdf($"{userDTO.UserId}-{userDTO.FirstName}-{userDTO.LastName}-document.txt");

            try
            {
                var client = new SendGridClient(Environment.GetEnvironmentVariable("SendGridClient"));
                var from = new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailAddress"), "Buy My House App");
                var subject = "Your House Offers";
                var to = new EmailAddress(userDTO.Email, "");
                var plainTextContent = "Find the houses available for you";
                var htmlContent = $"<div><strong>Follow the link</strong><br>" +
                                    $"<p>This is a temorary link <a href={uri}>Click here.</a>.</p></div>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
