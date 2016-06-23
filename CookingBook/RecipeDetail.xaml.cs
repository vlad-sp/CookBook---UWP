using CookingBook.Models;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CookingBook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecipeDetail : Page
    {
        public RecipeDetail()
        {
            this.InitializeComponent();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var recipe = (Recipe) e.Parameter;
            recipeDetail.DataContext = recipe;
            LoadIngredients(recipe.Id);
            LoadImage(recipe.Image);
            if (recipe.Favorite == true)
            {
                addToFavorite.Content = " - Премахни от любими";
            }
            else
            {
                addToFavorite.Content = " + Добави към любими";
            }
        }

        private async Task<List<Ingredient>> GetIngredients(int id)
        {
            var conn = this.GetDbConnectionAsync();
            var ingredients = await conn.Table<Ingredient>().Where(x => x.RecipeId == id).ToListAsync();

            return ingredients;
        }

        private void addToFavorite_Click(object sender, RoutedEventArgs e)
        {
            int id = (int) ((Button) sender).Tag;
            UpdateRecipeById(id);
        }

        private async void UpdateRecipeById(int id)
        {
            var conn = this.GetDbConnectionAsync();
            var recipe = await conn.Table<Recipe>().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (recipe.Favorite == true)
            {
                recipe.Favorite = false;
            }
            else
            {
                recipe.Favorite = true;
            }
            await conn.UpdateAsync(recipe);
            this.Frame.Navigate(this.GetType(), recipe);
            if (this.Frame.CanGoBack)
            {
                this.Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
            }
        }
        private async void LoadIngredients(int id)
        {
            var ingredients = await GetIngredients(id);
            ingridientsListView.ItemsSource = ingredients;

        }

        private async void LoadImage(byte[] imageArray)
        {
            var image = await LoadImageFromByteArray(imageArray);
            imageContainer.Source = image;
        }

        private async static Task<BitmapImage> LoadImageFromByteArray (byte[] image)
        {
            MemoryStream ms = new MemoryStream(image);
            BitmapImage bitmapImage = new BitmapImage();
            IRandomAccessStream a1 = await ConvertToRandomAccessStream(ms);
             await bitmapImage.SetSourceAsync(a1);
            return bitmapImage;
        }

        public static async Task<IRandomAccessStream> ConvertToRandomAccessStream(MemoryStream memoryStream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            var dw = new DataWriter(outputStream);
            var task = Task.Factory.StartNew(() => dw.WriteBytes(memoryStream.ToArray()));
            await task;
            await dw.StoreAsync();
            await outputStream.FlushAsync();
            return randomAccessStream;
        }
    }
}
