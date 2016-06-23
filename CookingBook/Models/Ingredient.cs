using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace CookingBook.Models
{
    public class Ingredient
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Text { get; set; }

        [ForeignKey(typeof(Recipe))]
        public int RecipeId { get; set; }
    }
}
