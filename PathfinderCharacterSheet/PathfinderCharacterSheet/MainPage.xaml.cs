using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathfinderCharacterSheet
{
    public partial class MainPage : ContentPage
    {

        public class CharacterItem
        {
            public string name { get; set; }
            public string race { get; set; }
            public int level { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            Characters.ItemSelected += (s, e) =>
            {
                
            };
            Characters.GestureRecognizers.Add(new TapGestureRecognizer { NumberOfTapsRequired = 2, Command = new Command(() =>
            {
            })});
            CharacterSheetStorage.Instance.LoadCharacters();
            UpdateListView();
        }

        public void UpdateListView()
        {
            var characterItems = new List<CharacterItem>();
            foreach (var kvp in CharacterSheetStorage.Instance.characters)
            {
                var c = kvp.Key;
                var totalLevel = 0;
                if (c.levelOfClass != null)
                    foreach (var loc in c.levelOfClass)
                        totalLevel += loc.level;
                var item = new CharacterItem
                {
                    name = c.characterName,
                    race = c.race,
                    level = totalLevel,
                };
                characterItems.Add(item);
            }
            Characters.ItemsSource = characterItems.ToArray();
            //Characters.ItemsSource = new List<CharacterSheet>(CharacterSheetStorage.Instance.characters.Keys).ToArray();
        }

        private void Add_Clicked(object sender, EventArgs args)
        {
            var nc = new NewCharacter();
            Navigation.PushAsync(nc);
        }

        private void Remove_Clicked(object sender, EventArgs args)
        {
            var nc = new NewCharacter();
            Navigation.PushAsync(nc);
        }
    }
}
