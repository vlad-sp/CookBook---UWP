using CookingBook.Models;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CookingBook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecipiesInCategory : Page
    {
        public RecipiesInCategory()
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
            var category = (Category) e.Parameter;
            recipiesInCategory.DataContext = category;
            LoadRecipies(category.Id);
        }

        private async Task<List<Recipe>> GetAllRecipeFromCategory(int id)
        {
            var conn = this.GetDbConnectionAsync();
            var recipies = await conn.Table<Recipe>().Where(x => x.CategoryId == id).ToListAsync();

            return recipies;
        }

        private async void LoadRecipies(int id)
        {
            var recipies = await GetAllRecipeFromCategory(id);
            recipiesList.ItemsSource = recipies;
        }

        private void recipiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe selectedItem = e.AddedItems[0] as Recipe;
            this.Frame.Navigate(typeof(RecipeDetail), selectedItem);
        }
    }
}
