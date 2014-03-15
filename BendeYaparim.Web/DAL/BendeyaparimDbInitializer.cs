using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BendeYaparim.Web.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Configuration;

namespace BendeYaparim.Web.DAL
{
    public class BendeyaparimDbInitializer : DropCreateDatabaseIfModelChanges<BendeyaparimContext>
    {
        int NumberOfUsers = 10;
        int NumberOfAdverts = 2000;
        int UserIndex = 0;
        int CityIndex = 0;
        int CategoryIndex = 0;
        int NumberofCategories = 340;
        int NumberOfCities = 80;

        Random random;

        public BendeyaparimDbInitializer()
        {
            random = new Random();
        }

        protected override void Seed(BendeyaparimContext context)
        {
            SeedRoles(context);
            SeedUsers(context);
            SeedCategories(context);
            SeedCities(context);
            SeedOffers(context);
            SeedSeeks(context);

            base.Seed(context);
        }

        private void SeedRoles(BendeyaparimContext context)
        {
            context.Roles.Add(new Role() { RoleName = "Customer" });
            context.Roles.Add(new Role() { RoleName = "Admin" });
            context.SaveChanges();
        }

        private void SeedUsers(BendeyaparimContext context)
        {
            List<User> users = new List<User>();
            Role r = context.Roles.Single(a => a.RoleName == "Customer");
            List<Role> roles = new List<Role>();
            roles.Add(r);

            for (int i = 0; i < NumberOfUsers; i++)
            {
                users.Add(new User() { EmailVisible = false, PhoneVisible = true, UserName = GetUserNames() + i, Roles = roles, BirthYear = 1982, Email = "blah@blah.com", PhoneNumber = "2123343322", FirstName = "Ilk adi", LastName = "Soyadi" });
            }

            users.ForEach(a => context.Users.Add(a));
            context.SaveChanges();
        }

        private void SeedCategories(BendeyaparimContext context)
        {
            //SeedEvHayati(context);
            SeedCategoriesFromTextFile(context);

        }

        private void SeedCategoriesFromTextFile(BendeyaparimContext context)
        {
            string path = ConfigurationManager.AppSettings["CategorySeedingText"];
            Category root = ProcessTextFile(path);
            root.Children.ForEach(a => context.Categories.Add(a));
        }

        private Category ProcessTextFile(string file)
        {
            StreamReader reader = new StreamReader(file, Encoding.GetEncoding("windows-1254"));
            string s;
            s = reader.ReadLine();

            Category root = new Category();
            root.Children = new List<Category>();
            string name;
            string image;
            while (s != null)
            {
                Console.WriteLine(s);

                if (s.StartsWith("1"))
                {
                    string[] sa = s.Split('|');
                    name = sa[0].Substring(1, sa[0].Length - 1);
                    image = sa[1];

                    root.Children.Add(new Category() { Name = name, Children = new List<Category>(), Parent = null, Level = 1, HtmlClassName = "icon_" + image, NumberOfJobOffers = 0, NumberOfJobSeeks = 0 });
                }
                if (s.StartsWith("2"))
                {
                    string[] sa = s.Split('|');
                    name = sa[0].Substring(1, sa[0].Length - 1);
                    image = sa[1];
                    root.Children.Last().Children.Add(new Category() { Name = name, Children = new List<Category>(), Level = 2, Parent = root.Children.Last(), HtmlClassName = "icon_" + image, NumberOfJobSeeks = 0, NumberOfJobOffers = 0 });
                }
                if (s.StartsWith("3"))
                {
                    string[] sa = s.Split('|');
                    name = sa[0].Substring(1, sa[0].Length - 1);
                    image = sa[1];
                    root.Children.Last().Children.Last().Children.Add(new Category() { Name = name, Level = 3, Parent = root.Children.Last().Children.Last(), HtmlClassName = "icon_" + image, NumberOfJobOffers = 0, NumberOfJobSeeks = 0, PageTitle = name, PageDescription = name });
                }

                s = reader.ReadLine();
            }

            return root;
        }

        private void SeedCities(BendeyaparimContext context)
        {
            GetCityNames().ToList().ForEach(a => context.Cities.Add(new City() { Name = a.Split('|')[0], NameExtension = Int32.Parse(a.Split('|')[1]) }));
            context.SaveChanges();
        }

        private void SeedOffers(BendeyaparimContext context)
        {
            List<JobOffer> adverts = new List<JobOffer>();
            //period array
            int[] periods = { 0, 1, 2, 3 };

            try
            {
                Category temp = null;
                for (int i = 0; i < NumberOfAdverts; i++)
                {
                    temp = GetCategory(context);
                    temp.NumberOfJobOffers++;
                    adverts.Add(new JobOffer()
                    {
                        Owner = GetUser(context),
                        ContentBody = GetAdvertContent() + 1,
                        Category = temp,
                        City = GetCity(context),
                        Price = 12 + i,
                        PricePeriod = periods[i % 4],
                        DisplayCount = 0,
                        CreateDate = DateTime.Now.AddDays(-i)
                    });
                }

                adverts.ForEach(a => context.JobOffers.Add(a));
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

        }

        private void SeedSeeks(BendeyaparimContext context)
        {
            List<JobSeek> adverts = new List<JobSeek>();
            int[] periods = { 0, 1, 2, 3 };

            try
            {
                Category temp = null;
                for (int i = 0; i < NumberOfAdverts; i++)
                {
                    temp = GetCategory(context);
                    temp.NumberOfJobSeeks++;

                    adverts.Add(new JobSeek()
                    {
                        Owner = GetUser(context),
                        ContentBody = GetAdvertContent() + 1,
                        Category = temp,
                        City = GetCity(context),
                        Price = 14 + i,
                        PricePeriod = periods[i % 4],
                        DisplayCount = 0,
                        CreateDate = DateTime.Now.AddDays(-i)
                    });
                }

                adverts.ForEach(a => context.JobSeeks.Add(a));
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

        }

        #region SeedingHelpers

        private Category GetCategory(BendeyaparimContext context)
        {
            if (CategoryIndex > NumberofCategories - 1)
            {
                CategoryIndex = 0;
            }

            Category cat = context.Categories.Where(a => a.Level == 3).OrderBy(a => a.Id).Skip(CategoryIndex).First();
            CategoryIndex++;

            return cat;
        }

        private City GetCity(BendeyaparimContext context)
        {
            if (CityIndex > NumberOfCities - 1)
            {
                CityIndex = 0;
            }

            City city = context.Cities.OrderBy(a => a.Id).Skip(CityIndex).First();
            CityIndex++;
            return city;
        }

        private User GetUser(BendeyaparimContext context)
        {
            if (UserIndex > NumberOfUsers - 1)
            {
                UserIndex = 0;
            }

            User user = context.Users.OrderBy(a => a.UserId).Skip(UserIndex).First();
            UserIndex++;
            return user;
        }

        #endregion

        private string GetAdvertContent()
        {
            int contentNum = RandomNumber(0, 10);
            return GetContents()[contentNum];
        }

        private string[] GetContents()
        {
            var contents = new string[]
            {
            "Merhaba dünya! yemek iptal etmek için burada mı. Korku her yazarın kapısı, ultricies fringilla fiyatı Topluluk istiyor. Biz üzgün daha yok. Evet, o ile DUI görevlileri anlaşma dayak istiyorum yumuşak bir yay intikam. Dünyada imperdiet kadar! Soft.",
            "Bu intikam amacıyla bu da COUCH zemin, ancak özgür bir consectetur aliquet yastıklı. Ama tam lor Oku arabaları tarafından, aşkım bazen daha Nisl olduğunu. ve fiyat bilge bir tekne ile Jaws görevlileri anlaşma. Askıya almak için olabilir. ",
            "Ama bir aslan korkusu, giriş ustaları değil bırakmaya sundurma ve sevgi vardır. Hiçbir şimdi AB örnek congue pellentesque gelen zamanı ultricies. Merhaba dünya! yemek iptal etmek için burada mı. Kolay yoktur. Bu üzücü urn Biz sevgi viverra çene aliquet kahkaha yazarıdır.",
            "Usta ve albeni dayak kitle. Bizi, benim ama intikam bu ders suç intikam, bir yatak nibh yaşayalım. Merhaba dünya! yemek iptal etmek için burada mı. Dolayısıyla, onu ben bir yay içine tahıl intikam olarak tasarlandı viverra lütfen.",
            "Yaşam ullamcorper tincidunt yay gibi, uyan seçenekleri kam. Yarın ya da yas bir yığın çap çap consectetur. Dolayısıyla, onun DUI, benim, gibi geliştirebilir nedenine göre arabalarını den fringilla eleman. Aynı zamanda kahkaha yumuşak bir örnek sundurma değildir. AB İçerik için buraya gelecek.",
            "E-posta Adresi Felis Kategori Yönetimi oklar AB, yas sapien düşünüyorum. Position: o aşk Nisl Topluluk albeni bir Yanıtla avantajdır. Bu habitasse dictumst bir sokakta. Ya da yoksulluk, ne de Moors bazı büyük malesuada. Hayatın yazar dayak için.",
            "Biz, ancak COUCH yastıklı Mozilla sen hast o içki yaptığınız için. Ürün ne de lor ultricies ya şimdi ya sonra kitle istiyor. Dokümantasyon baharat, Aulis porttitor üzgün aşk ne kadar iyi tedavi olacak.",
            "Bir titreme hendrerit içmeniz kadar. Biz ağrı vadi mauris. Askıya almak için olabilir. Evet, tarafından toprak söylenmiştir. Korku, ne kemerlerinde urn Rutrum Ides süslüyor değil, müneccim aşktır. Ücretli abonelik Biz kitle Consequat mattis gerekir. göller iyi kahkaha, ancak çelenk gibi Gate üyeleri bir tekne. Göller yumuşak bir ihtiyacı kadar burada daha o maya çalışan sürece.",
            "Bu tincidunt, Upload ve örnek sistem, kitle consequat mal sürece mete ağrısı, yay istiyor. Phasellus hendrerit, accumsan çocuk ile dart istediği, işkenceci şimdi feribot adam, ne de bir oyuk Gerçekten Pay Dizini olduğunu. Ama sadece pellentesque mattis bir yorum euismod. Her zaman üzgün nefret veya zehirli bir Düzenli liste değil kadar.",
            "Lütfen askıya alınması için, ne de işkenceci ne de, her zaman göl ve ne de, örnek bir ders öncesi, sundurma için hiçbir sıkıntı yoktur. Bu, Topluluk alır nisl ve örnek bir pin de bu nibh. De nibh için ne yüz kızartıcı olduğunu. Ama zehirli maddeleri kapıda komisyon çalışması. Ihtiyaç zamanı Ürün kahkaha."
            };

            return contents;
        }

        private string GetUserNames()
        {
            int contentNum = RandomNumber(0, 5);
            return GetNames()[contentNum];
        }

        private int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        public static string[] GetNames()
        {
            var users = new string[] { "John", "Cenk", "Ibrahim", "Mert", "Kazim", "Ahmet" };
            return users;
        }

        private string[] GetCityNames()
        {
            var cities = new string[] 
            { 
                "Adana|1",
                "Adıyaman|1",
                "Afyonkarahisar|1",
                "Ağrı|1",
                "Aksaray|1",
                "Amasya|1",
                "Ankara|1",
                "Antalya|1",
                "Ardahan|1",
                "Artvin|0",
                "Aydın|1",
                "Balıkesir|0",
                "Bartın|1",
                "Batman|1",
                "Bayburt|1",
                "Bilecik|0",
                "Bingöl|0",
                "Bitlis|0",
                "Bolu|1",
                "Burdur|1",
                "Bursa|1",
                "Çanakkale|0",
                "Çankırı|1",
                "Çorum|1",
                "Denizli|0",
                "Diyarbakır|1",
                "Düzce|0",
                "Edirne|0",
                "Elazığ|1",
                "Erzincan|1",
                "Erzurum|1",
                "Eskişehir|0",
                "Gaziantep|0",
                "Giresun|1",
                "Gümüşhane|0",
                "Hakkari|0",
                "Hatay|1",
                "Iğdır|1",
                "Isparta|1",
                "İstanbul|1",
                "İzmir|0",
                "Kahramanmaraş|1",
                "Karabük|0",
                "Karaman|1",
                "Kars|1",
                "Kastamonu|1",
                "Kayseri|0",
                "Kırıkkale|0",
                "Kırklareli|0",
                "Kırşehir|0",
                "Kilis|0",
                "Kocaeli|0",
                "Konya|1",
                "Kütahya|1",
                "Malatya|1",
                "Manisa|1",
                "Mardin|0",
                "Mersin|0",
                "Muğla|1",
                "Muş|1",
                "Nevşehir|0",
                "Niğde|0",
                "Ordu|1",
                "Osmaniye|0",
                "Rize|0",
                "Sakarya|1",
                "Samsun|1",
                "Siirt|0",
                "Sinop|1",
                "Sivas|1",
                "Şanlıurfa|1",
                "Şırnak|1",
                "Tekirdağ|1",
                "Tokat|1",
                "Trabzon|1",
                "Tunceli|0",
                "Uşak|1",
                "Van|1",
                "Yalova|1",
                "Yozgat|1",
                "Zonguldak|1"
            };
            return cities;
        }




    }
}