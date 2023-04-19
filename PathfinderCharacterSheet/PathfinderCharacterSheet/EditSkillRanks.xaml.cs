using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PathfinderCharacterSheet.CharacterSheets.V1;

namespace PathfinderCharacterSheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSkillRanks : ContentPage, ISheetView
    {
        private Page pushedPage = null;
        private ValueWithIntModifiers skillRanks = null;

        public EditSkillRanks()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            skillRanks = sheet.skillRanks.Clone as ValueWithIntModifiers;
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var ranksSpent = 0;
            foreach (var skill in sheet.skills)
                ranksSpent += skill.rank.GetTotal(sheet);
            var ranks = skillRanks.GetTotal(sheet);
            var ranksLeft = ranks - ranksSpent;
            Left.Text = ranksLeft.ToString();
            Ranks.Text = ranks.ToString();
            Spent.Text = ranksSpent.ToString();
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (!sheet.skillRanks.Equals(skillRanks))
            {
                sheet.skillRanks.Fill(skillRanks);
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void Ranks_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, skillRanks, "Edit Skill Ranks", "Skill Ranks", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            Navigation.PopAsync();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            pushedPage = this;
            EditToView();
            Navigation.PopAsync();
        }
    }
}