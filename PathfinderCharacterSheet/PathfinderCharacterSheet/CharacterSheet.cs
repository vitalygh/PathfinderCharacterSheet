using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet
{
    public class CharacterSheet
    {
        public class LevelOfClass
        {
            public ValueWithIntModifiers level = new ValueWithIntModifiers();
            public int GetLevel(CharacterSheet sheet) { return level.GetTotal(sheet); }
            public string className = null;
            public string ClassName { get { return className; } }

            public virtual object Clone
            {
                get
                {
                    var loc = new LevelOfClass();
                    loc.Fill(this);
                    return loc;
                }
            }

            public virtual LevelOfClass Fill(LevelOfClass source)
            {
                if (source == null)
                    return this;
                level = source.level.Clone as ValueWithIntModifiers;
                className = source.className;
                return this;
            }

            public static List<LevelOfClass> CreateClone(List<LevelOfClass> loc)
            {
                if (loc == null)
                    return null;
                var list = new List<LevelOfClass>();
                foreach (var l in loc)
                    if (l != null)
                        list.Add(l.Clone as LevelOfClass);
                    else
                        list.Add(l);
                return list;
            }

            public static int Total(CharacterSheet sheet, List<LevelOfClass> levelOfClass)
            {
                var level = 0;
                if (levelOfClass != null)
                    foreach (var loc in levelOfClass)
                    {
                        if (loc == null)
                            continue;
                        level += loc.GetLevel(sheet);
                    }
                return level;
            }

            public static string AsString(CharacterSheet sheet, List<LevelOfClass> levelOfClass)
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
                        var lvl = loc.GetLevel(sheet); ;
                        level += loc.ClassName + " (" + lvl.ToString() + ")";
                        totalLevel += lvl;
                    }
                if (level.Length > 0)
                    level = totalLevel.ToString() + ": " + level;
                return level;
            }

            public bool Equals(LevelOfClass other)
            {
                if (other == null)
                    return false;
                if (!other.level.Equals(level))
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

        public class AbilityPickerItem
        {
            public string Name { set; get; }
            public Ability Value { set; get; }
        }

        public class IntPickerItem
        {
            public string Name { set; get; }
            public int Value { set; get; }
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
            public ValueWithIntModifiers score = new ValueWithIntModifiers();
            public ValueWithIntModifiers tempAdjustment = new ValueWithIntModifiers();
            public int GetModifier(CharacterSheet sheet)
            {
                return CalcModifier(score.GetTotal(sheet), 0);
            }
            public int GetTempModifier(CharacterSheet sheet)
            {
                return CalcModifier(score.GetTotal(sheet), tempAdjustment.GetTotal(sheet));
            }
            private static int CalcModifier(int score, int tempAdjustment)
            {
                return (score + tempAdjustment) / 2 - 5;
            }
            public virtual object Clone
            {
                get
                {
                    return new AbilityScore()
                    {
                        score = score,
                        tempAdjustment = tempAdjustment.Clone as ValueWithIntModifiers,
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

        public static bool IsEqual<T>(List<T> a, List<T> b)
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

        public class IntModifier: Modifier<int>
        {
            public string sourceAbility = Ability.None.ToString();
            public Ability SourceAbility
            {
                get { return GetEnumValue(sourceAbility, Ability.None); }
                set { sourceAbility = value.ToString(); }
            }
            public int multiplier = 1;
            public int divider = 1;

            public int sourceItemUID = InvalidUID;
            public bool mustBeActive = true;

            public override int GetValue(CharacterSheet sheet)
            {
                if ((sourceItemUID != InvalidUID) && (sheet != null))
                {
                    var item = sheet.GetItemByUID(sourceItemUID);
                    if (item == null)
                        return 0;
                    if (!item.active && mustBeActive)
                        return 0;
                }
                if ((sheet == null) || (SourceAbility == Ability.None))
                    return value;
                var ability = multiplier * sheet.GetAbilityModifier(SourceAbility);
                if (divider != 0)
                    ability /= divider;
                return ability;
            }

            public string AsString(CharacterSheet sheet)
            {
                var text = Name;
                if (SourceAbility != Ability.None)
                    text = "(= " + sourceAbility + ") " + text;
                if (sourceItemUID != InvalidUID)
                {
                    var item = sheet.GetItemByUID(sourceItemUID);
                    if (item == null)
                        text = "[item missing] " + text;
                    else if (!mustBeActive)
                        text = "[with " + item.name + "] " + text;
                    else if (item.active)
                        text = "[with active " + item.name + "] " + text;
                    else
                        text = "[need active " + item.name + "] " + text;
                }
                return text;
            }

            public override object Clone
            {
                get
                {
                    var clone = new IntModifier();
                    clone.Fill(this);
                    return clone;
                }
            }

            public static List<IntModifier> CreateClone(List<IntModifier> mods)
            {
                if (mods == null)
                    return null;
                var list = new List<IntModifier>();
                foreach (var m in mods)
                    if (m != null)
                        list.Add(m.Clone as IntModifier);
                    else
                        list.Add(m);
                return list;
            }

            public virtual object Fill(IntModifier source)
            {
                if (source == null)
                    return this;
                base.Fill(source);

                sourceAbility = source.sourceAbility;
                multiplier = source.multiplier;
                divider = source.divider;

                sourceItemUID = source.sourceItemUID;
                mustBeActive = source.mustBeActive;

                return this;
            }

            public bool Equals(IntModifier other)
            {
                if (other == null)
                    return false;
                if (!base.Equals(other))
                    return false;
                if (other.sourceAbility != sourceAbility)
                    return false;
                if (other.multiplier != multiplier)
                    return false;
                if (other.divider != divider)
                    return false;
                if (other.sourceItemUID != sourceItemUID)
                    return false;
                if (other.mustBeActive != mustBeActive)
                    return false;
                return true;
            }
        }

        public class Modifier<T>
        {
            public bool active = true;
            public virtual bool IsActive { get { return active; } }
            public string name = null;
            public virtual string Name { get { return name; } }
            public  T value = default(T);
            public virtual T GetValue(CharacterSheet sheet) { return value; }

            public virtual object Clone
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
                        list.Add(m.Clone as Modifier<T>);
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
                value = source.value;
                return this;
            }

            public bool Equals(Modifier<T> other)
            {
                if (other == null)
                    return false;
                if (other.IsActive != IsActive)
                    return false;
                if (other.Name != Name)
                    return false;
                if (!other.value.Equals(value))
                    return false;
                return true;
            }

            public static T Sum<S>(CharacterSheet sheet, List<Modifier<T>> modifiers) where S : ISummable<T>, new()
            {
                return Sum<Modifier <T>, T, S>(sheet, modifiers);
            }
        }

        public class ModifiersList<M, T, S> : List<M> where S : ISummable<T>, new() where M: Modifier<T>
        {
            public T GetTotal(CharacterSheet sheet) { return Sum<M, T, S>(sheet, this); }
            public virtual object Clone { get { return new ModifiersList<M, T, S>().Fill(this); } }
            public ModifiersList<M, T, S> Fill(ModifiersList<M, T, S> source)
            {
                Clear();
                foreach (var m in source)
                    Add(m.Clone as M);
                return this;
            }
        }

        public class ValueWithIntModifiers: ValueWithModifiers<IntModifier, int, IntSum>
        {
            public override object Clone
            {
                get
                {
                    var clone = new ValueWithIntModifiers();
                    clone.Fill(this);
                    return clone;
                }
            }

            public ValueWithIntModifiers Fill(ValueWithIntModifiers source)
            {
                if (source == null)
                    return this;
                base.Fill(source);
                return this;
            }

            public bool Equals(ValueWithIntModifiers other)
            {
                if (other == null)
                    return false;
                if (!other.baseValue.Equals(baseValue))
                    return false;
                if (!IsEqual(other.modifiers, modifiers))
                    return false;
                return true;
            }
        }

        public class ValueWithModifiers<M, T, S> where S : ISummable<T>, new() where M: Modifier<T>
        {
            public T baseValue = default(T);
            public ModifiersList<M, T, S> modifiers = new ModifiersList<M, T, S>();

            public virtual T GetTotal(CharacterSheet sheet) { return Sum<T, S>(baseValue, modifiers.GetTotal(sheet)); }

            public virtual object Clone
            {
                get
                {
                    var clone = new ValueWithModifiers<M, T, S>();
                    clone.Fill(this);
                    return clone;
                }
            }

            public ValueWithModifiers<M, T, S> Fill(ValueWithModifiers<M, T, S> source)
            {
                if (source == null)
                    return this;
                baseValue = source.baseValue;
                modifiers = source.modifiers.Clone as ModifiersList<M, T, S>;
                return this;
            }

            public bool Equals(ValueWithModifiers<M, T, S> other)
            {
                if (other == null)
                    return false;
                if (!other.baseValue.Equals(baseValue))
                    return false;
                if (!IsEqual(other.modifiers, modifiers))
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

        public static T Sum<M, T, S>(CharacterSheet sheet, List<M> modifiers, bool activeOnly = true) where S: ISummable<T>, new() where M: Modifier<T>
        {
            T value = default(T);
            if (modifiers != null)
                foreach (var m in modifiers)
                    if (m.IsActive || !activeOnly)
                        value = Sum<T, S>(value, m.GetValue(sheet));
            return value;
        }

        public static T GetEnumValue<T>(string text, T defaultValue) where T: struct
        {
            T result = defaultValue;
            if (Enum.TryParse<T>(text, out result))
                return result;
            return defaultValue;
        }

        public static int GetAbilityModifier(CharacterSheet sheet, Ability ability)
        {
            int ab = (int)ability;
            if ((ab >= 0) && (ab < sheet.abilityScores.Length))
                return sheet.abilityScores[ab].GetTempModifier(sheet);
            return 0;
        }

        public int GetAbilityModifier(Ability ability)
        {
            return GetAbilityModifier(this, ability);
        }

        public class SavingThrow
        {
            public string abilityModifierSource = Ability.None.ToString();
            public Ability AbilityModifierSource
            {
                get { return GetEnumValue(abilityModifierSource, Ability.None); }
                set { abilityModifierSource = value.ToString(); }
            }
            public ValueWithIntModifiers baseSave = new ValueWithIntModifiers();
            public ValueWithIntModifiers magicModifier = new ValueWithIntModifiers();
            public ValueWithIntModifiers miscModifier = new ValueWithIntModifiers();
            public ValueWithIntModifiers tempModifier = new ValueWithIntModifiers();

            public SavingThrow()
            {

            }

            public SavingThrow(Ability abilityModifierSource)
            {
                this.abilityModifierSource = abilityModifierSource.ToString();
            }

            public int GetAbilityModifier(CharacterSheet sheet)
            {
                return CharacterSheet.GetAbilityModifier(sheet, AbilityModifierSource);
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return baseSave.GetTotal(sheet) + GetAbilityModifier(sheet) + magicModifier.GetTotal(sheet) + miscModifier.GetTotal(sheet) + tempModifier.GetTotal(sheet);
            }

            public virtual object Clone
            {
                get
                {
                    return new SavingThrow()
                    {
                        abilityModifierSource = abilityModifierSource,
                        baseSave = baseSave.Clone as ValueWithIntModifiers,
                        magicModifier = magicModifier.Clone as ValueWithIntModifiers,
                        miscModifier = miscModifier.Clone as ValueWithIntModifiers,
                        tempModifier = tempModifier.Clone as ValueWithIntModifiers,
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
            public ValueWithIntModifiers baseSpeed = new ValueWithIntModifiers();
            public ValueWithIntModifiers armorSpeed = new ValueWithIntModifiers();
            public ValueWithIntModifiers flySpeed = new ValueWithIntModifiers();
            public ValueWithIntModifiers maneuverability = new ValueWithIntModifiers();
            public ValueWithIntModifiers swimSpeed = new ValueWithIntModifiers();
            public ValueWithIntModifiers climbSpeed = new ValueWithIntModifiers();
            public bool defaultSwimSpeed = true;
            public int GetSwimSpeed(CharacterSheet sheet) { return defaultSwimSpeed ? (baseSpeed.GetTotal(sheet) / 2) : swimSpeed.GetTotal(sheet); }
            public bool defaultClimbSpeed = true;
            public int GetClimbSpeed(CharacterSheet sheet) { return defaultClimbSpeed ? (baseSpeed.GetTotal(sheet) / 4) : climbSpeed.GetTotal(sheet); }
            public ValueWithIntModifiers burrowSpeed = new ValueWithIntModifiers();

            public static int InSquares(int speed)
            {
                return speed / 5;
            }

            public virtual object Clone
            {
                get
                {
                    return new Speed()
                    {
                        baseSpeed = baseSpeed.Clone as ValueWithIntModifiers,
                        armorSpeed = armorSpeed.Clone as ValueWithIntModifiers,
                        flySpeed = flySpeed.Clone as ValueWithIntModifiers,
                        maneuverability = maneuverability.Clone as ValueWithIntModifiers,
                        swimSpeed = swimSpeed.Clone as ValueWithIntModifiers,
                        climbSpeed = climbSpeed.Clone as ValueWithIntModifiers,
                        defaultClimbSpeed = defaultClimbSpeed,
                        defaultSwimSpeed = defaultSwimSpeed,
                        burrowSpeed = burrowSpeed.Clone as ValueWithIntModifiers,
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
            public ValueWithIntModifiers maxHP = new ValueWithIntModifiers();
            public ValueWithIntModifiers hp = new ValueWithIntModifiers();
            public ValueWithIntModifiers damageResist = new ValueWithIntModifiers();
        }

        public class Initiative
        {
            public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
            public int GetInitiative(CharacterSheet sheet)
            {
                return GetAbilityModifier(sheet, Ability.Dexterity) + miscModifiers.GetTotal(sheet);
            }
        }

        public enum Alignments
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

        public enum Skills
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
            public string name = Skills.None.ToString();
            public bool hasSubject = false;
            public string subject = null;
            public bool classSkill = false;
            public string abilityModifierSource = Ability.None.ToString();
            public Ability AbilityModifierSource
            {
                get { return GetEnumValue(abilityModifierSource, Ability.None); }
                set { abilityModifierSource = value.ToString(); }
            }
            public ValueWithIntModifiers rank = new ValueWithIntModifiers();
            public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();
            public bool armorPenalty = false;
            public bool trainedOnly = false;
            public bool custom = false;

            public SkillRank()
            {
            }

            public SkillRank(Skills name, Ability abilityModifierSource, bool trainedOnly = false, bool hasSubject = false, bool custom = false):
                this(name.ToString(), abilityModifierSource, trainedOnly, hasSubject, custom)
            {
            }

            public SkillRank(string name, Ability abilityModifierSource, bool trainedOnly = false, bool hasSubject = false, bool custom = false)
            {
                this.name = name;
                AbilityModifierSource = abilityModifierSource;
                switch (abilityModifierSource)
                {
                    case Ability.Strength:
                    case Ability.Dexterity:
                        armorPenalty = true;
                        break;
                    default:
                        armorPenalty = false;
                        break;
                }
                this.trainedOnly = trainedOnly;
                this.hasSubject = hasSubject;
                this.custom = custom;
            }

            public string Name
            {
                get
                {
                    var text = string.Empty;
                    text += name;
                    if (trainedOnly)
                        text += "*";
                    if (hasSubject && !string.IsNullOrWhiteSpace(subject))
                        text += " (" + subject + ")";
                    return text;
                }
            }

            public virtual object Clone
            {
                get
                {
                    var clone = new SkillRank();
                    clone.Fill(this);
                    return clone;
                }
            }

            public SkillRank Fill(SkillRank source)
            {
                if (source == null)
                    return this;
                name = source.name;
                hasSubject = source.hasSubject;
                subject = source.subject;
                classSkill = source.classSkill;
                abilityModifierSource = source.abilityModifierSource;
                rank = source.rank.Clone as ValueWithIntModifiers;
                miscModifiers = source.miscModifiers.Clone as ValueWithIntModifiers;
                armorPenalty = source.armorPenalty;
                trainedOnly = source.trainedOnly;
                custom = source.custom;
                return this;
            }

            public bool Equals(SkillRank other)
            {
                if (other == null)
                    return false;
                if (name != other.name)
                    return false;
                if (hasSubject != other.hasSubject)
                    return false;
                if (subject != other.subject)
                    return false;
                if (classSkill != other.classSkill)
                    return false;
                if (abilityModifierSource != other.abilityModifierSource)
                    return false;
                if (!rank.Equals(other.rank))
                    return false;
                if (!miscModifiers.Equals(other.miscModifiers))
                    return false;
                if (armorPenalty != other.armorPenalty)
                    return false;
                if (trainedOnly != other.trainedOnly)
                    return false;
                if (custom != other.custom)
                    return false;
                return true;
            }

            public int GetAbilityModifier(CharacterSheet sheet)
            {
                return CharacterSheet.GetAbilityModifier(sheet, AbilityModifierSource);
            }

            public int GetTotal(CharacterSheet sheet)
            {
                var total = rank.GetTotal(sheet);
                if (classSkill && (total > 0))
                    total += 3;
                if (armorPenalty)
                    total += sheet.CheckPenalty();
                total += GetAbilityModifier(sheet);
                total += miscModifiers.GetTotal(sheet);
                return total;
            }
        }

        public class ItemWithDescription
        {
            public int uid = -1;
            public bool selected = false;
            public string name = null;
            public string description = null;

            public ItemWithDescription()
            {
                uid = CharacterSheetStorage.GetUID();
            }

            public virtual object Clone
            {
                get
                {
                    var clone = new ItemWithDescription();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(ItemWithDescription other)
            {
                if (other == null)
                    return false;
                if (uid != other.uid)
                    return false;
                if (selected != other.selected)
                    return false;
                if (name != other.name)
                    return false;
                if (description != other.description)
                    return false;
                return true;
            }

            public ItemWithDescription Fill(ItemWithDescription source)
            {
                if (source == null)
                    return this;
                uid = source.uid;
                selected = source.selected;
                name = source.name;
                description = source.description;
                return this;
            }

            public virtual string AsString(CharacterSheet sheet)
            {
                return name;
            }
        }

        public class Feat: ItemWithDescription
        {
            public override object Clone
            {
                get
                {
                    var clone = new Feat();
                    clone.Fill(this);
                    return clone;
                }
            }
        }

        public class SpecialAbility: ItemWithDescription
        {
            public ValueWithIntModifiers left = new ValueWithIntModifiers();
            public ValueWithIntModifiers total = new ValueWithIntModifiers();

            public override object Clone
            {
                get
                {
                    var clone = new SpecialAbility();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(SpecialAbility other)
            {
                if (other == null)
                    return false;
                if (!base.Equals(other))
                    return false;
                if (!left.Equals(other.left))
                    return false;
                if (!total.Equals(other.total))
                    return false;
                return true;
            }

            public SpecialAbility Fill(SpecialAbility source)
            {
                if (source == null)
                    return this;
                base.Fill(source);
                left = source.left.Clone as ValueWithIntModifiers;
                total = source.total.Clone as ValueWithIntModifiers;
                return this;
            }

            public override string AsString(CharacterSheet sheet)
            {
                var text = name;
                var t = total.GetTotal(sheet);
                if (t > 0)
                {
                    var l = left.GetTotal(sheet);
                    text += " (" + l + " / " + t + ")";
                }
                return text;
            }
        }

        public class Note: ItemWithDescription
        {
            public override object Clone
            {
                get
                {
                    var clone = new Note();
                    clone.Fill(this);
                    return clone;
                }
            }
        }

        public class GearItem: ItemWithDescription
        {
            public bool active = false;
            public ValueWithIntModifiers amount = new ValueWithIntModifiers() { baseValue = 1, };
            public ValueWithIntModifiers weight = new ValueWithIntModifiers();
            public int TotalWeight(CharacterSheet sheet) { return amount.GetTotal(sheet) * weight.GetTotal(sheet); }
            public override string AsString(CharacterSheet sheet)
            {
                var text = string.Empty;
                var count = amount.GetTotal(sheet);
                if (count > 1)
                    text += "(" + count + ") ";
                return text + name;
            }

            public override object Clone
            {
                get
                {
                    var clone = new GearItem();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(GearItem other)
            {
                if (other == null)
                    return false;
                if (!base.Equals(other))
                    return false;
                if (active != other.active)
                    return false;
                if (!amount.Equals(other.amount))
                    return false;
                if (!weight.Equals(other.weight))
                    return false;
                return true;
            }

            public GearItem Fill(GearItem source)
            {
                if (source == null)
                    return this;
                base.Fill(source);
                active = source.active;
                amount = source.amount.Clone as ValueWithIntModifiers;
                weight = source.weight.Clone as ValueWithIntModifiers;
                return this;
            }
        }

        public class WeaponItem: GearItem
        {
            public ValueWithIntModifiers attackBonus = new ValueWithIntModifiers();
            public string AttackBonus(CharacterSheet sheet)
            {
                if (sheet == null)
                    return null;
                var wab = attackBonus.GetTotal(sheet);
                var ab = sheet.AttackBonus + wab;
                var bonus = wab >= 0 ? "+" + wab : wab.ToString();
                bonus += " (" + (ab >= 0 ? "+" + ab : ab.ToString()) + ")";
                return bonus;
            }
            public CriticalHit critical = new CriticalHit();
            public DiceRoll damage = new DiceRoll();
            public string Damage(CharacterSheet sheet)
            {
                if (sheet == null)
                    return null;
                var wd = damage.AsString(sheet);
                return wd + " " + DamageBonus(sheet);
            }
            public string DamageBonus(CharacterSheet sheet)
            {
                if (sheet == null)
                    return null;
                var wdb = damageBonus.GetTotal(sheet);
                var db = sheet.DamageBonus + wdb;
                var bonus = (wdb >= 0 ? "+" + wdb : wdb.ToString());
                bonus += " (" + (db >= 0 ? "+" + db : db.ToString()) + ")";
                return bonus;
            }
            public override string AsString(CharacterSheet sheet)
            {
                var weapon = string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    weapon += name + ": ";
                weapon += AttackBonus(sheet);
                var c = critical.AsString(sheet);
                if (!string.IsNullOrWhiteSpace(c))
                    weapon += ", " + c;
                var d = Damage(sheet);
                if (!string.IsNullOrWhiteSpace(d))
                    weapon += ", " + d;
                var r = range.GetTotal(sheet);
                if (r > 0)
                {
                    weapon += ", " + Range(sheet);
                    weapon += ", " + ammunition.GetTotal(sheet).ToString();
                }
                return weapon;
            }
            public ValueWithIntModifiers damageBonus = new ValueWithIntModifiers();
            public string type = null;
            public ValueWithIntModifiers range = new ValueWithIntModifiers();
            public string Range(CharacterSheet sheet)
            {
                return range.GetTotal(sheet) + " ft";
            }
            public ValueWithIntModifiers ammunition = new ValueWithIntModifiers();
            public string special = null;

            public override object Clone
            {
                get
                {
                    var clone = new WeaponItem();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(WeaponItem other)
            {
                if (other == null)
                    return false;
                if (!base.Equals(other))
                    return false;
                if (!attackBonus.Equals(other.attackBonus))
                    return false;
                if (!critical.Equals(other.critical))
                    return false;
                if (!damage.Equals(other.damage))
                    return false;
                if (!damageBonus.Equals(other.damageBonus))
                    return false;
                if (type != other.type)
                    return false;
                if (!range.Equals(other.range))
                    return false;
                if (!ammunition.Equals(other.ammunition))
                    return false;
                if (special != other.special)
                    return false;
                return true;
            }

            public WeaponItem Fill(WeaponItem source)
            {
                if (source == null)
                    return this;
                base.Fill(source);
                attackBonus = source.attackBonus.Clone as ValueWithIntModifiers;
                critical = source.critical.Clone as CriticalHit;
                damage = source.damage.Clone as DiceRoll;
                damageBonus = source.damageBonus.Clone as ValueWithIntModifiers;
                type = source.type;
                range = source.range.Clone as ValueWithIntModifiers;
                ammunition = source.ammunition.Clone as ValueWithIntModifiers;
                special = source.special;
                return this;
            }
        }

        public class ArmorClassItem: GearItem
        {
            public enum ArmorTypes
            {
                Armor,
                Shield,
                Other,
            }

            public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
            public string ArmorBonus(CharacterSheet sheet)
            {
                var ab = armorBonus.GetTotal(sheet);
                return  ab >= 0 ? "+" + ab : ab.ToString();
            }
            public string armorType = ArmorTypes.Other.ToString();
            public ArmorTypes ArmorType
            {
                get { return GetEnumValue(armorType, ArmorTypes.Other); }
                set { armorType = value.ToString(); }
            }
            public bool limitMaxDexBonus = false;
            public ValueWithIntModifiers maxDexBonus = new ValueWithIntModifiers();
            public string MaxDexBonus(CharacterSheet sheet)
            {
                if (!limitMaxDexBonus)
                    return "-";
                var mdb = maxDexBonus.GetTotal(sheet);
                return mdb >= 0 ? "+" + mdb : mdb.ToString();
            }
            public ValueWithIntModifiers checkPenalty = new ValueWithIntModifiers();
            public string CheckPenalty(CharacterSheet sheet)
            {
                return checkPenalty.GetTotal(sheet).ToString();
            }
            public ValueWithIntModifiers spellFailure = new ValueWithIntModifiers();
            public string SpellFailure(CharacterSheet sheet)
            {
                return spellFailure.GetTotal(sheet) + "%";
            }
            public string properties = null;
            public override string AsString(CharacterSheet sheet)
            {
                var armor = string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    armor += name + " ";
                armor += "(" + armorType;
                //if (active)
                //    armor += ", active";
                armor += "): ";
                armor += ArmorBonus(sheet);
                if (limitMaxDexBonus)
                    armor += ", " + MaxDexBonus(sheet);
                armor += ", " + CheckPenalty(sheet);
                armor += ", " + SpellFailure(sheet);
                if (!string.IsNullOrWhiteSpace(properties))
                    armor += ", " + properties;
                return armor;
            }

            public override object Clone
            {
                get
                {
                    var clone = new ArmorClassItem();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(ArmorClassItem other)
            {
                if (other == null)
                    return false;
                if (!base.Equals(other))
                    return false;
                if (!armorBonus.Equals(other.armorBonus))
                    return false;
                if (armorType != other.armorType)
                    return false;
                if (limitMaxDexBonus != other.limitMaxDexBonus)
                    return false;
                if (!maxDexBonus.Equals(other.maxDexBonus))
                    return false;
                if (!checkPenalty.Equals(other.checkPenalty))
                    return false;
                if (!spellFailure.Equals(other.spellFailure))
                    return false;
                if (properties != other.properties)
                    return false;
                return true;
            }

            public ArmorClassItem Fill(ArmorClassItem source)
            {
                if (source == null)
                    return this;
                base.Fill(source);
                selected = source.selected;
                armorBonus = source.armorBonus.Clone as ValueWithIntModifiers;
                armorType = source.armorType;
                limitMaxDexBonus = source.limitMaxDexBonus;
                maxDexBonus = source.maxDexBonus.Clone as ValueWithIntModifiers;
                checkPenalty = source.checkPenalty.Clone as ValueWithIntModifiers;
                spellFailure = source.spellFailure.Clone as ValueWithIntModifiers;
                properties = source.properties;
                return this;
            }
        }

        public class ArmorClass
        {
            public enum DexterityModifierSources
            {
                DependsOnACItems,
                Full,
                Custom,
            }

            public ValueWithIntModifiers armorBonus = new ValueWithIntModifiers();
            public bool itemsArmorBonus = true;
            public ValueWithIntModifiers shieldBonus = new ValueWithIntModifiers();
            public bool itemsShieldBonus = true;
            public string dexterityModifierSource = DexterityModifierSources.DependsOnACItems.ToString();
            public DexterityModifierSources DexterityModifierSource
            {
                get { return GetEnumValue(dexterityModifierSource, DexterityModifierSources.DependsOnACItems); }
                set { dexterityModifierSource = value.ToString(); }
            }
            public ValueWithIntModifiers dexterityModifier = new ValueWithIntModifiers();
            public ValueWithIntModifiers sizeModifier = new ValueWithIntModifiers();
            public ValueWithIntModifiers naturalArmor = new ValueWithIntModifiers();
            public ValueWithIntModifiers deflectionModifier = new ValueWithIntModifiers();
            public ValueWithIntModifiers miscModifiers = new ValueWithIntModifiers();

            public virtual object Clone
            {
                get
                {
                    return new ArmorClass()
                    {
                        armorBonus = armorBonus.Clone as ValueWithIntModifiers,
                        itemsArmorBonus = itemsArmorBonus,
                        shieldBonus = shieldBonus.Clone as ValueWithIntModifiers,
                        itemsShieldBonus = itemsShieldBonus,
                        dexterityModifierSource = dexterityModifierSource,
                        dexterityModifier = dexterityModifier.Clone as ValueWithIntModifiers,
                        sizeModifier = sizeModifier.Clone as ValueWithIntModifiers,
                        naturalArmor = naturalArmor.Clone as ValueWithIntModifiers,
                        deflectionModifier = deflectionModifier.Clone as ValueWithIntModifiers,
                        miscModifiers = miscModifiers.Clone as ValueWithIntModifiers,
                    };
                }
            }

            public bool Equals(ArmorClass other)
            {
                if (itemsArmorBonus != other.itemsArmorBonus)
                    return false;
                if (!armorBonus.Equals(other.armorBonus))
                    return false;
                if (itemsShieldBonus != other.itemsShieldBonus)
                    return false;
                if (!shieldBonus.Equals(other.shieldBonus))
                    return false;
                if (dexterityModifierSource != other.dexterityModifierSource)
                    return false;
                if (!dexterityModifier.Equals(other.dexterityModifier))
                    return false;
                if (!sizeModifier.Equals(other.sizeModifier))
                    return false;
                if (!naturalArmor.Equals(other.naturalArmor))
                    return false;
                if (!deflectionModifier.Equals(other.deflectionModifier))
                    return false;
                if (!miscModifiers.Equals(other.miscModifiers))
                    return false;
                return true;
            }

            private int GetBaseArmorClass(CharacterSheet sheet)
            {
                return 10 + sizeModifier.GetTotal(sheet) + deflectionModifier.GetTotal(sheet) + miscModifiers.GetTotal(sheet);
            }

            public int GetArmorBonus(CharacterSheet sheet, ArmorClassItem.ArmorTypes type)
            {
                var ac = 0;
                foreach (var item in sheet.armorClassItems)
                {
                    if (item == null)
                        continue;
                    if (!item.active)
                        continue;
                    if (item.ArmorType != type)
                        continue;
                    ac += item.armorBonus.GetTotal(sheet);
                }
                return ac;
            }

            public int GetArmorBonus(CharacterSheet sheet)
            {
                if (!itemsArmorBonus)
                    return armorBonus.GetTotal(sheet);
                return GetArmorBonus(sheet, ArmorClassItem.ArmorTypes.Armor);
            }

            public int GetShieldBonus(CharacterSheet sheet)
            {
                if (!itemsShieldBonus)
                    return armorBonus.GetTotal(sheet);
                return GetArmorBonus(sheet, ArmorClassItem.ArmorTypes.Shield);
            }

            private ValueWithIntModifiers GetDexBonusLimit(CharacterSheet sheet)
            {
                ValueWithIntModifiers minValue = null;
                var value = 0;
                foreach (var item in sheet.armorClassItems)
                {
                    if (item == null)
                        continue;
                    if (!item.active)
                        continue;
                    if (!item.limitMaxDexBonus)
                        continue;
                    var itemValue = item.maxDexBonus.GetTotal(sheet);
                    if ((minValue == null) || (value > itemValue))
                    {
                        minValue = item.maxDexBonus;
                        value = itemValue;
                    }
                }
                return minValue;
            }

            public int GetDexterityModifier(CharacterSheet sheet)
            {
                var full = GetAbilityModifier(sheet, Ability.Dexterity);
                switch (DexterityModifierSource)
                {
                    case DexterityModifierSources.DependsOnACItems:
                        var limit = GetDexBonusLimit(sheet);
                        if (limit == null)
                            return full;
                        var maxValue = limit.GetTotal(sheet);
                        if (full < maxValue)
                            return full;
                        return maxValue;
                    case DexterityModifierSources.Full:
                        return full;
                    case DexterityModifierSources.Custom:
                        return dexterityModifier.GetTotal(sheet);
                    default:
                        return full;
                }
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetTotal(sheet);
            }

            public int GetTouch(CharacterSheet sheet)
            {
                return GetBaseArmorClass(sheet) + GetDexterityModifier(sheet);
            }

            public int GetFlatFooted(CharacterSheet sheet)
            {
                return GetBaseArmorClass(sheet) + GetArmorBonus(sheet) + GetShieldBonus(sheet) + naturalArmor.GetTotal(sheet);
            }
        }

        public class DiceRoll
        {
            public ValueWithIntModifiers diceCount = new ValueWithIntModifiers();
            public ValueWithIntModifiers diceSides = new ValueWithIntModifiers();
            public ValueWithIntModifiers additional = new ValueWithIntModifiers();
            public string AsString(CharacterSheet sheet)
            {
                var sides = diceSides.GetTotal(sheet);
                if (sides <= 0)
                    return string.Empty;
                var roll = diceCount.GetTotal(sheet) + "d" + sides;
                var add = additional.GetTotal(sheet);
                if (add == 0)
                    return "(" + roll + ")";
                if (add < 0)
                    roll += " - " + Math.Abs(add);
                else
                    roll += " + " + add;
                return "(" + roll + ")";
            }

            public object Clone
            {
                get
                {
                    var clone = new DiceRoll();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(DiceRoll other)
            {
                if (!diceCount.Equals(other.diceCount))
                    return false;
                if (!diceSides.Equals(other.diceSides))
                    return false;
                if (!additional.Equals(other.additional))
                    return false;
                return true;
            }

            public DiceRoll Fill(DiceRoll source)
            {
                if (source == null)
                    return this;
                diceCount = source.diceCount.Clone as ValueWithIntModifiers;
                diceSides = source.diceSides.Clone as ValueWithIntModifiers;
                additional = source.additional.Clone as ValueWithIntModifiers;
                return this;
            }
        }

        public class CriticalHit
        {
            public ValueWithIntModifiers min = new ValueWithIntModifiers();
            public ValueWithIntModifiers max = new ValueWithIntModifiers();
            public ValueWithIntModifiers multiplier = new ValueWithIntModifiers();
            public string AsString(CharacterSheet sheet)
            {
                var mint = min.GetTotal(sheet);
                var maxt = max.GetTotal(sheet);
                var mul = multiplier.GetTotal(sheet);
                if (mul <= 0)
                    return string.Empty;
                var crit = string.Empty;
                if (mint < maxt)
                    crit += mint + "-";
                crit += maxt;
                crit += "/x" + mul;
                return crit;
            }

            public object Clone
            {
                get
                {
                    var clone = new CriticalHit();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(CriticalHit other)
            {
                if (!min.Equals(other.min))
                    return false;
                if (!max.Equals(other.max))
                    return false;
                if (!multiplier.Equals(other.multiplier))
                    return false;
                return true;
            }

            public CriticalHit Fill(CriticalHit source)
            {
                if (source == null)
                    return this;
                min = source.min.Clone as ValueWithIntModifiers;
                max = source.max.Clone as ValueWithIntModifiers;
                multiplier = source.multiplier.Clone as ValueWithIntModifiers;
                return this;
            }
        }

        public class Money
        {
            public ValueWithIntModifiers cuprumPoints = new ValueWithIntModifiers();
            public ValueWithIntModifiers silverPoints = new ValueWithIntModifiers();
            public ValueWithIntModifiers goldenPoints = new ValueWithIntModifiers();
            public ValueWithIntModifiers platinumPoints = new ValueWithIntModifiers();

            public object Clone
            {
                get
                {
                    var clone = new Money();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(Money other)
            {
                if (!cuprumPoints.Equals(other.cuprumPoints))
                    return false;
                if (!silverPoints.Equals(other.silverPoints))
                    return false;
                if (!goldenPoints.Equals(other.goldenPoints))
                    return false;
                if (!platinumPoints.Equals(other.platinumPoints))
                    return false;
                return true;
            }

            public Money Fill(Money source)
            {
                if (source == null)
                    return this;
                cuprumPoints = source.cuprumPoints.Clone as ValueWithIntModifiers;
                silverPoints = source.silverPoints.Clone as ValueWithIntModifiers;
                goldenPoints = source.goldenPoints.Clone as ValueWithIntModifiers;
                platinumPoints = source.platinumPoints.Clone as ValueWithIntModifiers;
                return this;
            }
        }

        public class Encumbrance
        {
            public ValueWithIntModifiers lightLoad = new ValueWithIntModifiers();
            public string LightLoad(CharacterSheet sheet)
            {
                return lightLoad.GetTotal(sheet) + " lbs";
            }
            public ValueWithIntModifiers mediumLoad = new ValueWithIntModifiers();
            public string MediumLoad(CharacterSheet sheet)
            {
                var ml = mediumLoad.GetTotal(sheet);
                var text = ml + " lbs";
                var ll = lightLoad.GetTotal(sheet) + 1;
                if (ll < ml)
                    text = ll + " - " + text;
                return text;
            }
            public ValueWithIntModifiers heavyLoad = new ValueWithIntModifiers();
            public string HeavyLoad(CharacterSheet sheet)
            {
                var hl = heavyLoad.GetTotal(sheet);
                var text = hl + " lbs";
                var ml = mediumLoad.GetTotal(sheet) + 1;
                if (ml < hl)
                    text = ml + " - " + text;
                return text;
            }

            public bool defaultLiftOverHead = true;
            public ValueWithIntModifiers liftOverHead = new ValueWithIntModifiers();
            public int LiftOverHead(CharacterSheet sheet)
            {
                if (defaultLiftOverHead)
                    return heavyLoad.GetTotal(sheet);
                return liftOverHead.GetTotal(sheet);
            }

            public bool defaultLiftOffGround = true;
            public ValueWithIntModifiers liftOffGround = new ValueWithIntModifiers();
            public int LiftOffGround(CharacterSheet sheet)
            {
                if (defaultLiftOffGround)
                    return 2 * LiftOverHead(sheet);
                return liftOffGround.GetTotal(sheet);
            }

            public bool defaultDragOrPush = true;
            public ValueWithIntModifiers dragOrPush = new ValueWithIntModifiers();
            public int DragOrPush(CharacterSheet sheet)
            {
                if (defaultDragOrPush)
                    return 5 * LiftOverHead(sheet);
                return dragOrPush.GetTotal(sheet);
            }

            public object Clone
            {
                get
                {
                    var clone = new Encumbrance();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(Encumbrance other)
            {
                if (!lightLoad.Equals(other.lightLoad))
                    return false;
                if (!mediumLoad.Equals(other.mediumLoad))
                    return false;
                if (!heavyLoad.Equals(other.heavyLoad))
                    return false;
                if (defaultLiftOverHead != other.defaultLiftOverHead)
                    return false;
                if (!liftOverHead.Equals(other.liftOverHead))
                    return false;
                if (defaultLiftOffGround != other.defaultLiftOffGround)
                    return false;
                if (!liftOffGround.Equals(other.liftOffGround))
                    return false;
                if (defaultDragOrPush != other.defaultDragOrPush)
                    return false;
                if (!dragOrPush.Equals(other.dragOrPush))
                    return false;
                return true;
            }

            public Encumbrance Fill(Encumbrance source)
            {
                if (source == null)
                    return this;
                lightLoad = source.lightLoad.Clone as ValueWithIntModifiers;
                mediumLoad = source.mediumLoad.Clone as ValueWithIntModifiers;
                heavyLoad = source.heavyLoad.Clone as ValueWithIntModifiers;
                liftOverHead = source.liftOverHead.Clone as ValueWithIntModifiers;
                liftOffGround = source.liftOffGround.Clone as ValueWithIntModifiers;
                dragOrPush = source.dragOrPush.Clone as ValueWithIntModifiers;
                defaultLiftOverHead = source.defaultLiftOverHead;
                defaultLiftOffGround = source.defaultLiftOffGround;
                defaultDragOrPush = source.defaultDragOrPush;
                return this;
            }
        }

        public const int spellLevelsCount = 10;

        public class SpellLevel
        {
            public ValueWithIntModifiers spellsKnown = new ValueWithIntModifiers();
            public ValueWithIntModifiers spellSaveDC = new ValueWithIntModifiers();
            public ValueWithIntModifiers spellsPerDay = new ValueWithIntModifiers();
            public ValueWithIntModifiers bonusSpells = new ValueWithIntModifiers();
        }

        public class ChannelEnergy
        {
            public ValueWithIntModifiers left = new ValueWithIntModifiers();
            public ValueWithIntModifiers total = new ValueWithIntModifiers();
            public DiceRoll points = new DiceRoll();

            public object Clone
            {
                get
                {
                    var clone = new ChannelEnergy();
                    clone.Fill(this);
                    return clone;
                }
            }

            public bool Equals(ChannelEnergy other)
            {
                if (other == null)
                    return false;
                if (!left.Equals(other.left))
                    return false;
                if (!total.Equals(other.total))
                    return false;
                if (!points.Equals(other.points))
                    return false;
                return true;
            }

            public ChannelEnergy Fill(ChannelEnergy source)
            {
                if (source == null)
                    return this;
                left = source.left.Clone as ValueWithIntModifiers;
                total = source.total.Clone as ValueWithIntModifiers;
                points = source.points.Clone as DiceRoll;
                return this;
            }
        }

        public CharacterSheet()
        {

        }

        public void Init()
        {
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
            get { return GetEnumValue(alignment, Alignments.Neutral); }
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
        public List<LevelOfClass> levelOfClass = new List<LevelOfClass>();
        public int TotalLevel { get { return LevelOfClass.Total(this, levelOfClass); } }
        public string LevelAsString { get { return LevelOfClass.AsString(this, levelOfClass); } }
        public ValueWithIntModifiers experience = new ValueWithIntModifiers();
        public ValueWithIntModifiers nextLevelExperience = new ValueWithIntModifiers();
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

        public ValueWithIntModifiers spellResistance = new ValueWithIntModifiers();
        #endregion

        #region Attack
        public Initiative initiative = new Initiative();
        public int CurrentInitiative { get { return initiative.GetInitiative(this); } }

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
            ValueWithIntModifiers bab = null;
            if (attack < 0)
                bab = babs[0];
            else if (attack >= count)
                bab = babs[count - 1];
            else
                bab = babs[attack];
            if (bab != null)
                return bab.GetTotal(this);
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
                text = (attack + 1) + ": (" + text + ")";
            return text;
        }

        public ValueWithIntModifiers cmdSizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers cmbSizeModifier = new ValueWithIntModifiers();
        public int GetCMD(CharacterSheet sheet, ValueWithIntModifiers sizeModifier, int attack) { return 10 + GetBaseAttackBonus(attack) + GetAbilityModifier(this, Ability.Strength) + GetAbilityModifier(this, Ability.Dexterity) + sizeModifier.GetTotal(sheet); }
        public int CMD { get { return GetCMD(this, cmdSizeModifier, currentAttack); } }
        public int GetCMB(CharacterSheet sheet, ValueWithIntModifiers sizeModifier, int attack) { return GetBaseAttackBonus(attack) + GetAbilityModifier(this, Ability.Strength) + sizeModifier.GetTotal(sheet); }
        public int CMB { get { return GetCMB(this, cmbSizeModifier, currentAttack); } }

        public ValueWithIntModifiers attackSizeModifier = new ValueWithIntModifiers();
        public ValueWithIntModifiers attackBonusModifiers = new ValueWithIntModifiers();
        public int AttackBonus { get { return GetBaseAttackBonus() + attackBonusModifiers.GetTotal(this) + attackSizeModifier.GetTotal(this); } }
        public ValueWithIntModifiers damageBonusModifiers = new ValueWithIntModifiers();
        public int DamageBonus { get { return damageBonusModifiers.GetTotal(this); } }
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
                    sum += aci.checkPenalty.GetTotal(this);
            return sum;
        }
        #endregion

        #region Skills
        public ValueWithIntModifiers skillsPerLevel = new ValueWithIntModifiers();
        public int currentSkillsPerLevel { get { return GetAbilityModifier(this, Ability.Intelligence) + skillsPerLevel.GetTotal(this); } }
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
