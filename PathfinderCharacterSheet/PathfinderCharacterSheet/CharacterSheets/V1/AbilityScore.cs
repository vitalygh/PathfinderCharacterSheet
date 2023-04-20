using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class AbilityScore: IPrototype<AbilityScore>, IEquatable<AbilityScore>
    {
        public ValueWithIntModifiers score = new ValueWithIntModifiers();
        public ValueWithIntModifiers tempAdjustment = new ValueWithIntModifiers();
        public int GetModifier(CharacterSheet sheet)
        {
            return CalcModifier(score.GetValue(sheet), 0);
        }
        public int GetTempModifier(CharacterSheet sheet)
        {
            return CalcModifier(score.GetValue(sheet), tempAdjustment.GetValue(sheet));
        }
        private static int CalcModifier(int score, int tempAdjustment)
        {
            return (score + tempAdjustment) / 2 - 5;
        }
        public virtual AbilityScore Clone
        {
            get
            {
                var abs = new AbilityScore();
                abs.Fill(this);
                return abs;
            }
        }
        public bool Equals(AbilityScore other)
        {
            if (other == null)
                return false;
            if (score != other.score)
                return false;
            if (tempAdjustment != other.tempAdjustment)
                return false;
            return true;
        }

        public AbilityScore Fill(AbilityScore source)
        {
            if (source == null)
                return this;
            score = source.score?.Clone;
            tempAdjustment = source.tempAdjustment?.Clone;
            return this;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as AbilityScore);
        }

        public static bool operator ==(AbilityScore first, AbilityScore second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(AbilityScore first, AbilityScore second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, score) ? score.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, tempAdjustment) ? tempAdjustment.GetHashCode() : 0);
            return hash;
        }
    }
}
