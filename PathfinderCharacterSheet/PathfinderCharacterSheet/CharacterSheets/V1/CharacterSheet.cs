//#define SELECT_CURRENT_ATTACK_FOR_COMBAT_MANEUVERS
using System;
using System.Collections.Generic;
using System.Text;

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

        public const int spellLevelsCount = 10;

        public CharacterSheet()
        {

        }

        public void Init()
        {
            InitAbilityScrores();
            InitSkills();
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
        public string alignment = Alignments.Neutral.ToString();
        public Alignments Alignment
        {
            get { return Helpers.GetEnumValue(alignment, Alignments.Neutral); }
            set { alignment = value.ToString(); }
        }
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
        public AbilityScore[] abilityScores = null;
        private void InitAbilityScrores()
        {
            abilityScores = new AbilityScore[(int)Ability.Total]
            {
                new AbilityScore(),
                new AbilityScore(),
                new AbilityScore(),
                new AbilityScore(),
                new AbilityScore(),
                new AbilityScore(),
            };
        }
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
        public List<SkillRank> skills = new List<SkillRank>();
        private void InitSkills()
        {
            skills = new List<SkillRank>()
            {
                new SkillRank(Skills.Acrobatics, Ability.Dexterity),
                new SkillRank(Skills.Appraise, Ability.Intelligence),
                new SkillRank(Skills.Bluff, Ability.Charisma),
                new SkillRank(Skills.Climb, Ability.Strength),
                new SkillRank(Skills.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skills.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skills.Craft, Ability.Intelligence, false, true),
                new SkillRank(Skills.Diplomacy, Ability.Charisma),
                new SkillRank(Skills.DisableDevice, Ability.Dexterity, true),
                new SkillRank(Skills.Disguise, Ability.Charisma),
                new SkillRank(Skills.EscapeArtist, Ability.Dexterity),
                new SkillRank(Skills.Fly, Ability.Dexterity),
                new SkillRank(Skills.HandleAnimal, Ability.Charisma, true),
                new SkillRank(Skills.Heal, Ability.Wisdom),
                new SkillRank(Skills.Intimidate, Ability.Charisma),
                new SkillRank(Skills.KnowledgeOfArcana, Ability.Intelligence),
                new SkillRank(Skills.KnowledgeOfDungeoneering, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfEngineering, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfGeography, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfHistory, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfLocal, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfNature, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfNobility, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfPlanes, Ability.Intelligence, true),
                new SkillRank(Skills.KnowledgeOfReligion, Ability.Intelligence, true),
                new SkillRank(Skills.Linguistics, Ability.Intelligence, true),
                new SkillRank(Skills.Perception, Ability.Wisdom),
                new SkillRank(Skills.Perform, Ability.Charisma, false, true),
                new SkillRank(Skills.Perform, Ability.Charisma, false, true),
                new SkillRank(Skills.Profession, Ability.Wisdom, true, true),
                new SkillRank(Skills.Profession, Ability.Wisdom, true, true),
                new SkillRank(Skills.Ride, Ability.Dexterity),
                new SkillRank(Skills.SenseMotive, Ability.Wisdom),
                new SkillRank(Skills.SleightOfHand, Ability.Dexterity, true),
                new SkillRank(Skills.Spellcraft, Ability.Intelligence, true),
                new SkillRank(Skills.Stealth, Ability.Dexterity),
                new SkillRank(Skills.Survival, Ability.Wisdom),
                new SkillRank(Skills.Swim, Ability.Strength),
                new SkillRank(Skills.UseMagicDevice, Ability.Charisma, true),
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
        public SpellLevel[] spellLevel = new SpellLevel[spellLevelsCount]
        {
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
            new SpellLevel(),
        };
        #endregion

        #region Notes
        public List<Note> notes = new List<Note>();
        #endregion
    }
}
