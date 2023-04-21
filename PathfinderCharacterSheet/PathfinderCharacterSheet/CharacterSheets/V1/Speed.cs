using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Speed: IPrototype<Speed>, IEquatable<Speed>
    {
        public ValueWithIntModifiers baseSpeed = new ValueWithIntModifiers();
        public ValueWithIntModifiers armorSpeed = new ValueWithIntModifiers();
        public ValueWithIntModifiers flySpeed = new ValueWithIntModifiers();
        public ValueWithIntModifiers maneuverability = new ValueWithIntModifiers();
        public ValueWithIntModifiers swimSpeed = new ValueWithIntModifiers();
        public ValueWithIntModifiers climbSpeed = new ValueWithIntModifiers();
        public bool defaultSwimSpeed = true;
        public int GetSwimSpeed(CharacterSheet sheet) { return defaultSwimSpeed ? (baseSpeed.GetValue(sheet) / 2) : swimSpeed.GetValue(sheet); }
        public bool defaultClimbSpeed = true;
        public int GetClimbSpeed(CharacterSheet sheet) { return defaultClimbSpeed ? (baseSpeed.GetValue(sheet) / 4) : climbSpeed.GetValue(sheet); }
        public ValueWithIntModifiers burrowSpeed = new ValueWithIntModifiers();

        public static int InSquares(int speed)
        {
            return speed / 5;
        }

        public virtual Speed Clone
        {
            get
            {
                var speed = new Speed();
                speed.Fill(this);
                return speed;
            }
        }

        public bool Equals(Speed other)
        {
            if (other == null)
                return false;
            if (baseSpeed != other.baseSpeed)
                return false;
            if (armorSpeed != other.armorSpeed)
                return false;
            if (flySpeed != other.flySpeed)
                return false;
            if (maneuverability != other.maneuverability)
                return false;
            if (swimSpeed != other.swimSpeed)
                return false;
            if (climbSpeed != other.climbSpeed)
                return false;
            if (defaultClimbSpeed != other.defaultClimbSpeed)
                return false;
            if (defaultSwimSpeed != other.defaultSwimSpeed)
                return false;
            if (burrowSpeed != other.burrowSpeed)
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
            return Equals(other as Speed);
        }

        public static bool operator ==(Speed first, Speed second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (first is null)
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Speed first, Speed second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (baseSpeed is null ? 0 : baseSpeed.GetHashCode());
            hash = (hash * 7) + (armorSpeed is null ? 0 : armorSpeed.GetHashCode());
            hash = (hash * 7) + (flySpeed is null ? 0 : flySpeed.GetHashCode());
            hash = (hash * 7) + (maneuverability is null ? 0 : maneuverability.GetHashCode());
            hash = (hash * 7) + (swimSpeed is null ? 0 : swimSpeed.GetHashCode());
            hash = (hash * 7) + (climbSpeed is null ? 0 : climbSpeed.GetHashCode());
            hash = (hash * 7) + defaultClimbSpeed.GetHashCode();
            hash = (hash * 7) + defaultSwimSpeed.GetHashCode();
            hash = (hash * 7) + (burrowSpeed is null ? 0 : burrowSpeed.GetHashCode());
            return hash;
        }

        public virtual Speed Fill(Speed source)
        {
            if (source == null)
                return this;
            baseSpeed = source.baseSpeed?.Clone;
            armorSpeed = source.armorSpeed?.Clone;
            flySpeed = source.flySpeed?.Clone;
            maneuverability = source.maneuverability?.Clone;
            swimSpeed = source.swimSpeed?.Clone;
            climbSpeed = source.climbSpeed?.Clone;
            defaultClimbSpeed = source.defaultClimbSpeed;
            defaultSwimSpeed = source.defaultSwimSpeed;
            burrowSpeed = source.burrowSpeed?.Clone;
            return this;
        }
    }
}
