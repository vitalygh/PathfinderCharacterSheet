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
	public partial class EditCritical : ContentPage, ISheetView
	{
        private CharacterSheet sheet = null;
        private CharacterSheet.CriticalHit source = null;
        private CharacterSheet.CriticalHit critical = null;

        public EditCritical ()
		{
			InitializeComponent ();
		}

        public void Init(CharacterSheet sheet, CharacterSheet.CriticalHit critical)
        {
            if (sheet == null)
                return;
            if (critical == null)
                return;
            this.sheet = sheet;
            source = critical;
            this.critical = critical.Clone as CharacterSheet.CriticalHit;
            UpdateView();
        }

        public void UpdateView()
        {

        }
    }
}