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
    public partial class ViewSkills : ContentPage, ISheetView
    {
        public class SkillRow
        {
            public CheckBox classSkill = null;
            public Frame nameFrame = null;
            public Label name = null;
            public Frame totalFrame = null;
            public Label total = null;
        }

        private Page pushedPage = null;
        private Label skillNameTitle = null;
        private Label skillTotalTitle = null;
        private List<SkillRow> skillRows = new List<SkillRow>();

        public ViewSkills()
        {
            InitializeComponent();
            CreateControls();
            UpdateView();
        }

        private void CreateControls()
        {
            if (Skills.Children.Count <= 0)
            {
                skillNameTitle = CreateLabel("Name:");
                skillTotalTitle = CreateLabel("Total:");
                Skills.Children.Add(skillNameTitle, 1, 0);
                Skills.Children.Add(skillTotalTitle, 2, 0);
            }
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var skillsCount = sheet.skills.Count;
            var rowsCount = skillRows.Count;
            var updateCount = Math.Min(skillsCount, rowsCount);
            var index = skillsCount;
            var hasChanges = false;
            while (--index >= 0)
                if (sheet.skills[index] == null)
                {
                    sheet.skills.RemoveAt(index);
                    hasChanges = true;
                }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
            for (var i = 0; i < updateCount; i++)
            {
                var skillIndex = i;
                var row = skillRows[i];
                var skill = sheet.skills[i];
                row.classSkill.IsChecked = skill.classSkill;
                row.name.Text = skill.Name;
                row.name.TextColor = (skill.trainedOnly && (skill.rank.GetTotal(sheet) <= 0)) ? Color.Red : Color.Black;
                row.total.Text = skill.GetTotal(sheet).ToString();
                EventHandler handler = (s, e) => Skill_DoubleTap(skill, skillIndex);
                row.nameFrame.GestureRecognizers.Clear();
                MainPage.AddTapHandler(row.nameFrame, handler, 2);
                row.totalFrame.GestureRecognizers.Clear();
                MainPage.AddTapHandler(row.totalFrame, handler, 2);
            }
            var count = skillsCount - rowsCount;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var skillIndex = updateCount + i;
                    var skill = sheet.skills[skillIndex];
                    var skillRow = new SkillRow();
                    skillRow.classSkill = new CheckBox()
                    {
                        IsChecked = skill.classSkill,
                        IsEnabled = false,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    skillRow.nameFrame = CreateFrame(skill.Name);
                    skillRow.name = skillRow.nameFrame.Content as Label;
                    skillRow.name.TextColor = (skill.trainedOnly && (skill.rank.GetTotal(sheet) <= 0)) ? Color.Red : Color.Black;
                    skillRow.totalFrame = CreateFrame(skill.GetTotal(sheet).ToString());
                    skillRow.total = skillRow.totalFrame.Content as Label;
                    EventHandler handler = (s, e) => Skill_DoubleTap(skill, skillIndex);
                    MainPage.AddTapHandler(skillRow.nameFrame, handler, 2);
                    MainPage.AddTapHandler(skillRow.totalFrame, handler, 2);
                    skillRows.Add(skillRow);
                    var row = skillIndex + 1;
                    Skills.Children.Add(skillRow.classSkill, 0, row);
                    Skills.Children.Add(skillRow.nameFrame, 1, row);
                    Skills.Children.Add(skillRow.totalFrame, 2, row);
                }
            }
            else if (count < 0)
            {
                while (skillRows.Count > sheet.skills.Count)
                {
                    var last = skillRows.Count - 1;
                    var skillRow = skillRows[last];
                    skillRows.RemoveAt(last);
                    Skills.Children.Remove(skillRow.totalFrame);
                    Skills.Children.Remove(skillRow.nameFrame);
                    Skills.Children.Remove(skillRow.classSkill);
                }
            }
            rowsCount = skillRows.Count + 1;
            if ((Skills.RowDefinitions == null) || (Skills.RowDefinitions.Count != rowsCount))
            {
                Skills.RowDefinitions = new RowDefinitionCollection();
                for (int i = 0; i < rowsCount; i++)
                {
                    Skills.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = GridLength.Auto,
                    });
                }
            }
            Languages.Text = sheet.Languages;
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Center)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        public void Skill_DoubleTap(CharacterSheet.SkillRank skill = null, int index = -1)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var es = new EditSkill();
            es.InitEditor(skill, index);
            pushedPage = es;
            Navigation.PushAsync(pushedPage);
        }

        private void AddSkill_Clicked(object sender, EventArgs e)
        {
            Skill_DoubleTap();
        }

        private void Languages_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var el = new EditLanguages();
            el.InitEditor();
            pushedPage = el;
            Navigation.PushAsync(pushedPage);
        }
    }
}