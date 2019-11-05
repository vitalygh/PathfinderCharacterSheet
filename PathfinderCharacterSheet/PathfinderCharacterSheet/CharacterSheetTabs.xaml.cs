//#define MOVE_TABS
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
#if MOVE_TABS
        Page[] pages = new Page[]
        {
            new ViewBackground(),
            new ViewAbilities(),
            new ViewWeapon(),
            new ViewArmor(),
            new ViewInventory(),
            new ViewSkills(),
            new ViewFeats(),
            new ViewSpecialAbilities(),
            new ViewSpells(),
            new ViewNotes(),
        };
#endif

        public CharacterSheetTabs()
        {
            InitializeComponent();
#if MOVE_TABS
            Children.Add(pages[0]);
            Children.Add(pages[1]);
            Children.Add(pages[2]);
#else
            Children.Add(new ViewBackground());
            Children.Add(new ViewAbilities());
            Children.Add(new ViewWeapon());
            Children.Add(new ViewArmor());
            Children.Add(new ViewInventory());
            Children.Add(new ViewSkills());
            Children.Add(new ViewFeats());
            Children.Add(new ViewSpecialAbilities());
            Children.Add(new ViewSpells());
            Children.Add(new ViewNotes());
#endif
        }

        public void InitTabs()
        {
#if MOVE_TABS
            foreach (var tab in pages)
            {
                var sheet = tab as ISheetView;
                if (sheet != null)
                    sheet.UpdateView();
            }
#else
            foreach (var tab in Children)
            {
                var sheet = tab as ISheetView;
                if (sheet != null)
                    sheet.UpdateView();
            }
#endif
        }

        public void MoveTabs()
        {
#if MOVE_TABS
            const int size = 3;
            var p = CurrentPage;
            var count = pages.Length;
            if (Children[0] == p)
            {
                for (var i = 1; i < count; i++)
                    if (pages[i] == p)
                    {
                        Children.RemoveAt(2);
                        Children.Insert(0, pages[i - 1]);
                        CurrentPage = p;
                        break;
                    }
            }
            else if (Children[size - 1] == p)
            {
                for (var i = count - 2; i >= 0; i--)
                    if (pages[i] == p)
                    {
                        Children.RemoveAt(0);
                        Children.Add(pages[i + 1]);
                        CurrentPage = p;
                        break;
                    }
            }
#endif
        }

        public void UpdateView()
        {
            var view = CurrentPage as ISheetView;
            if (view != null)
                view.UpdateView();
        }

    }
}