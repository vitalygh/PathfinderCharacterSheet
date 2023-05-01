//#define SELECT_CURRENT_ATTACK_FOR_COMBAT_MANEUVERS
#define SAVE_DELTA
using System;
using System.Collections.Generic;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class CharacterSheet
    {
        public int GetAbilityModifier(Ability ability)
        {
            int ab = (int)ability;
            if ((ab >= 0) && (ab < abilityScores.Length))
                return abilityScores[ab].GetTempModifier(this);
            return 0;
        }

        public CharacterSheet()
        {

        }

        public static CharacterSheet Create(string name)
        {
            var characterSheet = new CharacterSheet();
            characterSheet.InitSkills();
            characterSheet.name = name;
            characterSheet.CreationTime = DateTime.Now;
            return characterSheet;
        }

        #region Technical info
        public DateTime CreationTime = DateTime.Now;
        public DateTime ModificationTime = DateTime.Now;
        public int currentUID = 0;
        public const int InvalidUID = -1;
        public int GetUID() { return currentUID++; }
        #endregion

        #region Character Background
        public string name = null;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return string.Empty;
                return name;
            }
        }
        public string biography = null;
        public const Alignment DefaultAlignment = Alignment.Neutral; // DO NOT CHANGE
        public string alignment
        {
            get =>
#if SAVE_DELTA
                DefaultAlignment == Alignment ? null :
#endif
                Alignment.ToString();
            set => Alignment = Helpers.GetEnumValue(value, DefaultAlignment);
        }
        internal Alignment Alignment { get; set; } = DefaultAlignment;
        public string deity = null;
        public string homeland = null;
        public string race = null;
        public string Race
        {
            get
            {
                if (string.IsNullOrEmpty(race))
                    return string.Empty;
                return race;
            }
        }
        public string size = null;
        public string gender = null;
        public string age = null;
        public string height = null;
        public string weight = null;
        public string hair = null;
        public string eyes = null;
        #endregion

        #region Level
        public LevelOfClassList levelOfClass = new LevelOfClassList();
        public int TotalLevel { get { return levelOfClass.GetValue(this); } }
        public ValueWithIntModifiers GetLevelOfClass(string className)
        {
            foreach (var loc in levelOfClass)
                if ((loc != null) && (loc.className == className))
                    return loc.level;
            return null;
        }
        public string LevelAsString { get { return levelOfClass.AsString(this); } }
        public ValueWithIntModifiers experience = new ValueWithIntModifiers();
        public ValueWithIntModifiers nextLevelExperience = new ValueWithIntModifiers();
        #endregion

        #region Ability Score
        public AbilityScore[] abilityScores = new AbilityScore[(int)Ability.Total].Populate();
        #endregion

        #region Hit Points
        public HP hp = new HP();
        #endregion

        #region Speed
        public Speed speed = new Speed();
        #endregion

        #region Defence
        public ArmorClass armorClass = new ArmorClass();
        public int ACArmorBonus { get { return armorClass.GetArmorBonus(this); } }
        public int ACShieldBonus { get { return armorClass.GetShieldBonus(this); } }
        public int ACTotal { get { return armorClass.GetTotal(this); } }
        public int ACTouch { get { return armorClass.GetTouch(this); } }
        public int ACFlatFooted { get { return armorClass.GetFlatFooted(this); } }

        public SavingThrow[] savingThrows = new SavingThrow[(int)Save.Total]
        {
            new SavingThrow(Ability.Constitution),
            new SavingThrow(Ability.Dexterity),
            new SavingThrow(Ability.Wisdom),
        };

        public SavingThrow GetSavingThrow(Save save)
        {
            var st = (int)save;
            if ((st < 0) || (st >= savingThrows.Length))
                return null;
            return savingThrows[st];
        }


        public int GetSavingThrowTotal(Save save)
        {
            var savingThrow = GetSavingThrow(save);
            if (savingThrow != null)
                return savingThrow.GetTotal(this);
            return 0;
        }

        public ValueWithIntModifiers spellResistance = new ValueWithIntModifiers();
        #endregion

        #region Attack
        public Initiative initiative = new Initiative();
        public int CurrentInitiative { get { return initiative.GetInitiative(this); } }


        public ValueWithIntModifiers currentAttacksCount = new ValueWithIntModifiers();
        public int AttacksCount
        {
            get
            {
                var oac = Math.Min(baseAttackBonus.Count, currentAttacksCount.GetValue(this));
                if (oac <= 0)
                    return baseAttackBonus.Count;
                return oac;
            }
        }

        public List<ValueWithIntModifiers> baseAttackBonus = new List<ValueWithIntModifiers>();
        public int currentAttack = 0;
        public int GetBaseAttackBonus()
        {
            return GetBaseAttackBonus(currentAttack);
        }
        public int GetBaseAttackBonus(int attack)
        {
            return GetBaseAttackBonus(baseAttackBonus, attack);
        }
        public int GetBaseAttackBonus(List<ValueWithIntModifiers> babs, int attack)
        {
            var count = babs.Count;
            if (count <= 0)
                return 0;
            ValueWithIntModifiers bab;
            if (attack < 0)
                bab = babs[0];
            else if (attack >= count)
                bab = babs[count - 1];
            else
                bab = babs[attack];
            if (bab != null)
                return bab.GetValue(this);
            return 0;
        }
        public string GetBaseAttackBonusForPicker(int attack)
        {
            return GetBaseAttackBonusForPicker(baseAttackBonus, attack);
        }
        public string GetBaseAttackBonusForPicker(List<ValueWithIntModifiers> babs, int attack, bool addAttackNumber = false)
        {
            var bab = GetBaseAttackBonus(babs, attack);
            var text = bab > 0 ? "+" + bab : bab.ToString();
            if (addAttackNumber && (babs != null) && (babs.Count > 1))
                text = attack + 1 + ": (" + text + ")";
            return text;
        }

        public ValueWithIntModifiers cmdSizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers cmbSizeModifier = new ValueWithIntModifiers();
        public int GetCMD(CharacterSheet sheet, ValueWithIntModifiers sizeModifier, int attack) { return 10 + GetBaseAttackBonus(attack) + GetAbilityModifier(Ability.Strength) + GetAbilityModifier(Ability.Dexterity) + sizeModifier.GetValue(sheet); }
        public int GetCMB(CharacterSheet sheet, ValueWithIntModifiers sizeModifier, int attack) { return GetBaseAttackBonus(attack) + GetAbilityModifier(Ability.Strength) + sizeModifier.GetValue(sheet); }
#if SELECT_CURRENT_ATTACK_FOR_COMBAT_MANEUVERS
public int CMD { get { return GetCMD(this, cmdSizeModifier, currentAttack); } }
public int CMB { get { return GetCMB(this, cmbSizeModifier, currentAttack); } }
#else
        public int CMD { get { return GetCMD(this, cmdSizeModifier, 0); } }
        public int CMB { get { return GetCMB(this, cmbSizeModifier, 0); } }
#endif

        public ValueWithIntModifiers attackSizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers attackBonusModifiers = new ValueWithIntModifiers();
        public string GetAttackBonus(int weaponBonus = 0)
        {
            var total = attackBonusModifiers.GetValue(this);
            total += attackSizeModifier.GetValue(this); ;
            total += weaponBonus;
            var count = AttacksCount;
            var values = string.Empty;
            if (count <= 0)
                values = weaponBonus >= 0 ? "+" + weaponBonus : weaponBonus.ToString();
            else
                for (var i = 0; i < count; i++)
                {
                    var bab = GetBaseAttackBonus(i);
                    bab += total;
                    if (values.Length > 0)
                        values += ", ";
                    var sbab = bab >= 0 ? "+" + bab : bab.ToString();
                    values += ((count > 1) && (i == currentAttack)) ? "[" + sbab + "]" : sbab;
                }
            return values;
        }
        public int AttackBonus { get { return GetBaseAttackBonus() + attackBonusModifiers.GetValue(this) + attackSizeModifier.GetValue(this); } }
        public ValueWithIntModifiers damageBonusModifiers = new ValueWithIntModifiers();
        public int DamageBonus { get { return damageBonusModifiers.GetValue(this); } }
        #endregion

        #region Weapons
        public List<WeaponItem> weaponItems = new List<WeaponItem>();
        #endregion

        #region AC Items
        public List<ArmorClassItem> armorClassItems = new List<ArmorClassItem>();
        public int CheckPenalty()
        {
            var sum = 0;
            foreach (var aci in armorClassItems)
                if (aci != null)
                    sum += aci.checkPenalty.GetValue(this);
            return sum;
        }
        #endregion

        #region Skills
        public ValueWithIntModifiers skillRanks = new ValueWithIntModifiers();
        public List<SkillRank> skills = null;
        private void InitSkills()
        {
            skills = new List<SkillRank>()
            {
                new SkillRank(Skill.Acrobatics, Ability.Dexterity),
                new SkillRank(Skill.Appraise, Ability.Intelligence),
                new SkillRank(Skill.Bluff, Ability.Charisma),
                new SkillRank(Skill.Climb, Ability.Strength),
                new SkillRank(Skill.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skill.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skill.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skill.Diplomacy, Ability.Charisma),
                new SkillRank(Skill.DisableDevice, Ability.Dexterity, true),
                new SkillRank(Skill.Disguise, Ability.Charisma),
                new SkillRank(Skill.EscapeArtist, Ability.Dexterity),
                new SkillRank(Skill.Fly, Ability.Dexterity),
                new SkillRank(Skill.HandleAnimal, Ability.Charisma, true),
                new SkillRank(Skill.Heal, Ability.Wisdom),
                new SkillRank(Skill.Intimidate, Ability.Charisma),
                new SkillRank(Skill.KnowledgeOfArcana, Ability.Intelligence),
                new SkillRank(Skill.KnowledgeOfDungeoneering, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfEngineering, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfGeography, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfHistory, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfLocal, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfNature, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfNobility, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfPlanes, Ability.Intelligence, true),
                new SkillRank(Skill.KnowledgeOfReligion, Ability.Intelligence, true),
                new SkillRank(Skill.Linguistics, Ability.Intelligence, true),
                new SkillRank(Skill.Perception, Ability.Wisdom),
                new SkillRank(Skill.Perform, Ability.Charisma, false, true),
                new SkillRank(Skill.Perform, Ability.Charisma, false, true),
                new SkillRank(Skill.Profession, Ability.Wisdom, true, true),
                new SkillRank(Skill.Profession, Ability.Wisdom, true, true),
                new SkillRank(Skill.Ride, Ability.Dexterity),
                new SkillRank(Skill.SenseMotive, Ability.Wisdom),
                new SkillRank(Skill.SleightOfHand, Ability.Dexterity, true),
                new SkillRank(Skill.Spellcraft, Ability.Intelligence, true),
                new SkillRank(Skill.Stealth, Ability.Dexterity),
                new SkillRank(Skill.Survival, Ability.Wisdom),
                new SkillRank(Skill.Swim, Ability.Strength),
                new SkillRank(Skill.UseMagicDevice, Ability.Charisma, true),
            };
        }
        public List<string> languages = new List<string>();
        public string Languages
        {
            get
            {
                var text = string.Empty;
                if (languages != null)
                    foreach (var l in languages)
                    {
                        if (l == null)
                            continue;
                        if (text.Length > 0)
                            text += ", ";
                        text += l;
                    }
                return text;
            }
        }
        #endregion

        #region Feats & Special Abilities
        public List<Feat> feats = new List<Feat>();
        public List<SpecialAbility> specialAbilities = new List<SpecialAbility>();
        #endregion

        #region Equipment
        public List<GearItem> gear = new List<GearItem>();
        public Encumbrance encumbrance = new Encumbrance();
        public Money money = new Money();
        public GearItem GetItemByUID(int uid)
        {
            if (uid == InvalidUID)
                return null;
            foreach (var item in gear)
                if (item.uid == uid)
                    return item;
            foreach (var item in armorClassItems)
                if (item.uid == uid)
                    return item;
            foreach (var item in weaponItems)
                if (item.uid == uid)
                    return item;
            return null;
        }
        public List<GearItem> GetAllGearItems()
        {
            var items = new List<GearItem>();
            items.AddRange(gear);
            items.AddRange(weaponItems);
            items.AddRange(armorClassItems);
            return items;
        }
#if DEBUG_FIX_UID
        public bool FixUIDs<T>(List<T> items) where T: ItemWithDescription
        {
            var hasChanges = false;
            foreach (var item in items)
                if ((item != null) && (item.uid == InvalidUID))
                {
                    item.uid = GetUID();
                    hasChanges = true;
                }
            return hasChanges;
        }
        public void FixUIDs()
        {
            var hasChanges = false;
            hasChanges |= FixUIDs(feats);
            hasChanges |= FixUIDs(specialAbilities);
            hasChanges |= FixUIDs(gear);
            hasChanges |= FixUIDs(armorClassItems);
            hasChanges |= FixUIDs(weaponItems);
            if (hasChanges)
                CharacterSheetStorage.Instance.SaveCharacter(this);
        }
#endif
        #endregion

        #region Spells
        public ChannelEnergy channelEnergy = new ChannelEnergy();
        public const int spellLevelsCount = 10;
        public SpellLevel[] spellLevel = new SpellLevel[spellLevelsCount].Populate();
        #endregion

        #region Notes
        public List<Note> notes = new List<Note>();
        #endregion
    }
}
