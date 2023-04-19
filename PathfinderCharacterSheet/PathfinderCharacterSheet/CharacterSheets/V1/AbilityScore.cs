using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
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
                var abs = new AbilityScore();
                abs.Fill(this);
                return abs;
            }
        }
        public bool Equals(AbilityScore obj)
        {
            var other = obj as AbilityScore;
            if (other == null)
                return false;
            if (!other.score.Equals(score))
                return false;
            if (!other.tempAdjustment.Equals(tempAdjustment))
                return false;
            return true;
        }

        public AbilityScore Fill(AbilityScore source)
        {
            if (source == null)
                return this;
            score = source.score.Clone as ValueWithIntModifiers;
            tempAdjustment = source.tempAdjustment.Clone as ValueWithIntModifiers;
            return this;
        }
    }
}
