using System.Collections.Generic;
using System;

[Serializable]
public class AuthTokenData
{
    public string cookie;
    public string socketURL;
    public string nameSpace;
}

[Serializable]
public class MessageData
{
    public string type;
    public Data payload = new();
}

[Serializable]
public class Data
{
    public int betIndex;
}

// InIt Data Classes

[Serializable]
public class Root
{
    public string id { get; set; }
    public GameData gameData { get; set; }
    public UiData uiData { get; set; }
    public Player player { get; set; }
    public JackpotData jackpotData { get; set; }
    public Values values { get; set; }
    public Features features { get; set; }

    // result Data Classes
    public bool success { get; set; }
    public List<List<string>> matrix { get; set; }
    public Payload payload { get; set; }
}

[Serializable]
public class Values
{
    public string miniJackpot { get; set; }
    public string minorJackpot { get; set; }
    public string majorJackpot { get; set; }
    public string grandJackpot { get; set; }
}

[Serializable]
public class GameData
{
    public List<List<int>> lines { get; set; }
    public List<double> bets { get; set; }
    public int totalLines { get; set; }
}

[Serializable]
public class Player
{
    public double balance { get; set; }
}

[Serializable]
public class JackpotData
{
    public Values values { get; set; }
}

[Serializable]
public class BalanceSyncPayload
{
    public double balance;
}

[Serializable]
public class UiData
{
    public Paylines paylines { get; set; }
}

[Serializable]
public class Paylines
{
    public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Symbol
{
    public int id { get; set; }
    public string name { get; set; }
    public List<double> multiplier { get; set; }
    public string description { get; set; }
}

[Serializable]
public class Features
{
    public FreeGamesConfig freeGames { get; set; }
    public GoldenMonopolyConfig goldenMonopoly { get; set; }
    public CheckersBonusConfig checkersBonus { get; set; }
}

[Serializable]
public class FreeGamesConfig
{
    public List<int> reels { get; set; }
    public bool enabled { get; set; }
    public int spinsCount { get; set; }
    public double triggerProbability { get; set; }
}

[Serializable]
public class GoldenMonopolyConfig
{
    public bool enabled { get; set; }
    public int multiplier { get; set; }
}

[Serializable]
public class CheckersBonusConfig
{
    public bool enabled { get; set; }
    public int rollsCount { get; set; }
    public List<int> boardValues { get; set; }
    public Dictionary<string, SpecialCell> specialCells { get; set; }
    public double triggerProbability { get; set; }
}

[Serializable]
public class SpecialCell
{
    public int value { get; set; }
    public string action { get; set; }
}

// Result Data Classes

[Serializable]
public class Payload
{
    public double betAmountPerLine { get; set; }
    public double totalBetAmount { get; set; }
    public double winAmount { get; set; }
    public List<LineWin> lineWins { get; set; }
    public double grandTotalWin { get; set; }
    public double netReturnRatio { get; set; }
    public bool isFreeSpinTriggered { get; set; }
    public bool isFreeSpinActive { get; set; }
    public List<MagicDiceMultiplier> magicDiceMultipliers { get; set; }
    public FreeGames freeGames { get; set; }
    public bool goldenMultiplierApplied { get; set; }
    public CheckersBonus checkersBonus { get; set; }
    public double linesWinAmount { get; set; }
}

[Serializable]
public class LineWin
{
    public int lineIndex { get; set; }
    public List<Position> positions { get; set; }
    public string symbolId { get; set; }
    public string symbolName { get; set; }
    public double payout { get; set; }
    public int matchCount { get; set; }
    public double basePayout { get; set; }
    public double winInCredits { get; set; }
    public double winInCash { get; set; }
    public int matchLength { get; set; }
    public double lineMultiplier { get; set; }
    public bool goldenMultiplierApplied { get; set; }
}

[Serializable]
public class Position
{
    public List<int> position { get; set; }
}

[Serializable]
public class FreeGames
{
    public bool triggered { get; set; }
    public bool active { get; set; }
    public int spinsRemaining { get; set; }
    public int totalSpins { get; set; }
    public double totalWinCash { get; set; }
    public List<int> lockedWildRows { get; set; }
    public List<ScatterTriggerPosition> scatterTriggerPosition { get; set; }
}

[Serializable]
public class MagicDiceMultiplier
{
    public int row { get; set; }
    public int col { get; set; }
    public int multiplier { get; set; }
}

[Serializable]
public class Board
{
    public int multiplier { get; set; }
    public int? addRolls { get; set; }
    public bool? isEnd { get; set; }
    public Vault vault { get; set; }
    public bool? resetPosition { get; set; }
}

[Serializable]
public class CheckersBonus
{
    public bool triggered { get; set; }
    public List<Board> board { get; set; }
    public double winInCash { get; set; }
    public List<Roll> rolls { get; set; }
    public int finalPosition { get; set; }
}

[Serializable]
public class Roll
{
    public int dice1 { get; set; }
    public int dice2 { get; set; }
    public int sum { get; set; }
    public int position { get; set; }
    public double rewardMultiplier { get; set; }
    public bool isEnd { get; set; }
    public double winInCash { get; set; }
    public int? rollsAdded { get; set; }
    public bool? isReset { get; set; }
    public bool? vaultActivated { get; set; }
}

[Serializable]
public class Vault
{
    public int minMultiplier { get; set; }
    public int maxMultiplier { get; set; }
}

[Serializable]
public class ScatterTriggerPosition
{
    public int row { get; set; }
    public int column { get; set; }
}


