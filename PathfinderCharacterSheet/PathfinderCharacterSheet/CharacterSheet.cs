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
            public int tempAdjustment = 0;
            public int tempModifier = 0;
            public int Modifier
            {
                get
                {
                    return CalcModifier(score, tempAdjustment, tempModifier);
                }
            }
            public static int CalcModifier(int score, int tempAdjustment, int tempModifier)
            {
                return (score + tempAdjustment) / 2 - 5 + tempModifier;
            }
        }

        public class IntModifier
        {
            public bool active = true;
            public bool IsActive { get { return active; } }
            public string name = null;
            public string Name { get { return name; } }
            public int value = 0;
            public int Value { get { return value; } }

            public bool Equals(IntModifier obj)
            {
                var other = obj as IntModifier;
                if (other == null)
                    return false;
                if (other.IsActive != IsActive)
                    return false;
                if (other.Name != Name)
                    return false;
                if (other.Value != Value)
                    return false;
                return true;
            }
        }

        public static int Sum(List<IntModifier> modifiers, bool activeOnly = true)
        {
            int value = 0;
            foreach (var m in modifiers)
                if (m.IsActive || !activeOnly)
                    value += m.Value;
            return value;
        }

        public static bool IsEqual(List<IntModifier> a, List<IntModifier> b)
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
            public List<IntModifier> magicModifiers = new List<IntModifier>();
            public List<IntModifier> tempModifiers = new List<IntModifier>();

            public SavingThrow()
            {

            }

            public SavingThrow(Ability abilityModifierSource)
            {
                this.abilityModifierSource = abilityModifierSource;
            }

            public int GetTotal(CharacterSheet sheet)
            {
                return baseSave + GetAbilityModifier(sheet, abilityModifierSource) + Sum(magicModifiers) + Sum(tempModifiers);
            }
        }

        public class Speed
        {
            public int baseSpeed = 0;
            public List<IntModifier> tempModifiers = new List<IntModifier>();
            public int CurrentBaseSpeed
            {
                get
                {
                    return baseSpeed + Sum(tempModifiers);
                }
            }
            public int armorSpeed = 0;
            public int CurrentArmorSpeed
            {
                get
                {
                    return armorSpeed + Sum(tempModifiers);
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
            public int maxHP = 0;
            public int hp = 0;
            public List<IntModifier> tempHP = new List<IntModifier>();
            public int damageResist = 0;
            public int CurrentHP { get { return hp + Sum(tempHP); } }
        }

        public class Initiative
        {
            public List<IntModifier> miscModifiers = new List<IntModifier>();
            public int GetInitiative(CharacterSheet sheet)
            {
                return GetAbilityModifier(sheet, Ability.Dexterity) + Sum(miscModifiers);
            }
        }

        public enum Alignment
        {
            None = -1,
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
            public List<IntModifier> miscModifiers = new List<IntModifier>();
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
            public List<IntModifier> miscModifiers = new List<IntModifier>();
            private int GetBaseArmorClass
            {
                get
                {
                    return 10 + sizeModifier + deflectionModifier + Sum(miscModifiers);
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

        public class Damage
        {
            public int diceCount = 0;
            public int diceValue = 0;
            public List<IntModifier> miscModifiers = new List<IntModifier>();
        }

        public class WeaponItem: Item
        {
            public int bonus = 0;
            public string critical = null;
            public string type = null;
            public int range = 0;
            public int ammunition = 0;
            public Damage damage = new Damage();
        }

        public class Item
        {
            public string name = null;
            public string description = null;
            public int amount = 0;
            public int weight = 0;
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
        public string Name { get { return name; } }
        public string biography = null;
        public Alignment alignment = Alignment.None;
        public string deity = null;
        public string homeland = null;
        public string race = null;
        public string Race { get { return race; } }
        public string size = null;
        public string gender = null;
        public int age = 0;
        public int height = 0;
        public int weight = 0;
        public string hair = null;
        public string eyes = null;
        #endregion

        #region Level
        public List<LevelOfClass> levelOfClass = new List<LevelOfClass>();
        public int TotalLevel
        {
            get
            {
                var level = 0;
                if (levelOfClass != null)
                    foreach (var loc in levelOfClass)
                        if (loc != null)
                            level += loc.level;
                return level;
            }
        }
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
        public int ACMiscModifier { get { return Sum(armorClass.miscModifiers); } }
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

        public int baseAttackBonus = 0;

        public int combatManeuverSpecialSizeModifier = 0;
        public int CurrentCMD { get { return 10 + baseAttackBonus + GetAbilityModifier(this, Ability.Strength) + GetAbilityModifier(this, Ability.Dexterity) + combatManeuverSpecialSizeModifier; } }
        public int CurrentCMB { get { return baseAttackBonus + GetAbilityModifier(this, Ability.Strength) + combatManeuverSpecialSizeModifier; } }

        public int sizeModifier = 0;
        public List<IntModifier> meleeAttackBonusModifiers = new List<IntModifier>();
        public int MeleeAttackBonus { get { return baseAttackBonus + Sum(meleeAttackBonusModifiers) + GetAbilityModifier(this, Ability.Strength) + sizeModifier; } }
        public List<IntModifier> meleeDamageBonusModifiers = new List<IntModifier>();
        public int MeleeDamageBonus { get { return Sum(meleeDamageBonusModifiers) + GetAbilityModifier(this, Ability.Strength); } }
        public List<IntModifier> rangeAttackBonusModifiers = new List<IntModifier>();
        public int RangeAttackBonus { get { return baseAttackBonus + Sum(rangeAttackBonusModifiers) + GetAbilityModifier(this, Ability.Dexterity) + sizeModifier; } }
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
        public SkillRank[] skills = new SkillRank[]
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
        public int channelsMaxCount = 0;
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
