// Arrowgene.O2Jam.Server/Data/Character.cs
public class Character
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Gems { get; set; } // Renamed from Gem
    public int Cash { get; set; } // Added
    public int Gender { get; set; } // Added
    public int Instrument { get; set; }
    public int Hat { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
    public int Shoes { get; set; }
    public int Glasses { get; set; }
    public int Wing { get; set; }
    public int HairAccessory { get; set; }
    public int SetAccessory { get; set; }
    public int Glove { get; set; }
    public int Necklace { get; set; }
    public int Earring { get; set; }
    public int Pet { get; set; }
    public int Props { get; set; } // Added
    public int CostumeProps { get; set; } // Added
    public int InstrumentProps { get; set; } // Added
    public int PenaltyCount { get; set; } // Added
    public int PenaltyLevel { get; set; } // Added

    // Avatar is likely an old name for Gender, removing to avoid confusion
    // public int Avatar { get; set; } 
}