namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberLoadout
    {
        // TODO: Validation.
        public string SkinColor { get; set; }

        public int GearHolo { get; set; }
        public int GearFace { get; set; }
        public int GearHead { get; set; }
        public int GearGloves { get; set; }
        public int GearUpperBody { get; set; }
        public int GearLowerBody { get; set; }
        public int GearBoots { get; set; }
        public int WeaponMelee { get; set; }
        public int WeaponPrimary { get; set; }
        public int WeaponSecondary { get; set; }
        public int WeaponTertiary { get; set; }
        public int QuickItem1 { get; set; }
        public int QuickItem2 { get; set; }
        public int QuickItem3 { get; set; }
        public int FunctionalItem1 { get; set; }
        public int FunctionalItem2 { get; set; }
        public int FunctionalItem3 { get; set; }
    }
}
