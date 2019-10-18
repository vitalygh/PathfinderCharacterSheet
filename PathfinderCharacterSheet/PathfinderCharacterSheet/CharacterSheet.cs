using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public class CharacterSheet
    {
        public class LevelOfClass
        {
            public int level = 0;
            public int Level { get { return level; } }
            public string className = null;
            public string ClassName { get { return className; } }

            public LevelOfClass Clone
            { 
                get
                {
                    return new LevelOfClass()
                    {
                        level = Level,
                        className = ClassName,
                    };
                }
            }

            public static LevelOfClass CreateClone(LevelOfClass loc)
            {
                if (loc == null)
                    return null;
                return loc.Clone;
            }

            public static List<LevelOfClass> CreateClone(List<LevelOfClass> loc)
            {
                if (loc == null)
                    return null;
                var list = new List<LevelOfClass>();
                foreach (var l in loc)
                    if (l != null)
                        list.Add(l.Clone);
                    else
                        list.Add(l);
                return list;
            }

            public static int Total(List<LevelOfClass> levelOfClass)
            {
                var level = 0;
                if (levelOfClass != null)
                    foreach (var loc in levelOfClass)
                    {
                        if (loc == null)
                            continue;
                        level += loc.Level;
                    }
                return level;
            }

            public static string AsString(List<LevelOfClass> levelOfClass)
            {
                var level = string.Empty;
                var totalLevel = 0;
                if (levelOfClass != null)
                    foreach (var loc in levelOfClass)
                    {
                        if (loc == null)
                            continue;
                        if (level.Length > 0)
                            level += ", ";
                        level += loc.ClassName + " (" + loc.Level.ToString() + ")";
                        totalLevel += loc.Level;
                    }
                if (level.Length > 0)
                    level = totalLevel.ToString() + ": " + level;
                return level;
            }

            public bool Equals(LevelOfClass obj)
            {
                var other = obj as LevelOfClass;
                if (other == null)
                    return false;
                if (other.Level != Level)
                    return false;
                if (other.ClassName != ClassName)
                    return false;
                return true;
            }

            public static bool Equal(List<LevelOfClass> a, List<LevelOfClass> b)
            {
                if ((a == null) && (b == null))
                    return true;
                if ((a == null) || (b == null))
                    return false;
                var count = a.Count;
                if (count != b.Count)
                    return false;
                for (var i = 0; i < count; i++)
                {
                    var ai = a[i];
                    var bi = b[i];
                    if ((ai == null) && (bi == null))
                        continue;
                    if ((ai == null) || (bi == null))
                        return false;
                    if (!ai.Equals(bi))
                        return false;
                }
                return true;
            }
        }

        public enum Ability
        {
            None = -1,
            Strength = 0,
            Dexterity,
            Constitution,
            Intelligence,
            Wisdom,
            Charisma,

            Total
        }

        public enum Save
        {
            None = -1,
            Fortitude = 0,
            Reflex,
            Will,

            Total
        }

        public class AbilityScore
        {
            public int score = 0;
            public ValueWithModifiers<int, IntSum> tempAdjustment = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> tempModifier = new ValueWithModifiers<int, IntSum>();
            public int Modifier
            {
                get
                {
                    return CalcModifier(score, tempAdjustment, tempModifier);
                }
            }
            public static int CalcModifier(int score, ValueWithModifiers<int, IntSum> tempAdjustment, ValueWithModifiers<int, IntSum> tempModifier)
            {
                return (score + tempAdjustment.Total) / 2 - 5 + tempModifier.Total;
            }
            public AbilityScore Clone
            {
                get
                {
                    return new AbilityScore()
                    {
                        score = score,
                        tempAdjustment = tempAdjustment.Clone,
                        tempModifier = tempModifier.Clone,
                    };
                }
            }
            public bool Equals(AbilityScore obj)
            {
                var other = obj as AbilityScore;
                if (other == null)
                    return false;
                if (other.score != score)
                    return false;
                if (!other.tempAdjustment.Equals(tempAdjustment))
                    return false;
                if (!other.tempModifier.Equals(tempModifier))
                    return false;
                return true;
            }
        }

        public class Modifier<T>
        {
            public bool active = true;
            public bool IsActive { get { return active; } }
            public string name = null;
            public string Name { get { return name; } }
            public T value = default(T);
            public T Value { get { return value; } }

            public static Modifier<T> CreateClone(Modifier<T> mod)
            {
                if (mod == null)
                    return null;
                return new Modifier<T>()
                {
                    active = mod.IsActive,
                    name = mod.Name,
                    value = mod.Value,
                };
            }

            public Modifier<T> Clone
            {
                get
                {
                    return new Modifier<T>()
                    {
                        active = active,
                        name = name,
                        value = value,
                    };
                }
            }

            public static List<Modifier<T>> CreateClone(List<Modifier<T>> mods)
            {
                if (mods == null)
                    return null;
                var list = new List<Modifier<T>>();
                foreach (var m in mods)
                    if (m != null)
                        list.Add(m.Clone);
                    else
                        list.Add(m);
                return list;
            }

            public Modifier<T> Fill(Modifier<T> source)
            {
                if (source == null)
                    return this;
                active = source.IsActive;
                name = source.Name;
                value = source.Value;
                return this;
            }

            public bool Equals(Modifier<T> obj)
            {
                var other = obj as Modifier<T>;
                if (other == null)
                    return false;
                if (other.IsActive != IsActive)
                    return false;
                if (other.Name != Name)
                    return false;
                if (!other.Value.Equals(Value))
                    return false;
                return true;
            }

            public static bool IsEqual(List<Modifier<T>> a, List<Modifier<T>> b)
            {
                if ((a == null) && (b == null))
                    return true;
                if ((a == null) || (b == null))
                    return false;
                var count = a.Count;
                if (count != b.Count)
                    return false;
                for (var i = 0; i < count; i++)
                {
                    var ai = a[i];
                    var bi = b[i];
                    if ((ai == null) && (bi == null))
                        continue;
                    if ((ai == null) || (bi == null))
                        return false;
                    if (!ai.Equals(bi))
                        return false;
                }
                return true;
            }

            public static T Sum<S>(List<Modifier<T>> modifiers) where S : ISummable<T>, new()
            {
                return Sum<T, S>(modifiers);
            }
        }

        public class ModifiersList<T, S> : List<Modifier<T>> where S : ISummable<T>, new()
        {
            public T Sum { get { return Sum<T, S>(this); } }
            public static ModifiersList<T, S> CreateClone(ModifiersList<T, S> ml)
            {
                if (ml == null)
                    return null;
                return ml.Clone;
            }
            public virtual ModifiersList<T, S> Clone { get { return new ModifiersList<T, S>().Fill(this); } }
            public ModifiersList<T, S> Fill(ModifiersList<T, S> source)
            {
                Clear();
                foreach (var m in source)
                    Add(m.Clone);
                return this;
            }
        }

        public class ValueWithModifiers<T, S> where S : ISummable<T>, new()
        {
            public T baseValue = default(T);
            public ModifiersList<T, S> modifiers = new ModifiersList<T, S>();
            public T Total { get { return Sum<T, S>(baseValue, modifiers.Sum); } }

            public static ValueWithModifiers<T, S> CreateClone(ValueWithModifiers<T, S> vwm)
            {
                if (vwm == null)
                    return null;
                return vwm.Clone;
            }

            public ValueWithModifiers<T, S> Clone
            {   get
                {
                    return new ValueWithModifiers<T, S>()
                    {
                        baseValue = baseValue,
                        modifiers = modifiers.Clone,
                    };
                }
            }

            public ValueWithModifiers<T, S> Fill(ValueWithModifiers<T, S> source)
            {
                if (source == null)
                    return this;
                baseValue = source.baseValue;
                modifiers = source.modifiers.Clone;
                return this;
            }

            public bool Equals(ValueWithModifiers<T, S> obj)
            {
                var other = obj as ValueWithModifiers<T, S>;
                if (other == null)
                    return false;
                if (!other.baseValue.Equals(baseValue))
                    return false;
                if (!Modifier<T>.IsEqual(other.modifiers, modifiers))
                    return false;
                return true;
            }
        }

        public interface ISummable<T>
        {
            T Add(T a, T b);
        }

        public struct IntSum : ISummable<int>
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        public static T Sum<T, S>(T a, T b) where S: ISummable<T>, new()
        {
            return new S().Add(a, b);
        }

        public static T Sum<T, S>(List<Modifier<T>> modifiers, bool activeOnly = true) where S: ISummable<T>, new()
        {
            T value = default(T);
            if (modifiers != null)
                foreach (var m in modifiers)
                    if (m.IsActive || !activeOnly)
                        value = Sum<T, S>(value, m.Value);
            return value;
        }

        public static int GetAbilityModifier(CharacterSheet sheet, Ability ability)
        {
            int ab = (int)ability;
            if ((ab >= 0) && (ab < sheet.abilityScores.Length))
                return sheet.abilityScores[ab].Modifier;
            return 0;
        }

        public int CurrentAbilityModifier(Ability ability)
        {
            return GetAbilityModifier(this, ability);
        }

        public class SavingThrow
        {
            public Ability abilityModifierSource = Ability.None;
            public int baseSave = 0;
            public ValueWithModifiers<int, IntSum> magicModifiers = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> tempModifiers = new ValueWithModifiers<int, IntSum>();

            public SavingThrow()
            {

            }

            public SavingThrow(Ability abilityModifierSource)
            {
                this.abilityModifierSource = abilityModifierSource;
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return baseSave + GetAbilityModifier(sheet, abilityModifierSource) + magicModifiers.Total + miscModifiers.Total + tempModifiers.Total;
            }
        }

        public class Speed
        {
            public int baseSpeed = 0;
            public ValueWithModifiers<int, IntSum> tempModifiers = new ValueWithModifiers<int, IntSum>();
            public int CurrentBaseSpeed
            {
                get
                {
                    return baseSpeed + tempModifiers.Total;
                }
            }
            public int armorSpeed = 0;
            public int CurrentArmorSpeed
            {
                get
                {
                    return armorSpeed + tempModifiers.Total;
                }
            }
            public int flySpeed = 0;
            public int SwimSpeed { get { return CurrentBaseSpeed / 2; } }
            public int ClimbSpeed { get { return CurrentBaseSpeed / 4; } }
            public int burrowSpeed = 0;

            public static int InSquares(int speed)
            {
                return speed / 5;
            }
        }

        public class HP
        {
            public ValueWithModifiers<int, IntSum> maxHP = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> hp = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> damageResist = new ValueWithModifiers<int, IntSum>();
        }

        public class Initiative
        {
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
            public int GetInitiative(CharacterSheet sheet)
            {
                return GetAbilityModifier(sheet, Ability.Dexterity) + miscModifiers.Total;
            }
        }

        public enum Alignment
        {
            LawfulGood,
            NeutralGood,
            ChaoticGood,
            LawfulNeutral,
            Neutral,
            ChaoticNeutral,
            LawulEvil,
            NeutralEvil,
            ChaoticEvil,
        }

        public enum Skill
        {
            None = -1,
            Acrobatics,
            Appraise,
            Bluff,
            Climb,
            Craft,
            Diplomacy,
            DisableDevice,
            Disguise,
            EscapeArtist,
            Fly,
            HandleAnimal,
            Heal,
            Intimidate,
            KnowledgeOfArcana,
            KnowledgeOfDungeoneering,
            KnowledgeOfEngineering,
            KnowledgeOfGeography,
            KnowledgeOfHistory,
            KnowledgeOfLocal,
            KnowledgeOfNature,
            KnowledgeOfNobility,
            KnowledgeOfPlanes,
            KnowledgeOfReligion,
            Linguistics,
            Perception,
            Perform,
            Profession,
            Ride,
            SenseMotive,
            SleightOfHand,
            Spellcraft,
            Stealth,
            Survival,
            Swim,
            UseMagicDevice,
        }

        public class SkillRank
        {
            public Skill skill = Skill.None;
            public bool hasName = false;
            public string name = null;
            public bool classSkill = false;
            public Ability abilityModifierSource = Ability.None;
            public int rank = 0;
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
            public bool armorPenalty = false;
            public bool trainedOnly = false;

            public SkillRank()
            {
            }

            public SkillRank(Skill skill, Ability abilityModifierSource, bool trainedOnly = false, bool hasName = false)
            {
                this.skill = skill;
                this.abilityModifierSource = abilityModifierSource;
                this.trainedOnly = trainedOnly;
                this.hasName = hasName;
            }
        }

        public class Feat
        {
            public string name = null;
            public string description = null;
        }

        public class SpecialAbility
        {
            public string name = null;
            public string description = null;
        }

        public class ArmorClass
        {
            public int armorBonus = 0;
            public bool autoArmorBonus = true;
            public int shieldBonus = 0;
            public bool autoShieldBonus = true;
            public int sizeModifier = 0;
            public int naturalArmor = 0;
            public int deflectionModifier = 0;
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
            private int GetBaseArmorClass
            {
                get
                {
                    return 10 + sizeModifier + deflectionModifier + miscModifiers.Total;
                }
            }

            public int GetArmorBonus(CharacterSheet sheet)
            {
                if (!autoArmorBonus)
                    return armorBonus;
                var ac = 0;
                foreach (var item in sheet.armorClassItems)
                    if (item.isArmor)
                        ac += item.bonus;
                return ac;
            }

            public int GetShieldBonus(CharacterSheet sheet)
            {
                if (!autoShieldBonus)
                    return shieldBonus;
                var ac = 0;
                foreach (var item in sheet.armorClassItems)
                    if (item.isShield)
                        ac += item.bonus;
                return ac;
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetAbilityModifier(sheet, Ability.Dexterity) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor;
            }

            public int GetTouch(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetAbilityModifier(sheet, Ability.Dexterity);
            }

            public int GetFlatFooted(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor;
            }
        }

        public class ArmorClassItem: Item
        {
            public int bonus = 0;
            public bool isShield = false;
            public bool isArmor = false;
            public string type = null;
            public int checkPenalty = 0;
            public int spellFailure = 0;
            public string properties = null;
        }

        public class DiceThrow
        {
            public int diceCount = 0;
            public int diceValue = 0;
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
        }

        public class WeaponItem: Item
        {
            public int bonus = 0;
            public string critical = null;
            public string type = null;
            public int range = 0;
            public int ammunition = 0;
            public DiceThrow damage = new DiceThrow();
        }

        public class Item
        {
            public string name = null;
            public string description = null;
            public int amount = 0;
            public int weight = 0;
            public int TotalWeight { get { return amount * weight; } }
        }

        public const int spellLevesCount = 10;

        public class SpellLevel
        {
            public int spellsKnown = 0;
            public int spellsPerDay = 0;
            public int bonusSpells = 0;
        }

        public class Note
        {
            public string title = null;
            public string description = null;
        }

        #region Character background
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
        public Alignment alignment = Alignment.Neutral;
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
        public List<LevelOfClass> levelOfClass = new List<LevelOfClass>();
        public int TotalLevel { get { return LevelOfClass.Total(levelOfClass); } }
        public string LevelAsString { get { return LevelOfClass.AsString(levelOfClass); } }
        public int experience = 0;
        public int nextLevelExperience = 0;
        #endregion

        #region Ability Score
        public AbilityScore[] abilityScores = new AbilityScore[(int)Ability.Total]
        {
            new AbilityScore(),
            new AbilityScore(),
            new AbilityScore(),
            new AbilityScore(),
            new AbilityScore(),
            new AbilityScore(),
        };
        #endregion

        #region Hit Points
        public HP hp = new HP();
        #endregion

        #region Speed
        Speed speed = new Speed();
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

        public int spellResistance = 0;
        #endregion

        #region Attack
        public Initiative initiative = new Initiative();
        public int CurrentInitiative { get { return initiative.GetInitiative(this); } }

        public ValueWithModifiers<int, IntSum> baseAttackBonus = new ValueWithModifiers<int, IntSum>();

        public int combatManeuverSpecialSizeModifier = 0;
        public int CurrentCMD { get { return 10 + baseAttackBonus.Total + GetAbilityModifier(this, Ability.Strength) + GetAbilityModifier(this, Ability.Dexterity) + combatManeuverSpecialSizeModifier; } }
        public int CurrentCMB { get { return baseAttackBonus.Total + GetAbilityModifier(this, Ability.Strength) + combatManeuverSpecialSizeModifier; } }

        public int sizeModifier = 0;
        public ValueWithModifiers<int, IntSum> meleeAttackBonusModifiers = new ValueWithModifiers<int, IntSum>();
        public int MeleeAttackBonus { get { return baseAttackBonus.Total + meleeAttackBonusModifiers.Total + GetAbilityModifier(this, Ability.Strength) + sizeModifier; } }
        public ValueWithModifiers<int, IntSum> meleeDamageBonusModifiers = new ValueWithModifiers<int, IntSum>();
        public int MeleeDamageBonus { get { return meleeDamageBonusModifiers.Total + GetAbilityModifier(this, Ability.Strength); } }
        public ValueWithModifiers<int, IntSum> rangeAttackBonusModifiers = new ValueWithModifiers<int, IntSum>();
        public int RangeAttackBonus { get { return baseAttackBonus.Total + rangeAttackBonusModifiers.Total + GetAbilityModifier(this, Ability.Dexterity) + sizeModifier; } }
        #endregion

        #region Weapons
        public List<WeaponItem> weaponItems = new List<WeaponItem>();
        #endregion

        #region AC Items
        public List<ArmorClassItem> armorClassItems = new List<ArmorClassItem>();
        #endregion

        #region Skills
        public int skillsPerLevel = 0;
        public int currentSkillsPerLevel { get { return GetAbilityModifier(this, Ability.Intelligence) + skillsPerLevel; } }
        public List<SkillRank>skills = new List<SkillRank>()
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
        public string skillsConditionalModifiers = null;
        public List<string> languages = new List<string>();
        #endregion

        #region Feats & Special Abilities
        public List<Feat> feats = new List<Feat>();
        public List<SpecialAbility> specialAbilities = new List<SpecialAbility>();
        #endregion

        #region Equipment
        public List<Item> gear = new List<Item>();
        public int lightLoad = 0;
        public int mediumItem = 0;
        public int heavyItem = 0;
        public int liftOverHead = 0;
        public int LiftOffGround { get { return 2 * liftOverHead; } }
        public int DragOrPush { get { return 5 * liftOverHead; } }
        #endregion

        #region Spells
        public ValueWithModifiers<int, IntSum> channelsMaxCount = new ValueWithModifiers<int, IntSum>();
        public int channelsCurrentCount = 0;
        public Ability spellDCAbilityModifierSource = Ability.None;
        public int GetSpellSaveDC(int level) { return 10 + GetAbilityModifier(this, spellDCAbilityModifierSource) + level; }
        public SpellLevel[] spellLevel = new SpellLevel[spellLevesCount];
        public string spellsConditionalModifiers = null;
        #endregion

        #region Notes
        public List<Note> notes = new List<Note>();
        #endregion
    }
}
