using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace CookingBook.Models
{
    public class Recipe
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [ManyToOne]
        public Category Category { get; set; }

        public bool Favorite { get; set; }

        public byte[] Image { get; set; }

        public int Portions { get; set; }

        public int TotalTime { get { return this.PreparationTime + this.CookingTime; } }

        public int PreparationTime { get; set; }

        public int CookingTime { get; set; }

        public string Preparation { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert)]
        public List<Ingredient> Ingredients { get; set; }
    }
}
