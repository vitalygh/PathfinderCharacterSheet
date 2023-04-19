using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
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
}
