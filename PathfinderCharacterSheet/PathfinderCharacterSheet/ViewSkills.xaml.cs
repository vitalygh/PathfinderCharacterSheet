//#define USE_GRID
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
#if !USE_GRID
            public StackLayout layout = null;
#endif
            public CheckBox classSkill = null;
            public Frame nameFrame = null;
            public Label name = null;
            public Frame totalFrame = null;
            public Label total = null;
        }

        private Page pushedPage = null;
        private List<SkillRow> skillRows = new List<SkillRow>();

        public ViewSkills()
        {
            InitializeComponent();
            MainPage.AddTapHandler(SkillRanksLayout, SkillRanks_DoubleTapped, 2);
            MainPage.AddTapHandler(LanguagesLayout, Languages_DoubleTapped, 2);
            UpdateView();
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
#if USE_GRID
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Auto, },
                new ColumnDefinition() { Width = GridLength.Star, },
                new ColumnDefinition() { Width = GridLength.Auto, },
            };
            Skills.Children.Add(grid);
#endif
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
                UpdateValue(row.classSkill, skill.classSkill);
                UpdateValue(row.name, skill.Name);
                row.name.TextColor = (skill.trainedOnly && (skill.rank.GetTotal(sheet) <= 0)) ? Color.Red : Color.Black;
                UpdateValue(row.total, skill.GetTotal(sheet).ToString());
                EventHandler handler = (s, e) => Skill_DoubleTap(skill, skillIndex);
                MainPage.SetTapHandler(row.nameFrame, handler, 2);
                MainPage.SetTapHandler(row.totalFrame, handler, 2);
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
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                    };
#if !USE_GRID
                    skillRow.layout = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                    };
#endif
                    skillRow.nameFrame = CreateFrame(skill.Name);
                    skillRow.nameFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
                    skillRow.name = skillRow.nameFrame.Content as Label;
                    skillRow.name.TextColor = (skill.trainedOnly && (skill.rank.GetTotal(sheet) <= 0)) ? Color.Red : Color.Black;
                    skillRow.totalFrame = CreateFrame(skill.GetTotal(sheet).ToString());
                    skillRow.totalFrame.HorizontalOptions = LayoutOptions.End;
                    skillRow.totalFrame.WidthRequest = 40;
                    skillRow.total = skillRow.totalFrame.Content as Label;
                    EventHandler handler = (s, e) => Skill_DoubleTap(skill, skillIndex);
                    MainPage.AddTapHandler(skillRow.nameFrame, handler, 2);
                    MainPage.AddTapHandler(skillRow.totalFrame, handler, 2);
                    skillRows.Add(skillRow);
#if USE_GRID
                    var row = skillIndex + 1;
                    grid.Children.Add(skillRow.classSkill, 0, row);
                    grid.Children.Add(skillRow.nameFrame, 1, row);
                    grid.Children.Add(skillRow.totalFrame, 2, row);
#else
                    skillRow.layout.Children.Add(skillRow.classSkill);
                    skillRow.layout.Children.Add(skillRow.nameFrame);
                    skillRow.layout.Children.Add(skillRow.totalFrame);
                    Skills.Children.Add(skillRow.layout);
#endif
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
#if USE_GRID
            rowsCount = skillRows.Count + 1;
            if ((grid.RowDefinitions == null) || (grid.RowDefinitions.Count != rowsCount))
            {
                grid.RowDefinitions = new RowDefinitionCollection();
                for (int i = 0; i < rowsCount; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = GridLength.Auto,
                    });
                }
            }
#endif
            var ranksSpent = 0;
            foreach (var skill in sheet.skills)
                ranksSpent += skill.rank.GetTotal(sheet);
            var ranksLeft = sheet.skillRanks.GetTotal(sheet) - ranksSpent;
            SkillRanksLeft.Text = ranksLeft.ToString();
            if (ranksLeft < 0)
                SkillRanksLeft.TextColor = Color.Red;
            else if (ranksLeft > 0)
                SkillRanksLeft.TextColor = Color.Green;
            else
                SkillRanksLeft.TextColor = Color.Black;
            Languages.Text = sheet.Languages;
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void UpdateValue(CheckBox checkbox, bool value)
        {
            if (checkbox == null)
                return;
            if (checkbox.IsChecked != value)
                checkbox.IsChecked = value;
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
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

        private void SkillRanks_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var esr = new EditSkillRanks();
            esr.InitEditor();
            pushedPage = esr;
            Navigation.PushAsync(pushedPage);
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