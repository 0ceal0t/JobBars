using JobBars.Data;

namespace JobBars {
    public enum ItemType {
        Buff,
        Action, // either GCD or OGCD
        GCD,
        OGCD
    }

    public struct Item {
        public uint Id;
        public ItemType Type;

        public Item(ActionIds action) {
            Id = (uint)action;
            Type = ItemType.Action;
        }

        public Item(BuffIds buff) {
            Id = (uint)buff;
            Type = ItemType.Buff;
        }

        public override bool Equals(object obj) {
            return obj is Item overrides && Equals(overrides);
        }

        public bool Equals(Item other) {
            return (Id == other.Id) && ((Type == ItemType.Buff) == (other.Type == ItemType.Buff));
        }

        public static bool operator ==(Item left, Item right) {
            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right) {
            return !(left == right);
        }

        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + (Type == ItemType.Buff).GetHashCode();
            return hash;
        }
    }
}