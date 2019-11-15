//#define MOVE_TABS
#define MOVE_TABS_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterSheetTabs : Xamarin.Forms.TabbedPage, ISheetView
    {
        private const int visibleTabsLimit = 5;
        private static readonly List<Page> pages = new List<Page>()
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

        private static bool NeedMoveTabs
        {
            get
            {
#if MOVE_TABS
                return true;
#elif MOVE_TABS_IOS
                return Device.RuntimePlatform == Device.iOS;
#else
                return false;
#endif
            }
        }


        public CharacterSheetTabs()
        {
            InitializeComponent();
            On<Android>().SetOffscreenPageLimit(pages.Count);
            var startTabsCount = NeedMoveTabs ? visibleTabsLimit : pages.Count;
            for (var i = 0; i < startTabsCount; i++)
                Children.Add(pages[i]);
        }

        public void InitTabs()
        {
            foreach (var tab in pages)
            {
                var view = tab as ISheetView;
                if (view != null)
                    view.UpdateView();
            }
        }

        private List<Page> GetVisiblePages()
        {
            var index = pages.IndexOf(CurrentPage);
            var count = pages.Count;
            var center = Math.Min(count, visibleTabsLimit) / 2;
            var visiblePages = new List<Page>();
            var start = index - center;
            start = Math.Max(0, start);
            start = Math.Min(Math.Max(0, count - visibleTabsLimit), start);
            var end = start + visibleTabsLimit;
            end = Math.Min(count, end);
            for (var i = start; i < end; i++)
                visiblePages.Add(pages[i]);
            return visiblePages;
        }

        public void MoveTabs()
        {
            if (!NeedMoveTabs)
                return;
            var vp = GetVisiblePages();
            if (vp[0] == Children[0])
                return;
            var cp = CurrentPage;
            Children.Clear();
            for (var i = 0; i < vp.Count; i++)
                Children.Add(vp[i]);
            CurrentPage = cp;
        }

        public void UpdateView()
        {
            var view = CurrentPage as ISheetView;
            if (view != null)
                view.UpdateView();
        }

    }
}