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
            public ValueWithModifiers<int, IntSum> score = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> tempAdjustment = new ValueWithModifiers<int, IntSum>();
            public int Modifier
            {
                get
                {
                    return CalcModifier(score.Total, 0);
                }
            }
            public int TempModifier
            {
                get
                {
                    return CalcModifier(score.Total, tempAdjustment.Total);
                }
            }
            private static int CalcModifier(int score, int tempAdjustment)
            {
                return (score + tempAdjustment) / 2 - 5;
            }
            public AbilityScore Clone
            {
                get
                {
                    return new AbilityScore()
                    {
                        score = score,
                        tempAdjustment = tempAdjustment.Clone,
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

        public int GetAbilityModifier(Ability ability)
        {
            return GetAbilityModifier(this, ability);
        }

        public class SavingThrow
        {
            public Ability abilityModifierSource = Ability.None;
            public ValueWithModifiers<int, IntSum> baseSave = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> magicModifier = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> miscModifier = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> tempModifier = new ValueWithModifiers<int, IntSum>();

            public SavingThrow()
            {

            }

            public SavingThrow(Ability abilityModifierSource)
            {
                this.abilityModifierSource = abilityModifierSource;
            }

            public int GetAbilityModifier(CharacterSheet sheet)
            {
                return CharacterSheet.GetAbilityModifier(sheet, abilityModifierSource);
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return baseSave.Total + GetAbilityModifier(sheet) + magicModifier.Total + miscModifier.Total + tempModifier.Total;
            }

            public SavingThrow Clone
            {
                get
                {
                    return new SavingThrow()
                    {
                        abilityModifierSource = abilityModifierSource,
                        baseSave = baseSave.Clone,
                        magicModifier = magicModifier.Clone,
                        miscModifier = miscModifier.Clone,
                        tempModifier = tempModifier.Clone,
                    };
                }
            }

            public bool Equals(SavingThrow other)
            {
                if (other == null)
                    return false;
                if (abilityModifierSource != other.abilityModifierSource)
                    return false;
                if (!baseSave.Equals(other.baseSave))
                    return false;
                if (!magicModifier.Equals(other.magicModifier))
                    return false;
                if (!miscModifier.Equals(other.miscModifier))
                    return false;
                if (!tempModifier.Equals(other.tempModifier))
                    return false;
                return true;
            }
        }

        public class Speed
        {
            public ValueWithModifiers<int, IntSum> baseSpeed = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> armorSpeed = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> flySpeed = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> maneuverability = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> swimSpeed = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> climbSpeed = new ValueWithModifiers<int, IntSum>();
            public bool defaultSwimSpeed = true;
            public int SwimSpeed { get { return defaultSwimSpeed ? (baseSpeed.Total / 2) : swimSpeed.Total; } }
            public bool defaultClimbSpeed = true;
            public int ClimbSpeed { get { return defaultClimbSpeed ? (baseSpeed.Total / 4) : climbSpeed.Total; } }
            public ValueWithModifiers<int, IntSum> burrowSpeed = new ValueWithModifiers<int, IntSum>();

            public static int InSquares(int speed)
            {
                return speed / 5;
            }

            public Speed Clone
            {
                get
                {
                    return new Speed()
                    {
                        baseSpeed = baseSpeed.Clone,
                        armorSpeed = armorSpeed.Clone,
                        flySpeed = flySpeed.Clone,
                        maneuverability = maneuverability.Clone,
                        swimSpeed = swimSpeed.Clone,
                        climbSpeed = climbSpeed.Clone,
                        defaultClimbSpeed = defaultClimbSpeed,
                        defaultSwimSpeed = defaultSwimSpeed,
                        burrowSpeed = burrowSpeed.Clone,
                    };
                }
            }

            public bool Equals(Speed other)
            {
                if (other == null)
                    return false;
                if (!baseSpeed.Equals(other.baseSpeed))
                    return false;
                if (!armorSpeed.Equals(other.armorSpeed))
                    return false;
                if (!flySpeed.Equals(other.flySpeed))
                    return false;
                if (!maneuverability.Equals(other.maneuverability))
                    return false;
                if (!swimSpeed.Equals(other.swimSpeed))
                    return false;
                if (!climbSpeed.Equals(other.climbSpeed))
                    return false;
                if (defaultClimbSpeed != other.defaultClimbSpeed)
                    return false;
                if (defaultSwimSpeed != other.defaultSwimSpeed)
                    return false;
                if (!burrowSpeed.Equals(other.burrowSpeed))
                    return false;
                return true;
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

        public class ItemWithDescription
        {
            public string name = null;
            public string description = null;
        }

        public class Feat: ItemWithDescription
        {
        }

        public class SpecialAbility: ItemWithDescription
        {
        }

        public class ArmorClass
        {
            public ValueWithModifiers<int, IntSum> armorBonus = new ValueWithModifiers<int, IntSum>();
            public bool itemsArmorBonus = true;
            public ValueWithModifiers<int, IntSum> shieldBonus = new ValueWithModifiers<int, IntSum>();
            public bool itemsShieldBonus = true;
            public ValueWithModifiers<int, IntSum> sizeModifier = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> naturalArmor = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> deflectionModifier = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();

            public ArmorClass Clone
            {
                get
                {
                    return new ArmorClass()
                    {
                        armorBonus = armorBonus.Clone,
                        itemsArmorBonus = itemsArmorBonus,
                        shieldBonus = shieldBonus.Clone,
                        itemsShieldBonus = itemsShieldBonus,
                        sizeModifier = sizeModifier.Clone,
                        naturalArmor = naturalArmor.Clone,
                        deflectionModifier = deflectionModifier.Clone,
                        miscModifiers = miscModifiers.Clone,
                    };
                }
            }

            public bool Equals(ArmorClass ac)
            {
                if (itemsArmorBonus != ac.itemsArmorBonus)
                    return false;
                if (!itemsArmorBonus && !armorBonus.Equals(ac.armorBonus))
                    return false;
                if (itemsShieldBonus != ac.itemsShieldBonus)
                    return false;
                if (!itemsShieldBonus && !shieldBonus.Equals(ac.shieldBonus))
                    return false;
                if (!sizeModifier.Equals(ac.sizeModifier))
                    return false;
                if (!naturalArmor.Equals(ac.naturalArmor))
                    return false;
                if (!deflectionModifier.Equals(ac.deflectionModifier))
                    return false;
                if (!miscModifiers.Equals(ac.miscModifiers))
                    return false;
                return true;
            }

            private int GetBaseArmorClass
            {
                get
                {
                    return 10 + sizeModifier.Total + deflectionModifier.Total + miscModifiers.Total;
                }
            }

            public int GetArmorBonus(CharacterSheet sheet)
            {
                if (!itemsArmorBonus)
                    return armorBonus.Total;
                var ac = 0;
                foreach (var item in sheet.armorClassItems)
                    if (item.isArmor)
                        ac += item.bonus.Total;
                return ac;
            }

            public int GetShieldBonus(CharacterSheet sheet)
            {
                if (!itemsShieldBonus)
                    return shieldBonus.Total;
                var ac = 0;
                foreach (var item in sheet.armorClassItems)
                    if (item.isShield)
                        ac += item.bonus.Total;
                return ac;
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetAbilityModifier(sheet, Ability.Dexterity) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.Total;
            }

            public int GetTouch(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetAbilityModifier(sheet, Ability.Dexterity);
            }

            public int GetFlatFooted(CharacterSheet sheet)
            {
                return GetBaseArmorClass + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.Total;
            }
        }

        public class ArmorClassItem: Item
        {
            public ValueWithModifiers<int, IntSum> bonus = new ValueWithModifiers<int, IntSum>();
            public bool isShield = false;
            public bool isArmor = false;
            public string type = null;
            public int checkPenalty = 0;
            public int spellFailure = 0;
            public string properties = null;
        }

        public class DiceThrow
        {
            public ValueWithModifiers<int, IntSum> diceCount = new ValueWithModifiers<int, IntSum>();
            public int diceValue = 0;
            public ValueWithModifiers<int, IntSum> miscModifiers = new ValueWithModifiers<int, IntSum>();
        }

        public class WeaponItem: Item
        {
            public ValueWithModifiers<int, IntSum> bonus = new ValueWithModifiers<int, IntSum>();
            public string critical = null;
            public string type = null;
            public ValueWithModifiers<int, IntSum> range = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> ammunition = new ValueWithModifiers<int, IntSum>();
            public DiceThrow damage = new DiceThrow();
        }

        public class Item: ItemWithDescription
        {
            public int amount = 0;
            public int weight = 0;
            public int TotalWeight { get { return amount * weight; } }
        }

        public class Money
        {
            public int cuprumPoints = 0;
            public int silverPoints = 0;
            public int goldenPoints = 0;
            public int platinumPoints = 0;
        }

        public const int spellLevesCount = 10;

        public class SpellLevel
        {
            public ValueWithModifiers<int, IntSum> spellsKnown = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> spellsPerDay = new ValueWithModifiers<int, IntSum>();
            public ValueWithModifiers<int, IntSum> bonusSpells = new ValueWithModifiers<int, IntSum>();
        }

        public class Note: ItemWithDescription
        {
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

        public ValueWithModifiers<int, IntSum> spellResistance = new ValueWithModifiers<int, IntSum>();
        #endregion

        #region Attack
        public Initiative initiative = new Initiative();
        public int CurrentInitiative { get { return initiative.GetInitiative(this); } }

        public ValueWithModifiers<int, IntSum> baseAttackBonus = new ValueWithModifiers<int, IntSum>();

        public ValueWithModifiers<int, IntSum> cmdSizeModifier = new ValueWithModifiers<int, IntSum>();
        public ValueWithModifiers<int, IntSum> cmbSizeModifier = new ValueWithModifiers<int, IntSum>();
        public int GetCMD(ValueWithModifiers<int, IntSum> sizeModifier) { return 10 + baseAttackBonus.Total + GetAbilityModifier(this, Ability.Strength) + GetAbilityModifier(this, Ability.Dexterity) + sizeModifier.Total; }
        public int CMD { get { return GetCMD(cmdSizeModifier); } }
        public int GetCMB(ValueWithModifiers<int, IntSum> sizeModifier) { return baseAttackBonus.Total + GetAbilityModifier(this, Ability.Strength) + sizeModifier.Total; }
        public int CMB { get { return GetCMB(cmbSizeModifier); } }

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
        public Money money = new Money();
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
