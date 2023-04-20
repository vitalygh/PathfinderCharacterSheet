using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
    public class Encumbrance: IPrototype<Encumbrance>, IEquatable<Encumbrance>
    {
        public ValueWithIntModifiers lightLoad = new ValueWithIntModifiers();
        public string LightLoad(CharacterSheet sheet)
        {
            return lightLoad.GetValue(sheet) + " lbs";
        }
        public ValueWithIntModifiers mediumLoad = new ValueWithIntModifiers();
        public string MediumLoad(CharacterSheet sheet)
        {
            var ml = mediumLoad.GetValue(sheet);
            var text = ml + " lbs";
            var ll = lightLoad.GetValue(sheet) + 1;
            if (ll < ml)
                text = ll + " - " + text;
            return text;
        }
        public ValueWithIntModifiers heavyLoad = new ValueWithIntModifiers();
        public string HeavyLoad(CharacterSheet sheet)
        {
            var hl = heavyLoad.GetValue(sheet);
            var text = hl + " lbs";
            var ml = mediumLoad.GetValue(sheet) + 1;
            if (ml < hl)
                text = ml + " - " + text;
            return text;
        }

        public bool defaultLiftOverHead = true;
        public ValueWithIntModifiers liftOverHead = new ValueWithIntModifiers();
        public int LiftOverHead(CharacterSheet sheet)
        {
            if (defaultLiftOverHead)
                return heavyLoad.GetValue(sheet);
            return liftOverHead.GetValue(sheet);
        }

        public bool defaultLiftOffGround = true;
        public ValueWithIntModifiers liftOffGround = new ValueWithIntModifiers();
        public int LiftOffGround(CharacterSheet sheet)
        {
            if (defaultLiftOffGround)
                return 2 * LiftOverHead(sheet);
            return liftOffGround.GetValue(sheet);
        }

        public bool defaultDragOrPush = true;
        public ValueWithIntModifiers dragOrPush = new ValueWithIntModifiers();
        public int DragOrPush(CharacterSheet sheet)
        {
            if (defaultDragOrPush)
                return 5 * LiftOverHead(sheet);
            return dragOrPush.GetValue(sheet);
        }

        public virtual Encumbrance Clone
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
            if (lightLoad != other.lightLoad)
                return false;
            if (mediumLoad != other.mediumLoad)
                return false;
            if (heavyLoad != other.heavyLoad)
                return false;
            if (defaultLiftOverHead != other.defaultLiftOverHead)
                return false;
            if (liftOverHead != other.liftOverHead)
                return false;
            if (defaultLiftOffGround != other.defaultLiftOffGround)
                return false;
            if (liftOffGround != other.liftOffGround)
                return false;
            if (defaultDragOrPush != other.defaultDragOrPush)
                return false;
            if (dragOrPush != other.dragOrPush)
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
            return Equals(other as Encumbrance);
        }

        public static bool operator ==(Encumbrance first, Encumbrance second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if (ReferenceEquals(null, first))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(Encumbrance first, Encumbrance second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + base.GetHashCode();
            hash = (hash * 7) + (!ReferenceEquals(null, lightLoad) ? lightLoad.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, mediumLoad) ? mediumLoad.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, heavyLoad) ? heavyLoad.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, liftOverHead) ? liftOverHead.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, liftOffGround) ? liftOffGround.GetHashCode() : 0);
            hash = (hash * 7) + (!ReferenceEquals(null, dragOrPush) ? dragOrPush.GetHashCode() : 0);
            hash = (hash * 7) + defaultLiftOverHead.GetHashCode();
            hash = (hash * 7) + defaultLiftOffGround.GetHashCode();
            hash = (hash * 7) + defaultDragOrPush.GetHashCode();
            return hash;
        }

        public Encumbrance Fill(Encumbrance source)
        {
            if (source == null)
                return this;
            lightLoad = source.lightLoad.Clone;
            mediumLoad = source.mediumLoad.Clone;
            heavyLoad = source.heavyLoad.Clone;
            liftOverHead = source.liftOverHead.Clone;
            liftOffGround = source.liftOffGround.Clone;
            dragOrPush = source.dragOrPush.Clone;
            defaultLiftOverHead = source.defaultLiftOverHead;
            defaultLiftOffGround = source.defaultLiftOffGround;
            defaultDragOrPush = source.defaultDragOrPush;
            return this;
        }
    }
}
