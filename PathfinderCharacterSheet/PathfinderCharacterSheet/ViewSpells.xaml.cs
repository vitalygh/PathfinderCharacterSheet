#define USE_GRID
using PathfinderCharacterSheet.CharacterSheets.V1;
using System;
using System.Collections.Generic;
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
        private readonly List<SpellLevelControls> levels = new List<SpellLevelControls>();

        public ViewSpells()
        {
            InitializeComponent();
            CreateControls();
            UIHelpers.AddTapHandler(ChannelsTitle, Channels_DoubleTapped, 2);
            UIHelpers.AddTapHandler(ChannelsFrame, Channels_DoubleTapped, 2);
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
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
                new RowDefinition() { Height = GridLength.Auto, },
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
            var label = UIHelpers.CreateLabel("Spells Known", TextAlignment.Center);
            controls.spellsKnown = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = UIHelpers.CreateLabel("Spell Save DC", TextAlignment.Center);
            controls.spellSaveDC = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = UIHelpers.CreateLabel("Level", TextAlignment.Center);
            controls.level = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = UIHelpers.CreateLabel("Spells Per Day", TextAlignment.Center);
            controls.spellsPerDay = label;
#if USE_GRID
            grid.Children.Add(label, column, row);
            column += 1;
#else
            horzLayout.Children.Add(label);
#endif

            label = UIHelpers.CreateLabel("Bonus Spells", TextAlignment.Center);
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
                var frame = UIHelpers.CreateFrame(string.Empty);
                controls.spellsKnown = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                UIHelpers.AddTapHandler(frame, (s, e) => SpellsKnown_DoubleTap(level), 2);

                frame = UIHelpers.CreateFrame(string.Empty);
                controls.spellSaveDC = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                UIHelpers.AddTapHandler(frame, (s, e) => SpellSaveDC_DoubleTap(level), 2);

                frame = UIHelpers.CreateFrame(level.ToString());
                frame.BackgroundColor = Color.LightGray;
                controls.level = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif

                frame = UIHelpers.CreateFrame(string.Empty);
                controls.spellsPerDay = frame.Content as Label;
#if USE_GRID
                grid.Children.Add(frame, column, row);
                column += 1;
#else
                horzLayout.Children.Add(frame);
#endif
                UIHelpers.AddTapHandler(frame, (s, e) => SpellsPerDay_DoubleTap(level), 2);

                frame = UIHelpers.CreateFrame(string.Empty);
                controls.bonusSpells = frame.Content as Label;
                if (i == 0)
                {
                    frame.BackgroundColor = Color.LightGray;
                    controls.bonusSpells.Text = "-";
                }
                else
                    UIHelpers.AddTapHandler(frame, (s, e) => BonusSpells_DoubleTap(level), 2);
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
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellsKnown, "Edit Spells Known (" + level + ")", "Spells Known", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpellSaveDC_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellSaveDC, "Edit Spell Save DC (" + level + ")", "Spell Save DC", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void SpellsPerDay_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].spellsPerDay, "Edit Spells Per Day (" + level + ")", "Spells Per Day", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void BonusSpells_DoubleTap(int level)
        {
            if (pushedPage != null)
                return;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            if (level < 0)
                return;
            if (level >= sheet.spellLevel.Length)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, sheet.spellLevel[level].bonusSpells, "Edit Bonus Spells (" + level + ")", "Bonus Spells", true);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        public void UpdateView()
        {
            pushedPage = null;
            var sheet = UIMediator.GetSelectedCharacter?.Invoke();
            if (sheet == null)
                return;
            bool hasChanges = false;
            for (var i = 0; i < CharacterSheet.spellLevelsCount; i++)
            {
                var level = levels[i + 1];
                var spells = sheet.spellLevel[i];
                if (spells == null)
                {
                    spells = new SpellLevel();
                    sheet.spellLevel[i] = spells;
                    hasChanges = true;
                }
                var spellsKnown = spells.spellsKnown.GetValue(sheet).ToString();
                UIHelpers.UpdateValue(level.spellsKnown, spellsKnown);
                var spellSaveDC = spells.spellSaveDC.GetValue(sheet).ToString();
                UIHelpers.UpdateValue(level.spellSaveDC, spellSaveDC);
                var spellsPerDay = spells.spellsPerDay.GetValue(sheet).ToString();
                UIHelpers.UpdateValue(level.spellsPerDay, spellsPerDay);
                if (i > 0)
                {
                    var bonusSpells = spells.bonusSpells.GetValue(sheet).ToString();
                    UIHelpers.UpdateValue(level.bonusSpells, bonusSpells);
                }
            }
            if (hasChanges)
                UIMediator.OnCharacterSheetChanged?.Invoke();
            var points = sheet.channelEnergy.points.AsString(sheet);
            var channels = "Channels";
            if (!string.IsNullOrWhiteSpace(points))
                channels += " " + points;
            channels += ":";
            UIHelpers.UpdateValue(ChannelsTitle, channels);
            var left = sheet.channelEnergy.left.GetValue(sheet);
            var total = sheet.channelEnergy.total.GetValue(sheet);
            UIHelpers.UpdateValue(Channels, left + " / " + total);
        }

        private void Channels_DoubleTapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var ece = new EditChannelEnergy();
            ece.InitEditor();
            pushedPage = ece;
            Navigation.PushAsync(ece);
        }
    }
}