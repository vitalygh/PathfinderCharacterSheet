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
	public partial class EditDiceRoll : ContentPage
	{
        private CharacterSheet sheet = null;
        private CharacterSheet.DiceRoll source = null;
        private CharacterSheet.DiceRoll roll = null;

        public EditDiceRoll()
        {
            InitializeComponent();
        }

        public void Init(CharacterSheet sheet, CharacterSheet.DiceRoll roll)
        {
            if (sheet == null)
                return;
            if (roll == null)
                return;
            this.sheet = sheet;
            source = roll;
            this.roll = roll.Clone as CharacterSheet.DiceRoll;
            UpdateView();
        }

        public void UpdateView()
        {

        }
    }
}