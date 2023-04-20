using System;
using System.Collections.Generic;
using System.Text;

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
            if (ReferenceEquals(null, other))
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
            if (ReferenceEquals(null, first))
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
            hash = (hash * 7) + (!ReferenceEquals(null, spellsKnown) ? spellsKnown.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, spellSaveDC) ? spellSaveDC.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, spellsPerDay) ? spellsPerDay.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, bonusSpells) ? bonusSpells.GetHashCode() : 0);
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
