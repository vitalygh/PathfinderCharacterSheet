using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderCharacterSheet.CharacterSheets.V1
{
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
}
