using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterSheetTabs : TabbedPage, ISheetView
    {
        public CharacterSheetTabs()
        {
            InitializeComponent();

            Children.Add(new ViewBackground());
            Children.Add(new ViewAbilities());
            Children.Add(new ContentPage() { Title = "Weapon" });
            Children.Add(new ContentPage() { Title = "AC Items" });
            Children.Add(new ContentPage() { Title = "Inventory" });
            Children.Add(new ContentPage() { Title = "Skills" });
            Children.Add(new ContentPage() { Title = "Feats" });
            Children.Add(new ContentPage() { Title = "Special Abilities" });
            Children.Add(new ContentPage() { Title = "Spells" });
            Children.Add(new ContentPage() { Title = "Notes" });
        }

        public void UpdateView()
        {
            var view = CurrentPage as ISheetView;
            if (view != null)
                view.UpdateView();
        }

    }
}