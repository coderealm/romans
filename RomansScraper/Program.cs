using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;

namespace RomansBuyerScraper
{

    class Program
    {
        static string PageSelector = "//*[@id='searchResultList']/li";
        static string Domain = "http://www.romans.co.uk";
        static string HouseNodeSelector = "//*[contains(@class,'details')]";
        static string HouseTitleSelector = "//*[contains(@id,'PropertyHeadline')]";
        static string ImagesSelector = "//*[@id='banners']/li";
        static string LocationSelector = "//*[contains(@id,'PropertyAddress')]";
        static string PriceSelector = "//*[@id='slideshow_banners']/div[1]/div/span";
        static string DescriptionSelector = "//*[contains(@id,'PropertyDetails')]/p";
        static string Purchase = "buy";
        static string Agent = "Romans";
        static string ToBuy = "to-buy";
        static string ToRent = "to-rent`";

        static void Main(string[] args)
        {
            List<Address> addresses = GetAddresses();
            DownloadWebSiteForEachAddress(addresses);
        }

        public static List<Address> GetAddresses()
        {
            return Http.Get();
        }

        public static void DownloadWebSiteForEachAddress(List<Address> addresses)
        {
            foreach (var address in addresses)
            {
                HtmlNodeCollection htmlNodes = GetHtmlNodeCollection(address);
                if (htmlNodes != null)
                {
                    SelectDataFromDownloadedWebsite(htmlNodes, address);
                }
            }
        }

        public static string CreateSearchUrlForWebSite(Address address, string sale)
        {
            return $"http://www.romans.co.uk/Searching-for-Property-in-{address.Town}/{sale}/00-999999999/0/excluding-Let/NoRadius"; ;
        }

        public static string CreateNodeSelector( string pageSelector)
        {
            return pageSelector;
        }

        public static HtmlNodeCollection GetHtmlNodeCollection(Address address)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.LoadFromWebAsync(CreateSearchUrlForWebSite(address, ToBuy))?.Result;
            return htmlDocument.DocumentNode.SelectNodes(CreateNodeSelector(PageSelector));
        }

        public static void SelectDataFromDownloadedWebsite(HtmlNodeCollection htmlNodes, Address address)    
        {
            foreach (var htmlNode in htmlNodes)
            {
                var hyperTextLink = SelectNodeForListedHouse(htmlNode);
                House house = HouseFactory();
                house.HouseUrl = $"{Domain}{hyperTextLink}";
                HtmlDocument htmlDocument = DownloadSpecificHouseOnWebsite(house.HouseUrl);
                house.Title = htmlDocument.DocumentNode.SelectNodes(HouseTitleSelector)[0].InnerText;
                house.HouseImages = HouseImageFactory();
                HtmlNodeCollection imageNodes = SelectImagesOfTheHouse(htmlDocument);
                SelectImages(house, imageNodes);
                house.Location = SelectHouseLocation(house, htmlDocument);
                house.Price = SelectHousePrice(house, htmlDocument);

                var myDate = DateTime.Now;
                house.ListedOn = new DateFormatterProvider().Format("{0}", myDate, new CultureInfo("en-GB"));
                house.Description = htmlDocument.DocumentNode.SelectNodes(DescriptionSelector)[0].InnerText.Replace("\n", "").Replace("\r", "");
                house.Domain = Domain;
                house.EstateAgent = Agent;
                house.EstateAgentId = Agent;
                house.Purchase = Purchase;

                if (!string.IsNullOrWhiteSpace(house.Location))
                {
                    string[] parts = house.Location.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    house.Street = parts[0];
                    house.Town = parts[1];
                    house.County = address.County;
                    house.Postcode = parts[2];
                }

                var created = DateTime.Now;
                house.Created = new DateFormatterProvider().Format("{0}", created, new CultureInfo("en-GB"));

                Http.Post(house);
            }
        }

        private static decimal SelectHousePrice(House house, HtmlDocument htmlDocument)
        {
            string date = htmlDocument.DocumentNode.SelectNodes(PriceSelector)[0].InnerText.Trim().Replace("\r", "").Replace("\n", "").Replace("£", "").Replace("\n\r", "").Replace(",", "").Replace(".", "").Replace("pcm", "").Replace("tenancy info", "");
            return Convert.ToDecimal(date);
        }

        private static string SelectHouseLocation(House house, HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.SelectNodes(LocationSelector)[0].InnerText;
        }

        private static HtmlNodeCollection SelectImagesOfTheHouse(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.SelectNodes(ImagesSelector);
        }

        public static string SelectNodeForListedHouse(HtmlNode htmlNode)
        {
            HtmlDocument propertyNode = new HtmlDocument();
            propertyNode.LoadHtml(htmlNode.InnerHtml);
            return propertyNode.DocumentNode.SelectSingleNode(HouseNodeSelector)?.Attributes["href"]?.Value;
        }
        
        public static HtmlDocument DownloadSpecificHouseOnWebsite(string specificHouseUrl)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
           return htmlWeb.LoadFromWebAsync(specificHouseUrl)?.Result;
        }

        public static void SelectImages(House house, HtmlNodeCollection imageNodes)
        {
            foreach (var banner in imageNodes)
            {
                var imageUrl = banner.OuterHtml.Split(new string[] { "data-thumb=\"", "\"" }, StringSplitOptions.RemoveEmptyEntries)[3];
                string part = Guid.NewGuid().ToString().Replace("-", "");
                string filename = $"{AppDomain.CurrentDomain.BaseDirectory}{part}.jpeg";
                SaveImage(filename, imageUrl);
                var image = GetDataURL(filename);
                HouseImage houseImage = new HouseImage
                {
                    ImageUrl = image
                };
                house.HouseImages.Add(houseImage);
            }           
        }

        public static House HouseFactory()
        {
            return new House();
        }

        public static List<HouseImage> HouseImageFactory()
        {
            return new List<HouseImage>();
        }

        public static string GetDataURL(string imgFile)
        {
            return "data:image/" + Path.GetExtension(imgFile).Replace(".", "") + ";base64," + Convert.ToBase64String(File.ReadAllBytes(imgFile));
        }

        public static void SaveImage(string filename, string imageUrl)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, ImageFormat.Jpeg);
            }
            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}
