using CookingBook.Models;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CookingBook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Init();
            this.LoadItems();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };
        }

        private async void LoadItems()
        {
            var recipies = await this.GetAllRecipes();
            var categories = await this.GetAllCategories();
            var favoriteRecipies = await this.GetFavouriteRecipies();

            allRecipies.ItemsSource = recipies;
            allCategories.ItemsSource = categories;
            favouriteRecipe.ItemsSource = favoriteRecipies;
        }
        private SQLiteAsyncConnection GetDbConnectionAsync()
        {
            var dbFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "cookBook.sqlite");

            var connectionFactory =
                new Func<SQLiteConnectionWithLock>(
                    () =>
                    new SQLiteConnectionWithLock(
                        new SQLitePlatformWinRT(),
                        new SQLiteConnectionString(dbFilePath, storeDateTimeAsTicks: false)));

            var asyncConnection = new SQLiteAsyncConnection(connectionFactory);

            return asyncConnection;
        }

        private async void Init()
        {
            await this.InitAsync(Seed);
        }

        private async Task InitAsync(Action seedCallback)
        {
            var connection = this.GetDbConnectionAsync();
            await connection.CreateTableAsync<Recipe>();
            await connection.CreateTableAsync<Category>();
            await connection.CreateTableAsync<Ingredient>();

            seedCallback();
        }

        private async void Seed()
        {
            var connection = this.GetDbConnectionAsync();

            var categoryList = new List<Category> {
                new Category() { Name = "Пилешко месо" },
                new Category() { Name = "Паста" },
                new Category() { Name = "Супи" },
                new Category() { Name = "Салати" },
                new Category() { Name = "Пици" }
            };

            var recipeList = new List<Recipe>
            {
                new Recipe () { Name = "Пилешки рулца с плънка от краставички и лютеница", CategoryId = 1, CookingTime = 30, Favorite = false, Portions = 4, PreparationTime = 20, Preparation = "Разстелете пилешките пържоли и ги овкусете със сол. Намажете ги с лютеница / по 1 с.л. на всяка/. Поръсете със нарязани на ситно кисели краставички и завийте на руло. Наредете пилешките рула в тава с незалепващо дъно и намазнено с олио. Намажете отгоре рулата с червен пипер и олио. Изпечете до златисто.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/pileshki-rulca-lutenica.jpg") },
                new Recipe () { Name = "Пилешко филе с бирена панировка", CategoryId = 1, CookingTime = 30,Favorite=false,Portions=4,PreparationTime=15, Preparation = "Почистваме и нарязваме пилешкото на хапки, след което го осоляваме и прибираме в хладилник докато приготвим панировката. Счукваме яйцата едно по едно и отделяме жълтъците от белтъците. Разбиваме с миксер жълтъците и бирата до пухкава смес и прибавяме брашно, за да се получи смес по-гъста от тази за палачинки. Разбиваме белтъците на сняг. Добавяме ги към бирената смес като разбъркваме внимателно до еднородна смес. Вземаме от пилешките хапки, потапяме ги в панировката и след това ги пържим до златисто от двете страни в сгорещеното олио.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/pile-birena-panirovka3.jpg")},
                new Recipe () { Name = "Равиоли със спанак", CategoryId = 2, CookingTime= 20, Favorite = false, Portions= 4, PreparationTime = 10, Preparation = "Кипнете вода в тенджера и сварете равиолите според упътването на опаковката. Междувременно загрейте 2 ч.л. зехтин в тиган и добавете пасирания чесън. След около 30 секунди добавете червения пипер и солта, нарязания спанак и водата. Гответе като от време на време разбърквате. След 5-7 минути махнете от котлона. Разделете спаначената смес в 4 чинии, отгоре разпределете тортелините. Поръсете с по 1 ч.л. зехтин всяка порция и сервирайте с пармезан.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/ravioli-cheese.jpg")},
                new Recipe () { Name = "Талиатели Алфредо с пиле", CategoryId=2, CookingTime = 30, PreparationTime = 10, Portions = 4, Favorite = false, Preparation = "Сварете пастата в 4 литра вода с малко сол и зехтин според указанието на опаковката. Нарежете пилешкото филе на дребни парченца. Сложете в тиган маслото и зехтина, загрейте на котлона и сложете филето да се изпържи до златисто от всички страни. Когато месото е готово сипете сметаната, добавете сол, черен пипер на вкус, скълцана скилидка чесън и чаша и половина настърган пармезан. Разбъркайте и гответе 3 минути. Изцедете пастата в гевгир и добавете към пилето със соса. Разбъркайте талиателите добре, така че да се овкусят всички със соса. Махнете от котлона и сервирайте. Поднесете пастата със нарязан на ситно магданоз, щипка черен пипер и няколко тънки резена пармезан.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/tagliateli-alfredo3.jpg") },
                new Recipe () { Name = "Супа със зеленчуци и два вида месо", CategoryId = 3, CookingTime = 40, PreparationTime = 15, Portions = 6, Favorite = false, Preparation = "Нарежете месото на дребни парчета. Запържете ги в малко олио. След като се зачерви, прибавете нарязаните лук, чушки и моркови. След 2-3 минути налейте вода и оставете да се свари месото. Когато месото е почти готово, добавете нарязания спанак, доматите и граха. След няколко минути прибавете фидето. Накрая направете варена застройка. Разбийте яйцето с млякото и брашното. Налейте от бульона и оставете на котлона за 5 минути. Върнете сместа обратно в супата и поръсете с нарязан магданоз.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/supa-zelenchuci-2mesa.jpg") },
                new Recipe () { Name = "Пилешката супа на баба", CategoryId = 3, CookingTime = 60, PreparationTime = 20, Portions = 6, Favorite = false, Preparation = "Слагам кокошката да се свари с малко олио. Нарязваме си зеленчуците на кубчета. Когато преценим, че е готова я изваждаме, за да я обезкостим и да накъсаме месцето на ситно. Изсипваме зеленчуците в бульона и оставяме да поврат. Когато обезкостим кокошката, добавяме месото към останалите продукти. Овкусяваме с подправките и добавяме фидето в последните 10 мин. преди да изключим супата. Разбиваме яйцето с киселото мляко и аз добавям няколко капки лимон. Изчаквам малко да поизстине супата и започвам да добавям малко от супата в купата с яйцето и млякото, докато се темперира.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/supa-pile-baba.jpg")},
                new Recipe () { Name = "Салата с нахут и чери домати", CategoryId = 4, CookingTime = 30, PreparationTime = 20, Favorite = false, Portions = 2, Preparation = "От вечерта накиснете нахут, на следващия ден го сварете и прецедете. През това време в дълбок тиган задушете в зехтин зеленчуците. След това смесете всичките продукти заедно с нахута. Ако искате може да добавите и маслинки.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/salata-nahut-tikvichki3.jpg") },
                new Recipe () { Name = "Салата Винегрет", CategoryId = 4, CookingTime = 60, PreparationTime = 20, Portions = 6, Favorite = false, Preparation= "Сварете кореноплодните. Нарежете ги на малки кубчета заедно с краставичките, след като ги обелите. Смесете ги в купа и объркайте с олио и сол на вкус.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/vinegret-salata.jpg") },
                new Recipe () { Name = "Уникална пица с бекон и спанак", CategoryId = 5, CookingTime = 15, PreparationTime = 35, Portions = 1, Favorite = false, Preparation ="Замесва се меко тесто и се оставя да втаса. Поставя се в тава, разстила се тестото и отгоре половината пица се намазва с лютеница, нарязания бекон, рендосаното сирене и се поръсва с чубрица. Другата половина на пицата се намазва с предварително приготвената плънка от спанак /спанакът е нарязан заедно с лука и задушен до омекване/, нарязан бекон и сирене. Отгоре се полива цялата пица с предварително разбити яйца. Пече се на 250 градуса, на вентилатор около 10 минути.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/pica-sinere-bekon-spanak.JPG") },
                new Recipe () { Name = "Пица с царевица и кайма", CategoryId = 5, CookingTime = 45, PreparationTime = 35, Portions = 4, Favorite = false, Preparation = "Замесваме меко тесто, което оставяме да постои и да втаса около 30 минути. Разделяме тестото на толкова, на колкото пици ще правим. Разстиламе с ръце и ги намазваме с доматено пюре. Добавяме от каймата, която за кратко сме запържили с малко лук и подправки, за да се раздроби и да стане на трохи. Поръсваме с царевица и риган и оставяме малко да втасат. Печем в загрята фурна, като към края поръсваме с кашкавал и пак запичаме.", Image = ImageConverter.ConvertFileToByte("Assets/DatabasePictures/piza-kaima-carevica.JPG") }
            };

            var ingredientList = new List<Ingredient>()
            {
                new Ingredient() {RecipeId = 1, Text = "пилешки пържоли - 4 бр." },
                new Ingredient() {RecipeId = 1, Text = "кисели краставички - 4 бр." },
                new Ingredient() {RecipeId = 1, Text = "лютеница - 4 с.л. домашна, едро смляна" },
                new Ingredient() {RecipeId = 1, Text = "сол" },
                new Ingredient() {RecipeId = 1, Text = "олио - за намазване" },
                new Ingredient() {RecipeId = 1, Text = "червен пипер" },
                new Ingredient() {RecipeId = 2, Text = "пилешко филе - 1 кг" },
                new Ingredient() {RecipeId = 2, Text = "сол" },
                new Ingredient() {RecipeId = 2, Text = "олио - за пържене" },
                new Ingredient() {RecipeId = 2, Text = "ЗА ПАНИРОВКАТА" },
                new Ingredient() {RecipeId = 2, Text = "яйца - 2 бр." },
                new Ingredient() {RecipeId = 2, Text = "бира - 200 мл" },
                new Ingredient() {RecipeId = 2, Text = "брашно" },
                new Ingredient() {RecipeId = 3, Text = "равиоли - 4 ч.ч." },
                new Ingredient() {RecipeId = 3, Text = "зехтин - 6 ч.л." },
                new Ingredient() {RecipeId = 3, Text = "чесън - 4 скилидки (пасиран)" },
                new Ingredient() {RecipeId = 3, Text = "пармезан - ¼ ч.ч. настърган" },
                new Ingredient() {RecipeId = 3, Text = "спанак - 400 г замразен" },
                new Ingredient() {RecipeId = 3, Text = "сол" },
                new Ingredient() {RecipeId = 3, Text = "червен пипер - ¼ ч.л. на люспи" },
                new Ingredient() {RecipeId = 3, Text = "вода - ½ ч.ч." },
                new Ingredient() {RecipeId = 4, Text = "талиатели - 400 г" },
                new Ingredient() {RecipeId = 4, Text = "течна сметана - 500 мл" },
                new Ingredient() {RecipeId = 4, Text = "масло - 1с.л" },
                new Ingredient() {RecipeId = 4, Text = "зехтин - 1с.л" },
                new Ingredient() {RecipeId = 4, Text = "сол" },
                new Ingredient() {RecipeId = 4, Text = "черен пипер" },
                new Ingredient() {RecipeId = 4, Text = "чесън - 1 скилидка" },
                new Ingredient() {RecipeId = 4, Text = "магданоз - 1/2 връзка свеж" },
                new Ingredient() {RecipeId = 4, Text = "пармезан - 1.5 ч.ч настърган" },
                new Ingredient() {RecipeId = 5, Text = "свинско месо - 200 г" },
                new Ingredient() {RecipeId = 5, Text = "телешко месо - 200 г" },
                new Ingredient() {RecipeId = 5, Text = "лук - 1 глава" },
                new Ingredient() {RecipeId = 5, Text = "чушки - 2 бр. (1 зелена и 1 червена)" },
                new Ingredient() {RecipeId = 5, Text = "моркови - 1 бр." },
                new Ingredient() {RecipeId = 5, Text = "спанак - 3 - 4 листа" },
                new Ingredient() {RecipeId = 5, Text = "доматен сок - 2 с.л." },
                new Ingredient() {RecipeId = 5, Text = "грах - 50 г от консерва" },
                new Ingredient() {RecipeId = 5, Text = "фиде - 50 г" },
                new Ingredient() {RecipeId = 5, Text = "кокошка - 1/2 бр." },
                new Ingredient() {RecipeId = 6, Text = "картофи - 2 бр. средни" },
                new Ingredient() {RecipeId = 6, Text = "моркови - 2 - 3 бр" },
                new Ingredient() {RecipeId = 6, Text = "лук - 1 малка глава" },
                new Ingredient() {RecipeId = 6, Text = "фиде" },
                new Ingredient() {RecipeId = 6, Text = "олио" },
                new Ingredient() {RecipeId = 6, Text = "яйца - 1 бр." },
                new Ingredient() {RecipeId = 6, Text = "кисело мляко - 3 - 4 с.л" },
                new Ingredient() {RecipeId = 6, Text = "чубрица" },
                new Ingredient() {RecipeId = 6, Text = "черен пипер" },
                new Ingredient() {RecipeId = 6, Text = "сол" },
                new Ingredient() {RecipeId = 6, Text = "вода" },
                new Ingredient() {RecipeId = 7, Text = "нахут" },
                new Ingredient() {RecipeId = 7, Text = "лук" },
                new Ingredient() {RecipeId = 7, Text = "чесън" },
                new Ingredient() {RecipeId = 7, Text = "Айсберг - 100 г" },
                new Ingredient() {RecipeId = 7, Text = "чери домати" },
                new Ingredient() {RecipeId = 7, Text = "краставици - 1 бр." },
                new Ingredient() {RecipeId = 7, Text = "зелен лук" },
                new Ingredient() {RecipeId = 7, Text = "сол" },
                new Ingredient() {RecipeId = 7, Text = "зехтин" },
                new Ingredient() {RecipeId = 7, Text = "чушки - 1 бр. зелена" },
                new Ingredient() {RecipeId = 8, Text = "картофи - 8 средни" },
                new Ingredient() {RecipeId = 8, Text = "моркови - 4 бр" },
                new Ingredient() {RecipeId = 8, Text = "червено цвекло - 2 глави" },
                new Ingredient() {RecipeId = 8, Text = "кисели краставички - 4 бр." },
                new Ingredient() {RecipeId = 8, Text = "олио - 5 с.л" },
                new Ingredient() {RecipeId = 8, Text = "сол" },
                new Ingredient() {RecipeId = 9, Text = "брашно - 1 ½ ч.ч." },
                new Ingredient() {RecipeId = 9, Text = "кисело мляко - 3 с.л." },
                new Ingredient() {RecipeId = 9, Text = "мая - 10 г" },
                new Ingredient() {RecipeId = 9, Text = "олио - 2 с.л." },
                new Ingredient() {RecipeId = 9, Text = "сол - 1 ч.л." },
                new Ingredient() {RecipeId = 9, Text = "захар - 1 ч.л." },
                new Ingredient() {RecipeId = 9, Text = "сирене" },
                new Ingredient() {RecipeId = 9, Text = "лютеница" },
                new Ingredient() {RecipeId = 9, Text = "бекон" },
                new Ingredient() {RecipeId = 9, Text = "яйца - 2 бр." },
                new Ingredient() {RecipeId = 9, Text = "спанак - 500 г" },
                new Ingredient() {RecipeId = 9, Text = "зелен лук" },
                new Ingredient() {RecipeId = 10, Text = "прясно мляко - 250 мл" },
                new Ingredient() {RecipeId = 10, Text = "яйца - 1 бр." },
                new Ingredient() {RecipeId = 10, Text = "мая - 1 суха" },
                new Ingredient() {RecipeId = 10, Text = "сол - 1 ч.л." },
                new Ingredient() {RecipeId = 10, Text = "захар - 1ч.л." },
                new Ingredient() {RecipeId = 10, Text = "олио - 3 с.л." },
                new Ingredient() {RecipeId = 10, Text = "кисело мляко - 1 кофичка" },
                new Ingredient() {RecipeId = 10, Text = "сода бикарбонат - 1 щ." },
                new Ingredient() {RecipeId = 10, Text = "брашно - около 1 кг" },
                new Ingredient() {RecipeId = 10, Text = "кайма - 300 г" },
                new Ingredient() {RecipeId = 10, Text = "лук - 1 глава" },
                new Ingredient() {RecipeId = 10, Text = "царевица - 1 малка кутийка" },
                new Ingredient() {RecipeId = 10, Text = "доматено пюре" },
                new Ingredient() {RecipeId = 10, Text = "кашкавал" },
                new Ingredient() {RecipeId = 10, Text = "риган" },
            };

            if (connection.Table<Category>().CountAsync().Result == 0)
            {
                await connection.InsertAllAsync(categoryList);
            }

            if (connection.Table<Recipe>().CountAsync().Result == 0)
            {
                await connection.InsertAllAsync(recipeList);
            }

            if (connection.Table<Ingredient>().CountAsync().Result == 0)
            {
                await connection.InsertAllAsync(ingredientList);
            }
        }

        private async Task<List<Category>> GetAllCategories()
        {
            var conn = this.GetDbConnectionAsync();
            var result = await conn.Table<Category>().ToListAsync();

            return result;
        }

        private async Task<List<Recipe>> GetAllRecipes()
        {
            var conn = this.GetDbConnectionAsync();
            var result = await conn.Table<Recipe>().ToListAsync();

            return result;
        }

        private void allRecipies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe selectedItem = e.AddedItems[0] as Recipe;
            this.Frame.Navigate(typeof(RecipeDetail), selectedItem);
        }

        private void allCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selectedItem = e.AddedItems[0] as Category;
            this.Frame.Navigate(typeof(RecipiesInCategory), selectedItem);
        }

        private async Task<List<Recipe>> GetFavouriteRecipies()
        {
            var conn = this.GetDbConnectionAsync();
            var recipies = await conn.Table<Recipe>().Where(x => x.Favorite == true).ToListAsync();
            return recipies;
        }

        private void favouriteRecipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe selectedItem = e.AddedItems[0] as Recipe;
            this.Frame.Navigate(typeof(RecipeDetail), selectedItem);
        }

        //private void search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        var matchingProducts = this.GetMatchingProducts(sender.Text).Result;
        //        search.ItemsSource = matchingProducts;
        //    }
        //}

        //private async Task<List<Product>> GetMatchingProducts(string query)
        //{
        //    var conn = this.GetDbConnectionAsync();
        //    var list = await conn.Table<Product>().Where(x => x.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1).ToListAsync();
        //    return list;
        //}
    }
}
