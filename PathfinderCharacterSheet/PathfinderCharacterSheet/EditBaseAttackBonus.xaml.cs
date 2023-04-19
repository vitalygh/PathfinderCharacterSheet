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
    public partial class EditBaseAttackBonus : ContentPage, ISheetView
    {
        public class AttackRow
        {
            public Label title = null;
            public Frame frame = null;
            public Label value = null;
        }

        private Page pushedPage = null;
        private List<ValueWithIntModifiers> baseAttackBonus = null;
        private int attacksCount = 0;
        private int currentAttack = 0;
        private ValueWithIntModifiers currentAttacksCount = null;
        private List<AttackRow> rows = new List<AttackRow>();
        private List<AttackRow> rowsPool = new List<AttackRow>();

        public EditBaseAttackBonus()
        {
            InitializeComponent();
        }

        public void InitEditor()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            baseAttackBonus = new List<ValueWithIntModifiers>();
            if (sheet.baseAttackBonus != null)
                foreach (var bab in sheet.baseAttackBonus)
                    if (bab != null)
                        baseAttackBonus.Add(bab.Clone as ValueWithIntModifiers);
            if (baseAttackBonus.Count <= 0)
                baseAttackBonus.Add(new ValueWithIntModifiers());
            attacksCount = baseAttackBonus.Count;
            currentAttack = sheet.currentAttack;
            currentAttacksCount = sheet.currentAttacksCount.Clone as ValueWithIntModifiers;
            UpdateView();
        }

        private void UpdateCurrentAttackPicker()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (baseAttackBonus == null)
                return;
            if (baseAttackBonus.Count <= 0)
                return;
            var items = new List<IntPickerItem>();
            var selectedIndex = -1;
            var cac = currentAttacksCount.GetTotal(sheet);
            var ac = Math.Min(attacksCount, cac);
            if (ac <= 0)
                ac = attacksCount;
            for (var i = 0; i < ac; i++)
            {
                if (i == currentAttack)
                    selectedIndex = i;
                var item = new IntPickerItem()
                {
                    Name = sheet.GetBaseAttackBonusForPicker(baseAttackBonus, i, true),
                    Value = i,
                };
                items.Add(item);
            }
            CurrentAttacksCount.Text = cac > 0 ? ac.ToString() : "(equals to attacks count)";
            CurrentAttack.ItemsSource = items;
            CurrentAttack.SelectedIndex = selectedIndex;
            var oneAttack = ac < 2;
            CurrentAttack.InputTransparent = oneAttack;
            CurrentAttackFrame.BackgroundColor = oneAttack ? Color.LightGray : Color.White;
        }

        public void UpdateView()
        {
            pushedPage = null;
            UpdateCurrentAttackPicker();
            AttacksCount.Text = attacksCount.ToString();
            var rowsCount = rows.Count;
            var update = Math.Min(rowsCount, attacksCount);
            for (var i = 0; i < update; i++)
                UpdateRow(rows[i], baseAttackBonus[i]);
            var create = rowsCount < attacksCount;
            var left = create ? attacksCount - rowsCount : rowsCount - attacksCount;
            for (int i = 0; i < left; i++)
            {
                if (create)
                    CreateRow(baseAttackBonus[i + update]);
                else
                    RemoveRow(rows[update]);
            }
            while (Attacks.RowDefinitions.Count < rows.Count)
                Attacks.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        }

        private void UpdateRow(AttackRow row, ValueWithIntModifiers bab)
        {
            if (row == null)
                return;
            if (bab == null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var value = bab.GetTotal(sheet);
            UpdateValue(row.value, value >= 0 ? "+" + value : value.ToString());
            MainPage.SetTapHandler(row.frame, (s, e) => EditBonus(bab), 1);
        }

        private void CreateRow(ValueWithIntModifiers bab)
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            if (rowsPool.Count > 0)
            {
                var row = rowsPool[0];
                rowsPool.RemoveAt(0);
                var rowIndex = rows.Count;
                rows.Add(row);
                UpdateRow(row, bab);
                Attacks.Children.Add(row.title, 0, rowIndex);
                Attacks.Children.Add(row.frame, 1, rowIndex);
                return;
            }
            var title = CreateLabel("Attack " + (rows.Count + 1) + " Bonus:");
            var bonus = bab.GetTotal(sheet);
            var frame = CreateFrame(bonus >= 0 ? "+" + bonus : bonus.ToString());
            var value = frame.Content as Label;
            value.TextDecorations = TextDecorations.Underline;
            var newRow = new AttackRow()
            {
                title = title,
                frame = frame,
                value = value,
            };
            var newRowIndex = rows.Count;
            rows.Add(newRow);
            MainPage.SetTapHandler(frame, (s, e) => EditBonus(bab), 1);
            Attacks.Children.Add(newRow.title, 0, newRowIndex);
            Attacks.Children.Add(newRow.frame, 1, newRowIndex);
        }

        private void RemoveRow(AttackRow row)
        {
            if (row == null)
                return;
            Attacks.Children.Remove(row.title);
            Attacks.Children.Remove(row.frame);
            rows.Remove(row);
            rowsPool.Add(row);
        }

        private void EditBonus(ValueWithIntModifiers bab)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, bab, "Edit Base Attack Bonus", "Base Attack Bonus", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private Label CreateLabel(string text, TextAlignment horz = TextAlignment.Start)
        {
            return MainPage.CreateLabel(text, horz);
        }

        private Frame CreateFrame(string text)
        {
            return MainPage.CreateFrame(text);
        }

        private void UpdateValue(Label label, string text)
        {
            if (label == null)
                return;
            if (label.Text != text)
                label.Text = text;
        }

        private void EditToView()
        {
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var bab = baseAttackBonus.GetRange(0, attacksCount);
            if ((currentAttack != sheet.currentAttack) ||
                !CharacterSheet.IsEqual(bab, sheet.baseAttackBonus) ||
                !currentAttacksCount.Equals(sheet.currentAttacksCount))
            {
                sheet.baseAttackBonus = bab;
                sheet.currentAttack = currentAttack;
                sheet.currentAttacksCount.Fill(currentAttacksCount);
                CharacterSheetStorage.Instance.SaveCharacter();
            }
        }

        private void CurrentAttacksCount_Tapped(object sender, EventArgs e)
        {
            if (pushedPage != null)
                return;
            var sheet = CharacterSheetStorage.Instance.selectedCharacter;
            if (sheet == null)
                return;
            var eivwm = new EditIntValueWithModifiers();
            eivwm.Init(sheet, currentAttacksCount, "Edit Base Attack Bonus", "Attacks Count", false);
            pushedPage = eivwm;
            Navigation.PushAsync(eivwm);
        }

        private void AttacksCount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AttacksCount_Unfocused(object sender, FocusEventArgs e)
        {
            UpdateAttacksCount();
        }

        private void AttacksCount_Completed(object sender, EventArgs e)
        {
            UpdateAttacksCount();
        }

        private void UpdateAttacksCount()
        {
            if (baseAttackBonus == null)
                return;
            if (baseAttackBonus.Count <= 0)
                return;
            var ac = attacksCount;
            if (!MainPage.StrToInt(AttacksCount.Text, ref ac))
                return;
            var maxEnlarge = 10;
            ac = Math.Min(Math.Max(1, ac), attacksCount + maxEnlarge);
            if (ac > attacksCount)
            {
                var count = baseAttackBonus.Count;
                var bab = baseAttackBonus[count - 1];
                while (baseAttackBonus.Count < ac)
                    baseAttackBonus.Add(bab.Clone as ValueWithIntModifiers);
                attacksCount = ac;
            }
            else if (ac < attacksCount)
                attacksCount = ac;
            if (currentAttack >= attacksCount)
                currentAttack = attacksCount - 1;
            UpdateView();
        }

        private void CurrentAttack_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (CurrentAttack.SelectedItem as IntPickerItem);
            if (selectedItem == null)
                return;
            currentAttack = selectedItem.Value;
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