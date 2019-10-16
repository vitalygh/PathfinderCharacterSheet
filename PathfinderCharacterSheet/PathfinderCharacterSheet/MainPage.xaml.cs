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
        public MainPage()
        {
            InitializeComponent();
            CharacterSheetStorage.Instance.LoadCharacters();
            UpdateListView();
        }

        public void UpdateListView()
        {
            Characters.IsVisible = true;
            Characters.ItemsSource = null;
            Characters.ItemsSource = CharacterSheetStorage.Instance.characters.Keys;
        }

        private void Add_Clicked(object sender, EventArgs args)
        {
            Characters.IsVisible = false;
            var nc = new NewCharacter();
            Navigation.PushAsync(nc);
        }

        private void Characters_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Characters.IsVisible = false;
            CharacterSheetStorage.Instance.selectedCharacter = e.Item as CharacterSheet;
            var cst = new CharacterSheetTabs();
            cst.Title = CharacterSheetStorage.Instance.selectedCharacter.Name;
            Navigation.PushAsync(cst);
        }

        public static bool StrToInt(string from, ref int to)
        {
            var i = 0;
            if (int.TryParse(from, out i))
            {
                var changed = to != i;
                to = i;
                return changed;
            }
            return false;
        }
    }
}
