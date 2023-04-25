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
        private static readonly int visibleTabsLimit = 5;
        private static Page[] availablePages = null;
        private static Page[] Pages
        {
            get
            {
                if (availablePages == null)
                {
                    var availablePagesList = new List<Page>()
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
                    availablePages = availablePagesList.ToArray();
                }
                return availablePages;
            }
        }

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
            On<Android>().SetOffscreenPageLimit(Pages.Length);
            var startTabsCount = NeedMoveTabs ? visibleTabsLimit : Pages.Length;
            for (var i = 0; i < startTabsCount; i++)
                Children.Add(Pages[i]);
        }

        public void InitTabs()
        {
            foreach (var tab in Pages)
            {
                if (tab is ISheetView sheetView)
                    sheetView.UpdateView();
            }
        }

        private Page[] GetVisiblePages()
        {
            var index = Array.IndexOf(Pages, CurrentPage);
            var count = Pages.Length;
            var center = Math.Min(count, visibleTabsLimit) / 2;
            var visiblePages = new List<Page>();
            var start = index - center;
            start = Math.Max(0, start);
            start = Math.Min(Math.Max(0, count - visibleTabsLimit), start);
            var end = start + visibleTabsLimit;
            end = Math.Min(count, end);
            for (var i = start; i < end; i++)
                visiblePages.Add(Pages[i]);
            return visiblePages.ToArray();
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
            for (var i = 0; i < vp.Length; i++)
                Children.Add(vp[i]);
            CurrentPage = cp;
        }

        public void UpdateView()
        {
            if (CurrentPage is ISheetView sheetView)
                sheetView.UpdateView();
        }

    }
}