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
            Children.Add(new ViewWeapon());
            Children.Add(new ViewArmor());
            Children.Add(new ViewInventory());
            Children.Add(new ViewSkills());
            Children.Add(new ViewFeats());
            Children.Add(new ViewSpecialAbilities());
            Children.Add(new ContentPage() { Title = "Spells" });
            Children.Add(new ViewNotes());
        }

        public void UpdateView()
        {
            var view = CurrentPage as ISheetView;
            if (view != null)
                view.UpdateView();
        }

    }
}