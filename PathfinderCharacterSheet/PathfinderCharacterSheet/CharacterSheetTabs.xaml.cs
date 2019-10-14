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
    public partial class CharacterSheetTabs : TabbedPage
    {
        public CharacterSheetTabs()
        {
            InitializeComponent();
            UpdateFields();
        }

        public void UpdateFields()
        {
            var c = CharacterSheetStorage.Instance.selectedCharacter;
            Name.Text = c.Name;
            Alignment.Text = c.alignment.ToString();
            Experience.Text = c.experience.ToString();
            NextLevel.Text = c.nextLevelExperience.ToString();
            var level = string.Empty;
            if (c.levelOfClass != null)
                foreach (var loc in c.levelOfClass)
                {
                    if (level.Length > 0)
                        level += ", ";
                    level += string.Format("({0}) {1}", loc.level, loc.className);
                }
            Level.Text = level;
            Deity.Text = c.deity;
            Homeland.Text = c.homeland;
            Race.Text = c.Race;
            Size.Text = c.size;
            Gender.Text = c.gender;
            Age.Text = c.age.ToString();
            Height.Text = c.height.ToString();
            Weight.Text = c.weight.ToString();
            Hair.Text = c.hair;
            Eyes.Text = c.eyes;
            Info.Text = c.info;
        }
    }
}