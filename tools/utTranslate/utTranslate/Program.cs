using Newtonsoft.Json;
using System.Text;

namespace FirstNodeReplacerHitman
{
    internal class Program
    {
        static List<char> russianLetters = new List<char>
{
    'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я',
    'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
};


        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string sourceFolder = @"C:\Users\roman\OneDrive\Desktop\Hitman_Ukr\ua"; // Ваша папка з текстовими файлами
            string destinationFolder = @"C:\Users\roman\OneDrive\Desktop\Hitman_Ukr\ua2"; // Папка для збереження перекладених файлів
            string ukrExample = @"C:\Users\roman\OneDrive\Desktop\Hitman_Ukr\HitmanBloodMoneyUkr.json";
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var jsonUkr = File.ReadAllTextAsync(ukrExample, Encoding.UTF8).GetAwaiter().GetResult();

            var ukr = JsonConvert.DeserializeObject<HLocClass>(jsonUkr);

            string[] files = Directory.GetFiles(sourceFolder, "*.JSON");

            for (int i1 = 0; i1 < files.Length; i1++)
            {
                string file = files[i1];
                string fileName = Path.GetFileName(file);
                var jsonText = File.ReadAllTextAsync(file, Encoding.UTF8).GetAwaiter().GetResult();

                var hloc = JsonConvert.DeserializeObject<HLocClass>(jsonText);

                hloc.children[0] = ukr.children[0];


                //GoThrouClass(hloc);

                //for (var i = 1; i < hloc.children.Count; i++)
                //{
                //    GoThrouClass(hloc.children[i]);
                //}

                var newJsonText = JsonConvert.SerializeObject(hloc, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                string destinationFilePath = Path.Combine(destinationFolder, fileName);
                File.WriteAllText(destinationFilePath, newJsonText, Encoding.UTF8);
                Console.WriteLine($"{i1}/{files.Length}");
            }

            Console.WriteLine("Переклад файлів завершено.");
        }

        private static void GoThrouClass(HLocClass root)
        {
            if (root == null) return;
            Stack<HLocClass> stack = new Stack<HLocClass>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                HLocClass current = stack.Pop();
                if (!string.IsNullOrEmpty(current.value))
                {
                    current.value = "";//TranslateText(current.value, current.name);
                }

                if (current.children != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        if (current.children[i] != null) // Переконуємося, що елемент не null
                        {
                            stack.Push(current.children[i]);
                        }
                    }
                }
            }
        }
    }
}
