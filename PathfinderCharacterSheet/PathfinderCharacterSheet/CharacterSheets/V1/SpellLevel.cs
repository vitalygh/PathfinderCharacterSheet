using System;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class SpellLevel : IPrototype<SpellLevel>, IEquatable<SpellLevel>
    {
        public ValueWithIntModifiers spellsKnown = new ValueWithIntModifiers();
        public ValueWithIntModifiers spellSaveDC = new ValueWithIntModifiers();
        public ValueWithIntModifiers spellsPerDay = new ValueWithIntModifiers();
        public ValueWithIntModifiers bonusSpells = new ValueWithIntModifiers();

        public virtual SpellLevel Clone
        {
            get
            {
                var speed = new SpellLevel();
                speed.Fill(this);
                return speed;
            }
        }

        public bool Equals(SpellLevel other)
        {
            if (other == null)
                return false;
            if (spellsKnown != other.spellsKnown)
                return false;
            if (spellSaveDC != other.spellSaveDC)
                return false;
            if (spellsPerDay != other.spellsPerDay)
                return false;
            if (bonusSpells != other.bonusSpells)
                return false;
            return true;
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals(other as SpellLevel);
        }

        public static bool operator ==(SpellLevel first, SpellLevel second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(SpellLevel first, SpellLevel second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (spellsKnown is null ? 0 : spellsKnown.GetHashCode());
            hash = (hash * 7) + (spellSaveDC is null ? 0 : spellSaveDC.GetHashCode());
            hash = (hash * 7) + (spellsPerDay is null ? 0 : spellsPerDay.GetHashCode());
            hash = (hash * 7) + (bonusSpells is null ? 0 : bonusSpells.GetHashCode());
            return hash;
        }

        public virtual SpellLevel Fill(SpellLevel source)
        {
            if (source == null)
                return this;
            spellsKnown = source.spellsKnown?.Clone;
            spellSaveDC = source.spellSaveDC?.Clone;
            spellsPerDay = source.spellsPerDay?.Clone;
            bonusSpells = source.bonusSpells?.Clone;
            return this;
        }
    }
}
