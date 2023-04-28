using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public static class StaticChecker
    {
        private static void DefaultsSafetyKeeper()
        {
            /* HOW TO SAFE CHANGE DEFAULT:

            // REPLACE ************************************************************

            public const RoundingType DefaultRounding = RoundingType.Up;
            public string roundingType
            {
                get =>
            #if SAVE_DELTA
                    DefaultRounding == RoundingType ? null :
            #endif
                    RoundingType.ToString();
                set => RoundingType = Helpers.GetEnumValue(value, DefaultRounding);
            }
            internal RoundingType RoundingType { get; set; } = DefaultRounding;

            // WITH ***************************************************************

            public const RoundingType DefaultRoundingV1 = RoundingType.Up;
            public string roundingType
            {
                get => null;
                set => RoundingType = Helpers.GetEnumValue(value, DefaultRoundingV1);
            }

            public const RoundingType DefaultRounding = RoundingType.Down;
            public string roundingTypeV2
            {
                get =>
            #if SAVE_DELTA
                    DefaultRounding == RoundingType ? null :
            #endif
                    RoundingType.ToString();
                set => RoundingType = Helpers.GetEnumValue(value, DefaultRounding);
            }

            internal RoundingType RoundingType { get; set; } = DefaultRounding;

            // UPDATE CHECKS *******************************************************
            Check(IntMultiplier.DefaultRoundingV1 == RoundingType.Up ? 0 : -1);
            Check(IntMultiplier.DefaultRounding == RoundingType.Down ? 0 : -1);
            */


            Check(ArmorClass.DefaultDexterityModifierSource == DexterityModifierSource.DependsOnACItems ? 0 : -1);

            Check(ArmorClassItem.DefaultArmorType == ArmorType.Other ? 0 : -1);

            Check(CharacterSheet.DefaultAlignment == Alignment.Neutral ? 0 : -1);

            Check(IntLimit.DefaultMinLimit == false ? 0 : -1);
            Check(IntLimit.DefaultMinValue == 0 ? 0 : -1);
            Check(IntLimit.DefaultMaxLimit == false ? 0 : -1);
            Check(IntLimit.DefaultMaxValue == 0 ? 0 : -1);

            Check(IntModifier.DefaultSourceAbility == Ability.None ? 0 : -1);
            Check(IntModifier.DefaultSourceItemUID == CharacterSheet.InvalidUID ? 0 : -1);
            Check(IntModifier.DefaultMustBeActive == true ? 0 : -1);
            Check(IntModifier.DefaultMultiplyToLevel == false ? 0 : -1);
            Check(IntModifier.DefaultAutoNaming == true ? 0 : -1);

            Check(IntMultiplier.DefaultAdditionalBefore == 0 ? 0 : -1);
            Check(IntMultiplier.DefaultMultiplier == 1 ? 0 : -1);
            Check(IntMultiplier.DefaultDivider == 1 ? 0 : -1);
            Check(IntMultiplier.DefaultAdditionalAfter == 0 ? 0 : -1);
            Check(IntMultiplier.DefaultRounding == RoundingType.Up ? 0 : -1);

            Check(SavingThrow.DefaultAbilityModifierSource == Ability.None ? 0 : -1);

            Check(SkillRank.DefaultAbilityModifierSource == Ability.None ? 0 : -1);

            Check(ValueWithIntModifiers.DefaultBaseValue == 0 ? 0 : -1);
        }

        private static void Check(byte test) { }
    }
}
