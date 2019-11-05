#define USE_GRID
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
    public partial class ViewSpells : ContentPage, ISheetView
    {
        private class SpellLevelControls
        {
            public Label spellsKnown = null;
            public Label spellSaveDC = null;
            public Label level = null;
            public Label spellsPerDay = null;
            public Label bonusSpells = null;
        }

        private Page pushedPage = null;
        private List<SpellLevelControls> levels = new List<SpellLevelControls>();

        public ViewSpells()
        {
            InitializeComponent();
            CreateControls();
            UpdateView();
        }

        private void CreateControls()
        {
            Spells.Children.Clear();
#if USE_GRID
            var grid = new Grid()
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                BackgroundColor = Color.LightGray,
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = GridLength.Star, },
                new ColumnDefinition() { Width = GridLength.Star, },
                new ColumnDefinition() { Width = GridLength.Star, },
                new ColumnDefinition() { Width = GridLength.Star, },
                new ColumnDefinition() { Width = GridLength.Star, },
            };
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
                new RowDefinition() { Height = GridLength.Star, },
            };
            var column = 0;
            var row = 0;
#else
            var horzLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
#endif
            var controls = new SpellLevelControls();
            var label = CreateLabel("Spells Known:");
            controls.spellsKnown = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = CreateLabel("Spell Save DC:");
            controls.spellSaveDC = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = CreateLabel("Level:");
            controls.level = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = CreateLabel("Spells Per Day:");
            controls.spellsPerDay = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = CreateLabel("Bonus Spells:");
            controls.bonusSpells = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            levels.Add(controls);
#if !USE_GRID
            Spells.Children.Add(horzLayout);
#else
            column = 0;
            row = 1;
#endif

            for (var i = 0; i < CharacterSheet.spellLevelsCount; i++)
            {
                var level = i;
#if !USE_GRID
                horzLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                };
#endif
                controls = new SpellLevelControls();
                var frame = CreateFrame(string.Empty);
                controls.spellsKnown = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                MainPage.AddTapHandler(controls.spellsKnown, (s, e) => SpellsKnown_DoubleTap(level), 2);

                frame = CreateFrame(string.Empty);
                controls.spellSaveDC = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                MainPage.AddTapHandler(controls.spellSaveDC, (s, e) => SpellSaveDC_DoubleTap(level), 2);

                frame = CreateFrame(level.ToString());
                frame.BackgroundColor = Color.LightGray;
                controls.level = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif

                frame = CreateFrame(string.Empty);
                controls.spellsPerDay = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                MainPage.AddTapHandler(controls.spellsPerDay, (s, e) => SpellsPerDay_DoubleTap(level), 2);

                frame = CreateFrame(string.Empty);
                controls.bonusSpells = frame.Content as Label;
                if (i == 0)
                {
                    frame.BackgroundColor = Color.LightGray;
                    controls.bonusSpells.Text = "-";
                }
                else
                    MainPage.AddTapHandler(controls.bonusSpells, (s, e) => BonusSpells_DoubleTap(level), 2);
#if USE_GRID
                grid.Children.Add(frame, column++, row);
#else
                horzLayout.Children.Add(frame);
#endif

                levels.Add(controls);
#if !USE_GRID
                Spells.Children.Add(horzLayout);
#else
                column = 0;
                row += 1;
#endif
            }
#if USE_GRID
            Spells.Children.Add(grid);
#endif
        }

        private void SpellsKnown_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellsKnown, "Edit Spells Known (" + level + ")", "Spells Known: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpellSaveDC_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellSaveDC, "Edit Spell Save DC (" + level + ")", "Spell Save DC: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpellsPerDay_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellsPerDay, "Edit Spells Per Day (" + level + ")", "Spells Per Day: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void BonusSpells_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].bonusSpells, "Edit Bonus Spells (" + level + ")", "Bonus Spells: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private Label CreateLabel(string text)
        {
            return new Label()
            {
                Text = text,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            bool hasChanges = false;
            for (var i = 0; i < CharacterSheet.spellLevelsCount; i++)
            {
                var level = levels[i + 1];
                var spells = sheet.spellLevel[i];
                if (spells == null)
                {
                    spells = new CharacterSheet.SpellLevel();
                    sheet.spellLevel[i] = spells;
                    hasChanges = true;
                }
                var spellsKnown = spells.spellsKnown.GetTotal(sheet).ToString();
                if (level.spellsKnown.Text != spellsKnown)
                    level.spellsKnown.Text = spellsKnown;
                var spellSaveDC = spells.spellSaveDC.GetTotal(sheet).ToString();
                if (level.spellSaveDC.Text != spellSaveDC)
                    level.spellSaveDC.Text = spellSaveDC;
                var spellsPerDay = spells.spellsPerDay.GetTotal(sheet).ToString();
                if (level.spellsPerDay.Text != spellsPerDay)
                    level.spellsPerDay.Text = spellsPerDay;
                if (i > 0)
                {
                    var bonusSpells = spells.bonusSpells.GetTotal(sheet).ToString();
                    if (level.bonusSpells.Text != bonusSpells)
                        level.bonusSpells.Text = bonusSpells;
                }
            }
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter();
            var channelsLeft = sheet.channelsLeft.GetTotal(sheet).ToString();
            if (ChannelsLeft.Text != channelsLeft)
                ChannelsLeft.Text = channelsLeft;
            var channelsPerDay = sheet.channelsPerDay.GetTotal(sheet).ToString();
            if (ChannelsPerDay.Text != channelsPerDay)
                ChannelsPerDay.Text = channelsPerDay;
        }

        private void ChannelsLeft_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.channelsLeft, "Edit Channels Left", "Channels Left: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void ChannelsPerDay_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.channelsPerDay, "Edit Channels Per Day", "Channels Per Day: ", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }
    }
}