using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        private const string Alf = "abcdefghijklmnopqrstuvwxyz";
        private const int MostFrequent = 4; //индекс для "e"
        private const int LessFrequent = 25; //индекс для "z"

        [AllowAnonymous]
        [HttpPost]
        public string TranslateTextToCaesar(string text, int offset)
        {
            try
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                text = text.ToLower();
                //проверки на корректность введных данных
                if (offset < 0 || offset > 26)
                    throw new OffsetOutOfRangeException(offset +
                                                        " is an incorrect offset, please set 0 >= value <= 26");

                System.IO.File.WriteAllText(folderPath + "\\input.txt", text);
                if (offset == 0 || offset == 26)
                {
                    System.IO.File.WriteAllText(folderPath + "\\output.txt", text);
                    return text;
                }
                var resultSb = new StringBuilder(text.Length);

                //проход по входной строке и обработка каждого символа
                foreach (var charachter in text)
                {
                    var currentChar = Alf.IndexOf(charachter.ToString(), 0, StringComparison.CurrentCultureIgnoreCase);
                    if (currentChar != -1) //на случай если этот символ не буква(".", ";", "!" etc)
                    {
                        var resultChar = currentChar + offset; //вычислить позицию нового символа в словаре
                        if (resultChar >= Alf.Length) //пройти по кругу, если необходимо
                            resultChar -= Alf.Length;
                        resultSb.Append(Alf[resultChar]);
                    }
                    else resultSb.Append(charachter);
                }
                System.IO.File.WriteAllText(folderPath + "\\output.txt", resultSb.ToString());
                //return View("Index");
                return resultSb.ToString(); //ConverObjectToJson(new TranslateData{Offset = inputTranslateData.Offset, Text = resultSb.ToString()});
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public string TranslateTextFromCaesar(string text, int offset)
        {
            try
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                text = text.ToLower();
                //проверки на корректность введных данных
                if (offset < 0 || offset > 26)
                    throw new OffsetOutOfRangeException(offset +
                                                        " is an incorrect offset, please set 0 >= value <= 26");

                System.IO.File.WriteAllText(folderPath + "\\input.txt", text);
                if (offset == 0 || offset == 26)
                {
                    System.IO.File.WriteAllText(folderPath + "\\output.txt", text);
                    return text;
                }
                var resultSb = new StringBuilder(text.Length);

                //проход по входной строке и обработка каждого символа
                foreach (var charachter in text)
                {
                    var currentChar = Alf.IndexOf(charachter.ToString(), 0, StringComparison.CurrentCultureIgnoreCase); //вычислить позицию нового символа в словаре
                    if (currentChar != -1) //на случай если этот символ не буква(".", ";", "!" etc)
                    {
                        var resultChar = currentChar - offset;
                        if (resultChar < 0) //пройти по кругу, если необходимо
                            resultChar += Alf.Length;
                        resultSb.Append(Alf[resultChar]);
                    }
                    else resultSb.Append(charachter);
                }
                System.IO.File.WriteAllText(folderPath + "\\output.txt", resultSb.ToString());
                return resultSb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public string MakePrediction(string text)
        {
            try
            {
                int[] freqArr = CalculateFreq(text);//new JavaScriptSerializer().Deserialize<int[]>(text);
                int prosOffsetByMax;
                int prosOffsetByMin;
                //найти максимальную и минимальную частоту
                int max = 0;
                int min = 0;
                for (int i = 0; i < freqArr.Length; i++)
                {
                    if (freqArr[i] > freqArr[max]) max = i;
                    if (freqArr[i] < freqArr[min]) min = i;
                }
                //предположить сдвиг по максимальному
                if (max > MostFrequent) prosOffsetByMax = max - MostFrequent;
                else if (max < MostFrequent) prosOffsetByMax = Alf.Length - MostFrequent + max;
                else prosOffsetByMax = 0;
                prosOffsetByMin = LessFrequent - min;
                return new JavaScriptSerializer().Serialize(new { Max = prosOffsetByMax, Min = prosOffsetByMin });
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private int[] CalculateFreq(string text)
        {
            int[] freqArr = new int[Alf.Length];
            foreach (var currentChar in text.Select(charachter => Alf.IndexOf(charachter.ToString(), 0, StringComparison.CurrentCultureIgnoreCase)).
                Where(currentChar => currentChar > -1))
            {
                freqArr[currentChar]++;
            }
            return freqArr;
        }

        private class OffsetOutOfRangeException : Exception
        {
            public OffsetOutOfRangeException(string message) : base(message) { }
        }
    }
}