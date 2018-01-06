using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HelperClass
{
    class RegexHelperClass
    {

        //    public static string getHtmlCode(string url)  url to html code
        //    public static string getHtmlTitle(string url) url to html title 
        //    public static List<string> getHtmlaTags(string link,string like) tüm a tagını getir
        //    public static List<string> getHtmlframeTags(string link, string like)  tüm frameleri getir
        //    public static List<string> getHtmlanyTags(string tagkey, string link, string like) 
        //    public static HtmlNodeCollection GetNodesToResult(string Url, string Nodes)
        //    public static string TagParseHtml(string htmlcode, string parseTag)
        //    public static bool IsNumber(object value) // Rakam/Sayı/Numara mı ?
        //    public static bool IsStringNullOrWhiteSpace(string value) // String "     " veya null kontrolü 
        //    public static bool IsStringNullorEmpty(string value) // String "" veya null kontrolü
        //    public static bool IsUrl(string url) // string Url mi değil mi kontrolü
        //DataTable table = RegexHelperClass.ExecuteQuery("select * from sozler");
        //foreach (DataRow row in table.Rows)
        //{
        //    MessageBox.Show(row["id"]+" "+ row["soz"]);
        //}



        static string SqlDatabaseConnection = "Data Source=" + Environment.CurrentDirectory + "\\gsozler.db";     
            
        public static DataTable ExecuteQuery(string sql)
        {
           // DataTable dt = RegexHelperClass.ExecuteQuery("select * from sozler");
            // Validate SQL
            if (string.IsNullOrWhiteSpace(sql))
            {
                return null;
            }
            else
            {
                if (!sql.EndsWith(";"))
                {
                    sql += ";";
                }
                SQLiteConnection connection = new SQLiteConnection(SqlDatabaseConnection);
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(connection);
                cmd.CommandText = sql;
                DataTable dt = new DataTable();
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                connection.Close();
                return dt;
            }
        }

        public static bool sqlliteWhere(string tablename,string fieldname, string value)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            String sSQL;
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            sSQL = "Select * from "+ tablename + " Where "+ fieldname + " = '" + value + "'";
            cmd.CommandText = sSQL;
            cmd.Connection = baglanti;
            SQLiteDataReader dr2;
            baglanti.Open();
            dr2 = cmd.ExecuteReader();
            dr2.Read();
            return dr2.HasRows;
        }

        public static bool sqlliteLike(string tablename, string fieldname, string value)
        {
            //  //  bool tst=RegexHelperClass.sqlliteLike("sozler","soz","mesk");
            SQLiteCommand cmd = new SQLiteCommand();
            String sSQL;
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            sSQL = "Select * from " + tablename + " Where " + fieldname + "  LIKE'%" + value + "%'";
            cmd.CommandText = sSQL;
            cmd.Connection = baglanti;
            SQLiteDataReader dr2;
            baglanti.Open();
            dr2 = cmd.ExecuteReader();
            dr2.Read();
            return dr2.HasRows;
        }

        public static void sqlliteInsert(string link)
        {
            // RegexHelperClass.sqlliteInsert("sakla samanı gelir zamanı,mesut demirci")
            // (soz, yazar) değiştirin

            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);

            String[] veriler = link.Split(',');

            String SqlText = "";
            for (int i = 0; i < veriler.Length; i++)
            {
                SqlText += "'"+ veriler[i]+"',";
            }
            SqlText = SqlText.Remove(SqlText.Length-1, 1);
            SqlText = "(" + SqlText + ")";
            SqlText = "INSERT INTO sozler (soz, yazar) VALUES" + SqlText;
         
            SQLiteCommand komut = new SQLiteCommand(SqlText, baglanti);

            baglanti.Open();
            try
            {
                komut.ExecuteNonQuery();
            }
            catch (Exception)
            {

                baglanti.Close();
            }

            baglanti.Close();
        }

        public void sqlliteEkle(string title, string link)
        {
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            SQLiteCommand komut = new SQLiteCommand("insert into films(title,link) values (@veri1,@veri2)", baglanti);
            komut.Parameters.AddWithValue("@veri1", title);
            komut.Parameters.AddWithValue("@veri2", link);
            baglanti.Open();
            try
            {
                komut.ExecuteNonQuery();
            }
            catch (Exception)
            {

                baglanti.Close();
            }

            baglanti.Close();

        }

        public static void sqlliteDeleteWhereValue(string tableName, string fieldID,string value) {
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            baglanti.Open();
            cmd.Connection = baglanti;
            cmd.CommandText = "delete from "+ tableName + " where "+ fieldID + " =  @id";    
            cmd.Parameters.AddWithValue("@id", value);
            cmd.ExecuteNonQuery();
        }

        public static void sqlliteDeleteLIKEValue(string tableName, string fieldname, string likevalue)
        {
           // RegexHelperClass.sqlliteDeleteLIKEValue("sozler", "soz", "ask");
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            baglanti.Open();
            cmd.Connection = baglanti;
            cmd.CommandText = "delete from " + tableName + " Where " + fieldname + "  LIKE'%" + likevalue + "%'";
            cmd.ExecuteNonQuery();  
        }

        public static void sqlliteUpdateValue(string tableName, string Wherefieldname, string whereValue,string updatefieldValue)
        {
            // RegexHelperClass.sqlliteUpdateIDValue("sozler","id","2","yazar='hamdi'");
            SQLiteConnection baglanti = new SQLiteConnection(SqlDatabaseConnection);
            baglanti.Open();
            using (SQLiteCommand command = new SQLiteCommand(baglanti))
            {
                command.CommandText =
                    "update "+ tableName + " set "+ updatefieldValue + " where "+ Wherefieldname + "="+ whereValue;
                command.ExecuteNonQuery();
            }

        }



        
        public static string getHtmlCode(string url)
        {
            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString(url);
                return htmlCode;
            }
        }

        public static string getHtmlTitle(string url)
        {
            const string tachUtlLink = "<title>(.*?)</title>";
            string htmlCode = getHtmlCode(url);
            MatchCollection tach211 = Regex.Matches(htmlCode, tachUtlLink, RegexOptions.Singleline);
            string title = tach211[0].Groups[1].Value.Trim();
            return title;
        }

        public static List<string> getHtmlaTags(string link,string like)
        {
            string htmlCode = getHtmlCode(link);
            const string tachUtlLink = "<a href=\"(.*?)\"";
            MatchCollection tach = Regex.Matches(htmlCode, tachUtlLink, RegexOptions.Singleline);
            string  sart="";
            string wsart = "";

            if (like == "") sart = ""; else sart = like;

            List<string> Linkler = tach
           .Cast<Match>()
           .Select(m => m.Value.Remove(0, 9).Replace('"', ' ').Trim())
           .Where(m => m.IndexOf(sart) >= 0)
           .Distinct()
           .ToList();

            return Linkler;

        }

        public static List<string> getHtmlframeTags(string link, string like)
        {
            string htmlCode = getHtmlCode(link);
            const string tachUtlLink = "<iframe.+?src=[\"'](.+?)[\"'].*?>";
            MatchCollection tach = Regex.Matches(htmlCode, tachUtlLink, RegexOptions.Singleline);
            string sart = "";
            string wsart = "";

            if (like == "") sart = ""; else sart = like;

            List<string> Linkler = tach
           .Cast<Match>()
           .Select(m => m.Groups[1].Value.Replace('"', ' ').Trim())
           .Where(m => m.IndexOf(sart) >= 0)
           .Distinct()
           .ToList();

            return Linkler;

        }

        public static List<string> getHtmlanyTags(string tagkey, string link, string like)
        {
            string htmlCode = getHtmlCode(link);
             string tachUtlLink = "<"+ tagkey + "=\"(.*?)\"";  // a href  a target
            MatchCollection tach = Regex.Matches(htmlCode, tachUtlLink, RegexOptions.Singleline);
            string sart = "";
            string wsart = "";

            if (like == "") sart = ""; else sart = like;

            List<string> Linkler = tach
           .Cast<Match>()
           .Select(m => m.Value.Remove(0, 9).Replace('"', ' ').Trim())
           .Where(m => m.IndexOf(sart) >= 0)
           .Distinct()
           .ToList();

            List<string> Link = new List<string>();
            foreach (var item in Linkler)
            {
                byte[] bytes = Encoding.Default.GetBytes(item);
                string title = Encoding.UTF8.GetString(bytes);
                if (IsStringNullOrWhiteSpace(title) ==false)
                Link.Add(title);
            }

            return Link;

        }

        public static HtmlNodeCollection GetNodesToResult(string Url, string Nodes)
        {
            string htmlCode = getHtmlCode(Url);
            HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
            dokuman.LoadHtml(htmlCode);
            HtmlNodeCollection basliklar = dokuman.DocumentNode.SelectNodes(Nodes);
            return basliklar;
        }

        public static string TagParseHtml(string htmlcode, string parseTag) {     
            XElement div = XElement.Parse(htmlcode);
            string width = (string)div.Attribute(parseTag);
            return width;
        }

        public static bool IsUrl(string url) // string Url mi değil mi kontrolü
        {
            Uri outUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)) // Url doğru ise devam et
                return true;

            return false;
        }




        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }


     



        public static string Md5(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }



        public static bool IsNumber(object value) // Rakam/Sayı/Numara mı ?
        {
            bool isNum;
            long retNum;
            isNum = long.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static bool IsStringNullOrWhiteSpace(string value) // String "     " veya null kontrolü 
        {
            if (!string.IsNullOrWhiteSpace(value))
                return false;
            return true;
        }

        public static bool IsStringNullorEmpty(string value) // String "" veya null kontrolü
        {
            if (!string.IsNullOrEmpty(value))
                return false;

            return true;
        }

        public static int? ConvertToInt32(string value) // Int32'ye çevir
        {
            int number;
            bool result = Int32.TryParse(value, out number);

            if (result)
                return number;

            return null;
        }

        public static bool ConvertToBool(string value) // Bool'a çevir. "True","False","1","0" için çalışıyor bunların dışındakilere tümünü false olarak return ediyor (Boşluk vs gibi)
        {
            var boolValue = false;
            if (bool.TryParse(value, out boolValue))
                return boolValue;

            var number = 0;
            int.TryParse(value, out number);
            return Convert.ToBoolean(number);
        }

        public static long? ConvertToLong(string value) // Long'a çevir
        {
            long number;
            bool result = long.TryParse(value, out number);

            if (result)
                return number;

            return null;
        }

        public static DateTime? ConvertToDateTime(string value) //Datetime'a çevir
        {
            DateTime dateValue;
            CultureInfo culture = CultureInfo.CurrentCulture;
            DateTimeStyles styles = DateTimeStyles.None;

            if (DateTime.TryParse(value, culture, styles, out dateValue))
                return dateValue;

            return null;
        }

        public static List<string> SplitByComma(string value) // Virgül (",")'e göre ayır ve string liste olarak döndür
        {
            return new List<string>(value.Split(','));
        }

        public static List<int> SplitByCommaConvertToInt32(string value) // Virgül (",")'e göre ayır ve int liste olarak döndür (String içerisinde 1,2,3,4 gibi rakamlar için çalışır)
        {
            return new List<int>(value.Split(',').Select(int.Parse));
        }

        public static string TurkishCharacterReplace(string Text) // Türkçe karakteri İngilizce karaktere çevir
        {
            return Text.Replace("ı", "i").Replace("İ", "I").
                        Replace("â", "a").
                        Replace("ç", "c").Replace("Ç", "C").
                        Replace("ğ", "g").Replace("Ğ", "G").
                        Replace("ö", "o").Replace("Ö", "O").
                        Replace("ş", "s").Replace("Ş", "S").
                        Replace("ü", "u").Replace("Ü", "U");
        }

        private static string key = "1b48f5effrhreherh43353hrthrhrthrthrthrthkukk..86jrww5asdasdjh5hj3gb5ad20db7834acf"; // _Encrypt ve _Decrypt metotları için oluşturduk

        public static string _Encrypt(string value) // String Şifrele
        {
            Byte[] inputArray = UTF8Encoding.UTF8.GetBytes(value);
            TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
            TripleDes.Key = UTF8Encoding.UTF8.GetBytes(key);
            TripleDes.Mode = CipherMode.ECB;
            TripleDes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = TripleDes.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            TripleDes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string _Decrypt(string value) // String Şifre Çöz
        {
            Byte[] inputArray = Convert.FromBase64String(value);
            TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
            TripleDes.Key = UTF8Encoding.UTF8.GetBytes(key);
            TripleDes.Mode = CipherMode.ECB;
            TripleDes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = TripleDes.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            TripleDes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static bool EmailAddressCheck(string emailAddress) // String Email mi Kontrolü
        {
            bool returnValue = false;

            string pattern = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            Match emailAddressMatch = Regex.Match(emailAddress, pattern);

            if (emailAddressMatch.Success)
                returnValue = true;

            return returnValue;
        }

        public static int getMonthDays(int year, int month) // Bugüne göre Ay içindeki gün sayısını bulur (Şubat 28 gün gibi)
        {
            return DateTime.DaysInMonth(year, month);
        }

     
    }






}
